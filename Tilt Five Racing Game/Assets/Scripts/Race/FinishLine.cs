using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private string PlayerName;

    private void Start()
    {
        PlayerName = "Konrad";
    }
   void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
           SaveScript.RaceFinished = true;
            Time.timeScale = 0.2f;
        }
    }   
}
