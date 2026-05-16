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
    [SerializeField] private float maxHealth     = 30f;
    [SerializeField] private float contactDamage = 10f;
    private float _health;

    public float Health    => _health;
    public float MaxHealth => maxHealth;

    private static int killCount = 0;
    private float _baseSpeed;

    public int TypeIndex { get; private set; }
    public void SetTypeIndex(int i) => TypeIndex = i;

    [Tooltip("Assign a MonoBehaviour that implements IEnemyMovement.")]
    [SerializeField] private MonoBehaviour movementStrategyBehaviour;
    private IEnemyMovement _movementStrategy;

    [Tooltip("Assign a MonoBehaviour that implements IEnemyAttack (optional).")]
    [SerializeField] private MonoBehaviour attackBehaviourBehaviour;
    private IEnemyAttack _attackBehaviour;
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

        if (attackBehaviourBehaviour != null)
        {
            _attackBehaviour = attackBehaviourBehaviour as IEnemyAttack;
            if (_attackBehaviour == null)
                Debug.LogWarning($"[Enemy] {name}: attackBehaviourBehaviour does not implement IEnemyAttack.");
        }

        GameObject playerGO = GameObject.FindWithTag("Player");
        if (playerGO != null)
            _playerTransform = playerGO.transform;
        else
            Debug.LogWarning("[Enemy] No GameObject with tag 'Player' found in scene.");

        if (coreTransform == null)
        {
            GameObject coreGO = GameObject.FindWithTag("Core");
            if (coreGO != null)
                coreTransform = coreGO.transform;
            else
                Debug.LogWarning("[Enemy] No GameObject with tag 'Core' found in scene.");
        }

        _baseSpeed = _agent != null ? _agent.speed : 3.5f;
    }

    private void OnEnable()
    {
        GameEventBus.OnWaveCompleted += HandleWaveCompleted;
    }

    private void OnDisable()
    {
        GameEventBus.OnWaveCompleted -= HandleWaveCompleted;
    }

    private void HandleWaveCompleted(int waveIndex)
    {
        if (_agent == null) return;
        // Her wave sonrası %15 hız artışı — pool'dan çıkarken base speed'e reset edilir
        _agent.speed = Mathf.Min(_agent.speed * 1.15f, _baseSpeed * 2f);
    }

    private void Update()
    {
        _movementStrategy?.Execute(_agent, _playerTransform, coreTransform);
        _attackBehaviour?.Execute(_agent, _playerTransform, coreTransform);
    }

    public void TakeDamage(float amount, string source = "Unknown")
    {
        if (_health <= 0f) return;

        _health -= amount;

        if (_health <= 0f)
        {
            _health = 0f;
            OnDeath(source);
        }
    }

    private void OnDeath(string killerName)
    {
        GameEventBus.RaiseEnemyDied(++killCount, killerName);
        EnemyPool.Instance?.Return(this);
    }

    public void OnSpawn()
    {
        _health = maxHealth;
        if (_agent != null)
        {
            _agent.speed   = _baseSpeed;
            _agent.enabled = true;
        }
        gameObject.SetActive(true);
        GameEventBus.OnWaveCompleted += HandleWaveCompleted; // ghost bug fix: re-subscribe
    }

    public void OnReturn()
    {
        GameEventBus.OnWaveCompleted -= HandleWaveCompleted; // ghost bug fix: unsubscribe
        if (_agent != null) _agent.enabled = false;
        gameObject.SetActive(false);
    }

    // ── Collision / Trigger ───────────────────────────────────────────────

    private void OnCollisionEnter(Collision collision)
    {
        TryDealContactDamage(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDealContactDamage(other.gameObject);
    }

    private void TryDealContactDamage(GameObject target)
    {
        if (!target.CompareTag("Core")) return;
        IDamageable damageable = target.GetComponentInParent<IDamageable>();
        if (damageable != null)
            damageable.TakeDamage(contactDamage, gameObject.name);
    }
}
