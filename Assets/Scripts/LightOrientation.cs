using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOrientation : MonoBehaviour
{
    public Rigidbody2D rigidbody;
    // Start is called before the first frame update
    void Start()
    {       
    }

    // Update is called once per frame
    void Update()
    {
        if(rigidbody.velocity.magnitude > 0.1) {
            // float angle = Vector2.Angle( Vector2.down, rigidbody.velocity); // in degrees
            float angle = Mathf.Atan2( rigidbody.velocity.y, rigidbody.velocity.x ); // in radians
            transform.rotation = Quaternion.Euler(0, 0, angle * 180 / Mathf.PI + 90);
        }
    }
}
