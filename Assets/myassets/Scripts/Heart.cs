using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour {

    public float Health = 2;

    private PlayerStats _stats;
	// Use this for initialization
	void Awake () {
        _stats = FindObjectOfType<PlayerStats>();
	}
	
	public void Collected()
    {
        _stats.Health += Health;
    }
}
