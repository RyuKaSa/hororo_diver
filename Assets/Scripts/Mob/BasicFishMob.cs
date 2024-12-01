using UnityEngine;


/// <summary>
/// Represents a basic fish mob that attacks in melee. This class defines the behavior and attributes
/// of a basic enemy mob, specifically designed for close combat encounters.
/// </summary>
public sealed class BasicFishMob : MonoBehaviour
{
    private Mob mob;
    public UnityEngine.AI.NavMeshAgent agent;

    private Vector3 spawnPoint;

    private float speed;

    private float health;

    private float visionRange = 15f;

    private float moveAreaRange = 20f;


    private void HuntingPlayer()
    {

    }

    public void Start()
    {
        mob = new Mob(agent, health, speed, visionRange, moveAreaRange, transform.position);
        mob.Start();

    }

    /// <summary>
    /// This method call different function which manage behavior according
    /// to Mob's state.
    /// </summary>
    private void BehaviorProcessBasedOnState(Mob.State state)
    {
        if (state == Mob.State.HUNTING)
        {
            Debug.Log("Follow Player");
            mob.SetMobAgentDestination(player.transform.position);
        }
        else
        {
            mob.PassiveMobMovement();
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
            BehaviorProcessBasedOnState(state); // Determines which behavior algo choose according to mob's state
        }
    }

}