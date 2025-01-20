using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextLevel : MonoBehaviour
{
    public int nextSceneId;
    public float waterDepth;

    public Player player;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(player.GetComponent<Transform>().position.y < waterDepth) {
            SceneManager.LoadSceneAsync(nextSceneId);
        }
    }
}
