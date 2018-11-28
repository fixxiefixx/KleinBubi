using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsoCameraState : CameraState {

    public Vector3 Offset = new Vector3(5, 10, 3);

	public IsoCameraState(GameObject go) : base(go, "iso")
    {

    }

    public override void Update()
    {
        camPos = this._camcont.SmoothTargetPos + Offset;
    }
}
