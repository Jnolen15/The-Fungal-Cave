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
    private float floatCounter;
    private Vector3 aimDir = Vector3.right;
    private bool falling = false;
    private bool faceplant = false;
    private bool canGetUp = false;
    private bool landed = true;
    private bool jumped = false;
    private bool throwMode = false;
    public bool jumpKeyHeld = false;
    private GameObject currentStar;
    private GameObject reticle;
    private Animator animator;
    private SpriteRenderer sr;


    // Public Variables
    public float speed;
    public float jumpHeight;
    public float midAirControl;
    public float airBrake;
    public float coyoteTime;
    public float jumpBufferTime;
    public float starBufferTime;
    public float faceplantBufferTime;
    public float floatTime;
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
    public AudioManager audioManager;
    public DialogueTrigger dialogueTrigger;
    public DialogueManager dialogueManager;
    public bool startedDialogue = false;
    public Vector2 counterJumpForce;

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
        if (!startedDialogue)
        {
            // If not falling normal movement
            if (!falling && !faceplant)
            {
                // Throw Mode (Only if star isn't already out)
                if (!starOut)
                {
                    // Directional quick shot way
                    /*if (Mathf.Abs(Input.GetAxisRaw("HorizontalThrow")) > 0.1 || Mathf.Abs(Input.GetAxisRaw("VerticalThrow")) > 0.1) // When button is pressed
                    {
                        if (Mathf.Abs(Input.GetAxisRaw("HorizontalThrow")) > 0.1 || Mathf.Abs(Input.GetAxisRaw("VerticalThrow")) > 0.1)
                        {
                            aimDir = new Vector3(Input.GetAxisRaw("HorizontalThrow"), Input.GetAxisRaw("VerticalThrow"));
                        }

                        currentStar = Instantiate(star, transform.position, transform.rotation);
                        currentStar.GetComponent<StarControl>().direction = aimDir.normalized;
                        currentStar.GetComponent<StarControl>().pc = this.gameObject.GetComponent<PlayerController>();
                        Physics2D.IgnoreCollision(currentStar.GetComponent<BoxCollider2D>(), this.GetComponent<BoxCollider2D>());
                        starOut = true;
                        starBufferCounter = starBufferTime;

                        audioManager.source.PlayOneShot(audioManager.MushroomThrow);

                        // Start float
                        floatCounter = floatTime;
                    }*/

                    // Hold X to enter aim mode way
                    if (Input.GetButtonDown("Throw")) // When button is pressed
                    {
                        throwMode = true;
                        reticle.GetComponent<SpriteRenderer>().enabled = true;
                        //tc.doSlowMotion();
                    }
                    if (Input.GetButton("Throw")) // While button is down
                    {
                        if (Mathf.Abs(Input.GetAxisRaw("HorizontalMove")) > 0.1 || Mathf.Abs(Input.GetAxisRaw("VerticalMove")) > 0.1)
                        {
                            aimDir = new Vector3(Input.GetAxisRaw("HorizontalMove"), Input.GetAxisRaw("VerticalMove"));
                        }
                        reticle.transform.position = this.transform.position + aimDir;
                    }
                    if (Input.GetButtonUp("Throw") && throwMode) // When button is let go
                    {
                        //tc.resetTime = true;
                        reticle.GetComponent<SpriteRenderer>().enabled = false;
                        currentStar = Instantiate(star, transform.position, transform.rotation);
                        currentStar.GetComponent<StarControl>().direction = aimDir.normalized;
                        currentStar.GetComponent<StarControl>().pc = this.gameObject.GetComponent<PlayerController>();
                        Physics2D.IgnoreCollision(currentStar.GetComponent<BoxCollider2D>(), this.GetComponent<BoxCollider2D>());
                        throwMode = false;
                        starOut = true;
                        starBufferCounter = starBufferTime;

                        audioManager.source.PlayOneShot(audioManager.MushroomThrow);

                        // Start float
                        floatCounter = floatTime;
                    }
                }

                // On the ground movement
                if (Grounded())
                {
                    if (!(TouchingWallLeft() || TouchingWallRight())) horizontalMove = Input.GetAxisRaw("HorizontalMove");
                    // If touching left wall only move away
                    else if (TouchingWallLeft())
                    {
                        if (Input.GetAxisRaw("HorizontalMove") >= 0.1) horizontalMove = Input.GetAxisRaw("HorizontalMove");
                        else horizontalMove = 0;
                    }
                    // If touching right wall only move away
                    else if (TouchingWallRight())
                    {
                        if (Input.GetAxisRaw("HorizontalMove") <= -0.1) horizontalMove = Input.GetAxisRaw("HorizontalMove");
                        else horizontalMove = 0;
                    }
                } // If touching a wall in air
                else if (TouchingWallLeft() || TouchingWallRight())
                {
                    horizontalMove = 0;
                } // In air movement
                else
                {
                    horizontalMove = Input.GetAxisRaw("HorizontalMove");
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

                jumped = true;
                landed = false;
                animator.SetBool("isJumping", true);

                GameObject particle = Instantiate(fx);
                particle.transform.position = transform.position;
                Destroy(particle, 3f);

                Vector2 jumpForce = new Vector2(0, jumpHeight * starBonus);
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, 0, jumpHeight * starBonus));

                audioManager.source.PlayOneShot(audioManager.Jump);
            }
            // Normal Jump
            else if (jumpBufferCounter > 0 && coyoteTimeCounter > 0f)
            {
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;

                jumped = true;
                landed = false;
                animator.SetBool("isJumping", true);

                Vector2 jumpForce = new Vector2(0, jumpHeight);
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, 0, jumpHeight));

                //audioManager.source.PlayOneShot(audioManager.Land);
            }
            // If let go of the jump key. Shorter jump.
            else if (Input.GetButtonUp("Jump"))
            {
                jumpKeyHeld = false;
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
                    landed = true;
                    audioManager.source.PlayOneShot(audioManager.Faceplant);
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
            if (Input.GetButtonDown("Jump"))
            {
                jumpKeyHeld = true;
                jumpBufferCounter = jumpBufferTime;
            }
            else if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;

            // Star Buffer
            if (starBufferCounter > 0) starBufferCounter -= Time.deltaTime;

            // Float Time
            if (floatCounter > 0) floatCounter -= Time.deltaTime;

            // Reset anims
            if (Grounded() && Mathf.Abs(rb.velocity.y) < 0.1)
            {
                jumped = false;
                //landed = true;
                animator.SetBool("isJumping", false);
            }
            animator.SetFloat("VerticalSpeed", rb.velocity.y);
            if (faceplant && canGetUp && (Input.GetButtonDown("Jump") || Mathf.Abs(Input.GetAxisRaw("HorizontalMove")) > 0))
            {
                faceplant = false;
                canGetUp = false;
                animator.SetBool("isFaceplant", false);
            }

            // Sprite Flip
            if (rb.velocity.x > 0.1) sr.flipX = false;
            else if (rb.velocity.x < -0.1) sr.flipX = true;

            // Landing sound effect
            if (!landed && !jumped)
            {
                landed = true;
                audioManager.source.PlayOneShot(audioManager.Land);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Z)) dialogueManager.DisplayNextSentence();
    }

    private void FixedUpdate()
    {
        if (!falling)
        {
            if (Grounded()) // Ground movement
            {
                animator.SetFloat("HorizontalMovement", Mathf.Abs(horizontalMove));
                rb.velocity = new Vector2(horizontalMove * speed, rb.velocity.y);
            }
            else // In air movement
            {
                if(floatCounter > 0) // Short float after throwing a mushroom ball
                {
                    rb.velocity += new Vector2(horizontalMove * speed * midAirControl * Time.deltaTime, 0);
                    rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -speed, +speed), Mathf.Clamp(rb.velocity.y, 0, jumpHeight*starBonus));
                }
                else
                {
                    // Horizontal controll
                    if (Mathf.Abs(horizontalMove) > 0.1)
                    {
                        rb.velocity += new Vector2(horizontalMove * speed * midAirControl * Time.deltaTime, 0);
                        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -speed, +speed), rb.velocity.y);
                    }
                    // Air Braking 
                    else
                    {
                        if (rb.velocity.x > 0.1)
                        {
                            rb.velocity -= new Vector2(airBrake, 0);
                            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, 0, +speed), rb.velocity.y);
                        } else if (rb.velocity.x < -0.1)
                        {
                            rb.velocity -= new Vector2(-airBrake, 0);
                            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -speed, 0), rb.velocity.y);
                        }

                    }
                    // Limiting jump if its no longer held down
                    /*if (!jumpKeyHeld && Vector2.Dot(rb.velocity, Vector2.up) > 0)
                    {
                        rb.AddForce(counterJumpForce * rb.mass);
                    }*/
                }
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
            jumped = false;
            landed = false;
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", true);
            falling = true;

            audioManager.source.PlayOneShot(audioManager.HitHazard);

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

    private void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("Hit " + col.gameObject.tag);
        if (col.gameObject.tag == "End" && !startedDialogue)
        {
            SpeedRunTimer.timerInstance.EndTimer();
            dialogueTrigger.TriggerDialogue();
            startedDialogue = true;
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
