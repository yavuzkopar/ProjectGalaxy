using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiInput : IInputService
{
    float horizontal = -1f;
    public AiInput()
    {
        horizontal = Random.Range(-1f, 1f);
    }
    public float Horizontal()
    {
        return horizontal * Time.deltaTime;
    }

    public bool Jump()
    {
        return false;
    }

    public float Vertical()
    {
        return 1f ;
    }
}
