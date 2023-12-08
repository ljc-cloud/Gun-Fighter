using UnityEngine;

/// <summary>
/// Health ÿ����ɫ����Я��������
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
