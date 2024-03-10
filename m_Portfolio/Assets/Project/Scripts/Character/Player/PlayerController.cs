using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerAttack playerAttack;
    public PlayerInput playerInput; //플레이어 Input
    public PlayerAnimationController playerAnimController;
    public PlayerWeaponInfo playerWeapon_Info;

    public PlayerComponents playerComponents = new PlayerComponents();
    public PlayerInputInfo playerInput_Info = new PlayerInputInfo();
    public PlayerOption playerOption = new PlayerOption();
    public PlayerCurState playerCurState = new PlayerCurState();
    public PlayerCurValue playerCurValue = new PlayerCurValue();
    public PlayerCamera playerCamera = new PlayerCamera();

    private PlayerComponents P_com => playerComponents;
    private PlayerInputInfo P_input_Info => playerInput_Info;
    private PlayerOption P_option => playerOption;
    private PlayerCurState P_state => playerCurState;
    private PlayerCurValue P_value => playerCurValue;
    private PlayerCamera P_camera => playerCamera;


    void Start()
    {
        Init();

        //* 플레이어의 애니메이션 관리
        playerAnimController = GetComponent<PlayerAnimationController>();
        playerAnimController.AnimInit(this);

        //* 플레이어의 움직임
        playerMovement = GetComponent<PlayerMovement>();
        playerMovement.PlayerMovement_Init();

        //* 플레이어의 공격
        playerAttack = GetComponent<PlayerAttack>();
        playerAttack.PlayerAttackInit(this);

        //* 플레이어 무기에 대한 정보
        playerWeapon_Info.Init(this);

        //*플레이어의 모든 Input을 관리
        playerInput = new PlayerInput();
        playerInput.PlayerInit(this);
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

    void Update()
    {
        playerWeapon_Info.WeaponUpdate(); //무기 스크립트의 Update
    }

    void FixedUpdate()
    {

    }


}
