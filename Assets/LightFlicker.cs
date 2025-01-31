using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Light pointLight;
    private float targetIntensity;
    private float flickerSpeed = 10f; // 10 flickers per second

    void Start()
    {
        pointLight = GetComponent<Light>();
        targetIntensity = Random.Range(0f, 2f);
    }

    void Update()
    {
        // Smoothly interpolate to the target intensity
        pointLight.intensity = Mathf.Lerp(pointLight.intensity, targetIntensity, Time.deltaTime * flickerSpeed);

        // Randomly assign a new target intensity at intervals
        if (Time.frameCount % (Mathf.FloorToInt(60f / flickerSpeed)) == 0)
        {
            targetIntensity = Random.Range(0f, 2f);
        }
    }
}
