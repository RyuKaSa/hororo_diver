using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ScrollText : MonoBehaviour
{
    public InputAction key;
    // Start is called before the first frame update
    // private Vector3 offset;
    public float speed;
    public float accelerationFactor;

    public float maxHeight;

    public int nextSceneId;
    void Start()
    {
        // offset = new Vector3(0, speed, 0);
        key.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        float finalSpeed = speed;

        float action = key.ReadValue<float>();
        if(action > 0.5) {
            // Debug.Log("pressed");
            finalSpeed *= accelerationFactor;
        }
        transform.position += new Vector3(0, finalSpeed, 0) * Time.deltaTime;

        if(transform.position.y >= maxHeight) {
            SceneManager.LoadSceneAsync(nextSceneId);
        }
    }
}
