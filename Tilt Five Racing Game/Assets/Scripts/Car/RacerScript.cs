using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RacerScript : MonoBehaviour
{
    public GameObject midTimeText;
    public GameObject endMenu;
    public Text finalTime;
    public EventSystem eventSystem;
    public GameObject buttonToSelect;

    public static string playerName;
    public float laptime;
    public bool raceFinished = false;

    private AudioSource audioSource;

    [SerializeField] private AudioClip engineStartSound;

    private bool TimerRunning = false;
    private bool startTimer = false;
    public bool checkPointPassed = false;
    public int levelNumber;

    public UnityEngine.UI.Text timer;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("PLAYERS NAME IS: " + playerName);
        audioSource = GetComponent<AudioSource>();
        // TODO: set name in main menu
    }

    // Update is called once per frame
    void Update()
    {
        if(!startTimer && (Input.GetAxis("Vertical") !=0))
        {
            Debug.Log("Key pressed, starting timer now");
            startTimer = true;
            TimerRunning = true;
            audioSource.PlayOneShot(engineStartSound);
        }
        if (startTimer&&TimerRunning)
        {
            //Time.deltaTime: how long it took to get to that frame
            laptime += Time.deltaTime;
            timer.text = "Time: " + laptime.ToString("F2") + " sec";
        }
        
    }

    private void FinishRace()
    {
        CarController carController = gameObject.GetComponent<CarController>();
        carController.EndGame();
        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(buttonToSelect, new BaseEventData(eventSystem));
            Debug.Log("Now selected " + buttonToSelect.name);
        }
        raceFinished = true;
        TimerRunning = false;
        finalTime.text = timer.text;
        endMenu.SetActive(true);
        Debug.Log("Adding to leaderboard with: "+playerName+" and "+laptime);
        Leaderboard.AddScore(playerName, laptime, levelNumber);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "CheckPoint LightRay Cube 2" && checkPointPassed)
        {
            FinishRace();
        }

        if(other.gameObject.name == "CheckPoint LightRay Cube 1")
        {
            midTimeText.GetComponent<Text>().text = timer.text;
            midTimeText.SetActive(true);
            StartCoroutine(SetInactiveAfterSeconds(midTimeText, 3));
            checkPointPassed = true;
        }
    }

    IEnumerator SetInactiveAfterSeconds(GameObject text, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        text.SetActive(false);
    }

}
