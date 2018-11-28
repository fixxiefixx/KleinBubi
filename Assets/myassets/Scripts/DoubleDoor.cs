using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleDoor : MonoBehaviour
{

    public Animator Anim1;
    public Animator Anim2;
    public string RequiredItem = "";
    public float DistanceStayOpened = 5;

    private bool _opened = false;
    private ItemCounter _itemCounter;
    private Player _player;
    private float _distanceStayOpenedSqr = 0;

    // Use this for initialization
    void Start()
    {
        _itemCounter = FindObjectOfType<ItemCounter>();
        _player = FindObjectOfType<Player>();
        _distanceStayOpenedSqr = DistanceStayOpened * DistanceStayOpened;
    }

    public void Open()
    {
        if (!_opened)
        {
            Anim1.SetBool("open", true);
            Anim2.SetBool("open", true);
            _opened = true;
        }
    }

    public void Close()
    {
        if (_opened)
        {
            Anim1.SetBool("open", false);
            Anim2.SetBool("open", false);
            _opened = false;
        }
    }

    void FixedUpdate()
    {
        if (_opened)
        {
            if ((_player.transform.position - transform.position).sqrMagnitude > _distanceStayOpenedSqr)
            {
                Close();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            bool playerHasItem = RequiredItem == "";
            if (!playerHasItem)
            {
                playerHasItem = _itemCounter.GetCollectedItemCount(RequiredItem) > 0;
            }
            if (playerHasItem)
            {
                Open();
            }
        }
    }
}
