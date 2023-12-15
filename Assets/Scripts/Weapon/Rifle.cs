using System;
using System.Collections;
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

    public bool IsAuto;

    public event Action<bool> OnGunModeChanged;

    protected override void OnEnable()
    {
        base.OnEnable();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        BulletContainerRatio = 0.1f;
        PerBulletInterval = 0.1f;
        IsAuto = true;
        BulletStartVelocity = 100f;
        BulletCapacity = 20;
        BulletLeft = BulletCapacity;
        WeaponBackRatio = 0.3f;
        bulletContainerOutDistance = 0.01f;
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
            OnGunModeChanged.Invoke(IsAuto);
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
        if (BulletLeft == 0 && !reloadInvoke)
        {
           
            StartCoroutine("Reload");
        }
        // 单击
        if (Input.GetMouseButtonDown(0))
        {
            onFire = true;
            if (BulletLeft > 0)
                OpenFire();
            else
            {
                StopCoroutine("AutoFire");
                //StartCoroutine("Reload");
            }

        }
        else if (Input.GetMouseButtonUp(0))
        {
            onFire = false;
            StopCoroutine("AutoFire");
        }
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
        while (IsAuto && BulletLeft > 0 && onFire)
        {
            ProcessFire();
            yield return new WaitForSeconds(PerBulletInterval);
        }
        StartCoroutine("WeaponToDefault");
    }

    protected override void BulletContainerAnim(int index)
    {
        Debug.Log($"Diantance = {bulletContainerOutDistance}");
        BulletContainers[index].Translate(0, bulletContainerOutDistance, 0, Space.Self);
    }
    //private void ProcessFire()
    //{
    //    if (BulletLeft > 0)
    //    {
    //        GameObject bullet = Instantiate(BulletPrefab, BulletStartPoint.position, BulletStartPoint.rotation);
    //        bullet.transform.Rotate(0, 180, 0);
    //        PlayShootAudio();
    //        bullet.GetComponent<Rigidbody>().AddForce(BulletStartPoint.forward * BulletStartVelocity, ForceMode.Impulse);
    //        bullet.GetComponent<Bullet>().BulletState = BulletState.PLAYER_BULLET;
    //        BulletLeft--;
    //        OnBulletLeftChange(BulletCapacity, BulletLeft);
    //        StartCoroutine(WeaponBack());
    //        StartCoroutine("BulletContainerAnim", (BulletCapacity - BulletLeft - 1) / (BulletCapacity / BulletContainers.Length));
    //        Destroy(bullet, 4f);
    //    }
    //}

    protected override void PlayShootAudio() => audioSource?.Play();
}
