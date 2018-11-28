using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCounter : MonoBehaviour {

    public int CoinsCollected = 0;
    public bool KeyCard1Collected = false;
    public Text coinsText;
    public Image KeyCard1Image;

	// Use this for initialization
	void Start () {
        UpdateGui();
	}
	
    private void UpdateGui()
    {
        coinsText.text = CoinsCollected.ToString();
        KeyCard1Image.enabled = KeyCard1Collected;
    }



    public void CollectItem(string type)
    {
        if (type == "coin")
        {
            CoinsCollected++;
            UpdateGui();
        }
        if(type=="keycard1")
        {
            KeyCard1Collected = true;
            UpdateGui();
        }
    }

    public int GetCollectedItemCount(string itemType)
    {
        switch (itemType)
        {
            case "coin":
                {
                    return CoinsCollected;
                }break;
            case "keycard1":
                {
                    return KeyCard1Collected ? 1 : 0;
                }break;
           
        }
        return 0;
    }
	
}
