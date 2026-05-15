using UnityEngine;
using UnityEngine.AI;

public class RangedAttackBehaviour : MonoBehaviour, IEnemyAttack, IWeapon
{
    [SerializeField] private float firingRange = 8f;
    [SerializeField] private EnemyWeapon weapon;

    public float Cooldown => weapon != null ? weapon.Cooldown : 0f;

    public void Fire(Vector3 direction) => weapon?.Fire(direction);

    public void Execute(NavMeshAgent agent, Transform player, Transform core)
    {
        if (agent == null || player == null || weapon == null) return;

        float dist = Vector3.Distance(agent.transform.position, player.position);
        if (dist > firingRange) return;

        Vector3 dir = player.position - agent.transform.position;
        dir.y = 0f;
        Fire(dir.normalized);
    }
}
