using System;
using UnityEngine;

public class Enemy_TurrentAnimatorController : MonoBehaviour
{

    private Animator anim;

    public bool Death;
    public bool IsActive;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        anim.SetBool("IsActive", IsActive);
        anim.SetBool("Death", Death);
    }

    public void TriggerAttack() => anim?.SetTrigger("Attack");
    public void TriggerOnDamaged() => anim?.SetTrigger("OnDamaged");
}
