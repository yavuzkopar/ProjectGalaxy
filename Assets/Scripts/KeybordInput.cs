using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeybordInput : IInputService
{
    public float Horizontal()
    {
        return Input.GetAxis("Horizontal");
    }

    public bool Jump()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public float Vertical()
    {
        return Input.GetAxis("Vertical");
    }
}
