using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

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
    
    public float minExplodeTime = .0f;

    private GameObject activeDynamite = null;
    private float activeDynamiteTimer = 0f;

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

        HandlePlayerDetonation();
    }

    void HandlePlayerDetonation()
    {
        if (activeDynamite != null)
        {
            activeDynamiteTimer += Time.deltaTime;
            
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (activeDynamiteTimer >= minExplodeTime)
                {
                    Dynamite dynamite = activeDynamite.GetComponent<Dynamite>();
                    if (dynamite != null)
                    {
                        dynamite.Explode();
                    }
                }
            }
        }
    }

    void FixedUpdate()
    {
        // rb.velocity = new Vector2(xInput * speed, rb.velocity.y);
    }

    void ThrowDynamite()
    {
        
        GameObject newDynamite = Instantiate(dynamite, transform.position, transform.rotation);
        Rigidbody2D rb = newDynamite.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            
        }

        activeDynamiteTimer = 0f;
        activeDynamite = newDynamite;
    }
}
