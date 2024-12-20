using System;
using UnityEngine;


/// <summary>
/// Represents a basic fish mob that attacks in melee. This class defines the behavior and attributes
/// of a basic enemy mob, specifically designed for close combat encounters.
/// </summary>
public sealed class BasicFishMob : MonoBehaviour, IDamageable
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
        private Vector3 backwardPoint; // Distance between startPoint and the point behind mob

        private float speedCharge; // Speed when the mob charge the player after BACKWARD_STG

        private int nbAttack = 0;

        public void SetStartPoint(Vector3 startPoint)
        {
            this.startPoint = startPoint;
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

        public float GetSpeedCharge()
        {
            return speedCharge;
        }

        public Vector3 GetBackwardPoint()
        {
            return backwardPoint;
        }

        public Vector3 GetStartPoint()
        {
            return startPoint;
        }

        public Vector3 ProcessBackwardPoint(float backwardDistance, Vector3 mobPosition, Vector3 playerPosition)
        {
            if (backwardDistance < 0)
            {
                Debug.Log("ERROR : BackwardDistance is negative, return mobPosition");
                return mobPosition;
            }
            float deltaX = Math.Abs(mobPosition.x - playerPosition.x);
            float deltaY = Math.Abs(mobPosition.y - playerPosition.y);
            float dist = Vector3.Distance(mobPosition, playerPosition);

            float angleX = (backwardDistance * (deltaX / dist));
            float angleY = (backwardDistance * (deltaY / dist));
            backwardPoint = new Vector3(mobPosition.x - angleX, mobPosition.y - angleY, mobPosition.z);
            return backwardPoint;
        }

        /// <summary>
        /// This method reset the field which indicating the number of attacks made by the mob 
        /// until it is out of reach of the player. This method is called when the mob is out of
        /// reach of the player.
        /// </summary>
        public void ResetNbAttack()
        {
            nbAttack = 0;
        }

        public int GetNbAttack()
        {
            return nbAttack;
        }

        public void IncreaseNbAttack()
        {
            nbAttack += 1;
        }


    }

    private Mob mob;
    public UnityEngine.AI.NavMeshAgent agent;

    private Vector3 spawnPoint;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float health;

    [SerializeField]
    private float damage = 1f;

    [SerializeField]
    private float visionRange = 15f;

    [SerializeField]
    private float moveAreaRange = 20f;

    [SerializeField]
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
            // Attack sequence not triggered so mob hunting player
            if (atk_stage == ATTACK_STAGE.NOATK)
            {
                mob.SetMobAgentDestination(player.transform.position);
            }
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
            float backwardDistance = distMobPlayer * 0.2f;
            var backwardPoint = attackSequenceData.ProcessBackwardPoint(backwardDistance, transform.position, player.transform.position);
            attackSequenceData.SetStartPoint(transform.position);
            mob.SetMobAgentDestination(backwardPoint);

        }

        if (atk_stage == ATTACK_STAGE.BACKWARD_STG && Vector3.Distance(attackSequenceData.GetBackwardPoint(), transform.position) <= 0.05f)
        {
            Debug.Log("CHANGE TO ATTACK STAGE");

            atk_stage = ATTACK_STAGE.ATK_STG;
            mob.SetMobAgentDestination(player.transform.position);
        }

        // Takes the destination agent as a reference not currens player's position because the player may have moved between 2 frame 
        if (atk_stage == ATTACK_STAGE.ATK_STG && Vector3.Distance(agent.destination, transform.position) <= 1f)
        {
            Debug.Log("CHANGE TO RETURN STAGE");

            attackSequenceData.IncreaseNbAttack();
            atk_stage = ATTACK_STAGE.RETURN_STG;
            mob.SetMobAgentDestination(attackSequenceData.GetStartPoint());

        }

        if (atk_stage == ATTACK_STAGE.RETURN_STG && Vector3.Distance(transform.position, attackSequenceData.GetStartPoint()) <= 0.005f)
        {
            Debug.Log("CHANGE TO NOATK STAGE");
            atk_stage = ATTACK_STAGE.NOATK; // Finish attack sequence, return to start point
        }
    }

    public void Damage(float damage)
    {
        Debug.Log(transform.name + " takes " + damage + " damage");
        health -= damage;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collide with other obj");
        if (atk_stage == ATTACK_STAGE.ATK_STG)
        {
            var damageable = other.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                Debug.Log(transform.name + " inflicts damage to " + other.gameObject.name);
                damageable.Damage(damage);
            }
            else
            {
                Debug.Log("Interface IDamageable not found");
            }
        }
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