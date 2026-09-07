using Eventyrerlauget.Dice;

namespace Eventyrerlauget.Combat;

// Implemented by Sorcerer. Encounter uses this so it does not have to name the subclass.
public interface ISpellcaster
{
    int CurrentMana { get; }
    int MaxMana { get; }
    // Returns a combat-log line. Throws InsufficientManaException if CurrentMana is too low.
    string CastSpell(IDamageable target, IDiceRoller diceRoller);
    // Used between fights so a Sorcerer can recover some mana without a full rest.
    void RestoreMana(int amount);
}
