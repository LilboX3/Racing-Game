using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveScript : MonoBehaviour
{
    public static float Speed;
    public static float TopSpeed;
    public static int Gear;
    public static int LapNumber;
    public static bool LapChange = false; 
    public static float _lapTime, _raceTime, _bestLapTime;
    public static float LastLapTime;
    public static float LastCheckpoint1=0;
    public static float LastCheckpoint2=0;
    public static float ThisCheckpoint1;
    public static float ThisCheckpoint2;
    public static bool CheckpointPass1 = false;
    public static bool CheckpointPass2 = false;
    public static bool Checkpoint1 = false;
    public static bool Checkpoint2 = false;
    public static bool OnTheRoad = true;
    public static bool OnTheTerrain = false;
    public static bool Rumble1 = false;
    public static bool Rumble2 = false;
    public static bool WrongWay = false;
    public static bool Racestart = false; 
    public static bool HalfWayActivated = true;
    public static bool WWTextReset = false;
    public static int GoldTargetTimeMinutes;
    public static int GoldTargetTimeSeconds;
    public static int SilverTargetTimeMinutes;
    public static int SilverTargetTimeSeconds;
    public static int BronzeTargetTimeMinutes;
    public static int BronzeTargetTimeSeconds;
    public static int MaxLaps;
    public static bool RaceFinished = false;
    public static bool Gold = false;
    public static bool Silver = false;
    public static bool Bronze = false;
    public static bool Fail= false;
    public static float PenaltySeconds = 0;
    public static bool BrakeSlide = false;
    public static bool Controller=false;
    public static bool Timetrial = true;
    public static bool NewLapRecord = false;

    // Start is called before the first frame update
    void Start()
    {
        MaxLaps = 3;
    }

    // Update is called once per frame
    void Update()
    {

        if (RaceFinished == false)
        {
            if (LapChange == true)
            {
                LapChange = false;
                _lapTime = 0;
                if (LapNumber == 0)
                {
                    _raceTime = 0;
                }

                
                if (LapNumber == 2)
                {
                    _bestLapTime = LastLapTime;
                    NewLapRecord = true;
                }
                if (LastLapTime < _bestLapTime)
                {
                    _bestLapTime = LastLapTime;
                    NewLapRecord = true;
                }
            }
            if (LapNumber > 0)
            {
                _raceTime = _raceTime + Time.deltaTime;
                _lapTime = _lapTime + Time.deltaTime;
            }

        }

    }
        public static float GetRaceTime()
        {
            return _raceTime;
        }

        public static float GetLapTime()
        {
            return _lapTime;
        }

        public static float GetBestLapTime()
        {
            return _bestLapTime;
        }
    
}
