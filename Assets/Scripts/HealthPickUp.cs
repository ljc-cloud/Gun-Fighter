using UnityEngine;

/// <summary>
/// Ѫ��
/// TODO ��ת
/// </summary>
public class HealthPickUp : MonoBehaviour
{
    public float HealthValue;


    private void Update()
    {
        transform.Rotate(Vector3.up * 60 * Time.deltaTime);
    }
}
