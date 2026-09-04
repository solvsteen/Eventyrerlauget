using Eventyrerlauget.Dice;

namespace Eventyrerlauget.Combat;

public interface ISpellcaster
{
    int CurrentMana { get; }
    int MaxMana { get; }
    string CastSpell(IDamageable target, IDiceRoller diceRoller);
}
