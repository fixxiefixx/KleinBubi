using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordAttackState : PlayerState {

    private  string[] _animationsAir =  { "sword_attack1"};
    private string[] _animationsGround = { "sword_attack2" };
    private int _beiAnim = 0;



	public PlayerSwordAttackState(GameObject go) : base(go, "swordAttack")
    {

    }

    public override void FixedUpdate()
    {
        
        if (player.onGround)
        {
            player.stickToGround = true;
            player.velocity = Vector3.MoveTowards(player.velocity, Vector3.zero, Time.deltaTime * 20f);
        }
        else
        {
            player.velocity.y -= Time.deltaTime * PlayerFallState.gravity;
        }
        float yvel = player.velocity.y;
        
        player.velocity.y = yvel;
        if (player.onGround &&  player.slopeAngle > player.slopeLimit)
        {
            /*player.velocity *= 0.2f; 
            player.machine.State = player.slideState;*/
            
        }
    }

    public override void EnterState()
    {
        player.AttackCollider.enabled = true;
        player.AirAttacked = true;
        player.trail.Emit = true;
        if (player.onGround)
        {
            //player.animator.Play(_animationsGround[(int)Mathf.Floor(Random.value * _animationsGround.Length * 0.99999f)]);
            player.animator.CrossFadeInFixedTime(_animationsGround[(int)Mathf.Floor(Random.value*_animationsGround.Length*0.99999f)], 0.1f);
        }else
        {
            //player.animator.Play("_animationsAir[(int)Mathf.Floor(Random.value * _animationsAir.Length * 0.99999f)]");
            player.animator.CrossFadeInFixedTime(_animationsAir[(int)Mathf.Floor(Random.value * _animationsAir.Length * 0.99999f)], 0.1f);
        }
        player.swordSound.Play();
        if (!player.onGround)
            player.velocity.y = 5f;
        
    }

    public override void ExitState()
    {
        player.AttackCollider.enabled = false;
        player.trail.Emit = false;
    }

    public void EndAttack()
    {
        if (player.onGround)
            player.machine.State = player.jumpnRunState;
        else
            player.machine.State = player.fallState;
    }
}
