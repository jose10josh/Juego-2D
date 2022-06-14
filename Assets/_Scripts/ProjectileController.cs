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

    [Header("Statistics")]
    [SerializeField] private float movementSpeed = 2f;
    private Vector2 direction;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        player_rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
        Debug.Log("Awake");

        direction = player_rb.transform.position - transform.position;
    }

    private void Update()
    {
        _rigidBody.velocity = new Vector2(direction.normalized.x * movementSpeed, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            transform.parent.GetComponent<EnemyController>().DealPlayerDamage();
            Destroy(gameObject);
        }
    }

}
