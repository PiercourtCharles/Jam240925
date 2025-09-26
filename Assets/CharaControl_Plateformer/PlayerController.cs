using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

public class PlayerController : MonoBehaviour
{
    public bool IsDead = false;
    public bool CanGrapple = false;
    public bool GrabBonus = false;
    public bool BlockBonus = false;
    public float LaunchForce = 1;
    public GameObject GrabDisplay;
    [SerializeField] Grapple _grapple;
    [SerializeField] bool _isGrab = false;
    public Block Block;
    public Transform BlockInitPos;

    [SerializeField] Vector2 _inputs;
    [SerializeField] bool _inputJump;
    [SerializeField] Rigidbody2D _rb;

    [Header("Movements")]
    [SerializeField] float _walkSpeed;
    [SerializeField] float _acceleration;

    [Header("Groundcheck")]
    [SerializeField] float _groundOffset;
    [SerializeField] float _groundRadius;
    [SerializeField] LayerMask _GroundLayer;

    [Header("Jump")]
    [SerializeField] float _timerMinBetweenJump;
    [SerializeField] float _jumpForce;
    [SerializeField] float _velocityFallMin;
    [SerializeField][Tooltip("Gravity when the player goes up and press jump")] float _gravityUpJump;
    [Tooltip("Gravity otherwise")] public float Gravity;
    [SerializeField] float _jumpInputTimer = 0.1f;

    [Header("")]
    [SerializeField] bool _isGrounded;
    [SerializeField] float _timerNoJump;
    [SerializeField] float _timerSinceJumpPressed;
    [SerializeField] float _TimeSinceGrounded;
    [Header("")]
    [SerializeField] float _coyoteTime;
    [SerializeField] float _slopeDetectOffset;
    [SerializeField] bool _isOnSlope;
    [Header("")]
    [SerializeField] Collider2D _collider;
    [SerializeField] PhysicsMaterial2D _physicsFriction;
    [SerializeField] PhysicsMaterial2D _physicsNoFriction;

    Collider2D[] _collidersGround = new Collider2D[5];
    Vector3 _offsetCollisionBox;
    Vector3 _offsetToReplace;
    Vector2 _collisionBox;

    RaycastHit2D[] _hitResults = new RaycastHit2D[2];
    float[] directions = new float[] { 1, -1 };

    #region Inputs
    Plateform _playerInputs;

    private void Awake()
    {
        _playerInputs = new Plateform();
        Block.gameObject.SetActive(false);
        GrabDisplay.SetActive(false);
    }

    private void OnEnable()
    {
        _playerInputs.Enable();
        _playerInputs.Player.Move.performed += MoveInput;
        _playerInputs.Player.Jump.started += JumpInput;
        _playerInputs.Player.Jump.canceled += JumpInputCanceled;
        _playerInputs.Player.Interact.started += InteractInput;
    }

    private void OnDisable()
    {
        _playerInputs.Disable();
        _playerInputs.Player.Move.performed -= MoveInput;
        _playerInputs.Player.Jump.started -= JumpInput;
        _playerInputs.Player.Jump.canceled -= JumpInputCanceled;
        _playerInputs.Player.Interact.started -= InteractInput;
    }

    void InteractInput(InputAction.CallbackContext context)
    {
        if ((GrabBonus && _grapple.Target != null) || _isGrab)
        {
            _isGrab = !_isGrab;
            if (!_grapple.Grab(_isGrab))
                _isGrab = !_isGrab;
            else
            {
                GrabBonus = false;
                GrabDisplay.SetActive(false);
                _rb.AddForce(Vector2.right * _inputs.x * LaunchForce, ForceMode2D.Impulse);
            }
        }
        else if (BlockBonus)
        {
            if (Block.gameObject.activeSelf)
            {
                Block.Drop(false);
                BlockBonus = false;
            }
        }
    }

    void JumpInput(InputAction.CallbackContext context)
    {
        if (IsDead)
            return;

        _inputJump = true;
        _timerSinceJumpPressed = 0;
    }

    void JumpInputCanceled(InputAction.CallbackContext context)
    {
        _inputJump = false;
    }

