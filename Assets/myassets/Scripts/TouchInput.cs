using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchInput : MonoBehaviour {

    public Image JoystickImage;
    public Image JumpImage;
    public Image HandImage;

    //Touch control
    private Vector2 touchJoyStart = Vector2.zero;
    private int touchJoyFingerId = -1;
    private Vector2 touchJoy = Vector2.zero;

    private List<ImageButton> imageButtons;

    private Player _player;


    private class ImageButton
    {
        public Image image;
        public Player.ActionButton action;

        public ImageButton(Image image,Player.ActionButton action)
        {
            this.image = image;
            this.action=action;
        }
    }

    // Use this for initialization
    void Start () {
        imageButtons = new List<ImageButton>(3);
        imageButtons.Add(new ImageButton(JumpImage,Player.ActionButton.jump));
        imageButtons.Add(new ImageButton(HandImage, Player.ActionButton.hand));
        _player = FindObjectOfType<Player>();

        if(SystemInfo.deviceType!=DeviceType.Handheld)
        {
            
            JumpImage.enabled = false;
            HandImage.enabled = false;
        }
        JoystickImage.enabled = false;
    }
	
    private Player.ActionButton getActionFromTouchPos(Vector2 pos)
    {

        ImageButton nearestButton = null;
        float nearestDistSqr = float.MaxValue;
        foreach(ImageButton ib in imageButtons)
        {
            float distSqr= (new Vector2(ib.image.transform.position.x, ib.image.transform.position.y) - pos).sqrMagnitude;
            if (distSqr < nearestDistSqr)
            {
                nearestButton = ib;
                nearestDistSqr = distSqr;
            }
        }
        if (nearestButton != null)
            return nearestButton.action;
        return Player.ActionButton.none;
    }

	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire1"))
        {
            JumpImage.enabled = false;
            HandImage.enabled = false;
        }
        if (touchJoyFingerId >= 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.fingerId == touchJoyFingerId)
                {
                    if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
                    {
                        touchJoyFingerId = -1;
                        JoystickImage.enabled = false;
                        _player.SetTouchJoystick(Vector2.zero);
                    }
                    else
                    {
                        touchJoy = (touch.position - touchJoyStart) / (Screen.height * 0.05f);
                        _player.SetTouchJoystick(touchJoy);
                    }

                }
                else
                {
                    if (touch.position.x > Screen.width * 0.5f && touch.phase == TouchPhase.Began)
                    {
                        /*if (touch.position.x > Screen.width * 0.75f)
                        {
                            jumpPressed = true;
                        }
                        else
                        {
                            attackPessed = true;
                        }*/
                        JumpImage.enabled = true;
                        HandImage.enabled = true;

                        Player.ActionButton ab = getActionFromTouchPos(touch.position);
                        _player.PressActionButton(ab);
                    }
                }
            }
        }
        else
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    if (touch.position.x < Screen.width * 0.5f)
                    {
                        touchJoyStart = touch.position;
                        touchJoyFingerId = touch.fingerId;
                        touchJoy = Vector2.zero;
                        _player.SetTouchJoystick(touchJoy);
                        JoystickImage.transform.position = touch.position;
                        JoystickImage.enabled = true;
                    }
                    else
                    {
                        /*if (touch.position.x > Screen.width * 0.75f)
                        {
                            jumpPressed = true;
                        }
                        else
                        {
                            attackPessed = true;
                        }*/
                        JumpImage.enabled = true;
                        HandImage.enabled = true;

                        Player.ActionButton ab = getActionFromTouchPos(touch.position);
                        _player.PressActionButton(ab);
                    }
                }
            }
        }
    }
}
