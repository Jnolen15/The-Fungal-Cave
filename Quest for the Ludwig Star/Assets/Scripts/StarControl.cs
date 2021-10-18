using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarControl : MonoBehaviour
{
    // Private variables
    private Rigidbody2D rb;

    // Public variables
    public Vector2 direction;
    public float speed = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity = direction * speed;
        //rb.AddForce(direction * speed, ForceMode2D.Impulse);
        //rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("Hit " + col.gameObject.tag);
        if (col.gameObject.tag == "Ground")
        {
            Destroy(this.gameObject);
        }
    }
}
