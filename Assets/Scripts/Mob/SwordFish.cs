using UnityEngine;
using UnityHFSM;
using System;



public sealed class SwordFish : MonoBehaviour
{
    enum State
    {
        PASSIVE,
        HUNTING,
        CHARGE,
        RUSH,
        DEAD
    }

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

    private Mob mob;

    private State state = State.PASSIVE;

    private StateMachine<State> stateMachine;

    void Start()
    {
        mob = new Mob(agent, health, speed, visionRange, moveAreaRange, transform.position);
        mob.Start();

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
            var stateMob = mob.HandleStateBasedOnSight(player, transform.position);
            if (stateMob == Mob.State.HUNTING)
            {
                if (agent.enabled = true)
                {
                    agent.enabled = false;
                }

                Vector3 directionToPlayer = player.transform.position - transform.position;
                directionToPlayer.z = 0;

                float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                Debug.Log("Look at player");

            }
            else
            {
                if (agent.enabled == false)
                {
                    agent.enabled = true;
                }

                mob.PassiveMobMovement();
            }

        }

    }
}