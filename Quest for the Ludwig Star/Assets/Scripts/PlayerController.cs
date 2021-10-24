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
    private Vector3 aimDir = Vector3.right;
    private bool throwMode = false;
    public bool falling = false;
    public bool faceplant = false;
    public bool canGetUp = false;
    private GameObject currentStar;
    private GameObject reticle;
    private Animator animator;
    private SpriteRenderer sr;


    // Public Variables
    public float speed;
    public float jumpHeight;
    public float midAirControl;
    public float coyoteTime;
    public float jumpBufferTime;
    public float starBufferTime;
    public float faceplantBufferTime;
    public LayerMask groundLayer;
    public LayerMask hazardLayer;
    public GameObject star;
    public bool starOverlap = false;
    public bool starOut = false;
    public float starBonus;
    public TimeControl tc;
    public float knockbackH;
    public float knockbackV;
    public GameObject fx;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        bc = this.GetComponent<BoxCollider2D>();
        animator = this.GetComponent<Animator>();
        sr = this.GetComponent<SpriteRenderer>();
        reticle = this.transform.GetChild(1).gameObject;
        reticle.GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // If not falling normal movement
        if (!falling && !faceplant)
        {
            // Throw Mode (Only if star isn't already out)
            if (!starOut)
            {
                if (Input.GetButtonDown("Throw")) // When button is pressed
                {
                    throwMode = true;
                    reticle.GetComponent<SpriteRenderer>().enabled = true;
                    tc.doSlowMotion();
                }
                if (Input.GetButton("Throw")) // While button is down
                {
                    if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1 || Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.1)
                    {
                        aimDir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                    }
                    reticle.transform.position = this.transform.position + aimDir;
                }
                if (Input.GetButtonUp("Throw") && throwMode) // When button is let go
                {
                    tc.resetTime = true;
                    reticle.GetComponent<SpriteRenderer>().enabled = false;
                    currentStar = Instantiate(star, reticle.transform.position, reticle.transform.rotation);
                    currentStar.GetComponent<StarControl>().direction = aimDir.normalized;
                    currentStar.GetComponent<StarControl>().pc = this.gameObject.GetComponent<PlayerController>();
                    Physics2D.IgnoreCollision(currentStar.GetComponent<BoxCollider2D>(), this.GetComponent<BoxCollider2D>());
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
            }
            else
            {
                if (Grounded()) horizontalMove = 0;
            }

            // Star Jump
            if (jumpBufferCounter > 0 && starOverlap && starBufferCounter <= 0)
            {
                starOut = false;
                Destroy(currentStar);
                starBufferCounter = starBufferTime * 2;
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;

                animator.SetBool("isJumping", true);

                GameObject particle = Instantiate(fx);
                particle.transform.position = transform.position;
                Destroy(particle, 3f);

                Vector2 jumpForce = new Vector2(0, jumpHeight * starBonus);
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, 0, jumpHeight * starBonus));
            }
            // Normal Jump
            else if (jumpBufferCounter > 0 && coyoteTimeCounter > 0f)
            {
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;

                animator.SetBool("isJumping", true);

                Vector2 jumpForce = new Vector2(0, jumpHeight);
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, 0, jumpHeight));
            }

        }
        // If falling
        else if (falling)
        {
            // When you hit the ground
            if (Grounded())
            {
                rb.velocity = new Vector2(0, 0);
                faceplant = true;
                canGetUp = false;
                falling = false;
                animator.SetBool("isFalling", false);
                animator.SetBool("isFaceplant", true);
                StartCoroutine(faceplantCD());
            }
        }
        // If faceplanted
        else if (faceplant)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            horizontalMove = 0;
        }

        // Coyote Time
        if (Grounded()) coyoteTimeCounter = coyoteTime;
        else if (coyoteTimeCounter > 0) coyoteTimeCounter -= Time.deltaTime;

        // Jump Buffer
        if (Input.GetButtonDown("Jump")) jumpBufferCounter = jumpBufferTime;
        else if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;

        // Star Buffer
        if(starBufferCounter > 0) starBufferCounter -= Time.deltaTime;

        // Reset anims
        if (Grounded() && Mathf.Abs(rb.velocity.y) < 0.1) animator.SetBool("isJumping", false);
        animator.SetFloat("VerticalSpeed", rb.velocity.y);
        if (faceplant && canGetUp && (Input.GetButtonDown("Jump") || Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0))
        {
            faceplant = false;
            canGetUp = false;
            animator.SetBool("isFaceplant", false);
        }

        // Sprite Flip
        if (rb.velocity.x > 0.1) sr.flipX = false;
        else if (rb.velocity.x < -0.1) sr.flipX = true;
    }

    private void FixedUpdate()
    {
        if (!falling)
        {
            if (Grounded())
            {
                animator.SetFloat("HorizontalMovement", Mathf.Abs(horizontalMove));
                rb.velocity = new Vector2(horizontalMove * speed, rb.velocity.y);
            }
            else
            {
                rb.velocity += new Vector2(horizontalMove * speed * midAirControl * Time.deltaTime, 0);
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -speed, +speed), rb.velocity.y);
            }
        }

    }

    IEnumerator faceplantCD()
    {
        yield return new WaitForSeconds(faceplantBufferTime);
        canGetUp = true;
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

    private void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log("Hit " + col.gameObject.tag);
        if (col.gameObject.tag == "Hazard")
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", true);
            falling = true;

            //Vector2 dir = (col.gameObject.transform.position - gameObject.transform.position).normalized;
            //Debug.Log(dir.x);

            RaycastHit2D raycastHitleft = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.left, .1f, hazardLayer);
            RaycastHit2D raycastHitright = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.right, .1f, hazardLayer);
            RaycastHit2D raycastHitdown = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0f, Vector2.down, .1f, hazardLayer);

            if (raycastHitleft.collider != null)
            {
                //Debug.Log("Left");
                Knockback(knockbackH, knockbackV);
            }
            else if (raycastHitright.collider != null)
            {
                //Debug.Log("Right");
                Knockback(-knockbackH, knockbackV);
            }
            else if (raycastHitdown.collider != null)
            {
                //Debug.Log("Down");
                if (Random.value < 0.5f)
                    Knockback(knockbackH, knockbackV);
                else
                    Knockback(-knockbackH, knockbackV);
            }

        }
    }

    private void Knockback(float horizontalKB, float verticalKB)
    {
        Vector2 knockbackForce = new Vector2(horizontalKB, verticalKB);
        rb.velocity = new Vector2(0, 0);
        rb.AddForce(knockbackForce, ForceMode2D.Impulse);
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -knockbackH, knockbackH), Mathf.Clamp(rb.velocity.y, -knockbackV, knockbackV));
    }
}
