using UnityEngine;

public abstract class WeaponAbstract
{
    public float PerBulletInterval;
    public bool IsAuto;
    public float Damage;
    public float BulletCapacity;
    public GameObject BulletPrefab;
    public float BulletStartVelocity;

    protected void ChangeShootState()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            IsAuto = !IsAuto;
        }
    }

    protected abstract void Fire();
    protected abstract void OpenFire();


}
