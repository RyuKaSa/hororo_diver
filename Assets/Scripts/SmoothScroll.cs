using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SmoothScroll : MonoBehaviour
{
    public InputAction key;
    // Start is called before the first frame update
    // private Vector3 offset;
    public float speed;
    public float accelerationFactor;

    public bool isActif = true;

    void Start()
    {
        // offset = new Vector3(0, speed, 0);
        key.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isActif) {
            return;
        }
        float finalSpeed = speed;

        float action = key.ReadValue<float>();
        if(action > 0.5) {
            // Debug.Log("pressed");
            finalSpeed *= accelerationFactor;
        }
        transform.position += new Vector3(0, finalSpeed, 0) * Time.deltaTime;
    }
}
