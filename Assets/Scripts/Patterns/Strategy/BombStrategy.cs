using UnityEngine;
using UnityEngine.AI;

public class BombStrategy : MonoBehaviour, IEnemyMovement
{
    [SerializeField] private float detonationRange = 2.5f;
    [SerializeField] private float explosionRadius = 4f;
    [SerializeField] private float explosionDamage = 50f;

    private bool _exploded;
    private Collider _coreCollider;

    private void OnEnable() => _exploded = false;

    public void Execute(NavMeshAgent agent, Transform player, Transform core)
    {
        if (agent == null || core == null || _exploded) return;

        // Cache the core collider
        if (_coreCollider == null)
            _coreCollider = core.GetComponent<Collider>();

        // Target the closest point on the core's surface, not its center.
        Vector3 target = _coreCollider != null
            ? _coreCollider.ClosestPoint(agent.transform.position)
            : core.position;

        agent.SetDestination(target);

        if (Vector3.Distance(agent.transform.position, target) <= detonationRange)
            Explode(agent.transform.position);
    }

    private void Explode(Vector3 center)
    {
        _exploded = true;

        Collider[] hits = Physics.OverlapSphere(center, explosionRadius);
        IDamageable self = GetComponentInParent<IDamageable>();

        foreach (Collider hit in hits)
        {
            IDamageable target = hit.GetComponentInParent<IDamageable>();
            if (target != null && !ReferenceEquals(target, self))
                target.TakeDamage(explosionDamage);
        }

        self?.TakeDamage(float.MaxValue);
    }
}
