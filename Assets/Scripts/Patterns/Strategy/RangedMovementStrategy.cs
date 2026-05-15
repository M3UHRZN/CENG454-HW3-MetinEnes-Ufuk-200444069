using UnityEngine;
using UnityEngine.AI;

public class RangedMovementStrategy : MonoBehaviour, IEnemyMovement
{
    [SerializeField] private float preferredRange  = 8f;
    [SerializeField] private float retreatDistance = 5f;

    public void Execute(NavMeshAgent agent, Transform player, Transform core)
    {
        if (agent == null || player == null) return;

        agent.updateRotation = false;

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
        }

        Vector3 lookDir = player.position - enemyPos;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.01f)
            agent.transform.rotation = Quaternion.LookRotation(lookDir);
    }
}
