using UnityEngine;

public class CapsuleOrientation : MonoBehaviour
{
    public GameObject capsule;  // Reference to the child capsule
    private Vector2 currentVelocity = Vector2.zero;

    public float initialRotation = 90f;  // 90-degree initial rotation on the Z-axis

    public void SetVelocity(Vector2 velocity)
    {
        currentVelocity = velocity;
    }

    void Update()
    {
        // Handle capsule rotation based on velocity
        if (currentVelocity.magnitude > 0.01f)  // Avoid unnecessary calculations when nearly still
        {
            // Calculate angle from velocity vector
            float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;

            // Add initial rotation of 90 degrees on the Z-axis
            angle += initialRotation;

            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            // Smoothly rotate the capsule to match the velocity direction with initial rotation
            capsule.transform.rotation = Quaternion.Lerp(capsule.transform.rotation, targetRotation, Time.deltaTime * 10f);

            // Handle flipping when crossing the 180-degree mark
            if (Mathf.Abs(capsule.transform.rotation.eulerAngles.z - angle) > 90f)
            {
                // Flip the capsule on itself to simulate diver flipping
                capsule.transform.Rotate(0, 180f, 0);
            }
        }
    }
}
