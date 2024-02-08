using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLights : MonoBehaviour
{
    // Car Mesh doesn't support this!
    /*
    [Header("Material Emissions")]
    [SerializeField] private Material emissiveMaterial;
    [SerializeField] private Renderer objectToChange;
    [SerializeField] private float breakColorIntensity = 5.0f;

    private Color offBreakColor = new Color(0f, 0f, 0f);
    private Color originalBreakColor;
    private Color activeBreakColor;
    */
    [Header("(Spot-)Lights")]
    [SerializeField] private Light headLightRight;
    [SerializeField] private Light headLightLeft;
    [SerializeField] private Light breakLightRight;
    [SerializeField] private Light breakLightLeft;

    private float originalHeadLightIntensity;
    private float originalBreakLightIntensity;

    // Start is called before the first frame update
    void Start()
    {
        // Car Mesh doesn't support this!
        /*
        // Save the renderer and the initial emission colors (for intensity scaling)
        emissiveMaterial = objectToChange.GetComponent<Renderer>().material;
        originalBreakColor = emissiveMaterial.GetColor("_EmissionColor");
        activeBreakColor = originalBreakColor * breakColorIntensity;
        */
        // Save initial light intensities
        originalHeadLightIntensity = headLightRight.intensity;
        originalBreakLightIntensity = breakLightRight.intensity;
    }

    // ---------- Emissions ----------
    /*
    private void TurnEmissionOff()
    {
        emissiveMaterial.DisableKeyword("_EMISSION");
    }
    private void TurnEmissionOn()
    {
        emissiveMaterial.EnableKeyword("_EMISSION");
    }
    */

    // Car Mesh doesn't support this!
    /*
    private void BreakColorOff()
    {
        emissiveMaterial.SetColor("_EmissionColor", offBreakColor);
    }
    private void BreakColorOn()
    {
        emissiveMaterial.SetColor("_EmissionColor", originalBreakColor);
    }
    private void BreakColorActive()
    {
        emissiveMaterial.SetColor("_EmissionColor", activeBreakColor);
    }
    */
    // ---------- Lights ----------
    
    public void Lights_Off()
    {
        headLightRight.enabled = false;
        headLightLeft.enabled = false;
        breakLightRight.enabled = false;
        breakLightLeft.enabled = false;
    }
    public void Lights_On()
    {
        headLightRight.enabled = true;
        headLightLeft.enabled = true;
        breakLightRight.enabled = true;
        breakLightLeft.enabled = true;
    }

    private void TurnHeadLightsOff()
    {
        headLightRight.intensity = 0f;
        headLightLeft.intensity = 0f;
    }

    private void TurnHeadLightsOn()
    {
        headLightRight.intensity = originalHeadLightIntensity;
        headLightLeft.intensity = originalHeadLightIntensity;
    }

    private void BreakLightsOff()
    {
        breakLightRight.intensity = 0f;
        breakLightLeft.intensity = 0f;
    }

    private void BreakLightsOn()
    {
        breakLightRight.intensity = originalBreakLightIntensity;
        breakLightLeft.intensity = originalBreakLightIntensity;
    }

    private void BreakLightsActive()
    {
        breakLightRight.intensity = originalBreakLightIntensity * 3;
        breakLightLeft.intensity = originalBreakLightIntensity * 3;
    }
    // ---------- Combined ----------

    public void Car_Not_Running()
    {
        //BreakColorOff();
        TurnHeadLightsOff();
        BreakLightsOff();
    }

    public void Car_Running()
    {
        //BreakColorOn();
        TurnHeadLightsOn();
        BreakLightsOn();
    }


    public void Car_Breaking()
    {
        //BreakColorActive();
        BreakLightsActive();
    }
}
