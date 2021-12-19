using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    public int GetDamage();
    public void TakeDamage(int damage);
    public void OnDeath();
    public void OnSpawn(float mapHeight);
}
