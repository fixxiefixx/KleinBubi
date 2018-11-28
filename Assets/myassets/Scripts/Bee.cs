using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour
{

    public float MaxSpeed = 10;
    public float AttackDistance = 2f;
    public float SeePlayerRange = 10f;
    public float Health = 1f;
    public GameObject Explosion;
    public AudioSource startSound;
    public Renderer Rend;
    public float ForwardForce = 5f;
    public float AdditionalforwardForceOnAttack = 0;
    public float AbovePlayerDist = 3f;
    public bool BossMode = false;
    public GameObject StachelPrefab;
    public Transform stachelStartTransform;
    public GameObject BeePrefab;

    private Material MainMat;


    private Animator _anim;
    private Player _player;
    private Rigidbody _rigid;
    private const float _MAXATTACKTIMER = 1;
    private float _attacktimer = 0;
    private float _MAXDEATHTIMER = 2.5f;
    private float _deathTimer = 0;
    private Vector3 middlePos = Vector3.zero;
    private const int _ANZAHLSTACHELN = 20;
    private int StachelnZuSpawnen = 0;
    private const float _MAXSTACHELDREHTIMER = 0.2f;
    private float _stachelDrehTimer = 0;
    

    private enum MoveState
    {
        FlytoPlayer,
        Attacking,
        PrepareAttack,
        Death,
        Idle,
        FlyToMiddle,
        StachelDrehAttacke,
        PrepareFlyToMiddle
    }

    private MoveState _state = MoveState.Idle;

    // Use this for initialization
    void Start()
    {
        _anim = GetComponent<Animator>();
        _player = FindObjectOfType<Player>();
        _rigid = GetComponent<Rigidbody>();
        MainMat= Rend.materials[0];
        middlePos = transform.position;
    }

    private void Attack()
    {
        _state = MoveState.PrepareAttack;
        
        _attacktimer = _MAXATTACKTIMER;
    }

    private void FlyToPlayer()
    {
        _state = MoveState.FlytoPlayer;
    }

    void FixedUpdate()
    {
        Vector3 playerDir = (_player.transform.position + Vector3.up * AbovePlayerDist) - transform.position;
        bool facingplayer = Vector3.Dot(playerDir, _rigid.velocity) < 0;
        switch (_state)
        {
            case MoveState.FlytoPlayer:
                {
                    
                    if (AttackDistance * AttackDistance > playerDir.sqrMagnitude && _player.isAttackable)
                    {
                        Attack();
                    }
                    else
                    {
                        if (playerDir.normalized.y > 0.9f && (_player.transform.position-transform.position).sqrMagnitude<1f)
                        {
                            Damage(1);
                        }
                        else
                        {
                            _rigid.AddForce(playerDir.normalized * (ForwardForce), ForceMode.Acceleration);
                        }

                        if (_rigid.velocity.sqrMagnitude > 0.001f)
                        {

                            Quaternion sollRot = Quaternion.LookRotation(_rigid.velocity);
                            

                            _rigid.transform.rotation = Quaternion.RotateTowards(_rigid.rotation, sollRot, Time.deltaTime * 360);
                        }
                    }

                }
                break;
            case MoveState.PrepareAttack:
                {
                    Quaternion sollRot = Quaternion.AngleAxis(-90, Vector3.right);
                    _rigid.transform.rotation = Quaternion.RotateTowards(_rigid.rotation, sollRot, Time.deltaTime * 360);
                    _rigid.AddForce(playerDir.normalized * (facingplayer ? 50f : 10f), ForceMode.Acceleration);
                    if(Quaternion.Angle(sollRot, _rigid.rotation)<1){
                        _state = MoveState.Attacking;
                        _rigid.velocity = Vector3.zero;
                    }
                }
                break;
            case MoveState.Attacking:
                {
                    _attacktimer -= Time.deltaTime;
                    if (_attacktimer <= 0)
                    {
                        FlyToPlayer();
                    }
                    else
                    {
                        Quaternion sollRot = Quaternion.AngleAxis(-90, Vector3.right);
                        _rigid.transform.rotation = Quaternion.RotateTowards(_rigid.rotation, sollRot, Time.deltaTime * 360);
                        _rigid.AddForce(Vector3.down * 20+playerDir.normalized*10f, ForceMode.Acceleration);
                    }
                }
                break;
            case MoveState.Idle:
                {
                    if (SeePlayerRange * SeePlayerRange > playerDir.sqrMagnitude)
                    {
                        _state = MoveState.FlytoPlayer;
                        startSound.Play();
                    }else
                    {
                        this.transform.position = this.transform.position + Vector3.up* Mathf.Sin(Time.time*2)*Time.deltaTime;
                    }
                }break;
            case MoveState.Death:
                {
                    _deathTimer -= Time.deltaTime;
                    if (_deathTimer <= 0)
                    {
                        Die();
                    }else
                    {
                        MainMat.SetColor("_EmissionColor", ((int)(Time.time * 10f)) % 2 == 1 ? Color.white : Color.red);
                    }
                }break;
            case MoveState.FlyToMiddle:
                {
                    Vector3 middleDir = middlePos - transform.position;
                    if (0.1f > middleDir.sqrMagnitude)
                    {
                        transform.position = middlePos;
                        startBossAttack();
                    }
                    else
                    {
                        _rigid.velocity = Vector3.zero;
                        transform.position = Vector3.MoveTowards(transform.position, middlePos, Time.deltaTime * 20f);
                        Quaternion sollRot = Quaternion.LookRotation(middlePos-transform.position);

                        _rigid.transform.rotation = Quaternion.RotateTowards(_rigid.rotation, sollRot, Time.deltaTime * 360);
                    }
                }
                break;
            case MoveState.StachelDrehAttacke:
                {
                    _stachelDrehTimer -= Time.deltaTime;
                    if (_stachelDrehTimer <= 0)
                    {
                        _rigid.velocity = Vector3.zero;
                        GameObject stachelGo = (GameObject)GameObject.Instantiate(StachelPrefab, stachelStartTransform.position, transform.rotation);
                        stachelGo.GetComponent<FliegenderStachel>().StartFlyingToDirection(transform.rotation * Vector3.back * 30f);
                        

                        StachelnZuSpawnen--;
                        if (StachelnZuSpawnen <= 0)
                        {
                            _rigid.velocity = Vector3.up * 5f;
                            _state = MoveState.FlytoPlayer;
                        }else
                        {
                            _stachelDrehTimer = _MAXSTACHELDREHTIMER;
                        }
                    }else
                    {
                        float toDir = Quaternion.LookRotation(-playerDir, Vector3.up).eulerAngles.y+Mathf.Sin(Time.time*1.5f)*15f;
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, toDir, 0), Time.deltaTime * 360f);
                    }
                }break;
            case MoveState.PrepareFlyToMiddle:
                {
                    _attacktimer -= Time.deltaTime;
                    if (_attacktimer <= 0)
                    {
                        _state = MoveState.FlyToMiddle;
                    }
                }break;
        }



        if (MaxSpeed * MaxSpeed < _rigid.velocity.sqrMagnitude)
        {
            _rigid.velocity = _rigid.velocity.normalized * MaxSpeed;
        }
    }

    private void startBossAttack()
    {
        int choice = Random.Range(0, 2);
        switch (choice) {
            case 0:
                {
                    transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);//Rotation begradigen
                    _rigid.velocity = Vector3.zero;
                    StachelnZuSpawnen = _ANZAHLSTACHELN;
                    _state = MoveState.StachelDrehAttacke;
                    _stachelDrehTimer = _MAXSTACHELDREHTIMER;
                }break;
            case 1:
                {
                    GameObject beeobj=(GameObject) GameObject.Instantiate(BeePrefab, stachelStartTransform.position, transform.rotation);
                    beeobj.GetComponent<Bee>().FlyToPlayer();
                    beeobj.GetComponent<ItemDropper>().HoldItem = null;
                    beeobj.GetComponent<Rigidbody>().velocity = Vector3.down * 3f;

                    beeobj = (GameObject)GameObject.Instantiate(BeePrefab, stachelStartTransform.position+Vector3.up, transform.rotation);
                    beeobj.GetComponent<Bee>().FlyToPlayer();
                    beeobj.GetComponent<ItemDropper>().HoldItem = null;
                    beeobj.GetComponent<Rigidbody>().velocity = Vector3.up * 10f;

                    _state = MoveState.FlytoPlayer;
                }break;
    }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (_state != MoveState.Death)
        {
            if (_state == MoveState.Attacking)
            {
                _rigid.velocity = new Vector3(_rigid.velocity.x, 5f, _rigid.velocity.z);
                FlyToPlayer();
            }
            if (collision.collider.tag == "Player")
            {
                if (_player.isAttackable)
                {
                    _player.Damage(1, transform);
                }
            }
        }
    }

    private void Die()
    {
        /*_rigid.useGravity = true;
        _rigid.AddForce(Vector3.down, ForceMode.Impulse);
        _state = MoveState.Death;
        _anim.Stop();*/
        GameObject.Instantiate(Explosion, transform.position, Explosion.transform.rotation);
        //GameObject.Instantiate(gameObject, transform.position + new Vector3(0, 8, 0), transform.rotation);
        GameObject.FindObjectOfType<CameraController>().dotheHarlemShake();
        float radius = 5;
        if ((_player.transform.position - transform.position).magnitude < radius)
        {
            _player.Damage(1, transform);
        }

       /* float power = 100f;
        
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(power, explosionPos, 100f, 3.0F);
            _state = MoveState.Death;
            

        }*/

        SendMessage("DropItem",SendMessageOptions.DontRequireReceiver);
        GameObject.Destroy(gameObject);
    }

    private IEnumerator damageBlink()
    {
        yield return new WaitForSeconds(0.5f);
        MainMat.SetColor("_EmissionColor", Color.black);
    }

    void Damage(float amount)
    {
        if (_state != MoveState.Death)
        {
            Health -= amount;
            if (Health <= 0)
            {
                //Die();
                _deathTimer = _MAXDEATHTIMER;
                _state = MoveState.Death;
                _rigid.useGravity = true;
                _rigid.freezeRotation = false;
                _rigid.drag = 0;
                startSound.pitch = 0.5f;
                startSound.Play();
            }else
            {
                MainMat.SetColor("_EmissionColor", Color.red);
                _attacktimer = _MAXATTACKTIMER;
                _state =BossMode?MoveState.PrepareFlyToMiddle: MoveState.Idle;
                StartCoroutine(damageBlink());
                _rigid.velocity = Vector3.up * 10f;
                ForwardForce += AdditionalforwardForceOnAttack;
            }
        }
    }
   
}