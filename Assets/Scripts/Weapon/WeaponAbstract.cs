using System;
using System.Collections;
using UnityEngine;

public abstract class WeaponAbstract : MonoBehaviour
{
    public float PerBulletInterval;
    public bool IsAuto;
    public int BulletCapacity;
    public GameObject BulletPrefab;
    public float BulletStartVelocity;

    public Transform BulletStartPoint;
    public Transform DefaultPoint;
    public Transform BackPoint;

    public Vector3 DefaultWeaponCameraPos;
    public Vector3 AimWeaponCameraPos;

    protected Camera weaponCamera;
    protected Camera mainCamera;

    public event Action<int, int> OnBulletLeftChanged;

    protected virtual void Update()
    {
        ChangeWeaponCameraPos();
        Fire();
    }

    private void Start()
    {
        weaponCamera = GameObject.FindGameObjectWithTag("WeaponCamera").GetComponent<Camera>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    protected abstract IEnumerator WeaponBack();

    protected void OnBulletLeftChange(int capacity, int left)
    {
        if (OnBulletLeftChanged != null)
        {
            OnBulletLeftChanged.Invoke(capacity, left);
        }
    }

    protected void ChangeWeaponCameraPos()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StopCoroutine("ChangeWeaponCameraToDefaultPos");
            StartCoroutine("ChangeWeaponCameraToAimPos");
        }
        else if (Input.GetMouseButtonUp(1))
        {
            StopCoroutine("ChangeWeaponCameraToAimPos");
            StartCoroutine("ChangeWeaponCameraToDefaultPos");
        }
    }

    private IEnumerator ChangeWeaponCameraToAimPos()
    {
        while (weaponCamera.transform.localPosition != AimWeaponCameraPos)
        {
            weaponCamera.transform.localPosition = Vector3.Lerp(weaponCamera.transform.localPosition, AimWeaponCameraPos, 0.1f);
            weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, 30, 0.1f);
            mainCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, 30, 0.1f);
            yield return null;
        }
    }
    private IEnumerator ChangeWeaponCameraToDefaultPos()
    {
        while (weaponCamera.transform.localPosition != DefaultWeaponCameraPos)
        {
            weaponCamera.transform.localPosition = Vector3.Lerp(weaponCamera.transform.localPosition, DefaultWeaponCameraPos, 0.1f);
            weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, 60, 0.1f);
            mainCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, 60, 0.1f);
            yield return null;
        }
    }
    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(BulletStartPoint.position, BulletStartPoint.forward * 100);
    }
    protected abstract void Fire();
    protected abstract void OpenFire();
    protected abstract void PlayShootAudio();

    
}
