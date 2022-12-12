using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string sceneName = "MainMenu";

    public void ClickStart()
    { 
        SceneManager.LoadScene("Stage1Sc");
    }

    public void ClickEnd()
    {
        Application.Quit();
    }

}
