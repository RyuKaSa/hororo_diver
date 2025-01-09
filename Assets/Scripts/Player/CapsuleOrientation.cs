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

    public void Reset()
    {
        currentVelocity = Vector2.zero;
        capsule.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void UpdateRotation()
    {
        // Handle capsule rotation based on velocity
        if (currentVelocity.magnitude > 0.01f)  // Avoid unnecessary calculations when nearly still
        {
            // Calculate angle from velocity vector
            float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;

            // Add initial rotation of 90 degrees on the Z-axis
            //angle += initialRotation;

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

    void OnDrawGizmos()
    {
        if (capsule != null)
        {
            // Obtenir l'angle de rotation actuel du sprite
            float angle = capsule.transform.rotation.eulerAngles.z;

            // Calculer la direction du sprite en utilisant l'angle
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);

            // Définir la couleur du Gizmo (par exemple, une ligne rouge)
            Gizmos.color = Color.red;

            // Dessiner la ligne depuis la position de la capsule pour indiquer la direction
            Gizmos.DrawLine(capsule.transform.position, capsule.transform.position + direction * 2f); // *2f pour ajuster la longueur de la ligne
        }
    }
}
