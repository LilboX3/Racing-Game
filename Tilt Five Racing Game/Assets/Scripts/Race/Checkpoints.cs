using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Checkpoints : MonoBehaviour
{
    public bool Checkpoint1 = true;
    public bool Checkpoint2 = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Progress")
        {
            if (Checkpoint1 == true)
            {
                SaveScript.ThisCheckpoint1 = SaveScript.GetLapTime();
                SaveScript.CheckpointPass1 = true;

            }
            if (Checkpoint2 == true)
            {
                SaveScript.ThisCheckpoint2 = SaveScript.GetLapTime();
                SaveScript.CheckpointPass2 = true;

            }

            StartCoroutine(CheckPointPassOff());
          

        }
    }
    IEnumerator CheckPointPassOff()
    {
        yield return new WaitForSeconds(1);
        SaveScript.CheckpointPass1 = false;
        SaveScript.CheckpointPass2 = false;
        
    }
}


