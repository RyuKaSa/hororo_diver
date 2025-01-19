using System.Collections.Generic;
using UnityHFSM;
using UnityEngine;
using UnityEngine.AI;

public sealed class BoidBehavior : MonoBehaviour, IDamageable
{
    public GameObject boidPrefab;
    public Transform mainCamera;
    public GameObject player;
    public int boidCount = 10;
    public float spawnRadius = 5f;
    public float speedRotation = 5f;
    public float targetRadius = 1f;
    public float damage = 1f;
    private List<Boid> boids;
    private Mob mob;
    private StateMachine<State> fsm = new StateMachine<State>();
    private float health = 10.0f;
    private float speed = 1.0f;
    private float visionRange = 30.0f;
    private float moveAreaRange = 30.0f; 
    private NavMeshAgent navMesh;

    enum State {
        IDLE,
        HUNTING,
        DEAD
    };

    void Start()
    {
        boids = new List<Boid>();
        mob = new Mob(navMesh, health, speed, visionRange, moveAreaRange, transform.position);
        var collider = GetComponent<CircleCollider2D>();
        if (collider != null) {
            collider.radius = targetRadius;
        }
        navMesh = GetComponent<NavMeshAgent>();
        mob.Start();

        generateBoids();

        fsm.AddState(State.IDLE, onLogic: state => mob.PassiveMobMovement());
        fsm.AddState(State.HUNTING, onLogic: state => navMesh.SetDestination(player.transform.position)); 

        fsm.AddTransition(State.IDLE, State.HUNTING, 
                        transition => mob.HandleStateBasedOnSight(player, transform.position) == Mob.State.HUNTING);
        
        fsm.AddTransition(State.HUNTING, State.IDLE, 
                        transition => mob.HandleStateBasedOnSight(player, transform.position) == Mob.State.PASSIVE);

        fsm.SetStartState(State.IDLE);
        fsm.Init(); 
    }

    void Update() {
        fsm.OnLogic();
    }

    public void Damage(float damage)
    {
        health -= damage;
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     Debug.Log("Collide with other obj");
    //     var damageable = other.gameObject.GetComponent<IDamageable>();
    //     if (damageable != null)
    //     {
    //         Debug.Log(transform.name + " inflicts damage to " + other.gameObject.name);
    //         damageable.Damage(damage);
    //     }
    //     else
    //     {
    //         Debug.Log("Interface IDamageable not found");
    //     }
    // }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("trigger stay");
        var damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Debug.Log(transform.name + " is damaging " + other.gameObject.name);
            damageable.Damage(damage * Time.deltaTime); // Apply damage over time
        }
        else
        {
            Debug.Log("Interface IDamageable not found");
        }
    }

    void generateBoids() {
        for (int i = 0; i < boidCount; i++)
        {
            Vector3 spawnPosition = transform.position + Random.insideUnitSphere * spawnRadius;
            GameObject boidObject = Instantiate(boidPrefab, spawnPosition, Quaternion.identity);
            Boid boid = boidObject.AddComponent<Boid>();

            boid.target = transform;
            boid.speed = speedRotation;
            boid.targetRadius = targetRadius;
            boid.mainCamera = mainCamera;
            boid.playerPos = player.transform;

            boids.Add(boid);
        }    
    }

    void OnDrawGizmos()
    {
        var collider = GetComponent<CircleCollider2D>();
        if (collider != null)
        {
            Gizmos.color = Color.green; // Set the gizmo color
            Vector3 position = transform.position + (Vector3)collider.offset; // Account for the collider offset
            Gizmos.DrawWireSphere(position, collider.radius); // Draw the collider radius
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
