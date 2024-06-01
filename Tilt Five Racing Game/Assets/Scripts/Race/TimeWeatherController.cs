using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeWeatherController : MonoBehaviour
{
    public GameObject DayDome;
    public GameObject RainEffect;
    public GameObject FogObject;

    public bool isDay;
    public bool isRaining;
    public bool isFoggy;

    public PhysicMaterial slipperyMaterial;
    public PhysicMaterial defaultMaterial;
    public Collider[] groundTiles;

    // Start is called before the first frame update
    void Start()
    {
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
        foreach(Collider coll in groundTiles)
        {
            coll.material = slipperyMaterial;
        }
    }

    private void DisableRainEffect()
    {
        RainEffect.SetActive(false);
        foreach (Collider coll in groundTiles)
        {
            coll.material = defaultMaterial;
        }
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
