using System.Collections;
using UnityEngine;

public enum Enemy_TurrentState { IDLE, ACTIVE }

public class Enemy_TurrentController : MonoBehaviour
{
    // Components
    private Enemy_TurrentAnimatorController animatorController;

    // Animation
    private bool isActive;

    private Enemy_TurrentState enemyState;

    public GameObject EnemyBulletPrefab;
    public Transform BulletStartPoint;
    public float bulletStartVelocity = 10f;

    private Transform PlayerTransform;

    private float attackTimer;
    public float ExitAlertTimer;

    public Transform TurrentTransform;

    public float AlertRadius = 7f;
    public float AttackInterval = 0.5f;
    public float ExitAlertTime = 4f;
    public LayerMask PlayerLayer;
    private void Awake()
    {
        animatorController = GetComponent<Enemy_TurrentAnimatorController>();
    }

    private void Update()
    {
        DetectPlayer();
        SetEnemyState();
        SwitchAnimation();
        TimerTick();
    }

    void TimerTick()
    {
        attackTimer += Time.deltaTime;
    }


    /// <summary>
    /// 切换动画
    /// </summary>
    private void SwitchAnimation()
    {
        if (animatorController == null)
        {
            Debug.LogError($"EnemyAnimatorController:24, Anim Is Null");
            return;
        }
        animatorController.IsActive = isActive;
    }

    /// <summary>
    /// 切换Enemy状态，根据状态做事
    /// </summary>
    private void SetEnemyState()
    {
        if (isActive)
        {
            enemyState = Enemy_TurrentState.ACTIVE;
        }
        else
        {
            enemyState = Enemy_TurrentState.IDLE;
        }
        switch (enemyState)
        {
            case Enemy_TurrentState.IDLE:
                
                break;
            case Enemy_TurrentState.ACTIVE:
                if (attackTimer > 1f)
                {
                    StartCoroutine("Attack");
                    attackTimer = 0;
                }            
                break;
        }
    }

    /// <summary>
    /// 攻击玩家，发射子弹
    /// TODO Turrent攻击时，四角架不动
    /// </summary>
    /// <returns></returns>
    private IEnumerator Attack()
    {
        if (!PlayerTransform)
        {
            Debug.LogError("Enemy_TurrentController:73, PlayerTransform Is Null");
        }
        while (true)
        {
            var lookDir = (PlayerTransform.position - transform.position).normalized;
            var tarRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, tarRotation, 0.04f);
            if (Vector3.Angle(transform.forward, lookDir) < 15f || !isActive)
            {
                break;
            }
            yield return null;

        }
        var dir = (PlayerTransform.position - transform.position).normalized;
        GameObject bullet = Instantiate(EnemyBulletPrefab, BulletStartPoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().BulletState = BulletState.ENEMY_BULLET;
        bullet.GetComponent<Rigidbody>().AddForce(dir * bulletStartVelocity, ForceMode.Impulse);
        animatorController.TriggerAttack();
        Destroy(bullet, 4f);
    }

    /// <summary>
    /// 侦测用户是否在AlertRadius范围内
    /// 找到玩家，切换状态 Alerted
    /// </summary>
    private void DetectPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, AlertRadius, PlayerLayer);
        if (colliders != null && colliders.Length > 0)
        {
            var collider = colliders[0];
            if (!isActive && collider.CompareTag("Player"))
            {
                isActive = true;
                PlayerTransform = colliders[0].transform;
            }
        }
        else
        {
            isActive = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AlertRadius);
    }
}
