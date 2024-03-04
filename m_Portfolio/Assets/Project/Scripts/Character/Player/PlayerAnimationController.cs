using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Collections.LowLevel.Unsafe;

public class PlayerAnimationController : MonoBehaviour
{

    public PlayerController playerController;

    private PlayerComponents P_com;
    private PlayerInput P_input;
    private PlayerOption P_option;
    private PlayerCurState P_state;
    private PlayerCurValue P_value;
    private PlayerCamera P_camera;

    float upperBodyLayerWeight = 0f;
    public enum CurAnimation
    {
        jumpUp,
        jumpDown,
        jumpUp_inPlace,
        jumpDown_inPlace,
        falling,
        fallingDown,
        drawSword

    }

    public void AnimInit(PlayerController _playerController)
    {
        //* 플레이어 컨트롤러 관련 값 할당
        playerController = _playerController;
        P_com = playerController.playerComponents;
        P_input = playerController.playerInput;
        P_option = playerController.playerOption;
        P_state = playerController.playerCurState;
        P_value = playerController.playerCurValue;
        P_camera = playerController.playerCamera;
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
            case CurAnimation.falling:
                P_com.anim.SetTrigger("isFalling");
                break;
            case CurAnimation.fallingDown:
                P_com.anim.SetTrigger("isFallingDown");
                break;
            case CurAnimation.drawSword:
                StartCoroutine(DrawSword_co());
                break;
            default:
                break;
        }
    }

    //* 움직임 애니메이션-------------------------------------------------------------------------------------------------------------//
    public void PlayerMovement_Animation()
    {
        float horizontal = 0;
        float vertical = 0;

        #region  Horizontal
        if (P_input.horizontalMovement > 0 && P_input.horizontalMovement <= 0.5f)
        {
            //0보다 큰데 0.5보다 같거나 작은 경우
            horizontal = 0.5f;
        }
        else if (P_input.horizontalMovement > 0.5f)
        {
            //0.5보다 큰경우
            horizontal = 1;
        }
        else if (P_input.horizontalMovement < 0 && P_input.horizontalMovement >= -0.5f)
        {
            //0보다 작은데 -0.5보다 같거나 큰 경우
            horizontal = -0.5f;
        }
        else if (P_input.horizontalMovement < -0.5f)
        {
            //-0.5보다 작은 경우
            horizontal = -1;
        }
        else
        {
            //아무것도 누르지 않은 경우
            horizontal = 0;
        }
        #endregion

        #region Vertical
        if (P_input.verticalMovement > 0 && P_input.verticalMovement <= 0.5f)
        {
            //0보다 큰데 0.5보다 같거나 작은 경우
            vertical = 0.5f;
        }
        else if (P_input.verticalMovement > 0.5f)
        {
            //0.5보다 큰경우
            vertical = 1;
        }
        else if (P_input.verticalMovement < 0 && P_input.verticalMovement >= -0.5f)
        {
            //0보다 작은데 -0.5보다 같거나 큰 경우
            vertical = -0.5f;
        }
        else if (P_input.verticalMovement < -0.5f)
        {
            //-0.5보다 작은 경우
            vertical = -1;
        }
        else
        {
            //아무것도 누르지 않은 경우
            vertical = 0;
        }
        #endregion

        if (P_state.isSprinting) // *전력질주
        {
            P_state.isStrafing = false; //뛸때는 주목 해제
            P_com.anim.SetFloat("Horizontal", 0, 0f, Time.deltaTime);
            P_com.anim.SetFloat("Vertical", 2, 0f, Time.deltaTime);
        }
        else
        {
            if (P_state.isStrafing) //* 주목 (카메라 forward)
            {
                if (P_state.isWalking)
                {
                    P_com.anim.SetFloat("Horizontal", horizontal / 2, 0.2f, Time.deltaTime);
                    P_com.anim.SetFloat("Vertical", vertical / 2, 0.2f, Time.deltaTime);
                }
                else if (P_state.isRunning)
                {
                    P_com.anim.SetFloat("Horizontal", horizontal, 0.2f, Time.deltaTime);
                    P_com.anim.SetFloat("Vertical", vertical, 0.2f, Time.deltaTime);
                }
            }
            else
            {
                if (P_state.isWalking)
                {
                    P_com.anim.SetFloat("Vertical", P_value.moveAmount / 2, 0.2f, Time.deltaTime);   //상
                    P_com.anim.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);
                }
                else if (P_state.isRunning)
                {
                    P_com.anim.SetFloat("Vertical", P_value.moveAmount, 0.2f, Time.deltaTime);   //상
                    P_com.anim.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime);          //하
                }
            }
            if (P_value.moveAmount == 0) // * 움직임이 없을경우
            {
                P_com.anim.SetFloat("Vertical", 0, 0.2f, Time.deltaTime);   //상
                P_com.anim.SetFloat("Horizontal", 0, 0.2f, Time.deltaTime); //하
            }
        }
    }

    //*----------------------------------------------------------------------------------------------------------------------------------//
    //* 총 뽑기
    IEnumerator DrawSword_co()
    {
        upperBodyLayerWeight = 1;
        P_com.anim.SetLayerWeight(1, upperBodyLayerWeight);
        P_com.anim.SetBool("isEquipGun", true);
        while (true)
        {
            if (P_com.anim.GetCurrentAnimatorStateInfo(1).IsName("Gun_equip") && P_com.anim.GetCurrentAnimatorStateInfo(1).normalizedTime > 0.7f)
            {
                if (upperBodyLayerWeight >= 0)
                {
                    upperBodyLayerWeight -= Time.deltaTime;
                }
                else
                {
                    break;
                }
                P_com.anim.SetLayerWeight(1, upperBodyLayerWeight);
            }
            yield return null;
        }
        P_com.anim.SetBool("isEquipGun", false);
        P_com.anim.SetLayerWeight(1, 0);
        yield return null;
    }
    //* 검 넣기

    //*----------------------------------------------------------------------------------------------------------------------------------//
}
