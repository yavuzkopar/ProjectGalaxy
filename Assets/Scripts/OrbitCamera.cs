using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    [SerializeField] Transform focus = default;
    [SerializeField, Range(1, 20)]
    float distance = 5f;
    [SerializeField, Min(0)]
    float focusRadius;
    [SerializeField, Range(0, 1)]
    float focusCentering = 0.5f;
    Vector3 focusPoint;

    Vector2 orbitAngles = new Vector2(4f, 0);

    [SerializeField, Range(0f, 360f)]
    float rotationSpeed = 90f;

    [SerializeField, Range(-89f, 89f)]
    float minVerticalAngle = -30f, maxVerticalAngle = 60f;
    [SerializeField] LayerMask obscureLayer;
    [SerializeField]
    float xDistance, yDistance;
    Quaternion gravityAlignment = Quaternion.identity;
    Quaternion orbitRotation;

    [SerializeField, Min(0f)]
    float upAlignmentSpeed = 360f;
    private void Awake()
    {
        focusPoint = focus.position;
        transform.localRotation = orbitRotation = Quaternion.Euler(orbitAngles);
    }
    void OnValidate()
    {
        if (maxVerticalAngle < minVerticalAngle)
        {
            maxVerticalAngle = minVerticalAngle;
        }
    }
    void LateUpdate()
    {
        UpdateGravityAlignment();
        UpdateFocusPoint();
        //  Quaternion lookRotation;
        if (ManulaRotation())
        {
            ConstrainAngles();
            //  lookRotation = Quaternion.Euler(orbitAngles);
            orbitRotation = Quaternion.Euler(orbitAngles);
        }
        //else
        //{
        //    lookRotation = transform.localRotation;
        //}
        Quaternion lookRotation = gravityAlignment * orbitRotation;


        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = focusPoint - lookDirection * distance;
        if (Physics.Raycast(
            focusPoint, -lookDirection, out RaycastHit hit, distance, obscureLayer
        ))
        {
            lookPosition = focusPoint - lookDirection * hit.distance;
        }
        transform.SetLocalPositionAndRotation(lookPosition, lookRotation);
    }

    private void UpdateGravityAlignment()
    {
        Vector3 fromUp = gravityAlignment * Vector3.up;
        Vector3 toUp = CustomGravity.GetUpAxis(focusPoint);
        float dot = Mathf.Clamp(Vector3.Dot(fromUp, toUp), -1f, 1f);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
        float maxAngle = upAlignmentSpeed * Time.deltaTime;

        Quaternion newAlignment =
            Quaternion.FromToRotation(fromUp, toUp) * gravityAlignment;
        if (angle <= maxAngle)
        {
            gravityAlignment = newAlignment;
        }
        else
        {
            gravityAlignment = Quaternion.SlerpUnclamped(
                gravityAlignment, newAlignment, maxAngle / angle
            );
        }
    }

    private bool ManulaRotation()
    {
        Vector2 input = new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));
        const float e = 0.001f;
        if (input.x < -e || input.x > e || input.y < -e || input.y > e)
        {
            orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
            return true;
        }
        else
            return false;
    }
    void ConstrainAngles()
    {
        orbitAngles.x =
            Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);

        if (orbitAngles.y < 0f)
        {
            orbitAngles.y += 360f;
        }
        else if (orbitAngles.y >= 360f)
        {
            orbitAngles.y -= 360f;
        }
    }
    void UpdateFocusPoint()
    {
        Vector3 targetPoint = focus.position;
        if (focusRadius > 0)
        {
            float distance = Vector3.Distance(targetPoint, focusPoint);
            float t = 1f;
            if (distance > 0.01f && focusCentering > 0)
            {
                t = Mathf.Pow(1 - focusCentering, Time.unscaledDeltaTime);
            }
            if (distance > focusRadius)
            {
                t = Mathf.Min(t, focusRadius / distance);
            }
            focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
        }
        else
        {
            focusPoint = targetPoint;
        }


    }
}
