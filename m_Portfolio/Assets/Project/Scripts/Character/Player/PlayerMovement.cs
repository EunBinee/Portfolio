using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    //! 플레이어의 움직임을 구현하는 스크립트입니다.
    public PlayerController playerController;
    public PlayerMovementInput playerMovementInput; //플레이어 움직임 관련 Input
    private PlayerComponents P_com;
    private PlayerInput P_input;
    private PlayerOption P_option;
    private PlayerCurState P_state;
    private PlayerCurValue P_value;
    private PlayerCamera P_camera;

    //캡슐 가운데 가장 위쪽
    private Vector3 CapsuleTopCenterPoint
    => new Vector3(transform.position.x, transform.position.y + P_com.capsuleCollider.height - P_com.capsuleCollider.radius, transform.position.z);
    //캡슐 가운데 가장 아래쪽
    private Vector3 CapsuleBottomCenterPoint
    => new Vector3(transform.position.x, transform.position.y + P_com.capsuleCollider.radius, transform.position.z);


    public void PlayerMovement_Init()
    {
        //* PlayerController.cs의 Start에서 Init
        PlayerControllerValue();
        playerMovementInput = new PlayerMovementInput();
        playerMovementInput.PlayerMovementInput_Init(playerController, this);

        P_value.castRadius = P_com.capsuleCollider.radius * 0.9f;
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
        P_camera = playerController.playerCamera;
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
        CheckedForward();
        HandleGroundCheck();            // 바닥체크
        HandlePlayerRotation();         // 플레이어 회전
        HandlePlayerMovement();         // 플레이어 움직임
    }

    //* 플레이어 바닥 체크
    private void HandleGroundCheck()
    {
        //캐릭터와 지면사이의 높이
        P_value.groundDistance = float.MaxValue; //float의 최대값을 넣어준다.
        P_value.groundNormal = Vector3.up;      // 현재 바닥의 노멀 값. 
        P_value.groundSlopeAngle = 0f;          // 현재 바닥의 경사면.
                                                // P_value.forwardSlopeAngle = 0f;         // 현재 플레이어가 이동하는(바라보는) 방향의 바닥의 경사면.

        bool cast = Physics.SphereCast(CapsuleBottomCenterPoint, P_value.castRadius, Vector3.down,
        out var hit, P_option.groundCheckDistance, P_option.groundLayerMask, QueryTriggerInteraction.Ignore);
        if (cast)
        {
            P_value.groundNormal = hit.normal; //현재 지면의 노멀값
            P_value.groundSlopeAngle = Vector3.Angle(P_value.groundNormal, Vector3.up); // 현재 지면의 경사각(기울기)
            //P_value.forwardSlopeAngle = Vector3.Angle(P_value.groundNormal, P_value.moveDirection) - 90f;  //캐릭터가 바라보는 방향의 경사각

            // 상태값 조정 //가파른 경사 있는지 체크
            P_state.isOnSteepSlop = P_value.groundSlopeAngle >= P_option.maxSlopAngle;
            //!=>
            P_value.groundDistance = hit.distance;// Mathf.Max((hit.distance - _capsuleRadiusDiff - P_option.groundCheckThreshold), -10f);
            P_state.isGround = (P_value.groundDistance <= 0.03f) && !P_state.isOnSteepSlop;
        }

        //경사면의 회전축벡터 => 플레이어가 경사면을 따라 움직일수있도록 월드 이동 벡터를 회전
        P_value.groundCross = Vector3.Cross(P_value.groundNormal, Vector3.up);
    }

    //* 전방체크
    public void CheckedForward()
    {
        //캐릭터가 이동하는 방향으로 막힘 길이 있는가?
        // 함수 파라미터 : Capsule의 시작점, Capsule의 끝점,
        // Capsule의 크기(x, z 중 가장 큰 값이 크기가 됨), Ray의 방향,
        // RaycastHit 결과, Capsule의 회전값, CapsuleCast를 진행할 거리
        /*bool cast = Physics.SphereCast(transform.position, 5f, transform.forward,
        out var hit, Mathf.Infinity, 0);*/

        bool cast = Physics.CapsuleCast(CapsuleBottomCenterPoint, CapsuleTopCenterPoint,
        P_value.castRadius, P_value.moveDirection + Vector3.down * 0.25f,
        out var hit, P_option.forwardCheckDistance, -1, QueryTriggerInteraction.Ignore);
        // QueryTriggerInteraction.Ignore 란? 트리거콜라이더의 충돌은 무시한다는 뜻

        float hitDistance = hit.distance;
        P_state.isForwardBlocked = false;
        P_value.forwardSlopeAngle = 0;

        if (cast)
        {
            P_state.isForwardBlocked = true;

            P_value.forwardSlopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            P_state.isForwardBlocked = P_value.forwardSlopeAngle >= P_option.maxSlopAngle;
            if (P_state.isForwardBlocked)
                Debug.Log("앞에 장애물있음!" + P_value.forwardSlopeAngle + "도");

        }
    }

    //* 플레이어 회전
    private void HandlePlayerRotation()
    {
        if (P_state.isStrafing)
        {
            // 주목중일 경우, 카메라 forward를 바라봄
            Vector3 rotationDirection = P_value.moveDirection;
            if (rotationDirection != Vector3.zero)
            {
                rotationDirection = P_camera.cameraObj.transform.forward;
                rotationDirection.y = 0;
            }
            rotationDirection.Normalize();
            Quaternion turnRot = Quaternion.LookRotation(rotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, turnRot, P_option.rotSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }
        else
        {
            Vector3 rotationDirection = Vector3.zero;
            rotationDirection = P_camera.cameraObj.transform.forward * P_input.verticalMovement;
            rotationDirection = rotationDirection + P_camera.cameraObj.transform.right * P_input.horizontalMovement;
            rotationDirection.Normalize();
            rotationDirection.y = 0;
            if (rotationDirection == Vector3.zero)
            {
                //방향 전환이 없다면, 그냥 캐릭터의 원래 방향
                rotationDirection = transform.forward;
            }

            Quaternion turnRot = Quaternion.LookRotation(rotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, turnRot, P_option.rotSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }
    }

    //* 플레이어 움직임
    private void HandlePlayerMovement()
    {
        /*
        if(!P_state.isGround)
            return;
        */

        //* 카메라 기준의 움직임
        P_value.moveDirection = P_camera.cameraObj.transform.forward * P_input.verticalMovement;
        P_value.moveDirection = P_value.moveDirection + P_camera.cameraObj.transform.right * P_input.horizontalMovement;
        P_value.moveDirection.Normalize();


        if ((P_state.isSprinting || P_state.isRunning) || P_state.isWalking)
        {
            P_value.moveDirection.y = 0;
            if (P_state.isSprinting)
            {
                P_value.moveDirection = P_value.moveDirection * P_option.sprintSpeed;
            }
            else if (P_state.isRunning)
            {
                P_value.moveDirection = P_value.moveDirection * P_option.runSpeed;
            }
            else if (P_state.isWalking)
            {
                P_value.moveDirection = P_value.moveDirection * P_option.walkSpeed;
            }

            Vector3 p_velocity = Vector3.ProjectOnPlane(P_value.moveDirection, P_value.groundNormal);
            p_velocity = p_velocity + Vector3.up * P_option.gravity;
            P_com.rigid.velocity = p_velocity;
        }

    }
}
