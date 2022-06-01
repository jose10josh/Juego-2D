using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D _rigidBody;
    private Animator _animator;
    private CapsuleCollider2D _capsuleCollider;

    [Header("GameObjects")]
    private CinemachineVirtualCamera _cinemachine;
    [SerializeField] private LayerMask floorLayer;

    [Header("Player Statistics")]
    private readonly float movementVelocity = 10;
    private readonly float jumpForce = 8;
    readonly float rollForce = 1.5f;
    private Vector2 playerDirection = Vector2.right;

    [Header("Conditionals")]
    private bool canMove = true;
    private int jumpCount = 0;
    private readonly int jumpLimit = 2;
    public bool isOnGround = true;
    public bool isRolling = false;
    private bool isAttacking = false;
    [SerializeField]
    private bool isOnWall = false;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        _cinemachine = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float rawHorizontalInput = Input.GetAxisRaw("Horizontal");
        float rawVerticalInput = Input.GetAxisRaw("Vertical");

        Vector2 direction = new(horizontalInput, verticalInput);
        Vector2 directionRaw = new(rawHorizontalInput, rawVerticalInput);

        Walk(direction);
        StartRolling();

        FixJump();
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < jumpLimit)
        {
            _animator.SetBool("Jump", true);
            StopRoll();
            jumpCount++;
            Jump();
        }

        Attack(AttackDirection(playerDirection, directionRaw));

        IsOnWall();


        if(isOnWall && Input.GetKey(KeyCode.LeftShift))
        {
            _rigidBody.gravityScale = 0;
            _animator.SetBool("Climb", true);

            Debug.Log($"On Wall {_rigidBody.gravityScale}");


            if(_rigidBody.velocity == Vector2.zero)
                _animator.SetFloat("Velocity", 0);
            else if(rawVerticalInput != 0)
            {
                _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 0);
                _animator.SetFloat("Velocity", 1);

                float climbVelocityModifier = rawVerticalInput > 0 ? 0.5f : 1;
                _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, verticalInput * (climbVelocityModifier * movementVelocity));
            }
        }
        else if(!isOnWall)
        {
            _rigidBody.gravityScale = 1;
            _animator.SetBool("Climb", false);
        }
    }

    private void StartRolling()
    {
        if (Input.GetKeyDown(KeyCode.X) && !isRolling)
        {
            isRolling = true;
            _animator.SetBool("Roll", true);
            _animator.SetBool("Jump", false);
            Camera.main.GetComponent<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
            StartCoroutine(ShakeCamera());
        }
        if(isRolling)
        {
            float force = rollForce;
            if (transform.eulerAngles.y == 180)
                force = -rollForce;

            MovePlayer(new Vector2(force * movementVelocity, _rigidBody.velocity.y));
        }
    }

    private void StopRoll()
    {
        isRolling = false;
        _rigidBody.velocity = new Vector2(0, _rigidBody.velocity.y);
        _animator.SetBool("Roll", false);
    }

    public void LandOnGround()
    {
        _animator.SetBool("Jump", false);
        jumpCount = 0;
    }

    private void FixJump()
    {
        if (!isOnGround)
        {
            float velocity = -1;
            if (_rigidBody.velocity.y > 0)
                velocity = 1;

            _animator.SetFloat("VerticalVelocity", velocity);
        }


        if (_rigidBody.velocity.y < 0)
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
        //_rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 0);
        _rigidBody.velocity = Vector2.up * jumpForce;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction"></param>
    private void Walk(Vector2 direction)
    {
        if(canMove && !isRolling)
        {
            MovePlayer(new Vector2(direction.x * movementVelocity, _rigidBody.velocity.y));

            if (direction.x != 0)
            {
                if (isOnGround)
                    _animator.SetBool("Walk", true);

                if (direction.x > 0 && transform.eulerAngles.y == 180) //Look right
                {
                    transform.eulerAngles = Vector3.up * 0;
                    playerDirection = Vector2.right;
                }
                else if (direction.x < 0 && transform.eulerAngles.y == 0) //Look left
                {
                    playerDirection = Vector2.left;
                    transform.eulerAngles = Vector3.up * 180;
                }
            }
            else
            {
                if (isOnGround)
                    _animator.SetBool("Walk", false);
            }
        }
    }

    private void MovePlayer(Vector2 direction)
    {
        _rigidBody.velocity = new Vector2(direction.x, direction.y);
    }

    private IEnumerator ShakeCamera ( )
    {
        CinemachineBasicMultiChannelPerlin shake = _cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        shake.m_AmplitudeGain = 10;

        yield return new WaitForSeconds(0.2f);

        shake.m_AmplitudeGain = 0;
    }

    private void Attack(Vector2 direction)
    {
        if (Input.GetButtonDown("Fire1") && !isRolling && !isAttacking)
        {
            isAttacking = true;
            //_animator.SetFloat("AttackX", direction.x);
            //_animator.SetFloat("AttackY", direction.y);

            int attackDirection = 0;
            if(Input.GetKey(KeyCode.C))
                attackDirection = 1;
            else if (!isOnGround)
                attackDirection = 2;


            _animator.SetFloat("SwordAttack", attackDirection);
            _animator.SetBool("Attack", true);

        }
    }

    private Vector2 AttackDirection(Vector2 moveDir, Vector2 rawDir)
    {
        if (_rigidBody.velocity.x == 0 && rawDir.y != 0)
            return new Vector2(0, rawDir.y);

        return new Vector2(moveDir.x, rawDir.y);
    }
    private void StopAttack()
    {
        isAttacking = false;
        _animator.SetBool("Attack", false);
    }

    /// <summary>
    /// Verify is on ground alternative
    /// Validate player is on ground using a Box cast
    /// </ summary >
    private void IsOnWall()
    {
        float extraWidth = 0.05f;
        RaycastHit2D raycastHitLeft = Physics2D.Raycast(_capsuleCollider.bounds.center, Vector2.left, _capsuleCollider.bounds.extents.x + extraWidth, floorLayer);
        RaycastHit2D raycastHitRight = Physics2D.Raycast(_capsuleCollider.bounds.center, Vector2.right, _capsuleCollider.bounds.extents.x + extraWidth, floorLayer);
        //RaycastHit2D raycastHit = Physics2D.BoxCast(_capsuleCollider.bounds.center, _capsuleCollider.bounds.size, 0f, Vector2.left, extraWidth, floorLayer);
    
        Color rayColor = Color.red;
        if (raycastHitLeft.collider != null || raycastHitRight.collider != null)
            rayColor = Color.blue;
    
        Debug.DrawRay(_capsuleCollider.bounds.center, Vector2.left * (_capsuleCollider.bounds.extents.x + extraWidth), rayColor);
        Debug.DrawRay(_capsuleCollider.bounds.center, Vector2.right * (_capsuleCollider.bounds.extents.x + extraWidth), rayColor);
        //Debug.DrawRay(_boxCollider.bounds.center - new Vector3(_boxCollider.bounds.extents.x, 0), Vector2.down * (_boxCollider.bounds.extents.y + extraHeight), rayColor);
        //Debug.DrawRay(_boxCollider.bounds.center - new Vector3(_boxCollider.bounds.extents.x, _boxCollider.bounds.extents.y + extraHeight), Vector2.right * (_boxCollider.bounds.size.x), rayColor);
        isOnWall = raycastHitLeft.collider != null || raycastHitRight.collider != null;
    }

    #region Old functions
    //[Header("Collisions")]
    //private Vector2 playerFeet = new(0, -0.65f);
    //private readonly float radioCollider = 0.2f;
    //[SerializeField]
    //private LayerMask floorLayer;

    //Course functions to check on ground
    //private void VerifyIsInGround ()
    //{
    //    bool inGround = Physics2D.OverlapCircle((Vector2)transform.position + playerFeet, radioCollider, floorLayer);
    //    Debug.Log($"Is en ground: {inGround}");
    //    if (inGround)
    //    {
    //        jumpCount = 1;
    //    }
    //}


    #endregion
}
