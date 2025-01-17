using UnityEngine;

public class Bubble : MonoBehaviour
{
    [HideInInspector] public float speed;
    [HideInInspector] public float lifetime;
    [HideInInspector] public float horizontalOffset;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move bubble up with slight horizontal drift
        transform.position += new Vector3(horizontalOffset * Time.deltaTime, speed * Time.deltaTime, 0);
    }
}
