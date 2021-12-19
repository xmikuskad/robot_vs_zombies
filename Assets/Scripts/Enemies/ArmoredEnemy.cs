using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoredEnemy : PlayerFollower, IEnemy
{
    [SerializeField]
    private int damage;

    [Header("Sounds")]
    [SerializeField]
    private AudioClip hitSound;
    [SerializeField]
    private AudioClip deathSound;

    new void Start()
    {
        base.Start();

    }

    void FixedUpdate()
    {
        FollowPlayer();
    }

    // Called as animator event
    public void DealDamage()
    {
        Vector2 playerPos = player.transform.position;
        if(Mathf.Abs(playerPos.x-transform.position.x) <= attackRange.x &&
            Mathf.Abs(playerPos.y-transform.position.y) <= attackRange.y)
        {
            // TODO deal damage
            player.GetComponent<Player>().HitForDamage(damage);
            AudioManager.Instance.PlayClip(hitSound, 3f);
        }
    }

    public void TakeExplosionDamage(int damage)
    {
        Stun();
        // Cannot take damage
    }

    public void TakeMeleeDamage(int damage)
    {
        // Cannot take damage
    }

    public void OnDeath()
    {
        animator.SetTrigger(Constants.AnimDeath);
        AudioManager.Instance.PlayClip(deathSound,3f);
    }

    // Called as animation event
    public void DestroyThis()
    {
        Destroy(this.gameObject);
    }

    public void OnSpawn(float mapHeight)
    {
        // nothing
    }
}
