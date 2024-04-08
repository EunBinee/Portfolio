using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;

    public HPBarManager hpBarManager;


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
        hpBarManager = GetComponent<HPBarManager>();
        

        CursorVisible(false);
    }

    private void CursorVisible(bool visible)
    {
        //true => 커서 보이게. false => 커서 안보이게

        if (!visible)
            Cursor.lockState = CursorLockMode.Locked; //커서 화면 고정.
        if (visible)
            Cursor.lockState = CursorLockMode.None; //커서 화면 고정 해제.

        Cursor.visible = visible;
    }
}
