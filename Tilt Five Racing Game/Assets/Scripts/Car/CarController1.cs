using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor.Build.Content;
#endif
using UnityEngine;

public class CarController1 : MonoBehaviour
{
    private const string HORIZONTAL = "Horizontal2";
    private const string VERTICAL = "Vertical2";

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
    [SerializeField] private float maxSteeringWheelRotation = 135f;

    //Lowered center of mass in rigidbody to prevent car from flipping
    //Might cause car to behave like a pendulum? see Start()
    [Header("Rigidbody Center of Mass Offset")]
    [SerializeField] private Vector3 centerOfMassOffset = new Vector3(0f, -1f, 0f);

    [Header("Wheel Colliders")]
    [SerializeField] public WheelCollider frontRightWheelCollider;
    [SerializeField] public WheelCollider frontLeftWheelCollider;
    [SerializeField] public WheelCollider rearRightWheelCollider;
    [SerializeField] public WheelCollider rearLeftWheelCollider;
    
    [Header("Wheel Transforms")]
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;

    [Header("Steering Wheel")]
    [SerializeField] private Transform steeringWheelTransform;
    private Quaternion initialSteeringWheelRotation;

    //[Header("Car Lights and Emissions Script")]
    //[SerializeField] private CarLights carLights;
    //carLights.Car_Not_Running();
    //carLights.Car_Running();
    //carLights.Car_Breaking();

    private void Start()
    {
        // adjusting the center of mass of the car.
        // lower to prevent flipping.s
        // might cause swinging like a pendulum.
        gameObject.GetComponent<Rigidbody>().centerOfMass += centerOfMassOffset;

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
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform, Quaternion.identity);
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform, Quaternion.Euler(0f, 180f, 0f));
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform, Quaternion.identity);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform, Quaternion.Euler(0f, 180f, 0f));
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


}