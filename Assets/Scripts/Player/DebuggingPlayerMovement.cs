using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;
using Random = UnityEngine.Random;

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
    
    [FormerlySerializedAs("minExplodeTime")] public float minMouseHoldToDetonateTime = .0f;

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
    private GameObject[] visualMagazineDynamites;
    public int maxVisualDynamites = 3;
    public GameObject visualDynamite;
    public float visualDynamiteRenderOffset = 0.35f;
    
    public Vector3 dynamiteThrowOriginOffset;

    [FormerlySerializedAs("deltaExplosionTime")] public float deltaDetonationTime = 0.0f;
    [FormerlySerializedAs("minDeltaExplosionTime")] public float minDeltaDetonationTime = 0.5f;

    float xInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        visualMagazineDynamites = new GameObject[Math.Max(maxDynamiteMagazineCount, maxVisualDynamites)];
        SpawnMagazineDynamites();
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
            SpawnSingleMagazineDynamite(dynamiteMagazineCount);
            dynamiteMagazineCount = Math.Min(maxDynamiteMagazineCount, dynamiteMagazineCount + dynamiteRechargeCount);
            dynamiteMagazineRechargeTimer -= dynamiteMagazineRechargeRate;
        }
    }

    private void SpawnMagazineDynamites()
    {
        for (int i = 0; i < dynamiteMagazineCount; i++)
        {
            SpawnSingleMagazineDynamite(i);
        }
    }

    private void SpawnSingleMagazineDynamite(int index)
    {
        if (index > maxVisualDynamites - 1) return;

        int position = maxVisualDynamites - index;
        Quaternion spawnRotation = Quaternion.Euler(0,0,90);
        Vector2 spawnPosition = transform.position;
        spawnPosition.y += visualDynamiteRenderOffset - (visualDynamiteRenderOffset * position);
        visualMagazineDynamites[index] = Instantiate(visualDynamite, spawnPosition, spawnRotation);
        visualMagazineDynamites[index].transform.parent = transform;
        
    }

    private void DestroySingleMagazineDynamite(int index)
    {
        if (index > maxVisualDynamites) return;
        
        Destroy(visualMagazineDynamites[index]);
    }

    private void HandlePlayerDetonation()
    {
        // Minimal time between detonations
        deltaDetonationTime += Time.deltaTime;
        if (deltaDetonationTime < minDeltaDetonationTime) return;
        // To prevent potential overflow 
        deltaDetonationTime = minDeltaDetonationTime;
        
        if (activeDynamite == null) return;
        
        activeDynamiteTimer += Time.deltaTime;

        // If left click up not detected, do nothing
        if (!Input.GetKeyUp(KeyCode.Mouse0)) return;

        // To make sure you can throw multiple long dynamites without immediately exploding
        if (!(activeDynamiteTimer >= minMouseHoldToDetonateTime)) return;
        
        var dynamiteScript = activeDynamite.GetComponent<Dynamite>();
        if (dynamiteScript != null)
        {
            dynamiteScript.Explode();
            deltaDetonationTime = 0.0f;
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
        DestroySingleMagazineDynamite(dynamiteMagazineCount);
        
        GameObject newDynamite = Instantiate(dynamite, transform.position, transform.rotation);
        
        Rigidbody2D dynamiteRb = newDynamite.GetComponent<Rigidbody2D>();
        if (dynamiteRb != null)
        {
            
            dynamiteRb.velocity = rb.velocity;
            dynamiteRb.rotation = rb.rotation;
            
            var mousePosition = Input.mousePosition;
            mousePosition.z = 10.0f;
            if (Camera.main != null) mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
            var mouseDirection = mousePosition - gameObject.transform.position;
            mouseDirection.z = 0.0f;
            dynamiteRb.AddForce(mouseDirection * throwForce, ForceMode2D.Impulse);
            var impulse = ( Random.Range(-180f, 180f) * Mathf.Deg2Rad) * dynamiteRb.inertia;
            dynamiteRb.AddTorque(impulse * throwForce, ForceMode2D.Impulse);
            
            newDynamite.transform.Translate(dynamiteThrowOriginOffset);
        }

        activeDynamiteTimer = 0f;
        activeDynamite = newDynamite;
    }
}