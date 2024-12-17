using UnityEditor.PackageManager;
using UnityEngine;

public sealed class Projectile : MonoBehaviour
{
    Vector3 origin;
    float radius;
    float speed;
    float damage;
    float gradient, offset; // Represent gradient and offset in linear equation
    public void Initialize(float speed, float gradient, float offset, float damage)
    {
        this.speed = speed;
        this.gradient = gradient;
        this.offset = offset;
        this.damage = damage;
    }

    public void Initialize(float speed, float radius, float damage, Vector3 origin)
    {
        this.speed = speed;
        this.radius = radius;
        this.damage = damage;
        this.origin = origin;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    void moveProjectiles() {
        if (origin == null) {
            Debug.LogError("ERROR ERROR ERROR ORIGIN NULL");
            return;
        }
        transform.position += transform.forward * Time.deltaTime * speed; 
        float distance = Vector3.Distance(origin, transform.position);
        if (distance > radius)
        {
            Debug.Log("DESTROYED");
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        moveProjectiles();
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
