using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerErtrinkenState : PlayerState {

    private const float _MAXERTRINKENTIMER = 3f;
    private float _ertrinkenTimer = 0;

	public PlayerErtrinkenState(GameObject go) : base(go, "ertrinken")
    {

    }

    public override void FixedUpdate()
    {
        player.stickToGround = true;
        _ertrinkenTimer -= Time.deltaTime;
        if (_ertrinkenTimer <= 0)
        {
            player.machine.State = player.jumpnRunState;
            player.transform.position = player.laststandpos;
            player.groundCollider = null;
        }
    }

    public override void ExitState()
    {
        player.Blink();
        
    }

    public override void EnterState()
    {
        player.isInAttackableState = false;
        player.velocity = Vector3.zero;
        _ertrinkenTimer = _MAXERTRINKENTIMER;
        player.animator.CrossFadeInFixedTime("ertrinken", 0.002f);
        player.ertrinkenSound.Play();
    }
}
