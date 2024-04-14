using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using Unity.VisualScripting;
using UnityEngine.Animations;

public class MonsterPattern : MonoBehaviour
{

    //* 플레이어 관련  변수
    protected PlayerController playerController;
    protected PlayerMovement playerMovement;
    protected Transform playerTrans;
    protected Transform playerTargetPos;
    protected int playerLayerId = 6;
    protected int playerlayerMask; //플레이어 캐릭터 레이어 마스크

    //* 몬스터
    protected Monster m_monster;
    protected Animator m_animator;
    public Rigidbody rigid;
    protected NavMeshAgent navMeshAgent;
    protected Vector3 originPosition; //몬스터 원래 위치
    public bool playerHide = false; //* 플레이어가 숨어있나? ( 원거리 한정 )
    public bool noAttack = false;   //플레이어에게 공격 안받음.
    public bool canAttack = false;  //몬스터의 공격이 들어가는 순간.//

    public bool forcedReturnHome = false; //플레이어 대화시 강제로 집으로 보내기

    protected Vector3 curHitPos;
    protected Quaternion curHitQuaternion;


    public enum MonsterState
    {
        Roaming,
        Discovery,
        Tracing,
        Attack,
        GetHit,
        GoingBack,
        Death,
        Stop // 잠깐 멈추는 상태. ex.연출씬
    }
    protected MonsterState curMonsterState; //현재 몬스터 상태
    protected MonsterState preMonsterState; //이전 몬스터 상태
    public enum MonsterAnimation
    {
        Idle,
        Move,
        Move_Dodge,
        GetHit,
        Death
    }

    public enum MonsterAttackAnimation
    {
        ResetAttackAnim,
        Short_Range_Attack,
        Long_Range_Attack,
    }

    protected float overlapRadius; //플레이어 발견 범위
    protected float findDistance_BehindPlayer;
    // * --------------------------------------------------------//
    // * 로밍관련 변수들
    protected int roaming_RangeX;
    protected int roaming_RangeZ;

    protected Vector3 roam_vertex01; //사각형 왼쪽 가장 위
    protected Vector3 roam_vertex02; //사각형 왼쪽 가장 아래
    protected Vector3 roam_vertex03; //사각형 오른쪽 가장 아래
    protected Vector3 roam_vertex04; //사각형 오른쪽 가장 위

    protected Vector3 mRoaming_randomPos = Vector3.zero;
    //* --------------------------------------------------------//
    public bool isRoaming = false;
    public bool isFinding = false; //* 로밍중이다가 발견
    public bool isTracing = false;
    public bool isGoingBack = false;
    public bool isGettingHit = false;

    public enum MonsterMotion
    {
        Short_Range_Attack,
        Long_Range_Attack,
        GetHit_KnockBack, //넉백
        Death
    }

    void Start()
    {
        Init();
    }

    public virtual void Init()
    {
        m_monster = GetComponent<Monster>();
        m_animator = GetComponent<Animator>();

        rigid = GetComponent<Rigidbody>();
        playerController = GameManager.instance.gameData.GetPlayerController();
        playerMovement = GameManager.instance.gameData.GetPlayerMovement();
        playerTrans = GameManager.Instance.gameData.GetPlayerTransform();
        playerTargetPos = GameManager.Instance.gameData.playertargetPos;

        m_monster.monsterPattern = this;

        //* 네비 메쉬
        if (m_monster.monsterData.movingMonster)
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            navMeshAgent.updateRotation = false;
        }

        playerlayerMask = 1 << playerLayerId; //플레이어 레이어

        ChangeMonsterState(MonsterState.Roaming);
        originPosition = transform.position;

        overlapRadius = m_monster.monsterData.overlapRadius; //플레이어 감지 범위.
        findDistance_BehindPlayer = m_monster.monsterData.findDistance_BehindPlayer; //뒤에 있는플레이어 감지 범위.
        roaming_RangeX = m_monster.monsterData.roaming_RangeX; //로밍 범위 x;
        roaming_RangeZ = m_monster.monsterData.roaming_RangeZ; //로밍 범위 y;
        CheckRoam_Range();

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
        CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
        capsuleCollider.enabled = true;

