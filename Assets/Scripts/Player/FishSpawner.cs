using UnityEngine;
using System.Collections.Generic;

public class FishSpawner : MonoBehaviour
{
    [Header("Solo Fish Settings")]
    [SerializeField] private List<GameObject> soloFishPrefabs; // Fish that spawn alone

    [Header("Group Fish Settings")]
    [SerializeField] private List<GameObject> groupFishPrefabs; // Fish that spawn in groups
    [SerializeField] private int minFishPerGroup = 3;
    [SerializeField] private int maxFishPerGroup = 6;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnWidth = 20f; // Width of the spawning square
    [SerializeField] private float spawnHeight = 10f; // Height of the spawning square
    [SerializeField] private float clusterRadius = 2f; // How tightly grouped the fish are
    [SerializeField] private float despawnDistance = 20f; // Distance at which fish despawn
    [SerializeField] private float spawnGroupsPerMinute = 10f; // Target groups per minute

    [Header("Depth Settings")]
    [SerializeField] private float depthOffsetRange = 1f; // Depth variation within a school
    private readonly float[] parallaxDepths = { -10f, 2.5f, 7.5f, 12.5f }; // Parallax depth levels

    [Header("Sorting Layers for Parallax Levels")]
    [SerializeField] private string sortLayerMinus10 = "Foreground"; 
    [SerializeField] private string sortLayer2_5 = "Background"; 
    [SerializeField] private string sortLayer7_5 = "Background"; 
    [SerializeField] private string sortLayer12_5 = "Background_Base"; 

    [Header("Fish Variance Settings")]
    [SerializeField] private float minFishSpeed = 1f;
    [SerializeField] private float maxFishSpeed = 3f;
    [SerializeField] private float minFishScale = 0.5f;
    [SerializeField] private float maxFishScale = 1.5f;

    private float spawnInterval; // Time between spawns

    private void Start()
    {
        spawnInterval = 60f / spawnGroupsPerMinute; // Calculate time interval per spawn
        InvokeRepeating(nameof(SpawnFish), 0f, spawnInterval);
    }

    private void SpawnFish()
    {
        bool spawnSolo = soloFishPrefabs.Count > 0 && (groupFishPrefabs.Count == 0 || Random.value > 0.5f);

        if (spawnSolo)
        {
            SpawnSoloFish();
        }
        else
        {
            SpawnFishGroup();
        }
    }

    private void SpawnSoloFish()
    {
        if (soloFishPrefabs.Count == 0) return;

        GameObject fishPrefab = soloFishPrefabs[Random.Range(0, soloFishPrefabs.Count)];
        Vector2 spawnPosition = GetRandomPointInSquare();
        Vector2 moveDirection = spawnPosition.x < transform.position.x ? Vector2.right : Vector2.left;
        float fishSpeed = Random.Range(minFishSpeed, maxFishSpeed);

        float depthLevel = parallaxDepths[Random.Range(0, parallaxDepths.Length)];
        string sortingLayer = GetSortingLayerForDepth(depthLevel);

        SpawnFish(fishPrefab, spawnPosition, moveDirection, fishSpeed, depthLevel, sortingLayer);
    }

    private void SpawnFishGroup()
    {
        if (groupFishPrefabs.Count == 0) return;

        GameObject fishPrefab = groupFishPrefabs[Random.Range(0, groupFishPrefabs.Count)];
        int fishCount = Random.Range(minFishPerGroup, maxFishPerGroup + 1);

        Vector2 groupSpawnCenter = GetRandomPointInSquare();
        Vector2 moveDirection = groupSpawnCenter.x < transform.position.x ? Vector2.right : Vector2.left;
        float groupSpeed = Random.Range(minFishSpeed, maxFishSpeed);

        float baseDepthLevel = parallaxDepths[Random.Range(0, parallaxDepths.Length)];
        string sortingLayer = GetSortingLayerForDepth(baseDepthLevel);

        for (int i = 0; i < fishCount; i++)
        {
            Vector2 spawnPosition = GetRandomPointInsideCluster(groupSpawnCenter);
            float depthOffset = Random.Range(-depthOffsetRange, depthOffsetRange);
            float finalDepth = baseDepthLevel + depthOffset;

            SpawnFish(fishPrefab, spawnPosition, moveDirection, groupSpeed, finalDepth, sortingLayer);
        }
    }

    private string GetSortingLayerForDepth(float depth)
    {
        if (depth == -5f) return sortLayerMinus10;
        if (depth == 1.25f) return sortLayer2_5;
        if (depth == 3.75f) return sortLayer7_5;
        if (depth == 6.25f) return sortLayer12_5;
        return sortLayer2_5; // Default to Background
    }

    private Vector2 GetRandomPointInSquare()
    {
        // Determine whether to spawn on the left or right edge
        bool spawnOnLeft = Random.value < 0.5f;

        // X position: If left, place at left edge; If right, place at right edge
        float x = spawnOnLeft ? transform.position.x - (spawnWidth / 2f) : transform.position.x + (spawnWidth / 2f);

        // Y position: Randomly within the spawn height
        float y = transform.position.y + Random.Range(-spawnHeight / 2f, spawnHeight / 2f);

        return new Vector2(x, y);
    }


    private Vector2 GetRandomPointInsideCluster(Vector2 groupCenter)
    {
        float x = groupCenter.x + Random.Range(-clusterRadius, clusterRadius);
        float y = groupCenter.y + Random.Range(-clusterRadius, clusterRadius);
        return new Vector2(x, y);
    }

    private void SpawnFish(GameObject fishPrefab, Vector2 spawnPosition, Vector2 moveDirection, float fishSpeed, float depth, string sortingLayer)
    {
        GameObject fish = Instantiate(fishPrefab, new Vector3(spawnPosition.x, spawnPosition.y, depth), Quaternion.identity);

        float fishScale = Random.Range(minFishScale, maxFishScale);
        fish.transform.localScale = new Vector3(fishScale, fishScale, 1f);

        // Assign sorting layer
        SpriteRenderer spriteRenderer = fish.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingLayerName = sortingLayer;
        }

        Fish fishScript = fish.AddComponent<Fish>();
        fishScript.SetMovement(moveDirection, fishSpeed, despawnDistance, transform);
    }
}
