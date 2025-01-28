using UnityEngine;

public class RotatedLookAtMouse : MonoBehaviour
{
    [Tooltip("Speed at which the object rotates toward the target angle.")]
    public float rotationSpeed = 5f;

    [Tooltip("Angle offset in degrees to align the object's apex with the target direction. Adjust based on sprite orientation.")]
    public float angleOffset = -90f; 

    private Quaternion targetRotation;

    void Start()
    {
        // Initialize targetRotation with the current rotation.
        targetRotation = transform.rotation;
    }

    void Update()
    {
        // Create a plane perpendicular to the Z-axis at the object's position.
        Plane plane = new Plane(Vector3.forward, transform.position);
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(mouseRay, out float distance))
        {
            // Get the intersection point on the plane.
            Vector3 hitPoint = mouseRay.GetPoint(distance);

            // Compute direction from the object to the mouse position on the XY-plane.
            Vector3 direction = hitPoint - transform.position;

            // Calculate the absolute angle in degrees from the object's position to the mouse position.
            float absoluteAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Apply offset to align the object's apex as needed.
            float desiredAngle = absoluteAngle + angleOffset;

            // Set the target rotation around the Z-axis to the desired angle.
            targetRotation = Quaternion.Euler(-desiredAngle - 90f, 90f, 0f);
        }

        // Smoothly interpolate the object's rotation toward the target rotation.
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
