using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;
    Mover mover;
    CapsuleCollider capsuleCollider;
    void Start()
    {
        mover = GetComponent<Mover>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        mover.OnJump += JumpAnim;
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dirVector = transform.InverseTransformDirection(rb.velocity);
        animator.SetFloat("Horizontal", dirVector.x);
        animator.SetFloat("Vertical", dirVector.z);
    }
    void JumpAnim()
    {
        animator.SetTrigger("Jump");
    }
    void OnSlideBegin()
    {
        capsuleCollider.enabled = false;
    }
    void OnSlideEnd()
    {
        capsuleCollider.enabled = true;
    }
}
