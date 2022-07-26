using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private enum WeaponType
    {
        Empty,
        Sword,
        Bow
    }

    [Header("Components")]
    private Rigidbody2D _rigidBody;
    private Animator _animator;
    private CapsuleCollider2D _capsuleCollider;
    [SerializeField] private GameObject _projectile;

    [Header("GameObjects")]
    [SerializeField] private LayerMask floorLayer;
    private GameManager gameManager;

    [Header("Statistics")]
    private readonly float movementVelocity = 8.5f;
    private readonly float jumpForce = 7;
    private readonly float rollForce = 1.5f;
    private Vector2 playerDirection = Vector2.right;
    private WeaponType _weaponType = WeaponType.Empty;
    private float arrowDamage = 2;
    

    [Header("Conditionals")]
    private bool canMove = true;
    private int jumpCount = 0;
    private readonly int jumpLimit = 2;
    public bool isOnGround = true;
    public bool isRolling = false;
    private bool isAttacking = false;
    [SerializeField]
    private bool isOnWall = false;
    private bool isClimbing = false;
    

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
        gameManager = FindObjectOfType<GameManager>();
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
        StartClimbing(rawVerticalInput, verticalInput);

    }

    private void StartClimbing(float rawVerticalInput, float verticalInput)
    {
        if (isOnWall && Input.GetKey(KeyCode.LeftShift))
        {
            _rigidBody.gravityScale = 0;
            _animator.SetBool("Climb", true);
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 0);
            jumpCount = 0;
            isClimbing = true;
            if (rawVerticalInput == 0)
                _animator.SetFloat("Velocity", 0);
            else if (rawVerticalInput != 0)
            {
                _animator.SetFloat("Velocity", 1);

                float climbVelocityModifier = rawVerticalInput > 0 ? 0.5f : 1;
                _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, verticalInput * (climbVelocityModifier * movementVelocity));
            }
        }
        else
        {
            isClimbing = false;
            _rigidBody.gravityScale = 1;
            _animator.SetBool("Climb", false);
        }
    }

    private void StartRolling()
    {
        if (Input.GetKeyDown(KeyCode.X) && !isRolling && !isClimbing)
        {
            isRolling = true;
            _animator.SetBool("Roll", true);
            _animator.SetBool("Jump", false);
            Camera.main.GetComponent<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
            StartCoroutine(gameManager.ShakeCamera(8, 0.3f));
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
        if(isClimbing)
        {
            _rigidBody.velocity = Vector2.up * jumpForce;
        }
        else
        {
            _rigidBody.velocity = Vector2.up * jumpForce;
        }
        //_rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 0);
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

    private void Attack(Vector2 direction)
    {
        if (Input.GetButtonDown("Fire1") && !isRolling && !isAttacking && !isClimbing && (_weaponType == WeaponType.Bow || _weaponType == WeaponType.Sword))
        {
            isAttacking = true;
            _animator.SetBool("Attack", true);

            if (_weaponType == WeaponType.Bow)
            {
                EnableRangeAttack(direction);
            }
        }
    }

    /// <summary>
    /// Instantiate ranger projectile in screen and set projectile direction
    /// </summary>
    private void EnableRangeAttack(Vector2 direction)
    {
        var newArrow = Instantiate(_projectile, gameObject.transform.position, _projectile.transform.rotation);
        newArrow.GetComponent<ProjectileController>().ShootProjectile(direction, "Player", arrowDamage);
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
        RaycastHit2D raycastHitLeft = Physics2D.BoxCast(_capsuleCollider.bounds.center, new Vector2(_capsuleCollider.bounds.extents.x + extraWidth, _capsuleCollider.bounds.extents.y), 0f, Vector2.left, _capsuleCollider.bounds.extents.x + extraWidth, floorLayer);
        RaycastHit2D raycastHitRight = Physics2D.BoxCast(_capsuleCollider.bounds.center, new Vector2(_capsuleCollider.bounds.extents.x + extraWidth, _capsuleCollider.bounds.extents.y), 0f, Vector2.right, _capsuleCollider.bounds.extents.x + extraWidth, floorLayer);

        Color rayColor = Color.red;
        if (raycastHitLeft.collider != null || raycastHitRight.collider != null)
            rayColor = Color.blue;
    
        //Debug.DrawRay(_capsuleCollider.bounds.center, Vector2.left * (_capsuleCollider.bounds.extents.x + extraWidth), rayColor);
        //Debug.DrawRay(_capsuleCollider.bounds.center, Vector2.right * (_capsuleCollider.bounds.extents.x + extraWidth), rayColor);
        isOnWall = raycastHitLeft.collider != null || raycastHitRight.collider != null;
    }

    public void ChangePlayerWeapon(int weapon)
    {
        int layerIndex;

        _animator.SetLayerWeight((int)_weaponType, 0);
        switch (weapon)
        {
            case 2:
                layerIndex = _animator.GetLayerIndex("BowLayer");
                _weaponType = WeaponType.Bow;
                break;
            case 1:
                layerIndex = _animator.GetLayerIndex("SwordLayer");
                _weaponType = WeaponType.Sword;
                break;
            default:
                layerIndex = _animator.GetLayerIndex("NoWeaponLayer");
                _weaponType = WeaponType.Empty;
                break;
        }
        _animator.SetLayerWeight(layerIndex, 1);
    }
}
