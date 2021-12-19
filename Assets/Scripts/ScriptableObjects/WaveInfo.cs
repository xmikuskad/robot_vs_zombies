using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Walking,Armored,Ranged, Boss
}

[CreateAssetMenu(fileName = "Wave", menuName = "Waves/WaveScriptableObject", order = 1)]
public class WaveInfo : ScriptableObject
{
    public List<EnemyType> enemies;
    public float minTimeBetweenSpawns = 2f;
    public float maxTimeBetweenSpawns = 4f;
    public float afterWaveTime = 10;
}
