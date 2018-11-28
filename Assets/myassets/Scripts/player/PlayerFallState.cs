using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerState {

    private const float maxSpeed = 7f;
    public const float gravity = 30;
    private CharacterController m_characterController;
    private const float _JUMPSPEED = 13f;
    private const float _JumpAirTime = 0.3f;
  

    public PlayerFallState(GameObject go) : base(go, "fall")
    {
        m_characterController = go.GetComponent<CharacterController>();
    }

    public override void FixedUpdate()
    {
        bool hasHitGround = false;
        if (player.onGround )
        {
            float groundSlope;
            //Vector3 norm=player.ProbeForwardGroundNormal(out groundSlope);
            Vector3 groundPos;
            Vector3 norm = player.ProbeGroundNormal(out groundSlope, out groundPos);
            if (groundSlope <= player.slopeLimit && m_characterController.isGrounded)
            {
                hasHitGround = true;
                player.machine.State = player.jumpnRunState;
            }else
            {
                //if (groundSlope < 70)
                {
                    /*player.velocity = player.velocity * 0.2f;
                    player.machine.State = player.slideState;*/
                     /*if (player.velocity.y < -20f)
                         player.velocity.y = -20f;*/

                    Vector3 normAcc = norm;
                    normAcc.y = 0;
                    normAcc.Normalize();
                    Vector3 horVel = player.velocity;
                    horVel.y = 0;

                    /*if(Vector3.Dot(horVel,normAcc)<0f)
                    {
                        player.velocity.x = 0;
                        player.velocity.z = 0;

                    }*/

                    Vector3 movement = new Vector3(player.velocity.x, 0, player.velocity.z);
                    movement = Vector3.MoveTowards(movement, normAcc * 7f, Time.deltaTime * 60f);
                    player.velocity.x = movement.x;
                    player.velocity.z = movement.z;

                    /*player.velocity += normAcc * 40f * Time.deltaTime;
                    player.stickToGround = true;*/


                }
                /*else
                {

                    player.velocity += new Vector3(norm.x, 0, norm.z).normalized * Time.deltaTime * 100f;
                }*/



            }
        }
        if(!hasHitGround)
        {

            if (player.velocity.y > 0 && player.CheckHeadBump())
            {
                player.velocity.y = 0;
            }
            Vector3 wallNormal;
            if(!player.wallSlided && player.velocity.y<0 && player.checkWallSlide(player.transform.forward,out wallNormal))
            {
                //WallSlide
                
                Quaternion sollRot = Quaternion.FromToRotation(Vector3.forward, -wallNormal);
                float angle = sollRot.eulerAngles.y;
                player.transform.rotation = Quaternion.Euler(0, angle, 0);
                player.machine.State = player.WallSlideState;
                return;
            }
            else
            {
                //Check Hang
                if (player.velocity.y < 0)
                {
                    Vector3 hangLocation;
                    Vector3 wallNorm;
                    if (!player.HasHanged && player.CheckCanhang(out hangLocation, out wallNorm))
                    {
                        float angle = Mathf.Atan2(-wallNorm.x, -wallNorm.z) * 57.2958f;
                        player.transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

                        player.transform.position = hangLocation - m_characterController.center;
                        player.machine.State = player.hangState;
                        return;
                    }
                }
            }



            player.velocity.y -= Time.deltaTime * gravity;

            if (player.jumpPressed && !player.AirJumped)
            {
                if ((Time.time - player.jumpnRunState.lastGroundTime) <= _JumpAirTime)
                {
                    player.Jump(PlayerJumpnRunState.jumpSpeed);
                }
                else
                {

                    player.velocity.y = _JUMPSPEED;
                    player.AirJumped = true;
                    player.animator.CrossFadeInFixedTime("airroll", 0.2f);
                    player.doublejumpSound.Play();
                    Debug.Log("airjumped");
                }
            }else
                if (player.attackPessed)
            {
                if(player.onGround||!player.AirAttacked)
                player.machine.State = player.swordAttackstate;
            }

            Vector3 movement = new Vector3(player.velocity.x,0, player.velocity.z);
            movement = Vector3.MoveTowards(movement, player.joyWorldVec*maxSpeed, Time.deltaTime*10f);
            player.velocity.x = movement.x;
            player.velocity.z = movement.z;

            //player drehen
            Vector3 veltmp = new Vector3(player.velocity.x, 0, player.velocity.z);
            
            if (veltmp.sqrMagnitude > 0.01f)
            {
                Quaternion sollRot2 = Quaternion.FromToRotation(Vector3.forward, player.velocity);
                float angle2 = sollRot2.eulerAngles.y;
                angle2 = Mathf.MoveTowardsAngle(player.transform.rotation.eulerAngles.y, angle2, Time.deltaTime * 700);
                player.transform.rotation = Quaternion.Euler(0, angle2, 0);
            }
        }

    }

    public override void ExitState()
    {
        //player.velocity = player.velocity * 0.5f;
    }

    public override void EnterState()
    {
        player.animator.Play("jump");
        //m_characterController.slopeLimit = 85;
    }
}
