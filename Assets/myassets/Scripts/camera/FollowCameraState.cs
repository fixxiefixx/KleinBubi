using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraState : CameraState {

    private float _sollDistance;
    private float _distance = 0;
    private const float _MINDISTANCE = 3f;
    private const float _MAXDISTANCE = 12f;
    private float _height;
    private Vector3 _smoothTargetPos;
    private float _angleSpeed = 0;
    private float _perfectangle = 0;
    private const float _ANGLEPROBES = 16;
    private const float _MAXANGLESPEED = 500;

	public FollowCameraState(GameObject go,float distance,float height) : base(go, "follow")
    {
        _sollDistance = distance;
        _distance = distance;

        _height = height;
    }

    private bool findPerfectAngle(out float angle,out float beklemmung)
    {
        angle = 0;
        Vector3 median = Vector3.zero;
        for(int i = 0; i < _ANGLEPROBES; i++)
        {
            float testAngle = (360 / 8) * i;
            Vector3 testDir = Quaternion.AngleAxis(testAngle, Vector3.up)*Vector3.forward*_sollDistance;
            testDir.y = camPos.y - _smoothTargetPos.y;

            RaycastHit hit;
            if(Physics.Raycast(_smoothTargetPos,testDir,out hit, _sollDistance,_camcont.ColliderMask)){
                median += hit.point - _smoothTargetPos;
                
            }else
            {
                median += testDir * _sollDistance;
            }
        }
        beklemmung = median.magnitude;
        if (median.sqrMagnitude > 0.01f)
        {
            angle = Quaternion.FromToRotation(Vector3.forward, median).eulerAngles.y;
            //Debug.DrawLine(_smoothTargetPos, _smoothTargetPos+median,Color.cyan);
            return true;
        }
        
        return false;
    }

    private bool camCollides(out float distance)
    {
        RaycastHit hit;
        Vector3 testDir = camPos - _smoothTargetPos;
        if (Physics.SphereCast(_smoothTargetPos,0.3f, testDir, out hit, _sollDistance,_camcont.ColliderMask))
        {
            distance = hit.distance;
            return true;
        }
        distance = 0;
        return false;
    }

    public override void Update()
    {
        _smoothTargetPos = Vector3.Lerp(_smoothTargetPos, _camcont.Target.position, 0.1f);
        Vector3 tmp= camPos - _smoothTargetPos;
        
        //float curentDistance = target_cam.magnitude;
        float curentDistance = _sollDistance;

        float beklemmung;

        bool angleFound = findPerfectAngle(out _perfectangle, out beklemmung);
        float hitDistance;

        if (camCollides(out hitDistance) && angleFound)
        {
            //_distance = Mathf.MoveTowards(_distance, Mathf.Max(0.5f,hitDistance), Time.deltaTime * 50f);

            _angleSpeed += Time.deltaTime * 500f;
            if (_angleSpeed > _MAXANGLESPEED)
            {
                _angleSpeed = _MAXANGLESPEED;
            }
        }else
        {
            _angleSpeed = Mathf.MoveTowards(_angleSpeed, 0, Time.deltaTime * 500f);
            //_distance = Mathf.MoveTowards(_distance, _sollDistance, Time.deltaTime * 10f);
        }

        if (_angleSpeed > 0.01f)
        {
            float currentAngle = Quaternion.FromToRotation(Vector3.forward, tmp).eulerAngles.y;
            currentAngle = Mathf.MoveTowardsAngle(currentAngle, _perfectangle, Time.deltaTime * _angleSpeed);
            //currentAngle = sollAngle;
            tmp = Quaternion.AngleAxis(currentAngle, Vector3.up) * Vector3.forward;

        }
        else
        {

            //Input
            float hor = -Input.GetAxis("RightHorizontal");
            if (Mathf.Abs(hor) > 0.1f)
            {
                tmp = Quaternion.AngleAxis(hor * Time.deltaTime * 100f, Vector3.up) * tmp;
            }

            float vert = Input.GetAxis("RightVertical");
            if (Mathf.Abs(vert) > 0.1f)
            {
                _sollDistance += vert * Time.deltaTime*10f;
                if (_sollDistance < _MINDISTANCE)
                {
                    _sollDistance = _MINDISTANCE;
                }
                if (_sollDistance > _MAXDISTANCE)
                {
                    _sollDistance = _MAXDISTANCE;
                }
            }
        }

        Vector2 target_cam = new Vector2(tmp.x, tmp.z);

        target_cam = target_cam.normalized * curentDistance;
        camPos = _smoothTargetPos + new Vector3(target_cam.x,_height,target_cam.y);
    }

    public override void EnterState()
    {
        if (_camcont.Target != null && _camcont!=null)
        {
            _smoothTargetPos = _camcont.Target.position;
            camPos = _camcont.transform.position;
        }
    }
}
