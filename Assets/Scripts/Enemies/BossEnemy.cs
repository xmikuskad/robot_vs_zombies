using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class BossEnemy : MonoBehaviour, IEnemy
{
    [SerializeField]
    private int health;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float timeBetweenDynamites = 0.1f;
    [SerializeField]
    private float timeBetweenAttacks = 4f;
    [SerializeField]
    private float timeBetweenAttacksCounter = 0f;
    [SerializeField]
    private float dynamiteCountDuringAttack = 10;
    [SerializeField]
    private float minThrowForce = 3f;
    [SerializeField]
    private float maxThrowForce = 10f;
    [SerializeField]
    private float minCountdown = 0.5f;
    [SerializeField]
    private float maxCountdown = 2f;

    [SerializeField]
    private GameObject dynamitePrefab;
    [SerializeField]
    private GameObject dynamiteSpawnPoint;
    private Transform player;

    private SpriteRenderer sr;
    private Animator animator;

    [Header("Sounds")]
    [SerializeField]
    private AudioClip hitSound;
    [SerializeField]
    private AudioClip deathSound;


    [SerializeField]
    private float startDelay = 3f;
    private float delayCounter = 3f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(Constants.PlayerTag).transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(delayCounter > 0f)
        {
            delayCounter -= Time.deltaTime;
            return;
        }

        if(timeBetweenAttacksCounter >=0f)
        {
            timeBetweenAttacksCounter -= Time.deltaTime;
        }

        if (timeBetweenAttacksCounter < 0f)
        {
            timeBetweenAttacksCounter = timeBetweenAttacks;
            StartCoroutine(MassShoot());
        }

        float degrees = (this.gameObject.transform.position - player.position).x > 0 ? 180f : 0f;
        transform.rotation = Quaternion.Euler(new Vector2(0,1) * degrees);
    }

    private IEnumerator MassShoot()
    {
        for (int i = 0; i < dynamiteCountDuringAttack; i++)
        {
            float randomForce = Random.Range(minThrowForce, maxThrowForce);
            float randomCountdown = Random.Range(minCountdown, maxCountdown);
            Shoot(randomForce,randomCountdown);
            yield return new WaitForSeconds(timeBetweenDynamites);
        }
    }

    private void Shoot(float throwForce, float countdown)
    {
        timeBetweenAttacksCounter = timeBetweenAttacks;
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = Random.Range(0.5f, 1f);
        animator.SetTrigger(Constants.AnimFire);
        GameObject projectile = Instantiate(dynamitePrefab, dynamiteSpawnPoint.transform.position, Quaternion.identity);
        Rigidbody2D dynamiteRb = projectile.GetComponent<Rigidbody2D>();

        dynamiteRb.AddForce(dir * throwForce, ForceMode2D.Impulse);
        var impulse = (Random.Range(-180f, 180f) * Mathf.Deg2Rad) * dynamiteRb.inertia;
        dynamiteRb.AddTorque(impulse * throwForce, ForceMode2D.Impulse);

        if (projectile.TryGetComponent<Dynamite>(out var dynamite)) {
            dynamite.SetIsEvil(true);
            dynamite.SetCountdown(countdown);
        }

        // TODO animate
    }

    public int GetDamage()
    {
        return damage;
    }

    public void TakeExplosionDamage(int damage)
    {
        //Cannot take explosion damage
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

    public void DestroyThis()
    {
        Destroy(this.gameObject);
    }

    public void OnSpawn(float mapHeight)
    {
        delayCounter = startDelay;
    }

    public float GetHealthRatio()
    {
        // TODO return actHealth/maxHealth
        return 0.5f;
    }

}
