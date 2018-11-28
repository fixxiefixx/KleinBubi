using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerState {

    private const float _WALLSLIDESPEED= 1.5f;
    private const float _MAXWALLSLIDETIMER = 2f;
    private const float _JUMPSPEED = 13f;
    private CharacterController m_characterController;

    private float _wallSlideTimer=0;
    

	public PlayerWallSlideState(GameObject go) : base(go, "wallSlide")
    {
        m_characterController = go.GetComponent<CharacterController>();
    }

    public override void FixedUpdate()
    {
        _wallSlideTimer -= Time.deltaTime;
        Vector3 wallNormal;
      
        if (_wallSlideTimer <= 0||!player.checkWallSlide(player.transform.forward,out wallNormal))
        {
            if (player.onGround)
            {
                player.machine.State = player.jumpnRunState;
            }else
            {
                if (_wallSlideTimer > 0)
                {
                    player.wallSlided = false;
                }
                player.machine.State = player.fallState;
                player.animator.SetTrigger("fall");
            }
        }else
        {
            m_characterController.Move(-wallNormal*Time.deltaTime*2f);
            if (player.jumpPressed)
            {
                Vector3 jumpDir = wallNormal;
                if(/*Vector3.Dot(player.joyWorldVec,wallNormal)>0 &&*/ player.joyWorldVec.magnitude > 0.1f)
                {
                    
                    jumpDir = player.joyWorldVec;
                    if (Vector3.Dot(jumpDir, wallNormal) < 0)
                    {
                        jumpDir = Vector3.Reflect(jumpDir, wallNormal);
                    }
                    jumpDir += wallNormal * 0.8f;
                    jumpDir.y = 0;
                    jumpDir.Normalize();
                }

                player.velocity = jumpDir*6f;
                player.Jump(_JUMPSPEED);
                player.transform.rotation = player.transform.rotation * Quaternion.AngleAxis(180, Vector3.up);

                player.wallSlided = false;
                //player.AirJumped = true;
            }
        }
    }

    public override void EnterState()
    {
        _wallSlideTimer = _MAXWALLSLIDETIMER;
        player.velocity = Vector3.down * _WALLSLIDESPEED;
        player.wallSlided = true;
        //player.AirJumped = true;
        //player.animator.SetTrigger("wallslide");
        player.animator.CrossFadeInFixedTime("wallslide",0.2f);

    }

}
