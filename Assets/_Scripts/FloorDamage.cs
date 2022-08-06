using UnityEngine;

public class FloorDamage : MonoBehaviour
{
    private float damage = 3;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameManager.ReceiveDamage(damage);
        }
        else if (collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }
    }

}
