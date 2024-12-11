using UnityEngine;

public sealed class CreeperMob : MonoBehaviour
{

    public UnityEngine.AI.NavMeshAgent agent;

    private Mob mob;
    private Vector3 spawnPoint;

    private float speed;

    private float health;

    private float visionRange = 20f;

    private float moveAreaRange = 20f;

    private float attackRange = 1.5f;

    private float timer = 0f;

    private float delay = 2f;

    private bool triggerExplosion = false;

    public void Start()
    {
        mob = new Mob(agent, health, speed, visionRange, moveAreaRange, transform.position);
        mob.InitMob();

    }

    /// <summary>
    /// This method calls different function which manage behavior according
    /// to Mob's state.
    /// </summary>
    private void BehaviorProcessBasedOnState(GameObject player, Mob.State state)
    {
        if (state == Mob.State.HUNTING)
        {
            agent.SetDestination(player.transform.position);
            AttackSequenceProcess(player);
        }
        else
        {
            mob.PassiveMobMovement();
        }
    }

    private void AttackSequenceProcess(GameObject player)
    {
        var MJDistance = Vector3.Distance(player.transform.position, transform.position);

        if (triggerExplosion)
        {
            timer += Time.deltaTime;
        }

        if (MJDistance <= attackRange + 0.5f)
        {
            triggerExplosion = true;
            agent.enabled = false;
        }

        if (timer >= delay)
        {
            Debug.Log("Explosion");
        }
    }

    void Update()
    {
        var player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.Log("PLAYER NULL");
        }
        else
        {
            var state = mob.HandleStateBasedOnSight(player, transform.position); // Update mob current state
            BehaviorProcessBasedOnState(player, state); // Determines which behavior algo choose according to mob's state
        }
    }

}