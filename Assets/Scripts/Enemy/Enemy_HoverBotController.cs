using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum Enemy_HoverBotState { GUARD, PATROL, CHASE, DEAD }

/// <summary>
/// HoverBot ������ ������NavMesh
/// ����ҹ���ʱ���Զ��л�״̬ Alerted,���������λ��
/// TODO ������Ч����Ҵ�ܿ��ܲ���Ѫ��
/// </summary>
public class Enemy_HoverBotController : MonoBehaviour
{
    // Components
    private NavMeshAgent agent;
    private Enemy_HoverBotAnimatorController animatorController;

    // Animation
    private bool alerted;
    private bool death;
    public bool HasAttacked;

    // enemyState
    public Enemy_HoverBotState enemyState;

    // Prefab
    [Header("Bullet")]
    public GameObject EnemyBulletPrefab;
    [Tooltip("Bullet Instantiate Point")]
    public Transform BulletStartPoint;
    [Tooltip("Bullet Start Velocity")]
    public float bulletStartVelocity = 10f;

    [SerializeField]
    private Transform[] PatrolPoints;
    private int patrolIndex;
    private Vector3 originPatrolPoint;
    private float originSpeed;
    [SerializeField]
    private Transform PlayerTransform;

    // Timers
    private float attackTimer;
    private float patrolStopTimer;
    private float exitAlertTimer;

    [Header("Enemy State")]
    public bool IsGuard;
    public float AlertRadius = 7f;
    public float AttackRadius = 4f;
    public float ChaseSpeedRatio = 1.5f;
    public float AttackInterval = 0.5f;
    public float PatrolStopTime = 1f;
    public float ExitAlertTime = 4f;

    [Header("Player Layer Mask")]
    public LayerMask PlayerLayer;

    private void Awake()
    {
        animatorController = GetComponent<Enemy_HoverBotAnimatorController>();
        agent = GetComponentInChildren<NavMeshAgent>();
    }

    private void Start()
    {
        Transform patrolParentTrans = GameObject.FindGameObjectWithTag("Patrol").transform;
        PatrolPoints = GameObject.FindGameObjectWithTag("Patrol").GetComponentsInChildren<Transform>();
        PatrolPoints = PatrolPoints.Where(item => !item.Equals(patrolParentTrans)).ToArray();
        originPatrolPoint = PatrolPoints[0].position;
        originSpeed = agent.speed;
    }

    private void Update()
    {
        DrawPatrolLine();
        SwitchAnimation();
        DetectPlayer();
        DetectHasAttacked();
        SetEnemyState();
        TimerTick();
    }

    /// <summary>
    /// Timer Tick
    /// </summary>
    private void TimerTick()
    {
        attackTimer += Time.deltaTime;

    }

    /// <summary>
    /// �л�Enemy״̬������״̬����
    /// </summary>
    private void SetEnemyState()
    {
        if (alerted)
        {
            enemyState = Enemy_HoverBotState.CHASE;
        }
        else if (IsGuard)
        {
            enemyState = Enemy_HoverBotState.GUARD;
        }
        else if (!alerted && !IsGuard && !death)
        {
            enemyState = Enemy_HoverBotState.PATROL;
        }
        switch (enemyState)
        {
            case Enemy_HoverBotState.GUARD:
                agent.isStopped = true;
                break;
            case Enemy_HoverBotState.PATROL:
                agent.isStopped = false;
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
                {
                    if (patrolStopTimer >= PatrolStopTime)
                    {
                        SetNextDestination();
                        patrolStopTimer = 0;
                    }
                    else
                    {
                        agent.destination = transform.position;
                        agent.isStopped = true;
                        patrolStopTimer += Time.deltaTime;
                    }
                }
                break;
            case Enemy_HoverBotState.CHASE:
                if (agent.speed < originSpeed * ChaseSpeedRatio)
                {
                    agent.speed *= ChaseSpeedRatio;
                }
                // ���ڹ�����Χ�ڣ�׷
                if (!InAttackRange())
                {
                    agent.isStopped = false;
                    agent.destination = PlayerTransform.position;
                }
                else
                {
                    // �ڷ�Χ�ڹ���
                    if (attackTimer >= AttackInterval)
                    {
                        StartCoroutine(Attack());
                        attackTimer = 0;
                        StopCoroutine("Attack");
                    }
                }
                break;
            case Enemy_HoverBotState.DEAD:
                break;
        }
    }

