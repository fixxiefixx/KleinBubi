using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Steinmann : MonoBehaviour {

    public float PlayerSeeDistance = 15f;
    public float PlayerPunchDistance = 3f;
    public GameObject SteinInHand;
    public GameObject WurfSteinPrefab;
    public float Health=2f;
    public AudioSource SteinWerfSound;
    public AudioSource FreuSound;
    public AudioSource DieSound;
    public bool BossMode = false;

    private Animator _anim;
    private Rigidbody _rigid;
    private MoveState _state = MoveState.idle;
    private Player _player;
    private NavMeshAgent _agent = null;
    private CameraController _camCont = null;


    private const float _MAXSTEINSAMMELTIMER = 0.5f;
    private const float _MAXSTEINWERFTIMER = 2f;
    private float _steinSammeltimer = 0;
    private float _steinWerfTimer = 0;
    private bool _steinWerfTimerActive = false;
    private bool _steinSammelTimerActive = false;
    private const float _MAXSTEINNACHGUCKTIMER = 1.5f;
    private float _steinNachguckTimer = 0;
    private const float _MAXFREUTIMER = 1.5f;
    private float _freuTimer = 0;
    private const float _MAXDIETIMER = 2f;
    private float _dieTimer = 0;
    private Renderer[] _renderers;
    private bool[] _renderersDefaultVisible;
    private const float _MAXPUNCHTIMER = 0.5f;
    private float _punchTimer = 0;
    private const float _MAXWALKTIMER = 0.5f;
    private float _walkTimer = 0;
    private const float _MAXERDBEBENTIMER = 0.5f;
    private float _erdbebenTimer = 0;
    private const float _MAXDANCETIMER = 10;
    private float _danceTimer = 0; 
    

    private enum MoveState
    {
        idle,
        steinheb,
        SteinNachGuck,
        freu,
        die,
        punch,
        walkToPlayer,
        dancing
    }

	// Use this for initialization
	void Start () {
        _anim = GetComponent<Animator>();
        _rigid = GetComponent<Rigidbody>();
        _player = FindObjectOfType<Player>();
        _agent = GetComponent<NavMeshAgent>();
        _camCont = FindObjectOfType<CameraController>();

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

    private void StartThrow()
    {
        _state = MoveState.steinheb;
        _anim.CrossFadeInFixedTime("throw", 0.2f);
        _steinSammeltimer = _MAXSTEINSAMMELTIMER;
        _steinWerfTimer = _MAXSTEINWERFTIMER;
        _steinWerfTimerActive = true;
        _steinSammelTimerActive = true;
        SteinWerfSound.Play();
    }

    private void StartPunch()
    {
        _state = MoveState.punch;
        _punchTimer = _MAXPUNCHTIMER;
        _anim.CrossFadeInFixedTime("punch", 0.2f);
        _player.Damage(1, transform);
    }

    private void StartWalkToPlayer()
    {
        _agent.SetDestination(_player.transform.position);
        _agent.Resume();
        if (_agent.hasPath)
        {
            _state = MoveState.walkToPlayer;
            _walkTimer = _MAXWALKTIMER;
            _anim.CrossFadeInFixedTime("walk", 0.2f);
        }else
        {
            _anim.CrossFadeInFixedTime("idle", 0.2f);
        }
    }

    private void StartDancing()
    {
        _anim.CrossFadeInFixedTime("freu", 0.2f);
        _erdbebenTimer = _MAXERDBEBENTIMER;
        _danceTimer = _MAXDANCETIMER;
        _state = MoveState.dancing;
    }

	void FixedUpdate () {
        Vector3 playerDir = _player.transform.position - transform.position;
        switch (_state)
        {
            case MoveState.idle:
                {
                    if (_player.isInAttackableState)
                    {
                        if (playerDir.sqrMagnitude < PlayerPunchDistance * PlayerPunchDistance)
                        {
                            StartPunch();
                        }
                        else
                            if (playerDir.sqrMagnitude < PlayerSeeDistance * PlayerSeeDistance)
                        {
                            StartThrow();
                        }else
                        if(BossMode)
                        {
                            StartWalkToPlayer();
                        }
                    }
                }break;
            case MoveState.steinheb:
                {
                    float yRot = transform.rotation.eulerAngles.y;
                    float sollYRot = Mathf.Atan2(playerDir.x, playerDir.z) * Mathf.Rad2Deg;
                    yRot = Mathf.MoveTowardsAngle(yRot, sollYRot, Time.deltaTime * 360f);
                    transform.rotation = Quaternion.AngleAxis(yRot, Vector3.up);

                    if (_steinSammelTimerActive)
                    {
                        _steinSammeltimer -= Time.deltaTime;
                        if (_steinSammeltimer <= 0)
                        {
                            _steinSammelTimerActive = false;
                            SteinInHand.SetActive(true);
                        }
                    }
                    if (_steinWerfTimerActive)
                    {
                        _steinWerfTimer -= Time.deltaTime;
                        if (_steinWerfTimer <= 0)
                        {
                             WurfStein ws=((GameObject) GameObject.Instantiate(WurfSteinPrefab, SteinInHand.transform.position, Quaternion.identity)).GetComponent<WurfStein>();

                            Vector3 dir = _player.transform.position - SteinInHand.transform.position;

                            ws.ThrowInDirection(dir.normalized * 15f,this);
                            _anim.CrossFadeInFixedTime("idle", 0.2f);
                            SteinInHand.SetActive(false);
                            _state = MoveState.SteinNachGuck;
                            _steinNachguckTimer = _MAXSTEINNACHGUCKTIMER;
                        }
                    }
                }break;
            case MoveState.SteinNachGuck:
                {
                    _steinNachguckTimer -= Time.deltaTime;
                    if (_steinNachguckTimer <= 0)
                    {
                        _state = MoveState.idle;
                    }
                }break;
            case MoveState.freu:
                {
                    _freuTimer -= Time.deltaTime;
                    if (_freuTimer <= 0)
                    {
                        _anim.CrossFadeInFixedTime("idle", 0.2f);
                        _state = MoveState.idle;
                    }
                } break;
            case MoveState.die:
                {
                    _dieTimer -= Time.deltaTime;
                    if (_dieTimer <= 0)
                    {
                        Health -= 1;
                        if (Health <= 0.001f)
                        {
                            SendMessage("DropItem", SendMessageOptions.DontRequireReceiver);
                            GameObject.Destroy(gameObject);
                        }else
                        {
                            SetVisible(true);
                            _anim.CrossFadeInFixedTime("idle", 0.2f);
                            _state = MoveState.idle;
                            if(BossMode)
                                StartDancing();
                        }
                    }else
                    {
                        if (_dieTimer < 0.5f)
                            SetVisible(((int)(Time.time * 20f)) % 2 == 1);
                    }
                }break;
            case MoveState.punch:
                {
                    _punchTimer -= Time.deltaTime;
                    if (_punchTimer <= 0)
                    {
                        _anim.CrossFadeInFixedTime("idle", 0.2f);
                        _state = MoveState.idle;
                    }
                }break;
            case MoveState.walkToPlayer:
                {
                    _walkTimer -= Time.deltaTime;
                    if(_walkTimer<=0)
                    {
                        _walkTimer = _MAXWALKTIMER;
                        if (playerDir.sqrMagnitude < PlayerSeeDistance * PlayerSeeDistance)
                        {
                            _agent.Stop();
                            StartThrow();
                        }
                        else
                        {
                            
                            if (!_agent.SetDestination(_player.transform.position))
                            {
                                _anim.CrossFadeInFixedTime("idle", 0.2f);
                                _state = MoveState.idle;
                            }
                        }
                    }
                }break;
            case MoveState.dancing:
                {
                    _danceTimer -= Time.deltaTime;
                    if (_danceTimer <= 0)
                    {
                        _state = MoveState.idle;
                        _anim.CrossFadeInFixedTime("idle", 0.2f);
                    }else
                    {
                        _erdbebenTimer -= Time.deltaTime;
                        if (_erdbebenTimer <= 0)
                        {
                            _erdbebenTimer = _MAXERDBEBENTIMER;
                            _camCont.dotheHarlemShake(0.5f);
                            DropFallingStone();
                            DieSound.Play();
                        }
                    }
                }break;
        }
	}

    private void DropFallingStone()
    {
        WurfStein ws = ((GameObject)GameObject.Instantiate(WurfSteinPrefab, _player.transform.position+Vector3.up*20f+new Vector3(Random.Range(-4f,4f),0, Random.Range(-4f, 4f)), Quaternion.identity)).GetComponent<WurfStein>();

        Vector3 dir = Vector3.down;

        ws.ThrowInDirection(dir.normalized * 15f, null);
    }

    public void VonSteingetroffen()
    {
        SteinWerfSound.Stop();
        if (_agent != null)
        {
            _agent.Stop();
        }
        _state = MoveState.die;
        _anim.CrossFadeInFixedTime("die", 0.2f);
        _dieTimer = _MAXDIETIMER;
        DieSound.Play();
    }

    public void SteinSpielerGetroffen()
    {
        if (_state == MoveState.SteinNachGuck)
        {
            _anim.CrossFadeInFixedTime("freu", 0.2f);
            _state = MoveState.freu;
            _freuTimer = _MAXFREUTIMER;
            FreuSound.Play();
        }
    }
}
