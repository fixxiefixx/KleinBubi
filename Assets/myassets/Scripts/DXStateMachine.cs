using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DXStateMachine : MonoBehaviour {

    public bool debugMode = false;

    private DXState currentState=null;

    public DXState State
    {
        get
        {
            return currentState;
        }

        set
        {
            if (currentState != value)
            {
                if (currentState != null)
                {
                    currentState.ExitState();
                }
                currentState = value;
                if (value != null)
                {
                    value.EnterState();
                    if(debugMode)
                        Debug.Log("State switched to " + value.Name);
                }
            }
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public void StateUpdate () {
        if (currentState != null)
        {
            currentState.Update();
        }
	}

    public void StateFixedUpdate()
    {
        if(currentState!=null)
        {
            currentState.FixedUpdate();
        }
    }

    void OnGUI()
    {
        if (Application.isEditor && debugMode)  // or check the app debug flag
        {
            GUI.Label(new Rect(10, 10, Screen.width - 10, Screen.height - 10), "State= "+currentState==null?"null":currentState.Name);
        }
    }
}
