using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class ShootingEnemy : MonoBehaviour, IEnemy
{
    [SerializeField]
    private int health;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float range;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float timeBetweenAttacks = 2f;
    [SerializeField]
    private float timeBetweenAttacksCounter = 0f;

    [Header("References")]
    [SerializeField]
    private GameObject zombieProjectile;
    private Transform player;

    [SerializeField]
    private LayerMask platformLayerMask;
    [SerializeField]
    private LayerMask defaultLayerMask;
    [SerializeField]
    private LayerMask fallingLayerMask;

    private int ignoringPlatformCount;
    private bool canShoot = false;

    private SpriteRenderer sr;
    private Animator animator;

    [Header("Sounds")]
    [SerializeField]
    private AudioClip hitSound;
    [SerializeField]
    private AudioClip deathSound;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag(Constants.PlayerTag).transform;
    }

    void Start()
    {
        this.gameObject.layer = GetLayerNumber(fallingLayerMask);
    }

    void Update()
    {
        if (!canShoot) return;
        if(timeBetweenAttacksCounter >=0f)
        {
            timeBetweenAttacksCounter -= Time.deltaTime;
        }

        float distance = Vector2.Distance(this.gameObject.transform.position, player.position);
        if (timeBetweenAttacksCounter < 0f && distance < range)
        {
            Shoot();
        }

        sr.flipX = (this.gameObject.transform.position - player.position).x > 0;
    }

    private void Shoot()
    {
        timeBetweenAttacksCounter = timeBetweenAttacks;
        Vector3 dir = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(zombieProjectile, transform.position, Quaternion.identity);
        if (dir.x < 0) projectile.GetComponent<SpriteRenderer>().flipX = true;
        if(projectile.TryGetComponent<ZombieProjectile>(out var projectileScript)) {
            projectileScript.InitializeProjectile(dir * speed, hitSound);
        }

    }

    public void TakeExplosionDamage(int damage)
    {
        this.health -= damage;
        if (health <= 0)
        {
            OnDeath();
        }
    }

    public void TakeMeleeDamage(int damage)
    {
        this.health -= damage;
        if (health <= 0)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        animator.SetTrigger(Constants.AnimDeath);
        AudioManager.Instance.PlayClip(deathSound, 3f);
    }

    // Called from animation event
    public void DestroyThis()
    {
        Destroy(this.gameObject);
    }

    public void OnSpawn(float mapHeight)
    {
        // Count number of platforms, choose random number which we will ignore so we end up on random "floor"
        Vector2 endPos = transform.position;
        endPos.y -= mapHeight;
        RaycastHit2D[] hitColliders = Physics2D.LinecastAll(transform.position, endPos, platformLayerMask);
        ignoringPlatformCount = Random.Range(0, Mathf.Max(0, hitColliders.Length - 1)); // -1 because of the top platform
    }

    // Convert layermask to layer number
    private int GetLayerNumber(LayerMask mask)
    {
        return (int)(Mathf.Log((uint)mask.value, 2));
    }

    // Used when falling on spawn
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals(Constants.PlatformTag) || collision.tag.Equals(Constants.PlatformTriggerTag))
        {
            ignoringPlatformCount--;
            if(ignoringPlatformCount <=0)
            {
                this.gameObject.layer = GetLayerNumber(defaultLayerMask);
                canShoot = true;
            }
        }
    }
}
