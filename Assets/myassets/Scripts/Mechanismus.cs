using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanismus : MonoBehaviour {

    public AnimInfo[] AnimInfos;
    public bool MoveCamera = true;
    public float MoveCameraTime = 2;
    public bool FreezePlayer = true;

    private MechaState _state = MechaState.notTriggered;
    private float _cameraTimer = 0;
    private CameraController _camCont;
    private Player _player;

    [System.Serializable]
    public class AnimInfo
    {
        public Animator Anim;
        public string AnimTrigger;
    }

    private enum MechaState
    {
        notTriggered,
        triggering,
        triggered
    }

	// Use this for initialization
	void Start () {
        _camCont = FindObjectOfType<CameraController>();
        _player = FindObjectOfType<Player>();
	}
	
    void FixedUpdate()
    {
        switch (_state)
        {
            case MechaState.triggering:
                {
                    _cameraTimer -= Time.deltaTime;
                    if (_cameraTimer <= 0)
                    {
                        _state = MechaState.notTriggered;
                        if (MoveCamera)
                        {
                            _camCont.Target = _player.transform;
                        }
                        if (FreezePlayer)
                        {
                            _player.CanMove = true;
                        }
                    }
                }break;
        }
    }

    public void Execute()
    {
        if (_state == MechaState.notTriggered)
        {
            foreach (AnimInfo ai in AnimInfos)
            {
                ai.Anim.SetTrigger(ai.AnimTrigger);
            }

            if (MoveCamera)
            {
                _camCont.Target = transform;
            }
            if (FreezePlayer)
            {
                _player.CanMove = false;
            }
            _state = MechaState.triggering;
            _cameraTimer = MoveCameraTime;
        }
    }
}
