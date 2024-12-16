using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Input : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 5f;
    public float runMultiplier = 2f;
    public float runThreshold = 0.9f;
    public float smoothTransitionTime = 0.1f;

    private Vector2 moveDirection = Vector2.zero;
    private Vector2 currentVelocity = Vector2.zero;
    private Vector2 targetVelocity = Vector2.zero;
    private bool isRunning = false;

    public InputAction moveAction;
    public InputAction runAction;
    public CapsuleOrientation capsuleOrientation;  // Reference to the new CapsuleOrientation script

    private void OnEnable()
    {
        moveAction.Enable();
        runAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        runAction.Disable();
    }

    public void Update()
    {
        // Read movement input
        moveDirection = moveAction.ReadValue<Vector2>();

        // Determine if the player is running
        if (Gamepad.current != null)
        {
            isRunning = moveDirection.magnitude > runThreshold;
        }
        else
        {
            isRunning = runAction.IsPressed();
        }

        // Set target velocity
        float targetSpeed = isRunning ? speed * runMultiplier : speed;
        targetVelocity = moveDirection * targetSpeed;

        // Smooth transition between velocities
        currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, Time.deltaTime / smoothTransitionTime);

        // Pass the velocity to the CapsuleOrientation script for rotation
        capsuleOrientation.SetVelocity(currentVelocity);
    }

    private void FixedUpdate()
    {
        rb.velocity = currentVelocity;
    }

    public Vector2 GetCurrentVelocity()
    {
        return currentVelocity;
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }
}
