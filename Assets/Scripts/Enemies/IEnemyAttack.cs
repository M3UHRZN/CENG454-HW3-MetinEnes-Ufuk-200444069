using UnityEngine;
using UnityEngine.AI;

public interface IEnemyAttack
{
    void Execute(NavMeshAgent agent, Transform player, Transform core);
}
