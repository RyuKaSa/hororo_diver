using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Input : MonoBehaviour
{
    public enum INPUT_ACTION
    {
        ATTACK_ACTION,
        SWAP_WEAPON_ACTION,
        NO_ACTION
    }

    public Rigidbody2D rb;
    public float baseSpeed = 5f;
    public float smoothTransitionTime = 0.1f;

    [Header("Acceleration Settings")]
    public float acceleration = 10f;  // Acceleration factor

    [SerializeField] private Animator animator;
    [SerializeField] private CapsuleOrientation capsuleOrientation;

    public InputActionAsset inputActionAsset;

    public InputAction moveAction;
    private InputAction attackAction;
    private InputAction swapAction;


    private Vector2 moveDirection = Vector2.zero;
    private Vector2 currentVelocity = Vector2.zero;
    private Vector2 targetVelocity = Vector2.zero;
    private ReadOnlyDictionary<string, Attribute> attributes;
    private Vector3 defaultScale;

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    private void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        defaultScale = transform.localScale;
    }

    public void Awake()
    {
        if (inputActionAsset != null)
        {
            attackAction = inputActionAsset.FindAction("attackAction");
            if (attackAction == null)
            {
                Debug.Log("attackAction is null");
            }
            swapAction = inputActionAsset.FindAction("swapAction");
            if (swapAction == null)
            {
                Debug.Log("swapAction is null");
            }

            attackAction.Enable();
            swapAction.Enable();

        }
    }

    private bool LoadAttributes()
    {
        if (attributes == null)
        {
            var roAttr = GetComponent<Player>()?.AsReadOnlyAttributes();
            if (roAttr == null) return false;
            attributes = roAttr;
        }
        return true;
    }

    public void IdleState()
    {
        UpdateAnimation(Vector2.zero);
        targetVelocity = Vector2.zero;
        currentVelocity = Vector2.zero;
        moveDirection = Vector2.zero;
        capsuleOrientation?.Reset();
        transform.localScale = defaultScale;
    }

    public void UpdateMovement()
    {
        if (!LoadAttributes()) return;

        // Read input and update animation
        moveDirection = moveAction.ReadValue<Vector2>();
        UpdateAnimation(moveDirection);

        // Calculate velocity directly from WASD input without running logic
        float speed = attributes["speed"].FinalValue();
        targetVelocity = moveDirection * speed;

        // Smoothly transition to target velocity using acceleration
        currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, acceleration * Time.deltaTime);

        // Pass velocity to orientation handler
        capsuleOrientation?.SetVelocity(currentVelocity);
    }

    private void UpdateAnimation(Vector2 direction)
    {
        bool moving = direction.sqrMagnitude > 0f;
        animator.SetBool("isSwimming", moving);

        // Get the SpriteRenderer component
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Flip the sprite when moving left, unflip when moving right
            if (direction.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (direction.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }

        // Ensure no unwanted rotation issues
        transform.localRotation = Quaternion.identity;
    }

    public bool MovementButtonIsTriggered()
    {
        moveDirection = moveAction.ReadValue<Vector2>();
        return moveDirection.sqrMagnitude > 0f;
    }

    public INPUT_ACTION GetPlayerActionByKey()
    {
        Debug.Log("attackAction.triggered = " + attackAction.triggered + " swapAction.triggered = " + swapAction.triggered);
        if (attackAction.triggered) return INPUT_ACTION.ATTACK_ACTION;
        if (swapAction.triggered) return INPUT_ACTION.SWAP_WEAPON_ACTION;
        return INPUT_ACTION.NO_ACTION;
    }

    private void FixedUpdate()
    {
        UpdateMovement();
        rb.velocity = currentVelocity;
        capsuleOrientation?.UpdateRotation();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)targetVelocity);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)currentVelocity);
    }

    public Vector2 GetCurrentVelocity() => currentVelocity;
    public Vector2 GetMoveDirection() => moveDirection;
}
