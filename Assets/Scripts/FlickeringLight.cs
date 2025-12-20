using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    [Header("Flicker setting")]
    [SerializeField] private float minIntensity = 0.5f;
    [SerializeField] private float maxIntensity = 1.5f;
    [SerializeField] private float flickerSpeed = 10f;
    [SerializeField] private float flickerIntensity = 0.1f;

    [Header("Randomness setting")]
    [SerializeField] private bool randomizeSpeed = true;
    [SerializeField] private float speedMin = 8f;
    [SerializeField] private float speedMax = 12f;

    private Light pointLight;
    private float baseIntensity;
    private float randomOffset;

    void Start()
    {
        pointLight = GetComponent<Light>();
        baseIntensity = pointLight.intensity;
        randomOffset = Random.Range(0f, 100f);

        if (randomizeSpeed)
        {
            flickerSpeed = Random.Range(speedMin, speedMax);
        }
    }

    void Update()
    {
        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, randomOffset);
        float flicker = Mathf.Lerp(minIntensity, maxIntensity, noise);

        float randomFlicker = Random.Range(-flickerIntensity, flickerIntensity);

        pointLight.intensity = baseIntensity * (flicker + randomFlicker);
    }
}