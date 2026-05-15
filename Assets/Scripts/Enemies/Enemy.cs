using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Core enemy MonoBehaviour. Implements IDamageable and IPoolable.
/// Movement is delegated to an IEnemyMovement strategy assigned in the Inspector.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour, IDamageable, IPoolable
{
    // ── Health ────────────────────────────────────────────────────────────
    [SerializeField] private float maxHealth = 30f;
    private float _health;

    public float Health    => _health;
    public float MaxHealth => maxHealth;

    private static int killCount = 0;

    [Tooltip("Assign a MonoBehaviour that implements IEnemyMovement.")]
    [SerializeField] private MonoBehaviour movementStrategyBehaviour;
    private IEnemyMovement _movementStrategy;
    [SerializeField] private Transform coreTransform;
    private Transform _playerTransform;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _health = maxHealth;

        _agent = GetComponent<NavMeshAgent>();
        if (movementStrategyBehaviour != null)
            _movementStrategy = movementStrategyBehaviour as IEnemyMovement;

        if (_movementStrategy == null)
            Debug.LogWarning($"[Enemy] {name}: movementStrategyBehaviour does not implement IEnemyMovement.");

        GameObject playerGO = GameObject.FindWithTag("Player");
        if (playerGO != null)
            _playerTransform = playerGO.transform;
        else
            Debug.LogWarning("[Enemy] No GameObject with tag 'Player' found in scene.");
    }

    private void Update()
    {
        _movementStrategy?.Execute(_agent, _playerTransform, coreTransform);
    }


    public void TakeDamage(float amount)
    {
        if (_health <= 0f) return; // already dead

        _health -= amount;

        if (_health <= 0f)
        {
            _health = 0f;
            OnDeath();
        }
    }

    private void OnDeath()
    {
        GameEventBus.RaiseEnemyDied(++killCount);
        OnReturn();
    }

    public void OnSpawn()
    {
        _health = maxHealth;

        if (_agent != null) _agent.enabled = true;
        gameObject.SetActive(true);

        // NOTE: If this Enemy were subscribed to any GameEventBus events,
        // re-subscribe here to avoid ghost subscriber issues after pool reuse.
    }

    public void OnReturn()
    {
        if (_agent != null) _agent.enabled = false;
        gameObject.SetActive(false);

        // NOTE: Unsubscribe from any GameEventBus events here to prevent
        // ghost subscribers firing on a pooled (inactive) Enemy instance.
        // e.g.: GameEventBus.OnSomeEvent -= HandleSomeEvent;
    }

    // ── Collision / Trigger ───────────────────────────────────────────────

    /// <summary>
    /// Physical collision: if the collided object is IDamageable,
    /// deal damage — represents the enemy ramming the core.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        TryDealContactDamage(collision.gameObject);
    }

    /// <summary>
    /// Trigger-based contact (use if the enemy collider is set to Is Trigger).
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        TryDealContactDamage(other.gameObject);
    }

    private void TryDealContactDamage(GameObject target)
    {
        IDamageable damageable = target.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(maxHealth); // ram deals full health as damage
        }
    }
}
