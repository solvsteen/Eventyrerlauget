namespace Eventyrerlauget.Combat;

// The choices a living hero can make on their turn.
// The UI turns these into a SelectionPrompt; Encounter.PlayHeroTurn runs the chosen one.
public enum HeroAction
{
    Attack,
    CastSpell,
    DrinkPotion,
}
