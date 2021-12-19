using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class ZombieProjectile : MonoBehaviour
{
    public int damage = 1;
    
    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;

    private AudioClip hitSound;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag(Constants.PlayerTag).transform;
    }

    public void InitializeProjectile(Vector2 velocity, AudioClip hitSound)
    {
        rb.velocity = velocity;
        this.hitSound = hitSound;
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
            player.GetComponent<Player>().HitForDamage(damage);
            AudioManager.Instance.PlayClip(hitSound, 3f);
            MakeSplash();
        }
    }

    public void MakeSplash()
    {
        animator.SetTrigger(Constants.AnimMakeSplash);
    }

    // Called from animation event
    public void DestroyThis() {
        Destroy(this.gameObject);
    }

}
