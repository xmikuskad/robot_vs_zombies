using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 2.0f;
    public int meleeDamage = 2;

    public float attackCooldown = .4f;
    public float attackCooldownTimer = 0f;

    public LayerMask enemyMask;

    public GameObject blastAttackEffect;
    public GameObject meleeAttackIndicator;
    public float blastEffectXOffset = 0.5f;

    public Transform rightAttackPosition;
    public Transform leftAttackPosition;

    private float meleeAttackIndicatorMaxWidth;

    // Start is called before the first frame update
    void Start()
    {
        meleeAttackIndicatorMaxWidth = meleeAttackIndicator.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (attackCooldownTimer >= attackCooldown)
        {
            
            attackCooldownTimer = attackCooldown;
            
            if (Input.GetKeyDown(KeyCode.A))
            {
                var attackPosition = transform.position;
                attackPosition.z = 1f;
                attackPosition.x -= blastEffectXOffset;
                Quaternion spawnRotation = Quaternion.Euler(0,0,180);
                GameObject attackAnimation = Instantiate(blastAttackEffect, attackPosition, spawnRotation);
                attackAnimation.transform.parent = transform;
                MeleeAttack(leftAttackPosition);
                
                attackCooldownTimer = 0.01f;
            }
        
            if (Input.GetKeyDown(KeyCode.D))
            {
                var attackPosition = transform.position;
                attackPosition.z = 1f;
                attackPosition.x += blastEffectXOffset;
                Quaternion spawnRotation = Quaternion.Euler(0,0,0);
                GameObject attackAnimation = Instantiate(blastAttackEffect, attackPosition, spawnRotation);
                attackAnimation.transform.parent = transform;
                MeleeAttack(rightAttackPosition);

                attackCooldownTimer = 0.01f;
            }
        }
        else
        {
            attackCooldownTimer += Time.deltaTime;
        }

        var newIndicatorScale = meleeAttackIndicator.transform.localScale;
        if (attackCooldownTimer != 0)
        {
            newIndicatorScale.x = meleeAttackIndicatorMaxWidth / ((attackCooldown / attackCooldownTimer) + 0.01f);
        }
        else
        {
            newIndicatorScale.x = 0;
        }
        
        meleeAttackIndicator.transform.localScale = newIndicatorScale;
    }
    
    void MeleeAttack(Transform attackPosition)
    {
        var colliders = Physics2D.OverlapCircleAll(attackPosition.position, attackRange, enemyMask);
        foreach (var nearbyObject in colliders)
        {
            var enemy = nearbyObject.GetComponent<IEnemy>();
            if (enemy == null) continue;
            
            enemy.TakeMeleeDamage(meleeDamage);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(leftAttackPosition.position, attackRange);
        Gizmos.DrawWireSphere(rightAttackPosition.position, attackRange);
    }
}
