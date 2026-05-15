using UnityEngine;

/// <summary>
/// Inspector'dan hedef atayıp, butona basınca damage veren debug scripti.
/// </summary>
public class DebugDamager : MonoBehaviour
{
    [Header("Hedef (IDamageable olan herhangi bir GameObject)")]
    [SerializeField] private GameObject target;

    [Header("Ayarlar")]
    [SerializeField] private float damageAmount = 10f;

    /// <summary>
    /// Inspector butonu veya ContextMenu'den çağrılır.
    /// </summary>
    [ContextMenu("Deal Damage")]
    public void DealDamage()
    {
        if (target == null)
        {
            Debug.LogWarning("[DebugDamager] Target atanmamış!", this);
            return;
        }

        var damageable = target.GetComponent<IDamageable>();
        if (damageable == null)
        {
            Debug.LogWarning($"[DebugDamager] {target.name} üzerinde IDamageable bulunamadı!", this);
            return;
        }

        damageable.TakeDamage(damageAmount, "DebugDamager");
        Debug.Log($"[DebugDamager] {target.name} hedefine {damageAmount} hasar verildi. Kalan HP: {damageable.Health}/{damageable.MaxHealth}");
    }
}
