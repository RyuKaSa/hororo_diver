using UnityEngine;

/// <summary>
/// Represents a generic mob character in the game. This class controls the mob's behavior, movement, and state transitions 
/// between passive, hunting, and dead states. The mob can randomly move around in a designated area, and 
/// chase the player when it detects them in its vision range. This class doesn't specifie pattern when the mob
/// is in hunting state that depends on class which encapsulate this class. 
/// </summary>
public sealed class Mob : MonoBehaviour
{

    public enum State
    {
        PASSIVE,
        HUNTING,
        DEAD
    }

    private UnityEngine.AI.NavMeshAgent agent;

    private Vector3 spawnPoint;

    private float speed;

    private float health;

    private State state = State.PASSIVE;

    private float visionRange;

    private float moveAreaRange = 20f;

    public Mob(UnityEngine.AI.NavMeshAgent agent, float health, float speed, float visionRange, float moveAreaRange, Vector3 spawnPoint)
    {
        this.agent = agent;
        this.health = health;
        this.speed = speed;
        this.visionRange = visionRange;
        this.moveAreaRange = moveAreaRange;
        this.spawnPoint = spawnPoint;
    }

    /// <summary>
    /// Tries to find a random point within the movement area around the spawn point.
    /// The point is returned through the <paramref name="result"/> parameter.
    /// </summary>
    /// <param name="result">The random position found within the allowed area (output parameter).</param>
    /// <returns>True if a valid random point is found, otherwise false.</returns>
    public bool RandomPoint(out Vector3 result)
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
    /// Moves the mob randomly within the designated area if it is in the passive state.
    /// The mob will randomly choose a destination point and move towards it.
    /// </summary>
    public void PassiveMobMovement()
    {
        Vector3 point;
        if (agent.enabled && state == State.PASSIVE && agent.remainingDistance <= 1f && RandomPoint(out point))
        {
            Debug.Log("Find random point");
            agent.destination = point;
        }
    }

    /// <summary>
    /// Reduces the mob's health by the specified damage amount.
    /// </summary>
    /// <param name="damage">The amount of damage to apply to the mob.</param>
    private void TakeDamage(float damage)
    {
        health -= damage;

    }


    /// <summary>
    /// Checks if the mob is dead based on its health.
    /// </summary>
    /// <returns>
    /// Returns true if the mob's health is less than or equal to zero, indicating that the mob is dead.
    /// Returns false if the mob's health is greater than zero, indicating that the mob is still alive.
    /// </returns>
    public bool IsDead()
    {
        return health <= 0;
    }

    /// <summary>
    /// Checks whether the player is within the mob's vision range.
    /// </summary>
    /// <param name="player">The GameObject representing the player.</param>
    /// <returns>True if the player is within the vision range, otherwise false.</returns>
    private bool IsPlayerInSight(GameObject player, Vector3 position)
    {
        return Vector3.Distance(player.transform.position, position) <= visionRange;

    }

    /// <summary>
    /// Updates the mob's state based on whether the player is within its vision range.
    /// If the player is in sight, the mob enters the "HUNTING" state. Otherwise, it enters the "PASSIVE" state.
    /// </summary>
    /// <param name="player">The GameObject representing the player.</param>
    /// <returns>The current state of the mob</returns>
    /// <remarks>
    /// This function calls <see cref="IsPlayerInSight"/> to determine whether the player is in the mob's vision range.
    /// If the player is detected, the mob switches to "HUNTING" state. If not, it reverts to "PASSIVE" state.
    /// </remarks>
    public State HandleStateBasedOnSight(GameObject player, Vector3 position)
    {
        state = IsPlayerInSight(player, position) ? State.HUNTING : State.PASSIVE;
        return state;
    }

    /// <summary>
    /// Sets the mob's state to "PASSIVE". 
    /// In this state, the mob moves randomly in the environment without actively pursuing the player.
    /// </summary>
    public void SetPassiveState()
    {
        state = State.PASSIVE;
    }

    /// <summary>
    /// Sets the mob's state to "HUNTING".
    /// In this state, the mob actively pursues the player or other targets.
    /// </summary>
    public void SetHuntingState()
    {
        state = State.HUNTING;
    }

    /// <summary>
    /// Sets the destination of the mob to the specified position.
    /// </summary>
    /// <param name="dest">The new destination for the mob to move towards.</param>
    public void SetMobAgentDestination(Vector3 dest)
    {
        agent.destination = dest;
    }

    public void Start()
    {
        if (agent != null)
        {
            Vector3 point;
            if (RandomPoint(out point))
            {
                agent.destination = point;
            }
        }
    }


}