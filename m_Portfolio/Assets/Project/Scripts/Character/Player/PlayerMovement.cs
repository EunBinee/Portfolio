using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerController playerController;
    public PlayerMovementInput playerMovementInput;
    private PlayerComponents P_com;
    private PlayerInput P_input;
    private PlayerOption P_option;
    private PlayerCurState P_state;
    private PlayerCurValue P_value;


    public void PlayerMovement_Init()
    {
        //* PlayerController.cs의 Start에서 Init
        PlayerControllerValue();
        playerMovementInput = new PlayerMovementInput();
        playerMovementInput.PlayerMovementInput_Init(playerController, this);

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
        playerMovementInput.HandleInputs();
        HandleAllPlayerLocomotion();
    }
    void FixedUpdate()
    {

    }

    //* 플레이어 움직임---------------------------------------------------------------------------------------------------------------//
    private void HandleAllPlayerLocomotion()
    {

    }


}
