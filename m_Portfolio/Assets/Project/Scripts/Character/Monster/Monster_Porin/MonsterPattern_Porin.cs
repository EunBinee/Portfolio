using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterPattern_Porin : MonsterPattern
{
    private Collider m_collider;

    private bool isGround; //* 현재 몬스터가 바닥에 있는지 체크
    private Vector3 m_Vector; //* 현재 몬스터의 x와 z은 최상위 부모의 것, y값은 m_collider의 y를 사용.
    Coroutine roamMonster_co = null;
    Coroutine discoveryMonster_co = null;

    public override void Init()
    {
        m_monster = GetComponent<Monster>();
        m_monster.monsterPattern = this;
        m_animator = GetComponent<Animator>();
        rigid = m_monster.monsterData.rigid;
        m_collider = m_monster.monsterData.collider;

        playerController = GameManager.instance.gameData.GetPlayerController();
        playerMovement = GameManager.instance.gameData.GetPlayerMovement();
        playerTrans = GameManager.Instance.gameData.GetPlayerTransform();
        playerTargetPos = GameManager.Instance.gameData.playertargetPos;

        //* 네비 메쉬
        if (m_monster.monsterData.movingMonster)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
        }

        playerlayerMask = 1 << playerLayerId; //플레이어 레이어

        //* 처음 상태는 Roaming
        ChangeMonsterState(MonsterState.Roaming);
        SetAnimation(MonsterAnimation.Idle);
        originPosition = transform.position;

        overlapRadius = m_monster.monsterData.overlapRadius; //플레이어 감지 범위.
        findDistance_BehindPlayer = m_monster.monsterData.findDistance_BehindPlayer; //뒤에 있는플레이어 감지 범위.

        roaming_RangeX = m_monster.monsterData.roaming_RangeX; //로밍 범위 x;
        roaming_RangeZ = m_monster.monsterData.roaming_RangeZ; //로밍 범위 y;
        CheckRoam_Range();

        playerHide = false;

        m_Vector = new Vector3(transform.position.x, m_collider.gameObject.transform.position.y, transform.position.z);

        if (!m_monster.HPBar_CheckNull())
        {
            m_monster.GetHPBar();
        }
    }

    public override void useUpdate()
    {
        base.useUpdate();
        PorinGroudCheck(); //* 현재 바닥에 닿아있는지 체크.
    }
    //*-------------------------------------------------------------------------------------------------------------------------------------------------//

    public override void Monster_Pattern()
    {
        //* 패턴 별로 update 에서 계속 검사해야하는 것들
        switch (curMonsterState)
        {
            case MonsterState.Roaming:
                Roam_Monster();
                CheckPlayerCollider();
                break;
            case MonsterState.Discovery:
                Discovery_Player();
                break;
            case MonsterState.Tracing:
                Tracing_Movement();
                break;
            case MonsterState.GoingBack:
                break;
            default:
                break;
        }
    }

    public override void SetAnimation(MonsterAnimation m_anim)
    {
        switch (m_anim)
        {
            case MonsterAnimation.Idle:
                m_animator.SetBool("m_Move", false);
                m_animator.SetBool("m_Idle", true);
                break;
            case MonsterAnimation.Move:
                m_animator.SetBool("m_Move", true);
                m_animator.SetBool("m_Idle", false);
                break;
            case MonsterAnimation.GetDamage:
                if (isGround)
                    m_animator.SetTrigger("m_GetDamage_isGround");
                else
                    m_animator.SetTrigger("m_GetDamage");
                break;
            case MonsterAnimation.Death:
                m_animator.SetBool("m_Move", false);
                m_animator.SetBool("m_Idle", false);
                m_animator.SetTrigger("m_Death");
                break;
            default:
                break;
        }
    }

    //*------------------------------------------------------------------------------------------------------------------------------------------------------//

    #region 로밍 
    public override void Roam_Monster()
    {
        if (!isRoaming)
        {
            Debug.Log($"로밍");
            isRoaming = true;

            if (roamMonster_co != null)
                StopCoroutine(roamMonster_co);
            roamMonster_co = StartCoroutine(RoamMonster_co());
        }
    }

    IEnumerator RoamMonster_co()
    {
        bool monsterRoaming = false;
        Vector3 randomPos = Vector3.zero; // 몬스터 랜덤 포스

        bool dontMove = false; // 몬스터가 갈 수 있는 랜덤 포스가 없는 경우.=> true

        SetMove_AI(false);

        while (curMonsterState == MonsterState.Roaming)
        {
            if (!monsterRoaming)
            {
                SetAnimation(MonsterAnimation.Idle);

                float roamTime = UnityEngine.Random.Range(1, 3);

                yield return new WaitForSeconds(roamTime);

                if ((isFinding == false) && (curMonsterState != MonsterState.Death))
                {
                    float distance = 0;
                    bool checkObstacle = false; //장애물 체크
                    float time = 0;
                    while (true)
                    {
                        dontMove = false;

                        time += Time.deltaTime;
                        randomPos = GetRandom_RoamingPos();
                        NavMeshHit hit;

                        if (NavMesh.SamplePosition(randomPos, out hit, 200f, NavMesh.AllAreas))
                        {
                            if (hit.position != randomPos)
                                randomPos = hit.position;
                            mRoaming_randomPos = randomPos;

                            distance = Vector3.Distance(transform.position, randomPos);
                            checkObstacle = CheckObstacleCollider(randomPos);

                            if (distance > 3f && checkObstacle)
                            {
                                //너무 가깝지도 않고, 장애물도 없다면 break;
                                break;
                            }
                        }

                        if (time > 2)
                        {
                            //시간이 2초가 지나도록 랜덤 포스를 못찾으면.? break;
                            dontMove = true;
                            Debug.LogError("2초동안 몬스터가 갈 수 있는 곳을 찾지 못했습니다.");
                            break;
                        }

                        yield return null;
                    }

                    if (!dontMove)
                    {
                        SetMove_AI(true);

                        navMeshAgent.SetDestination(randomPos);
                        SetAnimation(MonsterAnimation.Move);

                        monsterRoaming = true;
                    }

                }
            }
            else if (monsterRoaming)
            {
                if (Vector3.Distance(transform.position, randomPos) < 0.5f)
                {
                    monsterRoaming = false;
                    SetMove_AI(false);
                }
                yield return null;
            }
        }

        roamMonster_co = null;
    }


    //* 로밍중에 플레이어 감지
    public override void CheckPlayerCollider()
    {
        if (curMonsterState != MonsterState.Death)
        {

            Collider[] playerColliders = Physics.OverlapSphere(transform.position, overlapRadius, playerlayerMask);

            Vector3 curDirection = GetDirection(playerTargetPos.position, transform.position);
            playerHide = HidePlayer(transform.position, curDirection.normalized);

            if (0 < playerColliders.Length)
            {
                if (!playerHide) //*플레이어가 안숨었을 경우에만..
                {
                    //몬스터의 범위에 들어옴
                    //로밍 코루틴 제거
                    if (isRoaming)
                    {
                        bool inFrontOf_Player = PlayerLocationCheck_BackForth();
                        bool findPlayer = false;
                        if (!inFrontOf_Player)
                        {
                            //* 플레이어가 몬스터 뒤에 있음.
                            float distance = Vector3.Distance(transform.position, playerTrans.position);
                            if (distance < findDistance_BehindPlayer)
                            {
                                Debug.Log($"monster의 뒤에 있지만 발견 현재 거리{distance}");

                                //플레이어가 몬스터 뒤에 있지만 일정 거리 가까워졌을때.
                                findPlayer = true; //* 발견
                            }
                        }
                        else
                            findPlayer = true;

                        if (findPlayer) //* 플레이어를 발견했을 경우.
                        {
                            Debug.Log($">>>> monster가 플레이어를 발견");

                            if (roamMonster_co != null)
                                StopCoroutine(roamMonster_co);
                            isRoaming = false;

                            ChangeMonsterState(MonsterState.Discovery); // => 발견
                        }
                    }

                    if (isFinding || isGoingBack)
                    {
                        //집돌아가는 도중이면 다시 추적 또는 찾은 후라면
                        Debug.Log($"monster의 앞에 여전히 플레이어 있음");
                        ChangeMonsterState(MonsterState.Tracing);
                        isFinding = false;
                        isGoingBack = false;
                    }
                }
                else
                {
                    //* 숨어있을 경우

                    if (isFinding) //* State : Discorvery
                    {
                        Debug.Log($"플레이어 숨음. >> 다시 로밍");
                        isFinding = false;
                        ChangeMonsterState(MonsterState.Roaming);

                    }
                }
            }
            else
            {
                if (isFinding) //* State : Discorvery
                {
                    //플레이어가 나갔을 경우
                    isFinding = false;
                    ChangeMonsterState(MonsterState.GoingBack);

                }
            }
        }
    }

    #endregion


    //*------------------------------------------------------------------------------------------------------------------------------------------------------//
    #region 발견

    public override void Discovery_Player()
    {
        if (!isFinding)
        {
            Debug.Log("monsterState Discovery");

            isFinding = true;
            if (discoveryMonster_co != null)
                StopCoroutine(discoveryMonster_co);
            discoveryMonster_co = StartCoroutine(DiscoveryPlayer_co());
        }
    }

    IEnumerator DiscoveryPlayer_co()
    {
        SetMove_AI(false);
        SetAnimation(MonsterAnimation.Idle);
        float time = 0f; //예비 탈출용
        Vector3 curPlayerPos = playerTrans.position;
        Vector3 curPlayerdirection = curPlayerPos - transform.position;
        Quaternion targetAngle = Quaternion.LookRotation(curPlayerdirection);

        //TODO: 몬스터의 머리 위에 느낌표 띄우기.

        while (time < 2f) //* 몬스터의 방향을 돌린다.
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetAngle, Time.deltaTime * 4.0f);

            if (transform.rotation == targetAngle)
                break;
            else
            {
                time += Time.deltaTime;
                yield return null;
            }
        }

        CheckPlayerCollider();
    }
    #endregion

    public override void Tracing_Movement()
    {
        if (!isTracing)
        {
            Debug.Log("monsterState Tracing");

            isTracing = true;
            SetPlayerAttackList(true);

            SetMove_AI(true);
        }
        //움직임.

        //* 플레이어 쫓아감.
        navMeshAgent.SetDestination(playerTrans.position);
        SetAnimation(MonsterAnimation.Move);

        //몬스터와 플레이어 사이의 거리 체크
        CheckDistance();
    }
    //---------------------------------------------------------------------------------------------------------------------------------------------------//
    //* 몬스터가 점프하고 바닥에 닿았는지 체크
    public void PorinGroudCheck()
    {

        Debug.DrawRay(m_Vector, -transform.up, Color.red, 5f);
        RaycastHit hit;
        if (Physics.Raycast(m_Vector, -transform.up, out hit, 5f))
        {
            if (isGround && hit.distance > 0.1f)
            {
                Debug.Log("바닥에서 떨어짐");
                isGround = false;
            }
            else if (!isGround && hit.distance <= 0.1f)
            {
                Debug.Log("바닥에 붙음");
                isGround = true;
            }
        }
    }


}
