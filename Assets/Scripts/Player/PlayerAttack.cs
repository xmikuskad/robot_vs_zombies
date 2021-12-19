using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 2.0f;
    public int meleeDamage = 2;
    enum AttackDirection {Left, Right}
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            MeleeAttack(AttackDirection.Left);
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            MeleeAttack(AttackDirection.Right);
        }
    }
    
    void MeleeAttack(AttackDirection attackDirection)
    {
        Vector2 playerPos = transform.position;
        var colliders = Physics2D.OverlapCircleAll(playerPos, attackRange);
        foreach (var nearbyObject in colliders)
        {
            var enemy = nearbyObject.GetComponent<IEnemy>();
            if (enemy == null) continue;
 
            if (attackDirection == AttackDirection.Left)
            {
                if (nearbyObject.transform.position.x < transform.position.x)
                {
                    enemy.TakeDamage(meleeDamage);
                }
            }
            else // attackDirection == Right
            {
                if (nearbyObject.transform.position.x > transform.position.x)
                {
                    enemy.TakeDamage(meleeDamage);
                }
            }
            
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
