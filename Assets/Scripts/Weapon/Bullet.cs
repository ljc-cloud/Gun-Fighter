using UnityEngine;

public enum BulletState { ENEMY_BULLET, PLAYER_BULLET }

public class Bullet : MonoBehaviour
{
    public float damage;
    public BulletState BulletState;


    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.transform.tag)
        {
            case "Player":
                if (BulletState == BulletState.ENEMY_BULLET)
                {
                    collision.gameObject.GetComponent<Health>().TakeDamage(damage);
                }
                break;
            case "Enemy":
                if (BulletState == BulletState.PLAYER_BULLET)
                {
                    collision.gameObject.GetComponent<Health>().TakeDamage(damage);
                }
                break;
            default:
                break;
        }
    }
}
