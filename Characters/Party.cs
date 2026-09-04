namespace Eventyrerlauget.Characters;

// A named group of characters (the player's adventuring party).
// It stores members, can add new ones, and answers questions like
// "who is still alive?" and "is the whole party defeated?"
public class Party
{
    // The real list of characters. private = other classes cannot change it
    // directly; they must use AddMember(). readonly = this field always
    // points to the same list object (the list contents can still change).
    // [] is a collection expression: start with an empty list.
    private readonly List<Character> _members = [];

    public Party(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Name cannot be null or empty", nameof(name));
        }
        Name = name;
    }

    // The party's name, e.g. "The Fellowship". Set once in the constructor.
    public string Name { get; }

    // A read-only view of the members. Callers can look at the list
    // (foreach, Count, etc.) but cannot Add/Remove from the outside.
    // => means this property always returns the current _members list.
    public IReadOnlyList<Character> Members => _members;

    // Only the characters who are still alive.
    // Where(...) filters the list: keep a member if member.IsAlive is true.
    public IEnumerable<Character> AliveMembers() => _members.Where(member => member.IsAlive);

    // True when the party has at least one member AND every member is dead.
    // An empty party is not treated as defeated (Count > 0 must be true first).
    // All(...) is true only if the condition holds for every member.
    public bool IsDefeated() => _members.Count > 0 && _members.All(member => !member.IsAlive);

    public void AddMember(Character member)
    {
        // Throws if someone calls AddMember(null).
        ArgumentNullException.ThrowIfNull(member);

        // The same Character object cannot join twice.
        if (_members.Contains(member))
        {
            throw new ArgumentException($"{member.Name} is already in the party {Name}", nameof(member));
        }

        _members.Add(member);
    }
}