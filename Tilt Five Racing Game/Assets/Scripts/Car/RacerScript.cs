using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RacerScript : MonoBehaviour
{
    public GameObject midTimeText;

    public static string playerName;
    public float laptime;
    public bool raceFinished = false;

    private bool TimerRunning = false;
    private bool startTimer = false;
    public bool checkPointPassed = false;
    public int levelNumber;

    public UnityEngine.UI.Text timer;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("PLAYERS NAME IS: " + playerName);

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
        raceFinished = true;
        TimerRunning = false;
        //leaderboard.SetActive(true);
        //TODO: show leaderboard, try again, main menu
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
