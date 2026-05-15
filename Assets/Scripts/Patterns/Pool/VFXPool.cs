using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public struct VFXPoolEntry
{
    public PooledVFX prefab;
    public int       defaultCapacity;
    public int       maxSize;
}

/// <summary>
/// Singleton multi-prefab pool for VFX effects (muzzle flashes, hit impacts, explosions).
/// Mirrors BulletPool / EnemyPool: prefab-keyed Facade, per-type ObjectPool, auto-return
/// via PooledVFX when the particle finishes.
/// </summary>
public class VFXPool : MonoBehaviour
{
    public static VFXPool Instance { get; private set; }

    [SerializeField] private VFXPoolEntry[] entries;

    private ObjectPool<PooledVFX>[] _pools;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _pools = new ObjectPool<PooledVFX>[entries.Length];
        for (int i = 0; i < entries.Length; i++)
        {
            int          index = i;
            VFXPoolEntry entry = entries[i];
            _pools[i] = new ObjectPool<PooledVFX>(
                createFunc:      () => CreateVFX(entry.prefab, index),
                actionOnGet:     v => v.SetTypeIndex(index),
                actionOnRelease: v => v.gameObject.SetActive(false),
                actionOnDestroy: v => Destroy(v.gameObject),
                collectionCheck: true,
                defaultCapacity: entry.defaultCapacity,
                maxSize:         entry.maxSize);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public int GetTypeIndex(PooledVFX prefab)
    {
        if (prefab == null) return 0;
        for (int i = 0; i < entries.Length; i++)
            if (entries[i].prefab == prefab) return i;

        Debug.LogWarning($"[VFXPool] Prefab '{prefab.name}' not found in entries. Using index 0.");
        return 0;
    }

    public void Play(Vector3 position, Quaternion rotation, PooledVFX prefab)
        => Play(GetTypeIndex(prefab), position, rotation);

    public void Play(int typeIndex, Vector3 position, Quaternion rotation)
    {
        if (_pools == null || _pools.Length == 0) return;

        PooledVFX vfx = _pools[typeIndex].Get();

        // Defensive: a pooled instance may have been destroyed externally
        // (e.g. ParticleSystem.StopAction = Destroy on a prefab we didn't normalize).
        // Unity's overloaded == returns true for fake-null destroyed objects.
        if (vfx == null)
        {
            Debug.LogWarning($"[VFXPool] Pooled instance for type {typeIndex} was destroyed; recreating.");
            vfx = CreateVFX(entries[typeIndex].prefab, typeIndex);
        }

        vfx.transform.SetPositionAndRotation(position, rotation);
        vfx.OnSpawn();
    }

    public void Return(PooledVFX vfx)
    {
        vfx.OnReturn();
        _pools[vfx.TypeIndex].Release(vfx);
    }

    private PooledVFX CreateVFX(PooledVFX prefab, int typeIndex)
    {
        PooledVFX v = Instantiate(prefab, transform);
        v.SetTypeIndex(typeIndex);
        return v;
    }
}
