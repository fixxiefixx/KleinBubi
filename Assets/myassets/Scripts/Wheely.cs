using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheely : MonoBehaviour {

    public Animator Anim_leftEye;
    public Animator Anim_rightEye;
    public Transform Wheel;

    public float AttackDistance = 7f;
    public float Health = 3;
    public GameObject Explosion;
    public AudioSource WheelSound;

    private MoveState _state = MoveState.idle;
    private float _rollSpeed = 0;
    private const float _MAXROLLSPEED = 500f;
    private Player _player;
    private Rigidbody _rigid;
    private const float _MAXSPEED = 10f;
    private const float _MAXROLLTIMER = 3;
    private float _rollTimer = 0;
    private Material MainMat;
    private const float _MAXIDLETIMER = 1f;
    private float _idleTimer = 0;
    private Vector3 lastPos = Vector3.zero;

    private enum MoveState
    {
        idle,
        warmlaufen,
        rollen
    }

    // Use this for initialization
    void Start () {
        Anim_leftEye.SetTrigger("slash");
        Anim_rightEye.SetTrigger("backslash");
        _player = FindObjectOfType<Player>();
        _rigid = GetComponent<Rigidbody>();
        MainMat = Wheel.GetComponentInChildren<Renderer>().material;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 playerDir = _player.transform.position - transform.position;
        switch (_state)
        {
            case MoveState.idle:
                {
                    _idleTimer -= Time.deltaTime;
                    if (_idleTimer <= 0)
                    {
                        MainMat.SetColor("_EmissionColor", Color.black);
                        if (AttackDistance * AttackDistance > playerDir.sqrMagnitude && Mathf.Abs(_player.transform.position.y-transform.position.y)<2f)
                        {
                            _state = MoveState.warmlaufen;
                            WheelSound.Play();
                            Anim_leftEye.SetTrigger("slash");
                            Anim_rightEye.SetTrigger("backslash");
                        }
                    }
                }
                break;
            case MoveState.warmlaufen:
                {
                    _rollSpeed += Time.deltaTime*600f;
                    if (_rollSpeed > _MAXROLLSPEED)
                    {
                        _rollSpeed = _MAXROLLSPEED;
                        _state = MoveState.rollen;
                        _rollTimer = _MAXROLLTIMER;
                        
                    }
                    else
                    {
                        float yRot = transform.rotation.eulerAngles.y;
                        float sollYRot = Mathf.Atan2(playerDir.x, playerDir.z) * Mathf.Rad2Deg;
                        yRot = Mathf.MoveTowardsAngle(yRot, sollYRot, Time.deltaTime * 360f);
                        transform.rotation = Quaternion.AngleAxis(yRot, Vector3.up);
                    }
                }break;
            case MoveState.rollen:
                {
                    _rollTimer -= Time.deltaTime;
                    if (_rollTimer <= 0)
                    {
                        _state = MoveState.idle;
                        _rollSpeed = 0;
                    }
                    else
                    {
                        float yRot = transform.rotation.eulerAngles.y;
                        float sollYRot = Mathf.Atan2(playerDir.x, playerDir.z) * Mathf.Rad2Deg;
                        yRot = Mathf.MoveTowardsAngle(yRot, sollYRot, Time.deltaTime * 200f);
                        transform.rotation = Quaternion.AngleAxis(yRot, Vector3.up);

                        Vector3 dir = transform.forward * 30f;
                        _rigid.AddForce(dir, ForceMode.Force);
                        if (_rigid.velocity.sqrMagnitude > _MAXSPEED * _MAXSPEED)
                        {
                            _rigid.velocity = _rigid.velocity.normalized * _MAXSPEED;
                        }
                        
                    }
                    
                }break;
        }
        if (_rollSpeed > 0.01f)
        {
            Wheel.rotation = Wheel.rotation * Quaternion.AngleAxis(_rollSpeed * Time.deltaTime, Vector3.right);
            
        }
        if (!_rigid.IsSleeping() && checkoutOfBounds())
        {
            _rigid.position = lastPos;
            //_rigid.velocity = new Vector3(_rigid.velocity.x * -0.5f, _rigid.velocity.y, _rigid.velocity.z * -0.5f);
            _rigid.velocity = new Vector3(0, 0, 0);
        }
        lastPos = _rigid.position;
    }

    private bool checkoutOfBounds()
    {
        RaycastHit hit;
        if(Physics.Raycast(_rigid.position, Vector3.down,out hit,3f,LayerMask.GetMask(new string[] { "Default" })))
        {
            return Vector3.Angle(hit.normal,Vector3.up)>45f;
        }
        return true;
    }

    private void Die()
    {

        //GameObject.FindObjectOfType<CameraController>().dotheHarlemShake();
        GameObject.Instantiate(Explosion, transform.position, Explosion.transform.rotation);
        SendMessage("DropItem");
        GameObject.Destroy(gameObject);
    }

    private void fallBack()
    {
        _state = MoveState.idle;
        _idleTimer = _MAXIDLETIMER;
        Vector3 jumpdir = transform.position - _player.transform.position;
        jumpdir.y = 0;
        jumpdir.Normalize();
        jumpdir *= 5;
        jumpdir.y = 3;
        _rigid.velocity = jumpdir;
        _rollSpeed=0;
    }

    void Damage(float amount)
    {
        //if (_state != MoveState.Death)
        {
            Health -= amount;
            if (Health <= 0)
            {
                Die();

            }
            else
            {
                Anim_leftEye.SetTrigger("open");
                Anim_rightEye.SetTrigger("open");
                fallBack();
                MainMat.SetColor("_EmissionColor", Color.red);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //if (_state != MoveState.Death)
        {
            if (collision.collider.tag == "Player")
            {
                /*if (_state == MoveState.jump)
                {
                    _rigid.velocity = new Vector3(_rigid.velocity.x, 5f, _rigid.velocity.z);
                }*/

                if (_player.isAttackable)
                {
                    _player.Damage(1, transform);
                    fallBack();
                }
            }
        }
    }
}
