namespace Eventyrerlauget.Exceptions;

public class CharacterIsDefeatedException : Exception
{
    public CharacterIsDefeatedException(string characterName)
        : base($"{characterName} is defeated and cannot act.")
    {}
}
