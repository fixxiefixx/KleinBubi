using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform Target=null;
    public LayerMask ColliderMask;
    public float Lerpspeed = 0.1f;

    internal DXStateMachine machine;

    private Vector3 _prevCamPos = Vector3.zero;
    private float _camSwitchTimer = 0;
    private bool _camSwitching = false;
    private const float _MAXCAMSWITCHTIMER= 1;
    private const float _MAXSHAKE=1;
    private float shake = 0;
    internal Vector3 SmoothTargetPos;

    private Stack<CameraState> _camStack = null;

	// Use this for initialization
	void Start () {
        machine = GetComponent<DXStateMachine>();
        _camStack = new Stack<CameraState>();
        //PushCamera(new IsoCameraState(gameObject));
        //PushCamera(new CylinderCameraState(gameObject));
        PushCamera(new FollowCameraState(gameObject, 7, 3));
        _prevCamPos = Target.position;
        
    }
	
    public void PushCamera(CameraState camState)
    {
        _camStack.Push(camState);
        machine.State = camState;
        switchCamera(camState);
    }

    public void PopCamera()
    {
        _camStack.Pop();
        machine.State= _camStack.Peek();
        switchCamera(_camStack.Peek());
    }

    private void switchCamera(CameraState camstate)
    {
        if (this != null)
        {
            _prevCamPos = transform.position;
            _camSwitchTimer = _MAXCAMSWITCHTIMER;
            _camSwitching = true;
        }
    }

    public void dotheHarlemShake(float amount)
    {
        shake = amount*_MAXSHAKE;
    }

    public void dotheHarlemShake()
    {
        shake = _MAXSHAKE;
    }

    void Update()
    {
        SmoothTargetPos = Vector3.Lerp(SmoothTargetPos, Target.position, Lerpspeed);

        machine.StateUpdate();
        if (_camSwitching)
        {
            _camSwitchTimer -= Time.deltaTime;
            if(_camSwitchTimer<=0)
            {
                _camSwitchTimer = 0;
                _camSwitching = false;
            }

            transform.position = Vector3.Lerp(((CameraState)machine.State).camPos, _prevCamPos, _camSwitchTimer / _MAXCAMSWITCHTIMER);
        }else
        {
            transform.position = ((CameraState)machine.State).camPos;
        }

        

        transform.rotation = Quaternion.LookRotation(SmoothTargetPos - transform.position, Vector3.up);
        if (shake > 0)
        {
            shake -= Time.deltaTime*1f;
            Vector3 shakeVec = new Vector3(Mathf.Sin(Time.time * 20), Mathf.Sin(Time.time * 30 + 3), Mathf.Sin(Time.time * 20 + 4));
            shakeVec = shakeVec * shake;
            transform.rotation = transform.rotation * Quaternion.Euler(shakeVec  * 3f);
        }
    }

}
