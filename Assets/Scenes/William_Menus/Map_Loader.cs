using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map_Loader : MonoBehaviour
{
    public int levelID;
    public void PlayGame()
    {
        // 0 is menu
        // 1 is game test
        SceneManager.LoadSceneAsync(levelID);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
