using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipperyTile : MonoBehaviour
{
    private ParticleSystem waterParticles; 

    // Start is called before the first frame update
    void Start()
    {
        waterParticles = GetComponentInChildren<ParticleSystem>(); ;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered Trigger: " + other.name);
        if (other.tag == "Player")
        {
            waterParticles.Play();
            Debug.Log("on slippery ground");
        }
    }
}