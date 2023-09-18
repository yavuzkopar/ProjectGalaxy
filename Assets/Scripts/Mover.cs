using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mover : MonoBehaviour
{
    [Range(0, 100)]
    [SerializeField] float maxSpeed;
    [Range(0, 100)]
    [SerializeField] float maxAcceleration, maxAirAcceleration = 1f;
    Vector3 velocity;
    Vector3 desiredVelocity;
    [SerializeField]
    Rect allowedArea = new Rect(-5, -5, 10, 10);
    [Range(0, 1)]
    [SerializeField] float bounciness;

    Rigidbody rigidbody;
    bool desiredJump;
    [SerializeField, Range(0, 10)]
    float jumpHeight;
    [SerializeField, Range(0, 5)]
    int maxAirJump;
    int jumpPhase;
    int groundContactCount;

    bool OnGround => groundContactCount > 0;


    [SerializeField, Range(0, 90)]
    float maxGroundAngle = 25f;
    float minGroundDotProduct;
    Vector3 contactNormal;

    [SerializeField]
    Transform cameraTransform;

    int stepSinceLastGrounded, stepSinceLastJump;

    [SerializeField, Range(0f, 100f)]
    float maxSnapSpeed = 100f;
    [SerializeField, Min(0f)]
    float probeDistance = 1f;
    [SerializeField]
    LayerMask probeMask = -1;

    Vector3 upAxis, rightAxis, forwardAxis;
    [SerializeField] Transform cocuk;
    public event Action OnJump;
    IInputService input;

    [SerializeField]
    InputType inputType;
    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        OnValidate();

        input = GetInputType();
    }
    IInputService GetInputType()
    {
        switch (inputType)
        {
            case InputType.Keyboard:
                return new KeybordInput();
            case InputType.Joystick:
                return new JoystickInput(FindObjectOfType<Joystick>(), FindObjectOfType<JumpButton>());
            case InputType.AI:
                return new AiInput();
            default:
                return new KeybordInput();
        }
    }
    void Update()
    {
        // transform.rotation = cameraTransform.rotation;

        // transform.up = upAxis;



        Vector2 playerInput;
        playerInput.x = input.Horizontal();
        playerInput.y = input.Vertical();
        playerInput = Vector2.ClampMagnitude(playerInput, 1);
        if (cameraTransform != null)
        {
            //desiredVelocity = cameraTransform.TransformDirection(
            //    playerInput.x, 0f, playerInput.y
            //) * maxSpeed;
            rightAxis = ProjectDirectionOnPlane(cameraTransform.right, upAxis);
            forwardAxis =
                ProjectDirectionOnPlane(cameraTransform.forward, upAxis);



        }
        else
        {
            rightAxis = ProjectDirectionOnPlane(transform.right, upAxis);
            forwardAxis = ProjectDirectionOnPlane( transform.forward, upAxis);

        }
        desiredVelocity = new Vector3(playerInput.x, 0, playerInput.y) * maxSpeed;

        desiredJump |= input.Jump();
        if (desiredVelocity.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(rightAxis * desiredVelocity.x + forwardAxis * desiredVelocity.z, upAxis);
        }
        else
            transform.up = upAxis;
        

    }
    private void LateUpdate()
    {
        
            
        

    }
    private void FixedUpdate()
    {
        Vector3 gravity = CustomGravity.GetGravity(rigidbody.position, out upAxis);
        UpdateState();
        AdjustVelocity();


        if (desiredJump)
        {
            desiredJump = false;
            Jump(gravity);

        }
        velocity += gravity * Time.deltaTime;

        rigidbody.velocity = velocity;
        Vector3 dirVector = transform.InverseTransformDirection(velocity);

        ClearState();
    }

    private void ClearState()
    {
        groundContactCount = 0;
        contactNormal = Vector3.zero;
    }

    void AdjustVelocity()
    {
        Vector3 xAxis = ProjectDirectionOnPlane(rightAxis, contactNormal);
        Vector3 zAxis = ProjectDirectionOnPlane(forwardAxis, contactNormal);

        float currentX = Vector3.Dot(velocity, xAxis);
        float currentZ = Vector3.Dot(velocity, zAxis);

        float acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        float maxSpeedChange = acceleration * Time.deltaTime;

        float newX = Mathf.MoveTowards(currentX, desiredVelocity.x, maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, desiredVelocity.z, maxSpeedChange);
        velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);


    }
    Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
    {
        return (direction - normal * Vector3.Dot(direction, normal)).normalized;
    }

    private void UpdateState()
    {
        velocity = rigidbody.velocity;
        stepSinceLastGrounded++;
        stepSinceLastJump++;
        if (OnGround || SnappedToGround())
        {
            stepSinceLastGrounded = 0;
            jumpPhase = 0;
            if (groundContactCount > 1)
            {
                contactNormal.Normalize();
            }

        }
        else
        {
            contactNormal = upAxis;
        }
    }
    bool SnappedToGround()
    {
        if (stepSinceLastGrounded > 1 || stepSinceLastJump <= 2)
        {
            return false;
        }
        float speed = velocity.magnitude;
        if (speed > maxSnapSpeed)
        {
            return false;
        }
        bool isHitted = Physics.Raycast(rigidbody.position, -upAxis, out RaycastHit hit, probeDistance, probeMask);
        if (isHitted)
        {
            return false;
        }
        float upDot = Vector3.Dot(upAxis, hit.normal);
        if (upDot < minGroundDotProduct)
        {
            return false;
        }
        groundContactCount = 1;
        contactNormal = hit.normal;

        float dot = Vector3.Dot(velocity, hit.normal);
        if (dot > 0)
        {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }

        return true;
    }
    private void Jump(Vector3 gravity)
    {
        Vector3 jumpDirection = contactNormal;
        bool isJumpPhaseUnder = jumpPhase < maxAirJump;
        if (OnGround || isJumpPhaseUnder)
        {
            OnJump?.Invoke();
            stepSinceLastJump = 0;
            jumpPhase++;
            float jumpSpeed = Mathf.Sqrt(2f * gravity.magnitude * jumpHeight);
            float alignedSpeed = Vector3.Dot(velocity, jumpDirection);
            if (alignedSpeed > 0)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0);
            }
            jumpDirection = (jumpDirection + upAxis).normalized;
            velocity += jumpDirection * jumpSpeed;
        }

    }

    private Vector3 Bounce(Vector3 newPosition)
    {
        if (newPosition.x < allowedArea.xMin)
        {
            newPosition.x = allowedArea.xMin;
            velocity.x = -velocity.x * bounciness;
        }
        else if (newPosition.x > allowedArea.xMax)
        {
            newPosition.x = allowedArea.xMax;
            velocity.x = -velocity.x * bounciness;
        }
        if (newPosition.z > allowedArea.yMax)
        {
            newPosition.z = allowedArea.yMax;
            velocity.z = -velocity.z * bounciness;
        }
        else if (newPosition.z < allowedArea.yMin)
        {
            newPosition.z = allowedArea.yMin;
            velocity.z = -velocity.z * bounciness;
        }

        return newPosition;
    }
    private void OnCollisionEnter(Collision collision)
    {
        EveluateCollision(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        EveluateCollision(collision);
    }

    private void EveluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            float upDot = Vector3.Dot(upAxis, normal);
            if (upDot >= minGroundDotProduct)
            {
                groundContactCount++;
                contactNormal += normal;
            }
        }

    }
}
public enum InputType
{
    Keyboard,
    Joystick,
    AI
}
