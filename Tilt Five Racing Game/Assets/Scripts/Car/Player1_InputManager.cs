using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player1_InputManager : MonoBehaviour
{
    // =====*=====*=====*=====*==========[ Start of: ][ Code Helper ]=====*=====*=====*=====*==========
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    // =====*=====*=====*=====*==========[ End of:   ][ Code Helper ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Variable Declarations ]=====*=====*=====*=====*==========
    // ----------v---------- Settings and private runtime variables ----------v----------
    
    // ----------v---------- Public runtime variables for other scripts ----------v----------
    [HideInInspector] public float vertical;
    [HideInInspector] public float horizontal;
    [HideInInspector] public bool handbrake;
    [HideInInspector] public bool boosting;
    // =====*=====*=====*=====*==========[ End of:   ][ Variable Declarations ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Unity Native Functions ]=====*=====*=====*=====*==========

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetPlayerInput();
    }

    // =====*=====*=====*=====*==========[ End of:   ][ Unity Native Functions ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Core Functions ]=====*=====*=====*=====*==========

    void GetPlayerInput()
    {
        // Code to control the car by player input

        horizontal = Input.GetAxis(HORIZONTAL);
        vertical = Input.GetAxis(VERTICAL);
        handbrake = (Input.GetAxis("Jump") != 0) ? true : false;
        if (Input.GetKey(KeyCode.LeftShift)) boosting = true;
        else boosting = false;
    }
    // =====*=====*=====*=====*==========[ End of:   ][ Core Functions ]=====*=====*=====*=====*==========
} // <-[End of Class]
