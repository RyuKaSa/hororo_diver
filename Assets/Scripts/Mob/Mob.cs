using UnityEngine;

public class Mob : MonoBehaviour
{

    public UnityEngine.AI.NavMeshAgent agent;

    private Vector3 spawnPoint;

    private float visionRange;

    private float speed;

    private float moveAreaRange = 20f;


    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
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

    public void Start()
    {
        spawnPoint = transform.position;
        Vector3 point;
        while (!RandomPoint(spawnPoint, moveAreaRange, out point))
        {
            ;
        }
        agent.destination = point;

    }

    void Update()
    {
        Vector3 point;
        // Ajouter une marge pour éviter que le bord de la capsule cogne les bords des murs
        if (agent.remainingDistance <= 1f && RandomPoint(spawnPoint, moveAreaRange, out point))
        {
            Debug.Log("Find random point");
            agent.destination = point;
        }
    }
}