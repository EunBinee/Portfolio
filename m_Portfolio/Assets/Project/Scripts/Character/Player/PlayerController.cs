using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerComponents playerComponents = new PlayerComponents();
    public PlayerInput playerInput = new PlayerInput();
    public PlayerOption playerOption = new PlayerOption();
    public PlayerCurState playerCurState = new PlayerCurState();
    public PlayerCurValue playerCurValue = new PlayerCurValue();
    public PlayerCamera playerCamera = new PlayerCamera();

    private PlayerComponents P_com => playerComponents;
    private PlayerInput P_input => playerInput;
    private PlayerOption P_option => playerOption;
    private PlayerCurState P_state => playerCurState;
    private PlayerCurValue P_value => playerCurValue;
    private PlayerCamera P_camera => playerCamera;
    public enum CurAnimation
    {
        jumpUp,
        jumpDown,
        jumpUp_inPlace,
        jumpDown_inPlace
    }

    void Start()
    {
        Init();

        playerMovement = GetComponent<PlayerMovement>();
        playerMovement.PlayerMovement_Init();
    }

    private void Init()
    {
        Set_PlayerComponent();
        Set_PlayerCamera();
    }

    private void Set_PlayerComponent()
    {
        P_com.anim = GetComponent<Animator>();
        P_com.rigid = GetComponent<Rigidbody>();
        P_com.capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Set_PlayerCamera()
    {
        P_camera.playerCamera = GameManager.Instance.gameData.playerCamera;
        P_camera.playerCameraPivot = GameManager.Instance.gameData.playerCameraPivot;
        P_camera.cameraObj = GameManager.Instance.gameData.cameraObj;
    }

    public void PlayAnimation(CurAnimation curAnimation)
    {
        switch (curAnimation)
        {
            case CurAnimation.jumpUp:
                P_com.anim.SetTrigger("isJumpingUp");
                break;
            case CurAnimation.jumpDown:
                P_com.anim.SetTrigger("isJumpingDown");
                break;
            case CurAnimation.jumpUp_inPlace:
                P_com.anim.SetTrigger("isJumpingUp_inPlace");
                break;
            case CurAnimation.jumpDown_inPlace:
                P_com.anim.SetTrigger("isJumpingDown_inPlace");
                break;
            default:
                break;
        }
    }
    void Update()
    {

    }

    void FixedUpdate()
    {

    }

}
