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
    
    public InputAction moveAction;

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

        // Flip sprite using localScale based on horizontal movement
        Vector3 scale = transform.localScale;

        if (direction.x < 0)
        {
            // Moving left: Ensure the X scale is negative
            scale.x = Mathf.Abs(scale.x) * -1;
        }
        else if (direction.x > 0)
        {
            // Moving right: Ensure the X scale is positive
            scale.x = Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }


    public bool MovementButtonIsTriggered()
    {
        moveDirection = moveAction.ReadValue<Vector2>();
        return moveDirection.sqrMagnitude > 0f;
    }

    public INPUT_ACTION GetPlayerActionByKey()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) return INPUT_ACTION.ATTACK_ACTION;
        if (Keyboard.current.gKey.wasPressedThisFrame) return INPUT_ACTION.SWAP_WEAPON_ACTION;
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
