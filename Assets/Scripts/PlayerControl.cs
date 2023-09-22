using Cainos.LucidEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    #region Variables
    public float maxSpeed;
    public float jumpForce;
    public float velPower;
    public float frictionAmount;

    public float jumpInputBufferTime;
    public float coyoteTime;
    #endregion

    public LayerMask solidLayer;

    [Header("Checks")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize;

    private float moveInput;
    private Rigidbody2D rb;
    private float lastOnGroundTime;
    private float lastPressedJumpTime;
    private bool isJumping;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        isJumping = false;
    }

    private void Update()
    {
        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;

        if (IsGrounded() && !isJumping)
        {
            lastOnGroundTime = coyoteTime;
        }

        if (isJumping && rb.velocity.y < 0)
        {
            isJumping = false;
        }

        if (lastOnGroundTime > 0 && lastPressedJumpTime > 0)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        Run();
        //Debug.Log(rb.velocity.x);
    }

    private void Run()
    {
        float targetSpeed = moveInput * maxSpeed;
        float speedDiff = targetSpeed - rb.velocity.x;
        float movement = Mathf.Pow(Mathf.Abs(speedDiff), velPower) * Mathf.Sign(speedDiff);

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

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void JumpCut()
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, solidLayer);
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<float>();
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            lastPressedJumpTime = jumpInputBufferTime;
        }
        if (context.canceled && rb.velocity.y > 0f)
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
    }
}
