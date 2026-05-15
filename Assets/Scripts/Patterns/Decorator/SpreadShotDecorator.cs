using UnityEngine;

/// <summary>
/// Decorator that fires a fixed center bullet plus N spread bullet pairs.
/// Wraps an inner IWeapon (typically BaseWeapon) and forwards Cooldown.
/// PairCount is mutable so multiple pickups can stack onto the same decorator
/// instance instead of nesting wrappers (which would duplicate the center shot).
///
/// Bullet count formula: 1 (center, delegated to inner) + 2 * PairCount (wings).
/// Pair i fires at ±(i * anglePerPair) degrees from aim direction.
///
/// This decorator does NOT manage cooldown — gating is owned by PlayerController.
/// </summary>
public class SpreadShotDecorator : WeaponDecorator
{
    private readonly Transform origin;
    private readonly Bullet    bulletPrefab;
    private readonly string    shooterName;
    private readonly float     anglePerPair;

    public int PairCount { get; private set; }

    public SpreadShotDecorator(
        IWeapon   inner,
        Transform origin,
        Bullet    bulletPrefab,
        string    shooterName,
        int       initialPairCount = 1,
        float     anglePerPair     = 10f) : base(inner)
    {
        this.origin       = origin;
        this.bulletPrefab = bulletPrefab;
        this.shooterName  = shooterName;
        this.anglePerPair = anglePerPair;
        PairCount         = Mathf.Max(1, initialPairCount);
    }

    public void AddPairs(int delta)
    {
        PairCount = Mathf.Max(0, PairCount + delta);
    }

    public override void Fire(Vector3 direction)
    {
        // Center bullet — delegated to inner weapon (preserves chain behavior).
        inner.Fire(direction);

        // Wing pairs at progressively wider angles.
        for (int i = 1; i <= PairCount; i++)
        {
            float angle = i * anglePerPair;
            Vector3 left  = Quaternion.Euler(0f, -angle, 0f) * direction;
            Vector3 right = Quaternion.Euler(0f,  angle, 0f) * direction;
            BulletPool.Instance.Launch(origin.position, left,  shooterName, bulletPrefab);
            BulletPool.Instance.Launch(origin.position, right, shooterName, bulletPrefab);
        }
    }
}
