using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScript : MonoBehaviour
{
    public Image SpeedRing;
    public Text SpeedText;
    public Text GearText;
    public Text LapNumberText;
    public Text TotalLapsText;
    public Text lapTimeText;
    public Text RaceTimeText;
    public Text BestLapTime;
    public Text CheckPointTime;
    public Text WrongWayT;
    public Text LapRecordT;
    public GameObject CheckPointDisplay;
    public GameObject WrongWayText;
    public GameObject LapRecordText;

    private int TotalLaps = 3;




    // Start is called before the first frame update
    void Start()
    {
        TotalLaps = SaveScript.MaxLaps;
        SpeedRing.fillAmount = 0;
        SpeedText.text = "0";
        GearText.text = "1";
        LapNumberText.text = "0";
        TotalLapsText.text = "/" + TotalLaps.ToString();
        lapTimeText.text = "00:00";
        RaceTimeText.text = "00:00";
        BestLapTime.text = "00:00";
        CheckPointDisplay.SetActive(false);
        WrongWayText.SetActive(false);
        LapRecordText.SetActive(false);
        Debug.Log(TotalLaps);


    }

    // Update is called once per frame
    void Update()
    {
        //filling the values of UI
        SpeedRing.fillAmount = SaveScript.Speed / SaveScript.TopSpeed;
        SpeedText.text = (Mathf.Round(SaveScript.Speed).ToString());
        GearText.text = (SaveScript.Gear + 1).ToString();
        LapNumberText.text = SaveScript.LapNumber.ToString();
        lapTimeText.text = $"{GetCurrentLapTimeInMinutes()}:{GetCurrentLapTimeInSeconds()}";
        RaceTimeText.text = $"{GetRaceTimeInMinutes()}:{GetRaceTimeInSeconds()}";
        BestLapTime.text = $"{GetBestLapTimeInMinutes()}:{GetBestLapTimeInSeconds()}";

        //Checkpoint working out for Checkpoint 1
        if (SaveScript.CheckpointPass1 == true)
        {
            SaveScript.Checkpoint1 = false;
            CheckPointDisplay.SetActive(true);
            if (SaveScript.ThisCheckpoint1 > SaveScript.LastCheckpoint1)
            {
                CheckPointTime.color = Color.red;
                if ((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) / 60 == 0)
                {
                    if (((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) % 60) < 10)
                    {
                        CheckPointTime.text = "-00:0" + ((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) % 60)).ToString() + "s";
                    }
                    else
                    {
                        CheckPointTime.text = "-00:" + ((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) % 60)).ToString() + "s";
                    }
                }
                else if ((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) / 60 < 10 && ((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) % 60) < 10)
                {
                    CheckPointTime.text = "-0" + ((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) / 60)).ToString() + ":0" + ((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) % 60)).ToString() + "s";
                }
                else if ((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) / 60 < 10)
                {
                    CheckPointTime.text = "-0" + ((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) / 60)).ToString() + ":" + ((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) % 60)).ToString() + "s";
                }
                else if (((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) % 60) < 10)
                {
                    CheckPointTime.text = "-" + ((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) / 60)).ToString() + ":0" + ((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) % 60)).ToString() + "s";
                }
                else
                {
                    CheckPointTime.text = "-" + ((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) / 60)).ToString() + ":" + ((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) % 60)).ToString() + "s";


                }
                StartCoroutine(CheckPointOff());
            }
            if (SaveScript.ThisCheckpoint1 < SaveScript.LastCheckpoint1)
            {
                CheckPointTime.color = Color.green;
                if ((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) / 60 == 0)
                {
                    if (((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) % 60) < 10)
                    {
                        CheckPointTime.text = "+00:0" + (Math.Abs((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) % 60))).ToString() + "s";
                    }
                    else
                    {
                        CheckPointTime.text = "+00:" + (Math.Abs((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) % 60))).ToString() + "s";
                    }
                }
                else if ((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) / 60 < 10 && ((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) % 60) < 10)
                {
                    CheckPointTime.text = "+0" + (Math.Abs((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) / 60))).ToString() + ":0" + (Math.Abs((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) % 60))).ToString() + "s";
                }
                else if ((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) / 60 < 10)
                {
                    CheckPointTime.text = "+0" + (Math.Abs((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) / 60))).ToString() + ":" + (Math.Abs((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) % 60))).ToString() + "s";
                }
                else if (((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) % 60) < 10)
                {
                    CheckPointTime.text = "+" + (Math.Abs((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) / 60))).ToString() + ":0" + (Math.Abs((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) % 60))).ToString() + "s";
                }
                else
                {
                    CheckPointTime.text = "+" + (Math.Abs((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) / 60))).ToString() + ":" + (Math.Abs((int)((SaveScript.ThisCheckpoint1 - SaveScript.LastCheckpoint1) % 60))).ToString() + "s";
                }
                StartCoroutine(CheckPointOff());
            }
        }
        //Checkpoint working out for Checkpoint 2
        if (SaveScript.CheckpointPass2 == true)
        {
            SaveScript.Checkpoint2 = false;
            CheckPointDisplay.SetActive(true);
            if (SaveScript.ThisCheckpoint2 > SaveScript.LastCheckpoint2)
            {
                CheckPointTime.color = Color.red;
                if ((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) / 60 == 0)
                {
                    if (((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) % 60) < 10)
                    {
                        CheckPointTime.text = "-00:0" + ((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) % 60)).ToString() + "s";
                    }
                    else
                    {
                        CheckPointTime.text = "-00:" + ((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) % 60)).ToString() + "s";
                    }
                }
                else if ((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) / 60 < 10 && ((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) % 60) < 10)
                {
                    CheckPointTime.text = "-0" + ((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) / 60)).ToString() + ":0" + ((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) % 60)).ToString() + "s";
                }
                else if ((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) / 60 < 10)
                {
                    CheckPointTime.text = "-0" + ((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) / 60)).ToString() + ":" + ((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) % 60)).ToString() + "s";
                }
                else if (((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) % 60) < 10)
                {
                    CheckPointTime.text = "-" + ((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) / 60)).ToString() + ":0" + ((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) % 60)).ToString() + "s";
                }
                else
                {
                    CheckPointTime.text = "-" + ((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) / 60)).ToString() + ":" + ((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) % 60)).ToString() + "s";
                }
                StartCoroutine(CheckPointOff());

            }
            if (SaveScript.ThisCheckpoint2 < SaveScript.LastCheckpoint2)
            {
                CheckPointTime.color = Color.green;
                if ((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) / 60 == 0)
                {
                    if (((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) % 60) < 10)
                    {
                        CheckPointTime.text = "+00:0" + (Math.Abs((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) % 60))).ToString() + "s";
                    }
                    else
                    {
                        CheckPointTime.text = "+00:" + (Math.Abs((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) % 60))).ToString() + "s";
                    }
                }
                else if ((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) / 60 < 10 && ((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) % 60) < 10)
                {
                    CheckPointTime.text = "+0" + (Math.Abs((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) / 60))).ToString() + ":0" + (Math.Abs((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) % 60))).ToString() + "s";
                }
                else if ((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) / 60 < 10)
                {
                    CheckPointTime.text = "+0" + (Math.Abs((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) / 60))).ToString() + ":" + (Math.Abs((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) % 60))).ToString() + "s";
                }
                else if (((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) % 60) < 10)
                {
                    CheckPointTime.text = "+" + (Math.Abs((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) / 60))).ToString() + ":0" + (Math.Abs((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) % 60))).ToString() + "s";
                }
                else
                {
                    CheckPointTime.text = "+" + (Math.Abs((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) / 60))).ToString() + ":" + (Math.Abs((int)((SaveScript.ThisCheckpoint2 - SaveScript.LastCheckpoint2) % 60))).ToString() + "s";
                }
                    StartCoroutine(CheckPointOff());
                }
            }
            //Wrong way message
            if (SaveScript.WrongWay == true)
            {
                WrongWayText.SetActive(true);
            }
            if (SaveScript.WrongWay == false)
            {
                WrongWayText.SetActive(false);
            }

            //WrongWay Reset Text
            if (SaveScript.WWTextReset == false)
            {
                WrongWayT.text = "WRONG WAY!";
            }
            if (SaveScript.WWTextReset == true)
            {
                WrongWayT.text = " ";
            }

            //Lap Record Text
            if (SaveScript.NewLapRecord == true)
            {
                LapRecordText.SetActive(true);
                StartCoroutine(LapRecordOff());
            }
    }
        string GetCurrentLapTimeInMinutes()
        {
            float lapTime = SaveScript.GetLapTime();
            return TimeSpan.FromSeconds(lapTime).ToString("mm");
        }
        string GetCurrentLapTimeInSeconds()
        {
            float lapTime = SaveScript.GetLapTime();
            return TimeSpan.FromSeconds(lapTime).ToString("ss");
        }

        string GetRaceTimeInMinutes()
        {
            float raceTime = SaveScript.GetRaceTime();
            return TimeSpan.FromSeconds(raceTime).ToString("mm");
        }
        string GetRaceTimeInSeconds()
        {
            float raceTime = SaveScript.GetRaceTime();
            return TimeSpan.FromSeconds(raceTime).ToString("ss");
        }
        string GetBestLapTimeInMinutes()
        {
            float bestLapTime = SaveScript.GetBestLapTime();
            return TimeSpan.FromSeconds(bestLapTime).ToString("mm");
        }
        string GetBestLapTimeInSeconds()
        {
            float bestLapTime = SaveScript.GetBestLapTime();
            return TimeSpan.FromSeconds(bestLapTime).ToString("ss");
        }
        IEnumerator CheckPointOff()
        {
            yield return new WaitForSeconds(2);
            CheckPointDisplay.SetActive(false);
        }
        IEnumerator LapRecordOff()
        {
            yield return new WaitForSeconds(2);
            SaveScript.NewLapRecord = false;
            LapRecordText.SetActive(false);
        }
}

