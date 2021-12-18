using System.Collections;
using System.Collections.Generic;
using Extensions;
using UnityEngine;

public class Dynamite : MonoBehaviour
{
    public float delay = 3f;
    public float blastRadius = 5f;
    public float blastForce = 5f;
    public float upwardsModifier = 0.1f;

    private float countdown;

    public GameObject explosionEffect;
    private bool hasExploded = false;

    public float speed = 4f;

    public Vector3 launchOffset;

    public bool thrown;
    
    // Start is called before the first frame update
    void Start()
    {
        if (thrown)
        {
            var direction = -transform.right + Vector3.up;
            GetComponent<Rigidbody2D>().AddForce(direction * speed, ForceMode2D.Impulse);
        }
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

    void Explode()
    {
        hasExploded = true;
        // Show effect

        var localTransform = transform; // for performance purposes (IDE says its better, so i guess i'll believe it)
        var rotation = localTransform.rotation;
        var position = localTransform.position;
        Instantiate(explosionEffect, position, rotation);

        var colliders = Physics2D.OverlapCircleAll(position, blastRadius);

        foreach (var nearbyObject in colliders)
        {
            var rb = nearbyObject.GetComponent<Rigidbody2D>();
            if (rb == null) continue; // If object without rigidbody, skip to the next one

            var explosionDir = rb.position - (Vector2) transform.position;
            var explosionDistance = explosionDir.magnitude;
            const ForceMode2D mode = ForceMode2D.Impulse;

            if (upwardsModifier == 0)
                explosionDir /= explosionDistance;
            else
            {
                explosionDir.y += upwardsModifier;
                explosionDir.Normalize();
            }
                
            rb.AddForce(Mathf.Lerp(0, blastForce, (1 - explosionDistance)) * explosionDir, mode);
            
            rb.velocity = new Vector2(rb.velocity.x, blastForce);
        }


            // Get nearby objects
            // Add Force
            // Damage

        //Remove dynamite
        Destroy(gameObject);
    }
}
