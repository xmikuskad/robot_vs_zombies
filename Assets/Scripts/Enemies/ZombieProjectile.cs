using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class ZombieProjectile : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void InitializeProjectile(Vector2 velocity)
    {
        rb.velocity = velocity;
        Destroy(this.gameObject, 10f);  // In case this does not hit anything
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals(Constants.PlatformTag))
        {
            MakeSplash();
        }
        if (collision.tag.Equals(Constants.PlayerTag))
        {
            // TODO deal damage
            MakeSplash();
        }
    }

    public void MakeSplash()
    {
        animator.SetTrigger(Constants.AnimMakeSplash);
    }

    public void DestroyThis() {
        Destroy(this.gameObject);
    }

}
