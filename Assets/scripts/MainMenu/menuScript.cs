using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menuScript : MonoBehaviour
{
    public void loadMenu( string levelName )
    {
        SceneManager.LoadScene(levelName);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
