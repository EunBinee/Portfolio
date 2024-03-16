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

    public LayerMask monsterLayer;
    public Monster curTargetMonster;
    List<Monster> nearbyMonsters;

    public bool startComboAttack = false;
    bool firstComboAttackCheck = false; // 첫타 체크


    public void PlayerAttackInit(PlayerController _playerController)
    {
        //* PlayerAttackInit은 PlayerController => Start()에서 실행. 
        playerController = _playerController;
        p_anim = playerController.playerComponents.anim;
        nearbyMonsters = new List<Monster>();
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

                if (!stateInfo.IsName("Locomotion") && !firstComboAttackCheck)
                {
                    firstComboAttackCheck = true;
                }
                else if (stateInfo.IsName("Locomotion") && firstComboAttackCheck)
                {
                    Debug.Log("기본 콤보 공격끝");
                    playerController.playerWeapon_Info.ChangeWeaponState(PlayerWeaponInfo.WeaponState.unusedWeapon);
                    startComboAttack = false;
                    firstComboAttackCheck = false;
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

            //* Target 몬스터 가지고 오기
            curTargetMonster = GetNearestMonster();
        }
        startComboAttack = true;
        p_anim.SetTrigger("Weapon_ComboAttack");
    }

    //* 근처에 있는 몬스터중 가장 가까운 몬스터
    Monster GetNearestMonster()
    {
        Monster nearestMonster = null;
        float nearDistance = 10000;

        Vector3 curPlayerPos = this.gameObject.transform.position;

        NearbyMonsterList();

        if (nearbyMonsters.Count != 0)
        {
            foreach (Monster mon in nearbyMonsters)
            {
                float curMonDistance = Vector3.Distance(curPlayerPos, mon.gameObject.transform.position);
                if (curMonDistance < nearDistance)
                {
                    nearestMonster = mon;
                    nearDistance = curMonDistance;
                }
            }
        }
        else
        {
            //* 가까운 몬스터가 하나도 없는 경우.. null;
            nearestMonster = null;
        }


        return nearestMonster;
    }

    //* 근처 몬스터 리스트
    void NearbyMonsterList()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 3f, monsterLayer); //반지름 10

        nearbyMonsters.Clear();
        foreach (Collider col in colliders)
        {
            nearbyMonsters.Add(col.gameObject.GetComponent<Monster>());
        }
    }
}
