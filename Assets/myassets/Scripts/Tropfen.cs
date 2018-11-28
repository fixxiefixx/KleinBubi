using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tropfen : MonoBehaviour {

    public float AttackDistance = 5f;
    public float SeePlayerRange = 10f;
    public AudioSource JumpSound;
    public float Health = 1;
    public GameObject Explosion;
    public Renderer Rend;
    

    private Animator _anim;
    private Player _player;
    private MoveState _state = MoveState.idle;
    private const float _MAXATTACKWAITTIMER = 0.7f;
    private float _attackwaitTimer = 0;
    private Rigidbody _rigid;
    private const float _MAXJUMPTIMER = 0.5f;
    private float _jumpTimer = 0;
    private Material MainMat;
    private bool canBoing = true;

    private enum MoveState
    {
        idle,
        jump,
        prepareJump,
        injury
    }

	// Use this for initialization
	void Start () {
        _anim = GetComponent<Animator>();
        _player = FindObjectOfType<Player>();
        _rigid = GetComponent<Rigidbody>();
        MainMat = Rend.material;
    }
	

	void FixedUpdate () {
        Vector3 playerDir = _player.transform.position - transform.position;
        switch (_state)
        {
            case MoveState.idle:
                {
                    if (AttackDistance * AttackDistance > playerDir.sqrMagnitude && Mathf.Abs(_player.transform.position.y-transform.position.y)<2f)
                    {
                        _state = MoveState.prepareJump;
                        _attackwaitTimer = _MAXATTACKWAITTIMER;
                        
                    }
                }break;
            case MoveState.prepareJump:
                {
                    _attackwaitTimer -= Time.deltaTime;
                    if (_attackwaitTimer <= 0 && _player.isAttackable && canBoing)
                    {
                        canBoing = false;
                        _state = MoveState.jump;
                        Vector3 jumpDir = transform.forward*4f;
                        jumpDir.y = 5f;
                        _rigid.velocity = jumpDir;
                        _jumpTimer = _MAXJUMPTIMER;
                        _anim.SetTrigger("jump");
                        JumpSound.pitch = 0.8f + Random.value * 0.4f;
                        JumpSound.Play();
                        MainMat.SetColor("_EmissionColor", Color.black);
                    }
                    else
                    {
                        float yRot = transform.rotation.eulerAngles.y;
                        float sollYRot = Mathf.Atan2(playerDir.x, playerDir.z)*Mathf.Rad2Deg;
                        yRot = Mathf.MoveTowardsAngle(yRot, sollYRot, Time.deltaTime * 360f);
                        transform.rotation = Quaternion.AngleAxis(yRot, Vector3.up);
                    }
                }break;
            case MoveState.jump:
                {
                    _jumpTimer -= Time.deltaTime;
                    if (_jumpTimer <= 0)
                    {
                        _state = MoveState.idle;
                    }
                }break;
        }
	}

  

    private void Die()
    {

        //GameObject.FindObjectOfType<CameraController>().dotheHarlemShake();
        GameObject.Instantiate(Explosion, transform.position, Explosion.transform.rotation);
        SendMessage("DropItem");
        GameObject.Destroy(gameObject);
    }

    void Damage(float amount)
    {
        //if (_state != MoveState.Death)
        {
            Health -= amount;
            if (Health <= 0)
            {
                Die();

            }else
            {
                _state = MoveState.idle;
                Vector3 jumpdir = transform.position- _player.transform.position;
                jumpdir.y = 0;
                jumpdir.Normalize();
                jumpdir *= 5;
                jumpdir.y = 3;
                _rigid.velocity = jumpdir;
                MainMat.SetColor("_EmissionColor",  Color.red);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //if (_state != MoveState.Death)
        {
            if (collision.collider.tag == "Player")
            {
                if (_state == MoveState.jump)
            {
                _rigid.velocity = new Vector3(_rigid.velocity.x, 5f, _rigid.velocity.z);
            }
            
                if (_player.isAttackable)
                {
                    _player.Damage(1, transform);
                }
            }
        }
        canBoing = true;
    }
}
