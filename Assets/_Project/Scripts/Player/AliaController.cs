using JudgmentOfTheFallenWing.Abilities;
using UnityEngine;

namespace JudgmentOfTheFallenWing.Player
{
    /// <summary>
    /// Controlador principal de ALIA — movimiento Metroidvania con coyote time,
    /// jump buffer, wall slide/jump y dash.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(AliaHealth))]
    [RequireComponent(typeof(AliaAbilityController))]
    public class AliaController : MonoBehaviour
    {
        [Header("Movimiento")]
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float acceleration = 60f;
        [SerializeField] private float deceleration = 80f;
        [SerializeField] private float jumpForce = 14f;
        [SerializeField] private float coyoteTime = 0.12f;
        [SerializeField] private float jumpBufferTime = 0.12f;

        [Header("Wall")]
        [SerializeField] private float wallSlideSpeed = 2f;
        [SerializeField] private Vector2 wallCheckOffset = new(0.4f, 0f);
        [SerializeField] private float wallCheckDistance = 0.2f;
        [SerializeField] private Vector2 wallJumpForce = new(10f, 14f);

        [Header("Dash")]
        [SerializeField] private float dashSpeed = 20f;
        [SerializeField] private float dashDuration = 0.18f;
        [SerializeField] private float dashCooldown = 0.6f;

        [Header("Detección")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayer;

        [Header("Referencias")]
        [SerializeField] private AliaCombat combat;
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Rigidbody2D _rb;
        private AliaAbilityController _abilities;
        private AliaHealth _health;

        private float _horizontalInput;
        private float _coyoteCounter;
        private float _jumpBufferCounter;
        private float _dashTimer;
        private float _dashCooldownTimer;
        private bool _isDashing;
        private bool _isWallSliding;
        private int _facingDirection = 1;
        private int _jumpsRemaining = 1;

        public bool IsGrounded { get; private set; }
        public bool IsWallSliding => _isWallSliding;
        public bool IsDashing => _isDashing;
        public int FacingDirection => _facingDirection;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _abilities = GetComponent<AliaAbilityController>();
            _health = GetComponent<AliaHealth>();
        }

        private void Update()
        {
            if (!_health.IsAlive) return;

            ReadInput();
            UpdateTimers();
            CheckGround();
            CheckWallSlide();
            HandleJumpInput();
            HandleDashInput();
            HandleAttackInput();
            UpdateAnimator();
            FlipSprite();
        }

        private void FixedUpdate()
        {
            if (!_health.IsAlive) return;

            if (_isDashing)
            {
                _rb.velocity = new Vector2(_facingDirection * dashSpeed, 0f);
                return;
            }

            ApplyHorizontalMovement();
            ApplyWallSlide();
        }

        private void ReadInput()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump"))
                _jumpBufferCounter = jumpBufferTime;
        }

        private void UpdateTimers()
        {
            if (_coyoteCounter > 0f) _coyoteCounter -= Time.deltaTime;
            if (_jumpBufferCounter > 0f) _jumpBufferCounter -= Time.deltaTime;
            if (_dashCooldownTimer > 0f) _dashCooldownTimer -= Time.deltaTime;

            if (_isDashing)
            {
                _dashTimer -= Time.deltaTime;
                if (_dashTimer <= 0f)
                    EndDash();
            }
        }

        private void CheckGround()
        {
            IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            if (IsGrounded)
            {
                _coyoteCounter = coyoteTime;
                _jumpsRemaining = _abilities.HasAbility(AbilityType.DoubleJump) ? 2 : 1;
            }
        }

        private void CheckWallSlide()
        {
            if (IsGrounded || _horizontalInput == 0f)
            {
                _isWallSliding = false;
                return;
            }

            var wallDirection = _horizontalInput > 0 ? 1 : -1;
            var origin = (Vector2)transform.position + new Vector2(wallCheckOffset.x * wallDirection, wallCheckOffset.y);
            var hit = Physics2D.Raycast(origin, Vector2.right * wallDirection, wallCheckDistance, groundLayer);

            _isWallSliding = hit.collider != null && _rb.velocity.y < 0f
                && _abilities.HasAbility(AbilityType.WallJump);
        }

        private void HandleJumpInput()
        {
            if (_jumpBufferCounter <= 0f) return;

            if (_coyoteCounter > 0f)
            {
                PerformJump(jumpForce);
                _coyoteCounter = 0f;
                _jumpBufferCounter = 0f;
                return;
            }

            if (_isWallSliding)
            {
                var jumpDirection = -Mathf.Sign(_horizontalInput);
                if (jumpDirection == 0) jumpDirection = -_facingDirection;
                _rb.velocity = new Vector2(jumpDirection * wallJumpForce.x, wallJumpForce.y);
                _isWallSliding = false;
                _jumpBufferCounter = 0f;
                _coyoteCounter = 0f;
                return;
            }

            if (_jumpsRemaining > 0 && _abilities.HasAbility(AbilityType.DoubleJump))
            {
                PerformJump(jumpForce * 0.9f);
                _jumpsRemaining--;
                _jumpBufferCounter = 0f;
            }
        }

        private void PerformJump(float force)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, force);
        }

        private void HandleDashInput()
        {
            if (!Input.GetKeyDown(KeyCode.LeftShift)) return;
            if (!_abilities.HasAbility(AbilityType.Dash)) return;
            if (_isDashing || _dashCooldownTimer > 0f) return;

            StartDash();
        }

        private void StartDash()
        {
            _isDashing = true;
            _dashTimer = dashDuration;
            _dashCooldownTimer = dashCooldown;
            _rb.gravityScale = 0f;
        }

        private void EndDash()
        {
            _isDashing = false;
            _rb.gravityScale = 3f;
            _rb.velocity = new Vector2(_rb.velocity.x * 0.5f, _rb.velocity.y);
        }

        private void HandleAttackInput()
        {
            if (Input.GetKeyDown(KeyCode.J) && combat != null)
            {
                combat.TryAttack();
                combat.SetFacingDirection(_facingDirection);
            }
        }

        private void ApplyHorizontalMovement()
        {
            var targetSpeed = _horizontalInput * moveSpeed;
            var speedDiff = targetSpeed - _rb.velocity.x;
            var accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
            var movement = speedDiff * accelRate * Time.fixedDeltaTime;

            _rb.velocity = new Vector2(_rb.velocity.x + movement, _rb.velocity.y);

            if (Mathf.Abs(_horizontalInput) > 0.01f)
                _facingDirection = _horizontalInput > 0 ? 1 : -1;
        }

        private void ApplyWallSlide()
        {
            if (!_isWallSliding) return;
            _rb.velocity = new Vector2(_rb.velocity.x, Mathf.Max(_rb.velocity.y, -wallSlideSpeed));
        }

        private void UpdateAnimator()
        {
            if (animator == null) return;

            animator.SetFloat("Speed", Mathf.Abs(_rb.velocity.x));
            animator.SetFloat("VelocityY", _rb.velocity.y);
            animator.SetBool("IsGrounded", IsGrounded);
            animator.SetBool("IsWallSliding", _isWallSliding);
            animator.SetBool("IsDashing", _isDashing);
        }

        private void FlipSprite()
        {
            if (spriteRenderer == null) return;
            if (_facingDirection != 0)
                spriteRenderer.flipX = _facingDirection < 0;
        }

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
