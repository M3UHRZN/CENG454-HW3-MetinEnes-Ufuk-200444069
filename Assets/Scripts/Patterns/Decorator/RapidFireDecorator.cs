using UnityEngine;

/// <summary>
/// Decorator that reduces effective cooldown by multiplying the inner weapon's Cooldown.
/// Stacks via nested wrapping — each pickup wraps the current weapon with another
/// RapidFireDecorator, so multipliers chain through Cooldown queries
/// (e.g. 0.7 × 0.7 = 0.49 of base cooldown after two pickups).
/// Fire delegates to inner — bullet pattern stays whatever the wrapped weapon produces.
/// Cooldown gating is owned by PlayerController; this decorator only changes the value.
/// </summary>
public class RapidFireDecorator : WeaponDecorator
{
    private const float MinMultiplier = 0.1f;

    private readonly float multiplier;

    public override float Cooldown => inner.Cooldown * multiplier;

    public RapidFireDecorator(IWeapon inner, float multiplier = 0.7f) : base(inner)
    {
        this.multiplier = Mathf.Clamp(multiplier, MinMultiplier, 1f);
    }

    public override void Fire(Vector3 direction) => inner.Fire(direction);
}
