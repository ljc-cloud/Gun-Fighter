using System.Collections;
using UnityEngine;

public class Weapon02 : WeaponAbstract
{
    private AudioSource audioSource;

    protected override void OnEnable()
    {
        base.OnEnable();
        BulletContainerRatio = 0.3f;
        BulletLeft = BulletCapacity;
        reloadInvoke = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    protected override void Start()
    {
        base.Start();
        PerBulletInterval = 0.5f;
        BulletStartVelocity = 200f;
        BulletCapacity = 2;
        WeaponBackRatio = 0.3f;
        bulletContainerOutDistance = 0.04f;
    }

    protected override void Fire()
    {
        Debug.Log(reloadInvoke);
        if (BulletLeft == 0 && !reloadInvoke)
        {
            StartCoroutine("Reload");
            //reloadInvoke = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (BulletLeft > 0)
                OpenFire();
        }
    }

    protected override void OpenFire()
    {
        ProcessFire();
    }

    protected override void BulletContainerAnim(int index)
    {
        BulletContainers[index].Translate(0, bulletContainerOutDistance, 0, Space.Self);
    }

    protected override void PlayShootAudio() => audioSource?.Play();

    protected override IEnumerator WeaponBack()
    {
        while (transform.localPosition != BackPoint.localPosition)
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
}
