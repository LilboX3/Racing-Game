using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor.Build.Content;
#endif
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    internal enum DriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }

    [Header("Input Manager Script")]
    private InputManager1 inputManager;
    private Rigidbody carRigidbody;

    private const string HORIZONTAL = "Horizontal";
    private const string VERTICAL = "Vertical";

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentbreakForce;
    private float initialMotorForce;
    private bool isBreaking;

    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    [SerializeField] private Transform frontLeftWheelTransform;
    [SerializeField] private Transform frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform;
    [SerializeField] private Transform rearRightWheelTransform;

    [Header("Stats")]
    [SerializeField] private float health = 100f;
    private bool isImmune = false;  // Immunity flag
    private float immunityDuration = 0.5f;  // Immunity duration in seconds
    [SerializeField] private Text healthText;
    [SerializeField] private ParticleSystem smokeParticleSystem; // Reference to the smoke particle system
    [SerializeField] private ParticleSystem explosionParticleSystem; // Reference to the explosion particle system
    [SerializeField] private float healthThreshold = 50f; // Health threshold to enable isDamaged
    [SerializeField] private bool canDrive = true;
    [SerializeField] private bool isDamaged = false;
    [SerializeField] private Text countdownText; // Reference to the countdown text UI element

    //Lowered center of mass in rigidbody to prevent car from flipping
    private void Start()
    {
        inputManager = gameObject.GetComponent<InputManager1>();
        carRigidbody = gameObject.GetComponent<Rigidbody>();
        //Might cause car to behave like a pendulum?
        carRigidbody.centerOfMass += new Vector3(0, -1f, 0);
        initialMotorForce = motorForce;
    }

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        UpdateSpeed();
    }


    private void GetInput()
    {
        verticalInput = inputManager.vertical;
        horizontalInput = inputManager.horizontal;
        isBreaking = inputManager.handbrake;
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
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void UpdateSpeed()
    {
        if (isDamaged)
        {
            motorForce *= 0.5f;
        }
        else
        {
            motorForce = initialMotorForce;
        }
    }

    private void UpdateHealthText()
    {
        if (healthText != null) //TODO: setup health bar here
        {
            healthText.text = "HP: " + health.ToString("0") + "/100";
        }
    }

    private void UpdateParticles()
    {
        if (health < healthThreshold)
        {
            smokeParticleSystem.gameObject.SetActive(true);
        }
        else
        {
            smokeParticleSystem.gameObject.SetActive(false);
        }

        if (health <= 0)
        {
            explosionParticleSystem.gameObject.SetActive(true);
        }
        else
        {
            explosionParticleSystem.gameObject.SetActive(false);
        }
    }

    private IEnumerator ImmunityFrame()
    {
        isImmune = true;
        yield return new WaitForSeconds(immunityDuration);
        isImmune = false;
    }

    private void DisableCar()
    {
        carRigidbody.velocity = Vector3.zero;
        carRigidbody.angularVelocity = Vector3.zero;

        BreakWheel(frontLeftWheelCollider);
        BreakWheel(frontRightWheelCollider);
        BreakWheel(rearLeftWheelCollider);
        BreakWheel(rearRightWheelCollider);
    }

    private void BreakWheel(WheelCollider wheelCollider)
    {
        wheelCollider.brakeTorque = float.MaxValue;
        wheelCollider.motorTorque = 0;
    }

    private IEnumerator OnDeath(float duration)
    {
        float remainingTime = duration;

        canDrive = false;
        DisableCar();
        countdownText.gameObject.SetActive(true);

        while (remainingTime > 0)
        {
            countdownText.text = $"Respawn in: {remainingTime.ToString("F1")}s";
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        countdownText.gameObject.SetActive(false);
        canDrive = true;
        health = 100f;
        isDamaged = false;
        UpdateHealthText();
        UpdateParticles();
    }

    // Method to apply damage to the car's health
    private void ApplyDamage(float damage)
    {
        health -= damage;
        health = Mathf.Max(health, 0);  // Ensure health doesn't go below 0

        Debug.Log($"Damage applied: {damage}. Current health: {health}");

        if (health < healthThreshold)
        {
            isDamaged = true;
        }

        if (health <= 0 && canDrive)
        {
            StartCoroutine(OnDeath(10f));
            Debug.Log("Car is destroyed!");
        }

        UpdateHealthText();
        UpdateParticles();
    }

    // Method to calculate damage based on speed
    private float CalculateDamage(float speed)
    {
        float maxDamage = 50f;
        float maxSpeed = 100f; // The speed at which maxDamage is applied
        float currentSpeed = speed;

        // Scale damage linearly with speed up to maxSpeed
        float damage = (currentSpeed / maxSpeed) * maxDamage;

        // Ensure the damage does not exceed maxDamage
        damage = Mathf.Min(damage, maxDamage);

        return damage;
    }

    public void OnCarCollision()
    {
        if (!isImmune)
        {
            StartCoroutine(ImmunityFrame());
            float velocity = carRigidbody.velocity.magnitude;
            ApplyDamage(CalculateDamage(velocity));
        }
    }


}