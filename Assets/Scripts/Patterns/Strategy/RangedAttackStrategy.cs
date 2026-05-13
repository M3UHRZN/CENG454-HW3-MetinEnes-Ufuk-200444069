using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Ranged enemy movement strategy. Keeps preferred range from the player,
/// retreats if too close, and fires via an IProjectileLauncher when in range.
/// Plain C# class — attach to Enemy via SerializeField MonoBehaviour slot.
/// </summary>
[System.Serializable]
public class RangedAttackStrategy : MonoBehaviour, IEnemyMovement
{
    [SerializeField] private float preferredRange = 8f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float retreatDistance = 5f;

    // Assign a MonoBehaviour that implements IProjectileLauncher in the Inspector.
    [SerializeField] private MonoBehaviour launcherBehaviour;

    private IProjectileLauncher _launcher;
    private float _timer;

    private void Awake()
    {
        if (launcherBehaviour != null)
            _launcher = launcherBehaviour as IProjectileLauncher;
    }

    public void Execute(NavMeshAgent agent, Transform player, Transform core)
    {
        if (agent == null || player == null) return;

        // NavMeshAgent'ın kendi rotasyonunu kapat, biz yöneteceğiz
        agent.updateRotation = false;

        _timer -= Time.deltaTime;

        Vector3 enemyPos = agent.transform.position;
        float distToPlayer = Vector3.Distance(enemyPos, player.position);

        if (distToPlayer < retreatDistance)
        {
            Vector3 retreatDir = (enemyPos - player.position).normalized;
            Vector3 retreatTarget = enemyPos + retreatDir * preferredRange;
            agent.SetDestination(retreatTarget);
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

        // Her zaman player'a bak (hareket yönünden bağımsız)
        Vector3 lookDir = player.position - agent.transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.01f)
            agent.transform.rotation = Quaternion.LookRotation(lookDir);
    }

    private void FireAt(Vector3 origin, Vector3 targetPos)
    {
        if (_launcher != null)
        {
            Vector3 direction = (targetPos - origin).normalized;
            _launcher.Launch(origin, direction);
        }
        else
        {
            Debug.LogWarning("[RangedAttack] No launcher assigned");
        }
    }
}
