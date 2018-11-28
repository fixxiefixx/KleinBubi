using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbState : PlayerState {

    private CharacterController _controller;

	public PlayerClimbState(GameObject go) : base(go, "climb")
    {
        _controller = go.GetComponent<CharacterController>();
    }

    public override void EnterState()
    {
        player.isInAttackableState = false;
        //player.animator.SetTrigger("climb");
        player.animator.Play("edgeclimb");
        player.velocity = Vector3.zero;
        player.climbSound.Play();
    }

    public override void ExitState()
    {
        player.transform.position = player.transform.position + player.climbOffset+new Vector3(0,_controller.height*0.35f,0);
        player.onGround = true;
        //anim.SetBool("onGround", true);
    }
}
