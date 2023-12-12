using System;
using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// Rifle 
/// 装弹及动画
/// FIXME 子弹打完后后坐力不会回来
/// </summary>
public class Rifle : WeaponAbstract
{
    private AudioSource audioSource;

    private bool onFire;
    private int bulletLeft;
    private bool reloadComplete;

    public float WeaponBackRatio = 0.3f;
    public Transform[] BulletContainers;
    private Vector3[] bulletContainersOriginPosition;

    private void Awake()
    {
        PerBulletInterval = 0.1f;
        IsAuto = true;
        BulletStartVelocity = 100f;
        BulletCapacity = 20;
        bulletLeft = BulletCapacity;
        WeaponBackRatio = 0.3f;
        audioSource = GetComponent<AudioSource>();
        bulletContainersOriginPosition = BulletContainers.Select(item => item.localPosition).ToArray();
        audioSource.playOnAwake = false;
    }

    protected override void Update()
    {
        base.Update();
        ChangeGunShootState();
    }

    protected void ChangeGunShootState()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            IsAuto = !IsAuto;
        }
    }

    protected override IEnumerator WeaponBack()
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

    private IEnumerator WeaponToDefault()
    {
        while (transform.localPosition != DefaultPoint.localPosition)
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, DefaultPoint.localPosition, WeaponBackRatio);
            yield return null;
        }
    }

    protected override void Fire()
    {
        // 单击
        if (Input.GetMouseButtonDown(0))
        {
            onFire = true;
            if (bulletLeft > 0)
                OpenFire();
            else
            {
                StopCoroutine("AutoFire");
                Debug.Log("No Bullet Left");
                ////TODO 装弹
                //await Task.Delay(1000);
                StartCoroutine("Reload");

            }

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

    private IEnumerator Reload()
    {
        reloadComplete = false;
        // 
        for (int i = 0; i < BulletContainers.Length; i++)
        {
            while (BulletContainers[i].localPosition != bulletContainersOriginPosition[i])
            {
                BulletContainers[i].localPosition = Vector3.Lerp(BulletContainers[i].localPosition, bulletContainersOriginPosition[i], 0.1f);
                yield return null;
            }
        }
        bulletLeft = BulletCapacity;
        OnBulletLeftChange(BulletCapacity, bulletLeft);
        reloadComplete = true;
    }

    protected override void OpenFire()
    {
        if (!IsAuto)
        {
            ProcessFire();
        }
        else
        {
            StartCoroutine("AutoFire");
        }
    }
    private IEnumerator AutoFire()
    {
        while (IsAuto && bulletLeft > 0 && onFire)
        {
            ProcessFire();
            yield return new WaitForSeconds(PerBulletInterval);
        }
        StartCoroutine("WeaponToDefault");
    }
    private void ProcessFire()
    {
        if (bulletLeft > 0)
        {
            GameObject bullet = Instantiate(BulletPrefab, BulletStartPoint.position, BulletStartPoint.rotation);
            bullet.transform.Rotate(0, 180, 0);
            PlayShootAudio();
            bullet.GetComponent<Rigidbody>().AddForce(BulletStartPoint.forward * BulletStartVelocity, ForceMode.Impulse);
            bullet.GetComponent<Bullet>().BulletState = BulletState.PLAYER_BULLET;
            bulletLeft--;
            OnBulletLeftChange(BulletCapacity, bulletLeft);
            //StopCoroutine("WeaponBack");
            StartCoroutine("WeaponBack");
            StartCoroutine("BulletContainerAnim", (BulletCapacity - bulletLeft - 1) / 5);
            Destroy(bullet, 4f);
        }
    }

    private void BulletContainerAnim(int index)
    {
        BulletContainers[index].Translate(0, 0.01f, 0, Space.Self);
    }


    protected override void PlayShootAudio() => audioSource?.Play();
}
