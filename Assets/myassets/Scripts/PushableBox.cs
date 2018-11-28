using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableBox : MonoBehaviour {

    private BoxCollider boxCollider = null;
    private Vector3 fromPos = Vector3.zero;
    private Vector3 toPos = Vector3.zero;
    private bool pushing = false;
    private float lerp = 0;
    private Player player = null;
    private Rigidbody _rigid;

	// Use this for initialization
	void Start () {
        boxCollider = GetComponent<BoxCollider>();
        _rigid = GetComponent<Rigidbody>();
	}
	
    private Vector3 clampAxis(Vector3 dir)
    {
        Vector3 testdir = dir;
            testdir.y = 0;
        if (Mathf.Abs(dir.x) > 0.75f)
        {
            testdir.z = 0;
        }
        else
        {
            if (Mathf.Abs(testdir.z) > 0.75f)
            {
                testdir.x = 0;
            }
            else
            {
                return Vector3.zero;
            }
        }
        return testdir;
    }

    public bool canMoveInDirection(Vector3 dir)
    {
        /*Vector3 center = gameObject.transform.TransformPoint(boxCollider.center);
        Vector3 extents = boxCollider.bounds.extents;
         center.y -= extents.y -0.1f;
        Vector3 testDir = clampAxis(dir);
        if (testDir.Equals(Vector3.zero))
            return false;

        float testDist = extents.x * 2f;

        Debug.DrawLine(center, center + testDir.normalized * testDist);

        if (Physics.Raycast(center, testDir, testDist))
            return false;

        Vector3 center2 = testDir.normalized * testDist + center;
        if (!Physics.Raycast(center2, Vector3.down, 2f))
            return false;
        return true;*/

        //New Method
        Vector3 testDir = clampAxis(dir);
        if (testDir.Equals(Vector3.zero))
            return false;
        Vector3 center = gameObject.transform.TransformPoint(boxCollider.center);
        Ray ray = new Ray(center, testDir);
        RaycastHit hit;
        return !_rigid.SweepTest(testDir, out hit,2);

    }

    void Update()
    {
        if (pushing)
        {
            lerp += Time.deltaTime*2f;
            transform.position = Vector3.Lerp(fromPos, toPos, lerp);
            if (lerp >= 1)
            {
                pushing = false;
                player.pushState.EndPush();
            }
        }
    }

    public void Push(Player player,Vector3 dir)
    {
        Vector3 offset = clampAxis(dir);
        fromPos = transform.position;
        toPos = transform.position + offset.normalized * 2f;
        lerp = 0;
        this.player = player;
        pushing = true;
    }

}
