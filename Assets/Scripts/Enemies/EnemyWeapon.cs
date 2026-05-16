using UnityEngine;

public class EnemyWeapon : MonoBehaviour, IWeapon
{
    [SerializeField] private float  cooldown     = 2f;
    [SerializeField] private float  fireHeight   = 1f;
    [SerializeField] private Bullet bulletPrefab;

    public float Cooldown => cooldown;

    private float _timer;

    private void Update()
    {
        if (_timer > 0f)
            _timer -= Time.deltaTime;
    }

    public void Fire(Vector3 direction)
    {
        if (_timer > 0f) return;
        _timer = cooldown;

        Vector3 origin = transform.position + Vector3.up * fireHeight;
        BulletPool.Instance.Launch(origin, direction, gameObject.name, bulletPrefab);

        GameEventBus.RaiseWeaponFired(gameObject.name);
    }
}
