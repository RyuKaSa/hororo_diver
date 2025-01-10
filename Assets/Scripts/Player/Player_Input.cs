using System;
using System.Collections.Generic;
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
    public float speed = 5f;
    public float runMultiplier = 2f;
    public float runThreshold = 0.9f;
    public float smoothTransitionTime = 0.1f;

    private Animator animator;
    private Vector2 moveDirection = Vector2.zero;
    private Vector2 currentVelocity = Vector2.zero;
    private Vector2 targetVelocity = Vector2.zero;
    private bool isRunning = false;
    private ReadOnlyDictionary<string, Attribute> attributes;

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

    public void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.Log("Animator not found");
        }

    }

    private bool WaitingLoadAttributes()
    {
        if (attributes == null)
        {
            var roAttribute = GetComponent<Player>().AsReadOnlyAttributes();
            if (roAttribute == null)
            {
                return false;
            }
            attributes = roAttribute;
        }
        return true;
    }

    public void IdleState()
    {
        Debug.Log("speed = " + targetVelocity + " currentVelocity = " + currentVelocity);
        UpdateAnimation(Vector2.zero);
        targetVelocity = Vector2.zero;
        currentVelocity = Vector2.zero;
        moveDirection = Vector2.zero;
        capsuleOrientation.Reset();

    }

    public void UpdateMovement()
    {
        Debug.Log("Debug movement call");
        if (!WaitingLoadAttributes())
        {
            return;
        }

        // Read movement input
        moveDirection = moveAction.ReadValue<Vector2>();

        UpdateAnimation(moveDirection);

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
        float targetSpeed = isRunning ? attributes["speed"].FinalValue() * runMultiplier : attributes["speed"].FinalValue();
        targetVelocity = moveDirection * targetSpeed;

        // Smooth transition between velocities
        currentVelocity = Vector2.Lerp(currentVelocity, targetVelocity, Time.deltaTime / smoothTransitionTime);

        // Pass the velocity to the CapsuleOrientation script for rotation
        capsuleOrientation.SetVelocity(currentVelocity);
        capsuleOrientation.UpdateRotation();
    }

    public bool MovementButtonIsTriggered()
    {
        moveDirection = moveAction.ReadValue<Vector2>();
        Debug.Log("moveDirection = " + moveDirection);
        return moveDirection.magnitude > 0f;
    }

    public INPUT_ACTION GetPlayerActionByKey()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            return INPUT_ACTION.ATTACK_ACTION;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            return INPUT_ACTION.SWAP_WEAPON_ACTION;
        }
        return INPUT_ACTION.NO_ACTION;

    }

    private void UpdateAnimation(Vector2 moveDirection)
    {
        if (moveDirection.x != 0f || moveDirection.y != 0f)
        {
            animator.SetBool("isSwimming", true);
            return;
        }
        else if (moveDirection.x == 0f && moveDirection.y == 0f)
        {
            animator.SetBool("isSwimming", false);
            return;
        }

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
