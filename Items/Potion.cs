namespace Eventyrerlauget.Items;

// Potion is an Item, so it gets Name, Describe(), and ToString() from Item.
public class Potion : Item
{

    // : base(name) calls Item's constructor first, which stores the name.
    public Potion(string name, int healAmount) : base(name)
    {
        if (healAmount <= 0){
            // nameof(healAmount) becomes the string "healAmount",
            // so the exception shows which argument was invalid.
            throw new ArgumentException("Heal amount must be greater than 0", nameof(healAmount));
        }

        HealAmount = healAmount;
    }
    // How much HP this potion restores. Cannot be changed after the constructor.
    // { get; } with no setter = read-only after the constructor.
    public int HealAmount { get; private set; }

    // override = this is Potion's version of Item.Describe().
    public override string Describe() => $"A potion of {Name} that heals {HealAmount} HP";
}
