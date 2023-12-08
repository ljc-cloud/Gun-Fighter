using System;
using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    private Animator anim;

    private float MoveSpeed;
    private bool Alerted;
    private bool Death;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (anim == null) 
        {
            Debug.LogError($"EnemyAnimatorController:24, Anim Is Null");
            return;
        }
        anim.SetFloat("MoveSpeed", MoveSpeed);
        anim.SetBool("Alerted", Alerted);
        anim.SetBool("Death", Death);
    }

    private void TriggerAttack() => anim?.SetTrigger("Attack");
    private void TriggerOnDamage() => anim?.SetTrigger("OnDamage");
}
