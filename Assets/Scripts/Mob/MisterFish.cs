using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityHFSM;

public class MisterFish : MonoBehaviour
{

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private Rigidbody2D rb;

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

    private float radius; // Rayon actuel

    private float angle = 0f;

    private float radiusChangeTimer = 0f;



    void Start()
    {
        radius = initialRadius;
        SpawnAndMoveTowardsPlayer();

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
        // radiusChangeTimer += Time.deltaTime;

        // if (radiusChangeTimer >= radiusChangeInterval)
        // {
        //     radiusChangeTimer = 0f; // Réinitialiser le timer

        //     radius = Random.Range(minRadius, maxRadius);
        // }

        // erraticMovement();
        // ChasePlayer();
        ChargePlayer();
    }

    private void FixedUpdate()
    {
        // rb.velocity = new Vector2(moveDirection.x, moveDirection.y) * speed;
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


}
