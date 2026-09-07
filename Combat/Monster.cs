using Eventyrerlauget.Characters;
using Eventyrerlauget.Dice;
using Eventyrerlauget.Exceptions;

namespace Eventyrerlauget.Combat;

// An enemy in a fight. Same IDamageable contract as Character, plus an optional poison bite.
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

    // Returns a combat-log line. 1 on a d6 misses; 6 is a critical (double damage).
    public string Attack(IDamageable target, IDiceRoller diceRoller)
    {
        // A slain monster cannot act. IsAlive itself does not throw; this call does.
        if (!IsAlive)
        {
            throw new CharacterIsDefeatedException(Name);
        }

        int roll = diceRoller.Roll(6);
        if (roll == 1)
        {
            return $"{Name} misses {target.Name}.";
        }

        int damage = roll;
        // A 6 on a d6 is a critical: double the dice before armor.
        bool isCritical = roll == 6;
        if (isCritical)
        {
            damage *= 2;
        }

        // Pattern matching: only heroes wear armor, so only they can mitigate.
        if (target is Character hero)
        {
            damage = hero.MitigateDamage(damage);
        }

        if (damage <= 0)
        {
            return $"The attack glances past {target.Name}.";
        }

        target.TakeDamage(damage);

        string line = isCritical
            ? $"{Name} lands a critical hit on {target.Name} for {damage} damage."
            : $"{Name} hits {target.Name} for {damage} damage.";

        if (IsPoisonous && target is Character victim && victim.IsAlive)
        {
            // d100 vs PoisonChancePercent (e.g. 25 = 25% chance).
            if (diceRoller.Roll(100) <= PoisonChancePercent)
            {
                victim.ApplyPoison();
                line += $" {victim.Name} is poisoned!";
            }
        }

        return line;
    }
}