using UnityEngine;
using UnityEngine.UI;

public class EnemyUIHandler : MonoBehaviour
{
    public Slider PH_Slider;

    private void Awake()
    {
        GetComponentInParent<Health>().OnDamaged += EnemyUIHandler_OnDamaged;
    }

    private void EnemyUIHandler_OnDamaged(float maxPh, float ph, float damage)
    {
        if (ph > 0)
        {
            ph = Mathf.Max(ph - damage, 0);
            PH_Slider.value = (ph / maxPh) * PH_Slider.maxValue;
        }
        else
        {
            PH_Slider.value = 0;
        }
    }

    /// <summary>
    /// 血条UI始终正对摄像头
    /// </summary>
    private void LateUpdate()
    {
        Vector3 lookPoint = Vector3.ProjectOnPlane(gameObject.transform.position - Camera.main.transform.position, Camera.main.transform.forward);
        gameObject.transform.LookAt(lookPoint + Camera.main.transform.position);
    }
}
