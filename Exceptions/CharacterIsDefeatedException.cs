namespace Eventyrerlauget.Exceptions;

public class CharacterIsDefeatedException : Exception
{
    public CharacterIsDefeatedException(string characterName)
        : base($"{characterName} er besejret og kan ikke handle.")
    {}
}
