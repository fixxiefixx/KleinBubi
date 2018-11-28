using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DXState {


    private string name = "";

    public string Name
    {
        get
        {
            return name;
        }
    }

    public DXState(string name)
    {
        this.name = name;
    }

    public virtual void EnterState()
    {

    }

    public virtual void ExitState()
    {

    }

    public virtual void Update()
    {
        
    }

    public virtual void FixedUpdate()
    {

    }
}
