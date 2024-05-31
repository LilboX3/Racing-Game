using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeWeatherController : MonoBehaviour
{
    public GameObject DayDome;
    public GameObject RainEffect;

    public bool isDay;
    public bool isRaining;

    public PhysicMaterial slipperyMaterial;
    public PhysicMaterial defaultMaterial;
    public Collider[] groundTiles;

    // Start is called before the first frame update
    void Start()
    {
        isDay = false;
        isRaining = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDay) DayDome.SetActive(true);
        else DayDome.SetActive(false);

        if (isRaining) SetRainEffect();
        else DisableRainEffect();
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
}
