using UnityEngine;

public interface IWeapon
{
    float Cooldown { get; }
    void Fire(Vector3 direction);
}
