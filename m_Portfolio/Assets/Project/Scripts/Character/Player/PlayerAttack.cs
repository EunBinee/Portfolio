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
    PlayerWeaponInfo playerWeaponInfo;
    public PlayerInput playerInput;
    Animator p_anim;

    float attackRange = 5f; // 공격 범위
    public LayerMask monsterLayer;
    public Monster curTargetMonster;
    List<Monster> nearbyMonsters;

    public bool playerAttack_ing = false; //* 전체적인 공격 체크 ( 콤보, 스킬 상관없이 플레이어가 공격중인지 체크 )
    public bool startComboAttack = false; //기본 콤보공격 체크
    bool firstComboAttackCheck = false; // 기본 콤보공격 첫타 체크


    public void PlayerAttackInit(PlayerController _playerController)
    {
        //* PlayerAttackInit은 PlayerController => Start()에서 실행. 
        playerController = _playerController;
        playerWeaponInfo = playerController.playerWeapon_Info;
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
                    playerAttack_ing = true;
                    firstComboAttackCheck = true;
                }
                else if (stateInfo.IsName("Locomotion") && firstComboAttackCheck)
                {
                    playerWeaponInfo.ChangeWeaponState(PlayerWeaponInfo.WeaponState.unusedWeapon);
                    playerAttack_ing = false;
                    startComboAttack = false;

                    firstComboAttackCheck = false;

                    curTargetMonster = null; //콤보 공격의 타겟 몬스터도 null;
                }
            }
        }
    }

    public void BasicAttack_Combo()
    {
        if (!startComboAttack)
        {
            //첫공격이면? 무기 세팅
            playerWeaponInfo.ChangeWeaponState(PlayerWeaponInfo.WeaponState.useWeapon);
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
        float nearDistance = 100;

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
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, monsterLayer); //반지름 10

        nearbyMonsters.Clear();
        foreach (Collider col in colliders)
        {
            Monster monster = col.gameObject.GetComponent<Monster>();

            if (monster == null)
                monster = GetMonsterScript(col.gameObject);


            nearbyMonsters.Add(monster);
        }
    }

    //*----------------------------------------------------------------------------------------------------------------//
    //* 자식 오브젝트에서 부모 오브젝트의 몬스터 스크립트를 가지고 오고 싶을 때
    public Monster GetMonsterScript(GameObject childObject)
    {
        Transform parent = childObject.transform.parent;
        Monster monster = null;

        while (parent != null)
        {
            monster = parent.GetComponent<Monster>();
            if (monster != null)
            {
                break;
            }

            childObject = parent.gameObject;
            parent = childObject.transform.parent;
        }

        if (parent == null && monster == null)
        {
            monster = parent.GetComponent<Monster>();
        }

        return monster;
    }
}
