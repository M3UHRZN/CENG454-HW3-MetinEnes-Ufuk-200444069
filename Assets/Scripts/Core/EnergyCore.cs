using UnityEngine;

public class EnergyCore : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;

    private float _currentHealth;
    private bool _isDead;

    public float Health    => _currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        _currentHealth = maxHealth;
        _isDead        = false;
    }

    public void TakeDamage(float amount)
    {
        if (_isDead) return;

        _currentHealth = Mathf.Max(0f, _currentHealth - amount);

        GameEventBus.RaiseCoreDamaged(_currentHealth, maxHealth);

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        _isDead = true;
        GameEventBus.RaiseGameOver(false);
    }
}
