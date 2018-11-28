using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderCameraState : CameraState {


    public Transform PoleTarget=null;
    public float Distance=10f;
    public float Height = 5f;


    private Vector3 _smoothTargetPos;

    public CylinderCameraState(GameObject go) : base(go, "cylinder")
    {

        _smoothTargetPos = _camcont.Target.position;
    }




    public override void Update()
    {

        _smoothTargetPos = Vector3.Lerp(_smoothTargetPos, _camcont.Target.position, 0.1f);

        Vector3 toTargetVec = new Vector3(PoleTarget.position.x, _smoothTargetPos.y, PoleTarget.position.z);
        Vector3 pole_target = _smoothTargetPos - toTargetVec;
        Vector3 target_cam = pole_target.normalized * Distance;
        target_cam.y += Height;

        camPos = _smoothTargetPos + target_cam;
	}
}
