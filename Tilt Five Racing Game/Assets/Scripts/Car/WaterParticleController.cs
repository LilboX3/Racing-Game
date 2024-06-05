using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterParticleController : MonoBehaviour
{
    public GameObject weatherControllerObject;
    private ParticleSystem waterParticles;
    private TimeWeatherController weatherController;
    // Start is called before the first frame update
    void Start()
    {
        waterParticles = GetComponentInChildren<ParticleSystem>();
        weatherController = weatherControllerObject.GetComponent<TimeWeatherController> ();
    }

    // Update is called once per frame
    void Update()
    {
        if (weatherController.isRaining)
        {
            waterParticles.Play();
        } else
        {
            waterParticles.Stop();
        }
    }


}
