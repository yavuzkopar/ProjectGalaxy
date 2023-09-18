using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputService
{
    float Horizontal();
    float Vertical();
    bool Jump();
}
