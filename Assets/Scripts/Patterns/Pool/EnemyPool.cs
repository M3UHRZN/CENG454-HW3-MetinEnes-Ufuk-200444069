using UnityEngine;
using UnityEngine.Pool;

public class EnemyPool : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 30;

    private ObjectPool<Enemy> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Enemy>(
            createFunc:      CreateEnemy,
            actionOnGet:     OnGetEnemy,
            actionOnRelease: OnReleaseEnemy,
            actionOnDestroy: OnDestroyEnemy,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    public Enemy Get()
    {
        return _pool.Get();
    }

    public void Return(Enemy enemy)
    {
        enemy.OnReturn();
        _pool.Release(enemy);
    }

    private Enemy CreateEnemy()
    {
        Enemy e = Instantiate(enemyPrefab, transform);
        return e;
    }

    private void OnGetEnemy(Enemy e)
    {
        e.gameObject.SetActive(true);
        e.OnSpawn();
    }

    private void OnReleaseEnemy(Enemy e) => e.gameObject.SetActive(false);
    private void OnDestroyEnemy(Enemy e) => Destroy(e.gameObject);
}
