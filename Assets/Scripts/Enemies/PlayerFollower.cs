using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class PlayerFollower : MonoBehaviour
{
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private float platformCheckRange = 0.2f;
    [SerializeField]
    private bool isJumping = false;

    protected Rigidbody2D rb;
    private Transform player;
    private GroundChecker platformChecker;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag(Constants.PlayerTag).transform;
    }

    public void FollowPlayer()
    {
        if(isJumping)
        {
            return;
        }

        Vector3 playerPos = player.position;
        playerPos.y = transform.position.y;
        Vector3 dir = (playerPos - transform.position).normalized;
        rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
    }

    public void SetIsJumping(bool value)
    {
        isJumping = value;
    }
}
