using UnityEditor.PackageManager;
using UnityEngine;

public sealed class Projectile : MonoBehaviour
{

    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private bool isPreFab;

    Vector3 origin;
    Vector3 direction;
    public float lifeDuration;
    private float remainingTime;
    float speed;
    float damage;

    public void Initialize(float speed, float time, float damage, Vector3 origin)
    {
        this.speed = speed;
        this.lifeDuration = time;
        this.damage = damage;
        this.origin = origin;
    }

    public void Initialize(float speed, float damage, bool preFabFlag)
    {
        this.damage = damage;
        rb.velocity = transform.right * speed;
        isPreFab = preFabFlag;
    }

    // Start is called before the first frame update
    void Start()
    {
        direction = transform.forward;
        //GetComponent<Rigidbody>().AddForce(direction * 1000);
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
        if (!isPreFab)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0)
            {
                Debug.Log("DESTROYED");
                Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        moveProjectiles();
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
