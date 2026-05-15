using UnityEngine;

public class BaseWeapon : MonoBehaviour, IWeapon
{
    [SerializeField] private float  cooldown     = 0.5f;
    [SerializeField] private Bullet bulletPrefab;

    public float     Cooldown     => cooldown;
    public Transform Origin       => transform;
    public Bullet    BulletPrefab => bulletPrefab;
    public string    ShooterName  => "Player";

    public void Fire(Vector3 direction)
    {
        BulletPool.Instance.Launch(transform.position, direction, ShooterName, bulletPrefab);
    }
}
