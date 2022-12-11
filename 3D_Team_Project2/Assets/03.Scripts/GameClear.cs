using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClear : MonoBehaviour
{
    private int SceneNum;
    private float time;
    private bool End = false;
    public GameObject ClearUI;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (SceneNum != 3)
                SceneManager.LoadScene(SceneNum + 1);
            else if (SceneNum == 3)
            {
                End = true;
                Time.timeScale = 0;
                ClearUI.SetActive(true);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SceneNum = SceneManager.GetActiveScene().buildIndex;
        time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (End == true)
            time += Time.unscaledDeltaTime;
        if (time >= 3)
            SceneManager.LoadScene("SelectSc");
    }
}
