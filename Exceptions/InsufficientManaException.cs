namespace Eventyrerlauget.Exceptions;

// Kastes hvis en spellcaster forsøger at kaste en besværgelse uden nok mana.
// Encounter fanger den og lader karakteren fumle turen i stedet for at crashe.
public class InsufficientManaException : Exception
{
    public InsufficientManaException(string characterName, int required, int available)
        : base($"{characterName} har ikke mana nok ({available}/{required} krævet).")
    {
        CharacterName = characterName;
        Required = required;
        Available = available;
    }

    public string CharacterName { get; }
    public int Required { get; }
    public int Available { get; }
}
