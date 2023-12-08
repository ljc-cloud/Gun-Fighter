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
    public float WalkSpeed = 10f;
    public float RunSpeed = 15f;

    [Header("Vertical Velocity")]
    public float GravityForce = -10f;
    [Tooltip("Character In Vertical Force")]public float VerticalVelocity;
    public Transform CheckGroundPoint;
    public float CheckGroundRadius;
    public LayerMask GroundLayerMask;
    public KeyCode JumpKey;
    [Tooltip("Character Jum Max Height")]public float JumMaxHeight = 5f;

    [Header("Max Slope Angle")]
    public float MaxSlopeAngle = 30f;
    [Range(1,10)]
    public float GravityWhenFallParam = 2f;

    private Vector3 moveDir;
    //private bool isWalk;
    private bool isRun;
    private bool isJump;
    private bool isGround = true;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
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
        GetMoveAxis();
        if (moveDir != Vector3.zero)
        {
            // Move
            characterController.Move(moveDir * Speed * Time.fixedDeltaTime);
        }
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
    /// Fixme 当角色跳跃并下落时，只会下落到y轴为0.13就停止
    /// </summary>
    private void CheckGround()
    {
        if (CheckGroundPoint == null)
        {
            Debug.LogError($"PlayerMoveMent:97, CheckGroundPoint Is Null");
            return;
        }
        isGround = Physics.CheckSphere(CheckGroundPoint.position, CheckGroundRadius, GroundLayerMask);
        if (isGround) Debug.Log("On The Ground");
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

    private void AdjustVerticalVelocity()
    {
        if (isGround && VerticalVelocity <= 0)
        {
            VerticalVelocity = 0;
        }
        else if (!isGround)
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(CheckGroundPoint.position, CheckGroundRadius);
    }
}
