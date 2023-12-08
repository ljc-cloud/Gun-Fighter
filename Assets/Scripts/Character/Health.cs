using UnityEngine;

/// <summary>
/// Health 每个角色都会携带这个组件
/// </summary>
public class Health : MonoBehaviour
{
    public float PH = 100f;

    public void TakeDamage(float damage)
    {
        if (PH > 0)
        {
            PH = Mathf.Max(PH - damage, 0);
        }
    }
}
