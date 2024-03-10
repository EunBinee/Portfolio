using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    //! 플레이어의 모든 공격을 관리합니다.
    PlayerController playerController;
    public PlayerInput playerInput;
    Animator p_anim;

    public bool startComboAttack = false;
    bool comboAttack = false; // 첫타 체크


    public void PlayerAttackInit(PlayerController _playerController)
    {
        //* PlayerAttackInit은 PlayerController => Start()에서 실행. 
        playerController = _playerController;
        p_anim = playerController.playerComponents.anim;
    }


    void Update()
    {
        playerInput.HandlePlayerAttackInput();
        if (startComboAttack)
        {
            //* 지금 기본 콤보 공격중이라면? => 공격이 끝났는지 체크
            if (p_anim != null)
            {
                AnimatorStateInfo stateInfo = p_anim.GetCurrentAnimatorStateInfo(0);

                // 현재 재생 중인 상태가 블렌드 트리인지 확인
                if (!stateInfo.IsName("Locomotion") && !comboAttack)
                {
                    comboAttack = true;
                }
                else if (stateInfo.IsName("Locomotion") && comboAttack)
                {
                    Debug.Log("기본 콤보 공격끝");
                    playerController.playerWeapon_Info.ChangeWeaponState(PlayerWeaponInfo.WeaponState.unusedWeapon);
                    startComboAttack = false;
                    comboAttack = false;
                }
            }
        }
    }

    public void BasicAttack_Combo()
    {
        if (!startComboAttack)
        {
            //첫공격이면? 무기 세팅
            playerController.playerWeapon_Info.ChangeWeaponState(PlayerWeaponInfo.WeaponState.useWeapon);

        }
        startComboAttack = true;
        p_anim.SetTrigger("Weapon_ComboAttack");
    }
}
