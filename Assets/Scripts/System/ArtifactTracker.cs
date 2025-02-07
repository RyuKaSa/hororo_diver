using UnityEngine;
using System.Collections.Generic;

public class ArtifactTracker : MonoBehaviour
{
    public List<Artifact> artifacts; // List of all artifacts in the game
    public Transform playerTransform;
    public float distanceThreshold = 10f; // Distance at which to notify the player
    public GameObject indicatorPrefab;
    public AudioClip sonarSound; // Sonar sound to play
    private GameObject currentIndicator;
    private AudioSource audioSource; // AudioSource for playing the sonar sound

    void Start()
    {
        // Find all artifacts in the scene at the start
        artifacts = new List<Artifact>(FindObjectsOfType<Artifact>());

        // Get the AudioSource component attached to the player (or another object)
        audioSource = playerTransform.GetComponent<AudioSource>();
    }

    void Update()
    {
        // Find the nearest artifact
        Artifact nearestArtifact = GetNearestArtifact();

        if (nearestArtifact != null)
        {
            Vector3 directionToArtifact = (nearestArtifact.transform.position - playerTransform.position).normalized;

            if (currentIndicator == null)
            {
                currentIndicator = Instantiate(indicatorPrefab, nearestArtifact.transform.position, Quaternion.identity);
            }
            currentIndicator.transform.position = nearestArtifact.transform.position + directionToArtifact * 2f; // Place indicator near the artifact
            currentIndicator.transform.LookAt(playerTransform.position); // Make the indicator point towards the player

            float distance = Vector3.Distance(playerTransform.position, nearestArtifact.transform.position);

            if (distance <= distanceThreshold)
            {
                // Trigger sonar sound when near an artifact
                PlaySonarSound(distance);

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
            audioSource.clip = sonarSound;
            audioSource.Play();
        }

        // Adjust pitch based on the distance (the closer the player is, the higher the pitch)
        float pitch = Mathf.Clamp(1.0f / (distance / distanceThreshold), 0.5f, 2f);
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
