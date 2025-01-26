using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;
using EZCameraShake;

public class MisterFish : MonoBehaviour
{

    [System.Serializable]
    public struct KinematicData
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
        MOVE_AROUND,
        SPAWN_AND_RUSH,
        HUNTING,
        KINEMATIC,
        CHARGE,
        RUSH
    }

    public KinematicData kinematicData;

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
    private float huntingMaxDuration = 20f;

    [SerializeField]
    private float chargeTime = 1.5f;

    [SerializeField]
    private float chargeSpeedCoeff = 10f;

    [SerializeField]
    private float exitHunting = 20f;

    private StateMachine<MisterFishStates> stateMachine = new StateMachine<MisterFishStates>();

    private float radius; // Rayon actuel

    private float angle = 0f;

    private float radiusChangeTimer = 0f;

    private float startTime;

    private float timeToReachPlayer; // Approximative required time for mister Fish to reach player

    private Vector3 lastPlayerPos;

    private bool waitNextCharge = false; // Flag which allow mister Fish to charge the player

    private float huntingDurationTimer = 0f; // Timer that measures Mister Fish's hunting time

    private float randomBehaviorTimer = 0f; // Delay between 2 appearance of Mister Fish

    private float spawnDelay; // Delay for first spawn of Mister Fish

    private float minSpawnTime = 120f; // Minimum time before Mister Fish can reappear

    private float maxSpawnTime = 180f; // Maximum time before Mister Fish can reappear

    private float moveAroundDuration = 23f; // Duration of Move Around state


    void Start()
    {
        spawnDelay = Random.Range(minSpawnTime, maxSpawnTime);
        randomBehaviorTimer = Time.time;

        radius = initialRadius;
        // kinematicData.isTriggered = true;
        kinematicData.startTime = Time.time;

        stateMachine.AddState(MisterFishStates.IDLE, new State<MisterFishStates>(
            onLogic: state =>
            {
                if (huntingDurationTimer > 0f)
                {
                    huntingDurationTimer = 0f; // Reset timer for the next time where Mister Fish chase player 
                }
                Debug.Log("IDLE sate");
            }
        ));

        stateMachine.AddState(MisterFishStates.MOVE_AROUND, new State<MisterFishStates>(
            onLogic: state => MoveAround(),
            canExit: state => state.timer.Elapsed >= moveAroundDuration,
            needsExitTime: true

        ));

        stateMachine.AddState(MisterFishStates.SPAWN_AND_RUSH, new CoState<MisterFishStates>(
            this,
            MoveTowardsPlayer,
            onEnter: action => SpawnAndMoveTowardsPlayer(),
            loop: true,
            needsExitTime: true
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
                // Debug.Log("Dans HUNTING state + dist = " + Vector3.Distance(transform.position, player.transform.position));
                Debug.Log("Dans HUNTING state");
                animator.SetBool("isScreaming", false);
                ChasePlayer();
            }
        ));

        stateMachine.AddState(MisterFishStates.CHARGE, new State<MisterFishStates>(
            onLogic: state =>
            {
                Debug.Log("Charge state");
                ChargeAttackProcess(state.timer.Elapsed);
            },
            canExit: state => state.timer.Elapsed > chargeTime,
            needsExitTime: true
        ));

        stateMachine.AddState(MisterFishStates.RUSH, new State<MisterFishStates>(
            onLogic: state => RushPlayer(),
            canExit: state => state.timer.Elapsed > 5f,
            needsExitTime: true
        ));

        stateMachine.AddTransition(new Transition<MisterFishStates>(
            MisterFishStates.KINEMATIC,
            MisterFishStates.HUNTING,
            transition =>
            {
                huntingDurationTimer = Time.time;
                return !kinematicData.isTriggered && !audioSource.isPlaying;
            }
        ));

        stateMachine.AddTransition(new Transition<MisterFishStates>(
            MisterFishStates.HUNTING,
            MisterFishStates.CHARGE,
            transition =>
            {
                var randNum = Random.Range(1, 101);
                return Vector3.Distance(transform.position, player.transform.position) <= 5f && randNum % 25 == 0;
            }
        ));

        stateMachine.AddTransition(new Transition<MisterFishStates>(
            MisterFishStates.CHARGE,
            MisterFishStates.RUSH,
            transition => true
        ));

        stateMachine.AddTransition(new Transition<MisterFishStates>(
            MisterFishStates.RUSH,
            MisterFishStates.HUNTING,
            transition =>
            {
                waitNextCharge = false;
                return Vector3.Distance(transform.position, lastPlayerPos) <= 2f;
            }
        ));

        stateMachine.AddTransition(new Transition<MisterFishStates>(
            MisterFishStates.IDLE,
            MisterFishStates.KINEMATIC,
            transition => kinematicData.isTriggered
        ));

        stateMachine.AddTransition(new Transition<MisterFishStates>(
            MisterFishStates.HUNTING,
            MisterFishStates.IDLE,
            transition => (Time.time - huntingDurationTimer) >= huntingMaxDuration
        ));

        stateMachine.AddTransition(new Transition<MisterFishStates>(
            MisterFishStates.SPAWN_AND_RUSH,
            MisterFishStates.HUNTING
        ));

        stateMachine.AddTransition(new Transition<MisterFishStates>(
            MisterFishStates.MOVE_AROUND,
            MisterFishStates.HUNTING,
            transition =>
            {
                var randNum = Random.Range(1, 101);
                return randNum % 25 == 0;
            }
        ));


        stateMachine.SetStartState(MisterFishStates.IDLE);
        stateMachine.Init();

    }

    private void MoveAround()
    {

        Camera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        float camWidth = cam.orthographicSize * cam.aspect;
        float camHeight = cam.orthographicSize;
        angle += (speed / 15f) * Time.deltaTime;

        float x = Mathf.Cos(angle) * (22f);
        float y = Mathf.Sin(angle) * (22f);

        transform.position = player.transform.position + new Vector3(x, y, 0);

        // Face Player
        Vector3 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.z = 0; // Ensure we remain on the 2D plane

        float teta = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, teta));

    }

    void Update()
    {
        if (Time.time - randomBehaviorTimer >= spawnDelay && stateMachine.ActiveState.name == MisterFishStates.IDLE)
        {
            stateMachine.RequestStateChange((MisterFishStates)Random.Range((int)MisterFishStates.MOVE_AROUND, (int)MisterFishStates.HUNTING), true);
            spawnDelay = Random.Range(minSpawnTime, maxSpawnTime);
            randomBehaviorTimer = Time.time;
        }

        stateMachine.OnLogic();

        // radiusChangeTimer += Time.deltaTime;

        // if (radiusChangeTimer >= radiusChangeInterval)
        // {
        //     radiusChangeTimer = 0f; // Réinitialiser le timer

        //     radius = Random.Range(minRadius, maxRadius);
        // }

        // MoveAround();

        // ChasePlayer();
        // RushPlayer();
    }

    private void FixedUpdate()
    {
        if (waitNextCharge)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            rb.velocity = new Vector2(moveDirection.x, moveDirection.y) * speed;
        }
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
        float[] boundX = { player.transform.position.x - 22f, player.transform.position.x + 22f };
        float[] boundY = { player.transform.position.y - 22f, player.transform.position.y + 22f };

        // Choose random side
        Vector3 spawnPosition = Vector3.zero;
        int side = Random.Range(0, 4); // 0 = left, 1 = right, 2 = up, 3 = down

        switch (side)
        {
            case 0:
                spawnPosition = new Vector3(boundX[0] - 1f, Random.Range(boundY[0], boundY[1]), 0);
                break;
            case 1:
                spawnPosition = new Vector3(boundX[1] + 1f, Random.Range(boundY[0], boundY[1]), 0);
                break;
            case 2:
                spawnPosition = new Vector3(Random.Range(boundX[0], boundX[1]), boundY[1] + 1f, 0);
                break;
            case 3:
                spawnPosition = new Vector3(Random.Range(boundX[0], boundX[1]), boundY[0] - 1f, 0);
                break;
        }

        transform.position = spawnPosition;
    }

    IEnumerator MoveTowardsPlayer()
    {
        float step = (speed * 2) * Time.deltaTime;

        while (Vector3.Distance(transform.position, player.transform.position) > 5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, step);
            Vector3 directionToPlayer = player.transform.position - transform.position;
            directionToPlayer.z = 0; // Ensure we remain on the 2D plane

            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            yield return null; // Wait next frame
        }

        // Because needsExitTime is true, we have to tell the FSM when it can
        // safely exit the state.
        stateMachine.StateCanExit();
    }

    private void RushPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, lastPlayerPos);
        // Debug.Log("distanceToPlayer = " + distanceToPlayer);
        Debug.Log("RUSH State");
        // Move the SwordFish towards the player if it is farther than 1 unit
        if (distanceToPlayer > 2f)
        {
            transform.position = SwordFish.RushTowardsPlayer(lastPlayerPos, transform.position, chargeSpeedCoeff, speed);
        }
        else
        {
            waitNextCharge = true;
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
        transform.position = new Vector3(pos.x, pos.y, 0f);

        if (Vector3.Distance(transform.position, kinematicData.end.position) <= 1f)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
                kinematicData.isTriggered = false; // Kinematic is finished
                CameraShaker.Instance.ShakeOnce(3f, 4f, 3f, 5f);
                animator.SetBool("isScreaming", true);
            }
            else
            {
                Debug.Log("Le son est déjà en train de jouer");
            }
        }

    }

    private void ChargeAttackProcess(float elapsedTime)
    {
        // saves the player's position up to a certain time
        if (elapsedTime <= chargeTime * 0.95)
        {
            lastPlayerPos = player.transform.position;

            float chargeSpeed = chargeSpeedCoeff * speed;
            float distanceToPlayer = Vector3.Distance(transform.position, lastPlayerPos);

            timeToReachPlayer = distanceToPlayer / chargeSpeed;
        }

        // Face to player during his charge
        Vector3 directionToPlayer = player.transform.position - transform.position;
        directionToPlayer.z = 0; // Ensure we remain on the 2D plane

        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

    }

    public void TriggerKinematic()
    {
        Debug.Log("Trigger kinematic");
        kinematicData.isTriggered = true;
        kinematicData.startTime = Time.time;

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collide with other obj");
        var damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Debug.Log(transform.name + " inflicts damage to " + other.gameObject.name);
            damageable.Damage(5f);
        }
        else
        {
            Debug.Log("Interface IDamageable not found");
        }
    }

}
