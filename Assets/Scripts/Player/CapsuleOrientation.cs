using UnityEngine;

public class CapsuleOrientation : MonoBehaviour
{
    private Vector2 currentVelocity = Vector2.zero;

    public void SetVelocity(Vector2 velocity)
    {
        currentVelocity = velocity;
    }

    public void UpdateRotation()
    {
        // Intentionally left empty – no rotation logic applied.
    }

    public void Reset()
    {
        currentVelocity = Vector2.zero;
    }

    // private void OnDrawGizmos()
    // {
    //     // Visualize the current velocity direction from capsule's position
    //     Gizmos.color = Color.yellow;
    //     Gizmos.DrawLine(transform.position, transform.position + (Vector3)currentVelocity);
    // }
}
