using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class WalkingEnemy : PlayerFollower,IEnemy
{
    [SerializeField]
    private int health;
    [SerializeField]
    private int damage;

    [Header("Sounds")]
    [SerializeField]
    private AudioClip hitSound;
    [SerializeField]
    private AudioClip deathSound;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FollowPlayer();
    }

    public void DealDamage()
    {
        Vector2 playerPos = player.transform.position;
        if (Mathf.Abs(playerPos.x - transform.position.x) <= attackRange.x &&
            Mathf.Abs(playerPos.y - transform.position.y) <= attackRange.y)
        {
            // TODO deal damage
            player.GetComponent<Player>().HitForDamage(damage);
            AudioManager.Instance.PlayClip(hitSound, 3f);
        }
    }

    public int GetDamage()
    {
        return damage;
    }

    public void TakeDamage(int damage)
    {
        this.health -= damage;
        if(health <= 0)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        animator.SetTrigger(Constants.AnimDeath);
        AudioManager.Instance.PlayClip(deathSound, 3f);
    }

    public void DestroyThis()
    {
        Destroy(this.gameObject);
    }

    public void OnSpawn(float mapHeight)
    {
        // TODO
    }
}
