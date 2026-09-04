using Eventyrerlauget.Dice;
using Eventyrerlauget.Exceptions;

namespace Eventyrerlauget.Combat;


public class Monster : IDamageable
{
    public string Name { get; private set; }
    public int HP { get; private set; }
    public int MaxHP { get; private set; }

    // Same rule as Character: a boolean the UI can check. Dead monsters show as "slain".
    public bool IsAlive => HP > 0;

    public int PoisonChancePercent { get; }
    // True when this monster has any chance to poison; used by the UI poison line.
    public bool IsPoisonous => PoisonChancePercent > 0;

    public Monster(string name, int maxHitPoints, int poisonChancePercent = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }

        if (maxHitPoints <= 0)
        {
            throw new ArgumentException("Max hit points must be positive.", nameof(maxHitPoints));
        }

        Name = name;
        MaxHP = maxHitPoints;
        HP = maxHitPoints;
        PoisonChancePercent = poisonChancePercent;
    }

    // Clamp like Character so HP never leaves the 0..MaxHP range the UI draws.
    public void Heal(int amount)
    {
        HP = Math.Min(MaxHP, HP + amount);
    }

    public void TakeDamage(int damage)
    {
        HP = Math.Max(0, HP - damage);
    }

    public void Attack(IDamageable target, IDiceRoller diceRoller)
    {
        // A slain monster cannot act. IsAlive itself does not throw; this call does.
        if (!IsAlive)
        {
            throw new CharacterIsDefeatedException(Name);
        }

        int damage = diceRoller.Roll(6);
        target.TakeDamage(damage * PoisonChancePercent);
    }
}