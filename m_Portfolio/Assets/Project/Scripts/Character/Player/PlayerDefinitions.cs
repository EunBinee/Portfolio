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
public class PlayerInput
{
    //* 플레이어 Input
    public float verticalMovement;   //상하
    public float horizontalMovement; //좌우
    public float mouseY;             //마우스 상하
    public float mouseX;             //마우스 좌우
    public float jumpMovement;       //점프
}
[Serializable]
public class PlayerOption
{
    //* 플레이어 옵션 값

}

[Serializable]
public class PlayerCurState
{
    //* 플레이어의 현재 상태
    public bool isWalking;
    public bool isRunning;
    public bool isSprinting;
    public bool isJumping;
    public bool isStrafing;         //주목중
    public bool isDodge;            //피하기 (ex. 구르기)
    public bool isGround;           //현재 발이 바닥에 닿아있는지
    public bool isForwardBlocked;   //앞에 장애물이 있는지 여부
    public bool isOnSteepSlop;      //앞에 가파른 경사있음.
}

[Serializable]
public class PlayerCurValue
{
    public float moveAmount = 0;     // 움직임. (0 움직이지않음, 1 움직임)
}