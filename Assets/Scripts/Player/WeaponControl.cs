using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// 
/// </summary>
public class WeaponControl : MonoBehaviour
{
    public LayerMask PickUpLayer;
    public Transform WeaponParentTransform;

    public event Action<bool> OnDetectWeapon;
    public event Action OnDropWeapon;

    private bool couldPickUp;
    private GameObject weaponObj;

    private readonly int WEAPON_LAYER = 6;

    public Dictionary<string, int> WeaponBulletLeft = new Dictionary<string, int>();

    private void Update()
    {
        DetectWeapon();
        PickUpWeapon();
    }

    private void PickUpWeapon()
    {
        bool b = couldPickUp && Input.GetKeyDown(KeyCode.F);
        if (b)
        {
            //
            couldPickUp = false;
            OnDropWeapon.Invoke();
            ChangeLayer(weaponObj, "Weapon");
            weaponObj.GetComponent<WeaponAbstract>().enabled = true;
            weaponObj.GetComponent<ParentConstraint>().constraintActive = true;
            weaponObj.GetComponent<AudioSource>().enabled = true;
            weaponObj.GetComponent<Collider>().enabled = false;
            weaponObj.GetComponent<WeaponAbstract>().BulletLeft = WeaponBulletLeft[weaponObj.name];
            weaponObj.transform.SetParent(WeaponParentTransform);
            weaponObj.transform.localPosition = Vector3.zero;
            weaponObj.transform.localRotation = Quaternion.identity;
        }
    }

    private void ChangeLayer(GameObject go, string layerName)
    {
        go.layer = WEAPON_LAYER;
        foreach (Transform child in go.transform.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }

    private void DetectWeapon()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2.5f, PickUpLayer))
        {
            couldPickUp = true;
            weaponObj = hit.collider.gameObject;
            OnDetectWeapon.Invoke(true);
        }
        else
        {
            couldPickUp = false;
            OnDetectWeapon.Invoke(false);
        }
    }
}
