using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextLevel : MonoBehaviour
{
    public int nextSceneId;
    public float waterDepth;

    private bool isLoading;

    public Player player;
    // Start is called before the first frame update
    void Start()
    {
        isLoading = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isLoading && player.GetComponent<Transform>().position.y < waterDepth) {
            isLoading = true;
            SceneManager.LoadSceneAsync(nextSceneId);
        }
    }
}
