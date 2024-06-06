using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager1 : MonoBehaviour
{

    // =====*=====*=====*=====*==========[ Start of: ][ Code Helper ]=====*=====*=====*=====*==========
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    // =====*=====*=====*=====*==========[ End of:   ][ Code Helper ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Variable Declarations ]=====*=====*=====*=====*==========
    // ----------v---------- Settings and private runtime variables ----------v----------
    public bool isPlayerControlled = true;

    // ----------v---------- Public runtime variables for other scripts ----------v----------
    [HideInInspector] public float vertical;
    [HideInInspector] public float horizontal;
    [HideInInspector] public bool handbrake;
    [HideInInspector] public bool boosting;

    // ----------v---------- Delegates ----------v----------
    private System.Action controlInputMethod;  // Action delegate to switch input control methods

    // =====*=====*=====*=====*==========[ End of:   ][ Variable Declarations ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Unity Native Functions ]=====*=====*=====*=====*==========
    
    // Start is called before the first frame update
    void Start()
    {
        // Set the input method based on control type
        controlInputMethod = isPlayerControlled ? (System.Action)PlayerInput : AIInput;
    }
    
    // Update is called once per frame
    void Update()
    {
        controlInputMethod();
    }

    // =====*=====*=====*=====*==========[ End of:   ][ Unity Native Functions ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Core Functions ]=====*=====*=====*=====*==========
    // Delegated functions to choose between AI and Player input control
    void PlayerInput()
    {
        // Code to control the car by player input

        horizontal = Input.GetAxis(HORIZONTAL);
        vertical = Input.GetAxis(VERTICAL);
        handbrake = (Input.GetAxis("Jump") != 0) ? true : false;
        if (Input.GetKey(KeyCode.LeftShift)) {
            boosting = true;
        } 
        else boosting = false;
    }
    void AIInput()
    {
        // Code for AI controlling the car

        vertical = 0.5f;  // Move forward at half speed
        horizontal = 0.0f;
        handbrake = false;
        boosting = false;
    }
    // =====*=====*=====*=====*==========[ End of:   ][ Core Functions ]=====*=====*=====*=====*==========
}
