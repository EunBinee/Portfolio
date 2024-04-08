using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics.Contracts;

[Serializable]
public class GameData
{
    [Header("플레이어")]
    public GameObject player;
    public Transform playerHeadPos;
    public Transform playerBackPos;
    public Transform playertargetPos;

    public Transform GetPlayerTransform()
    {
        return player.GetComponent<Transform>();
    }
    public PlayerController GetPlayerController()
    {
        return player.GetComponent<PlayerController>();
    }
    public PlayerMovement GetPlayerMovement()
    {
        return player.GetComponent<PlayerMovement>();
    }

    [Space]
    [Header("카메라")]
    public GameObject playerCamera;
    public GameObject playerCameraPivot;
    public Camera cameraObj;
    public CameraController GetCameraController()
    {
        return cameraObj.GetComponent<CameraController>();
    }
}
