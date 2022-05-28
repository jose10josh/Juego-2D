using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rigidBody;

    [Header("Player Statistics")]
    [SerializeField]
    private float movementVelocity = 10;
    [SerializeField]
    private float jumpForce = 7;

    [Header("Conditionals")]
    private bool canMove = true;
    [SerializeField]
    private int jumpCount = 0;
    private bool isInGround = true;

    [Header("Collisions")]
    private Vector2 playerFeet = new(0, -0.9f);
    private readonly float radioCollider = 0.2f;
    [SerializeField]
    private LayerMask floorLayer;


    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 direction = new(horizontalInput, verticalInput);
        Walk(direction);

        VerifyIsInGround();
        FixJump();
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < 2)
        {
            isInGround = false;
            jumpCount++;
            Jump();
        }
    }

    private void FixJump()
    {
        if(_rigidBody.velocity.y < 0)
        {
            _rigidBody.velocity += (2.5f - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }
        else if(_rigidBody.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            _rigidBody.velocity += (2f - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        }
    }
    private void Jump()
    {
        _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 0);
        _rigidBody.velocity += Vector2.up * jumpForce;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction"></param>
    private void Walk(Vector2 direction)
    {
        if(canMove)
        {
            _rigidBody.velocity = new Vector2(direction.x * movementVelocity, _rigidBody.velocity.y);

            if (direction.x != 0)
            {
                if (direction.x > 0 && transform.eulerAngles.y == 180) //Look right
                {
                    transform.eulerAngles = Vector3.up * 0;
                }
                else if (direction.x < 0 && transform.eulerAngles.y == 0) //Look left
                {
                    transform.eulerAngles = Vector3.up * 180;
                }
            }
        }
    }

    private void VerifyIsInGround ()
    {
        bool inGround = Physics2D.OverlapCircle((Vector2)transform.position + playerFeet, radioCollider, floorLayer);
        Debug.Log($"Is en ground: {inGround}");
        if (inGround && isInGround)
            jumpCount = 0;
    }
}
