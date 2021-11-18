using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

public class Dynamite : MonoBehaviour
{
    public float delay = 3f;
    public float blastRadius = 5f;
    public float blastForce = 50f;
    public float upwardsModifier = 0.0f;

    private float countdown;

    public GameObject explosionEffect;

    private bool hasExploded = false;
    // Start is called before the first frame update
    void Start()
    {
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

    void Explode()
    {
        hasExploded = true;
        // Show effect
 
        Instantiate(explosionEffect, transform.position, transform.rotation);

        var colliders = Physics2D.OverlapCircleAll(transform.position, blastRadius);

        foreach (var nearbyObject in colliders)
        {
            Rigidbody2D rb = nearbyObject.GetComponent<Rigidbody2D>();
            if (rb == null) continue; // If object without rigidbody, skip to the next one
            
            rb.AddExplosionForce(blastForce, transform.position, blastRadius, upwardsModifier, ForceMode2D.Impulse);


            // var explosionDir = rb.position - (Vector2) transform.position;
            // var explosionDistance = explosionDir.magnitude;
            // ForceMode2D mode = ForceMode2D.Impulse;
            // float upwardsModifier = 0.2f;
            //     
            // if (upwardsModifier == 0)
            //     explosionDir /= explosionDistance;
            // else
            // {
            //     explosionDir.y += upwardsModifier;
            //     explosionDir.Normalize();
            // }
            //     
            // rb.AddForce(Mathf.Lerp(0, blastForce, (1 - explosionDistance)) * explosionDir, mode);
            //     
            // rb.AddExplosionForce(blastForce, transform.position, blastRadius);
            // rb.velocity = new Vector2(rb.velocity.x, blastForce);
        }


            // Get nearby objects
            // Add Force
            // Damage

        //Remove dynamite
        Destroy(gameObject);
    }
}
