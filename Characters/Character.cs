using Eventyrerlauget.Combat;

namespace Eventyrerlauget.Characters;

public abstract class Character : IDamageable
{
    public int HP { get; private set; }

    public int MaxHP { get; private set; }

    public string Name { get; private set; }
    public int Level { get; private set; }
    public bool IsAlive { get; private set; }


    public void Heal(int amount)
    {
        HP += amount;
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
    }
}