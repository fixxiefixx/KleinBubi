using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	public void startLevel(string name)
    {
        SceneManager.LoadScene(name);
    }
}
