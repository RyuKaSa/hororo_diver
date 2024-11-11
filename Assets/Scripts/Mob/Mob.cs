using UnityEngine;

public class Mob : MonoBehaviour
{

    public UnityEngine.AI.NavMeshAgent agent;

    private Vector3 spawnPoint;

    private float speed;

    [SerializeField] private float visionRange;

    [SerializeField] private float moveAreaRange = 20f;

    bool RandomPoint(out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = spawnPoint + Random.insideUnitSphere * moveAreaRange;
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, 1.0f, UnityEngine.AI.NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    /// <summary>
    /// Méthode utilisé pour faire se déplacer le mob lorsqu'il est en mode passif.
    /// Trouve un point aléatoire dans un rayon autour du mob et en fait sa destination.
    /// </summary>
    private void PassiveMobMovement()
    {
        Vector3 point;
        if (agent.remainingDistance <= 1f && RandomPoint(out point))
        {
            Debug.Log("Find random point");
            agent.destination = point;
        }
    }

    public void Start()
    {
        spawnPoint = transform.position;
        Vector3 point;
        while (!RandomPoint(out point))
        {
            ;
        }
        agent.destination = point;

    }

    void Update()
    {
        PassiveMobMovement();
    }
}