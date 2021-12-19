using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 2.0f;
    public int meleeDamage = 2;

    public float attackCooldown = .4f;
    public float attackCooldownTimer = 0f;

    public LayerMask enemyMask;

    public Transform rightAttackPosition;
    public Transform leftAttackPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (attackCooldownTimer >= attackCooldown)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                MeleeAttack(leftAttackPosition);
            }
        
            if (Input.GetKeyDown(KeyCode.D))
            {
                MeleeAttack(rightAttackPosition);
            }

            attackCooldownTimer = attackCooldown;
        }
        else
        {
            attackCooldownTimer += Time.deltaTime;
        }
    }
    
    void MeleeAttack(Transform attackPosition)
    {
        var colliders = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, enemyMask);
        foreach (var nearbyObject in colliders)
        {
            var enemy = nearbyObject.GetComponent<IEnemy>();
            if (enemy == null) continue;
            
            enemy.TakeDamage(meleeDamage);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(leftAttackPosition.position, attackRange);
        Gizmos.DrawWireSphere(rightAttackPosition.position, attackRange);
    }
}
