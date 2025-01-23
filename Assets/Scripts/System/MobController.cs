using UnityEngine;
using System.Collections;

public class MobSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] mobs;

    [SerializeField]
    private Transform player;

    [SerializeField]
    private Camera mainCamera;

    [SerializeField]
    private int minSpawnTime = 40;

    [SerializeField]
    private int maxSpawnTime = 90;

    private void Start()
    {
        StartCoroutine(SpawnMob());
    }

    private IEnumerator SpawnMob()
    {
        while (true)
        {
            float spawnDelay = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(spawnDelay);

            Vector3 spawnPosition = GetRandomSpawnPosition();
            int mobIndex = Random.Range(0, mobs.Length);
            Debug.Log("Spawn mob");
            Instantiate(mobs[mobIndex], spawnPosition, Quaternion.identity);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float spawnDistance = 10f; // Distance from player
        Vector3 spawnDirection = Random.insideUnitCircle.normalized;

        Vector3 spawnPosition = player.position + new Vector3(spawnDirection.x, spawnDirection.y, 0) * spawnDistance;

        Vector3 cameraBounds = mainCamera.ViewportToWorldPoint(new Vector3(1.1f, 1.1f, mainCamera.nearClipPlane));
        spawnPosition.x = Mathf.Clamp(spawnPosition.x, -cameraBounds.x, cameraBounds.x);
        spawnPosition.y = Mathf.Clamp(spawnPosition.y, -cameraBounds.y, cameraBounds.y);

        return spawnPosition;
    }
}
