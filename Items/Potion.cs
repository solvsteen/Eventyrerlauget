namespace Eventyrerlauget.Items;

public class Potion : Item
{


    public Potion(string name, int healAmount) : base(name)
    {
        if (healAmount <= 0){
            throw new ArgumentException("Heal amount must be greater than 0", nameof(healAmount));
        }
        HealAmount = healAmount;
    }

    public int HealAmount { get; private set; }

    public override string Describe() => $"A potion of {Name} that heals {HealAmount} HP";
}
