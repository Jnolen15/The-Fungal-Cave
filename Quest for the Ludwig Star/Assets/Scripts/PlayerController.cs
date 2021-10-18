using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Private Variables
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private float horizontalMove;
    public float coyoteTimeCounter;
    public float jumpBufferCounter;

    // Public Variables
    public float speed;
    public float jumpHeight;
    public float midAirControl;
    public float coyoteTime;
    public float jumpBufferTime;
    public LayerMask groundLayer;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        bc = this.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // On the ground movement
        if (Grounded())
        {
            if (!(TouchingWallLeft() || TouchingWallRight())) horizontalMove = Input.GetAxisRaw("Horizontal");
            // If touching left wall only move away
            else if (TouchingWallLeft())
            {
                if (Input.GetAxisRaw("Horizontal") >= 0.1) horizontalMove = Input.GetAxisRaw("Horizontal");
                else horizontalMove = 0;
            }
            // If touching right wall only move away
            else if (TouchingWallRight())
            {
                if (Input.GetAxisRaw("Horizontal") <= -0.1) horizontalMove = Input.GetAxisRaw("Horizontal");
                else horizontalMove = 0;
            }
        } // If touching a wall in air
        else if (TouchingWallLeft() || TouchingWallRight())
        {
            horizontalMove = 0;
        } // In air movement
        else
        {
            horizontalMove = Input.GetAxisRaw("Horizontal");
        }

        // Coyote Time
        if (Grounded()) coyoteTimeCounter = coyoteTime;
        else if (coyoteTimeCounter > 0) coyoteTimeCounter -= Time.deltaTime;

        // Jump Buffer
        if (Input.GetButtonDown("Jump")) jumpBufferCounter = jumpBufferTime;
        else if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;

        // Get Jump
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0f)
        {
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;

            Vector2 jumpForce = new Vector2(0, jumpHeight);
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, 0, jumpHeight));
        }
    }

    private void FixedUpdate()
    {
        if(Grounded()) rb.velocity = new Vector2(horizontalMove * speed, rb.velocity.y);
        else
        {
            rb.velocity += new Vector2(horizontalMove * speed * midAirControl * Time.deltaTime, 0);
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -speed, +speed), rb.velocity.y);
        }
    }

    public bool Grounded()      //Tests to see if the player is touching the ground in order to jump
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, .1f, groundLayer);
        return raycastHit2d.collider != null;
    }

    public bool TouchingWallLeft()      //Tests to see if the player is touching the ground in order to jump
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.left, .1f, groundLayer);
        return raycastHit2d.collider != null;
    }
    public bool TouchingWallRight()      //Tests to see if the player is touching the ground in order to jump
    {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.right, .1f, groundLayer);
        return raycastHit2d.collider != null;
    }
}
