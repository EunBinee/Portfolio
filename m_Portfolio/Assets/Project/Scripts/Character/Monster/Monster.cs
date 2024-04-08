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

    private HPBarUI_Info m_HPBar; //몬스터의 hp바

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

    //*------------------------------------------------------------------------------------------//
    //* HP바 //
    public void GetHPBar() //* hp바 받아와서 사용
    {
        if (monsterData.monsterType == MonsterData.MonsterType.BossMonster)
        {
            //보스전
            //m_HPBar = UIManager.instance.hpBarManager.Get_BossHPBar();
            //m_HPBar.Reset(this, monsterData.MaxHP, true);
        }
        else
        {
            m_HPBar = UIManager.instance.hpBarManager.Get_HPBar();
            m_HPBar.Reset(this, monsterData.MaxHP);
        }
    }

    public void SetActive_HPBar() //* hp바 안보이게.
    {
        if (m_HPBar.gameObject.activeSelf == true)
            m_HPBar.gameObject.SetActive(false);
        else
            m_HPBar.gameObject.SetActive(true);
    }

    public void RetrunHPBar() //* hp바 반납. 
    {
        if (monsterData.monsterType == MonsterData.MonsterType.BossMonster)
        {
            //보스전
            //if (m_HPBar != null)
            //{
            //    UIManager.instance.hpBarManager.Return_BossHPBar();
            //    m_HPBar = null;
            //}
        }
        else
        {
            if (m_HPBar != null)
            {
                UIManager.instance.hpBarManager.Add_HPBarPool(m_HPBar);
                m_HPBar = null;
            }
        }
    }


    public bool HPBar_CheckNull() //*hp바 없으면 false , 있으면 true
    {
        if (m_HPBar != null)
            return true;
        return false;
    }
}
