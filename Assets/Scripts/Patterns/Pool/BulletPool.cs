using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public struct BulletPoolEntry
{
    public Bullet prefab;
    public int    defaultCapacity;
    public int    maxSize;
}

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    [SerializeField] private BulletPoolEntry[] entries;

    private ObjectPool<Bullet>[] _pools;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _pools = new ObjectPool<Bullet>[entries.Length];
        for (int i = 0; i < entries.Length; i++)
        {
            int             index = i;
            BulletPoolEntry entry = entries[i];
            _pools[i] = new ObjectPool<Bullet>(
                createFunc:      () => CreateBullet(entry.prefab, index),
                actionOnGet:     _ => { },
                actionOnRelease: b => b.gameObject.SetActive(false),
                actionOnDestroy: b => Destroy(b.gameObject),
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

    public int GetTypeIndex(Bullet prefab)
    {
        for (int i = 0; i < entries.Length; i++)
            if (entries[i].prefab == prefab) return i;

        Debug.LogWarning($"[BulletPool] Prefab '{prefab?.name}' not found in entries. Using index 0.");
        return 0;
    }

    public void Launch(Vector3 origin, Vector3 direction, string shooterName, Bullet prefab)
        => Launch(origin, direction, shooterName, GetTypeIndex(prefab));

    public void Launch(Vector3 origin, Vector3 direction, string shooterName = "Unknown", int typeIndex = 0)
    {
        Bullet bullet = _pools[typeIndex].Get();
        bullet.ShooterName = shooterName;
        bullet.transform.SetPositionAndRotation(origin, Quaternion.LookRotation(direction));
        bullet.OnSpawn();
    }

    public void ReturnBullet(Bullet bullet)
    {
        bullet.OnReturn();
        _pools[bullet.TypeIndex].Release(bullet);
    }

    private Bullet CreateBullet(Bullet prefab, int typeIndex)
    {
        Bullet b = Instantiate(prefab, transform);
        b.SetTypeIndex(typeIndex);
        return b;
    }
}
