using UnityEngine;
using UnityEngine.Pool;

public class BulletPool : MonoBehaviour, IProjectileLauncher
{
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int defaultCapacity = 20;
    [SerializeField] private int maxSize = 50;

    private ObjectPool<Bullet> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Bullet>(
            createFunc:      CreateBullet,
            actionOnGet:     OnGetBullet,
            actionOnRelease: OnReleaseBullet,
            actionOnDestroy: OnDestroyBullet,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    // IProjectileLauncher
    public void Launch(Vector3 origin, Vector3 direction)
    {
        Bullet bullet = _pool.Get();
        bullet.transform.SetPositionAndRotation(
            origin,
            Quaternion.LookRotation(direction)
        );
        bullet.OnSpawn();
    }

    public void ReturnBullet(Bullet bullet)
    {
        bullet.OnReturn();
        _pool.Release(bullet);
    }

    private Bullet CreateBullet()
    {
        Bullet b = Instantiate(bulletPrefab, transform);
        b.Init(this);
        return b;
    }

    private void OnGetBullet(Bullet b)     => b.gameObject.SetActive(true);
    private void OnReleaseBullet(Bullet b)  => b.gameObject.SetActive(false);
    private void OnDestroyBullet(Bullet b)  => Destroy(b.gameObject);
}
