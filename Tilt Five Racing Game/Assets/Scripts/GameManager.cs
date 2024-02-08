using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Singleton
    static GameManager gameManager;
    public static GameManager Instance
    {
        get { return gameManager; }
    }

    [Header("Volume Settings for Audio Mixer Groups")]
    public float MasterVolume = 0f;
    public float SfxVolume = -5f;
    public float SceneVolume = -10f;
    public float AmbientVolume = -30f;
    public float MenuVolume = 0f;
    public float BgmVolume = -20f;

    // Keep track of the current scene by its type of scene
    public enum SceneType
    {
        LoadingScreen,
        MainMenu,
        Workshop,
        Race
    }
    [SerializeField] public SceneType currentScene = SceneType.LoadingScreen;

    // alternate variant if currentScene needs to be a property (copy paste this to find all references)
    //[SerializeField] private SceneType serializedCurrentScene = SceneType.LoadingScreen; // to set currentScene when starting right off a specific scene (for testing scenes)

    // =====/////===== Start of: Unity Lifecycle Functions =====/////=====
    // Awake
    // Called when the script instance is being loaded.
    // Use it for initializing variables or setting up references before Start() is called.
    // It is not guaranteed to run before any other script's Awake()
    void Awake()
    {
        // - start of singleton handling
        // There can only be one!
        if (gameManager != null && gameManager != this)
        {
            Destroy(gameObject);
            return;
        }

        gameManager = this;

        DontDestroyOnLoad(gameObject);
        // - end of singleton handling

        LoadSettings(); // always first! or else the game does random crap the user doesn't want, like blasting their eardrums with main menu turned up to max because default is 1f.
        //SaveSettings(); // here only for play testing default sound settings

        // alternate variant if currentScene needs to be a property (copy paste this to find all references)
        //currentScene = (SceneType)serializedCurrentScene;
    }
    // **************************************************
    /* Start
    // Start is called before the first frame update
    // Use it for initializing game state or any setup that needs to happen once at the beginning.
    void Start()
    {
        
        // Load other stuff

        // initialize current game

    }
    */
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
    void FixedUpdate()
    {

    }
    */
    // **************************************************
    /* LateUpdate
    // Called after all Update() functions have been called.
    // Useful for actions that need to happen after other updates, like camera movement or following other objects.
    void LateUpdate()
    {

    }
    */
    // **************************************************
    /* OnEnable
    // Called when the object becomes enabled and active.
    // This is often used for setting up references or registering for events.
    void OnEnable()
    {

    }
    */
    // **************************************************
    /* OnDisable
    // Called when the object becomes disabled.
    // Useful for cleaning up references or stopping any ongoing processes.
    void OnDisable()
    {

    }
    */
    // **************************************************
    /* OnDestroy
    // Called when the script is being destroyed.
    // Used for releasing resources or cleaning up before the object is removed.
    void OnDestroy()
    {

    }
    */
    // =====/////===== End of: Unity Lifecycle Functions =====/////=====
    // ========== Start of: Settings Handling ==========
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);
        PlayerPrefs.SetFloat("SfxVolume", SfxVolume);
        PlayerPrefs.SetFloat("SceneVolume", SceneVolume);
        PlayerPrefs.SetFloat("AmbientVolume", AmbientVolume);
        PlayerPrefs.SetFloat("MenuVolume", MenuVolume);
        PlayerPrefs.SetFloat("BgmVolume", BgmVolume);

        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        MasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        SfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.8f);
        SceneVolume = PlayerPrefs.GetFloat("SceneVolume", 0.7f);
        AmbientVolume = PlayerPrefs.GetFloat("AmbientVolume", 0.3f);
        MenuVolume = PlayerPrefs.GetFloat("MenuVolume", 1f);
        BgmVolume = PlayerPrefs.GetFloat("BgmVolume", 0.1f);
    }

    // ========== End of: Settings Handling ==========
    // ========== Start of: Getter / Setter / Etc. ==========
    
    // ========== End of: Getter / Setter / Etc. ==========
}
