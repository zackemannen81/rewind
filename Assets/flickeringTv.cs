using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flickeringTv : MonoBehaviour
{
    [Header("Light Source")]
    public Light targetLight;  // Ljuskällan som ska styras

    [Header("Intensity Settings")]
    [Range(0f, 10f)] public float intensity = 1f;  // Standardintensitet
    public bool pulse = false;                     // Om ljuset ska pulsera
    public float pulseSpeed = 2f;                  // Hastighet för pulsering
    public float pulseAmplitude = 0.5f;            // Hur mycket ljuset varierar

    private float baseIntensity;

    void Start()
    {
        if (targetLight == null)
        {
            targetLight = GetComponent<Light>();
        }

        if (targetLight != null)
        {
            baseIntensity = targetLight.intensity;
        }
    }

    void Update()
    {
        if (targetLight == null) return;

        if (pulse)
        {
            // Enkel sinuspuls för mjuk variation i ljusstyrka
            targetLight.intensity = baseIntensity + Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
        }
        else
        {
            // Manuell kontroll via Inspector eller annan kod
            targetLight.intensity = intensity;
        }
    }

    // Exempel: justera ljuset från annan kod
    public void SetIntensity(float newValue)
    {
        intensity = Mathf.Clamp(newValue, 0f, 10f);
        if (!pulse && targetLight != null)
            targetLight.intensity = intensity;
    }
}
