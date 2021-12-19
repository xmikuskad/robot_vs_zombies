using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    public int GetDamage();
    public void TakeExplosionDamage(int damage);

    public void TakeMeleeDamage(int damage);
    public void OnDeath();
    public void OnSpawn(float mapHeight);
}
