using System;
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
    private CarController1 carController2;
    private ChangePostProcessing postProcessingScript;

    // Start is called before the first frame update
    void Start()
    {
        carController1 = Car1.GetComponent<CarController>();
        carController2 = Car2.GetComponent<CarController1>();
        postProcessingScript = gameObject.GetComponent<ChangePostProcessing>();
        
        isDay = MultiplayerValueController.isDay;
        isRaining = MultiplayerValueController.isRaining;
        isFoggy = MultiplayerValueController.isFoggy;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDay) SetDay();
        else SetNight();

        if (isRaining) SetRainEffect();
        else DisableRainEffect();

        if (isFoggy) StartFog();
        else StopFog();
    }

    private void SetDay()
    {
        DayDome.SetActive(true);
        postProcessingScript.DisableNightEffect();
        postProcessingScript.SetDayEffect();
    }

    private void SetNight()
    {
        DayDome.SetActive(false);
        postProcessingScript.DisableDayEffect();
        postProcessingScript.SetNightEffect();
    }

    private void SetRainEffect()
    {
        RainEffect.SetActive(true);

        //TODO? carController1.ChangeToSlipperyFriction();
        carController2.ChangeToSlipperyFriction();
    }

    private void DisableRainEffect()
    {
        RainEffect.SetActive(false);

        //TODO? carController1.RevertToInitialFriction();
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
