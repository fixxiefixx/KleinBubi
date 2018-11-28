using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloudLightningTrigger : MonoBehaviour
{
    public cloud cloud;
    // Use this for initialization
    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage(1, transform);
                cloud.GetFriendly();
            }
        }
    }
}
