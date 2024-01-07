using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerController playerController;
    private PlayerComponents P_com;
    private PlayerInput P_input;
    private PlayerOption P_option;
    private PlayerCurState P_state;
    private PlayerCurValue P_value;

    public void PlayerMovement_Init()
    {
        //* PlayerController.cs의 Start에서 Init
        PlayerControllerValue();
    }

    private void PlayerControllerValue()
    {
        //* 플레이어 컨트롤러 관련 값 할당
        playerController = GetComponent<PlayerController>();
        P_com = playerController.playerComponents;
        P_input = playerController.playerInput;
        P_option = playerController.playerOption;
        P_state = playerController.playerCurState;
        P_value = playerController.playerCurValue;
    }

    void Update()
    {
        HandleInputs();

    }
    void FixedUpdate()
    {

    }

    private void HandleInputs()
    {
        P_input.mouseX = Input.GetAxis("Mouse X");
        P_input.mouseY = Input.GetAxis("Mouse Y");

        HandleSprint();
        HandleWalkOrRun();

        if (Input.GetKey(KeyCode.W))
        {
            P_input.verticalMovement = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            P_input.verticalMovement = -1;
        }
        else
        {
            P_input.verticalMovement = 0;
        }

        if (Input.GetKey(KeyCode.D))
        {
            P_input.horizontalMovement = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            P_input.horizontalMovement = -1;
        }
        else
        {
            P_input.horizontalMovement = 0;
        }

        P_value.moveAmount = Mathf.Clamp01(Mathf.Abs(P_input.horizontalMovement) + Mathf.Abs(P_input.verticalMovement));
    }

    //* 플레이어 상태 체크
    private void HandleSprint()
    {
        if (!P_state.isGround)
            return;

        if (Input.GetKey(KeyCode.LeftControl) && P_value.moveAmount > 0)
        {
            P_state.isWalking = false;
            P_state.isRunning = true;
            P_state.isSprinting = true;
        }
        else
        {
            P_state.isSprinting = false;
        }
    }

    private void HandleWalkOrRun()
    {
        //* 걷거나 뛰는 지 체크
        if (P_state.isSprinting) //전력으로 뛰는 중이면, Pass
            return;

        if (P_value.moveAmount > 0)
        {
            //움직임.
            if (!P_state.isWalking && !P_state.isRunning)
            {
                P_state.isWalking = false;
                P_state.isRunning = true;
            }

            if (Input.GetKeyUp(KeyCode.V))
            {
                if (!P_state.isWalking || P_state.isRunning)
                {
                    P_state.isWalking = true;
                    P_state.isRunning = false;
                }
                else if (P_state.isWalking || !P_state.isRunning)
                {
                    P_state.isWalking = false;
                    P_state.isRunning = true;
                }
            }
        }
        else
        {
            //* 움직임 없을 경우
            P_state.isWalking = false;
            P_state.isRunning = false;
        }
    }
    private bool HandleJump()
    {
        //점프 안하면 false;
        //하면 true
        //일단 false러

        return false;
    }



}
