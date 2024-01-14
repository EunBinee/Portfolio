using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerMovementInput
{
    public PlayerController playerController;
    public PlayerMovement playerMovement;
    private PlayerComponents P_com;
    private PlayerInput P_input;
    private PlayerOption P_option;
    private PlayerCurState P_state;
    private PlayerCurValue P_value;

    public void PlayerMovementInput_Init(PlayerController _playerController, PlayerMovement _playerMovement)
    {
        playerController = _playerController;
        playerMovement = _playerMovement;
        GetPlayerController();
    }
    public void GetPlayerController()
    {
        P_com = playerController.playerComponents;
        P_input = playerController.playerInput;
        P_option = playerController.playerOption;
        P_state = playerController.playerCurState;
        P_value = playerController.playerCurValue;
    }

    public void HandleInputs()
    {
        P_input.mouseX = Input.GetAxis("Mouse X");
        P_input.mouseY = Input.GetAxis("Mouse Y");

        HandleSprint();     //? 전력 질주 Input 체크
        HandleWalkOrRun();  //? 걷기 뛰기 Input 체크

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

    //* 플레이어 상태 체크 (Input)---------------------------------------------------------------------------------------------------//
    //* - 전력질주
    private void HandleSprint()
    {
        if (P_state.isPerformingAction)
            return;
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
    //* - 뛰기, 걷기
    private void HandleWalkOrRun()
    {
        //* 걷거나 뛰는 지 체크
        if (P_state.isSprinting || P_state.isPerformingAction) //전력으로 뛰는 중이면, Pass
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
                    //뛰기 상태에서 V키  => 걷기로 변경
                    P_state.isWalking = true;
                    P_state.isRunning = false;
                }
                else if (P_state.isWalking || !P_state.isRunning)
                {
                    //걷기 상태에서 V키  => 뛰기로 변경
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
    //* - 점프 (구현 안함.)
    private bool HandleJump()
    {
        //점프 안하면 false;
        //하면 true
        //일단 false러

        return false;
    }
}

