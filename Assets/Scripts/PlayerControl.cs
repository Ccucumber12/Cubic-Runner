using Cainos.LucidEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    #region Variables
    public float maxSpeed;
    public float jumpPower;
    #endregion

    public LayerMask solidLayer;

    [Header("Checks")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize;

    private float horizontalInput;
    private Rigidbody2D rb;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * maxSpeed, rb.velocity.y);
    }

    bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, solidLayer);
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalInput = context.ReadValue<float>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
        }
        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    public void Fire(InputAction.CallbackContext context)
    {

    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
    }
}
