using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public PlayerController playerController;
    public PlayerAttack playerAttack;
    LayerMask monsterLayer;

    public Transform rayPoint;
    List<Monster> curAttackMonster;

    public void Init()
    {
        playerController = GameManager.instance.gameData.GetPlayerController();
        playerAttack = playerController.playerAttack;
        monsterLayer = GameManager.instance.gameData.monsterLayer;
        curAttackMonster = new List<Monster>();
    }

    public void Update()
    {
        if (playerAttack.startComboBasicAttack)
        {
            ComboBasicAttack_RayCheck();
        }
    }

    public void ResetAttackMonsterList()
    {
        curAttackMonster.Clear();
    }

    public void ComboBasicAttack_RayCheck()
    {
        Ray weaponRay = new Ray(rayPoint.position, rayPoint.forward);
        Debug.DrawRay(rayPoint.position, rayPoint.forward * 3f, Color.red);
        RaycastHit hitInfo;
        if (Physics.Raycast(weaponRay, out hitInfo, 3f, monsterLayer))
        {
            // 충돌한 물체가 몬스터인지 확인
            if (hitInfo.collider.CompareTag("Monster"))
            {
                // 몬스터와 충돌한 경우
                if (!curAttackMonster.Contains(hitInfo.collider.gameObject.GetComponent<Monster>()))
                {
                    curAttackMonster.Add(hitInfo.collider.gameObject.GetComponent<Monster>());
                    Debug.Log($" monster.name  :  {hitInfo.collider.gameObject.GetComponent<Monster>().name}");
                }
            }
        }
    }
}
