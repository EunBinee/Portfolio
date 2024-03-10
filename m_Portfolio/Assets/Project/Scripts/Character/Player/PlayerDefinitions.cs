using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//! Player 캐릭터의 모든 것
[Serializable]
public class PlayerComponents
{
    //* 플레이어 컴포넌트
    public Rigidbody rigid;
    public Animator anim;
    public CapsuleCollider capsuleCollider;
}
[Serializable]
public class PlayerInputInfo
{
    //* 플레이어 Input
    public float verticalMovement;   //상하
    public float horizontalMovement; //좌우
    public float mouseY;             //마우스 상하
    public float mouseX;             //마우스 좌우
    public float jumpMovement;       //점프
}
//[Serializable]
[Serializable]
public class PlayerOption
{
    //* 플레이어 옵션 값

    [Range(1f, 30f), Tooltip("전력으로 달리는 속도")]
    public float sprintSpeed = 15f;
    [Range(1f, 30f), Tooltip("달리는 속도")]
    public float runSpeed = 10f;
    [Range(1f, 30f), Tooltip("걷는 속도")]
    public float walkSpeed = 5f;
    [Range(1f, 20f), Tooltip("회전속도")]
    public float rotSpeed = 20f;         // 몸 회전 속도

    [Space]
    public float originGravity = -9.81f; //원래 중력값
    public float gravity = 0f;           // 직접 제어하는 중력

    public float jumpGravity = -20f;
    public float jumpHeight = 3;


    [Space]
    [Tooltip("지면으로 체크할 레이어 설정")]
    public LayerMask groundLayerMask = -1;

    [Range(0.1f, 10.0f), Tooltip("지면 감지 거리")]
    public float groundCheckDistance = 2.0f;
    [Range(0.0f, 0.1f), Tooltip("지면 감지 거리_임계값")]
    public float groundCheckThreshold = 0.01f;
    [Range(1f, 70f), Tooltip("등반이 가능한 경사각")]
    public float maxSlopAngle = 50f;

    [Range(0.01f, 0.05f), Tooltip("전방 장애물 감지 거리")]
    public float forwardCheckDistance = 0.1f;
}

[Serializable]
public class PlayerCurState
{
    //* 플레이어의 현재 상태
    public bool isPerformingAction; // 공격 같은 액션 도중인지 체크

    public bool isWalking;
    public bool isRunning;
    public bool isSprinting;
    public bool isJumping;
    public bool isStrafing;         //주목중
    public bool isDodge;            //피하기 (ex. 구르기)
    public bool isFallig;          // 절벽에서 떨어지는 중인지.
    public bool isGround;           //현재 발이 바닥에 닿아있는지
    public bool isForwardBlocked;   //앞에 장애물이 있는지 여부
    public bool isOnSteepSlop;      //앞에 가파른 경사있음. (true =>> maxSlopAngle보다 더 높은 기울기. 가파름. false =>> 올라갈 수 있는 기울기)
}

[Serializable]
public class PlayerCurValue
{
    public float moveAmount = 0;    // 움직임. (0 움직이지않음, 1 움직임)
    public Vector3 moveDirection;   // 플레이어 이동 방향
    public Vector3 jumpDirection;   // 플레이어 점프 방향

    public float castRadius;       // Ground 체크에 쓰일 레이케스트(원형) 반지름
    public Vector3 groundNormal;    // 플레이어 이동 벡터 투영을 위한 바닥 노말 벡터
    public Vector3 groundCross;     // 캐릭터 이동벡터 회전축 (지면의 외적 )
    public float groundDistance;    // 플레이어와 땅의 거리
    public float groundSlopeAngle;  // 현재 바닥의 경사각
    public float forwardSlopeAngle; // 캐릭터가 바라보는 방향의 경사각
}

[Serializable]
public class PlayerCamera
{
    public GameObject playerCamera;
    public GameObject playerCameraPivot;
    public Camera cameraObj;

}