using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public MonsterData monsterData;
    public MonsterPattern monsterPattern;
    public AudioClip[] monsterSoundClips;
    public PlayerController playerController;
    private Transform playerTrans;

    public bool resetHP = false;

    public enum monsterSound
    {
        Death
    }

    private void Awake()
    {

    }
    private void Start()
    {
        Init();
    }
    private void Update()
    {

    }
    //*------------------------------------------------------------------------------------------//
    //* 초기화 //
    private void Init()
    {
        playerController = GameManager.Instance.gameData.GetPlayerController();
        playerTrans = GameManager.Instance.gameData.GetPlayerTransform();
        if (!resetHP)
            ResetHP();
    }

    private void Reset()
    {

    }

    public void ResetHP()
    {
        //* 처음 시작햇을때 HP 
        resetHP = true;
        monsterData.HP = monsterData.MaxHP;
    }

    //*------------------------------------------------------------------------------------------//
    //* 몬스터가 플레이어를 때렸을 때 //
    public virtual void OnHit(float damage = 0, Action action = null)
    {

    }
    //*------------------------------------------------------------------------------------------//
    //* 플레이어에게 공격 당했을 때 //
    public virtual void GetDamage(double damage, Vector3 attackPos, Quaternion atteckRot)//플레이어에게 공격 당함.
    {

    }

    public virtual void Death()
    {

    }

}
