using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingEnemy : PlayerFollower,IEnemy
{
    [SerializeField]
    private int health;

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
        Destroy(this.gameObject);
    }
}
