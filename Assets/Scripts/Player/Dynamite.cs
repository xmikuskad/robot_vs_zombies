using System;
using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


public class Dynamite : MonoBehaviour
{
    public float delay = 3f;
    public float blastRadius = 5f;
    public float blastForce = 5f;
    public float upwardsModifier = 0.1f;
    public float minimalExplosionDistance = 0.1f;

    private float countdown;
    private Rigidbody2D rb;

    public GameObject explosionEffect;
    private bool hasExploded = false;

    [FormerlySerializedAs("speed")] public float initialThrowForce = 4f;

    public Vector3 launchOffset;

    public bool thrown = false;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
       
        transform.Translate(launchOffset);
        
        countdown = delay;

    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded)
        {
            Explode();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }

    // Method is public so that player can call it on command
    // Partially inspired by https://stackoverflow.com/a/66453571
    public void Explode()
    {
        hasExploded = true;
        // Show effect
    
        var localTransform = transform; // for performance purposes (IDE says its better, so i guess i'll believe it)
        var rotation = localTransform.rotation;
        var position = localTransform.position;
        
        // Create visual explosion
        Instantiate(explosionEffect, position, rotation);
    
        // Get nearby objects
        var colliders = Physics2D.OverlapCircleAll(position, blastRadius);
    
        foreach (var nearbyObject in colliders)
        {
            
            // Add Calculated Force
            var rb = nearbyObject.GetComponent<Rigidbody2D>();
            if (rb == null) continue; // If object without rigidbody, skip to the next one
    
            var explosionDir = rb.position - (Vector2) transform.position;
            
            // In case dynamite is inside the object
            var explosionDistance = Mathf.Max(explosionDir.magnitude, minimalExplosionDistance);
            explosionDistance /= 8; // for better reach
            
            // var explosionDistance = explosionDir.magnitude;
            const ForceMode2D mode = ForceMode2D.Impulse;

            if (upwardsModifier <= 0.01f)
                explosionDir /= explosionDistance;
            else
            {
                explosionDir.y += upwardsModifier;
                explosionDir.Normalize();
            }
            
            rb.AddForce(Mathf.Lerp(0, blastForce, (1 - explosionDistance)) * explosionDir, mode);
       
            // Damage
        }
    
        //Remove dynamite
        Destroy(gameObject);
    }
}
