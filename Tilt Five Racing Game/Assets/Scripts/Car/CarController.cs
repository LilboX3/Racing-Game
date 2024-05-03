using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Build.Content;
#endif
using UnityEngine;

public class CarController : MonoBehaviour
{
    // =====*=====*=====*=====*==========[ Start of: ][ Data Types ]=====*=====*=====*=====*==========
    internal enum DriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }
    // =====*=====*=====*=====*==========[ End of:   ][ Data Types ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Code helper ]=====*=====*=====*=====*==========
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";
    // =====*=====*=====*=====*==========[ End of:   ][ Code Helper ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Variable Declarations ]=====*=====*=====*=====*==========
    // ---------- References ----------
    [Header("Wheel Colliders")]
    [SerializeField] public WheelCollider frontLeftWheelCollider;
    [SerializeField] public WheelCollider frontRightWheelCollider;
    [SerializeField] public WheelCollider rearLeftWheelCollider;
    [SerializeField] public WheelCollider rearRightWheelCollider;
    private WheelCollider[] wheelColliders = new WheelCollider[4];
    private WheelCollider[] torqueWheels; // wheels that motor torque is applied to (acceleration)
    private WheelCollider[] brakeWheels; // wheels that brake force is applied to (braking)

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

    [Header("Input Axes")]
    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;

    //Lowered center of mass in rigidbody to prevent car from flipping
    //Might cause car to behave like a pendulum? see Start()
    [Header("Rigidbody Center of Mass Offset")]
    private Rigidbody carRigidbody;
    private Vector3 centerOfMassOffset;

    [Header("Motor")]
    [SerializeField] private float motorForce;  // the power the motor applies to the wheels // to be deleted for newer calculation
    public float maxRPM, minRPM;    // used to determine when to shift gears
    private float wheelsRPM;
    [SerializeField] private AnimationCurve enginePowerCurve;
    private float totalEnginePower;
    private float smoothTime = 0.09f;

    [Header("Gear")]
    [SerializeField] public float[] gearRatios;
    public float[] gearChangeSpeed; // used to stop gear shifting when midair
    private bool isNearMaxRPM;

    [Header("Brake")]
    [SerializeField] private float brakeForce;
    private float currentbrakeForce;
    private bool isBreaking;

    [Header("Steering")]
    [SerializeField] private float maxSteerAngle;
    private float turningRadius = 6;

    [Header("Steering Wheel (Cockpit)")]
    [SerializeField] private Transform steeringWheelTransform;
    private Quaternion initialSteeringWheelRotation;
    [SerializeField] private float maxSteeringWheelRotation = 135f;

    [Header ("Wheel Fiction Calculation")]
    public WheelFrictionCurve forwardFriction;
    public WheelFrictionCurve sidewaysFriction;
    public float handBrakeFrictionMultiplier = 2f;

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
        // adjusting the center of mass of the car.
        // lower to prevent flipping.
        // might cause swinging like a pendulum.
        //gameObject.GetComponent<Rigidbody>().centerOfMass += centerOfMassOffset;
        carRigidbody = gameObject.GetComponent<Rigidbody>();
        changeCenterOfMass();

        // Initialize the working wheel arrays for code streamlining
        InitializeWheelConfiguration();

        // Memorize default steering wheel position
        initialSteeringWheelRotation = steeringWheelTransform.localRotation;
    }

    private void FixedUpdate() 
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        UpdateSteeringWheel();
    }

    // =====*=====*=====*=====*==========[ End of:   ][ Unity Native Functions ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Old Functions ]=====*=====*=====*=====*==========
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

    // =====*=====*=====*=====*==========[ End of:   ][ Old Functions ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Core Functions ]=====*=====*=====*=====*==========



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

    // ----- Center of Mass adjustment
    // puts the center of mass at the height of the wheels
    private void changeCenterOfMass()
    {
        float yMean = (frontLeftWheelTransform.position.y + frontRightWheelTransform.position.y + rearLeftWheelTransform.position.y + rearRightWheelTransform.position.y) / 4;
        float yOffset = yMean - carRigidbody.centerOfMass.y;
        centerOfMassOffset = new Vector3(0f, yOffset, 0f);
        carRigidbody.centerOfMass += centerOfMassOffset;
    }

    // ----- Ackerman Steering
    private void steerVehicle()
    {
        // Ackermann steering formula:
        // steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;
        if (horizontalInput > 0)
        {
            //rear tracks size is set to 1.5f       wheel base has been set to 2.55f
            frontLeftWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (turningRadius + (1.5f / 2))) * horizontalInput; //left
            frontRightWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (turningRadius - (1.5f / 2))) * horizontalInput; //right
        }
        else if (horizontalInput < 0)
        {
            frontLeftWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (turningRadius - (1.5f / 2))) * horizontalInput;//left
            frontRightWheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (turningRadius + (1.5f / 2))) * horizontalInput;//right
            //transform.Rotate(Vector3.up * steerHelping);

        }
        else
        {
            frontLeftWheelCollider.steerAngle = 0;
            frontRightWheelCollider.steerAngle = 0;
        }
    }
    private IEnumerator timedLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(.7f);
            turningRadius = 6 + KPH / 20;

        }
    }



    // =====*=====*=====*=====*==========[ End of:   ][ Private Functions ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Support Functions ]=====*=====*=====*=====*==========

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
    }

    // =====*=====*=====*=====*==========[ End of:   ][ Support Functions ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ Public Functions ]=====*=====*=====*=====*==========



    // =====*=====*=====*=====*==========[ End of:   ][ Public Functions ]=====*=====*=====*=====*==========
    // =====*=====*=====*=====*==========[ Start of: ][ WIP Functions ]=====*=====*=====*=====*==========


    // ----- Engine Power/RPM calculations
    private void CalculateEnginePower()
    {
        WheelRPM();

        carRigidbody.drag = (verticalInput != 0) ? 0.005f : 0.1f; // car drags less forward/backward than sideways
        totalEnginePower = 3.6f * enginePowerCurve.Evaluate(currentEngineRPM) * (verticalInput);

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
        MoveVehicle();
        UpdateGearShift();
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
        ApplyBraking();
        // Apply torque to configured wheels
        foreach (WheelCollider wheel in torqueWheels)
        {
            wheel.motorTorque = totalEnginePower / torqueWheels.Length;
        }
        // Apply braking force to configured wheels
        foreach (WheelCollider wheel in brakeWheels)
        {
            wheel.brakeTorque = currentbrakeForce;
        }

        KPH = carRigidbody.velocity.magnitude * 3.6f;
    }
    private void ApplyBraking()
    {

        if (verticalInput < 0)
        {
            currentbrakeForce = (KPH >= 10) ? brakeForce : 0;
        }
        else if (verticalInput == 0 && (KPH <= 10 || KPH >= -10))
        {
            currentbrakeForce = 10;
        }
        else
        {
            currentbrakeForce = 0;
        }

        foreach (WheelCollider wheel in wheelColliders) // new ... check if makes sense
        {
            wheel.brakeTorque = currentbrakeForce;
        }
    }
    private void UpdateGearShift()  
    {
        if (!IsGrounded())
        {
            return;// ~ WIP ~
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
    private bool IsGrounded()
    {   
        // should change this or expand it
        return wheelColliders[0].isGrounded && wheelColliders[1].isGrounded && wheelColliders[2].isGrounded && wheelColliders[3].isGrounded;
    }
    private bool CheckGearShiftSpeed()
    {   // checks if the car is going fast enough to be worth shifting gears
        return KPH >= gearChangeSpeed[currentGear];
    }
    // ----- Friction Calculations


    // ----- Speed Boost


}