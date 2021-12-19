using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public abstract class PlayerFollower : MonoBehaviour
{
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float platformCheckRange = 0.2f;
    [SerializeField]
    private bool isFlying = false;

    protected Rigidbody2D rb;   // Needs to be protected because of inheritance
    protected Animator animator;   // Needs to be protected because of inheritance
    protected SpriteRenderer sr;   // Needs to be protected because of inheritance
    protected Transform player;

    [SerializeField]
    private LayerMask platformTriggerMask;
    [SerializeField]
    private float triggerCheckRange = 15f;

    [SerializeField]
    private LayerMask defaultMask;
    [SerializeField]
    private LayerMask jumpingLayerMask;

    [SerializeField]
    private float jumpPowerPerUnit = 10f;
    [SerializeField]
    private float jumpTimePerUnit= 1f;

    [SerializeField]
    private float minPlatformHeight = 1.5f;

    [SerializeField]
    private float waitAfterJump = 0.5f;
    [SerializeField]
    private float waitTimeCounter = 0f;

    [SerializeField]
    private float jumpDownWait = 0.3f;

    [SerializeField]
    protected Vector2 attackRange = new Vector2(1f, 0.5f); // Needs to be protected because of inheritance

    [SerializeField]
    private float timeBetweenAttacks = 1f;
    private float timeBetweenAttacksCounter = 0f;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag(Constants.PlayerTag).transform;
    }

    public void FollowPlayer()
    {
        if(timeBetweenAttacksCounter >= 0f)
        {
            timeBetweenAttacksCounter -= Time.deltaTime;
        }

        if (waitTimeCounter >= 0f)
        {
            waitTimeCounter -= Time.deltaTime;
        }
        else
        {
            if (isFlying)
            {
                animator.SetBool(Constants.AnimRunning, false);
                return;
            }
            // TODO make a delay for this?
            // Should we jump up/down ?
            if (Mathf.Abs(this.transform.position.y - player.transform.position.y) > minPlatformHeight)
            {
                if (!TryToJump(true))
                    return;
            }
        }

        if (isFlying)
        {
            animator.SetBool(Constants.AnimRunning, false);
            return;
        }

        // Attack range check
        if (Mathf.Abs(player.position.x - this.transform.position.x) < attackRange.x &&
            Mathf.Abs(player.position.y - this.transform.position.y) < attackRange.y && 
            timeBetweenAttacksCounter < 0f)
        {
            // TODO ATTACK
            animator.SetTrigger(Constants.AnimAttack);
            timeBetweenAttacksCounter = timeBetweenAttacks;
            return;
        }

        // Move enemy
        if (Mathf.Abs(player.position.x - this.transform.position.x) < attackRange.x) return;   // prevent bugging on one place
        Vector3 playerPos = player.position;
        playerPos.y = transform.position.y;
        Vector3 dir = (playerPos - transform.position).normalized;
        rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);

        // Process animation and sprite changes
        animator.SetBool(Constants.AnimRunning, true);
        sr.flipX = dir.x < 0f;
    }

    public void SetIsFlying(bool value)
    {
        isFlying = value;
    }

    public bool TryToJump(bool canJumpDown)
    {
        // Do not jump if in air or on cooldown
        if (isFlying || waitTimeCounter > 0f)
        {
            return false;
        }
        waitTimeCounter = 1f;    // To prevent this function from going off multiple times
        bool shouldBeLower = this.gameObject.transform.position.y - player.transform.position.y > 0;
        bool shouldBeRight = this.gameObject.transform.position.x - player.transform.position.x > 0;

        // Check if player is under us. If he is then jump down
        if (canJumpDown && shouldBeLower)
        {
            StartCoroutine(JumpDown());
            return false;
        }

        // Get all platforms in range
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, triggerCheckRange, platformTriggerMask);
        if (colliders.Length == 0)
        {
            Debug.LogWarning("Couldnt find position to jump to");
            waitTimeCounter = waitAfterJump*2;  // Allow him to move to other position
            return false;
        }

        // Try to find perfect positions
        foreach (Collider2D collider in colliders)
        {
            float widthDistance = this.gameObject.transform.position.x - collider.gameObject.transform.position.x;  // Used to prevent "jumping" on the same place
            bool isLower = this.gameObject.transform.position.y - collider.gameObject.transform.position.y > 0;
            bool isRight = widthDistance > 0;
            if (Mathf.Abs(widthDistance)>1f && isLower == shouldBeLower && isRight == shouldBeRight)    // Perfect situation
            {
                // Calculate random pos for jump
                SpriteRenderer parentRenderer = collider.gameObject.GetComponentInParent<SpriteRenderer>(); // Fix around GetComponentInParent<Transform>() returning same Transform
                if (parentRenderer != null) {
                    Vector3 destPos = collider.transform.position;
                    float halfSize = (parentRenderer.transform.localScale.x / 2f) - 1f;
                    destPos.x = destPos.x + Random.Range(-halfSize, halfSize);
                    MakeJump(destPos);
                    return true;
                }
            }
        }
        Debug.LogWarning("No position to jump found!?");
        return false;
    }

    private void OnDrawGizmos()
    {
        // Range of checking the jump
        Gizmos.DrawWireSphere(transform.position, triggerCheckRange);
    }

    private int GetLayerNumber(LayerMask mask)
    {
        return (int)(Mathf.Log((uint)mask.value, 2));
    }

    private IEnumerator JumpDown()
    {
        waitTimeCounter = waitAfterJump+jumpDownWait;
        this.gameObject.layer = GetLayerNumber(jumpingLayerMask);
        yield return new WaitForSeconds(jumpDownWait);
        this.gameObject.layer = GetLayerNumber(defaultMask);
        yield return null;
    }

    private void MakeJump(Vector3 position)
    {
        float distance = Vector2.Distance(this.gameObject.transform.position, position);
        position.y += 0.5f; // Move to the top of the boxCollider
        // Make the jump
        DOTween.Sequence()
            .PrependCallback(() =>
            {
                waitTimeCounter += jumpTimePerUnit * distance;
                this.gameObject.layer = GetLayerNumber(jumpingLayerMask);
            })
            .Insert(0, rb.DOJump(position, jumpPowerPerUnit*distance, 1, jumpTimePerUnit * distance))
            .AppendCallback(() =>
            {
                this.gameObject.layer = GetLayerNumber(defaultMask);
                waitTimeCounter = waitAfterJump;
            });
    }
}
