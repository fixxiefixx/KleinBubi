using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(DXStateMachine))]
public class Player : MonoBehaviour {

    public float slopeLimit = 46;
    public Animator animator = null;
    public float JoyDeadZone = 0.01f;
    public float PushPower = 10f;
    public bool Movement2d=false;
    public MeleeWeaponTrail trail;
    public Collider AttackCollider;
    public bool CanMove = true;
    

    //Sounds
    public AudioSource jumpSound;
    public AudioSource doublejumpSound;
    public AudioSource climbSound;
    public AudioSource swordSound;
    public AudioSource auSound;
    public AudioSource ertrinkenSound;

    internal Vector3 velocity = Vector3.zero;
    internal DXStateMachine machine = null;
    
    internal Vector3 joyVec = Vector3.zero;
    internal Vector3 joyWorldVec = Vector3.zero;
    public bool onGround = false;
    internal float slopeAngle = 0;
    internal Vector3 groundPos = Vector3.zero;
    internal Vector3 normal = Vector3.up;
    internal bool jumpPressed = false;
    internal bool attackPessed = false;
    internal bool wallSlided = false;
    internal Vector3 climbOffset = Vector3.zero;
    internal Vector3 lastPos = Vector3.zero;
    internal bool AirJumped = false;
    internal bool AirAttacked = false;
    internal bool stickToGround = false;
    internal Collider groundCollider = null;
    internal Vector3 laststandpos = Vector3.zero;
    internal bool HasHanged = false;

    public bool isAttackable
    {
        get
        {
            return isInAttackableState && CanMove && !_blinking;
        }
    }

    internal bool isInAttackableState = true;

    private CharacterController m_characterController = null;
    private Camera cam = null;
    //private GvrViewer _gvr=null;
    private GroundChecker groundChecker;
    private const float _MAXPUSHTIMER= 0.2f;
    private float _pushTimer=0;
    private PlayerStats _stats;
    private const float _MAXBLINKTIMER = 1f;
    private float _blinkTimer = 0;
    private bool _blinking = false;
    private Renderer[] _renderers;
    private bool[] _renderersDefaultVisible;
    

    private const float maxVerticalspeed = 20f;
    private Vector3 _startPos;
    private float pushAngle = 0;
    private Vector2 touchJoy = Vector2.zero;

    public enum ActionButton
    {
        none,
        jump,
        hand
    }

    //States
    internal PlayerJumpnRunState jumpnRunState;
    internal PlayerFallState fallState;
    //internal PlayerSlideState slideState;
    internal PlayerWallSlideState WallSlideState;
    internal PlayerHangState hangState;
    internal PlayerClimbState climbState;
    internal PlayerPushState pushState;
    internal PlayerSwordAttackState swordAttackstate;
    internal PlayerInjuryState injuryState;
    internal PlayerErtrinkenState ertrinkenState;

    private void InitStates()
    {
        jumpnRunState = new PlayerJumpnRunState(gameObject);
        fallState = new PlayerFallState(gameObject);
        //slideState = new PlayerSlideState(gameObject);
        WallSlideState = new PlayerWallSlideState(gameObject);
        hangState = new PlayerHangState(gameObject);
        climbState = new PlayerClimbState(gameObject);
        pushState = new PlayerPushState(gameObject);
        swordAttackstate = new PlayerSwordAttackState(gameObject);
        injuryState = new PlayerInjuryState(gameObject);
        ertrinkenState = new PlayerErtrinkenState(gameObject);
    }

        // Use this for initialization
        void Start () {
        m_characterController = GetComponent<CharacterController>();
        machine = GetComponent<DXStateMachine>();
        cam = FindObjectOfType<Camera>();
        //_gvr = FindObjectOfType<GvrViewer>();
        groundChecker = GetComponent<GroundChecker>();
        _stats = FindObjectOfType<PlayerStats>();
        _renderers = GetComponentsInChildren<Renderer>();
        _renderersDefaultVisible = new bool[_renderers.Length];
        for(int i = 0; i < _renderers.Length; i++)
        {
            _renderersDefaultVisible[i] = _renderers[i].enabled;
        }

        InitStates();
        machine.State = jumpnRunState;
        _startPos = transform.position;
        laststandpos = _startPos;
    }

