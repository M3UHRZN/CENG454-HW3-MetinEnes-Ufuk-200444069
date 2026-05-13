using UnityEngine;
using UnityEngine.AI;

public interface IEnemyMovement
{
    void Execute(NavMeshAgent agent, Transform player, Transform core);
}
