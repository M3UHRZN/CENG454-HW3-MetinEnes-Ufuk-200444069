using UnityEngine;
using UnityEngine.AI;

public class RangedAttackStrategy : MonoBehaviour, IEnemyMovement
{
    [SerializeField] private float preferredRange  = 8f;
    [SerializeField] private float attackCooldown  = 2f;
    [SerializeField] private float retreatDistance = 5f;

    private float _timer;

    public void Execute(NavMeshAgent agent, Transform player, Transform core)
    {
        if (agent == null || player == null) return;

        agent.updateRotation = false;
        _timer -= Time.deltaTime;

        Vector3 enemyPos     = agent.transform.position;
        float   distToPlayer = Vector3.Distance(enemyPos, player.position);

        if (distToPlayer < retreatDistance)
        {
            Vector3 retreatDir = (enemyPos - player.position).normalized;
            agent.SetDestination(enemyPos + retreatDir * preferredRange);
        }
        else if (distToPlayer > preferredRange)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.SetDestination(enemyPos);

            if (_timer <= 0f)
            {
                _timer = attackCooldown;
                FireAt(enemyPos, player.position);
            }
        }

        Vector3 lookDir = player.position - agent.transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.01f)
            agent.transform.rotation = Quaternion.LookRotation(lookDir);
    }

    private void FireAt(Vector3 origin, Vector3 targetPos)
    {
        Vector3 direction = (targetPos - origin).normalized;
        BulletPool.Instance.Launch(origin, direction, gameObject.name);
    }
}