    public void SetVisible(bool visible)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].enabled = visible && _renderersDefaultVisible[i];
        }
    }

    public void moveToGround()
    {
        if (onGround)
            transform.position = new Vector3(transform.position.x, groundPos.y+m_characterController.height*0.5f, transform.position.z);
    }

    public void Jump(float speed)
    {
        jumpnRunState.lastGroundTime = 0;
        velocity.y = speed;
        onGround = false;
        machine.State = fallState;
        animator.SetTrigger("jump");
        jumpSound.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
            jumpPressed = true;
        if (Input.GetButtonDown("Fire1"))
            attackPessed = true;
        /*if (Input.GetButtonDown("Fire"))
            firePressed = true;*/

        

        machine.StateUpdate();

        GlobalPostUpdate();
    }

    public Vector3 ProbeGroundNormal(out float pslopeAngle,out Vector3 groundPos)
    {
        groundPos = Vector3.zero;
        Vector3 center = m_characterController.transform.position + m_characterController.center;
        Vector3 normal = Vector3.up;
        RaycastHit hit;
        pslopeAngle = 0;
        Ray ray = new Ray(center, -transform.up);
        float groundDist = m_characterController.height * 0.5f + 0.1f;
        if (Physics.SphereCast(center, m_characterController.radius, -Vector3.up, out hit, groundDist + 1 - m_characterController.radius))
        {

            //if (ySpeed < 0 && hit.distance <= groundDist)
            {
                groundPos = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                pslopeAngle = Vector3.Angle(Vector3.up, hit.normal);
                normal = hit.normal;
            }
        }
        return normal;
    }

    public Vector3 ProbeForwardGroundNormal(out float pslopeAngle)
    {
        Vector3 center = m_characterController.transform.position + m_characterController.center+transform.forward*m_characterController.radius;
        Vector3 normal = Vector3.up;
        RaycastHit hit;
        pslopeAngle = 0;
        Ray ray = new Ray(center, -transform.up);
        float groundDist = m_characterController.height * 0.5f + 0.1f;
        if (Physics.Raycast(center, -Vector3.up, out hit, groundDist + 1))
        {

            //if (ySpeed < 0 && hit.distance <= groundDist)
            {
                //groundPos = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                pslopeAngle = Vector3.Angle(Vector3.up, hit.normal);
                normal = hit.normal;
            }
        }
        return normal;
    }

    public bool checkPushable(out Vector3 normal,out PushableBox box)
    {
        float wallCheckDist = m_characterController.radius + 0.3f;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, wallCheckDist))
        {
            if (hit.collider.tag == "pushable")
            {
                normal = hit.normal;

                box = hit.collider.GetComponent<PushableBox>();
                if (box != null)
                {
                    if (box.canMoveInDirection(transform.forward))
                    {
                        return true;
                    }
                }
                
            }
            
        }
        normal = Vector3.zero;
        box = null;
        return false;
    }

    public bool checkWallSlide(Vector3 lookDir,out Vector3 wallNormal)
    {
        float wallCheckDist = m_characterController.radius + 0.3f;
        float maxWallNormalDiff = 0.3f;
        maxWallNormalDiff = maxWallNormalDiff * maxWallNormalDiff;
        wallNormal = Vector3.zero;
        if (onGround)
            return false;
        //We cast 3 Rays
        Vector3 topStart = transform.position + new Vector3(0, m_characterController.height * 0.5f, 0);
        Vector3 centerStart = transform.position;
        Vector3 bottomStart= transform.position + new Vector3(0, -m_characterController.height * 0.5f, 0);

        
        RaycastHit hit;

        if(Physics.Raycast(topStart,lookDir,out hit, wallCheckDist)){
            wallNormal = hit.normal;
            if (Mathf.Abs(wallNormal.y) > 0.01)
                return false;
            Debug.DrawLine(hit.point, hit.point + hit.normal);
        }else
        {
            return false;
        }

        if(Physics.Raycast(centerStart,lookDir,out hit, wallCheckDist))
        {
            if ((wallNormal - hit.normal).sqrMagnitude > maxWallNormalDiff)
            {
                //return false;
                
            }
            Debug.DrawLine(hit.point, hit.point + hit.normal);
        }
        else
        {
            return false;
        }

        if (Physics.Raycast(bottomStart, lookDir, out hit, wallCheckDist))
        {
            if ((wallNormal - hit.normal).sqrMagnitude > maxWallNormalDiff)
            {
                //return false;
                
            }
            Debug.DrawLine(hit.point, hit.point + hit.normal);
        }
        else
        {
            return false;
        }
        return true;
    }

    public bool CheckCanhang(out Vector3 hangLocation,out Vector3 WallNorm)
    {
        hangLocation = Vector3.zero;
        WallNorm = Vector3.zero;
        climbOffset = transform.forward * m_characterController.radius * 1.1f;
        climbOffset.y = m_characterController.height * 0.7f;
        Vector3 testPos = climbOffset + transform.position;
        float checkDownDist = 0.2f + m_characterController.height * 0.6f;
        Ray raytest = new Ray(testPos, Vector3.down);
        Debug.DrawLine(raytest.origin, raytest.origin + raytest.direction * checkDownDist);
        RaycastHit hittest;
        if (Physics.Raycast(raytest, out hittest, checkDownDist))
        {
            float angle = Vector3.Angle(Vector3.up, hittest.normal);
            if (angle < slopeLimit && hittest.collider.tag != "enemy")
            {
                //Step 2
                Vector3 wallTestDir = transform.forward;
                Ray wallTestRay = new Ray(transform.position + m_characterController.center + new Vector3(0, 0.3f, 0), transform.forward);
                Debug.DrawLine(wallTestRay.origin, wallTestRay.origin + wallTestRay.direction.normalized * m_characterController.radius * 1.3f);
                RaycastHit wallHit;
                if (Physics.Raycast(wallTestRay, out wallHit, m_characterController.radius * 1.3f))
                {
                    //Step4
                    WallNorm = wallHit.normal;
                    //hangLocation = (testPos + new Vector3(0, checkDownDist - hittest.distance - 1f)) - climbOffset;
                    hangLocation = wallHit.point + (transform.forward * -m_characterController.radius*0.7f);
                    hangLocation.y = hittest.point.y-m_characterController.height*0.5f;

                    Vector3 zwichenLocation = new Vector3(transform.position.x, testPos.y, transform.position.z);
                    Vector3 zwischenDir = testPos - zwichenLocation;

                    if (!Physics.Raycast(zwichenLocation, zwischenDir, zwischenDir.magnitude))
                    {
                        //Erfolg
                        return true;
                        
                    }

                }
            }

        }
        return false;
    }

    public bool CheckHeadBump()
    {
        RaycastHit hit;
        if(Physics.SphereCast(transform.position,m_characterController.radius,Vector3.up,out hit, m_characterController.height * 0.5f + 0.001f))
        {
            if(hit.normal.y<-0.75f)
                return true;
        }
        return false;
    }

    public void SetTouchJoystick(Vector2 joy)
    {
        touchJoy = joy;
    }

    public void PressActionButton(ActionButton action)
    {
        switch (action)
        {
            case ActionButton.hand:
                attackPessed = true;
                break;
            case ActionButton.jump:
                jumpPressed = true;
                break;
        }
    }

    private void GlobalPreFixedUpdate()
    {
        stickToGround = false;
        //Input handling-------------

        Vector3 center = m_characterController.transform.position + m_characterController.center;
        float moveHorizontal = Input.GetAxis("Horizontal");
        float  moveVertical = Input.GetAxis("Vertical");
        
        if (touchJoy.sqrMagnitude >= 0.001f)
        {
            moveHorizontal = touchJoy.x;
            moveVertical = touchJoy.y;
        }
        if (Movement2d)
        {
            moveVertical = 0;
        }

        if (!CanMove)
        {
            moveHorizontal = 0;
            moveVertical = 0;
            jumpPressed = false;
            attackPessed = false;
            velocity.x = 0;
            velocity.z = 0;
        }

        Vector3 joyVec  = new Vector3(moveHorizontal, 0, moveVertical);
        if(joyVec.magnitude<JoyDeadZone)
        {
            joyVec = Vector3.zero;
        }

        if (joyVec.sqrMagnitude > 1)
        {
            joyVec.Normalize();
        }
        float camdir = 0;
        /*if (_gvr != null)
        {
            camdir = _gvr.HeadPose.Orientation.eulerAngles.y;
        }else*/
        {
            camdir = cam.transform.rotation.eulerAngles.y;
        }
        joyWorldVec = Quaternion.Euler(0, camdir, 0) * joyVec;

        //---------------------------

        //Ground detection--------------

        groundPos = transform.position;
        groundPos.y -= m_characterController.height * 0.5f;

        normal = Vector3.up;
        RaycastHit hit;
        
        bool rayGrounded = false;
        Ray ray = new Ray(center, -transform.up);
        float groundDist = m_characterController.height * 0.5f + 0.1f;
        if (Physics.Raycast(ray, out hit, groundDist + 1))
        {

            //if (ySpeed < 0 && hit.distance <= groundDist)
            {
                rayGrounded = true;
                groundPos = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
                normal = hit.normal;
                groundCollider = hit.collider;
            }
        }


        //Spherecast logic -----------------------
        if (Physics.SphereCast(center, m_characterController.radius, -Vector3.up, out hit, groundDist + 1 - m_characterController.radius))
        {
            if (velocity.y <= 0 && hit.distance <= groundDist)
            {
                if (rayGrounded)
                {
                    if (m_characterController.isGrounded)
                    {
                        
                        velocity.y = 0;
                        


                    }
                    onGround = true;
                    groundPos = new Vector3(transform.position.x, hit.point.y, transform.position.z);

                }
                else
                {

                }

                if (onGround)
                {

                    normal = hit.normal;
                    slopeAngle = Vector3.Angle(Vector3.up, normal);
                    if (groundChecker.groundDetected)
                    {
                        slopeAngle = groundChecker.groundSlopeAngle;
                        normal = groundChecker.groundSlopeDir;
                    }
                }

            }
            Debug.DrawLine(hit.point, hit.point + normal);
        }
        else
        {
            
            onGround = false;
        }
        if(Mathf.Abs(slopeAngle)<1f)
            slopeAngle = groundChecker.groundSlopeAngle;

        if (slopeAngle < 0.1f && onGround && groundCollider!=null && groundCollider.tag=="Untagged")
            laststandpos = transform.position;

        //----------------------------------------

        if (m_characterController.isGrounded && velocity.y < 0)
        {
            onGround = true;
        }

        
      
        //------------------------------

    }


    private void GlobalPostFixedUpdate()
    {
       
        jumpPressed = false;
        attackPessed = false;
        if (velocity.y < -maxVerticalspeed)
        {
            velocity.y = -maxVerticalspeed;
        }

        if (velocity.y > maxVerticalspeed)
        {
            velocity.y = maxVerticalspeed;
        }

        //velocity.z = 0;

        Vector3 vel = velocity;

        lastPos = transform.position;
        //m_characterController.Move(vel*Time.deltaTime);

        //2D Movement
        if (Movement2d)
        {
            transform.position =new Vector3(_startPos.x,transform.position.y,transform.position.z);
            
        }

        //Player runtergefallen?
        if (transform.position.y < -10)
        {

            Runtergefallen();
        }

        if (_blinking)
        {
            _blinkTimer -= Time.deltaTime;
            if (_blinkTimer <= 0)
            {
                SetVisible(true);
                _blinking = false;
            }else
            {
                SetVisible(((int)(Time.time * 20f)) % 2 == 1);

            }
        }

        animator.SetFloat("yspeed", vel.y);
        
       
    }

    public void Runtergefallen()
    {
        transform.position = laststandpos;
        Damage(1, null);
        Blink();
    }

    public void Blink()
    {
        _blinkTimer = _MAXBLINKTIMER;
        _blinking = true;
    }

    

    private void GlobalPostUpdate()
    {
        if (stickToGround&&slopeAngle<60)
        {
            //moveToGround();
            Vector3 horVel = velocity*Time.deltaTime;
            horVel.y = 0;
            transform.position = transform.position + Vector3.up * 0.01f;
            //m_characterController.Move(Vector3.up * -0.5f);
            m_characterController.Move(Vector3.up * -(horVel.magnitude+0.01f));
        }
        
        m_characterController.Move(velocity * Time.deltaTime);
    }

    void FixedUpdate()
    {
        GlobalPreFixedUpdate();

        machine.StateFixedUpdate();

        GlobalPostFixedUpdate();
    }

 

    public void DoneClimb()
    {
        machine.State = jumpnRunState;

    }

    public void DoneAttack()
    {
        if (machine.State == swordAttackstate)
        {
            swordAttackstate.EndAttack();
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //velocity += hit.moveDirection * hit.moveLength;
    }

    public void Damage(float amount, Transform source)
    {
        if (isAttackable)
        {
            _stats.Health -= amount;
            if (_stats.Health <= 0)
            {

                //Analytics
                ItemCounter itemCounter = FindObjectOfType<ItemCounter>();
                Dictionary<string, object> args = new Dictionary<string, object>();
                args.Add("level", SceneManager.GetActiveScene().name);
                args.Add("coins_collected", itemCounter.CoinsCollected);
                args.Add("death_position", transform.position);
                Analytics.CustomEvent("PlayerDie", args);

                SceneManager.LoadScene(0);
                Cursor.visible = true;
            }
            else
            {
                if (source != null) {
                auSound.Play();
                machine.State = injuryState;
                Vector3 tmp = transform.position - source.position;
                tmp.y = 0;
                    if (tmp.sqrMagnitude > 0.001f)
                    {
                        velocity = tmp.normalized * 10f;
                        velocity.y = 0;


                        Quaternion sollRot = Quaternion.FromToRotation(Vector3.forward, velocity);
                        float angle = sollRot.eulerAngles.y;



                        transform.rotation = Quaternion.Euler(0, angle + 180, 0);
                    }
                }
            }
        }
    }
}
