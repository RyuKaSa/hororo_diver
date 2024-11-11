using UnityEngine;

public class Mob : MonoBehaviour
{
    enum State
    {
        PASSIVE,
        HUNTING,
        DEAD
    }

    public UnityEngine.AI.NavMeshAgent agent;

    private Vector3 spawnPoint;

    private float speed;

    private float health;

    private State state = State.PASSIVE;

    [SerializeField] private float visionRange;

    [SerializeField] private float moveAreaRange = 20f;

    bool RandomPoint(out Vector3 result)
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
    /// Méthode utilisé pour faire se déplacer le mob lorsqu'il est en mode passif.
    /// Trouve un point aléatoire dans un rayon autour du mob et en fait sa destination.
    /// </summary>
    private void PassiveMobMovement()
    {
        Vector3 point;
        if (agent.remainingDistance <= 1f && RandomPoint(out point))
        {
            Debug.Log("Find random point");
            agent.destination = point;
        }
    }

    /// <summary>
    /// Réduit la santé du mob en fonction de la quantité de dégâts reçus en paramètre.
    /// </summary>
    /// <param name="damage">Le montant des dégâts à infliger au mob. La santé sera réduite de cette valeur</param>
    private void TakeDamage(float damage)
    {
        health -= damage;

    }

    /// <summary>
    /// Vérifie si le joueur est dans le champ de vision du mob.
    /// </summary>
    /// <param name="player">Le GameObject représentant le joueur</param>
    /// <returns>Retourne vrai si le joueur est dans le champ de vision, sinon faux.</returns>
    private bool IsPlayerInSight(GameObject player)
    {
        return Vector3.Distance(player.transform.position, transform.position) <= visionRange;

    }

    /// <summary>
    /// Modifie l'état du mob en fonction de la visibilité du joueur dans son champ de vision
    /// </summary>
    /// <param name="player">Le GameObject représentant le joueur.</param>
    /// <remarks>
    /// Cette fonction vérifie si le joueur est dans le champ de vision du mob en appelant la fonction <see cref="IsPlayerInSight"/>.
    /// Si le joueur est détecté, l'état du mob passe à <see cref="State.HUNTING"/>. Sinon, l'état du mob passe à <see cref="State.PASSIVE"/>.
    /// </remarks>
    public void HandleStateBasedOnSight(GameObject player)
    {
        state = IsPlayerInSight(player) ? State.HUNTING : State.PASSIVE;
    }

    public void SetPassiveState()
    {
        state = State.PASSIVE;
    }

    public void SetHuntingState()
    {
        state = State.HUNTING;
    }


    public void Start()
    {
        spawnPoint = transform.position;
        Vector3 point;
        while (!RandomPoint(out point))
        {
            ;
        }
        agent.destination = point;

    }

    void Update()
    {
        PassiveMobMovement();

    }
}