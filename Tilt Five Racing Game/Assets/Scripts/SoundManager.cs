using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    // =//=//=//=//=//= Start of: SoundManager Class =//=//=//=//=//=

    // Singleton - one instance to rule them all
    static SoundManager soundManager;
    public static SoundManager Instance
    {
        get { return soundManager; }
    }

    [SerializeField] private AudioClip[] loadingScreenBGM;
    [SerializeField] private AudioClip[] mainMenuBGM;
    [SerializeField] private AudioClip[] workshopBGM;
    [SerializeField] private AudioClip[] raceBGM;
    private AudioClip[] currentBGM;

    private AudioSource menuSource;
    private AudioSource musicSource;

    [Header("Audio Mixer Groups")]
    [SerializeField] private AudioMixerGroup masterGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup sceneGroup;
    [SerializeField] private AudioMixerGroup ambientGroup;
    [SerializeField] private AudioMixerGroup menuGroup;
    [SerializeField] private AudioMixerGroup bgmGroup;

    [Header("Duration of BGM fade-in and fade-out")]
    [SerializeField] private float fadeInDuration = 2f;
    [SerializeField] private float fadeOutDuration = 2f;
    [SerializeField] private float transitionDuration;
    [SerializeField] private bool isTransitioningFollowupSong = false;

    private bool isTransitioning = false; // Flag to prevent overlapping transitions
    private Coroutine manageBGMCoroutine;

    // =====/////===== Start of: Unity Lifecycle Functions =====/////=====
    // Awake
    // Called when the script instance is being loaded.
    // Use it for initializing variables or setting up references before Start() is called.
    // It is not guaranteed to run before any other script's Awake()
    void Awake()
    {
        // Singleton SoundManager has to exist accross scenes.
        if (soundManager == null)
        {
            soundManager = this;
            DontDestroyOnLoad(gameObject); //makes sure the instance doesn't get removed during runtime
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        menuSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();

        menuSource.outputAudioMixerGroup = menuGroup;
        musicSource.outputAudioMixerGroup = bgmGroup;
    }

    // **************************************************
    //Start
    // Start is called before the first frame update
    // Use it for initializing game state or any setup that needs to happen once at the beginning.
    void Start()
    {
        // Subscribe the OnSceneLoaded method to the sceneLoaded event of the SceneManager.
        // This means that when a new scene is loaded, the OnSceneLoaded method will be automatically called.
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (isTransitioningFollowupSong)
        {
            transitionDuration = fadeInDuration + fadeOutDuration;
        }
        else
        {
            transitionDuration = fadeOutDuration;
        }

        RefreshAudioVolume();
        // Start playing main menu BGM by default
        StartBGM();
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
    // ========== Start of: Startup Functions ==========
    private void RefreshMasterVolume()
    {
        // Set the Master volume based on the GameManager's master volume
        masterGroup.audioMixer.SetFloat("MasterVolume", GameManager.Instance.MasterVolume);
    }
    private void RefreshSFXVolume()
    {
        // Set the SFX volume based on the GameManager's SFX volume
        sfxGroup.audioMixer.SetFloat("SFXVolume", GameManager.Instance.SfxVolume);
    }
    private void RefreshSceneVolume()
    {
        // Set the Scene volume based on the GameManager's Scene volume
        sceneGroup.audioMixer.SetFloat("SceneVolume", GameManager.Instance.SceneVolume);
    }
    private void RefreshAmbientVolume()
    {
        // Set the Ambient volume based on the GameManager's Ambient volume
        ambientGroup.audioMixer.SetFloat("AmbientVolume", GameManager.Instance.AmbientVolume);
    }
    private void RefreshMenuVolume()
    {
        //menuSource.volume = GameManager.Instance.MenuVolume; //old version

        // Set the Menu volume based on the GameManager's Menu volume
        menuGroup.audioMixer.SetFloat("MenuVolume", GameManager.Instance.MenuVolume);
    }
    private void RefreshMusicVolume()
    {
        //musicSource.volume = GameManager.Instance.BgmVolume; //old version

        // Set the Background Music volume based on the GameManager's Music volume
        bgmGroup.audioMixer.SetFloat("BGMVolume", GameManager.Instance.BgmVolume);

        // Debug version, comment out after done and reinstitute above version
        /*
        float targetVolume = GameManager.Instance.BgmVolume;
        Debug.Log($"Setting BGMVolume to: {targetVolume}");
        bgmGroup.audioMixer.SetFloat("BGMVolume", targetVolume);
        */
    }
    private void RefreshAudioVolume()
    {
        RefreshMasterVolume();
        RefreshSFXVolume();
        RefreshSceneVolume();
        RefreshAmbientVolume();
        RefreshMenuVolume();
        RefreshMusicVolume();
    }
    // ========== End of: Startup Functions ==========
    // ========== Start of: BGM Functions ==========
    private void SetCurrentBgmBySceneType()
    {
        switch (GameManager.Instance.currentScene)
        {
            case GameManager.SceneType.LoadingScreen:
                currentBGM = loadingScreenBGM;
                break;
            case GameManager.SceneType.MainMenu:
                currentBGM = mainMenuBGM;
                break;
            case GameManager.SceneType.Workshop:
                currentBGM = workshopBGM;
                break;
            case GameManager.SceneType.Race:
                currentBGM = raceBGM;
                break;
            default:
                currentBGM = mainMenuBGM;
                break;
        }
    }
    //---
    private IEnumerator FadeAudioSource(AudioSource audioSource, float startVolume, float targetVolume, float duration)
    {
        float currentTime = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / duration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }

    // Transitions from one music to another by making one quieter over time and then the new one louder over time.
    // TODO: check if we even have to make the new music louder over time, just playing it might work just fine since most songs have their own intro.
    private IEnumerator TransitionBGM(AudioClip oldClip, AudioClip newClip)
    {
        isTransitioning = true;

        // Fade Out old clip
        StartCoroutine(FadeAudioSource(musicSource, musicSource.volume, 0f, fadeOutDuration));
        // Wait until old clip is gone
        yield return new WaitForSeconds(fadeOutDuration);
        // Fade In new clip
        musicSource.clip = newClip;
        if (isTransitioningFollowupSong)
        {
            StartCoroutine(FadeAudioSource(musicSource, 0f, GameManager.Instance.BgmVolume, fadeInDuration));
        }
        else
        {
            RefreshMusicVolume();
        }
        musicSource.Play();

        isTransitioning = false;
    }
    //---
    private void StartBGM()
    {
        SetCurrentBgmBySceneType();
        if (currentBGM != null)
        {
            if (currentBGM.Length > 1)
            {
                // Play a random song from the playlist
                musicSource.clip = currentBGM[Random.Range(0, currentBGM.Length - 1)];
                musicSource.loop = false;
            }
            else
            {
                musicSource.clip = currentBGM[0];
                musicSource.loop = true;
            }

            musicSource.Play();
            RefreshAudioVolume();
        }

    }

    private void StopBGM()
    {
        // Interrupt all running music
        musicSource.Stop();
        musicSource.loop = false;
    }

    // This function is called when a music transition is already running but a new transition is already queued. Might be dangerous to chain them a lot.
    private IEnumerator WaitAndManageBGM()
    {
        yield return new WaitForSecondsRealtime(transitionDuration + 0.5f); // Adding a little extra time as safeguard, then trying again to transition
        ManageBGM();
    }

    private void ManageBGM()
    {
        // Avoid overlapping transitions
        if (isTransitioning)
        {
            StopCoroutine(soundManager.manageBGMCoroutine);  // Stop the ongoing coroutine
            manageBGMCoroutine = StartCoroutine(WaitAndManageBGM());
            return;
        }

        // Check which kind of BGM is supposed to play.
        SetCurrentBgmBySceneType();

        // switch to new music unless we have no music for the scene type
        if (currentBGM != null && currentBGM.Length > 0)
        {
            if (currentBGM.Length != 1 || currentBGM[0] != musicSource.clip)
            {
                StartCoroutine(TransitionBGM(musicSource.clip, currentBGM[Random.Range(0, currentBGM.Length - 1)]));
            }
        }
        else
        {
            StopBGM();
        }
    }

    // Event function to handle scene transitions.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ManageBGM();
    }

    // ========== End of: BGM Functions ==========
    // ========== Start of: Sound Effect Functions ==========

    // ========== End of: Sound Effect Functions ==========
    // =//=//=//=//=//= End of: SoundManager Class =//=//=//=//=//=
}