    void MoveInput(InputAction.CallbackContext context)
    {
        if (IsDead)
        {
            _inputs = Vector2.zero;
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        _inputs = new Vector2(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y);
    }
    #endregion Inputs

    private void FixedUpdate()
    {
        HandleMovements();
        HandleGrounded();
        HandleJump();
        HandleSlope();
        HandleCorners();
    }

    void HandleMovements()
    {
        var velocity = _rb.linearVelocity;
        Vector2 wantedVelocity = new Vector2(_inputs.x * _walkSpeed, velocity.y);
        _rb.linearVelocity = Vector2.MoveTowards(velocity, wantedVelocity, _acceleration * Time.deltaTime);
    }

    void HandleGrounded()
    {
        Vector2 point = transform.position + Vector3.up * _groundOffset;
        bool currentGrounded = Physics2D.OverlapCircleNonAlloc(point, _groundRadius, _collidersGround, _GroundLayer) > 0;

        _TimeSinceGrounded += Time.deltaTime;

        if (currentGrounded == false && _isGrounded)
        {
            _TimeSinceGrounded = 0;
        }

        _isGrounded = currentGrounded;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * _groundOffset, _groundRadius);
        Gizmos.color = Color.white;
    }

    void HandleJump()
    {
        _timerNoJump -= Time.deltaTime;
        _timerSinceJumpPressed += Time.deltaTime;

        //Limite vitesse chute
        if (_rb.linearVelocity.y < _velocityFallMin)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _velocityFallMin);
        }

        if (_isGrounded == false)
        {
            if (_rb.linearVelocity.y < 0)
            {
                _rb.gravityScale = Gravity;
            }
            else
            {
                _rb.gravityScale = _inputJump ? _gravityUpJump : Gravity;
            }
        }
        else
        {
            _rb.gravityScale = Gravity;
        }

        if (_inputJump && (_rb.linearVelocity.y <= 0 /*|| _isOnSlope*/) && (_isGrounded || _TimeSinceGrounded < _coyoteTime) && _timerNoJump <= 0 && _timerSinceJumpPressed < _jumpInputTimer)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
            _timerNoJump = _timerMinBetweenJump;
        }
    }

    void HandleSlope()
    {
        //Vector3 origin = transform.position + Vector3.up * _groundOffset;
        //bool slopeRight = Physics2D.RaycastNonAlloc(origin, Vector2.right, _hitResults, _slopeDetectOffset, _GroundLayer) > 0;
        //bool slopeLeft = Physics2D.RaycastNonAlloc(origin, -Vector2.right, _hitResults, _slopeDetectOffset, _GroundLayer) > 0;

        //_isOnSlope = (slopeRight || slopeLeft) && (slopeRight == false || slopeLeft == false);

        //if (Mathf.Abs(_inputs.x) < 0.1f && (slopeLeft || slopeRight))
        //{
        //    _collider.sharedMaterial = _physicsFriction;
        //}
        //else
        //{
        //    _collider.sharedMaterial = _physicsNoFriction;
        //}
    }

    void HandleCorners()
    {
        for (int i = 0; i < directions.Length; i++)
        {
            float dir = directions[i];

            if (Mathf.Abs(_inputs.x) > 0.1f && Mathf.Abs(Mathf.Sign(dir) - Mathf.Sign(_inputs.x)) < 0.001f && _isGrounded == false && _isOnSlope == false)
            {
                Vector3 position = transform.position + new Vector3(_offsetCollisionBox.x + dir * _offsetToReplace.x, _offsetCollisionBox.y, 0);
                int result = Physics2D.BoxCastNonAlloc(position, _collisionBox, 0, Vector2.zero, _hitResults, 0, _GroundLayer);

                if (result > 0)
                {
                    position = transform.position + new Vector3(_offsetCollisionBox.x + dir * _offsetToReplace.x, _offsetCollisionBox.y + _offsetToReplace.y, 0);
                    result = Physics2D.BoxCastNonAlloc(position, _collisionBox, 0, Vector2.zero, _hitResults, 0, _GroundLayer);

                    if (result == 0)
                    {
                        Debug.Log("replace");
                        transform.position += new Vector3(dir * _offsetToReplace.x, _offsetToReplace.y);

                        if (_rb.linearVelocity.y < 0)
                        {
                            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
                        }
                    }
                }
            }
        }
    }
}
