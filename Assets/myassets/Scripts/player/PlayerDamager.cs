using UnityEngine;
using System.Collections;

public class PlayerDamager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}




    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            other.gameObject.SendMessage("Damage", 1f);
        }

        if (other.gameObject.tag == "door")
        {
            other.gameObject.SendMessage("doorHit");
        }
    }
    
}
