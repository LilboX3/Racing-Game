using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_InputManager : MonoBehaviour
{
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
        GetAIInput();
    }

    // =====*=====*=====*=====*==========[ End of:   ][ Unity Native Functions ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Core Functions ]=====*=====*=====*=====*==========

    void GetAIInput()
    {
        // Code for AI controlling the car

        vertical = 0.5f;  // Move forward at half speed
        horizontal = 0.0f;
        handbrake = false;
        boosting = false;
    }
    // =====*=====*=====*=====*==========[ End of:   ][ Core Functions ]=====*=====*=====*=====*==========
}
