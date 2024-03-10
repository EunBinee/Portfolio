using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerController playerController;
    public Transform playerHeadPos; //* 벽체크에쓰임
    public Transform playerBackPos; //* 조준 할때 쓰임

    bool isBeingAttention = false; //주목  여부 (true 주목, false 주목 아님)

    [Header("카메라 오브젝트.")]
    public GameObject playerCamera;      //카메라 오브젝트
    public GameObject playerCameraPivot; //카메라 피봇
    public Camera cameraObj;             //카메라.
    Transform cameraTrans;        //카메라의  transform 

    [Header("스피트")]
    public float cameraFollowSpeed = 15f;
    public float left_right_LookSpeed = 500; //왼 오 돌리는 스피드
    public float up_down_LookSpeed = 500;    //위아래로 돌리는 스피드

    [Header("위아래 고정 비율  >> 0이면 위아래로 카메라 안움직임")]
    public float minPivot = -35;              //위아래 고정 시키기 위한 Pivot -35로 아래 고정
    public float maxPivot = 35;               //35로 위 고정

    [Header("Camera Debug")]
    //카메라가 캐릭터를 쫒아가는 데 속력. zero로 초기화
    public Vector3 cameraFllowVelocity = Vector3.zero;
    public float left_right_LookAngle;
    public float up_down_LookAngle;

    [Header("벽체크 후, 앞으로 가는 속도")]
    public float frontOfTheWall_Speed = 20;
    //* 주목X Z값
    float minZ = -0.9f; //벽 앞이라 Z값을 땡겼을때
    float maxZ = -5f;
    float minY = 1.2f; //Pivot의 Y값 고정

    //* 주목O Z값
    float minZ_Attention = -0.9f; //벽 앞이라 Z값을 땡겼을때
    float maxZ_Attention = -7f;
    float minY_Attention = 1.4f; //벽 앞이라 Z값을 땡겼을때 Pivot의 Y값
    float maxY_Attention = 1.7f;
    float time_Z = 0;

    [Header("스크롤")]
    public float scrollSpeed = 20;// 스크롤 속도
    public float default_FieldOfView = 35f; //기본 zoom


    [Header("조준 카메라")]
    public bool use_aimCamera = false; // 조준 사용여부

    public void Awake()
    {
        cameraTrans = cameraObj.gameObject.GetComponent<Transform>();
    }
    void Start()
    {
        Init();
    }

    private void Init()
    {
        playerController = GameManager.Instance.gameData.GetPlayerController();
        playerHeadPos = GameManager.Instance.gameData.playerHeadPos; //* 벽체크에쓰임
        playerBackPos = GameManager.Instance.gameData.playerBackPos; //* 조준 할때 쓰임
        left_right_LookAngle = 0;
        up_down_LookAngle = 0;
        CamReset();
    }

    void Update()
    {
        CameraInput();
    }

    //* 카메라 Input
    public void CameraInput()
    {            //* 마우스 휠 줌인줌아웃
        float scroll = -Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        if (scroll != 0)
        {
            ScrollZoomInOut(scroll);
        }
    }

    //* 스크롤 줌인 줌아웃
    public void ScrollZoomInOut(float scroll)
    {
        if (cameraObj.fieldOfView <= 20f && scroll < 0)
        {
            cameraObj.fieldOfView = 20f;
        }
        else if (cameraObj.fieldOfView >= 50f && scroll > 0)
        {
            cameraObj.fieldOfView = 50f;
        }
        else
        {
            cameraObj.fieldOfView += scroll;
        }
    }
    private void FixedUpdate()
    {
        if (use_aimCamera)
        {
            //* 조준 카메라
        }
        else
        {
            //CameraFollowPlayer();
        }

    }

    private void LateUpdate()
    {
        if (use_aimCamera)
        {
            //* 조준 카메라
        }
        else
        {
            CameraActions();
        }
    }

    #region 일반 카메라 (조준X, 주목X
    //* 카메라 움직임
    private void CameraActions()
    {
        CameraFollowPlayer(); //*플레이어를 따라다니는 카메라

        if (isBeingAttention) //* 주목 카메라
        {
            //! 몬스터 생성되면 이제 만들 예정
            //TargetRotate();
            //FixCamZ();
        }
        else
        {
            CameraRotate();  //마우스 방향에 따른 카메라 방향
            float camPosZ = WallInFrontOfCamera(minZ, maxZ);
            Vector3 targetPos = new Vector3(0, 0, camPosZ);
            if (playerController.playerCurState.isSprinting)
            {
                cameraObj.gameObject.transform.localPosition = Vector3.Lerp(cameraObj.gameObject.transform.localPosition, targetPos, frontOfTheWall_Speed * 2f * Time.deltaTime);
            }
            else
                cameraObj.gameObject.transform.localPosition = Vector3.Lerp(cameraObj.gameObject.transform.localPosition, targetPos, frontOfTheWall_Speed * Time.deltaTime);

            if (playerCameraPivot.transform.localPosition.y != minY)
            {
                playerCameraPivot.transform.localPosition = new Vector3(0, minY, 0);
            }
        }
    }

    public void CameraFollowPlayer()
    {
        //플레이어를 따라다니는 카메라
        Vector3 cameraPos;

        CameraFollowCameraSpeed();
        cameraPos = Vector3.Lerp(playerCamera.transform.position, playerController.gameObject.transform.position, cameraFollowSpeed * Time.deltaTime);
        playerCamera.transform.position = cameraPos;

    }

    private void CameraRotate() // 일반 카메라
    {
        //마우스 방향에 따른 카메라 방향
        Vector3 cameraRot;
        Quaternion targetCameraRot;
        left_right_LookAngle += (playerController.playerInput_Info.mouseX * left_right_LookSpeed) * Time.deltaTime;
        up_down_LookAngle -= (playerController.playerInput_Info.mouseY * up_down_LookSpeed) * Time.deltaTime;

        up_down_LookAngle = Mathf.Clamp(up_down_LookAngle, minPivot, maxPivot); //위아래 고정

        //가로 세로 => 카메라 최상위 부모
        cameraRot = Vector3.zero;
        cameraRot.y = left_right_LookAngle;
        targetCameraRot = Quaternion.Euler(cameraRot);
        playerCamera.transform.rotation = targetCameraRot;
        //위아래 => cameraPivot
        cameraRot = Vector3.zero;
        cameraRot.x = up_down_LookAngle;
        targetCameraRot = Quaternion.Euler(cameraRot);
        playerCameraPivot.transform.localRotation = targetCameraRot;
    }

    #endregion

    #region 벽체크
    //*------------------------------------------------------------------------------------------//
    //* 카메라 벽체크
    public float WallInFrontOfCamera(float max = -0.9f, float min = -5f)
    {
        //if (playerController.playerCurState.isStartComboAttack || playerController.playerCurState.isDodgeing)
        //    return cameraObj.gameObject.transform.localPosition.z;

        int monsterLayerMask = 1 << LayerMask.NameToLayer("Monster"); //몬스터 제외
        Vector3 curDirection = cameraObj.gameObject.transform.position - playerHeadPos.position;
        Debug.DrawRay(playerHeadPos.position, curDirection * 20, Color.magenta);
        Ray ray = new Ray(playerHeadPos.position, curDirection);
        RaycastHit hit;
        // Ray와 충돌한 경우
        if (Physics.Raycast(ray, out hit, 20, ~monsterLayerMask)) //몬스터 제외
        {
            float dist = Vector3.Distance(hit.point, cameraObj.gameObject.transform.position);//  (hit.point - cameraObj.gameObject.transform.position).magnitude;

            bool isbehind = CheckObj_behindCamera(hit.point);

            float camPosZ = 0;
            if (isbehind)
            {
                //앞에있음
                camPosZ = cameraObj.gameObject.transform.localPosition.z + dist;
            }
            else
            {
                //뒤에 있음.
                camPosZ = cameraObj.gameObject.transform.localPosition.z - dist;
            }

            if (camPosZ >= max)
            {
                camPosZ = max;
            }
            if (camPosZ <= min)
            {
                camPosZ = min;
            }
            return camPosZ;
        }
        return min;
    }

    //* 감지된 객체가 카메라의 뒤에 있는지 앞에 있는지 확인용 함수
    public bool CheckObj_behindCamera(Vector3 objPos)
    {
        // 충돌 지점이 현재 객체의 앞쪽에 있는지 뒷쪽에 있는지 확인
        Vector3 directionToHitPoint = objPos - cameraObj.gameObject.transform.position;
        float dotProduct = Vector3.Dot(directionToHitPoint, cameraObj.gameObject.transform.forward);

        if (dotProduct > 0)
        {
            // 충돌 지점이 현재 객체의 앞쪽에 있음
            //Debug.Log("충돌 지점이 앞에 있습니다.");
            return true;
        }
        else
        {
            // 충돌 지점이 현재 객체의 뒷쪽에 있음
            //Debug.Log("충돌 지점이 뒤에 있습니다.");
            return false;
        }
    }

    #endregion

    //* -------------------------------------------------------------------------------------------------------------------------------------------//
    void CamReset()
    {
        float resetZ = WallInFrontOfCamera(minZ, maxZ);
        cameraObj.gameObject.transform.localPosition = new Vector3(0, 0, resetZ);
        cameraObj.gameObject.transform.localRotation = Quaternion.identity;
    }

    public void Check_Z()
    {

        //* 주목X Z값
        minZ = -0.9f; //벽 앞이라 Z값을 땡겼을때
        maxZ = -5f;
        minY = 1.2f; //Pivot의 Y값 고정
                     //* 주목O Z값
        minZ_Attention = -0.9f; //벽 앞이라 Z값을 땡겼을때
        maxZ_Attention = -7f;
        minY_Attention = 1.4f; //벽 앞이라 Z값을 땡겼을때 Pivot의 Y값
        maxY_Attention = 1.7f;
        time_Z = 0;
    }

    public void CameraFollowCameraSpeed()
    {

        if (playerController.playerCurState.isSprinting)
        {
            if (cameraFollowSpeed != 20f)
                cameraFollowSpeed = 20f;
        }
        else
        {
            if (cameraFollowSpeed != 15f)
                cameraFollowSpeed = 15f;
        }

    }
}
