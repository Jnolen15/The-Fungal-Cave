using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Private Variables
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private float horizontalMove;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private float starBufferCounter;
    private Vector2 aimDir = Vector2.right;
    private bool throwMode = false;


    // Public Variables
    public float speed;
    public float jumpHeight;
    public float midAirControl;
    public float coyoteTime;
    public float jumpBufferTime;
    public float starBufferTime;
    public LayerMask groundLayer;
    public GameObject star;
    public bool starOverlap = false;
    public bool starOut = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        bc = this.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Throw Mode (Only if star isn't already out)
        if (!starOut)
        {
            if (Input.GetButtonDown("Throw")) throwMode = true;
            if (Input.GetButtonUp("Throw") && throwMode)
            {
                if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1 || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1)
                {
                    aimDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                }
                GameObject currentStar = Instantiate(star, transform.position, transform.rotation);
                currentStar.GetComponent<StarControl>().direction = aimDir.normalized;
                currentStar.GetComponent<StarControl>().pc = this.gameObject.GetComponent<PlayerController>();
                Physics2D.IgnoreCollision(currentStar.GetComponent<CircleCollider2D>(), this.GetComponent<BoxCollider2D>());
                throwMode = false;
                starOut = true;
                starBufferCounter = starBufferTime;
            }
        }

        //Movement if not in throw mode
        if (!throwMode)
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
        } else
        {
            if(Grounded()) horizontalMove = 0;
        }

        // Coyote Time
        if (Grounded() || (starOverlap && starBufferCounter <= 0)) coyoteTimeCounter = coyoteTime;
        else if (coyoteTimeCounter > 0) coyoteTimeCounter -= Time.deltaTime;

        // Jump Buffer
        if (Input.GetButtonDown("Jump")) jumpBufferCounter = jumpBufferTime;
        else if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;

        // Star Buffer
        if(starBufferCounter > 0) starBufferCounter -= Time.deltaTime;

        // Get Jump
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0f)
        {
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;

            Vector2 jumpForce = new Vector2(0, jumpHeight);
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, 0, jumpHeight));

            if(starOverlap) starBufferCounter = starBufferTime * 2;
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