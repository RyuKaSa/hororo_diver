using UnityEngine;

public sealed class BasicDistanceMob : MonoBehaviour, IDamageable
{

    public UnityEngine.AI.NavMeshAgent agent;

    public GameObject projectilePrefab;

    private Mob mob;
    private Vector3 spawnPoint;

    private float speed;

    private float health;

    [SerializeField]
    private float visionRange = 20f;

    [SerializeField]
    private float moveAreaRange = 20f;

    [SerializeField]
    private float attackRange = 15f;

    [SerializeField]
    private float waitingPeriod = 1.5f; // Time between 2 shoot

    private float timer = 0f;

    private bool isShooting = false;

    private bool isFleing = false;

    public void Start()
    {
        mob = new Mob(agent, health, speed, visionRange, moveAreaRange, transform.position);
        mob.Start();

    }

    /// <summary>
    /// Manages the escape behavior of a mob when it detects that the player is within attack range. 
    /// If the player is too close, the mob will flee to a location opposite to the player's position.
    /// If the player is far enough away, the mob will stop fleeing.
    /// </summary>
    /// <param name="player">The player GameObject whose position is used to determine the mob's escape behavior.</param>
    private void ManageEscapeBehaviorProcess(GameObject player)
    {
        // Checks if the mob has moved far enough away from the player
        if (isFleing && Vector3.Distance(player.transform.position, transform.position) >= attackRange * 0.5f)
        {
            isFleing = false;
            agent.SetDestination(transform.position);
            Debug.Log("Fuite finis");
            return;
        }

        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            isFleing = true;
            Debug.Log("Fuit Joueur");

            Vector3 fleeVector = player.transform.position - transform.position;
            Vector3 oppositeDirection = transform.position - fleeVector;
            agent.SetDestination(oppositeDirection);
            return;
        }

        Debug.Log("Aucun état actuellement isFleing = " + isFleing + " Vector3.Distance(player.transform.position, transform.position) = " + Vector3.Distance(player.transform.position, transform.position) + " attackRange = " + attackRange);
    }

    /// <summary>
    /// This method calls different function which manage behavior according
    /// to Mob's state.
    /// </summary>
    private void BehaviorProcessBasedOnState(GameObject player, Mob.State state)
    {
        if (state == Mob.State.HUNTING)
        {
            ManageEscapeBehaviorProcess(player);
            AttackSequenceProcess(player);

        }
        else
        {
            mob.PassiveMobMovement();
        }
    }

    private void AttackSequenceProcess(GameObject player)
    {
        if (timer >= waitingPeriod)
        {
            timer = 0f;
            isShooting = false;
        }

        // Mob can shoot Player
        if (!isShooting && timer <= waitingPeriod && Vector3.Distance(player.transform.position, transform.position) <= attackRange + 2f)
        {
            Debug.Log("Shoot !");
            GameObject projectileObject = Instantiate(projectilePrefab, transform.position, transform.rotation);
            Projectile projectileScript = projectileObject.GetComponent<Projectile>();

            projectileScript.Initialize(0.5f, 1f, false, player.transform.position - transform.position);
            isShooting = true;

            return;
        }
        timer += Time.deltaTime;
    }

    public void Damage(float damage)
    {
        health -= damage;
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


    void OnDrawGizmos()
    {
        // Définir la couleur de la ligne pour la rendre bien visible (par exemple, rouge)
        Gizmos.color = Color.red;

        // Dessiner une ligne de la position actuelle de l'objet dans la direction de son axe droit
        // La ligne va de la position de l'objet (transform.position) à transform.position + transform.right * longueur
        float longueur = 2f; // Longueur de la ligne (vous pouvez ajuster cette valeur)
        Gizmos.DrawLine(transform.position, transform.position + transform.right * longueur);

        Gizmos.color = Color.black;

        Gizmos.DrawLine(transform.position, transform.position + transform.up * longueur);

    }

}