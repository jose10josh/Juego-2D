using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D _rigidBody;

    [Header("GameObjects")]
    private GameManager gamemanager;

    [Header("Statistics")]
    [SerializeField] private float movementSpeed = 2f;
    private Vector2 direction = Vector2.zero;
    private string isParent;
    private float damage;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        gamemanager = FindObjectOfType<GameManager>();
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
            gamemanager.ReceiveDamage(damage);
        }
        else if (collision.CompareTag("Enemy") && isParent != "Enemy")
        {
            collision.GetComponent<EnemyController>().ReceiveDamage(damage);
        }

        if (collision.CompareTag("Ground") || (collision.CompareTag("Enemy") && isParent != "Enemy") || (collision.CompareTag("Player") && isParent != "Player") )
            Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }


    public void ShootProjectile(Vector2 newDirection, string parent, float dmg)
    {
        damage = dmg;
        isParent = parent;
        direction = newDirection;
        transform.right = newDirection;
    }
}
