using UnityEngine;


public sealed class old_Projectile : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D rb;

    float speed = 0.05f;
    float damage;

    public void Initialize(float speed, float damage)
    {
        this.damage = damage;
        rb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collide with other obj");
        var damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Debug.Log(transform.name + " inflicts damage to " + other.name);
            damageable.Damage(damage);
        }
        else
        {
            Debug.Log("Interface IDamageable not found");
        }
    }
}