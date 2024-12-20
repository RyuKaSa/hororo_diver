using UnityEngine;

public sealed class CreeperMob : MonoBehaviour, IDamageable
{

    public UnityEngine.AI.NavMeshAgent agent;

    [SerializeField]
    private ParticleSystem explosionParticle;

    private Mob mob;
    private Vector3 spawnPoint;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float health;

    [SerializeField]
    private float damage = 1f;

    [SerializeField]
    private float visionRange = 20f;

    [SerializeField]
    private float moveAreaRange = 20f;

    [SerializeField]
    private float attackRange = 1.5f;

    private float timer = 0f;

    private float delay = 2f;

    private bool triggerExplosion = false;

    public void Start()
    {
        mob = new Mob(agent, health, speed, visionRange, moveAreaRange, transform.position);
        mob.Start();

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
            explosionParticle.transform.position = transform.position;
            explosionParticle.Play();
            // Destroy(GetComponent<Renderer>());
            // Destroy(GetComponent<Transform>());
            // Destroy(GetComponent<UnityEngine.AI.NavMeshAgent>());
            Destroy(gameObject);
        }
    }

    public void Damage(float damage)
    {
        Debug.Log(transform.name + " takes " + damage + " damage");
        health -= damage;
    }


    void Update()
    {
        var player = GameObject.FindGameObjectsWithTag("Player")[0];
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