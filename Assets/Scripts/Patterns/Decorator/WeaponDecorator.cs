using UnityEngine;

/// <summary>
/// Decorator pattern base: wraps an inner IWeapon and forwards Cooldown by default.
/// Subclasses override Fire to extend or replace shoot behavior without modifying
/// the wrapped weapon.
/// </summary>
public abstract class WeaponDecorator : IWeapon
{
    protected readonly IWeapon inner;

    protected WeaponDecorator(IWeapon inner)
    {
        this.inner = inner;
    }

    public virtual float Cooldown => inner.Cooldown;

    public abstract void Fire(Vector3 direction);
}
