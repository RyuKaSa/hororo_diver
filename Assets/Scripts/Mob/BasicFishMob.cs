using UnityEngine;


/// <summary>
/// Represents a basic fish mob that attacks in melee. This class defines the behavior and attributes
/// of a basic enemy mob, specifically designed for close combat encounters.
/// </summary>
public sealed class BasicFishMob : MonoBehaviour
{
    Mob mob;


    private void HuntingPlayer()
    {

    }

    public void Start()
    {
        Vector3 point;
        while (!mob.RandomPoint(out point))
        {
            ;
        }
        mob.SetMobAgentDestination(point);

    }

    void Update()
    {
        mob.PassiveMobMovement();

    }

}