using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundButton : MonoBehaviour {

    public bool Once = false;
    public Mechanismus PushedMechanismus;
    public Mechanismus PulledMechanismus;

    private bool _pushed = false;
    private Animator anim;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}

    /// <summary>
    /// Wird aufgerufen wenn ein Collider den Collider dieses Objektes eindringt.
    /// </summary>
    /// <param name="other">Der Collider der in diesen Collider eingedrungen ist.</param>
    void OnTriggerEnter(Collider other)
    {
        if (!_pushed && other.tag == "Player")
        {
            _pushed = true;
            anim.SetBool("pushed", true);
            if(PushedMechanismus!=null)
            {
                PushedMechanismus.Execute();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!Once && _pushed && other.tag == "Player")
        {
            _pushed = false;
            anim.SetBool("pushed", false);
            if (PulledMechanismus != null)
            {
                PulledMechanismus.Execute();
            }
        }
    }
}
