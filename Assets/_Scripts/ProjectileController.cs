using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D _rigidBody;

    [Header("GameObjects")]
    private Rigidbody2D player_rb;
    private GameManager gameManager;
    private EnemyController gameParent;

    [Header("Statistics")]
    [SerializeField] private float movementSpeed = 2f;
    private Vector2 direction;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        player_rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
        gameParent = transform.parent.GetComponent<EnemyController>();
        direction = player_rb.transform.position - transform.position;

    }
    private void Start()
    {
        transform.right = direction;
    }

    private void Update()
    {
        _rigidBody.velocity = direction.normalized * movementSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            gameParent.DealPlayerDamage();
        }

        var isParent = gameParent == collision.GetComponent<EnemyController>();
        if(collision.CompareTag("Ground") || (collision.CompareTag("Enemy") && !isParent) || collision.CompareTag("Player"))
            Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

}
