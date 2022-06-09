using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D _rigidBody;
    private Animator _animator;

    [Header("GameObjects")]
    private CinemachineVirtualCamera cinemachine;
    private PlayerController player;
    private Rigidbody2D player_rb;
    [SerializeField] private LayerMask playerMask;
    private GameManager gameManager;

    [Header("Statistics")]
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private float health = 9;
    [SerializeField] private int damage = 2;
    [SerializeField] private float awakeDistance = 6f;
    [SerializeField] private Vector2 enemyHead = new(0, 0.75f);
    [SerializeField] private Vector2 headSize = new(1.5f, 0.5f);

    [Header("Conditionals")]
    [SerializeField] private bool isAwake;
    [SerializeField] private string type;
    private enum AttackList // your custom enumeration
    {
        Close,
        Range,
        Collision
    };
    [SerializeField] private AttackList attackType = AttackList.Close;
    [SerializeField] private bool headKill;
    [SerializeField] private bool isOnHead;
    [SerializeField] private bool isDead;
    [SerializeField] private bool receiveDamage;

    private void Awake()
    {
        cinemachine = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player_rb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        Vector2 direction = player.transform.position - transform.position;
        float distance = Vector2.Distance(transform.position, player.transform.position);

        if(isDead)
        {
            if(type == "bird")
            {
                _rigidBody.gravityScale = 2;
                _rigidBody.velocity = Vector2.down *3;
            }
        }
        else
        {
            if (distance < awakeDistance && !receiveDamage)
            {
                if (type == "bird")
                    _rigidBody.velocity = direction.normalized * movementSpeed;
                else
                    _rigidBody.velocity = new Vector2(direction.x, _rigidBody.velocity.y) * movementSpeed;
            }
            else
            {
                _rigidBody.velocity = Vector2.zero;
            }
            ChangeDirection(direction.x);
        }
    }

    private void ChangeDirection(float directionX)
    {
        if (directionX >= 0 && transform.eulerAngles.y == 180) //Look right
        {
            transform.eulerAngles = Vector3.up * 0;
        }
        else if (directionX <= 0 && transform.eulerAngles.y == 0) //Look left
        {
            transform.eulerAngles = Vector3.up * 180;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(headKill)
                isOnHead = Physics2D.OverlapBox((Vector2)transform.position + enemyHead, headSize, 0f, playerMask);

            if (isOnHead && headKill)
                Destroy(gameObject);
            else if(attackType == AttackList.Collision)
            {
                DealPlayerDamage();
            }
        }

        if (collision.gameObject.CompareTag("Ground") && isDead)
        {
            if(type == "bird")
            {
                _rigidBody.gravityScale = 0;
                gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
                Invoke("DestroyEnemy", 2f);
                this.enabled = false;
            }
        }
        
    }
    private void DealPlayerDamage()
    {
        _rigidBody.AddForce((transform.position - player.transform.position).normalized * 7000, ForceMode2D.Force);

        int playerForce = 500;
        if (player.isOnGround)
            playerForce = 2000;
        player_rb.AddForce((player.transform.position - transform.position).normalized * playerForce, ForceMode2D.Force);

        gameManager.ReceiveDamage(damage);
    }


    public void ReceiveDamage(float damage)
    {
        health -= damage;

        _rigidBody.AddForce((transform.position - player.transform.position).normalized * 7000, ForceMode2D.Force);
        if (health <= 0)
        {
            isDead = true;
            _animator.SetBool("Die", true);
        } 
        else
        {
            Debug.Log("Receive Damaga");
            receiveDamage = true;
            _animator.SetTrigger("ReceiveDamage");
            Invoke("StopReceiveDamage", 0.16f);
        }
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void StopReceiveDamage()
    {
        receiveDamage = false;
    }

    private void OnDrawGizmosSelected()
    {
        if(headKill)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube((Vector2)transform.position + enemyHead, headSize);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, awakeDistance);
    }

}
