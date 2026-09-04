namespace Eventyrerlauget.Items;

// Armor is an Item. The extra data is a defense bonus.
public class Armor : Item
{
    public Armor(string name, int defenseBonus)
        : base(name)
    {
        if (defenseBonus < 0)
        {
            // nameof(defenseBonus) becomes the string "defenseBonus",
            // so the exception shows which argument was invalid.
            throw new ArgumentException("Defense bonus cannot be negative", nameof(defenseBonus));
        }

        DefenseBonus = defenseBonus;
    }

    // How much incoming damage this armor reduces.
    // { get; } with no setter = read-only after the constructor.
    public int DefenseBonus { get; }

    // override = this is Armor's version of Item.Describe().
    public override string Describe() => $"{Name} (-{DefenseBonus} incoming damage)";
}
