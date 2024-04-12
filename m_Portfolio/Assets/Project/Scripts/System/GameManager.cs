using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    public GameData gameData;
    public CameraController cameraController;

    public List<Monster> monsterUnderAttackList; //플레이어를 공격중인 몬스터

    private void Awake()
    {
        Init();

    }

    void Start()
    {

    }

    void Init()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);

        cameraController = gameData.GetCameraController();
        monsterUnderAttackList = new List<Monster>();
    }

    void Update()
    {

    }
}
