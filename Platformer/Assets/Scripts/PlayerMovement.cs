using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    Rigidbody2D rb;
    Animator anim;

    //Ground check related
    BoxCollider2D boxCol;
    Vector3 groundCheckColSize;

    [Header("Movement")]
    [Tooltip("Horizontal movement speed")]
    [SerializeField] private float playerSpeed;
    private bool facingRight = true;

    [Header("Input")]
    private float moveInput;


    [Header("Jump")]
    [Tooltip("How high will you be able to jump...")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpDelay = 0.25f;
    private float jumpTimer;

    [Header("Physics")]
    //Jump gravity tweak
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultilpier = 2f;
    private bool isFalling;
    [SerializeField] private float fallSpeed = 2f;

    //Ground Check Related
    [Tooltip("Point of origin for the collider on Y axis")]
    [SerializeField] private Vector3 colOffset;

    [Tooltip("What layer will be classified as ground")]
    [SerializeField] private LayerMask whatIsGround;

    [Tooltip("Collider size. Higher = easier ground detection")]
    [SerializeField] private float groundCheckSize;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        //groundCheckColSize = new Vector3(boxCol.bounds.size.x, boxCol.bounds.size.y + groundCheckSize);
        anim = GetComponentInChildren<Animator>();


    }
    private void FixedUpdate()
    {
        if ((moveInput > 0 && !facingRight) || (moveInput < 0 && facingRight)) Flip();
        if (isFalling) Falling();
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultilpier - 1) * Time.deltaTime;
        }
        rb.velocity = new Vector2(moveInput * playerSpeed, rb.velocity.y);
        if (jumpTimer > Time.time && CheckGround() == true) Jump();

        anim.SetBool("isGrounded", CheckGround());
    }

    private void Update()
    {
        moveInput = Input.GetAxis("Horizontal");
        if (moveInput != 0) anim.SetBool("isRunning", true);
        else anim.SetBool("isRunning", false);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpTimer = Time.time + jumpDelay;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }

    private void Falling()
    {
        rb.AddForce(Vector2.down * fallSpeed);
    }
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumpTimer = 0;
    }

    private bool CheckGround()
    {
        //if (Physics2D.BoxCast(boxCol.bounds.center, groundCheckColSize, 0f, Vector2.down, colOffset, whatIsGround))
        //{
        //    return true;
        //}
        //else
        //{
        //    return false;
        //}
        if (Physics2D.Raycast(transform.position + colOffset, Vector2.down, groundCheckSize, whatIsGround) || Physics2D.Raycast(transform.position - colOffset, Vector2.down, groundCheckSize, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colOffset, transform.position + colOffset + Vector3.down * groundCheckSize);
        Gizmos.DrawLine(transform.position - colOffset, transform.position - colOffset + Vector3.down * groundCheckSize);
    }
}