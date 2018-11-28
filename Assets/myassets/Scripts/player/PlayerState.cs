using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : DXState
{

    protected Player player;

    public PlayerState(GameObject go,string name):base(name)
    {
        player = go.GetComponent<Player>();
    }


}
