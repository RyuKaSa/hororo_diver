using UnityEditor.PackageManager;
using UnityEngine;

public sealed class Projectile : MonoBehaviour
{
    Vector3 origin;
    Vector3 direction;
    public float lifeDuration;
    private float remainingTime;
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

    public void Initialize(float speed, float time, float damage, Vector3 origin)
    {
        this.speed = speed;
        this.lifeDuration = time;
        this.damage = damage;
        this.origin = origin;
    }

    public void Initialize(float speed, float damage)
    {
        this.speed = speed;
        this.damage = damage;
    }



    // Start is called before the first frame update
    void Start()
    {
        direction = transform.forward;
        GetComponent<Rigidbody>().AddForce(direction * 1000);
        remainingTime = lifeDuration;
    }

    void moveProjectiles()
    {
        if (origin == null)
        {
            Debug.LogError("ERROR ERROR ERROR ORIGIN NULL");
            return;
        }
        // transform.position += direction * Time.deltaTime * speed; 
        // float distance = Vector3.Distance(origin, transform.position);
        remainingTime -= Time.deltaTime;
        if (remainingTime <= 0)
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

    private void OnTriggerEnter(Collider col)
    {

        Debug.Log("Collision !!!");

        if (col.GetComponent<Collider>().tag == "BasicMob")
        {
            // It is object tagged with TagB
            Debug.Log("Collide with BasicMob");
        }
    }

    private void OnTriggerEnterNop(Collider other)
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
