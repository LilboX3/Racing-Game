using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RacerScript : MonoBehaviour
{
    public GameObject leaderboard;

    public string playerName;
    public float laptime;
    private bool TimerRunning = false;
    private bool startTimer = false;
    private Leaderboard leaderboardScript;

    public UnityEngine.UI.Text timer;

    // Start is called before the first frame update
    void Start()
    {
        leaderboardScript = leaderboard.GetComponent<Leaderboard>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!startTimer && (Input.GetAxis("Vertical")!=0))
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
            //Debug.Log(laptime);
        }
        
    }

    private void FinishRace()
    {
        TimerRunning = false;
        leaderboard.SetActive(true);
        leaderboardScript.AddScore(playerName, laptime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided!");
        if(other.tag == "Finish")
        {
            FinishRace();
        }
        
    }

}
