using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropMecha : MonoBehaviour {
    public Mechanismus Mecha;

    private bool _dropped = false;

	// Use this for initialization
	void Start () {
		
	}
	
	public void DropItem()
    {
        if (!_dropped)
        {
            _dropped = true;
            Mecha.Execute();
        }
    }
}
