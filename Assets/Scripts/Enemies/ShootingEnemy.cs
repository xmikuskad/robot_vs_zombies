using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemy : MonoBehaviour, IEnemy
{
    [SerializeField]
    private int health;
    [SerializeField]
    private int damage;
    [SerializeField]
    private int range;

    [SerializeField]
    private LayerMask platformLayerMask;
    [SerializeField]
    private LayerMask defaultLayerMask;
    [SerializeField]
    private LayerMask fallingLayerMask;

    private int ignoringPlatformCount;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.layer = GetLayerNumber(fallingLayerMask);
    }

    // Update is called once per frame
    void Update()
    {

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
        ignoringPlatformCount = Random.Range(0, Mathf.Max(0, hitColliders.Length - 2));
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
