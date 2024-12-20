using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadMap : MonoBehaviour
{
    public int level;

    public Transform playerTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider col) {
        Debug.Log("Collision !!!");

        if (col.GetComponent<Collider>().tag == "Player")
        {
            // It is object tagged with TagB
            Debug.Log("Collide with Player");
            // SceneManager.LoadSceneAsync(level);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((transform.position - playerTransform.position).magnitude < 5.0) {
            SceneManager.LoadSceneAsync(level);
        }
    }
}
