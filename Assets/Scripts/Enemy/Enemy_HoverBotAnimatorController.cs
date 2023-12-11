using UnityEngine;

public class Enemy_HoverBotAnimatorController : MonoBehaviour
{
    private Animator anim;

    public float MoveSpeed;
    public bool Alerted;
    public bool Death;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        anim.SetFloat("MoveSpeed", MoveSpeed);   
        anim.SetBool("Alerted", Alerted);
        anim.SetBool("Death", Death);
    }

    public void TriggerAttack() => anim?.SetTrigger("Attack");
    public void TriggerOnDamaged() => anim?.SetTrigger("OnDamaged");
}
