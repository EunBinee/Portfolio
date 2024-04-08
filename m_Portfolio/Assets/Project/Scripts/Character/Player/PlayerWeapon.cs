using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public PlayerController playerController;
    public PlayerAttack playerAttack;

    public void Init()
    {
        playerController = GameManager.instance.gameData.GetPlayerController();
        playerAttack = playerController.playerAttack;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Monster monster = other.gameObject.GetComponent<Monster>();

            if (monster == null)
            {
                monster = playerAttack.GetMonsterScript(other.gameObject);

                if (monster == null)
                {
                    Debug.LogError("감지된 몬스터 null");
                    return;
                }
            }

            Debug.Log($"몬스터 이름 : {monster.gameObject.name}");

        }
    }
}
