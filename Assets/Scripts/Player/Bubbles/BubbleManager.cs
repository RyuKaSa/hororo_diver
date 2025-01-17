using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    [SerializeField] private GameObject bubblePrefab; // Assign in Inspector
    [SerializeField] private float minSpeed = 1f;
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float minLifetime = 2f;
    [SerializeField] private float maxLifetime = 5f;
    [SerializeField] private float horizontalRange = 0.5f; // Side movement range

    [Header("Breathing Settings")]
    [SerializeField] private float breathSpeed = 1f; // Controls the sine wave speed
    [SerializeField] private float spawnProbabilityScale = 0.1f; // Scale probability to avoid spawning every frame
    [SerializeField] private float spawnThreshold = 0f; // Minimum sine wave value needed to spawn

    private float timeElapsed;

    private void Update()
    {
        timeElapsed += Time.deltaTime * breathSpeed;
        float breathValue = Mathf.Sin(timeElapsed); // Generates sine wave (-1 to 1)

        if (breathValue > spawnThreshold) // Only spawn when breath value is above threshold
        {
            float spawnProbability = (breathValue - spawnThreshold) * spawnProbabilityScale; // Adjust probability
            if (Random.value < spawnProbability) // Random.value is between 0 and 1
            {
                SpawnBubble();
            }
        }
    }

    private void SpawnBubble()
    {
        if (bubblePrefab == null)
        {
            Debug.LogError("Bubble Prefab is missing in BubbleManager!");
            return;
        }

        GameObject bubble = Instantiate(bubblePrefab, transform.position, Quaternion.identity);
        Bubble bubbleScript = bubble.GetComponent<Bubble>();

        if (bubbleScript != null)
        {
            bubbleScript.speed = Random.Range(minSpeed, maxSpeed);
            bubbleScript.lifetime = Random.Range(minLifetime, maxLifetime);
            bubbleScript.horizontalOffset = Random.Range(-horizontalRange, horizontalRange);
        }
    }
}
