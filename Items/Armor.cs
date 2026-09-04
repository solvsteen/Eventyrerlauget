namespace Eventyrerlauget.Items;

public class Armor : Item
{
    public Armor(string name, int defenseBonus)
        : base(name)
    {
        if (defenseBonus < 0)
        {
            throw new ArgumentException("Forsvarsbonus kan ikke være negativ.", nameof(defenseBonus));
        }

        DefenseBonus = defenseBonus;
    }

    public int DefenseBonus { get; }

    public override string Describe() => $"{Name} (-{DefenseBonus} indgående skade)";
}
