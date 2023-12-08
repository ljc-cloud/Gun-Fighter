using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// AK47 
/// 枪口初速 710米/秒
/// 理论射速 600发/分钟
/// 弹匣容量 30发
/// 表尺射程 800米
/// 有效射程 300米
/// 
/// TODO 设置准星，使子弹可以命中准星
/// TODO 添加音效
/// </summary>
public class WeaponControl : MonoBehaviour
{
    // Components
    private AudioSource audioSource;

    public TMP_Text ModeText;

    public Transform BulletStartPoint;
    public GameObject BulletPrefab;
    public float BulletStartVelocity;
    public Transform DefaultPoint;
    public Transform BackPoint;
    public Vector3 DefaultWeaponCameraPos;
    public Vector3 AimWeaponCameraPos;

    public bool IsAuto;
    public float WeaponBackRatio = 0.3f;
    public float PerBulletInterval = 0.1f;
    private bool onFire;
    private Camera weaponCamera;
    private Camera mainCamera;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        weaponCamera = GameObject.FindGameObjectWithTag("WeaponCamera").GetComponent<Camera>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        ChangeGunShootState();
        ChangeWeaponCameraPos();
        Fire();
    }

    private void ChangeWeaponCameraPos()
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

    /// <summary>
    /// Toggle枪的射击模式 手动 or 自动
    /// </summary>
    private void ChangeGunShootState()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            IsAuto = !IsAuto;
        }
        // TODO UI提示
    }

    /// <summary>
    /// 读取鼠标输入
    /// </summary>
    private void Fire()
    {
        // 单击
        if (Input.GetMouseButtonDown(0))
        {
            onFire = true;
            OpenFire();
        }
        else if (Input.GetMouseButton(0))
        {
            // 长按
        }
        else if (Input.GetMouseButtonUp(0))
        {
            onFire = false;
            StopCoroutine("AutoFire");
        }
    }

    private void OpenFire()
    {
        if (!IsAuto)
        {
            GameObject bullet = Instantiate(BulletPrefab, BulletStartPoint.position, BulletStartPoint.rotation);
            bullet.transform.Rotate(0, 180, 0);
            PlayShootAudio();
            bullet.GetComponent<Rigidbody>().AddForce(BulletStartPoint.forward * BulletStartVelocity, ForceMode.Impulse);
            Destroy(bullet, 4f);
        }
        else
        {
            StartCoroutine("AutoFire");
        }
    }

    private IEnumerator AutoFire()
    {
        while (IsAuto && onFire)
        {
            GameObject bullet = Instantiate(BulletPrefab, BulletStartPoint.position, BulletStartPoint.rotation);
            bullet.transform.Rotate(0, 180, 0);
            PlayShootAudio();
            bullet.GetComponent<Rigidbody>().AddForce(BulletStartPoint.forward * BulletStartVelocity, ForceMode.Impulse);
            StartCoroutine("WeaponBack");
            Destroy(bullet, 4f);
            yield return new WaitForSeconds(PerBulletInterval);
        }
    }

    private IEnumerator WeaponBack()
    {
        if (DefaultPoint != null || BackPoint != null)
        {
            while (onFire && transform.localPosition != BackPoint.localPosition)
            {
                transform.localPosition = Vector3.Slerp(transform.localPosition, BackPoint.localPosition, WeaponBackRatio);

                yield return null;
            }

            while (transform.localPosition != DefaultPoint.localPosition)
            {
                transform.localPosition = Vector3.Slerp(transform.localPosition, DefaultPoint.localPosition, WeaponBackRatio);
                yield return null;
            }
        }
        else
        {
            Debug.LogError("GunFire:92 DefaultPoint Or BackPoint Is Null");
        }
    }

    private void PlayShootAudio() => audioSource?.Play();

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(BulletStartPoint.position, BulletStartPoint.forward * 100);
    }
}
