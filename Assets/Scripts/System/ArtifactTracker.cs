using UnityEngine;
using System.Collections.Generic;

public class ArtifactTracker : MonoBehaviour
{
    public List<Artifact> artifacts; // List of all artifacts in the game
    public Transform playerTransform;
    public float distanceThreshold = 1000f; // Distance at which to notify the player
    public GameObject indicatorPrefab;
    private GameObject currentIndicator;
    private AudioSource audioSource; // AudioSource for playing the sonar sound

    public float sonarCooldown = 3.7f; // Time in seconds between sonar sounds
    private float sonarCooldownTimer = 0f; 

    void Start()
    {
        // Find all artifacts in the scene at the start
        artifacts = new List<Artifact>(FindObjectsOfType<Artifact>());

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Find the nearest artifact
        Artifact nearestArtifact = GetNearestArtifact();

        sonarCooldownTimer += Time.deltaTime;

        if (nearestArtifact != null)
        {
            Vector3 directionToArtifact = (nearestArtifact.transform.position - playerTransform.position).normalized;

            float distance = Vector3.Distance(playerTransform.position, nearestArtifact.transform.position);
            if (distance <= distanceThreshold)
            {
                if (sonarCooldownTimer >= sonarCooldown){
                    PlaySonarSound(distance);
                    sonarCooldownTimer = 0f;
                }
                Debug.Log($"Artifact is within range: {distance} meters.");

            }
            else
            {
                // Stop the sonar sound if the player moves too far
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }
    }

    void PlaySonarSound(float distance)
    {
        // Play the sonar sound and adjust its pitch based on the proximity
        if (!audioSource.isPlaying)
        {
            Debug.Log("Audio is playing");
            audioSource.Play();
        }

        // Adjust pitch based on the distance
        float normalizedDistance = Mathf.InverseLerp(0f, distanceThreshold, distance); // Normalize distance between 0 and 1
        float volume = Mathf.Lerp(0.01f, 1f, 1 - normalizedDistance);
        float pitch = Mathf.Lerp(0.5f, 1f, 1 - normalizedDistance);

        audioSource.volume = volume;
        audioSource.pitch = pitch;
    }

    Artifact GetNearestArtifact()
    {
        Artifact nearestArtifact = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Artifact artifact in artifacts)
        {
            float distance = Vector3.Distance(playerTransform.position, artifact.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestArtifact = artifact;
            }
        }

        return nearestArtifact;
    }
}
