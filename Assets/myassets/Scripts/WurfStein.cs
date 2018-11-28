using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WurfStein : MonoBehaviour {

    private Rigidbody _rigid;
    private const float _MAXLIVETIMER = 5f;
    private float _liveTimer = 0;
    private Renderer[] _renderers;
    private bool[] _renderersDefaultVisible;
    private Player _player;
    private Steinmann _vonSteinMann = null;
    private bool _throwingBack=false;

    // Use this for initialization
    void Awake () {
        _liveTimer = _MAXLIVETIMER;
        _player = FindObjectOfType<Player>();
        _rigid = GetComponent<Rigidbody>();

        _renderers = GetComponentsInChildren<Renderer>();
        _renderersDefaultVisible = new bool[_renderers.Length];
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderersDefaultVisible[i] = _renderers[i].enabled;
        }
    }

    public void SetVisible(bool visible)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].enabled = visible && _renderersDefaultVisible[i];
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (_throwingBack)
        {
            if (_vonSteinMann != null)
            {
                Vector3 dir = (_vonSteinMann.transform.position + Vector3.up * 2f) - _rigid.position;
                if (dir.magnitude < 2f)
                {
                    _vonSteinMann.VonSteingetroffen();
                    GameObject.Destroy(gameObject);
                }
                else
                {
                    _rigid.velocity = dir.normalized * 15f;
                }
            }else
            {
                _throwingBack = false;
            }
        }
        else
        {
            _liveTimer -= Time.deltaTime;
            if (_liveTimer <= 0)
            {
                GameObject.Destroy(gameObject);
            }
            else
            {
                if (_liveTimer < 0.5f)
                    SetVisible(((int)(Time.time * 20f)) % 2 == 1);
            }
        }
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player" && !_throwingBack)
        {
            _player.Damage(1, transform);
            if(_vonSteinMann!=null)
                _vonSteinMann.SteinSpielerGetroffen();
        }
        //GameObject.Destroy(gameObject);
    }

    public void ThrowInDirection(Vector3 dir,Steinmann from)
    {
        _rigid.velocity = dir;
        _vonSteinMann = from;
    }

    void Damage(float amount)
    {
        _throwingBack = true;
    }
}
