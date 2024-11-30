using UnityEngine;


/// <summary>
/// Represents a basic fish mob that attacks in melee. This class defines the behavior and attributes
/// of a basic enemy mob, specifically designed for close combat encounters.
/// </summary>
public sealed class BasicFishMob : MonoBehaviour
{
    private Mob mob = new Mob();

    public UnityEngine.AI.NavMeshAgent agent;

    private void HuntingPlayer()
    {

    }

    public void Start()
    {
        mob.Start();

    }

    void Update()
    {
        mob.PassiveMobMovement();

    }

}