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
    [SerializeField]
    private float forcePower = 10f;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            SetIsJumping(true);
            rb.AddForce(new Vector2(1, 1) * forcePower, ForceMode2D.Impulse);
        }
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
