using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarRepositioner : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GUIBarScript guiBar = GetComponent<GUIBarScript>();
        guiBar.Position *= Screen.width / 800;
        //guiBar.ScaleSize *= Screen.width / 800;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
