using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarControl : MonoBehaviour
{
    // Private variables
    private Rigidbody2D rb;
    public PlayerController pc;

    // Public variables
    public Vector2 direction;
    public float speed = 10;
    public float bounces = 1;
    public float life = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction * speed;
    }

    private void Update()
    {
        if (life > 0) life -= Time.deltaTime;
        else if (life <= 0)
        {
            pc.starOut = false;
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log("Hit " + col.gameObject.tag);
        if (col.gameObject.tag == "Ground")
        {
            Vector3 reflect = Vector3.Reflect(transform.right, col.contacts[0].normal);
            float rot = 90 - Mathf.Atan2(reflect.z, reflect.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, rot);

            if (bounces > 0) bounces -= 1;
            else if (bounces <= 0)
            {
                pc.starOut = false;
                Destroy(gameObject);
            }
        }
    }
}
