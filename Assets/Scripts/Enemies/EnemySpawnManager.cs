using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using DG.Tweening;
using UnityEngine.UI;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField]
    private List<WaveInfo> waveInfos;

    [SerializeField]
    private float mapHeight;
    [SerializeField]
    private GameObject bossSpawnPosition;

    [Header("Object references")]
    [SerializeField]
    private List<GameObject> enemySpawners;
    // TODO
    [SerializeField]
    private TMP_Text waveText;
    [SerializeField]
    private TMP_Text timerText;
    [SerializeField]
    private Slider bossHp;

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

    private BossEnemy boss;

    [Header("Sounds")]
    [SerializeField]
    private AudioClip winSound;

    void Start()
    {
        waveIndex = -1;
        waveTimeLeft = 200f;
        loadingWave = true;
        NextWave();
        shouldSpawn = true;
        StartCoroutine(EnemySpawner());
    }

    void Update()
    {
        if(boss != null)
        {
            bossHp.value = boss.GetHealthRatio();
        }

        if (!shouldSpawn || loadingWave) return;

        waveTimeLeft -= Time.deltaTime;
        if (waveTimeLeft <= 0f)
        {
            loadingWave = true; // Prevent from firing multiple times
            NextWave();
            return;
        }

        timerText.text = GetTimeFromSeconds(waveTimeLeft);
    }

    private void NextWave()
    {
        Debug.Log("Next wave");
        waveIndex++;

        if (waveIndex >= waveInfos.Count)
        {
            return;
        }
        if (waveIndex >= waveInfos.Count)
        {
            timerText.gameObject.SetActive(false);
        }
        UpdateWaveText();
        actualWave = waveInfos[waveIndex];
        waveTimeLeft = actualWave.afterWaveTime;
        waveEnemies = actualWave.enemies;

        // Calculate all wave spawn times
        spawnTimes.Clear();
        foreach (EnemyType enemy in waveEnemies)
        {
            Debug.Log(enemy);
            float waitTime = UnityEngine.Random.Range(actualWave.minTimeBetweenSpawns, actualWave.maxTimeBetweenSpawns);
            spawnTimes.Add(waitTime);
            waveTimeLeft += waitTime;
        }
        waveEnemyIndex = 0;
        loadingWave = false;
    }

    private IEnumerator EnemySpawner()
    {
        while (shouldSpawn)
        {
            if (waveEnemies != null && waveEnemyIndex < waveEnemies.Count)
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
                yield return new WaitForSeconds(0.1f);
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
                SpawnEnemy(enemyType, GetSpawnPosition(enemyType, enemySpawner));
                return;
            }
        }

        Debug.LogError("Are both places in camera view?");
        SpawnEnemy(enemyType, GetSpawnPosition(enemyType, enemySpawners[0]));
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
            case EnemyType.Armored:
                return enemySpawner.transform.position;
            case EnemyType.Boss:
                return bossSpawnPosition.transform.position;
            case EnemyType.Ranged:
                Vector2 startEndVector = GetMapStartAndEnd();
                return new Vector2(UnityEngine.Random.Range(startEndVector.x, startEndVector.y), mapHeight);
        }
    }

    public void SpawnEnemy(EnemyType enemyType, Vector3 position)
    {
        GameObject spawnObj = GetEnemyPrefab(enemyType);
        GameObject spawnedEnemy = Instantiate(spawnObj, position, Quaternion.identity);
        if (spawnedEnemy.TryGetComponent<IEnemy>(out var enemy))
        {
            enemy.OnSpawn(mapHeight);
        }
        if (spawnedEnemy.TryGetComponent<BossEnemy>(out var bossEnemy))
        {
            boss = bossEnemy;
            bossHp.gameObject.SetActive(true);
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

    // Return time formatted as 00:00
    private string GetTimeFromSeconds(float timeInSeconds)
    {
        int mins = (int)((Mathf.Max(0,timeInSeconds)) / 60);
        String minsString = mins.ToString();
        if (minsString.Length <= 1)
        {
            minsString = "0" + minsString;
        }
        String seconds = string.Format("{0:F0}", Mathf.Min(59, timeInSeconds % 60));
        if (seconds.Length <= 1)
        {
            seconds = "0" + seconds;
        }
        return minsString + ":" + seconds;
    }

    // Makes a nice effect when changing text
    private void UpdateWaveText()
    {
        String newWaveText = (waveIndex + 1)==waveInfos.Count ? "Last wave!" : "Wave " + (waveIndex + 1) + "/" + waveInfos.Count;
        if(waveIndex == 0)  // Do not animate first wave
        {
            waveText.text = newWaveText;
            return;
        }
        DOTween.Sequence()
            .PrependCallback(() => waveText.text = newWaveText)
            .Insert(0, waveText.transform.DOPunchPosition(Vector3.up * 10, 1f, 4, 0.3f));
    }
}
