using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

    public float StartHealth = 10;
    public GUIBarScript HealthBar;

    private float _health = 0;

    public float Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            if (_health > StartHealth)
                _health = StartHealth;
            HealthBar.Value = _health / StartHealth;
        }
    }

	// Use this for initialization
	void Start () {
        _health = StartHealth;
	}
	
	
}
