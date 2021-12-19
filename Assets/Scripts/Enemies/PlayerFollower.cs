using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class PlayerFollower : MonoBehaviour
{
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float platformCheckRange = 0.2f;
    [SerializeField]
    private bool isJumping = false;
    [SerializeField]
    private bool isFlying = false;

    protected Rigidbody2D rb;
    private Transform player;
    private GroundChecker platformChecker;

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
    private float chillAfterJump = 0.5f;
    private float chillCounter = 0f;

    [SerializeField]
    private float jumpDownWait = 0.3f;

    [SerializeField]
    private Vector2 attackRange = new Vector2(1f, 0.5f);

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag(Constants.PlayerTag).transform;
    }

    public void FollowPlayer()
    {
        if (chillCounter > 0f)
        {
            chillCounter -= Time.deltaTime;
        }
        else
        {

            if (isJumping || isFlying)
            {
                return;
            }

            // Should we jump up/down ?
            if (Mathf.Abs(this.transform.position.y - player.transform.position.y) > minPlatformHeight)
            {
                if (!TryToJump(true))
                    return;
            }
        }

        if (isJumping || isFlying)
        {
            return;
        }

        // Can we attack ?
        if (Mathf.Abs(player.position.x - this.transform.position.x) < attackRange.x &&
            Mathf.Abs(player.position.y - this.transform.position.y) < attackRange.y)
        {
            // TODO ATTACK
            return;
        }

        // Move enemy
        if (Mathf.Abs(player.position.x - this.transform.position.x) < attackRange.x) return;   // prevent bugging on one place
        Vector3 playerPos = player.position;
        playerPos.y = transform.position.y;
        Vector3 dir = (playerPos - transform.position).normalized;
        rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
    }

    public void SetIsFlying(bool value)
    {
        isFlying = value;
    }

    public bool TryToJump(bool canJumpDown)
    {
        // Do not jump if in air or on cooldown
        if (isFlying || isJumping || chillCounter > 0f)
        {
            return false;
        }  
        isJumping = true;    // To prevent this function from going off multiple times
        bool shouldBeLower = this.gameObject.transform.position.y - player.transform.position.y > 0;
        bool shouldBeRight = this.gameObject.transform.position.x - player.transform.position.x > 0;

        if (canJumpDown && shouldBeLower)
        {
            StartCoroutine(JumpDown());
            return false;
        }

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, triggerCheckRange, platformTriggerMask);
        if (colliders.Length == 0)
        {
            Debug.LogWarning("Couldnt find position to jump to");
            chillCounter = chillAfterJump * 2;  // Allow him to move
            isJumping = false;
            return false;
        }

        // Try to find perfect positions
        foreach (Collider2D collider in colliders)
        {
            bool isLower = this.gameObject.transform.position.y - collider.gameObject.transform.position.y > 0;
            bool isRight = this.gameObject.transform.position.x - collider.gameObject.transform.position.x > 0;
            if (isLower == shouldBeLower && isRight == shouldBeRight)    // Perfect situation
            {
                // Calculate random pos for jump
                if (collider.TryGetComponent<BoxCollider2D>(out BoxCollider2D boxCol)) {
                    Vector3 destPos = collider.transform.position;
                    float halfSize = (boxCol.size.x / 2f) - 0.2f;
                    destPos.x = destPos.x + Random.Range(-halfSize, halfSize);
                    MakeJump(destPos);
                    return true;
                }
            }
        }
        Debug.LogWarning("Nothing??");
        isJumping = false;

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
        chillCounter = chillAfterJump+jumpDownWait;
        isJumping = false;
        this.gameObject.layer = GetLayerNumber(jumpingLayerMask);
        yield return new WaitForSeconds(jumpDownWait);
        this.gameObject.layer = GetLayerNumber(defaultMask);
        yield return null;
    }

    private void MakeJump(Vector3 position)
    {
        float distance = Vector2.Distance(this.gameObject.transform.position, position);
        // Make the jump
        DOTween.Sequence()
            .PrependCallback(() =>
            {
                this.gameObject.layer = GetLayerNumber(jumpingLayerMask);
            })
            .Insert(0, rb.DOJump(position, jumpPowerPerUnit*distance, 1, jumpTimePerUnit * distance))
            //.AppendInterval((jumpTimePerUnit * distance) / 3)
            .AppendCallback(() =>
            {
                this.gameObject.layer = GetLayerNumber(defaultMask);
                isJumping = false;
                chillCounter = chillAfterJump;
            });
    }
}
