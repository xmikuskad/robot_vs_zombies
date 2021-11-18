using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DebuggingPlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float jumpForce = 10f;
    private Rigidbody2D rb;

    public float throwForce = 10f;
    public GameObject dynamite;

    float xInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        xInput = Input.GetAxisRaw("Horizontal");
        if(Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            ThrowDynamite();
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(xInput * speed, rb.velocity.y);

    }

    void ThrowDynamite()
    {
        
        
        GameObject newDynamite = Instantiate(dynamite, transform.position, transform.rotation);
        Rigidbody2D rb = newDynamite.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            
        }
    }
}
