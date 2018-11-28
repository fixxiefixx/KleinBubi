using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpnRunState : PlayerState
{
    private const float maxSpeed = 7f;
    public const float jumpSpeed = 13f;
    private const float _ACCELERATION=30;
    private const float _BREAK = 100;
    public float forwardSpeed = 0;
    internal float angle = 0;
    internal float lastGroundTime = 0;


    
    private CharacterController m_characterController;

    public PlayerJumpnRunState(GameObject go) : base(go, "JumpnRun")
    {
        m_characterController = go.GetComponent<CharacterController>();
    }



    public override void FixedUpdate()
    {
        player.stickToGround = true;
        float sollSpeed = player.joyWorldVec.magnitude * maxSpeed;

        if (Vector3.Dot(player.transform.forward, player.joyWorldVec) < 0)
        {
            sollSpeed = 0;
            forwardSpeed = Mathf.MoveTowards(forwardSpeed, sollSpeed, Time.deltaTime * _BREAK);
            if (forwardSpeed < 0.001f)
            {
                float sollRot = Mathf.Atan2(player.joyWorldVec.x, player.joyWorldVec.z) * 57.2958f;
                angle = angle = Mathf.MoveTowardsAngle(angle, sollRot, Time.deltaTime * 1000);
                player.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            }
        }
        else
        {

            if (sollSpeed > 0.001f)
            {
                //Quaternion sollRot = Quaternion.FromToRotation(Vector3.forward, player.joyWorldVec);
                float sollRot = Mathf.Atan2(player.joyWorldVec.x, player.joyWorldVec.z) * 57.2958f;
                if (forwardSpeed > 0.01f)
                {
                    angle = Mathf.MoveTowardsAngle(angle, sollRot, Time.deltaTime * 360);
                }
                else
                {
                    angle = sollRot;
                }
                player.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);
            }
            forwardSpeed = Mathf.MoveTowards(forwardSpeed, sollSpeed, Time.deltaTime * _ACCELERATION);
        }
        
        Vector3 speedV = player.transform.forward * forwardSpeed;

        

        player.velocity.x = speedV.x;
        player.velocity.z = speedV.z;
        if (!player.onGround)
        {
            //fallen
            player.machine.State = player.fallState;
        }else
        {
            lastGroundTime = Time.time;
            if(player.groundCollider!=null && player.groundCollider.tag == "water")
            {
                player.Damage(1, null);
                player.machine.State = player.ertrinkenState;
                return;
            }

            if (player.jumpPressed)
            {

                player.Jump(jumpSpeed);
            }
            if (player.attackPessed)
            {
                player.machine.State = player.swordAttackstate;
            }
            else
            {

                if (player.slopeAngle > player.slopeLimit && (Vector3.Scale( player.velocity,new Vector3(1,0,1)).sqrMagnitude<0.1f || Vector3.Dot( Vector3.Scale( player.velocity,new Vector3(1,0,1)),Vector3.Scale( player.normal,new Vector3(1,0,1)))<0))
                {
                    //player.machine.State = player.slideState;
                    player.machine.State = player.fallState;
                }
                else
                {
                    Vector3 pushableNormal;
                    PushableBox pbox;
                    if(forwardSpeed>0.01f && player.checkPushable(out pushableNormal,out pbox))
                    {
                        if (Vector3.Dot(player.joyWorldVec, pushableNormal) < 0)
                        {
                            pbox.Push(player, player.transform.forward);
                            player.pushState.PushingBox = pbox;
                            player.machine.State = player.pushState;
                        }
                    }
                }
            } 
        }
        player.animator.SetFloat("runspeed", forwardSpeed);

    }

    public override void EnterState()
    {
        player.isInAttackableState = true;
        if (player.onGround && player.slopeAngle<player.slopeLimit)
        {
            player.wallSlided = false;
            player.AirJumped = false;
            player.AirAttacked = false;
            player.HasHanged = false;
        }
        //m_characterController.slopeLimit = player.slopeLimit;
        Vector2 fromMag = new Vector2(player.velocity.x,player.velocity.z);
        forwardSpeed = fromMag.magnitude;
        if (forwardSpeed > 0.001f)
        {

            //Quaternion sollRot = Quaternion.FromToRotation(Vector3.forward, player.velocity);
            //angle = sollRot.eulerAngles.y;

            angle = player.transform.rotation.eulerAngles.y;


            //player.transform.rotation = Quaternion.Euler(0,angle,0);
        }
        //player.animator.SetFloat("runspeed", forwardSpeed);
        player.animator.CrossFadeInFixedTime("stand",0.1f);
    }
}
