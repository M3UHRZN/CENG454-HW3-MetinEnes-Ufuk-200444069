using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class EnemySpawnEntry
{
    public Enemy prefab;
    public int   count;
    public float spawnInterval;
}

[System.Serializable]
public class WaveConfig
{
    public EnemySpawnEntry[] enemies;
    public float             delayAfterWave;
}

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private WaveConfig[] waves;
    [SerializeField] private Transform[]  spawnPoints;
    [SerializeField] private float        countdownBeforeFirst = 3f;

    private int _aliveCount;

    private void OnEnable()  => GameEventBus.OnEnemyDied += OnEnemyDied;
    private void OnDisable() => GameEventBus.OnEnemyDied -= OnEnemyDied;

    private void Start()
    {
        if (waves == null || waves.Length == 0)
        {
            Debug.LogError("[WaveSpawner] No waves configured.");
            return;
        }
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("[WaveSpawner] No spawn points configured.");
            return;
        }
        StartCoroutine(RunWaves());
    }

    private IEnumerator RunWaves()
    {
        yield return new WaitForSeconds(countdownBeforeFirst);

        for (int i = 0; i < waves.Length; i++)
        {
            yield return StartCoroutine(SpawnWave(waves[i]));
            yield return new WaitUntil(() => _aliveCount <= 0);

            GameEventBus.RaiseWaveCompleted(i);

            if (i < waves.Length - 1)
                yield return new WaitForSeconds(waves[i].delayAfterWave);
        }

        GameEventBus.RaiseGameOver(true);
    }

    private IEnumerator SpawnWave(WaveConfig wave)
    {
        foreach (EnemySpawnEntry entry in wave.enemies)
        {
            for (int i = 0; i < entry.count; i++)
            {
                SpawnEnemy(entry.prefab);
                yield return new WaitForSeconds(entry.spawnInterval);
            }
        }
    }

    private void SpawnEnemy(Enemy prefab)
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Enemy enemy = EnemyPool.Instance.Get(prefab);

        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null && NavMesh.SamplePosition(spawnPoint.position, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            agent.Warp(hit.position);
        else
            enemy.transform.position = spawnPoint.position;

        enemy.transform.rotation = spawnPoint.rotation;
        _aliveCount++;
    }

    private void OnEnemyDied(int killCount, string killerName)
    {
        _aliveCount = Mathf.Max(0, _aliveCount - 1);
    }
}
