using UnityEngine;

public class AttackController : MonoBehaviour
{
    [SerializeField] private float swordDamage = 5;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyController>().ReceiveDamage(swordDamage);
        }

        if (collision.CompareTag("Player"))
        {
            transform.parent.GetComponent<EnemyController>().DealPlayerDamage();
        }
    }


    public void ChangeSwordDamage(float extraDamage)
    {
        swordDamage *= extraDamage;
    }
}