    /// <summary>
    /// ������ң������ӵ�
    /// </summary>
    /// <returns></returns>
    private IEnumerator Attack()
    {
        if (!PlayerTransform)
        {
            Debug.LogError("Enemy_HoverBotController:135, PlayerTransform Is Null");
        }
        agent.isStopped = true;
        while (true)
        {
            var lookDir = (PlayerTransform.position - transform.position).normalized;
            var tarRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, tarRotation, 0.1f);

            if (Vector3.Angle(transform.forward, lookDir) < 15f || !alerted)
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
    /// ����Ƿ��ڹ�����Χ
    /// </summary>
    /// <returns></returns>
    private bool InAttackRange()
    {
        if (PlayerTransform)
            return Vector3.Distance(PlayerTransform.position, transform.position) <= AttackRadius;
        return false;
    }


    /// <summary>
    /// ����û��Ƿ���AlertRadius��Χ��
    /// </summary>
    private void DetectPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, AlertRadius, PlayerLayer);
        if (colliders != null && colliders.Length > 0)
        {
            var collider = colliders[0];
            if (!alerted && collider.CompareTag("Player"))
            {
                Debug.Log("Detect Player");
                alerted = true;
                PlayerTransform = colliders[0].transform;
            }
        }
        else if (!HasAttacked || exitAlertTimer >= ExitAlertTime)
        {
            alerted = false;
            agent.destination = originPatrolPoint;
        }
    }

    /// <summary>
    /// ����Ƿ���ҹ���
    /// </summary>
    private void DetectHasAttacked()
    {
        if (HasAttacked && exitAlertTimer < ExitAlertTime)
        {
            exitAlertTimer += Time.deltaTime;
            alerted = true;
            PlayerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        else if (!alerted)
        {
            if (HasAttacked && exitAlertTimer >= ExitAlertTime)
            {
                HasAttacked = false;
                alerted = false;
            }
            else
            {
                exitAlertTimer = 0;
                HasAttacked = false;
                alerted = false;
            }
          
        }
    }

    /// <summary>
    /// ������һ��Ѳ�ߵ�
    /// </summary>
    private void SetNextDestination()
    {
        if (PatrolPoints == null || PatrolPoints.Length == 0)
        {
            Debug.LogError("Enemy_HoverBotController:39, PatrolPoints Has No Value");
            return;
        }
        agent.destination = PatrolPoints[patrolIndex++].position;
        patrolIndex = patrolIndex % PatrolPoints.Length;
        originPatrolPoint = PatrolPoints[patrolIndex].position;

    }

    /// <summary>
    /// �л�����
    /// </summary>
    private void SwitchAnimation()
    {
        if (animatorController == null)
        {
            Debug.LogError($"EnemyAnimatorController:24, Anim Is Null");
            return;
        }
        animatorController.MoveSpeed = agent.velocity.sqrMagnitude;
        animatorController.Alerted = alerted;
        animatorController.Death = death;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AlertRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, AttackRadius);
    }

    private void DrawPatrolLine()
    {
        for (int i = 0; i < PatrolPoints.Length; i++)
        {
            if (i < PatrolPoints.Length - 1)
            {
                Debug.DrawLine(PatrolPoints[i].position, PatrolPoints[i + 1].position);
            }
            else
            {
                Debug.DrawLine(PatrolPoints[0].position, PatrolPoints[i].position);
            }
        }
    }
}
