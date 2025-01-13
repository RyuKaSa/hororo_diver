using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;
using EZCameraShake;

public class MisterFish : MonoBehaviour
{

    [System.Serializable]
    struct KinematicData
    {
        public Transform begin;

        public Transform end;

        public float startTime;

        public float speed;

        public bool isTriggered;

        public Vector3 MoveStraightLine()
        {
            float distance = Vector3.Distance(begin.position, end.position);

            // Calculate the fraction of the path traveled in each frame
            float distanceCovered = (Time.time - startTime) * speed;
            float fractionOfStraightLine = distanceCovered / distance;

            // Interpolate position
            Vector3 position = Vector3.Lerp(begin.position, end.position, fractionOfStraightLine);

            if (fractionOfStraightLine >= 1f)
            {
                return end.position;
            }
            return position;
        }

    }

    enum MisterFishStates
    {
        IDLE,
        KINEMATIC,
        HUNTING
    }

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private Vector2 moveDirection;

    [SerializeField]
    private float initialRadius = 5f;

    [SerializeField]
    private float minRadius = 2f;

    [SerializeField]
    private float maxRadius = 10f;

    [SerializeField]
    private float speed = 0.5f;

    [SerializeField]
    private float radiusChangeInterval = 3f; // Intervalle de temps entre les changements de rayon

    [SerializeField]
    private float huntingDuration = 20f;

    [SerializeField]
    private KinematicData kinematicData;

    private StateMachine<MisterFishStates> stateMachine = new StateMachine<MisterFishStates>();

    private float radius; // Rayon actuel

    private float angle = 0f;

    private float radiusChangeTimer = 0f;

    private float startTime;


    void Start()
    {
        radius = initialRadius;
        kinematicData.isTriggered = true;
        kinematicData.startTime = Time.time;

        stateMachine.AddState(MisterFishStates.IDLE, new State<MisterFishStates>(
            onLogic: state => erraticMovement()
        ));

        stateMachine.AddState(MisterFishStates.KINEMATIC, new State<MisterFishStates>(
            onLogic: state =>
            {
                // Begin kinematic
                Debug.Log("Dans kinematic state");
                if (kinematicData.isTriggered)
                {
                    KinematicProcess();
                }
            }
        ));

        stateMachine.AddState(MisterFishStates.HUNTING, new State<MisterFishStates>(
            onLogic: state =>
            {
                Debug.Log("Dans HUNTING state");
                ChasePlayer();
            }
        ));

        stateMachine.AddTransition(new Transition<MisterFishStates>(
            MisterFishStates.KINEMATIC,
            MisterFishStates.HUNTING,
            transition => !kinematicData.isTriggered && !audioSource.isPlaying
        ));

        stateMachine.SetStartState(MisterFishStates.KINEMATIC);
        stateMachine.Init();

    }

    private void erraticMovement()
    {
        angle += speed * Time.deltaTime;

        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;

        transform.position = player.transform.position + new Vector3(x, y, 0);
    }

    void Update()
    {
        stateMachine.OnLogic();

        // radiusChangeTimer += Time.deltaTime;

        // if (radiusChangeTimer >= radiusChangeInterval)
        // {
        //     radiusChangeTimer = 0f; // Réinitialiser le timer

        //     radius = Random.Range(minRadius, maxRadius);
        // }

        // erraticMovement();
        // ChasePlayer();
        // ChargePlayer();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveDirection.x, moveDirection.y) * speed;
    }

    private void ChasePlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
        moveDirection = direction;
    }

    private void SpawnAndMoveTowardsPlayer()
    {
        Camera cam = Camera.main;
        float camWidth = cam.orthographicSize * cam.aspect;
        float camHeight = cam.orthographicSize;

        // Choose random side
        Vector3 spawnPosition = Vector3.zero;
        int side = Random.Range(0, 4); // 0 = left, 1 = right, 2 = up, 3 = down

        switch (side)
        {
            case 0:
                spawnPosition = new Vector3(-camWidth - 1f, Random.Range(-camHeight, camHeight), 0);
                break;
            case 1:
                spawnPosition = new Vector3(camWidth + 1f, Random.Range(-camHeight, camHeight), 0);
                break;
            case 2:
                spawnPosition = new Vector3(Random.Range(-camWidth, camWidth), camHeight + 1f, 0);
                break;
            case 3:
                spawnPosition = new Vector3(Random.Range(-camWidth, camWidth), -camHeight - 1f, 0);
                break;
        }

        transform.position = spawnPosition;

        // Start Move 
        StartCoroutine(MoveTowardsPlayer());
    }

    private IEnumerator MoveTowardsPlayer()
    {
        float step = speed * Time.deltaTime;

        while (Vector3.Distance(transform.position, player.transform.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
            yield return null; // Wait next frame
        }
    }

    private void ChargePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        // Move the SwordFish towards the player if it is farther than 1 unit
        if (distanceToPlayer > 1.5)
        {
            transform.position = SwordFish.RushTowardsPlayer(player.transform.position, transform.position, 10f, speed);
        }

    }

    public void BeginKinematic()
    {
        if (!kinematicData.isTriggered)
        {
            kinematicData.isTriggered = true;
            kinematicData.startTime = Time.time;
            Debug.Log("kinematicData.isTriggered = true");
        }
    }

    public void KinematicProcess()
    {
        var pos = kinematicData.MoveStraightLine();
        transform.position = pos;

        if (Vector3.Distance(transform.position, kinematicData.end.position) <= 1f)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                kinematicData.isTriggered = false; // Kinematic is finished
                CameraShaker.Instance.ShakeOnce(3f, 4f, 3f, 5f);
                animator.SetBool("isScreaming", true);
                Debug.Log("Joue le son");
            }
            else
            {
                Debug.Log("Le son est déjà en train de jouer");
            }
        }

    }

}
