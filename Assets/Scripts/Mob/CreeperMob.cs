using UnityEngine;

public sealed class CreeperMob : MonoBehaviour, IDamageable
{

    public UnityEngine.AI.NavMeshAgent agent;

    [SerializeField]
    private ParticleSystem explosionParticle;

    private Mob mob;
    private Vector3 spawnPoint;

    [SerializeField]
    private ColoredFlash coloredFlash;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float health;

    [SerializeField]
    private float damage = 12f;

    [SerializeField]
    private float visionRange = 20f;

    [SerializeField]
    private float moveAreaRange = 20f;

    [SerializeField]
    private float attackRange = 1.5f;

    private float timer = 0f;

    private float delay = 2f;

    private bool triggerExplosion = false;

    private float explosionStartTime = 0f;

    private float currentTime = 0f;

    private float explosionRadius = 2f;

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

        if (timer >= delay && !explosionParticle.isPlaying)
        {
            explosionParticle.transform.position = transform.position;
            explosionParticle.Play();
            explosionStartTime = Time.deltaTime;
            currentTime = explosionStartTime;
            ManageExplosion(player);
            Destroy(GetComponent<Renderer>());
            Destroy(GetComponent<Rigidbody2D>());
            Destroy(GetComponent<BoxCollider2D>());

            // Destroy(GetComponent<UnityEngine.AI.NavMeshAgent>());
            // Destroy(gameObject);
        }
    }

    private void ManageExplosion(GameObject player)
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= explosionRadius)
        {
            player.GetComponent<Player>().Damage(damage);
        }

    }

    public void Damage(float damage)
    {
        Debug.Log(transform.name + " takes " + damage + " damage");
        health -= damage;
    }


    void Update()
    {

        if (explosionParticle.isPlaying)
        {
            currentTime += Time.deltaTime;
        }

        if (currentTime - explosionStartTime >= explosionParticle.main.duration)
        {
            Destroy(gameObject);
        }

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.Log("PLAYER NULL");
        }
        else
        {
            if (Vector3.Distance(transform.position, player.transform.position) >= 30f)
            {
                agent.enabled = false;
            }
            else
            {
                agent.enabled = true;
            }
            var state = mob.HandleStateBasedOnSight(player, transform.position); // Update mob current state
            BehaviorProcessBasedOnState(player, state); // Determines which behavior algo choose according to mob's state
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Knife") || other.gameObject.layer == LayerMask.NameToLayer("Projectile"))
        {
            Debug.Log("Receive damage");
            coloredFlash.Flash(Color.red);
        }

    }

}