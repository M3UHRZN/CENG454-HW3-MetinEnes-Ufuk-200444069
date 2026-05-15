using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour, IProjectileLauncher
{
    public static BulletPool Instance { get; private set; }

    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 50;

    private ObjectPool<Bullet> _pool;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _pool = new ObjectPool<Bullet>(
            createFunc:      CreateBullet,
            actionOnGet:     _ => { },
            actionOnRelease: b => b.gameObject.SetActive(false),
            actionOnDestroy: b => Destroy(b.gameObject),
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize:         maxSize
        );
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Launch(Vector3 origin, Vector3 direction, string shooterName = "Unknown")
    {
        Bullet bullet = _pool.Get();
        bullet.ShooterName = shooterName;
        bullet.transform.SetPositionAndRotation(origin, Quaternion.LookRotation(direction));
        bullet.OnSpawn();
    }

    public void ReturnBullet(Bullet bullet)
    {
        bullet.OnReturn();
        _pool.Release(bullet);
    }

    private Bullet CreateBullet()
    {
        return Instantiate(bulletPrefab, transform);
    }
}
