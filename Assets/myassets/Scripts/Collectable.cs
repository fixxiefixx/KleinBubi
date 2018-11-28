using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour {


    public float RotationSpeed = 360;
    public string ItemType = "";
    public AudioSource collectSound = null;

    private ItemCounter _counter = null;
    private bool _eingesammelt = false;
    private float _gravity = -40f;
    private float _yspeed = 0;
    private float _ystart = 0;
    private Player _player = null;
    private bool _falling = false;
    private float checkGroundDist = 1f;
    private LayerMask _checkGroundMask;
    private const float _MAXYSPEED = 3f;

    // Use this for initialization
    void Awake () {
        transform.rotation = Quaternion.AngleAxis(Random.value * 360, Vector3.up);
        _counter = FindObjectOfType<ItemCounter>();
        _ystart = transform.position.y;
        _player = FindObjectOfType<Player>();
        _checkGroundMask= LayerMask.GetMask(new string[] { "Default" });
    }

    private bool CheckGround()
    {
        return Physics.Raycast(transform.position, Vector3.down * checkGroundDist, _checkGroundMask);
    }
	
	// Update is called once per frame
	void Update () {
        if (_eingesammelt)
        {
            _yspeed += _gravity * 0.5f * Time.deltaTime;
            Vector3 solloc = transform.position + new Vector3(0, _yspeed * Time.deltaTime, 0);
            float interp = Mathf.Clamp(-_yspeed * 0.05f, 0, 1f);
            transform.position= Vector3.Lerp(solloc, _player.transform.position + new Vector3(0, 1.3f, 0), interp);

            if (interp == 1f)
            {
                GameObject.Destroy(gameObject);
                _gravity = 0;
            }
        }else
        {
            if (_falling)
            {
                if (CheckGround())
                {
                    _yspeed = 0;
                    _falling = false;
                }else
                {
                    _yspeed -= Time.deltaTime * 0.3f;
                    if (_yspeed < -_MAXYSPEED)
                    {
                        _yspeed = -_MAXYSPEED;
                    }
                    transform.position = new Vector3(transform.position.x, transform.position.y + _yspeed, transform.position.z);
                }
            }
        }
        transform.rotation = transform.rotation * Quaternion.AngleAxis(RotationSpeed * Time.deltaTime, Vector3.up);
	}

    public void Collect()
    {
        if (!_eingesammelt)
        {
            if (collectSound != null)
            {
                collectSound.Play();
            }

            SendMessage("Collected", SendMessageOptions.DontRequireReceiver);
            _counter.CollectItem(ItemType);
            //GameObject.Destroy(gameObject);
            _eingesammelt = true;
            RotationSpeed *= 2;
            _yspeed = 10f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_eingesammelt && other.tag == "Player")
        {
            Collect();
        }
    }

    public void FallToGround()
    {
        _falling = true;
    }
}
