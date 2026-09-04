namespace Eventyrerlauget.Exceptions;

// Thrown when a spellcaster tries to cast without enough mana.
// Encounter should catch this and let the character fumble the turn instead of crashing.
public class InsufficientManaException : Exception
{
    public InsufficientManaException(string characterName, int required, int available)
        : base($"{characterName} does not have enough mana ({available}/{required} required).")
    {
        CharacterName = characterName;
        Required = required;
        Available = available;
    }

    public string CharacterName { get; }
    public int Required { get; }
    public int Available { get; }
}
