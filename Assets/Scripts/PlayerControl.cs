using Cainos.LucidEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    #region Variables
    public float maxSpeed;
    public float jumpForce;
    public float wallJumpForce;
    public float acceleratePower;
    public float frictionAmount;

    public float jumpInputBufferTime;
    public float coyoteTime;
    [Range(0, 1)] public float jumpCutAmount;
    public float wallJumpFreeze;

    public float gravityScale;
    public float fallGravityMultiplier;
    public float maxFallSpeed;
    public float jumpCoolDown;
    public bool renderTrail;
    #endregion

    public LayerMask solidLayer;
    public ParticleSystem doubleJumpEffect;
    public TrailRenderer trail;

    [Header("Checks")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private Transform leftWallCheckPoint;
    [SerializeField] private Transform rightWallCheckPoint;
    [SerializeField] private Vector2 wallCheckSize;

    private float moveInput;
    private Rigidbody2D rb;
    private float lastOnGroundTime;
    private float lastOnWallTime;
    private float lastWallJumpTime;
    private float lastPressedJumpTime;
    private float jumpCoolDownTime;
    private int wallJumpDirection;
    private bool isJumping;
    private bool canDoubleJump;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        isJumping = false;
        canDoubleJump = true;
        rb.gravityScale = gravityScale;
        trail.enabled = renderTrail;
    }

    private void Update()
    {
        lastOnGroundTime -= Time.deltaTime;
        lastOnWallTime -= Time.deltaTime;
        lastWallJumpTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;
        jumpCoolDownTime -= Time.deltaTime;

        trail.enabled = renderTrail;

        if (GoundCheck() && !isJumping)
        {
            lastOnGroundTime = coyoteTime;
            canDoubleJump = true;
        }
        else if(WallCheck())
        {
            lastOnWallTime = coyoteTime;
        }

        if (isJumping && rb.velocity.y < 0)
        {
            isJumping = false;
        }

        if (lastPressedJumpTime > 0 && jumpCoolDownTime <= 0)
        {
            if (lastOnGroundTime > 0 && !isJumping)
            {
                Jump();
            }
            else if (lastOnWallTime > 0)
            {
                WallJump(wallJumpDirection);
            }
            else if (canDoubleJump)
            {
                DoubleJump();
            }
        }

        if (rb.velocity.y < 0)
        {
            rb.gravityScale = gravityScale * fallGravityMultiplier;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
    }

    private void FixedUpdate()
    {
        if (lastWallJumpTime <= 0)
        {
            Run();
        }
    }

    private void Run()
    {
        float targetSpeed = moveInput * maxSpeed;
        float speedDiff = targetSpeed - rb.velocity.x;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff), acceleratePower) * Mathf.Sign(speedDiff);

        rb.AddForce(movement * Vector2.right);

        if (Mathf.Abs(moveInput) < 0.01f)
        {
            float friction = Mathf.Min(Mathf.Abs(rb.velocity.x), frictionAmount);
            friction *= -Mathf.Sign(rb.velocity.x);
            rb.AddForce(friction * Vector2.right, ForceMode2D.Impulse);
        }
    }

    private void Jump()
    {
        isJumping = true;
        lastOnGroundTime = 0;
        lastPressedJumpTime = 0;
        jumpCoolDownTime = jumpCoolDown;

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void WallJump(int direction)
    {
        lastWallJumpTime = wallJumpFreeze;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.right * direction * wallJumpForce, ForceMode2D.Impulse);
        Jump();
    }

    private void DoubleJump()
    {
        canDoubleJump = false;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        doubleJumpEffect.Play();
        Jump();
    }

    private void JumpCut()
    {
        rb.AddForce(Vector2.down * rb.velocity.y * jumpCutAmount, ForceMode2D.Impulse);
    }

    bool GoundCheck()
    {
        return Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, solidLayer);
    }

    bool WallCheck()
    {
        wallJumpDirection = 0;
        if (Physics2D.OverlapBox(leftWallCheckPoint.position, wallCheckSize, 0, solidLayer))
            wallJumpDirection = 1;
        else if (Physics2D.OverlapBox(rightWallCheckPoint.position, wallCheckSize, 0, solidLayer))
            wallJumpDirection = -1;
        return wallJumpDirection != 0;
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<float>();
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            lastPressedJumpTime = jumpInputBufferTime;
        }
        if (context.canceled && rb.velocity.y > 0 && isJumping)
        {
            JumpCut();
        }
    }

    public void OnFireInput(InputAction.CallbackContext context)
    {

    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
        Gizmos.DrawWireCube(leftWallCheckPoint.position, wallCheckSize);
        Gizmos.DrawWireCube(rightWallCheckPoint.position, wallCheckSize);
    }
}
