using UnityEngine;

/// <summary>
/// Pickup that adds spread shot pairs to the player's weapon.
/// First pickup wraps the current IWeapon with a SpreadShotDecorator;
/// subsequent pickups bump the existing decorator's PairCount so the
/// center shot stays fixed and only wing pairs accumulate.
/// </summary>
public class WeaponUpgradePickup : PickupBase
{
    [Tooltip("How many wing pairs this pickup adds (+1 = +2 bullets).")]
    [SerializeField] private int   pairsToAdd  = 1;

    [Tooltip("Angle (degrees) between successive wing pairs.")]
    [SerializeField] private float anglePerPair = 10f;

    protected override bool Apply(PlayerController player)
    {
        if (player.Weapon is SpreadShotDecorator existing)
        {
            existing.AddPairs(pairsToAdd);
            return true;
        }

        BaseWeapon baseWeapon = player.GetComponentInChildren<BaseWeapon>();
        if (baseWeapon == null) return false;

        IWeapon upgraded = new SpreadShotDecorator(
            inner:            player.Weapon,
            origin:           baseWeapon.Origin,
            bulletPrefab:     baseWeapon.BulletPrefab,
            shooterName:      baseWeapon.ShooterName,
            initialPairCount: pairsToAdd,
            anglePerPair:     anglePerPair);

        player.SetWeapon(upgraded);
        return true;
    }
}
