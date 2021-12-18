using System;
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
    
    // how many dynamites should regenerate in one iteration
    public int dynamiteRechargeCount = 1;
    // max magazine
    public int maxDynamiteMagazineCount = 2;
    // current magazine
    public int dynamiteMagazineCount = 2;
    // how often magazine recharges
    public float dynamiteMagazineRechargeRate = 1.0f;
    // current recharge progression
    public float dynamiteMagazineRechargeTimer = 0.0f;

    public float deltaExplosionTime = 0.0f;
    public float minDeltaExplosionTime = 0.5f;

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
        UpdateDynamiteMagazine();
    }

    private void UpdateDynamiteMagazine()
    {
        dynamiteMagazineRechargeTimer += Time.deltaTime;
        
        if (dynamiteMagazineRechargeTimer >= dynamiteMagazineRechargeRate)
        {
            dynamiteMagazineCount = Math.Min(maxDynamiteMagazineCount, dynamiteMagazineCount + dynamiteRechargeCount);
            dynamiteMagazineRechargeTimer -= dynamiteMagazineRechargeRate;
        }
    }

    private void HandlePlayerDetonation()
    {
        // Minimal time between detonations
        deltaExplosionTime += Time.deltaTime;
        if (deltaExplosionTime < minDeltaExplosionTime) return;
        
        if (activeDynamite == null) return;
        
        activeDynamiteTimer += Time.deltaTime;

        // Handle on key release
        if (!Input.GetKeyUp(KeyCode.Mouse0)) return;

        if (!(activeDynamiteTimer >= minExplodeTime)) return;
        
        var dynamiteScript = activeDynamite.GetComponent<Dynamite>();
        if (dynamiteScript != null)
        {
            dynamiteScript.Explode();
            deltaExplosionTime = 0.0f;
        }
    }

    void FixedUpdate()
    {
        // rb.velocity = new Vector2(xInput * speed, rb.velocity.y);
    }
    
    void ThrowDynamite()
    {
        // If magazine is empty, do nothing
        if (dynamiteMagazineCount <= 0) return;
        
        dynamiteMagazineCount -= 1;
        
        GameObject newDynamite = Instantiate(dynamite, transform.position, transform.rotation);
        
        Rigidbody2D dynamiteRb = newDynamite.GetComponent<Rigidbody2D>();
        if (dynamiteRb != null)
        {
            dynamiteRb.velocity = rb.velocity;
        }

        activeDynamiteTimer = 0f;
        activeDynamite = newDynamite;
    }
}
