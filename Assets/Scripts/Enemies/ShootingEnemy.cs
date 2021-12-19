using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private SpriteRenderer sr;


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        this.gameObject.layer = GetLayerNumber(fallingLayerMask);
        player = GameObject.FindGameObjectWithTag(Constants.PlayerTag).transform;
    }

    // Update is called once per frame
    void Update()
    {
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
            projectileScript.InitializeProjectile(dir * speed);
        }

    }

    public int GetDamage()
    {
        return damage;
    }

    public void TakeDamage(int damage)
    {
        this.health -= damage;
        if (health <= 0)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        Destroy(this.gameObject);
    }

    public void OnSpawn(float mapHeight)
    {
        // Count number of platforms, choose random number which we will ignore so we end up on random "floor"
        Vector2 endPos = transform.position;
        endPos.y -= mapHeight;
        RaycastHit2D[] hitColliders = Physics2D.LinecastAll(transform.position, endPos, platformLayerMask);
        ignoringPlatformCount = Random.Range(0, Mathf.Max(0, hitColliders.Length - 2)); // -2 because of the top and bottom platform
    }

    private int GetLayerNumber(LayerMask mask)
    {
        return (int)(Mathf.Log((uint)mask.value, 2));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals(Constants.PlatformTag) || collision.tag.Equals(Constants.PlatformTriggerTag))
        {
            ignoringPlatformCount--;
            if(ignoringPlatformCount <=0)
            {
                this.gameObject.layer = GetLayerNumber(defaultLayerMask);
            }
        }
    }
}
