using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : MonoBehaviour
{
    public Slider PH_Slider;
    public Slider Bullet_Slider;
    public Image MoveState;

    private readonly string runSprite = "Stance_Sprint_Icon";
    private readonly string walkSprite = "Stance_Stand_Icon";

    private void Awake()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().OnDamaged += PlayerUIHandler_OnDamaged;
        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Rifle>().OnBulletLeftChanged += PlayerUIHandler_OnBulletShooted;
        GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerMovement>().OnMoveStateChanged += PlayerUIHandler_OnMoveStateChanged;
    }

    private void PlayerUIHandler_OnMoveStateChanged(bool run)
    {
        MoveState.sprite = run ? Resources.Load<Sprite>(runSprite) : Resources.Load<Sprite>(walkSprite);
    }

    private void PlayerUIHandler_OnBulletShooted(int capacity, int left)
    {
        Bullet_Slider.maxValue = capacity;
        Bullet_Slider.value = left;
        Debug.Log(Bullet_Slider.value);
    }

    private void PlayerUIHandler_OnDamaged(float maxPh, float ph, float damage)
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
}
