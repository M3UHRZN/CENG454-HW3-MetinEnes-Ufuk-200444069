using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Moves the enemy directly toward the core, ignoring the player.
/// </summary>
public class DirectMoveStrategy : MonoBehaviour, IEnemyMovement
{
    private Collider _coreCollider;

    public void Execute(NavMeshAgent agent, Transform player, Transform core)
    {
        if (agent == null || core == null) return;

        // Cache the core collider
        if (_coreCollider == null)
            _coreCollider = core.GetComponent<Collider>();

        // Target the closest point on the core's surface, not its center.
        // core.position is often inside a NavMesh obstacle (unreachable),
        // causing the agent to circle around endlessly.
        Vector3 target = _coreCollider != null
            ? _coreCollider.ClosestPoint(agent.transform.position)
            : core.position;

        if (NavMesh.SamplePosition(target, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            target = hit.position;

        agent.SetDestination(target);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) return;
        if (collision.gameObject.GetComponentInParent<IDamageable>() != null)
            GetComponent<Enemy>()?.TakeDamage(float.MaxValue, "Contact");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) return;
        if (other.GetComponentInParent<IDamageable>() != null)
            GetComponent<Enemy>()?.TakeDamage(float.MaxValue, "Contact");
    }
}
