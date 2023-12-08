using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public Transform EyeViewTransform;
    [Header("Mouse Rotation Speed")]
    public float RotateSpeed = 200f;
    [Header("Mouse Rotation Ratio")]
    [Range(1,2)]
    public float RotateRatio = 1f;
    [Header("Mouse Y Rotation Limit")]
    public float MaxYRotation = 80f;
    public float MinYRotation = -80f;

    private float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CameraFollowMouse();
    }

    /// <summary>
    /// 鼠标水平控制玩家，上下控制EyeView
    /// </summary>
    private void CameraFollowMouse()
    {
        if (EyeViewTransform == null)
        {
            Debug.LogError($"MouseLook:27, EyeViewTransform is null");
            return;
        }
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");

        // x轴旋转玩家
        var xRotation = Vector3.up * mouseX * RotateSpeed * RotateRatio * Time.deltaTime;
        transform.Rotate(xRotation);

        yRotation -= mouseY * RotateSpeed * RotateRatio * Time.deltaTime;
        yRotation = Mathf.Clamp(yRotation, MinYRotation, MaxYRotation);
        // y轴旋转EyeView
        EyeViewTransform.localRotation = Quaternion.Euler(new Vector3(yRotation, EyeViewTransform.localRotation.y, EyeViewTransform.localRotation.z));
    }
}
