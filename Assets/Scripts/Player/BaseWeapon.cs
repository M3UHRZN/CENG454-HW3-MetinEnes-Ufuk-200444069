using UnityEngine;

public class BaseWeapon : MonoBehaviour, IWeapon
{
    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private float cooldown = 0.5f;

    public float Cooldown => cooldown;

    private float _cooldownTimer;

    private void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;
    }

    public void Fire(Vector3 direction)
    {
        if (_cooldownTimer > 0f) return;

        _cooldownTimer = cooldown;
        bulletPool.Launch(transform.position, direction);
    }
}
