using UnityEngine;
using UnityEngine.AI;

public class BombAttackBehaviour : MonoBehaviour, IEnemyAttack
{
    [SerializeField] private float     detonationRange = 2.5f;
    [SerializeField] private float     explosionRadius = 4f;
    [SerializeField] private float     explosionDamage = 50f;
    [SerializeField] private PooledVFX explosionVFX;

    private bool _exploded;
    private Collider _coreCollider;

    private void OnEnable() => _exploded = false;

    public void Execute(NavMeshAgent agent, Transform player, Transform core)
    {
        if (agent == null || core == null || _exploded) return;

        if (_coreCollider == null)
            _coreCollider = core.GetComponent<Collider>();

        Vector3 closest = _coreCollider != null
            ? _coreCollider.ClosestPoint(agent.transform.position)
            : core.position;

        if (Vector3.Distance(agent.transform.position, closest) <= detonationRange)
            Explode(agent.transform.position);
    }

    private void Explode(Vector3 center)
    {
        _exploded = true;

        if (explosionVFX != null && VFXPool.Instance != null)
            VFXPool.Instance.Play(center, Quaternion.identity, explosionVFX);

        Collider[] hits = Physics.OverlapSphere(center, explosionRadius);
        IDamageable self = GetComponentInParent<IDamageable>();

        foreach (Collider hit in hits)
        {
            IDamageable target = hit.GetComponentInParent<IDamageable>();
            if (target != null && !ReferenceEquals(target, self))
                target.TakeDamage(explosionDamage, gameObject.name);
        }

        self?.TakeDamage(float.MaxValue, gameObject.name);
    }
}
