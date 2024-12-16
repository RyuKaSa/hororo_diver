using UnityEngine;


public sealed class Projectile : MonoBehaviour
{
    float speed;
    float gradient, offset; // Represent gradient and offset in linear equation

    float damage;

    public void Initialize(float speed, float gradient, float offset, float damage)
    {
        this.speed = speed;
        this.gradient = gradient;
        this.offset = offset;
        this.damage = damage;
    }

    void Update()
    {
        float x = transform.position.x + speed;
        float y = gradient * x + offset;

        transform.position = new Vector3(x, y, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collide with other obj");
        if (atk_stage == ATTACK_STAGE.ATK_STG)
        {
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
}