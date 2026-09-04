using Eventyrerlauget.Combat;
using Eventyrerlauget.Dice;
using Eventyrerlauget.Exceptions;

namespace Eventyrerlauget.Characters;

public class Sorcerer : Character, ISpellcaster
{
    public int CurrentMana { get; private set; }
    public int MaxMana { get; private set; }
    public int SpellManaCost { get; private set; } = 5;
    public int SpellPowerBonus { get; private set; } = 3;

    public Sorcerer(string name, int level, int maxHitPoints, int maxMana)
        : base(name, level, maxHitPoints)
    {
        if (maxMana <= 0)
        {
            throw new ArgumentException("Maks. mana skal være positivt.", nameof(maxMana));
        }

        MaxMana = maxMana;
        CurrentMana = maxMana;
    }

    public string CastSpell(IDamageable target, IDiceRoller diceRoller)
    {
        if (CurrentMana < SpellManaCost)
        {
            throw new InsufficientManaException(Name, SpellManaCost, CurrentMana);
        }

        CurrentMana -= SpellManaCost;
        int damage = diceRoller.Roll(10) + SpellPowerBonus;
        target.TakeDamage(damage);

        return $"{Name} kaster en ildkugle på {target.Name} for {damage} magisk skade (mana: {CurrentMana}/{MaxMana}).";
    }
}
