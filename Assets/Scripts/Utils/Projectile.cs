using UnityEngine;


public sealed class Projectile : MonoBehaviour
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collide with other obj");
        var damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Debug.Log(transform.name + " inflicts damage to " + other.gameObject.name);
            damageable.Damage(damage);
        }
        else
        {
            Debug.Log("Interface IDamageable not found");
        }
    }
}