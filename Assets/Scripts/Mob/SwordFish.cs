using UnityEngine;
using UnityHFSM;
using System;



public sealed class SwordFish : MonoBehaviour, IDamageable
{
    enum SwordFishState
    {
        PASSIVE,
        HUNTING,
        CHARGE,
        RUSH,
        DEAD
    }

    public UnityEngine.AI.NavMeshAgent agent;

    private Vector3 spawnPoint;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float health;

    [SerializeField]
    private float damage = 1f;

    [SerializeField]
    private float visionRange = 15f;

    [SerializeField]
    private float moveAreaRange = 20f;

    [SerializeField]
    private float attackRange = 2f;

    [SerializeField]
    private float chargeTime = 2f;

    [SerializeField]
    private float chargeSpeedCoeff = 5f;

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private float timeBetween2Charge = 2f;

    private float timeToReachPlayer;

    private Vector3 lastPlayerPos;

    private Mob mob;

    private SwordFishState state = SwordFishState.PASSIVE;

    private StateMachine<SwordFishState> stateMachine = new StateMachine<SwordFishState>();


    void Start()
    {
        mob = new Mob(agent, health, speed, visionRange, moveAreaRange, transform.position);
        mob.Start();

        stateMachine.AddState(SwordFishState.PASSIVE, new State<SwordFishState>(
            onLogic: state => mob.PassiveMobMovement()
        ));

        stateMachine.AddState(SwordFishState.CHARGE, new State<SwordFishState>(
            onLogic: state => ChargeAttackProcess(state.timer.Elapsed),
            canExit: state => state.timer.Elapsed > chargeTime,
            needsExitTime: true
        ));

        stateMachine.AddState(SwordFishState.RUSH, new State<SwordFishState>(
            onLogic: state =>
            {
                float distanceToPlayer = Vector3.Distance(transform.position, lastPlayerPos);
                // Move the SwordFish towards the player if it is farther than 1 unit
                if (distanceToPlayer > 1.5)
                {
                    transform.position = RushTowardsPlayer(lastPlayerPos, transform.position, chargeSpeedCoeff, speed);
                }


            },
            canExit: state => Vector3.Distance(transform.position, lastPlayerPos) <= 1.5 && state.timer.Elapsed >= timeBetween2Charge,
            needsExitTime: true
        ));

        stateMachine.AddTransition(new Transition<SwordFishState>(
            SwordFishState.PASSIVE,
            SwordFishState.CHARGE,
            transition => mob.HandleStateBasedOnSight(player, transform.position) == Mob.State.HUNTING
        ));

        stateMachine.AddTransition(new Transition<SwordFishState>(
            SwordFishState.CHARGE,
            SwordFishState.PASSIVE,
            transition => mob.HandleStateBasedOnSight(player, transform.position) == Mob.State.PASSIVE
        ));

        stateMachine.AddTransition(new Transition<SwordFishState>(
            SwordFishState.CHARGE,
            SwordFishState.RUSH,
            transition => true
        ));

        stateMachine.AddTransition(new Transition<SwordFishState>(
            SwordFishState.RUSH,
            SwordFishState.CHARGE,
            transition => true
        ));


        stateMachine.Init();

    }

    /// <summary>
    /// Handles the logic when the sword fish charge his attck.
    /// This method calculates the necessary speed, rotation, and visual effects
    /// for the SwordFish before to charge towards the player
    /// </summary>
    /// <param name="elapsedTime">The elapsed time since the start of the charge state</param>
    private void ChargeAttackProcess(float elapsedTime)
    {
        // Disable the NavMeshAgent to stop random movement
        if (agent.enabled == true)
        {
            agent.enabled = false;
        }

        // saves the player's position up to a certain time
        if (elapsedTime <= chargeTime * 0.98)
        {
            lastPlayerPos = player.transform.position;

            float chargeSpeed = chargeSpeedCoeff * speed;
            float distanceToPlayer = Vector3.Distance(transform.position, lastPlayerPos);

            timeToReachPlayer = distanceToPlayer / chargeSpeed;
        }

        // Face to player during his charge
        Vector3 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.z = 0; // Ensure we remain on the 2D plane

        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        ShakeEffect(elapsedTime);
    }


    /// <summary>
    /// Moves the SwordFish towards the player's last known position at a certain charge speed.
    /// The SwordFish will continue moving towards the player until it is close enough.
    /// </summary>
    /// <param name="chargeSpeedCoeff">A coefficient that scales the base speed of the SwordFish's charge.</param>
    /// <param name="speed">The base speed of the SwordFish (before applying the charge speed coefficient).</param>
    // private void RushTowardsPlayer()
    // {
    //     // Calculate the direction from the SwordFish to the player's last known position
    //     Vector3 directionToPlayer = lastPlayerPos - transform.position;
    //     directionToPlayer.z = 0; // Ignore Z-axis
    //     directionToPlayer.Normalize();

    //     float chargeSpeed = chargeSpeedCoeff * speed;
    //     float distanceToPlayer = Vector3.Distance(transform.position, lastPlayerPos);

    //     // Move the SwordFish towards the player if it is farther than 1 unit
    //     if (distanceToPlayer > 1.5)
    //     {
    //         transform.position += directionToPlayer * chargeSpeed * Time.deltaTime;
    //     }
    // }

    /// <summary>
    /// Moves a mob towards the player's last known position at a certain charge speed.
    /// </summary>
    public static Vector3 RushTowardsPlayer(Vector3 targetPos, Vector3 sourcePos, float coeff, float speed)
    {
        // Calculate the direction from the SwordFish to the player's last known position
        Vector3 directionToPlayer = targetPos - sourcePos;
        directionToPlayer.z = 0; // Ignore Z-axis
        directionToPlayer.Normalize();

        float chargeSpeed = coeff * speed;
        float distanceToPlayer = Vector3.Distance(sourcePos, targetPos);

        return sourcePos + directionToPlayer * chargeSpeed * Time.deltaTime;
    }


    private void ShakeEffect(float time)
    {
        var shakeX = Mathf.Sin(Time.time * 10f) * 0.005 * time;
        var shakeY = Mathf.Cos(Time.time * 10f) * 0.005 * time;

        transform.position += new Vector3((float)shakeX, (float)shakeY, 0f);
    }

    public void Damage(float damage)
    {
        Debug.Log(transform.name + " takes " + damage + " damage");
        health -= damage;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collide with other obj");
        var damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Debug.Log(transform.name + " inflicts damage to " + other.gameObject.name);
            damageable.Damage(damage);
        }
        else
        {
            Debug.Log("Interface IDamageable not found");
        }
    }


    void Update()
    {
        stateMachine.OnLogic();

    }
}