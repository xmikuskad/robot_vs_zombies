using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Serialization;

public class ExplosionEffect : MonoBehaviour
{
    public float destroyAfter = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyAfter);
    }

    // Update is called once per frame
    void Update()
    {
     
    }
}
