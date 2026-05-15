using UnityEngine;
using UnityEngine.AI;

public class RangedAttackBehaviour : MonoBehaviour, IEnemyAttack
{
    [SerializeField] private float firingRange    = 8f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float fireHeight     = 1f;

    private float _timer;

    public void Execute(NavMeshAgent agent, Transform player, Transform core)
    {
        if (agent == null || player == null) return;

        _timer -= Time.deltaTime;

        float distToPlayer = Vector3.Distance(agent.transform.position, player.position);
        if (distToPlayer > firingRange) return;

        if (_timer <= 0f)
        {
            _timer = attackCooldown;
            FireAt(agent.transform.position, player.position);
        }
    }

    private void FireAt(Vector3 enemyBase, Vector3 playerPos)
    {
        Vector3 origin    = enemyBase + Vector3.up * fireHeight;
        Vector3 targetXZ  = new Vector3(playerPos.x, origin.y, playerPos.z);
        Vector3 direction = (targetXZ - origin).normalized;

        BulletPool.Instance.Launch(origin, direction, gameObject.name);
    }
}
