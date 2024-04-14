using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.TerrainTools;
using System.IO.Compression;

[Serializable]
public class PlayerWeaponInfo
{
    //! 플레이어의 무기를 관리하는 스크립트입니다.
    PlayerController playerController;

    //! 현재 무기 1. 검
    public Transform p_Chest;
    public enum PlayerWeapons
    {
        None,
        greatSword //대검
    }
    public PlayerWeapons curPlayerWeapon; //현재 플레이어 무기

    //* 무기 상태는 3가지 (1. 사용중,  2. 사용안하지만 눈에 보이도록 보관중,  3. 눈에 안보이게 사라진 상태)
    public enum WeaponState
    {
        useWeapon, //무기 사용O ** 공격할때만
        unusedWeapon,//무기 사용X 눈에 보이게 보관중
        invisibleWeapon // 무기 사용X 눈에 안보이게 보관중
    }
    public WeaponState curWeaponState;
    //* 대검----------------------------------------------------//
    public Transform greatSwordPos_unused;//사용안하고 보관중인 대검의 부모 POS
    public GameObject greatSword_unused; //사용안하고 보관중인 대검
    public GameObject greatSword_used;   //사용중인 대검
    PlayerWeapon greatSword_PlayerWeapon; //* 대검의 충돌 체크를 하는 스크립트 
    //----------------------------------------------------------//

    public void Init(PlayerController _playerController)
    {
        //처음 플레이어 기본 무기는 대검
        playerController = _playerController;
        greatSword_PlayerWeapon = greatSword_used.GetComponent<PlayerWeapon>();
        greatSword_PlayerWeapon.Init();

        ChangePlayerWeapon(PlayerWeapons.greatSword);
        ChangeWeaponState(WeaponState.unusedWeapon);
        SetCurWeapon_AnimParameters(PlayerWeapons.greatSword);

    }

    public void WeaponUpdate()
    {
        //PlayerController 의 Update에서 update
        if (curWeaponState == WeaponState.unusedWeapon)
        {
            //* 눈에 보이도록 무기를 보관중//
            unusedW_RotationUpdate();
        }
    }
    //* 현재 무기의 playerWeapon.cs가져오기
    public PlayerWeapon GetCurPlayerWeapon()
    {
        switch (curPlayerWeapon)
        {
            case PlayerWeapons.None:
                return null;
            case PlayerWeapons.greatSword:
                return greatSword_PlayerWeapon;
            default:
                return null;
        }
    }

    //* 무기 바꾸기
    public void ChangePlayerWeapon(PlayerWeapons weapon)
    {
        if (curPlayerWeapon != weapon)
        {
            curPlayerWeapon = weapon;
            SetCurWeapon_AnimParameters(weapon);
        }
    }

    //*무기 변경할때 지금 무기를 알려주는 애니메이션 파라미터 변경
    public void SetCurWeapon_AnimParameters(PlayerWeapons weapon)
    {

        switch (weapon)
        {
            case PlayerWeapons.greatSword:
                playerController.playerComponents.anim.SetBool("isGreatSword", true);
                //* 다른 무기도 생기면 여기에서 false; (아래 주석 참고)
                //playerController.playerComponents.anim.SetBool("isSword", false);
                break;
            /*
            case PlayerWeapons.Sword:
                playerController.playerComponents.anim.SetBool("isGreatSword", false);
                playerController.playerComponents.anim.SetBool("isSword", true);
                break;
            */
            default:
                playerController.playerComponents.anim.SetBool("isGreatSword", false);
                break;
        }
    }


    public void ChangeWeaponState(WeaponState weaponState)
    {
        if (curWeaponState != weaponState)
        {
            curWeaponState = weaponState;

            switch (curWeaponState)
            {
                case WeaponState.useWeapon:
                    greatSword_used.SetActive(true);
                    greatSword_unused.SetActive(false);
                    break;
                case WeaponState.unusedWeapon:
                    greatSword_used.SetActive(false);
                    greatSword_unused.SetActive(true);
                    break;
                case WeaponState.invisibleWeapon:
                    greatSword_used.SetActive(false);
                    greatSword_unused.SetActive(false);
                    break;
            }

        }
    }


    //* 사용안하는 무기의 rot 값
    public void unusedW_RotationUpdate()
    {
        Vector3 w_Euler = new Vector3(greatSwordPos_unused.rotation.eulerAngles.x, greatSwordPos_unused.rotation.eulerAngles.y, p_Chest.rotation.eulerAngles.z);

        greatSwordPos_unused.rotation = Quaternion.Lerp(greatSwordPos_unused.rotation, Quaternion.Euler(w_Euler), 0.5f);
    }



}
