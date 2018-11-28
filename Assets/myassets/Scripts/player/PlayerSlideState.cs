using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlideState : PlayerState {

    private const float maxSpeed= 20;
    private CharacterController m_characterController;
    private const float _MAXSENDTIMER= 0.2f;
    private float _endTimer = 0;

	public PlayerSlideState(GameObject go) : base(go, "slide")
    {
        m_characterController = go.GetComponent<CharacterController>();
    }

    public override void FixedUpdate()
    {
        player.stickToGround = true;
        if (player.onGround)
        {
            if (player.slopeAngle < player.slopeLimit)
            {
                _endTimer -= Time.deltaTime;
                if (_endTimer <= 0)
                {
                    player.machine.State = player.jumpnRunState;
                }
            }else
            {
                Vector3 sollVel = new Vector3(player.normal.x*10,0,player.normal.z*10);
                Vector3 tmpVel = player.velocity;
                tmpVel.y = 0;
                player.velocity = Vector3.MoveTowards(tmpVel, sollVel, Time.deltaTime * 20);
                if (player.slopeAngle > 60)
                {
                    player.velocity.y = -10;
                }

                Vector3 movement = new Vector3(player.velocity.x, 0, player.velocity.z);
                if (movement.magnitude > maxSpeed)
                {
                    movement = movement.normalized * maxSpeed;
                }
                movement = Vector3.MoveTowards(movement, player.joyWorldVec * maxSpeed, Time.deltaTime * 4f);
                player.velocity.x = movement.x;
                player.velocity.z = movement.z;
               // player.velocity.y = -10f;

                Quaternion sollRot = Quaternion.FromToRotation(Vector3.forward, player.velocity);

                float angle = sollRot.eulerAngles.y;

                player.transform.rotation = Quaternion.Euler(0, angle, 0);
            }
        }else
        {
            player.machine.State = player.jumpnRunState;
            //player.velocity.y = 0;
        }
    }

    public override void EnterState()
    {
        //m_characterController.slopeLimit = 85;
        if (player.slopeAngle>60)
            player.velocity = player.velocity * 0.2f;
        //player.animator.SetTrigger("slide");
        player.animator.CrossFadeInFixedTime("slide",0.2f);
        _endTimer = _MAXSENDTIMER;
    }

    public override void ExitState()
    {

    }
}
