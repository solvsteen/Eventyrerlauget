namespace Eventyrerlauget.Combat;

// Anything the combat system can hurt or heal: heroes and monsters.
// Shared so Attack / CastSpell take one target type instead of two overloads.
public interface IDamageable
{
    string Name { get; }
    int HP { get; }
    int MaxHP { get; }
    bool IsAlive { get; }

    void TakeDamage(int damage);
    void Heal(int amount);
}
