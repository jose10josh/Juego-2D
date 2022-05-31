using System.Collections;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D _rigidBody;
    private Animator _animator;

    [Header("GameObjects")]
    private CinemachineVirtualCamera _cinemachine;

    [Header("Player Statistics")]
    private readonly float movementVelocity = 10;
    private readonly float jumpForce = 8;
    readonly float rollForce = 1.5f;

    [Header("Conditionals")]
    private bool canMove = true;
    private int jumpCount = 0;
    private readonly int jumpLimit = 2;
    public bool isOnGround = true;
    public bool isRolling = false;
    private bool isShaking;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _cinemachine = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 direction = new(horizontalInput, verticalInput);

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
                }
                else if (direction.x < 0 && transform.eulerAngles.y == 0) //Look left
                {
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
        isShaking = true;
        CinemachineBasicMultiChannelPerlin shake = _cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        shake.m_AmplitudeGain = 10;

        yield return new WaitForSeconds(0.2f);

        shake.m_AmplitudeGain = 0;
        isShaking = false;
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

    /// <summary>
    /// Verify is on ground alternative
    /// Validate player is on ground using a Box cast
    /// </summary>
    //private void IsOnGround()
    //{
    //float extraHeight = 0.05f;
    ////RaycastHit2D raycastHit = Physics2D.Raycast(_boxCollider.bounds.center, Vector2.down, _boxCollider.bounds.extents.y + extraHeight, floorLayer);
    //RaycastHit2D raycastHit = Physics2D.BoxCast(_boxCollider.bounds.center, _boxCollider.bounds.size, 0f, Vector2.down, extraHeight, floorLayer);

    //Color rayColor = Color.red;
    //if (raycastHit.collider != null)
    //    rayColor = Color.blue;

    //Debug.DrawRay(_boxCollider.bounds.center + new Vector3(_boxCollider.bounds.extents.x, 0), Vector2.down * (_boxCollider.bounds.extents.y + extraHeight), rayColor);
    //Debug.DrawRay(_boxCollider.bounds.center - new Vector3(_boxCollider.bounds.extents.x, 0), Vector2.down * (_boxCollider.bounds.extents.y + extraHeight), rayColor);
    //Debug.DrawRay(_boxCollider.bounds.center - new Vector3(_boxCollider.bounds.extents.x, _boxCollider.bounds.extents.y + extraHeight), Vector2.right * (_boxCollider.bounds.size.x), rayColor);
    //isOnGround = raycastHit.collider != null;
    //}
    #endregion
}
