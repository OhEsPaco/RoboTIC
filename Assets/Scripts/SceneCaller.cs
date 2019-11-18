using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCaller : MonoBehaviour
{

    public void LoadLevel(string pName)
    {
        SceneManager.LoadScene(pName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
