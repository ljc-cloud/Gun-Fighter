using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHandler : MonoBehaviour
{
    public Slider PH_Slider;
    public Slider Bullet_Slider;
    public Image MoveState;
    public TMP_Text GunMode;
    public TMP_Text PickUp;

    private float gunModeTextTimer = 0;

    private readonly string runSprite = "Stance_Sprint_Icon";
    private readonly string walkSprite = "Stance_Stand_Icon";

    private void Awake()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().OnDamaged += PlayerUIHandler_OnDamaged;
        foreach (var item in FindObjectsOfType<WeaponAbstract>(true))
        {
            item.OnBulletLeftChanged += PlayerUIHandler_OnBulletShooted;
        }
        Rifle rifle;
        if (GameObject.FindGameObjectWithTag("Weapon").TryGetComponent(out rifle))
        {
            rifle.OnGunModeChanged += Rifle_OnGunModeChanged;
        }
        GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponControl>().OnDetectWeapon += PlayerUIHandler_OnDetectWeapon;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().OnMoveStateChanged += PlayerUIHandler_OnMoveStateChanged;
    }

    private void PlayerUIHandler_OnDetectWeapon(bool pick)
    {
        PickUp.text = pick ? "Press [F] To Pick Up " : "";
    }

    private void Update()
    {
        gunModeTextTimer += Time.deltaTime;
        if (gunModeTextTimer > 2f)
        {
            GunMode.enabled = false;
        }
    }

    private void Rifle_OnGunModeChanged(bool mode)
    {
        GunMode.enabled = true;
        gunModeTextTimer = 0;
        GunMode.text = mode ? "Auto" : "Single";
    }

    private void PlayerUIHandler_OnMoveStateChanged(bool run)
    {
        MoveState.sprite = run ? Resources.Load<Sprite>(runSprite) : Resources.Load<Sprite>(walkSprite);
    }

    private void PlayerUIHandler_OnBulletShooted(int capacity, int left)
    {
        Bullet_Slider.maxValue = capacity;
        Bullet_Slider.value = left;
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
