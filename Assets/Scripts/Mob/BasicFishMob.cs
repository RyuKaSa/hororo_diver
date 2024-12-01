using UnityEngine;


/// <summary>
/// Represents a basic fish mob that attacks in melee. This class defines the behavior and attributes
/// of a basic enemy mob, specifically designed for close combat encounters.
/// </summary>
public sealed class BasicFishMob : MonoBehaviour
{

    /// <summary>
    /// Enum which describe all stages during an attack sequence
    /// </summary>
    private enum ATTACK_STAGE
    {
        BEGIN_STG, // Stage when calculate data for attack
        BACKWARD_STG, // Stage where the mob backward before charge player
        ATK_STG, // Stage where the mob charges player
        RETURN_STG, // Stage where the mob returns to his start point
        NOATK
    };

    /// <summary>
    /// Class containing all the data necessary during a mob attack sequence.
    /// </summary>
    private sealed class AttackSequenceData
    {
        private Vector3 startPoint;
        private float backwardDistance; // Distance between startPoint and the point behind mob

        private float speedCharge; // Speed when the mob charge the player after BACKWARD_STG

        public void SetStartPoint(Vector3 startPoint)
        {
            this.startPoint = startPoint;
        }

        public void SetBackWardDistance(float backwardDistance)
        {
            if (this.backwardDistance < 0)
            {
                Debug.Log("ERROR : BackWardDistance is negative");
                return;
            }
            this.backwardDistance = backwardDistance;
        }

        public void SetSpeedCharge(float speedCharge)
        {
            if (this.speedCharge <= 0)
            {
                Debug.Log("ERROR : speedCharge is negative or equal to 0");
                return;
            }

            this.speedCharge = speedCharge;
        }


    }

    private Mob mob;
    public UnityEngine.AI.NavMeshAgent agent;

    private Vector3 spawnPoint;

    private float speed;

    private float health;

    private float visionRange = 15f;

    private float moveAreaRange = 20f;

    private float attackRange = 2f;

    private ATTACK_STAGE atk_stage = ATTACK_STAGE.NOATK;

    private AttackSequenceData attackSequenceData = new AttackSequenceData();

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
            Debug.Log("Follow Player");
            mob.SetMobAgentDestination(player.transform.position);
            AttackSequenceProcess(player);
        }
        else
        {
            mob.PassiveMobMovement();
        }
    }

    /// <summary>
    /// Function managing an attack sequence when the mob is at a certain distance from the player.
    /// An attack sequence is broken down into 4 phases. The first consists of initializing the data relating to the attack
    /// (the position of the mob and the distance between mob and the backward point). The second consists of the mob 
    /// going to a point behind it before rushing towards the player to attack. The third is when the mob rush to the player and attacks.
    /// The last phase consists of go back to start point.
    /// </summary>
    /// <param name="player"></param>
    private void AttackSequenceProcess(GameObject player)
    {
        float distMobPlayer = Vector3.Distance(transform.position, player.transform.position);
        // Trigger firt stage
        if (atk_stage == ATTACK_STAGE.NOATK && distMobPlayer <= attackRange)
        {
            Debug.Log("CHANGE TO BACKWARD STAGE");

            atk_stage = ATTACK_STAGE.BACKWARD_STG;
            float backwardDistance = distMobPlayer * 0.05f;
            attackSequenceData.SetBackWardDistance(backwardDistance);
            attackSequenceData.SetStartPoint(transform.position);
            mob.SetMobAgentDestination(new Vector3(transform.position.x - backwardDistance, transform.position.y, transform.position.z));
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