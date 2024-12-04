using UnityEngine;

public sealed class BasicDistanceMob : MonoBehaviour
{

    public UnityEngine.AI.NavMeshAgent agent;

    public GameObject projectilePrefab;

    private Mob mob;
    private Vector3 spawnPoint;

    private float speed;

    private float health;

    private float visionRange = 15f;

    private float moveAreaRange = 20f;

    private float attackRange = 15f;

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
            Debug.Log("Follow Player");
            AttackSequenceProcess(player);
        }
        else
        {
            mob.PassiveMobMovement();
        }
    }

    private void AttackSequenceProcess(GameObject player)
    {
        // Mob can shoot Player
        if (Vector3.Distance(player.transform.position, transform.position) <= attackRange)
        {
            Debug.Log("Mob shoot player");
            // Calculate Linear equation with mob position and player position
            float gradient = (player.transform.position.y - transform.position.y) / (player.transform.position.x - transform.position.x);
            float offset = transform.position.y - (gradient * transform.position.x);


            GameObject projectileObject = Instantiate(projectilePrefab, transform.position, transform.rotation);
            Projectile projectileScript = projectileObject.GetComponent<Projectile>();

            projectileScript.Initialize(0.5f, gradient, offset);
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