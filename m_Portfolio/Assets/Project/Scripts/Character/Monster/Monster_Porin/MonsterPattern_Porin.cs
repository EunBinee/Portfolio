using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterPattern_Porin : MonsterPattern
{
    Collider collider;
    public override void Init()
    {
        m_monster = GetComponent<Monster>();
        m_monster.monsterPattern = this;
        m_animator = GetComponent<Animator>();
        rigid = m_monster.monsterData.rigid;
        collider = m_monster.monsterData.collider;

        playerController = GameManager.instance.gameData.GetPlayerController();
        playerMovement = GameManager.instance.gameData.GetPlayerMovement();
        playerTrans = GameManager.Instance.gameData.GetPlayerTransform();

        //* 네비 메쉬
        if (m_monster.monsterData.movingMonster)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
        }

        playerlayerMask = 1 << playerLayerId; //플레이어 레이어

        //* 처음 상태는 Roaming
        ChangeMonsterState(MonsterState.Roaming);
        originPosition = transform.position;

        overlapRadius = m_monster.monsterData.overlapRadius; //플레이어 감지 범위.
        roaming_RangeX = m_monster.monsterData.roaming_RangeX; //로밍 범위 x;
        roaming_RangeZ = m_monster.monsterData.roaming_RangeZ; //로밍 범위 y;
        CheckRoam_Range();


        playerHide = false;
    }



}
