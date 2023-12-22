using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 玩家移动脚本
/// TODO 完成斜坡移动（下坡）
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;

    [Header("Move Speed")]
    public float Speed;
    public float WalkSpeed = 5f;
    public float RunSpeed = 8f;

    [Header("Vertical Velocity")]
    public float GravityForce = -10f;
    [Tooltip("Character In Vertical Force")] public float VerticalVelocity;
    public Transform CheckGroundPoint;
    public float CheckGroundRadius;
    public LayerMask GroundLayerMask;
    public KeyCode JumpKey;
    [Tooltip("Character Jum Max Height")] public float JumMaxHeight = 5f;

    [Header("Max Slope Angle")]
    public float MaxSlopeAngle = 30f;
    [Range(1, 10)]
    public float GravityWhenFallParam = 2f;

    public event Action<bool> OnMoveStateChanged;
    public event Action<float> OnPickUpHealth;

    private Vector3 moveDir;
    //private bool isWalk;
    private bool isRun;
    private bool isJump;
    private bool isGround = true;
    private bool climb;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }
    private void Start()
    {
        GameManager.Instance.PlayerTransform = transform;
    }

    private void Update()
    {
        CheckGround();
        CheckJump();
        AdjustVerticalVelocity();
    }

    private void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// 获取虚拟轴控制玩家移动
    /// </summary>
    private void Move()
    {
        // 判断是否按下左Shift键，是，加速
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Speed = RunSpeed;
            isRun = true;
        }
        else
        {
            Speed = WalkSpeed;
            isRun = false;
        }
        OnMoveStateChanged?.Invoke(isRun);
        GetMoveAxis();
        if (moveDir != Vector3.zero)
            // Move
            characterController.Move(moveDir * Speed * Time.fixedDeltaTime);

    }
    /// <summary>
    /// 获取前后左右虚拟轴
    /// </summary>
    private void GetMoveAxis()
    {
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        // 第一人称人物移动
        moveDir = transform.right * h + transform.forward * v;
        bool onSlope = IsOnSlope(out Vector3 hitNormal);
        if (onSlope)
        {
            moveDir = Vector3.ProjectOnPlane(moveDir, hitNormal);
        }
        moveDir.Normalize();
    }

    /// <summary>
    /// Check Palyer Is On The Groud
    /// </summary>
    private void CheckGround()
    {
        if (CheckGroundPoint == null)
        {
            Debug.LogError($"PlayerMoveMent:97, CheckGroundPoint Is Null");
            return;
        }
        isGround = Physics.CheckSphere(CheckGroundPoint.position, CheckGroundRadius, GroundLayerMask);
        //Physics.Check
        //if (isGround) Debug.Log("On The Ground");
    }

    /// <summary>
    /// Check Player Is Jump
    /// </summary>
    private void CheckJump() => isJump = isGround && Input.GetKeyDown(JumpKey);

    //private void AdjustForwardDir()
    //{
    //    var planeNormalDir = Vector3.up;
    //    if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1))
    //    {
    //        planeNormalDir = hit.normal;
    //    }
    //    var forward = Vector3.ProjectOnPlane(transform.forward, planeNormalDir);
    //    transform.rotation = Quaternion.LookRotation(forward, planeNormalDir);
    //}

    #region Fixme 爬梯子
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Ladder"))
    //    {
    //        climb = true;
    //        Debug.Log("Climb Ladder");
    //        StartCoroutine(ClimbLadder());
    //    }
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    if (other.CompareTag("Ladder"))
    //    {
    //        StartCoroutine(ClimbLadder());
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Ladder"))
    //    {
    //        climb = false;
    //        Debug.Log("Leave Ladder");
    //        StopCoroutine("ClimbLadder");
    //    }
    //}

    //private IEnumerator ClimbLadder()
    //{
    //    while (climb)
    //    {
    //        Debug.Log("HERE");
    //        var v = Input.GetAxis("Vertical");
    //        var tarPos = new Vector3(transform.position.x, transform.position.y + v, transform.position.z);
    //        transform.position = Vector3.Lerp(transform.position, tarPos, 0.02f);
    //        yield return null;
    //    }
    //}

    #endregion

    private void AdjustVerticalVelocity()
    {
        if (isGround && VerticalVelocity <= 0)
        {
            VerticalVelocity = 0;
        }
        if (!isGround)
        {
            VerticalVelocity += GravityForce * Time.deltaTime * GravityWhenFallParam;
        }
        if (isGround && isJump)
        {
            VerticalVelocity = Mathf.Sqrt(JumMaxHeight * 2 / -GravityForce) * -GravityForce;
        }
        characterController.Move(Vector3.up * VerticalVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Player是否在斜坡上
    /// </summary>
    /// <returns></returns>
    private bool IsOnSlope(out Vector3 hitNormal)
    {
        if (isJump)
        {
            hitNormal = Vector3.zero;
            return false;
        }
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 1f))
        {
            hitNormal = hit.normal;
            float slopeAngle = Vector3.Angle(hitNormal, Vector3.up);
            if (slopeAngle < MaxSlopeAngle)
            {
                return true;
            }
        }
        hitNormal = Vector3.zero;
        return false;
    }

    /// <summary>
    /// 捡到血包
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("HealthPickUp"))
        {
            float healthValue = other.transform.GetComponent<HealthPickUp>().HealthValue;
            float ph = GetComponent<Health>().PH;
            if (ph >= GetComponent<Health>().MaxPH)
                return;
            GetComponent<Health>().PH = Mathf.Min(ph + healthValue, GetComponent<Health>().MaxPH);
            OnPickUpHealth.Invoke(healthValue);
            Destroy(other.gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(CheckGroundPoint.position, CheckGroundRadius);
    }
}
