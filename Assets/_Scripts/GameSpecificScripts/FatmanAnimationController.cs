using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FatmanAnimationController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        RandomIdleAnims();
      //  RandomDanceAnims();
    }

    public void RandomDanceAnims()
    {
        animator.SetInteger("DanceIndex", Random.Range(0, 5));
        animator.SetTrigger("Dance");
    }

    public void RandomIdleAnims()
    {
        animator.SetInteger("IdleIndex", Random.Range(0, 3));
        animator.SetTrigger("Idle");
    }
}
