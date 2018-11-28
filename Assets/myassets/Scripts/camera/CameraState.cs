using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraState : DXState {

    protected CameraController _camcont;
    public Vector3 camPos = Vector3.zero;

	public CameraState(GameObject go,string name) : base(name)
    {
        _camcont = go.GetComponent<CameraController>();
    }
}
