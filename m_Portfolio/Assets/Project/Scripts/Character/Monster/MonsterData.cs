using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MonsterData
{
    public enum MonsterType
    {
        None,
        NomalMonster,
        DistantAttackMonster,
        BossMonster
    }

    [Header("몬스터 정보")]
    public int monsterid;
    public string monsterName;
    public string monsterExplanation; //* 보스 몬스터일 경우 필수
    public MonsterType monsterType;


    [Header("몬스터 체력")]
    public Transform HPBarPos;
    public double MaxHP;
    public double HP;


    [Header("몬스터 움직임")]
    public bool movingMonster;

    [Space]
    [Header("몬스터 플레이어 탐지 범위 (반지름)")]
    public float overlapRadius;

    [Header("몬스터 로밍 범위 (x , z)")]
    [Range(5f, 50f), Tooltip("몬스터 로밍 범위 x (가로)")]
    public int roaming_RangeX;
    [Range(5f, 50f), Tooltip("몬스터 로밍 범위 z (세로)")]
    public int roaming_RangeZ;

    [Space]
    [Header("몬스터 정보 보이는 거리")]
    public float canSeeMonsterInfo_Distance = 20;

    [Space]
    public Transform effectTrans;

}