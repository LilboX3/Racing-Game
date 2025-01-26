using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ●◆♦■▪▲▴△▼▾▽❖⚫⬤★
// Debug message hierarchy: Debug.Log < Debug.LogWarning < Debug.LogError

public class Car_SFX_Controller : MonoBehaviour
{
    // ██████████▽▽▽▽▽██████████ START OF: Variables                 ██████████▽▽▽▽▽██████████

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioClip collisionFrontSFX;   // Crash_Smash-Designed_Plastic_Crash_Designed_027
    [SerializeField] private AudioClip collisionSideSFX;    // Crash_Smash-Designed_Metal_Crash_Designed_011
    [SerializeField] private AudioClip driftSFX;
    [SerializeField] private AudioClip motorSFX;

    // ██████████△△△△△██████████  END  OF: Variables                 ██████████△△△△△██████████
    // ██████████▽▽▽▽▽██████████ START OF: Unity Lifecycle Functions ██████████▽▽▽▽▽██████████
    /* Awake
    // Called when the script instance is being loaded.
    // Use it for initializing variables or setting up references before Start() is called.
    // It is not guaranteed to run before any other script's Awake()
    void Awake()
    {

    }
    */
    // **************************************************
    // Start
    // Start is called before the first frame update
    // Use it for initializing game state or any setup that needs to happen once at the beginning.
    void Start()
    {
        // Try to get the AudioSource component
        audioSource = GetComponent<AudioSource>();

        // If AudioSource is not found, add a new one
        if (audioSource == null)
        {
            Debug.Log("Car_SFX_Controller on " + gameObject.name + ": Audio Source was not found, attempting to create a new one ...");
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("Car_SFX_Controller on " + gameObject.name + ": ... Audio Source was created.");
            if(audioSource == null)
            {
                Debug.LogWarning("Car_SFX_Controller on " + gameObject.name + ": AudioSource still does not exist in Car_SFX_Controller!");
            }
            else
            {
                // Configuring the properties of the existing AudioSource.

                // Allow multiple instances of the same sound to play simultaneously
                audioSource.playOnAwake = false;

                audioSource.spatialBlend = 1f; // 0f means 2D sound, 1f means 3D sound
                Debug.Log("Car_SFX_Controller on " + gameObject.name + ": Configured AudioSource properties.");
            }
        }

        
    }
    
    // **************************************************
    /* Update
    // Update is called once per frame
    // Used for regular updates such as input processing, game logic, and animations.
    // It's not tied to the frame rate and may vary based on the performance of the system.
    void Update()
    {
        
    }
    */
    // **************************************************
    /* FixedUpdate
    // Called at a fixed time interval (Physics updates).
    // Used for physics calculations or anything that needs to be updated in sync with the physics engine.
    // It's typically used for rigidbody physics.
    FixedUpdate()
    {

    }
    */
    // **************************************************
    /* LateUpdate
    // Called after all Update() functions have been called.
    // Useful for actions that need to happen after other updates, like camera movement or following other objects.
    LateUpdate()
    {

    }
    */
    // **************************************************
    /* OnEnable
    // Called when the object becomes enabled and active.
    // This is often used for setting up references or registering for events.
    OnEnable()
    {

    }
    */
    // **************************************************
    /* OnDisable
    // Called when the object becomes disabled.
    // Useful for cleaning up references or stopping any ongoing processes.
    OnDisable()
    {

    }
    */
    // **************************************************
    /* OnDestroy
    // Called when the script is being destroyed.
    // Used for releasing resources or cleaning up before the object is removed.
    OnDestroy()
    {

    }
    */
    // ██████████△△△△△██████████  END  OF: Unity Lifecycle Functions ██████████△△△△△██████████
    // ██████████▽▽▽▽▽██████████ START OF: Private Functions         ██████████▽▽▽▽▽██████████



    // ██████████△△△△△██████████  END  OF: Private Functions         ██████████△△△△△██████████
    // ██████████▽▽▽▽▽██████████ START OF: Public Functions          ██████████▽▽▽▽▽██████████
    public void PlayCollisionSound(Car_Collision_Controller.CollisionType collisionType)
    {
        // Adjust sound based on collision type
        switch (collisionType)
        {
            case Car_Collision_Controller.CollisionType.Front:
                audioSource.clip = collisionFrontSFX;
                break;

            case Car_Collision_Controller.CollisionType.Side:
                audioSource.clip = collisionSideSFX;
                break;

            // Add more cases for other collision types as needed

            default:
                Debug.LogWarning("Car_SFX-Controller on " + gameObject.name + ": Unknown collision type! Defaulted in PlayCollisionSound function switch case for collision type.");
                audioSource.clip = collisionFrontSFX;
                break;
        }
        // Play the assigned sound
        audioSource.loop = false;
        audioSource.Play();
    }

    public void PlayDriftSound()
    {

    }

    public void PlayMotorSound()
    {

    }
    // ██████████△△△△△██████████  END  OF: Public Functions          ██████████△△△△△██████████
}
