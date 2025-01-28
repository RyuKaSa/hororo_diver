using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class IntroLoadNextLevel : MonoBehaviour
{
    public float maxHeight;

    public int nextSceneId;

    private bool isLoading;
    void Start()
    {
        // offset = new Vector3(0, speed, 0);
        isLoading = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isLoading && transform.position.y >= maxHeight) {
            isLoading = true;
            SceneManager.LoadSceneAsync(nextSceneId);
        }
    }
}
