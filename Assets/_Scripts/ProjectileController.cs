using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D _rigidBody;

    [Header("GameObjects")]

    [Header("Statistics")]
    [SerializeField] private float movementSpeed = 2f;
    private Vector2 direction = Vector2.zero;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Debug.Log($"Direction {direction}");
        _rigidBody.velocity = direction.normalized * movementSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //TODO: Move player projectile damage variable to player controller
        if (collision.CompareTag("Player") && !transform.parent.CompareTag("Player"))
        {
            transform.parent.GetComponent<EnemyController>().DealPlayerDamage();
        }
        else if (collision.CompareTag("Enemy") && !transform.parent.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyController>().ReceiveDamage(2);
        }

        if (collision.CompareTag("Ground") || (collision.CompareTag("Enemy") && !transform.parent.CompareTag("Enemy")) || (collision.CompareTag("Player") && !transform.parent.CompareTag("Player")) )
            Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }


    public void ShootProjectile(Vector2 newDirection)
    {
        direction = newDirection;
        transform.right = newDirection;
    }
}
