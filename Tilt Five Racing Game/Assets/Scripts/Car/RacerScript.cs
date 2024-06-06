using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class RacerScript : MonoBehaviour
{
    public GameObject leaderboard;

    public string playerName;
    public float laptime;
    public bool raceFinished = false;

    private bool TimerRunning = false;
    private bool startTimer = false;
    private bool checkPointPassed = false;
    private Leaderboard leaderboardScript;

    public UnityEngine.UI.Text timer;

    // Start is called before the first frame update
    void Start()
    {
        leaderboardScript = leaderboard.GetComponent<Leaderboard>();

        Debug.Log(MultiplayerValueController.player1Name);
        if (gameObject.name == "Drift Racer")
        {
            playerName = MultiplayerValueController.player1Name;
        }
        else if (gameObject.name == "Drift Racer (1)")
        {
            playerName = MultiplayerValueController.player2Name;
        }
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
        leaderboardScript.AddScore(playerName, laptime);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "CheckPoint LightRay Cube 2" && checkPointPassed)
        {
            FinishRace();
        }

        if(other.gameObject.name == "CheckPoint LightRay Cube 1")
        {
            checkPointPassed = true;
        }
    }

}
