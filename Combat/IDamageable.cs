namespace Eventyrerlauget.Combat;

public interface IDamageable
{
    string Name { get; }
    int CurrentHitPoints { get; }
    int MaxHitPoints { get; }
    bool IsAlive { get; }

    void TakeDamage(int damage);
    void Heal(int amount);
}
