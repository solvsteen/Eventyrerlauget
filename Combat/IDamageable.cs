namespace Eventyrerlauget.Combat;

public interface IDamageable
{
    string Name { get; }
    int HP { get; }
    int MaxHP { get; }
    bool IsAlive { get; }

    void TakeDamage(int damage);
    void Heal(int amount);
}
