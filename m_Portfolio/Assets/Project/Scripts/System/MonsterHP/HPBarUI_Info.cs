using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Concurrent;

public class HPBarUI_Info : MonoBehaviour
{
    public RectTransform hpBarRectTransform;
    public TMP_Text monsterName;
    public Slider m_slider;
    public Monster m_Monster;

    public Transform m_HPBarPos; //몬스터의 HP Bar 위치

    public bool useDifferentPos = false;

    private double monsterMaxHP = 0; //몬스터의 최대 HP

    public bool isReset = false; //리셋된 상태의 HP바인가.?
    Coroutine updateHP_Anim_co = null;



    public void Reset(Monster _monster, double _monsterMaxHP, bool isBoss = false)
    {
        hpBarRectTransform = GetComponent<RectTransform>();
        monsterMaxHP = _monsterMaxHP;
        m_Monster = _monster;

        if (isBoss)
        {
            Debug.LogError("보스입니다");
        }

        if (m_Monster.monsterData.useDifferentPos)
        {
            useDifferentPos = true;
        }
        m_HPBarPos = m_Monster.monsterData.HPBarPos;

        monsterName.text = _monster.monsterData.monsterName;

        resetHP();
        isReset = true;
    }

    private void OnDisable()
    {
        //비활성화 될때마다.
        isReset = false;
    }

    public void resetHP()
    {
        float monsterHP_Value = (float)(m_Monster.monsterData.HP / m_Monster.monsterData.MaxHP);
        m_slider.value = monsterHP_Value;
    }

    public void UpdateHP()
    {
        float monsterHP_Value = (float)(m_Monster.monsterData.HP / m_Monster.monsterData.MaxHP);

        if (monsterHP_Value <= 0)
            monsterHP_Value = 0;

        if (m_slider.value > 0)
        {
            if (updateHP_Anim_co != null)
            {
                StopCoroutine(updateHP_Anim_co);
            }
            updateHP_Anim_co = StartCoroutine(UpdateHPBar_Anim(monsterHP_Value));
        }
    }

    //* HP 깎이는 연출
    IEnumerator UpdateHPBar_Anim(float monsterHP_Value)
    {
        float time = 0;
        while (time < 0.5f)
        {
            time += Time.deltaTime;

            m_slider.value = Mathf.Lerp(m_slider.value, monsterHP_Value, 0.5f);
            if (m_slider.value == monsterHP_Value)
            {
                break;
            }
            yield return null;
        }

        m_slider.value = monsterHP_Value;
    }

}
