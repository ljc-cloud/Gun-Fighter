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
                    Destroy(gameObject);
                }
                break;
            case "Enemy":
                if (BulletState == BulletState.PLAYER_BULLET)
                {
                    Debug.Log("»÷ÖÐÄ¿±ê");
                    collision.gameObject.GetComponent<Health>().TakeDamage(damage);
                    collision.gameObject.GetComponent<Enemy_HoverBotAnimatorController>().TriggerOnDamaged();
                    collision.gameObject.GetComponent<Enemy_HoverBotController>().HasAttacked = true;
                    Destroy(gameObject);
                }
                break;
            default:
                break;
        }
        //Destroy(gameObject);
    }
}
