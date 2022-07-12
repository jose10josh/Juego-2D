using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D _rigidBody;

    [Header("GameObjects")]

    [Header("Statistics")]
    [SerializeField] private float movementSpeed = 2f;
    private Vector2 direction = Vector2.zero;
    private string isParent;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _rigidBody.velocity = direction.normalized * movementSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //TODO: Move player projectile damage variable to player controller
        if (collision.CompareTag("Player") && isParent != "Player")
        {
            transform.parent.GetComponent<EnemyController>().DealPlayerDamage();
        }
        else if (collision.CompareTag("Enemy") && isParent != "Enemy")
        {
            collision.GetComponent<EnemyController>().ReceiveDamage(2);
        }

        if (collision.CompareTag("Ground") || (collision.CompareTag("Enemy") && isParent != "Enemy") || (collision.CompareTag("Player") && isParent != "Player") )
            Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }


    public void ShootProjectile(Vector2 newDirection, string parent)
    {
        isParent = parent;
        direction = newDirection;
        transform.right = newDirection;
    }
}
