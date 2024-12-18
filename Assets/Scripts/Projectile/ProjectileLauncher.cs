using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public sealed class ProjectileLauncher : MonoBehaviour
{
    private new Camera camera;
    public GameObject prefab;
    private List<GameObject> projectiles;
    public float radius = 5f;
    public float speed = 10f;
    public float damage = 1f;
    float gradient, offset; // Represent gradient and offset in linear equation
    public void Initialize(float speed, float gradient, float offset, float damage)
    {
        this.speed = speed;
        this.gradient = gradient;
        this.offset = offset;
        this.damage = damage;
    }

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        projectiles = new List<GameObject>();
    }

    void rotateToMouse() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = camera.WorldToScreenPoint(transform.position).z;
        Vector3 worldMousePos = camera.ScreenToWorldPoint(mousePos);
        Vector3 direction = (worldMousePos - transform.position).normalized;

        // Rotate the object towards the mouse position
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360);
    }

    void getMouseInput() {
        if (Input.GetButtonDown("Fire1"))
        {
            //Debug.Log("SHOOT ! : " + Input.mousePosition);
            var proj = Instantiate(prefab, transform.position, transform.rotation);
            proj.GetComponent<Projectile>().Initialize(speed, radius, damage, transform.position); //other args ??
            //projectiles.Add(proj);
        }
    }

    void moveProjectiles() {
        for (int i = projectiles.Count - 1; i >= 0; i--){
            GameObject proj = projectiles[i];
            if (proj != null){
                proj.transform.position += proj.transform.forward * Time.deltaTime * speed; 
                float distance = Vector3.Distance(transform.position, proj.transform.position);
                if (distance > radius)
                {
                    Destroy(proj);
                    projectiles.RemoveAt(i);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        rotateToMouse();
        getMouseInput();
        
        //moveProjectiles();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2.0f);
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
