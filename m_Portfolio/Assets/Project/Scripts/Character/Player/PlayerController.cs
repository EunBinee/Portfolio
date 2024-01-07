using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerComponents playerComponents = new PlayerComponents();
    public PlayerInput playerInput = new PlayerInput();
    public PlayerOption playerOption = new PlayerOption();
    public PlayerCurState playerCurState = new PlayerCurState();
    public PlayerCurValue playerCurValue = new PlayerCurValue();

    private PlayerComponents P_com => playerComponents;
    private PlayerInput P_input => playerInput;
    private PlayerOption P_option => playerOption;
    private PlayerCurState P_State => playerCurState;
    private PlayerCurValue P_Value => playerCurValue;



    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerMovement.PlayerMovement_Init();
    }

    void Update()
    {

    }

    void FixedUpdate()
    {

    }

}
