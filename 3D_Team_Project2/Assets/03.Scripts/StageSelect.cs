using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour
{
    public GameObject StageUI;
    public GameObject StartUI;
    public void OnClickStartBtn()
    {
        StageUI.SetActive(true);
        StartUI.SetActive(false);
    }
    public void OnClickStage1()
    {
        SceneManager.LoadScene("Stage1Sc");
    }

    public void OnClickStage2()
    {
        SceneManager.LoadScene("Stage2Sc");
    }

    public void OnClickStage3()
    {
        SceneManager.LoadScene("Stage3Sc");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
