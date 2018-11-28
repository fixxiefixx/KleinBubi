using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInjuryState : PlayerState {

    private const float _MAXDAMAGEDTIMER = 0.8f;
    private float _damagedTimer = 0;

    public PlayerInjuryState(GameObject go) : base(go, "injury")
    {

    }

    public override void FixedUpdate()
    {
        
        if (player.onGround)
        {
            player.stickToGround = true;
        }else
        {
            player.velocity.y -= Time.deltaTime * PlayerFallState.gravity;
        }

        Vector2 tmp = new Vector2(player.velocity.x, player.velocity.z);
        tmp = Vector2.MoveTowards(tmp, Vector2.zero, Time.deltaTime * 10f);
        player.velocity.x = tmp.x;
        player.velocity.z = tmp.y;
        

        _damagedTimer -= Time.deltaTime;
        if (_damagedTimer <= 0)
        {
            player.machine.State = player.jumpnRunState;
        }
    }

    public override void ExitState()
    {
        player.Blink();
    }

    public override void EnterState()
    {
        player.isInAttackableState = false;
        _damagedTimer = _MAXDAMAGEDTIMER;
        player.animator.CrossFadeInFixedTime("injury", 0.1f);

    }
}
