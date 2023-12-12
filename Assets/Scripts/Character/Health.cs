using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Health 每个角色都会携带这个组件
/// </summary>
public class Health : MonoBehaviour
{
    public float PH = 100f;
    public float MaxPH = 100f;
    public Slider PH_Slider;
    public GameObject BotExplosion;

    public void TakeDamage(float damage)
    {
        if (PH > 0)
        {
            PH = Mathf.Max(PH - damage, 0);
            PH_Slider.value = (PH / MaxPH) * PH_Slider.maxValue;
        }
        else
        {
            PH_Slider.value = 0;
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
