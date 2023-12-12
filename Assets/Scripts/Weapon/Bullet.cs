using UnityEngine;

public enum BulletState { ENEMY_BULLET, PLAYER_BULLET }

public class Bullet : MonoBehaviour
{
    public float damage;
    public BulletState BulletState;
    public GameObject BulletExplosion;

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
                    collision.gameObject.GetComponent<Enemy_HoverBotAnimatorController>().TriggerOnDamaged();
                    collision.gameObject.GetComponent<Enemy_HoverBotController>().HasAttacked = true;
                }
                else
                {
                    Debug.Log("Debug");
                }
                break;
            default:
                break;
        }
        CreateBulletExplosion();
        Destroy(gameObject);
    }

    private void CreateBulletExplosion()
    {
        if (BulletExplosion)
        {
            GameObject exp = Instantiate(BulletExplosion, transform.position, transform.rotation);
            Destroy(exp, 1.3f);
        }
    }
}
