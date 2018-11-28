using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloud : MonoBehaviour {

    public Animator Anim_leftEye;
    public Animator Anim_rightEye;
    public float FlyToPlayerkDist = 5;
    public float AttackDist = 1f;
    public float flySpeed = 3f;
    public Renderer Rend;
    public GameObject Lightning;
    public GameObject Explosion;

    private MoveState _state = MoveState.idle;
    private Player _player = null;
    private float angryness = 0;
    private float blackness = 0;
    private Material cloudmat;
    private Vector3 _startPos;
    private const float _MAXFLYTOPLAYERTIMER = 10;
    private float _flyToPlayerTimer = 0;

    private enum MoveState
    {
        idle,
        flyToPlayer,
        getAngry,
        getFrindly
    }

    // Use this for initialization
    void Start () {
        //Anim_leftEye.SetTrigger("open");
        //Anim_rightEye.SetTrigger("open");
        _player = GameObject.FindObjectOfType<Player>();
        cloudmat= Rend.materials[0];
        _startPos = transform.position;
    }
	
    private void FlyToPlayer()
    {
        _state = MoveState.flyToPlayer;
        _flyToPlayerTimer = _MAXFLYTOPLAYERTIMER;
    }

    private void GetAngry()
    {
        _state = MoveState.getAngry;
        Anim_leftEye.SetTrigger("slash");
        Anim_rightEye.SetTrigger("backslash");
    }
    public void GetFriendly()
    {
        _state = MoveState.getFrindly;
        Anim_leftEye.SetTrigger("open");
        Anim_rightEye.SetTrigger("open");
        Lightning.SetActive(false);
    }
    private void Idle()
    {
        _state = MoveState.idle;
        //Anim_leftEye.SetTrigger("open");
        //Anim_rightEye.SetTrigger("open");
    }

	void Update () {
        Vector3 targetLoc = _player.transform.position + Vector3.up * 3f;
        Vector3 playerDir = targetLoc - transform.position;
        switch (_state)
        {
            case MoveState.idle:
                {
                    if (playerDir.sqrMagnitude < FlyToPlayerkDist * FlyToPlayerkDist)
                    {
                        FlyToPlayer();
                    }
                }break;
            case MoveState.flyToPlayer:
                {
                    _flyToPlayerTimer -= Time.deltaTime;
                    if (_flyToPlayerTimer <= 0)
                    {
                        GetFriendly();
                    }
                    else
                    {
                        if (playerDir.sqrMagnitude < AttackDist * AttackDist && _player.isInAttackableState)
                        {
                            GetAngry();
                            Lightning.SetActive(true);
                        }
                        else
                        {
                            this.transform.position = Vector3.MoveTowards(transform.position, targetLoc, Time.deltaTime * flySpeed);
                        }
                    }
                }break;
            case MoveState.getAngry:
                {
                    angryness += Time.deltaTime*0.2f;
                    blackness += Time.deltaTime*2;
                    if (blackness > 1)
                        blackness = 1;
                    if(angryness>1)
                    {
                        angryness = 1;
                        
                        GetFriendly();
                    }else
                    {
                        this.transform.position = Vector3.MoveTowards(transform.position, targetLoc, Time.deltaTime * flySpeed*1.5f);
                        this.transform.position = this.transform.position + Vector3.up * Mathf.Sin(Time.time * 40) * Time.deltaTime*1f;

                    }
                    cloudmat.SetColor("_Color", Color.Lerp(Color.white, Color.black, blackness));
                }break;
            case MoveState.getFrindly:
                {
                    angryness -= Time.deltaTime * 0.2f;
                    blackness -= Time.deltaTime;
                    if (blackness < 0)
                        blackness = 0;
                    if (angryness <= 0)
                    {
                        
                        angryness = 0;
                        if ((_startPos - transform.position).sqrMagnitude < 0.01f)
                        {
                            Idle();
                        }
                    }
                    this.transform.position = Vector3.MoveTowards(transform.position, _startPos, Time.deltaTime * flySpeed);
                    cloudmat.SetColor("_Color", Color.Lerp(Color.white, Color.black, blackness));
                }
                break;
        }
	}

    

    void Damage(float amount)
    {
        GameObject.Instantiate(Explosion, transform.position, Explosion.transform.rotation);
        SendMessage("DropItem");
        GameObject.Destroy(gameObject);

    }
}
