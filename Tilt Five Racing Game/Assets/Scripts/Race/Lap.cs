using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lap : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (SaveScript.RaceFinished == false)
        {

            if (other.gameObject.CompareTag("Player"))
            {
                    SaveScript.WWTextReset = true;
                    StartCoroutine(WrongWayReset());
                if (SaveScript.RaceFinished == false)
                {
                    if (SaveScript.HalfWayActivated == true)
                    {
                        SaveScript.HalfWayActivated = false;
                        SaveScript.LastLapTime = SaveScript.GetLapTime();
                        SaveScript.LapNumber++;
                        SaveScript.LapChange = true;

                        SaveScript.CheckpointPass1 = false;
                        SaveScript.CheckpointPass2 = false;
                        SaveScript.LastCheckpoint1 = SaveScript.ThisCheckpoint1;
                        SaveScript.LastCheckpoint2 = SaveScript.ThisCheckpoint2;
                    }
                }
            }
        }

    }
IEnumerator WrongWayReset()
    {
        yield return new WaitForSeconds(1.5f);
        SaveScript.WWTextReset = false;
    }
}
