using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{

    public enum CameraType
    {
        Cylinder,
        Iso,
        Follow
    }

    public CameraType CamType = CameraType.Iso;
    public Transform PoleTarget;
    public float Distance = 10;
    public float Height = 5;
    public Vector3 IsoOffset= new Vector3(5, 10, 3);

    private CameraController _camCont;
    private bool inTrigger = false;

    // Use this for initialization
    void Start ()
    {
        _camCont = FindObjectOfType<CameraController>();
    }

    /// <summary>
    /// Wird aufgerufen wenn ein Collider den Collider dieses Objektes eindringt.
    /// </summary>
    /// <param name="other">Der Collider der in diesen Collider eingedrungen ist.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            inTrigger = true;
            switch (CamType)
            {
                case CameraType.Cylinder:
                {
                    CylinderCameraState cam = new CylinderCameraState(_camCont.gameObject);
                    cam.Distance = Distance;
                    cam.Height = Height;
                    cam.PoleTarget = PoleTarget;
                    _camCont.PushCamera(cam);
                }
                break;
                case CameraType.Iso:
                {
                    IsoCameraState cam = new IsoCameraState(_camCont.gameObject);
                    cam.Offset = IsoOffset;
                    _camCont.PushCamera(cam);
                }
                break;
                case CameraType.Follow:
                {
                    FollowCameraState cam = new FollowCameraState(_camCont.gameObject, Distance, Height);
                    _camCont.PushCamera(cam);
                }
                break;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            inTrigger = false;
            _camCont.PopCamera();
        }
    }

    void OnDestroy()
    {
        if (inTrigger)
        {
            inTrigger = false;
            _camCont.PopCamera();
        }
    }

}
