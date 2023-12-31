using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySource : MonoBehaviour
{
    public virtual Vector3 GetGravity(Vector3 position)
    {
        return Physics.gravity;
    }
    private void OnEnable()
    {
        CustomGravity.Register(this);
    }
    private void OnDisable()
    {
        CustomGravity.Unregister(this);
    }
}
