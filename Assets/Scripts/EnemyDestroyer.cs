using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyDestroyer : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.tag.Equals(Constants.EnemyTag))
        {
            Destroy(col.gameObject);
        }
        
        if(col.gameObject.tag.Equals(Constants.PlayerTag))
        {
            col.gameObject.GetComponent<Player>().HitForDamage(1);
            col.gameObject.GetComponent<Player>().HitForDamage(1);
            col.gameObject.GetComponent<Player>().HitForDamage(1);
        }
    }
}
