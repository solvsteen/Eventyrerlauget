using Eventyrerlauget.Combat;
using Eventyrerlauget.Dice;
using Eventyrerlauget.Exceptions;

namespace Eventyrerlauget.Characters;

// Spellcasting hero. ISpellcaster adds mana and CastSpell on top of Character.Attack.
public class Sorcerer : Character, ISpellcaster
{
    public int CurrentMana { get; private set; }
    public int MaxMana { get; private set; }
    // Fireball costs this much mana and adds SpellPowerBonus on top of a d10.
    public int SpellManaCost { get; private set; } = 5;
    public int SpellPowerBonus { get; private set; } = 3;

    public Sorcerer(string name, int level, int maxHitPoints, int maxMana)
        : base(name, level, maxHitPoints)
    {
        if (maxMana <= 0)
        {
            throw new ArgumentException("Max mana must be positive.", nameof(maxMana));
        }

        MaxMana = maxMana;
        CurrentMana = maxMana;
    }

    public string CastSpell(IDamageable target, IDiceRoller diceRoller)
    {
        // Same rule as Character.Attack: a defeated caster cannot spend mana.
        if (!IsAlive)
        {
            throw new CharacterIsDefeatedException(Name);
        }

        if (CurrentMana < SpellManaCost)
        {
            throw new InsufficientManaException(Name, SpellManaCost, CurrentMana);
        }

        CurrentMana -= SpellManaCost;
        int damage = diceRoller.Roll(10) + SpellPowerBonus;
        target.TakeDamage(damage);

        // The UI colors this line from words like "casts"; Encounter adds a "defeated" line if the target dies.
        return $"{Name} casts a fireball at {target.Name} for {damage} magical damage (mana: {CurrentMana}/{MaxMana}).";
    }

    public void RestoreMana(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Mana restored cannot be negative.", nameof(amount));
        }

        // Math.Min keeps mana from going above MaxMana (same idea as Heal and MaxHP).
        CurrentMana = Math.Min(MaxMana, CurrentMana + amount);
    }
}
