using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarManager : MonoBehaviour
{
    //* 몬스터 HP바를 관리하는 스크립트입니다.

    public string HPBar_name = ""; // 몬스터 HP UI 이름
    public Transform HPBar_Parent; // HP바를 모아둘 부모

    //* 일반 몬스터 HP바
    private HPBarUI_Info HPBar_Prefab; // HP Bar 프리펩
    private List<HPBarUI_Info> HPBarInUse; // 사용중인  HP바
    private List<HPBarUI_Info> hpBarPools; // 사용이 끝난 HP바
    private int hpBarPoolsCount = 15; //최대  HP 바

    public Camera m_Camera; //main카메라

    //* HP바 크기 조절
    public float HPbar_minScale = 0.3f;
    public float HPbar_maxScale = 1f;
    public float minDistance = 1.0f; // 크기 조절이 시작되는 최소 거리
    public float maxDistance = 50.0f; // 크기 조절이 완료되는 최대 거리


    private void Awake()
    {
        InitHPBar();
    }

    private void Start()
    {
        if (CanvasManager.instance.monster_HPBarUI == null)
        {

            Debug.LogError("monster_HPBar를 담는 canvas 빈오브젝트 없음");
        }
        HPBar_Parent = CanvasManager.instance.monster_HPBarUI.GetComponent<Transform>();

        m_Camera = GameManager.instance.gameData.cameraObj;
    }

    private void InitHPBar()
    {
        HPBarInUse = new List<HPBarUI_Info>();
        hpBarPools = new List<HPBarUI_Info>();

        if (HPBar_Prefab == null)
        {
            HPBar_Prefab = Resources.Load<HPBarUI_Info>("SystemPrefabs/" + HPBar_name);
        }
    }

    private void FixedUpdate()
    {
        if (HPBarInUse.Count > 0) // 사용중인 HP 바가 있을 경우
        {
            UpdateHPBarPos();
        }
    }

    public void UpdateHPBarPos()
    {
        for (int i = 0; i < HPBarInUse.Count; i++)
        {
            //* HP바의 위치-------------------------------------------------------------------------------
            Vector3 hpPos = Vector3.zero;

            if (HPBarInUse[i].useDifferentPos)
            {
                hpPos = HPBarInUse[i].m_Monster.transform.position + new Vector3(0, HPBarInUse[i].m_HPBarPos.position.y, 0);
            }
            else
            {
                hpPos = HPBarInUse[i].m_HPBarPos.position;
            }
            // 오브젝트와 카메라 간의 거리를 계산
            Vector3 cameraToObj = hpPos - m_Camera.transform.position;

            // 카메라 정면 방향과 오브젝트 간의 각도를 계산
            float angle = Vector3.Angle(m_Camera.transform.forward, cameraToObj);

            if (angle < 90f)
            {
                Vector3 targetScreenPos = m_Camera.WorldToScreenPoint(hpPos);
                if (HPBarInUse[i].isReset)
                {
                    HPBarInUse[i].gameObject.transform.position = Vector3.Lerp(HPBarInUse[i].gameObject.transform.position, targetScreenPos, 0.5f);
                }

                //*------------------------------------------------------------------------------------------------------------//
                //* HP바 Scale조정
                float distance = Vector3.Distance(HPBarInUse[i].m_Monster.transform.position, m_Camera.transform.position);

                // 크기 조절을 위한 보간값 계산
                float t = Mathf.InverseLerp(minDistance, maxDistance, distance);
                t = Mathf.Clamp01(t * 2);

                // HP 바의 크기 조절
                float targetScale = Mathf.Lerp(HPbar_maxScale, HPbar_minScale, t);
                HPBarInUse[i].hpBarRectTransform.localScale = new Vector3(targetScale, targetScale, targetScale);
                //----------------------------------------------------------------------------------------------------------------------//
            }

        }
    }

    //*----------------------------------------------------------------------------//
    //* hp바 오브젝트 풀링//
    //HP바 받기.
    public HPBarUI_Info Get_HPBar()
    {
        HPBarUI_Info curHPBar = null;

        //오브젝트 풀에 
        if (hpBarPools.Count > 0)
        {
            curHPBar = hpBarPools[0];

            HPBarInUse.Add(curHPBar);
            hpBarPools.Remove(curHPBar);
        }
        else
        {
            curHPBar = UnityEngine.Object.Instantiate(HPBar_Prefab);
            HPBarInUse.Add(curHPBar);
        }

        curHPBar.gameObject.transform.SetParent(HPBar_Parent);
        curHPBar.gameObject.SetActive(true);
        return curHPBar;
    }

    //HP바 반납.
    public void Add_HPBarPool(HPBarUI_Info HPBar)
    {
        HPBar.gameObject.SetActive(false);

        if (hpBarPools.Count >= hpBarPoolsCount)
        {
            //만약 풀이 가득 찼다면, 그냥 삭제.
            UnityEngine.Object.Destroy(HPBar.gameObject);
        }
        else
        {
            HPBarInUse.Remove(HPBar);
            hpBarPools.Add(HPBar);
        }
    }
}
