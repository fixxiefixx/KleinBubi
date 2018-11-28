using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPushState : PlayerState {

    public PushableBox PushingBox = null;


    private const float _MAXSPEED = 4f;
    private const float _ACCELERATION = 30;
    private float _forwardSpeed = 0;
    private float _angle = 0;

    public PlayerPushState(GameObject go) : base(go, "push")
    {
        
    }

    public override void FixedUpdate()
    {
        player.stickToGround = true;
        if (player.slopeAngle > player.slopeLimit)
        {

            //player.machine.State = player.slideState;
            player.machine.State = player.fallState;
            return;
           
        }

        if (PushingBox != null)
        {
            
            float sollSpeed = _MAXSPEED;

            
                Vector3 toPushable = PushingBox.transform.position-player.transform.position;
           

                float sollRot = Mathf.Atan2(toPushable.x, toPushable.z) * 57.2958f;
                
                    _angle = Mathf.MoveTowardsAngle(_angle, sollRot, Time.deltaTime * 360);
               
                player.transform.rotation = Quaternion.AngleAxis(_angle, Vector3.up);
           
            _forwardSpeed = Mathf.MoveTowards(_forwardSpeed, sollSpeed, Time.deltaTime * _ACCELERATION);
            Vector3 speedV = player.transform.forward * _forwardSpeed;

            player.velocity.x = speedV.x;
            player.velocity.z = speedV.z;
            player.animator.SetFloat("runspeed", _forwardSpeed);

        }
        else
        {
            player.machine.State = player.jumpnRunState;
        }
        
    }

    public override void EnterState()
    {
        _angle = player.jumpnRunState.angle;
        player.animator.SetBool("pushing", true);
    }

    public override void ExitState()
    {
        player.animator.SetBool("pushing", false);
    }

    public void EndPush()
    {
        if (player.machine.State == this)
        {
            player.machine.State = player.jumpnRunState;
        }
    }
}
