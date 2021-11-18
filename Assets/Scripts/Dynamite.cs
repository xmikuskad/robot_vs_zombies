using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamite : MonoBehaviour
{
    public float delay = 3f;
    public float blastRadius = 5f;
    public float blastForce = 50f;

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
            if (rb != null)
            {
                var explosionDir = rb.position - (Vector2) transform.position;
                var explosionDistance = explosionDir.magnitude;
                ForceMode2D mode = ForceMode2D.Force;
                float upwardsModifier = 0.0f;
                
                if (upwardsModifier == 0)
                    explosionDir /= explosionDistance;
                else
                {
                    explosionDir.y += upwardsModifier;
                    explosionDir.Normalize();
                }
                
                rb.AddForce(Mathf.Lerp(0, blastForce, (1 - explosionDistance)) * explosionDir, mode);
                
                // rb.AddExplosionForce(blastForce, transform.position, blastRadius);
                // rb.velocity = new Vector2(rb.velocity.x, blastForce);
            }
        }


            // Get nearby objects
            // Add Force
            // Damage

        //Remove dynamite
        Destroy(gameObject);
    }
}
