using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FlyController : MonoBehaviour
{
    [Header("Components")]
    private SpriteRenderer _spriteRendere;
    private Rigidbody2D _rigidBody;
    private CapsuleCollider2D _collider;

    [Header("GameObjects")]
    private CinemachineVirtualCamera cinemachine;
    private PlayerController player;
    [SerializeField] private LayerMask playerMask;

    [Header("Statistics")]
    private float flySpeed = 4f;
    private float life = 9;
    private int damage = 2;
    [SerializeField] private Vector2 enemyHead = new(0, 0.75f);
    private float awakeDistance = 10f;

    [Header("Conditionals")]
    private bool addForce;
    [SerializeField] private bool isOnHead;
    private bool pushBack;


    private void Awake()
    {
        cinemachine = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        _spriteRendere = GetComponent<SpriteRenderer>();
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = player.transform.position - transform.position;   
        float distance = Vector2.Distance(transform.position, direction);

        if(distance < awakeDistance)
        {
            //Debug.Log($"Is on range {direction.normalized}");
            _rigidBody.velocity = direction.normalized * flySpeed;
        }
        else
        {
            //Debug.Log($"Not is on range");
            _rigidBody.velocity = Vector2.zero;
        }

        isOnHead = Physics2D.OverlapBox((Vector2)transform.position + enemyHead, new Vector2(2, 0.5f), 0f, playerMask);

        ChangeDirection(direction.x);

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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, awakeDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawCube((Vector2)transform.position + enemyHead, new Vector2(2, 0.5f));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Damage Player");
            if (isOnHead)
            {
                Destroy(gameObject);
            }
            else
            {
                pushBack = true;
                //player.ApplyDamage((transform.position - player.transform.position).normalized);
            }

        }
    }

    private void FixedUpdate()
    {
        if(pushBack)
        {
            Debug.Log("Push back", player);
            _rigidBody.AddForce((transform.position - player.transform.position).normalized * 100);
            player.GetComponent<Rigidbody2D>().AddForce((player.transform.position - transform.position).normalized * 70);
            pushBack = false;
        }
    }

    public void ReceiveDamage(float damage)
    {
        if(life > 0)
        {
            life -= damage;
        }

        if(life < 0)
        {
            Destroy(gameObject);
        }

    }
}
