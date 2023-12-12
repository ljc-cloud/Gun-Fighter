using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Health ÿ����ɫ����Я��������
/// </summary>
public class Health : MonoBehaviour
{
    public float PH = 100f;
    public float MaxPH = 100f;
    public GameObject BotExplosion;

    public event Action<float, float, float> OnDamaged;

    public void TakeDamage(float damage)
    {
        if (PH > 0)
        {
            PH = Mathf.Max(PH - damage, 0);
            OnDamaged.Invoke(MaxPH, PH, damage);
        }
        else
        {
            CreateBotExplosion();
            Destroy(gameObject);
        }
    }

    private void CreateBotExplosion()
    {
        if (BotExplosion)
        {
            GameObject exp = Instantiate(BotExplosion, transform.position, transform.rotation);
            Destroy(exp, 1.3f);
        }
    }
}
