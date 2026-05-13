using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Moves the enemy directly toward the core, ignoring the player.
/// </summary>
public class DirectMoveStrategy : MonoBehaviour, IEnemyMovement
{
    public void Execute(NavMeshAgent agent, Transform player, Transform core)
    {
        if (agent == null || core == null) return;

        agent.SetDestination(core.position);
    }
}
