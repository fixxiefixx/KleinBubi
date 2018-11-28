using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    public GameObject MenuObject;

	// Use this for initialization
	void Start () {
        Cursor.visible = false;
       
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Start") && FindObjectOfType<Endscreen>()==null)
        {
            ToggleMenu();
        }
	}

    public void ToggleMenu()
    {
        MenuObject.SetActive(!MenuObject.activeSelf);
        if (MenuObject.activeSelf)
        {
            Cursor.visible = true;
            Time.timeScale = 0;
        }else
        {
            Cursor.visible = false;
            Time.timeScale = 1;
        }
    }

    public void LevelSelect()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ReplayLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }else
        {
            LevelSelect();
        }
    }

    void OnDestroy()
    {
        Time.timeScale = 1;
    }
}
