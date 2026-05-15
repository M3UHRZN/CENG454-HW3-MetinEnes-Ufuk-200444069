using UnityEngine;

/// <summary>
/// Template Method base for world pickups. Centralizes the trigger lifecycle
/// (Reset sets isTrigger, OnTriggerEnter validates Player + Weapon, destroys
/// the pickup if Apply succeeds). Subclasses only override Apply to implement
/// the specific effect.
/// </summary>
[RequireComponent(typeof(Collider))]
public abstract class PickupBase : MonoBehaviour
{
    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null || player.Weapon == null) return;

        if (Apply(player))
            Destroy(gameObject);
    }

    /// <summary>
    /// Apply this pickup's effect to the player.
    /// Return true if the effect was applied (pickup gets destroyed),
    /// false if the pickup could not apply (pickup remains in the scene).
    /// </summary>
    protected abstract bool Apply(PlayerController player);
}
