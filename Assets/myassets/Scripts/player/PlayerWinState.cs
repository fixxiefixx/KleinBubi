using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWinState : PlayerState {

	public PlayerWinState(GameObject go) : base(go, "win")
    {

    }

    public override void FixedUpdate()
    {
        player.stickToGround = true;
    }

    public override void EnterState()
    {
        player.isInAttackableState = true;
        player.velocity = Vector3.zero;
    }
}
