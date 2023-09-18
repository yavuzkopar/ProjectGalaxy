using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoystickInput : IInputService
{
    Joystick joystick;
    JumpButton jumpButton;
    public JoystickInput(Joystick joystick,JumpButton jumpButton)
    {
        this.joystick = joystick;
        this.jumpButton = jumpButton;
    }
    public float Horizontal()
    {
        return joystick.Horizontal;
    }

    public bool Jump()
    {
        return jumpButton.buttonPressed;
    }

    public float Vertical()
    {
        return joystick.Vertical;
    }
}
