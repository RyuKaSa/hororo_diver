using UnityEngine;

public class Fish : MonoBehaviour
{
    private Vector2 moveDirection;
    private float speed;
    private float despawnDistance;
    private Transform spawnerTransform;
    
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public void SetMovement(Vector2 direction, float moveSpeed, float despawnDist, Transform spawner)
    {
        moveDirection = direction;
        speed = moveSpeed;
        despawnDistance = despawnDist;
        spawnerTransform = spawner;
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        if (moveDirection.x < 0)
        {
            // Flip the fish sprite if moving left
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }

        AdjustAnimationSpeed();
    }

    private void Update()
    {
        MoveFish();
        CheckDespawn();
    }

    private void MoveFish()
    {
        transform.position += (Vector3)(moveDirection * speed * Time.deltaTime);
    }

    private void CheckDespawn()
    {
        if (Vector2.Distance(transform.position, spawnerTransform.position) > despawnDistance)
        {
            Destroy(gameObject);
        }
    }

    private void AdjustAnimationSpeed()
    {
        if (animator != null)
        {
            float sizeFactor = transform.localScale.y;
            animator.speed = (speed / sizeFactor) * 2 * speed * 0.1f;
        }
    }
}
