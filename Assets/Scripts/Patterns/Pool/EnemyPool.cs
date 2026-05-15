using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public struct EnemyPoolEntry
{
    public Enemy prefab;
    public int   defaultCapacity;
    public int   maxSize;
}

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance { get; private set; }

    [SerializeField] private EnemyPoolEntry[] entries;

    private ObjectPool<Enemy>[] _pools;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _pools = new ObjectPool<Enemy>[entries.Length];
        for (int i = 0; i < entries.Length; i++)
        {
            int            index = i;
            EnemyPoolEntry entry = entries[i];
            _pools[i] = new ObjectPool<Enemy>(
                createFunc:      () => CreateEnemy(entry.prefab, index),
                actionOnGet:     e  => OnGetEnemy(e, index),
                actionOnRelease: e  => e.gameObject.SetActive(false),
                actionOnDestroy: e  => Destroy(e.gameObject),
                collectionCheck: true,
                defaultCapacity: entry.defaultCapacity,
                maxSize:         entry.maxSize
            );
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public int GetTypeIndex(Enemy prefab)
    {
        if (prefab == null) return 0;
        for (int i = 0; i < entries.Length; i++)
            if (entries[i].prefab == prefab) return i;

        Debug.LogWarning($"[EnemyPool] Prefab '{prefab.name}' not found in entries. Using index 0.");
        return 0;
    }

    public Enemy Get(int typeIndex)
    {
        return _pools[typeIndex].Get();
    }

    public Enemy Get(Enemy prefab)
        => Get(GetTypeIndex(prefab));

    public void Return(Enemy enemy)
    {
        enemy.OnReturn();
        _pools[enemy.TypeIndex].Release(enemy);
    }

    private Enemy CreateEnemy(Enemy prefab, int typeIndex)
    {
        Enemy e = Instantiate(prefab, transform);
        e.SetTypeIndex(typeIndex);
        return e;
    }

    private void OnGetEnemy(Enemy e, int typeIndex)
    {
        e.SetTypeIndex(typeIndex);
        e.OnSpawn();
    }
}
