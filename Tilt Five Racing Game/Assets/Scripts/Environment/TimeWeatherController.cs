using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TimeWeatherController : MonoBehaviour
{
    public GameObject DayDome;
    public GameObject RainEffect;
    public GameObject FogObject;
    public GameObject Car1;
    public GameObject Car2;

    public bool isDay;
    public bool isRaining;
    public bool isFoggy;

    private CarController carController1;
    private CarController carController2;

    // Start is called before the first frame update
    void Start()
    {
        carController1 = Car1.GetComponent<CarController>();
        carController2 = Car2.GetComponent<CarController>();

        isDay = false;
        isRaining = true;
        isFoggy = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDay) DayDome.SetActive(true);
        else DayDome.SetActive(false);

        if (isRaining) SetRainEffect();
        else DisableRainEffect();

        if (isFoggy) StartFog();
        else StopFog();
    }


    private void SetRainEffect()
    {
        RainEffect.SetActive(true);

        carController1.ChangeToSlipperyFriction();
        carController2.ChangeToSlipperyFriction();
    }

    private void DisableRainEffect()
    {
        RainEffect.SetActive(false);

        carController1.RevertToInitialFriction();
        carController2.RevertToInitialFriction();

    }

    private void StartFog()
    {
         ParticleSystem fogParticles = FogObject.GetComponent<ParticleSystem>();
         fogParticles.Emit(1);
    }

    private void StopFog()
    {
        ParticleSystem fogParticles = FogObject.GetComponent<ParticleSystem>();
        fogParticles.Clear();
    }
}
