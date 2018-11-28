using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ziel : MonoBehaviour {

    public AudioSource WinSound;

    private bool _finished = false;
    private Endscreen _endscreen;
    private Player _player;

	// Use this for initialization
	void Start () {
        _endscreen = FindObjectOfType<Endscreen>();
        _player = FindObjectOfType<Player>();
        _endscreen.gameObject.SetActive(false);
	}

    void OnTriggerEnter(Collider other)
    {
        if (!_finished && other.tag == "Player")
        {
            WinSound.Play();
            _endscreen.ShowEndScreen();
            _finished = true;
            _player.CanMove = false;
        }
    }
}
