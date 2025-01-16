using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BoidBehavior : MonoBehaviour
{
    public GameObject boidPrefab;
    public Transform mainCamera;
    public Transform playerPos;
    // public UnityEngine.AI.NavMeshAgent navMesh;
    public int boidCount = 10; // Number of boids to spawn
    public float spawnRadius = 5f; // Radius to spawn boids around the target
    public float speed = 5f; // Movement speed
    public float targetRadius = 1f; // Minimum distance to stay near the target
    private List<Boid> boids;
    // private Mob mob;

    void Start()
    {
        boids = new List<Boid>();
        // mob = new Mob(navMesh, 10.0f, 1.0f, 30.0f, 30.0f, transform.position);

        // Spawn boids
        for (int i = 0; i < boidCount; i++)
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
            GameObject boidObject = Instantiate(boidPrefab, spawnPosition, Quaternion.identity);
            Boid boid = boidObject.AddComponent<Boid>();

            boid.target = transform;
            boid.speed = speed;
            boid.targetRadius = targetRadius;
            boid.mainCamera = mainCamera;
            boid.playerPos = playerPos;

            boids.Add(boid);
        }
    }
}

public class Boid : MonoBehaviour
{
    public Transform target; // The object to orbit around
    public Transform mainCamera;
    public Transform playerPos;
    public float speed; // Movement speed
    public float targetRadius; // Minimum distance to stay near the target
    private SpriteRenderer sprite;

    public Vector3 velocity;

    void Start()
    {
        velocity = Random.insideUnitSphere * speed; // init random velocity
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector3 acceleration = OrbitAroundTarget();

        // Apply acceleration to velocity
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, speed); // clamp to limit speed

        
        if ((target.position - transform.position).magnitude > targetRadius)// boids stay within the target radius
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            velocity = Vector3.Lerp(velocity, directionToTarget * speed, Time.deltaTime * 2f);
        }

        // Update position
        transform.position += velocity * Time.deltaTime;

        // // Face direction of movement
        // if (velocity != Vector3.zero)
        //     transform.forward = velocity.normalized;

        // Face Camera
        transform.LookAt(mainCamera.position);
        if (playerPos.position.x < transform.position.x) sprite.flipX = true;
        else sprite.flipX = false;
    }

    Vector3 OrbitAroundTarget()
    {
        if (target == null) return Vector3.zero;

        Vector3 directionToTarget = target.position - transform.position;
        float distanceToTarget = directionToTarget.magnitude;

        if (distanceToTarget > targetRadius)
        {
            return directionToTarget.normalized; // Move closer if outside target radius
        }
        else
        {
            Vector3 randomAxis = Random.onUnitSphere; // Allow up-and-down variation
            Vector3 tangent = Vector3.Cross(directionToTarget, randomAxis).normalized;
            return tangent * speed; // Move tangentially for rotation
        }
    }
}
