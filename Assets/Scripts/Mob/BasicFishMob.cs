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

    private float visionRange;

    private float moveAreaRange = 20f;


    private void HuntingPlayer()
    {

    }

    public void Start()
    {
        mob = new Mob(agent, health, speed, visionRange, moveAreaRange, transform.position);
        mob.InitMob();

    }

    void Update()
    {
        mob.PassiveMobMovement();

    }

}