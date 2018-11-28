using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FliegenderStachel : MonoBehaviour {

    private Rigidbody _rigid;
    private Player _player;
    private const float _MAXALIVETIMER = 10f;
    private float _aliveTmer = 0;

	// Use this for initialization
	void Awake () {
        _rigid = GetComponent<Rigidbody>();
        _player = FindObjectOfType<Player>();
        _aliveTmer = _MAXALIVETIMER;
	}
	
	public void StartFlyingToDirection(Vector3 dir)
    {
        _rigid.velocity = dir;
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            _player.Damage(1, transform);
        }
        GameObject.Destroy(gameObject);
    }

    void FixedUpdate()
    {
        _aliveTmer -= Time.deltaTime;
        if (_aliveTmer <= 0)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
