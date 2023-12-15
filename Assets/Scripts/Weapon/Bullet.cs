using UnityEngine;

public enum BulletState { ENEMY_BULLET, PLAYER_BULLET }

public class Bullet : MonoBehaviour
{
    public float Damage;
    public BulletState BulletState;
    public GameObject BulletExplosion;

    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.transform.tag)
        {
            case "Player":
                if (BulletState == BulletState.ENEMY_BULLET)
                {
                    collision.gameObject.GetComponent<Health>().TakeDamage(Damage);
                }
                break;
            case "Enemy":
                if (BulletState == BulletState.PLAYER_BULLET)
                {
                    collision.gameObject.GetComponent<Health>().TakeDamage(Damage);
                    collision.gameObject.GetComponent<Enemy_HoverBotAnimatorController>().TriggerOnDamaged();
                    Enemy_HoverBotController enemy_HoverBotController = collision.gameObject.GetComponent<Enemy_HoverBotController>();
                    enemy_HoverBotController.HasAttacked = true;
                    enemy_HoverBotController.ExitAlertTimer = 0;
                }
                else
                {

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
