using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager instance = null;

    public GameObject monster_HPBarUI;

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        if (instance == null)
        {
            instance = this;
        }

    }

}
