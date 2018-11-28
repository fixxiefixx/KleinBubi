using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropper : MonoBehaviour {

    public GameObject HoldItem;

    private bool _dropped = false;

    // Use this for initialization
    void Start () {
		
	}
	
	public void DropItem()
    {
        if (HoldItem != null && !_dropped)
        {
            GameObject holdItemInst = GameObject.Instantiate(HoldItem, transform.position, HoldItem.transform.rotation);
            Collectable collectable = holdItemInst.GetComponent<Collectable>();
            if (collectable != null)
            {
                //collectable.FallToGround();
                collectable.Collect();
            }
            _dropped = true;
        }
    }
}
