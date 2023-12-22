using System;
using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// FIXME 切枪后不能装弹
/// </summary>
public abstract class WeaponAbstract : MonoBehaviour
{
    public float PerBulletInterval;

    public float WeaponBackRatio = 0.3f;
    public int BulletCapacity;
    public GameObject BulletPrefab;
    public float BulletStartVelocity;

    public Transform BulletStartPoint;
    public Transform DefaultPoint;
    public Transform BackPoint;
    public Transform[] BulletContainers;
    public int BulletLeft;
    public float BulletContainerRatio;

    public Vector3 DefaultWeaponCameraPos;
    public Vector3 AimWeaponCameraPos;

    protected Camera weaponCamera;
    protected Camera mainCamera;

    protected bool reloadComplete;
    protected Vector3[] bulletContainersOriginPosition;
    protected bool reloadInvoke;
    protected float bulletContainerOutDistance;
    private Transform playerTransform;

    public event Action<int, int> OnBulletLeftChanged;

    private void Awake()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponControl>().WeaponBulletLeft.Add(gameObject.name, BulletLeft);
    }

    protected virtual void OnEnable()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        weaponCamera = GameObject.FindGameObjectWithTag("WeaponCamera").GetComponent<Camera>();
        GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponControl>().OnDropWeapon += WeaponAbstract_OnDropWeapon;
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        bulletContainersOriginPosition = BulletContainers.Select(item => item.localPosition).ToArray();
    }

    protected virtual void Start()
    {
        OnBulletLeftChanged.Invoke(BulletCapacity, BulletLeft);
    }

    /// <summary>
    /// 
    /// </summary>
    private void WeaponAbstract_OnDropWeapon()
    {
        // 存储剩余子弹数，等待下次回来时恢复
        GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponControl>().WeaponBulletLeft[gameObject.name] = BulletLeft;
        //transform.SetParent(GameObject.FindGameObjectWithTag("Ground").transform);
        transform.SetParent(null);
        GetComponent<WeaponAbstract>().enabled = false;
        GetComponent<Collider>().enabled = true;
        ChangeLayer(gameObject, "UnPickUpWeapon");
        transform.position = new Vector3(playerTransform.position.x, 0.35f, playerTransform.position.z);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        GetComponent<AudioSource>().enabled = false;
    }

    private void OnDisable()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponControl>().OnDropWeapon -= WeaponAbstract_OnDropWeapon;
    }

    private void ChangeLayer(GameObject go, string layerName)
    {
        go.layer = LayerMask.NameToLayer(layerName);
        foreach (Transform child in go.transform.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = LayerMask.NameToLayer(layerName);
        }
    }


    protected virtual void Update()
    {
        ChangeWeaponCameraPos();
        Fire();
        ReloadCheck();
    }

    private void ReloadCheck()
    {
        if (!reloadInvoke && Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }

    protected IEnumerator Reload()
    {
        reloadInvoke = true;
        reloadComplete = false;
        for (int i = 0; i < BulletContainers.Length; i++)
        {
            while (BulletContainers[i].localPosition != bulletContainersOriginPosition[i])
            {
                BulletContainers[i].localPosition = Vector3.Lerp(BulletContainers[i].localPosition, bulletContainersOriginPosition[i], 0.05f);
                yield return null;
            }
        }
        BulletLeft = BulletCapacity;
        OnBulletLeftChange(BulletCapacity, BulletLeft);
        reloadComplete = true;
        reloadInvoke = false;
    }

    protected abstract IEnumerator WeaponBack();
    protected void ProcessFire()
    {
        if (BulletLeft > 0)
        {
            GameObject bullet = Instantiate(BulletPrefab, BulletStartPoint.position, BulletStartPoint.rotation);
            bullet.transform.Rotate(0, 180, 0);
            PlayShootAudio();
            bullet.GetComponent<Rigidbody>().AddForce(BulletStartPoint.forward * BulletStartVelocity, ForceMode.Impulse);
            bullet.GetComponent<Bullet>().BulletState = BulletState.PLAYER_BULLET;
            BulletLeft--;
            OnBulletLeftChange(BulletCapacity, BulletLeft);
            StartCoroutine(WeaponBack());
            StartCoroutine("BulletContainerAnim", (BulletCapacity - BulletLeft - 1) / (BulletCapacity / BulletContainers.Length));
            Destroy(bullet, 4f);
        }
    }
    protected abstract void BulletContainerAnim(int index);


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
