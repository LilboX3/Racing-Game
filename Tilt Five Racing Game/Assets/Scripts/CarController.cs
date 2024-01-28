using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Build.Content;
#endif
using UnityEngine;

public class CarController : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;

    [Header("Motor Settings")]
    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;

    [Header("Steering Settings")]
    [SerializeField] private float maxSteerAngle;

    //Lowered center of mass in rigidbody to prevent car from flipping
    //Might cause car to behave like a pendulum? see Start()
    [Header("Rigidbody Center of Mass Offset")]
    [SerializeField] private Vector3 centerOfMassOffset = new Vector3(0f, -1f, 0f);

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    
    [Header("Wheel Transforms")]
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;

    private void Start()
    {
        // adjusting the center of mass of the car.
        // lower to prevent flipping.
        // might cause swinging like a pendulum.
        gameObject.GetComponent<Rigidbody>().centerOfMass += centerOfMassOffset;
    }

    private void FixedUpdate() 
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();

    }


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
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform, false);
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform, true);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform, false);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform, true);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform, bool isLeftWheel)
    {
        Vector3 pos;
        Quaternion rot;       
        wheelCollider.GetWorldPose(out pos, out rot);
        if (isLeftWheel)
        {
            rot *= Quaternion.Euler(0f, 180f, 0f);
        }
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

}