        playerHide = false;
    }

    public void Update()
    {
        Monster_Pattern();
        useUpdate();

        if (m_monster.monsterData.movingMonster)
        {
            UpdateRotation();
        }
    }

    public virtual void useUpdate()
    {

    }

    private void FixedUpdate()
    {
        if (m_monster.monsterData.movingMonster)
        {
            FreezeVelocity();
        }
    }

    private void FreezeVelocity()
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }

    public virtual void UpdateRotation()
    {
        if (navMeshAgent.desiredVelocity.sqrMagnitude >= 0.1f * 0.1f)
        {
            //적 ai의 이동방향
            Vector3 direction = navMeshAgent.desiredVelocity;
            //회전 각도 산출 후, 선형 보간 함수로 부드럽게 회전
            Quaternion targetAngle = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetAngle, Time.deltaTime * 8.0f);
        }
    }

    public MonsterState GetCurMonsterState()
    {
        return curMonsterState;
    }

    public virtual void SetAnimation(MonsterAnimation m_anim)
    {
        switch (m_anim)
        {
            case MonsterAnimation.Idle:
                break;
            case MonsterAnimation.Move:
                break;
            case MonsterAnimation.GetHit:
                break;
            case MonsterAnimation.Death:
                break;
            default:
                break;
        }
    }

    public virtual void SetAttackAnimation(MonsterAttackAnimation monsterAttackAnimation, int animIndex = 0)
    {
        switch (monsterAttackAnimation)
        {
            case MonsterAttackAnimation.ResetAttackAnim:
                break;
            case MonsterAttackAnimation.Short_Range_Attack:
                break;
            case MonsterAttackAnimation.Long_Range_Attack:
                break;
            default:
                break;
        }
    }

    protected void NavMesh_Enable(bool enable)
    {
        if (enable)
        {
            //네비메쉬 키기
            navMeshAgent.enabled = true;
        }
        else
        {
            //끄기 
            navMeshAgent.enabled = false;
        }
    }

    protected void SetMove_AI(bool moveAI)
    {
        if (moveAI)
        {
            //움직임.
            navMeshAgent.isStopped = false;
            navMeshAgent.updatePosition = true;
        }
        else if (!moveAI)
        {
            //멈춤
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
            navMeshAgent.updatePosition = false;
        }
    }

    protected void ChangeMonsterState(MonsterState monsterState)
    {

        if (curMonsterState != MonsterState.Death)
        {
            preMonsterState = curMonsterState;
            curMonsterState = monsterState;
        }
    }

    public virtual void Monster_Pattern()
    {
        switch (curMonsterState)
        {
            case MonsterState.Roaming:
                break;
            case MonsterState.Discovery:
                break;
            case MonsterState.Tracing:
                break;
            case MonsterState.GoingBack:
                break;
            default:
                break;
        }
    }

    // * -------------------------------------------------------------------------//
    // * 몬스터 상태 =>> 로밍
    public virtual void Roam_Monster()
    {
        if (!isRoaming)
        {
            isRoaming = true;
        }
    }
    //로밍할 랜덤 좌표
    public Vector3 GetRandom_RoamingPos()
    {
        //로밍시, 랜덤한 위치 생성
        float randomX = UnityEngine.Random.Range(originPosition.x + ((roaming_RangeX / 2) * -1), originPosition.x + (roaming_RangeX / 2));
        float randomZ = UnityEngine.Random.Range(originPosition.z + ((roaming_RangeZ / 2) * -1), originPosition.z + (roaming_RangeZ / 2));
        return new Vector3(randomX, transform.position.y, randomZ);
    }
    //랜덤 좌표에 장애물이 있는지 확인하는 함수
    public bool CheckObstacleCollider(Vector3 randomPos)
    {
        //몬스터 로밍시 지정된 장소에 장애물이 있는지 확인
        Collider[] colliders = Physics.OverlapSphere(randomPos, 1);
        if (colliders.Length > 1)
        {
            //장애물 존재 (1은 바닥 콜라이더)
            return false;
        }
        return true;
    }

    // * -------------------------------------------------------------------------//
    // * 몬스터의 발견범위에 플레이어가 들어왔는지 확인하는 함수.
    public virtual void CheckPlayerCollider()
    {
        if (curMonsterState != MonsterState.Death)
        {
            //로밍중, 집돌아갈 때 플레이어 콜라이더 감지중
            Collider[] playerColliders = Physics.OverlapSphere(transform.position, overlapRadius, playerlayerMask);

            if (0 < playerColliders.Length)
            {
                //몬스터의 범위에 들어옴
            }
        }
    }

    // * -------------------------------------------------------------------------//
    // * 몬스터 상태 =>> 발견
    public virtual void Discovery_Player()
    {
        if (!isFinding)
        {
            isFinding = true;
        }
    }

    // * ------------------------------------------------------------------------//
    // * 몬스터 상태 =>> 추적
    public virtual void Tracing_Movement()
    {
    }

    // * ------------------------------------------------------------------------//
    // * 몬스터 상태 =>> 다시 자기자리로
    public virtual void GoingBack_Movement()
    {
        SetMove_AI(true);

        SetAnimation(MonsterAnimation.Move);

        navMeshAgent.SetDestination(originPosition);
        CheckDistance();       //계속 거리 체크
        CheckPlayerCollider();
    }

    // * 몬스터 추격, 자기자리로 돌아갈 때 ==>>> 몬스터와 플레이어 사이의 거리 체크
    public virtual void CheckDistance()
    {
        //해당 몬스터와 플레이어 사이의 거리 체크
        switch (curMonsterState)
        {
            case MonsterState.Roaming:
                break;

            case MonsterState.Tracing:
                break;

            case MonsterState.Attack:
                break;

            case MonsterState.GoingBack:
                break;
        }
    }

    // * -----------------------------------------------------------------------//
    // * 몬스터 공격 모션, 피격 모션, 죽음 모션 등등
    public virtual void Monster_Motion(MonsterMotion monsterMotion)
    {
        switch (monsterMotion)
        {
            case MonsterMotion.Short_Range_Attack:
                break;
            case MonsterMotion.Long_Range_Attack:
                break;
            case MonsterMotion.GetHit_KnockBack:
                break;
            case MonsterMotion.Death:
                break;
            default:
                break;
        }
    }


    public virtual void StopAtackCoroutine()
    {

    }

    // * ---------------------------------------------------------------------------------------//
    //! 특정 범위안에 플레이어가 있는지 파악하고, 데미지 주는 함수
    public bool CheckPlayerDamage(float _overlapRadius, Vector3 _targetPos, float damage = 0, bool isFallDown = false)
    {
        Collider[] playerColliders = Physics.OverlapSphere(_targetPos, _overlapRadius, playerlayerMask);
        if (0 < playerColliders.Length)
        {
            if (isFallDown)
            {
                //m_monster.OnHit_FallDown(damage, 8);
            }
            else
                m_monster.OnHit(damage);
            return true;
        }
        else
        {
            return false;
        }
    }

    //* ----------------------------------------------------------------------------------------//
    /*
    protected Effect GetDamage_electricity(Vector3 randomPos, Transform parent = null, float angle = -1)
    {
        Effect effect;
        //* 전기 이펙트
        if (parent != null)
            effect = GameManager.Instance.objectPooling.ShowEffect("electric", parent);
        else
            effect = GameManager.Instance.objectPooling.ShowEffect("electric");
        effect.transform.position = randomPos;

        if (angle != -1)
        {
            Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
            effect.transform.localPosition = rotation * effect.transform.localPosition;
        }

        return effect;
    }
    */
    // * ---------------------------------------------------------------------------------------//
    //! 발사체 쏘는 공격시, 플레이어 앞에 물체가 있는지 확인
    public virtual bool HidePlayer(Vector3 curOriginPos, Vector3 targetDir)
    {
        //* 발사체 공격 시, 플레이어의 앞에 물체가 있는지 확인.!
        //*  curOriginPos : 레이를 발사하는 곳 ; targetDir : originPos에서 부터 플레이어로 향하는 방향 벡터
        //* 리턴 false 플레이어가 가장 앞에 있음. 
        //* 리턴 true 플레이어 앞에 장애물 있음 ( = 플레이어 숨음).

        float range = 100f;
        float playerDistance = 0;
        float shortestDistance = 1000;

        bool playerInRay = false;

        Debug.DrawRay(curOriginPos, targetDir * 100f, Color.blue);

        RaycastHit[] hits;
        hits = Physics.RaycastAll(curOriginPos, targetDir, range);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.name != this.gameObject.name && hit.collider.gameObject.tag != "Monster") //자기자신과 몬스터 제외
            {
                float distance = hit.distance;
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;

                    if (hit.collider.tag == "Player")
                    {
                        playerInRay = true;
                        playerDistance = hit.distance;
                    }
                }
            }
        }

        if (playerInRay)
        {
            if (shortestDistance >= playerDistance) //* 플레이어 가장 앞에 있음.
                return false;
            else
                return true;
        }
        return true;
    }

    //* ----------------------------------------------------------------------------------------//
    //! 플레이어가 몬스터의 뒤에 있는지 앞에 있는지 확인용 함수
    //* 위아래
    public bool PlayerLocationCheck_BackForth()
    {
        //* 앞뒤 체크
        Vector3 curDirection = GetDirection(playerTrans.position, transform.position);
        // 몬스터에서 플레이어로의 벡터와 몬스터의 전방 벡터를 내적
        float dotProduct = Vector3.Dot(curDirection.normalized, transform.forward);

        if (dotProduct > 0)
        {
            // * 플레이어가 몬스터의 전방에 있을 때:
            //Debug.Log("플레이어는 몬스터의 앞에 있습니다.");
            return true;
        }
        else
        {
            // * 플레이어가 몬스터의 뒤에 있을 때:
            //Debug.Log("플레이어는 몬스터의 뒤에 있습니다.");
            return false;
        }
    }
    //* 좌우
    public bool PlayerLocationCheck_LeftRight()
    {
        if (playerTrans.position.x < transform.position.x)
        {
            //* 왼쪽일 경우 true
            return true;
        }
        else if (playerTrans.position.x > transform.position.x)
        {
            //* 오른 쪽일 경우 false;
            return false;
        }
        else
        {
            return false;
        }
    }
    //*------------------------------------------------------------------------------------------//
    //* 플레이어를 공격하는 몬스터 리스트 (주목에서 쓰임)

    public void SetPlayerAttackList(bool attackMonster)
    {
        if (GameManager.instance.cameraController != null)
        {
            PlayerAttackList(attackMonster, GameManager.instance.cameraController);
        }
    }

    private void PlayerAttackList(bool attackMonster, CameraController cameraObj)
    {
        //* true 공격을 시작한 몬스터 => 리스트에 넣기
        //* false 공격을 마친 몬스터  => 리스트에서 빼기

        if (attackMonster)
        {
            if (!GameManager.instance.monsterUnderAttackList.Contains(m_monster))
            {
                GameManager.instance.monsterUnderAttackList.Add(m_monster);
            }
        }
        else
        {
            if (GameManager.instance.monsterUnderAttackList.Contains(m_monster))
            {
                GameManager.instance.monsterUnderAttackList.Remove(m_monster);
            }

        }
    }

    //*-----------------------------------------------------------------------------------------//
    public void SetGetDemageMonster(Vector3 pos, Quaternion qua)
    {
        curHitPos = pos;
        curHitQuaternion = qua;
    }
    // * ---------------------------------------------------------------------------------------//
    private void OnDrawGizmos()
    {
        //몬스터 감지 범위 Draw
        //크기는  monsterData.overlapRadius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, overlapRadius);

        if (curMonsterState == MonsterState.Roaming)
        {
            //만약 로밍 중이면, 로밍 범위 그리기
            Gizmos.DrawLine(roam_vertex01, roam_vertex02);
            Gizmos.DrawLine(roam_vertex02, roam_vertex03);
            Gizmos.DrawLine(roam_vertex03, roam_vertex04);
            Gizmos.DrawLine(roam_vertex04, roam_vertex01);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(mRoaming_randomPos, 1);
    }
    //* 랜덤 로밍 범위 체크
    protected void CheckRoam_Range()
    {
        //사각형 왼쪽 가장 위
        roam_vertex01 = new Vector3(transform.position.x + ((roaming_RangeX / 2) * -1), transform.position.y, transform.position.z + (roaming_RangeZ / 2));
        //사각형 왼쪽 가장 아래
        roam_vertex02 = new Vector3(transform.position.x + ((roaming_RangeX / 2) * -1), transform.position.y, transform.position.z + ((roaming_RangeZ / 2) * -1));
        //사각형 오른쪽 가장 아래
        roam_vertex03 = new Vector3(transform.position.x + (roaming_RangeX / 2), transform.position.y, transform.position.z + ((roaming_RangeZ / 2) * -1));
        //사각형 오른쪽 가장 위
        roam_vertex04 = new Vector3(transform.position.x + (roaming_RangeX / 2), transform.position.y, transform.position.z + (roaming_RangeZ / 2));
    }

    //* 방향 벡터 start--->target
    public Vector3 GetDirection(Vector3 target, Vector3 startPos)
    {
        //startPos에서 target으로 가는 방향 벡터;
        Vector3 curDirection = target - startPos;

        return curDirection;
    }

    /*
    public virtual void StopMonster()
    {
        //* HP bar UI 회수
        //플레이어 대화시 강제로 집으로 보내기
        forcedReturnHome = true; //* true 일때 플레이어를 인지하지 못함.
        if (m_monster.HPBar_CheckNull() == true)
            m_monster.RetrunHPBar();
    }
    public virtual void StartMonster()
    {
        forcedReturnHome = false;
    }
    */

    //*-------------------------------------------------------------------------------------//
    //*현재 객체의 아래로 레이를 쏴서 아래에 있는 객체의 접점 point를 가지고와줌
    public Vector3 GetGroundPos(Transform raySelf)
    {
        //* 바로 아래가 ground가 아니더라도 바로 아래에 있는 객체의 Point 좌표를 가지고 옴.
        float range = 50f;
        RaycastHit[] hits;
        RaycastHit shortHit;
        Debug.DrawRay(raySelf.position + (raySelf.up * 0.5f), -raySelf.up * 100, Color.red);
        hits = Physics.RaycastAll(raySelf.position + (raySelf.up * 0.5f), -raySelf.up, range);

        float shortDist = 1000f;

        if (hits.Length != 0)
        {
            shortHit = hits[0];
            foreach (RaycastHit hit in hits)
            {
                //자기 자신 제외.
                if (hit.collider.transform.name != raySelf.gameObject.name)
                {

                    if (raySelf.CompareTag("Player") && hit.collider.gameObject.CompareTag("Player"))
                    {
                        //* 플레이어일때 플레이어 태그가 붙은 아이들은 무시
                        //- ex. 무기들
                    }
                    else
                    {
                        //자기 자신은 패스
                        float distance = hit.distance;

                        if (shortDist > distance)
                        {
                            shortHit = hit;
                            shortDist = distance;
                        }
                    }

                }
            }

            Vector3 hitPoint = shortHit.point;
            return hitPoint;
        }
        return Vector3.zero;
    }
    public Vector3 GetGroundNormal(Transform raySelf)
    {
        //* 바로 아래가 ground가 아니더라도 바로 아래에 있는 객체의 Point 좌표를 가지고 옴.
        float range = 50f;
        RaycastHit[] hits;
        RaycastHit shortHit;
        Debug.DrawRay(raySelf.position + (raySelf.up * 0.5f), -raySelf.up * 100, Color.red);
        hits = Physics.RaycastAll(raySelf.position + (raySelf.up * 0.5f), -raySelf.up, range);

        float shortDist = 1000f;

        if (hits.Length != 0)
        {
            shortHit = hits[0];
            foreach (RaycastHit hit in hits)
            {
                //자기 자신 제외.
                if (hit.collider.transform.name != raySelf.gameObject.name)
                {

                    if (raySelf.CompareTag("Player") && hit.collider.gameObject.CompareTag("Player"))
                    {
                        //* 플레이어일때 플레이어 태그가 붙은 아이들은 무시
                        //- ex. 무기들
                    }
                    else
                    {
                        //자기 자신은 패스
                        float distance = hit.distance;

                        if (shortDist > distance)
                        {
                            shortHit = hit;
                            shortDist = distance;
                        }
                    }

                }
            }

            Vector3 hitnormal = shortHit.normal;
            return hitnormal;
        }
        return Vector3.zero;
    }
}