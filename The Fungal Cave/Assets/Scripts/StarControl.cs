using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarControl : MonoBehaviour
{
    // Private variables
    private Rigidbody2D rb;
    public PlayerController pc;
    public GameObject fx;

    // Public variables
    public Vector2 direction;
    public float speed = 10;
    public float life = 2.5f;
    public AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction * speed;

        audioManager = pc.audioManager;
    }

    private void Update()
    {
        if (life > 0) life -= Time.deltaTime;
        else if (life <= 0)
        {
            pc.starOut = false;
            GameObject particle = Instantiate(fx);
            particle.transform.position = transform.position;
            Destroy(particle, 3f);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log("Hit " + col.gameObject.tag);
        if (col.gameObject.tag == "Bounce")
        {
            Vector3 reflect = Vector3.Reflect(transform.right, col.contacts[0].normal);
            float rot = 90 - Mathf.Atan2(reflect.z, reflect.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, rot);

            audioManager.source.PlayOneShot(audioManager.MushroomBounce);

            GameObject particle = Instantiate(fx);
            particle.transform.position = transform.position;
            Destroy(particle, 3f);
        }

        if (col.gameObject.tag == "Hazard")
        {
            pc.starOut = false;

            audioManager.source.PlayOneShot(audioManager.MushroomBreak);

            GameObject particle = Instantiate(fx);
            particle.transform.position = transform.position;
            Destroy(particle, 3f);

            Destroy(gameObject);
        }

        if (col.gameObject.tag == "Ground")
        {
            pc.starOut = false;

            audioManager.source.PlayOneShot(audioManager.MushroomBreak);

            GameObject particle = Instantiate(fx);
            particle.transform.position = transform.position;
            Destroy(particle, 3f);

            Destroy(gameObject);
        }
    }
}
