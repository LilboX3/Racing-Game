using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Build.Content;
#endif
using UnityEngine;

// Syntax Note: Functions with a line of space inbetween are unrelated, functions with no line of space inbetween are a set.

public class CarController1 : MonoBehaviour
{
    // =====*=====*=====*=====*==========[ Start of: ][ Data Types ]=====*=====*=====*=====*==========
    internal enum DriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }
    // =====*=====*=====*=====*==========[ End of:   ][ Data Types ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Variable Declarations ]=====*=====*=====*=====*==========
    // ----------v---------- References ----------v----------
    [Header("Input Manager Script")]
    private InputManager1 inputManager;

    [Header("Wheel Colliders")]
    [SerializeField] public WheelCollider frontLeftWheelCollider;
    [SerializeField] public WheelCollider frontRightWheelCollider;
    [SerializeField] public WheelCollider rearLeftWheelCollider;
    [SerializeField] public WheelCollider rearRightWheelCollider;
    private WheelCollider[] wheelColliders = new WheelCollider[4];
    private WheelCollider[] torqueWheels; // wheels that motor torque is applied to (acceleration)
    private WheelCollider[] brakeWheels; // wheels that brake force is applied to (braking)
    private WheelCollider[] handBrakeWheels; // wheels that handbrake is applied to (handbraking)

    /* WheelCollider.motorTorque:
     *      public float motorTorque;
     *  Description
     *      Motor torque on the wheel axle expressed in Newton metres.Positive or negative depending on direction.
     *      To simulate brakes, do not use negative motor torque - use brakeTorque instead.
    */

    [Header("Wheel Transforms")]
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;
    private Transform[] wheelTransforms = new Transform[4];

    // ----------v---------- Settings and private runtime variables ----------v----------
    [Header("Drive Setup")]
    [SerializeField] private DriveType driveType = DriveType.RearWheelDrive;

    [Header("Input Buttons and Axes")]
    [HideInInspector] private float horizontalInput;
    [HideInInspector] private float verticalInput;
    [HideInInspector] private bool handbrakeInput = false;
    [HideInInspector] private bool boostInput = false;

    //Lowered center of mass in rigidbody to prevent car from flipping
    //Might cause car to behave like a pendulum? see Start()
    [Header("Rigidbody Center of Mass Offset")]
    private Rigidbody carRigidbody;
    private Vector3 centerOfMassOffset;

    [Header("Motor")]
    //[SerializeField] private float motorForce;  // the power the motor applies to the wheels // to be deleted for newer calculation
    public float maxRPM, minRPM;    // used to determine when to shift gears
    private float wheelsRPM;
    [SerializeField] private AnimationCurve enginePowerCurve;
    private float totalEngineTorque;
    private float torquePerWheel;
    private float smoothTime = 0.09f;

    [Header("Gear")]
    [SerializeField] public float[] gearRatios = { 4.0f, 2.8f, 2.2f, 1.6f, 1.1f, 0.7f };
    [SerializeField] public float[] gearChangeSpeed = { 60, 100, 130, 170, 200, 250 }; // used to stop gear shifting when midair
    private bool isNearMaxRPM;

    [Header("Brake")]
    [SerializeField] private float brakeForce = 5.0f;
    private float currentbrakeForce;
    private bool isBreaking;
    private bool isHandBraking;
    [SerializeField] [Range(0.0f, 20.0f)] [Tooltip("Threshhold below which the brake is automatically applied to stop the vehicle from continuously rolling.")]
    private float brakeThreshhold = 5.0f;
    [SerializeField] [Range(0.0f, 20.0f)] [Tooltip("Determines how much a vehicle decelerates when rolling without vertical input (gas pedal being pressed).")]
    private float rollDecay = 5.0f;

    [Header("Steering")]
    private float currentSteerAngle;
    [SerializeField] [Range(10, 80)] [Tooltip("The maximum rotation of the wheel along the steering axis when steering.")]
    private float maxSteerAngle = 30;
    private float turningRadius = 6;

    [Header("Steering Wheel (Cockpit)")]
    [SerializeField] private Transform steeringWheelTransform;
    private Quaternion initialSteeringWheelRotation;
    [SerializeField] private float maxSteeringWheelRotation = 135f;

    [Header ("Wheel Fiction Calculation")]
    [SerializeField] [Range(0.3f, 1.0f)] [Tooltip("Base stiffness for forward friction. Lower: Wheel slips forward/backward easier. Higher: Wheel sticks more to the ground, improving control.")]
    private float forwardStiffness = 1.0f;
    [SerializeField] [Range(0.3f, 1.0f)] [Tooltip("Base stiffness for sideways friction. Lower: Wheel slips sideways easier. Higher: Wheel sticks more to the ground, improving control.")]
    private float sidewaysStiffness = 1.0f;
    [SerializeField] [Range(0.3f, 0.8f)] [Tooltip("Forward friction when the handbrake is engaged. Lower: Wheel slips straight forward/backward easier. Higher: Wheel sticks more to the ground, improving control as if ABS was on.")]
    private float handBrakeForwardStiffnessMultiplier = 0.55f;
    [SerializeField] [Range(0.3f, 0.8f)] [Tooltip("Sideways friction when the handbrake is engaged. Lower: Wheel slips sideways easier. Higher: Wheel sticks more to the ground, improving control as if ABS was on.")]
    private float handBrakeSidewaysStiffnessMultiplier = 0.55f;
    private float handBrakeForwardStiffness; // In Start() pre-calculated forward stiffness for handbraking
    private float handBrakeSidewaysStiffness; // In Start() pre-calculated sideways stiffness for handbraking
    private WheelFrictionCurve normalForwardFriction;
    private WheelFrictionCurve normalSidewaysFriction;
    private WheelFrictionCurve handBrakeForwardFriction;
    private WheelFrictionCurve handBrakeSidewaysFriction;

    [SerializeField] [Range(5,20)] [Tooltip("The effectiveness of the car's spoiler.")]
    private float downForceValue = 10f;

    [Header("Speed Boost Settings")]
    public float speedBoostForceMultiplier = 1.5f;
    public float speedBoostFrictionMultiplier = 1.5f;
    public float speedBoostCharge = 100.0f;                 // charge like a battery from 0 to 100%, this way players can decide how long to speed boost while maintaining a reasonable limit
    public float speedBoostDepletionRate = 20.0f;           // charge depleted per second while speedboost is running
    public float speedBoostRenerationRate = 6.25f;          // charge replenished per second

    // ----------v---------- Public runtime variables for other scripts ----------v----------
    [HideInInspector] public int currentGear = 1;           // the currently active gear
    [HideInInspector] public float KPH;                     // kilometers per hour the car is going
    [HideInInspector] public float currentEngineRPM;        // the engine's current Rotisseriechickens Per Minimies
    [HideInInspector] public bool reverse = false;          // is the car driving in reverse?
    [HideInInspector] public bool isSpeedBoosting = false;  // is the speed boost currently active?
    [HideInInspector] public float currentBoost;            // Current boost charge

    // =====*=====*=====*=====*==========[ End of:   ][ Variable Declarations ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Unity Native Functions ]=====*=====*=====*=====*==========

    private void Awake()
    {

    }

    private void Start()
    {
        GetAllComponents(); // Find all required components
        InitializeWheelConfiguration(); // Initialize the working wheel arrays for code streamlining
        ChangeCenterOfMass(); // Manipulate the center of mass to make the car more stable
        InitializeSteeringWheel(); // Prepare the car's steering wheel (in the cockpit)
    }

    private void FixedUpdate() 
    {
        // old order:
        /*
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        UpdateSteeringWheel();
        */

        GetInput(); // Process input every physics step
        CalculateCarPhysics(); // Manage the motor force application
        SteerVehicle(); // Adjust steering based on inputs
        AdjustTraction(); // NEW: Adjust traction dynamically based on current vehicle state
        UpdateWheels(); // Update wheel positions and rotations
        UpdateSteeringWheel(); // Adjust the visual steering wheel if applicable
        AddDownForce(); // Apply downforce for aerodynamic effects
    }

    // =====*=====*=====*=====*==========[ End of:   ][ Unity Native Functions ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Old Functions ]=====*=====*=====*=====*==========
    /*
    private void GetInput()
    {
        horizontalInput = Input.GetAxis(HORIZONTAL);
        verticalInput = Input.GetAxis(VERTICAL);
        isBreaking = Input.GetKey(KeyCode.Space);
    }
    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        currentbrakeForce = isBreaking ? brakeForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        frontLeftWheelCollider.brakeTorque = currentbrakeForce;
        frontRightWheelCollider.brakeTorque = currentbrakeForce;
        rearLeftWheelCollider.brakeTorque = currentbrakeForce;
        rearRightWheelCollider.brakeTorque = currentbrakeForce;
    }
    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }
    */

    // =====*=====*=====*=====*==========[ End of:   ][ Old Functions ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Initializer Functions ]=====*=====*=====*=====*==========

    private void GetAllComponents()
    {
        // Find the InputManager component on the GameObject
        inputManager = GetComponent<InputManager1>();
        if (inputManager == null) // In case it's not on the same GameObject
        {
            Debug.LogError($"{gameObject.name}: CarController: InputManager component not found!");
        }

        // Find the Rigidbody component
        carRigidbody = gameObject.GetComponent<Rigidbody>();
        if(carRigidbody == null)
        {
            Debug.LogError($"{gameObject.name}: CarController: Rigidbody component not found!");
        }
    }

    private void InitializeWheelConfiguration()
    {
        // Initialize wheel colliders
        wheelColliders[0] = frontLeftWheelCollider;
        wheelColliders[1] = frontRightWheelCollider;
        wheelColliders[2] = rearLeftWheelCollider;
        wheelColliders[3] = rearRightWheelCollider;

        // Initialize wheel transforms
        wheelTransforms[0] = frontLeftWheelTransform;
        wheelTransforms[1] = frontRightWheelTransform;
        wheelTransforms[2] = rearLeftWheelTransform;
        wheelTransforms[3] = rearRightWheelTransform;

        switch (driveType)
        {
            case DriveType.FourWheelDrive:
                torqueWheels = wheelColliders; // All wheels receive torque
                brakeWheels = wheelColliders;  // All wheels receive brake force
                break;
            case DriveType.RearWheelDrive:
                torqueWheels = new WheelCollider[] { rearLeftWheelCollider, rearRightWheelCollider };
                brakeWheels = wheelColliders; // Assuming all wheels should brake
                break;
            case DriveType.FrontWheelDrive:
                torqueWheels = new WheelCollider[] { frontLeftWheelCollider, frontRightWheelCollider };
                brakeWheels = wheelColliders; // Assuming all wheels should brake
                break;
        }

        handBrakeWheels = new WheelCollider[] { rearLeftWheelCollider, rearRightWheelCollider };

        // Initialize and pre-calculate values for wheel friction calculation
        InitializeFrictionValues();

        foreach (WheelCollider wheel in wheelColliders)
        {
            wheel.forwardFriction = normalForwardFriction;
            wheel.sidewaysFriction = normalSidewaysFriction;
        }
    }
    private void InitializeFrictionValues()
    {
        // Pre-calculate the stiffness values for handbraking at the start
        handBrakeForwardStiffness = forwardStiffness * handBrakeForwardStiffnessMultiplier;
        handBrakeSidewaysStiffness = sidewaysStiffness * handBrakeSidewaysStiffnessMultiplier;

        // Initialize default friction curves
        normalForwardFriction = new WheelFrictionCurve
        {
            extremumSlip = 0.4f,
            extremumValue = 1f,
            asymptoteSlip = 0.8f,
            asymptoteValue = 0.5f,
            stiffness = forwardStiffness
        };
        normalSidewaysFriction = new WheelFrictionCurve
        {
            extremumSlip = 0.2f,
            extremumValue = 1f,
            asymptoteSlip = 0.5f,
            asymptoteValue = 0.75f,
            stiffness = sidewaysStiffness
        };

        // Handbraking conditions
        handBrakeForwardFriction = new WheelFrictionCurve
        {
            extremumSlip = normalForwardFriction.extremumSlip,
            extremumValue = normalForwardFriction.extremumValue,
            asymptoteSlip = normalForwardFriction.asymptoteSlip,
            asymptoteValue = normalForwardFriction.asymptoteValue,
            stiffness = forwardStiffness * handBrakeForwardStiffnessMultiplier
        };
        handBrakeSidewaysFriction = new WheelFrictionCurve
        {
            extremumSlip = normalSidewaysFriction.extremumSlip,
            extremumValue = normalSidewaysFriction.extremumValue,
            asymptoteSlip = normalSidewaysFriction.asymptoteSlip,
            asymptoteValue = normalSidewaysFriction.asymptoteValue,
            stiffness = sidewaysStiffness * handBrakeSidewaysStiffnessMultiplier
        };
    }

    public void ChangeToSlipperyFriction()
    {
        handBrakeForwardStiffnessMultiplier = 0.3f;
        handBrakeSidewaysStiffnessMultiplier = 0.3f;

        handBrakeForwardStiffness = forwardStiffness * handBrakeForwardStiffnessMultiplier;
        handBrakeSidewaysStiffness = sidewaysStiffness * handBrakeSidewaysStiffnessMultiplier;
        handBrakeForwardFriction = new WheelFrictionCurve
        {
            extremumSlip = normalForwardFriction.extremumSlip,
            extremumValue = normalForwardFriction.extremumValue,
            asymptoteSlip = normalForwardFriction.asymptoteSlip,
            asymptoteValue = normalForwardFriction.asymptoteValue,
            stiffness = forwardStiffness * handBrakeForwardStiffnessMultiplier
        };
        handBrakeSidewaysFriction = new WheelFrictionCurve
        {
            extremumSlip = normalSidewaysFriction.extremumSlip,
            extremumValue = normalSidewaysFriction.extremumValue,
            asymptoteSlip = normalSidewaysFriction.asymptoteSlip,
            asymptoteValue = normalSidewaysFriction.asymptoteValue,
            stiffness = sidewaysStiffness * handBrakeSidewaysStiffnessMultiplier
        };
    }

    public void RevertToInitialFriction()
    {
        handBrakeForwardStiffnessMultiplier = 0.55f;
        handBrakeSidewaysStiffnessMultiplier = 0.55f;

        handBrakeForwardStiffness = forwardStiffness * handBrakeForwardStiffnessMultiplier;
        handBrakeSidewaysStiffness = sidewaysStiffness * handBrakeSidewaysStiffnessMultiplier;
        handBrakeForwardFriction = new WheelFrictionCurve
        {
            extremumSlip = normalForwardFriction.extremumSlip,
            extremumValue = normalForwardFriction.extremumValue,
            asymptoteSlip = normalForwardFriction.asymptoteSlip,
            asymptoteValue = normalForwardFriction.asymptoteValue,
            stiffness = forwardStiffness * handBrakeForwardStiffnessMultiplier
        };
        handBrakeSidewaysFriction = new WheelFrictionCurve
        {
            extremumSlip = normalSidewaysFriction.extremumSlip,
            extremumValue = normalSidewaysFriction.extremumValue,
            asymptoteSlip = normalSidewaysFriction.asymptoteSlip,
            asymptoteValue = normalSidewaysFriction.asymptoteValue,
            stiffness = sidewaysStiffness * handBrakeSidewaysStiffnessMultiplier
        };
    }

    // ----- Center of Mass adjustment
    // puts the center of mass at the height of the wheels
    private void ChangeCenterOfMass()
    {
        // adjusting the center of mass of the car.
        // lower to prevent flipping.s
        // might cause swinging like a pendulum.

        // old version:
        //gameObject.GetComponent<Rigidbody>().centerOfMass += centerOfMassOffset; // this offset was -1 on y-axis (vertical). but this could put the center of mass below the vehicle, which causes the mentioned pendulum swinging and my have further weird side-effects.

        float yMean = (frontLeftWheelTransform.position.y + frontRightWheelTransform.position.y + rearLeftWheelTransform.position.y + rearRightWheelTransform.position.y) / 4;
        float yOffset = yMean - carRigidbody.centerOfMass.y;
        centerOfMassOffset = new Vector3(0f, yOffset, 0f);
        carRigidbody.centerOfMass += centerOfMassOffset;
    }

    private void InitializeSteeringWheel()
    {
        // Memorize default steering wheel position
        initialSteeringWheelRotation = steeringWheelTransform.localRotation;
    }

    // =====*=====*=====*=====*==========[ End of:   ][ Initializer Functions ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Core Functions ]=====*=====*=====*=====*==========

    private void GetInput()
    {
        // Get raw input from Input Manager
        verticalInput = inputManager.vertical;
        horizontalInput = inputManager.horizontal;
        handbrakeInput = inputManager.handbrake;
        boostInput = inputManager.boosting;

        // Extrapolate raw input
        // This is to allow for treating the variables differently in the logic, because the input doesn't necessarily reflect what the vehicle actually CAN do. e.g.: handbrake in mid-air
        // For now steering and acceleration/deceleration is treated within the functions.
        isHandBraking = handbrakeInput;
        isSpeedBoosting = boostInput;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform, Quaternion.Euler(0f, 180f, 0f));
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform, Quaternion.identity);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform, Quaternion.Euler(0f, 180f, 0f));
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform, Quaternion.identity);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform, Quaternion rotationOffset)
    {
        Vector3 pos;
        Quaternion rot;       
        wheelCollider.GetWorldPose(out pos, out rot);
        rot *= rotationOffset;
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void UpdateSteeringWheel()
    {
        float normalizedSteerAngle = currentSteerAngle / maxSteerAngle;
        float steeringWheelRotation = normalizedSteerAngle * maxSteeringWheelRotation;

        Quaternion steeringWheelRotationQuat = initialSteeringWheelRotation * Quaternion.Euler(0f, 0f, -steeringWheelRotation);
        steeringWheelTransform.localRotation = steeringWheelRotationQuat;
    }

    // =====*=====*=====*=====*==========[ End of:   ][ Core Functions ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Private Functions ]=====*=====*=====*=====*==========

    // ----- Ackerman Steering
    private void SteerVehicle()
    {
        // Ackermann steering formula:,
        // steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;
        if (horizontalInput > 0)
        {
            //rear tracks size is set to 1.5f       wheel base has been set to 2.55f
            frontRightWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (turningRadius - (1.5f / 2))) * horizontalInput; //right
            frontLeftWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (turningRadius + (1.5f / 2))) * horizontalInput; //left
        }
        else if (horizontalInput < 0)
        {
            frontRightWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (turningRadius + (1.5f / 2))) * horizontalInput;//right
            frontLeftWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (turningRadius - (1.5f / 2))) * horizontalInput;//left
            //transform.Rotate(Vector3.up * steerHelping);

        }
        else
        {
            frontLeftWheelCollider.steerAngle = 0;
            frontRightWheelCollider.steerAngle = 0;
        }
    }
    private IEnumerator TimedLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(.7f);
            turningRadius = 6 + KPH / 20;

        }
    }

    // ----- Engine Power/RPM calculations + braking and actual movement
    private void CalculateCarPhysics()
    {
        WheelRPM();
        CalculateEnginePower();
        MoveVehicle();
        UpdateGearShift();
    }
    private void CalculateEnginePower()
    {
        carRigidbody.drag = (verticalInput != 0) ? 0.005f : 0.1f; // car drags less forward/backward than sideways
        totalEngineTorque = 3.6f * enginePowerCurve.Evaluate(currentEngineRPM) * (verticalInput);

        float velocity = 0.0f;
        if (currentEngineRPM >= maxRPM || isNearMaxRPM)
        {
            currentEngineRPM = Mathf.SmoothDamp(currentEngineRPM, maxRPM - 500, ref velocity, 0.05f);
            isNearMaxRPM = currentEngineRPM >= maxRPM - 450;
        }
        else
        {
            currentEngineRPM = Mathf.SmoothDamp(currentEngineRPM, 1000 + (Mathf.Abs(wheelsRPM) * 3.6f * gearRatios[currentGear]), ref velocity, smoothTime);
            isNearMaxRPM = false;
        }
        currentEngineRPM = Mathf.Clamp(currentEngineRPM, 0, maxRPM + 1000);
    }
    private void WheelRPM() // ok, maybe remove last line
    {
        float sum = 0;
        int count = 0;
        foreach (WheelCollider wheel in wheelColliders)
        {
            sum += wheel.rpm;
            count++;
        }
        wheelsRPM = (count > 0) ? sum / count : 0;

        reverse = (wheelsRPM < 0);
    }
    private void MoveVehicle()
    {
        ApplyTorque();
        // Apply braking force to configured wheels
        foreach (WheelCollider wheel in brakeWheels)
        {
            wheel.brakeTorque = currentbrakeForce;
        }

        ApplyBraking();
        UpdateVehicleSpeed();
    }
    private void ApplyTorque()
    {
        torquePerWheel = totalEngineTorque / torqueWheels.Length;
        // Apply calculated engine torque to the appropriate wheels based on drive type
        foreach (WheelCollider wheel in torqueWheels)
        {
            wheel.motorTorque = torquePerWheel;
        }
    }
    private void ApplyBraking()
    {
        currentbrakeForce = CalculateBrakeForce();
        //FIX !! cant go backwards
        
        foreach (WheelCollider wheel in wheelColliders) // new ... check if makes sense
        {
            wheel.brakeTorque = currentbrakeForce;
        }
    }
    private float CalculateBrakeForce()
    {
        if (Mathf.Abs(verticalInput) < 0.1f)
        {
            // if not pressing gas pedal, either brake at low speeds or slowly decelerate while rolling
            return (KPH >= brakeThreshhold) ? brakeForce : rollDecay;
        }
        /*else if (verticalInput < 0) unnötig, so kann man gar nicht rückwarts
        {
            if(KPH > 0)
            {   
                // brake if moving forward
                return brakeForce;
            }
            else
            {
                // go backwards if standing still or already going backwards
                //reverse = true;
                return 0;
            }
        }*/
        else
        {
            return 0;
        }
    }
    private void UpdateVehicleSpeed()
    {
        KPH = carRigidbody.velocity.magnitude * 3.6f;
    }
    private void UpdateGearShift()  
    {
        if (!IsGroundedForShift())
        {
            return;
        }
        //automatic
        if (currentEngineRPM > maxRPM && currentGear < gearRatios.Length - 1 && !reverse && CheckGearShiftSpeed())
        {
            currentGear++;
            //if (gameObject.tag != "AI") manager.changeGear();     // TODO: AI
        }
        else if (currentEngineRPM < minRPM && currentGear > 0)
        {
            currentGear--;
            // if (gameObject.tag != "AI") manager.changeGear();    // TODO: AI
        }
    }
    private bool IsGroundedForShift()
    {
        return wheelColliders[0].isGrounded && wheelColliders[1].isGrounded && wheelColliders[2].isGrounded && wheelColliders[3].isGrounded;
    }
    private bool CheckGearShiftSpeed()
    {   // checks if the car is going fast enough to be worth shifting gears
        return KPH >= gearChangeSpeed[currentGear];
    }
    // ----- Friction Calculations
    
    private void AdjustTraction()
    {
        WheelHit hit;
        float sumSidewaysSlip = 0;
        float[] sidewaysSlip = new float[wheelColliders.Length];

        for (int i = 0; i < wheelColliders.Length; i++)
        {
            if (wheelColliders[i].GetGroundHit(out hit) && i >= 2)
            {
                // Adjust friction based on handbraking
                wheelColliders[i].forwardFriction = isHandBraking ? handBrakeForwardFriction : normalForwardFriction;
                wheelColliders[i].sidewaysFriction = isHandBraking ? handBrakeSidewaysFriction : normalSidewaysFriction;

                // Calculate slip sums only for the rear wheels
                sumSidewaysSlip += Mathf.Abs(hit.sidewaysSlip);
                sidewaysSlip[i] = Mathf.Abs(hit.sidewaysSlip);
            }
        }

        // Adjust the turning radius based on average slip of the rear wheels
        if (wheelColliders.Length > 2) // To avoid division by zero
        {
            float averageSidewaysSlip = sumSidewaysSlip / 2; // Assuming the rear wheels are at indices 2 and 3
            turningRadius = (KPH > 60) ? 4 + (averageSidewaysSlip * -25) + KPH / 8 : 4;
        }
    }

    // Add aerodynamic down force caused by the car's spoiler. This presses the vehicle on the ground depending how good the car's spoiler is at its job (downForceValue).
    private void AddDownForce()
    {
        carRigidbody.AddForce(-transform.up * Mathf.Abs(downForceValue * carRigidbody.velocity.magnitude));

        /*  more realistic, exponential curve - not tested and harder to calculate:
            Velocity Squared: The function uses Mathf.Pow(rigidbody.velocity.magnitude, 2), which calculates the square of the velocity. This better mimics the actual quadratic relationship between velocity and aerodynamic forces like downforce.
            Scaling Factor: The / 1000.0f is a scaling factor to keep the downforce values within a reasonable range for the game physics engine. This factor may need to be adjusted based on the specific game's speed units and how the physics are supposed to feel. It's important that this scaling factor is tuned to prevent excessive or insufficient downforce.
        */
        //carRigidbody.AddForce(-transform.up * Mathf.Abs(DownForceValue * Mathf.Pow(rigidbody.velocity.magnitude, 2) / 1000.0f));

    }

    // ----- Speed Boost



    // =====*=====*=====*=====*==========[ End of:   ][ Private Functions ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Public Functions ]=====*=====*=====*=====*==========

    // nobody here but us chickens

    // =====*=====*=====*=====*==========[ End of:   ][ Public Functions ]=====*=====*=====*=====*==========
}