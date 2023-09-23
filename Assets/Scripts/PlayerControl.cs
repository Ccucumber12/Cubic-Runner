using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public float maxSpeed;
    public float acceleratePower;
    public float frictionAmount;
    public float gravityScale;
    public float fallGravityMultiplier;
    public float maxFallSpeed;
    public bool renderTrail;

    public LayerMask solidLayer;
    public ParticleSystem doubleJumpEffect;
    public TrailRenderer trail;
    public Transform spriteTransform;

    [Header("Jump")]
    public bool hasDoubleJumpAbility;
    public bool hasWallJumpAbility;
    public float jumpForce;
    public float wallJumpForce;
    public float coyoteTime;
    public float jumpCoolDown;
    public float wallJumpFreeze;
    [Range(0, 1)] public float jumpCutAmount;
    public float jumpInputBufferTime;

    [Header("Dash")]
    public bool hasDashAbility;
    public float dashForce;
    public float dashTime;
    public float dashCoolDown;
    public float dashInputBufferTime;

    [Header("Fire")]
    public bool hasFireAbility;
    public Transform firePoint;
    public GameObject bullet;
    public float fireCoolDownTime;
    public float fireInputBufferTime;

    [Header("Checks")]
    public Transform groundCheckPoint;
    public Vector2 groundCheckSize;
    public Transform leftWallCheckPoint;
    public Transform rightWallCheckPoint;
    public Vector2 wallCheckSize;

    private float moveInput;
    private Rigidbody2D rb;
    private float lastOnGroundTime;
    private float lastOnWallTime;
    private float lastWallJumpTime;
    private float lastPressedJumpTime;
    private float lastPressedDashTime;
    private float lastPressedFireTime;
    private float jumpCoolDownTime;
    private int wallJumpDirection;
    private bool isJumping;
    private bool isDashing;
    private bool canDash;
    private bool dashInCoolDown;
    private bool fireInCoolDown;
    private bool canDoubleJump;
    private bool isFacingRight;

    private Vector3 leftWallCheckPointDelta;
    private Vector3 rightWallCheckPointDelta;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        isJumping = false;
        canDoubleJump = true;
        canDash = true;
        isFacingRight = true;
        rb.gravityScale = gravityScale;
        trail.enabled = renderTrail;
        leftWallCheckPointDelta = leftWallCheckPoint.position - transform.position;
        rightWallCheckPointDelta = rightWallCheckPoint.position - transform.position;
    }

    private void Update()
    {
        lastOnGroundTime -= Time.deltaTime;
        lastOnWallTime -= Time.deltaTime;
        lastWallJumpTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;
        lastPressedDashTime -= Time.deltaTime;
        lastPressedFireTime -= Time.deltaTime;
        jumpCoolDownTime -= Time.deltaTime;

        trail.enabled = renderTrail;

        if (GoundCheck())
        {
            lastOnGroundTime = coyoteTime;
            canDoubleJump = true;
            canDash = true;
        }
        else if(WallCheck())
        {
            lastOnWallTime = coyoteTime;
            canDoubleJump = true;
            canDash = true;
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
            else if (hasWallJumpAbility && lastOnWallTime > 0)
            {
                WallJump(wallJumpDirection);
            }
            else if (hasDoubleJumpAbility && canDoubleJump)
            {
                DoubleJump();
            }
        }

        if (hasDashAbility && canDash && !dashInCoolDown && lastPressedDashTime > 0)
        {
            StartCoroutine(Dash());
        }

        if (hasFireAbility && !fireInCoolDown && lastPressedFireTime > 0)
        {
            StartCoroutine(Fire());
        }

        if (moveInput != 0)
        {
            if (isFacingRight ^ (moveInput > 0))
                Turn();
        }

        if (!isDashing)
        {
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
    }

    private void FixedUpdate()
    {
        if (lastWallJumpTime <= 0 && !isDashing)
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

    private IEnumerator Dash()
    {
        canDash = false;
        dashInCoolDown = true;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(0, 0);
        rb.AddForce(Vector2.right * GetDirection() * dashForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashCoolDown);
        dashInCoolDown = false;
    }

    private IEnumerator Fire()
    {
        fireInCoolDown = true;
        GameObject newBullet = Instantiate(bullet, firePoint.position, transform.rotation);
        newBullet.GetComponent<Bullet>().direction = GetDirection();
        
        yield return new WaitForSeconds(fireCoolDownTime);
        fireInCoolDown = false;
    }

    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        isFacingRight = !isFacingRight;
    }

    private int GetDirection()
    {
        if (moveInput != 0)
            return (int)Mathf.Sign(moveInput);
        else
            return isFacingRight ? 1 : -1;
    }

    bool GoundCheck()
    {
        return Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, solidLayer);
    }

    bool WallCheck()
    {
        wallJumpDirection = 0;
        if (Physics2D.OverlapBox(transform.position + leftWallCheckPointDelta, wallCheckSize, 0, solidLayer))
            wallJumpDirection = 1;
        else if (Physics2D.OverlapBox(transform.position + rightWallCheckPointDelta, wallCheckSize, 0, solidLayer))
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
        if (context.started)
        {
            lastPressedFireTime = fireInputBufferTime;
        }
    }


    public void OnDashInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("dash");
            lastPressedDashTime = dashInputBufferTime;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
        Gizmos.DrawWireCube(leftWallCheckPoint.position, wallCheckSize);
        Gizmos.DrawWireCube(rightWallCheckPoint.position, wallCheckSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(firePoint.position, bullet.GetComponent<CircleCollider2D>().radius * bullet.transform.localScale.x);
    }
#endif
}
