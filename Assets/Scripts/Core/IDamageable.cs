public interface IDamageable
{
    float Health { get; }
    float MaxHealth { get; }
    void TakeDamage(float amount, string source = "Unknown");
}
