using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButton : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public bool buttonPressed;
    private void LateUpdate()
    {
        buttonPressed = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        buttonPressed = true;
        Debug.Log("qqqqqqqqqqqqq");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        buttonPressed = false;
    }
}
