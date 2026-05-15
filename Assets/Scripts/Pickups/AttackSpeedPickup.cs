using UnityEngine;

/// <summary>
/// Pickup that wraps the player's current weapon with a RapidFireDecorator,
/// reducing effective cooldown. Stacks via nested wrapping — cooldowns multiply
/// through the decorator chain.
/// </summary>
public class AttackSpeedPickup : PickupBase
{
    [Tooltip("Cooldown multiplier applied per pickup. 0.7 = 30% faster fire rate.")]
    [Range(0.1f, 0.99f)]
    [SerializeField] private float cooldownMultiplier = 0.7f;

    protected override bool Apply(PlayerController player)
    {
        IWeapon upgraded = new RapidFireDecorator(player.Weapon, cooldownMultiplier);
        player.SetWeapon(upgraded);
        return true;
    }
}
