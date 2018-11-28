using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHangState : PlayerState {

    private CharacterController m_characterController = null;

    public PlayerHangState(GameObject go) : base(go, "Hang")
    {
        m_characterController = go.GetComponent<CharacterController>();
    }
	
	public override void FixedUpdate () {
        player.velocity = Vector3.zero;
        Vector3 hangpos;
        Vector3 wallNorm;
        if (!player.CheckCanhang(out hangpos,out wallNorm))
        {
            player.machine.State = player.jumpnRunState;
            player.animator.SetTrigger("cancelHang");
            return;
        }

        if (player.jumpPressed)
        {
            player.machine.State = player.climbState;

        }
        else
        {
            if (player.joyWorldVec.magnitude > 0.8f && Vector3.Dot(player.joyWorldVec, player.transform.forward) < 0)
            {
                player.transform.position = player.transform.position+ player.transform.forward * -0.5f;
                player.machine.State = player.fallState;
                //player.animator.SetTrigger("cancelHang");
            }
        }
    }

    public override void EnterState()
    {
        player.HasHanged = true;
        player.animator.CrossFadeInFixedTime("edgehang",0.2f);
        player.animator.SetFloat("runspeed",0);
        player.velocity=Vector3.zero;
    }
}
