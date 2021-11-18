using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Serialization;

public class ExplosionEffect : MonoBehaviour
{
    public float destroyAfter = 1.0f;
    private float countdown;
    
    // Start is called before the first frame update
    void Start()
    {
        countdown = destroyAfter;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
