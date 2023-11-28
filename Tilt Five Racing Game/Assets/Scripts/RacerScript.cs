using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacerScript : MonoBehaviour
{
    public float laptime;
    private bool TimerRunning = false;
    private bool startTimer = false;

    public UnityEngine.UI.Text timer;

    // Start is called before the first frame update
    void Start()
    {
        
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

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided!");
        if(other.tag == "Finish")
        {
            TimerRunning = false;
        }
        
    }
}
