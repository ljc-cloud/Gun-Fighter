using UnityEngine;

/// <summary>
/// Ñª°ü
/// TODO ×Ô×ª
/// </summary>
public class HealthPickUp : MonoBehaviour
{
    public float HealthValue;


    private void Update()
    {
        transform.Rotate(Vector3.up * 60 * Time.deltaTime);
    }
}
