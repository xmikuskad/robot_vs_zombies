using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField]
    private List<WaveInfo> waveInfos;

    [SerializeField]
    private float mapHeight;

    [Header("Object references")]
    [SerializeField]
    private List<GameObject> enemySpawners;
    // TODO
    [SerializeField]
    private TMP_Text waveText;
    [SerializeField]
    private TMP_Text timerText;

    [Header("Enemy prefabs")]
    [SerializeField]
    private GameObject walkingEnemyPrefab;
    [SerializeField]
    private GameObject armoredEnemyPrefab;
    [SerializeField]
    private GameObject shootingEnemyPrefab;
    [SerializeField]
    private GameObject bossEnemyPrefab;

    private bool shouldSpawn = false;

    private int waveIndex = -1;
    [SerializeField]
    private float waveTimeLeft = 0f;
    private List<EnemyType> waveEnemies;
    private List<float> spawnTimes = new List<float>();
    private int waveEnemyIndex = 0;
    private WaveInfo actualWave;

    private bool loadingWave = false;

    void Start()
    {
        waveIndex = -1;
        waveTimeLeft = 200f;
        NextWave();
        shouldSpawn = true;
        StartCoroutine(EnemySpawner());
    }

    void Update()
    {
        if (!shouldSpawn || loadingWave) return;

        waveTimeLeft -= Time.deltaTime;
        if (waveTimeLeft <= 0f)
        {
            loadingWave = true; // Prevent from firing multiple times
            NextWave();
        }
    }

    private void NextWave()
    {
        Debug.Log("Next wave");
        waveIndex++;
        if (waveIndex >= waveInfos.Count)
        {
            // TODO game win!
            Debug.Log("GG!");
            shouldSpawn = false;
            return;
        }
        Debug.Log("HIR?");
        actualWave = waveInfos[waveIndex];
        waveTimeLeft = actualWave.afterWaveTime;
        waveEnemies = actualWave.enemies;

        // Calculate all wave spawn times
        spawnTimes.Clear();
        foreach (EnemyType enemy in waveEnemies)
        {
            float waitTime = Random.Range(actualWave.minTimeBetweenSpawns, actualWave.maxTimeBetweenSpawns);
            spawnTimes.Add(waitTime);
            waveTimeLeft += waitTime;
        }
        loadingWave = false;
    }

    private IEnumerator EnemySpawner()
    {
        while (shouldSpawn)
        {
            if (!loadingWave && waveEnemyIndex < waveEnemies.Count)
            {
                Debug.Log("Spawning " + (waveEnemyIndex + 1) + "/" + waveEnemies.Count);
                CallSpawnEnemy(waveEnemies[waveEnemyIndex]);
                float waitTime = spawnTimes[waveEnemyIndex];
                waveEnemyIndex++;
                yield return new WaitForSeconds(waitTime);
            }
            else
            {
                // Wait while new wave loads
                yield return new WaitForSeconds(waveTimeLeft > 2f ? waveTimeLeft : 0.5f);
            }
        }
    }

    // Check which spawner is not seen by camera and spawn enemy at his position
    private void CallSpawnEnemy(EnemyType enemyType)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        foreach (GameObject enemySpawner in enemySpawners)
        {
            if (!IsSpawnerVisibleToCamera(enemySpawner.transform.position, planes))
            {
                SpawnEnemy(GetEnemyPrefab(enemyType), GetSpawnPosition(enemyType, enemySpawner));
                return;
            }
        }

        Debug.LogError("Are both places in camera view?");
        SpawnEnemy(GetEnemyPrefab(enemyType), GetSpawnPosition(enemyType, enemySpawners[0]));
    }

    private bool IsSpawnerVisibleToCamera(Vector3 centerPos, Plane[] planes)
    {
        return GeometryUtility.TestPlanesAABB(planes, new Bounds(centerPos, new Vector3(0.1f, 0.1f, 0.1f)));
    }

    public GameObject GetEnemyPrefab(EnemyType enemyType)
    {
        switch (enemyType)
        {
            default:
                Debug.LogError("Enemy prefab not found!!");
                return walkingEnemyPrefab;
            case EnemyType.Walking:
                return walkingEnemyPrefab;
            case EnemyType.Armored:
                return armoredEnemyPrefab;
            case EnemyType.Ranged:
                return shootingEnemyPrefab;
            case EnemyType.Boss:
                return bossEnemyPrefab;
        }
    }

    public Vector2 GetSpawnPosition(EnemyType enemyType, GameObject enemySpawner)
    {
        switch (enemyType)
        {
            default:
                Debug.LogError("Enemy prefab not found!!");
                return enemySpawner.transform.position;
            case EnemyType.Walking:
                return enemySpawner.transform.position;
            case EnemyType.Armored:
                return enemySpawner.transform.position;
            case EnemyType.Boss:
                return enemySpawner.transform.position;
            case EnemyType.Ranged:
                Vector2 startEndVector = GetMapStartAndEnd();
                return new Vector2(Random.Range(startEndVector.x, startEndVector.y), mapHeight);
        }
    }

    public void SpawnEnemy(GameObject spawnObj, Vector3 position)
    {
        GameObject spawnedEnemy = Instantiate(spawnObj, position, Quaternion.identity);
        if (spawnedEnemy.TryGetComponent<IEnemy>(out var enemy))
        {
            enemy.OnSpawn(mapHeight);
        }
    }

    // x is start, y is end pos on x axis
    public Vector2 GetMapStartAndEnd()
    {
        Vector2 startEndVector = new Vector2(1000, -1000);

        foreach (GameObject enemySpawner in enemySpawners)
        {
            startEndVector.x = Mathf.Min(startEndVector.x, enemySpawner.transform.position.x);
            startEndVector.y = Mathf.Max(startEndVector.y, enemySpawner.transform.position.x);
        }

        return startEndVector;
    }
}
