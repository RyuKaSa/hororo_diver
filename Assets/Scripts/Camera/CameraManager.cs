using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Player_Input playerInput;  // Reference to the Player_Input script
    public float distanceMultiplier = 3f;  // How far ahead the camera should look
    public float responsiveness = 0.1f;  // How quickly the camera responds to changes in velocity
    public float normalMaxOffset = 5f;  // Max lookahead distance for normal speed
    public float runMaxOffset = 8f;  // Max lookahead distance for running speed
    public float stopSmoothing = 0.2f;  // Additional smoothing when stopping movement

    private Vector3 currentOffset = Vector3.zero;  // Current offset of the camera from the target

    void LateUpdate()
    {
        // Get the target's current velocity and running state from Player_Input
        Vector2 targetVelocity = playerInput.GetCurrentVelocity();
        bool isRunning = playerInput.IsRunning();

        // Get the target's current position and ensure the Z is the same as the camera's
        Vector3 targetPosition = playerInput.transform.position;
        targetPosition.z = transform.position.z;

        // Determine the max offset based on whether the player is running or moving normally
        float maxOffset = isRunning ? runMaxOffset : normalMaxOffset;

        // If the target's velocity is below a threshold (stopping), smoothly stop the camera
        if (targetVelocity.magnitude < 0.1f)
        {
            // Continue smoothing the camera offset when stopping
            currentOffset = Vector3.Lerp(currentOffset, Vector3.zero, stopSmoothing * Time.deltaTime);
        }
        else
        {
            // Calculate lookahead based on the velocity
            Vector3 lookAhead = new Vector3(targetVelocity.x, targetVelocity.y, 0).normalized * distanceMultiplier;

            // Clamp the lookahead to the appropriate max offset
            lookAhead = Vector3.ClampMagnitude(lookAhead, maxOffset);

            // Smoothly transition the current offset toward the calculated lookahead
            currentOffset = Vector3.Lerp(currentOffset, lookAhead, responsiveness * Time.deltaTime);
        }

        // Apply the new camera position, adding the current offset
        targetPosition += currentOffset;
        transform.position = targetPosition;

        // Debugging: Draw lines in the scene view to visualize the camera behavior
        Debug.DrawLine(playerInput.transform.position, playerInput.transform.position + (Vector3)targetVelocity, Color.green); // Target velocity direction
        Debug.DrawLine(playerInput.transform.position, playerInput.transform.position + currentOffset, Color.red);             // Camera lookahead direction
        Debug.DrawLine(transform.position, playerInput.transform.position, Color.blue);                                       // Camera to target line
    }
}
