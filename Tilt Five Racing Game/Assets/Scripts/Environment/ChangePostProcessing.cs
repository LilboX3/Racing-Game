using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChangePostProcessing : MonoBehaviour
{
    [SerializeField] private Volume volume;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDayEffect()
    {
        if(volume.profile.TryGet(out Bloom bloom))
        {
            bloom.intensity.value = 4.49f;
        }
    }
    public void DisableDayEffect()
    {
        if (volume.profile.TryGet(out Bloom bloom))
        {
            bloom.intensity.value = 0f;
        }
    }
    public void SetNightEffect()
    {
        if(volume.profile.TryGet(out ShadowsMidtonesHighlights shadowsMidtonesHighlights))
        {
            shadowsMidtonesHighlights.active = true;
        }
    }
    public void DisableNightEffect()
    {
        if (volume.profile.TryGet(out ShadowsMidtonesHighlights shadowsMidtonesHighlights))
        {
            shadowsMidtonesHighlights.active = false;
        }
    }
}
