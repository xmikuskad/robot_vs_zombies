using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoredEnemy : PlayerFollower, IEnemy
{
    [SerializeField]
    private int damage;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FollowPlayer();
    }

    public void DealDamage()
    {
        Vector2 playerPos = player.transform.position;
        if(Mathf.Abs(playerPos.x-transform.position.x) <= attackRange.x &&
            Mathf.Abs(playerPos.y-transform.position.y) <= attackRange.y)
        {
            // TODO deal damage
        }
    }

    public int GetDamage()
    {
        return damage;
    }

    public void TakeDamage(int damage)
    {
        // Cannot take damage
    }

    public void OnDeath()
    {
        Destroy(this.gameObject);
    }

    public void OnSpawn(float mapHeight)
    {
        // TODO
    }
}
