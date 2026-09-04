namespace Eventyrerlauget.Items;

public class Weapon : Item
{
    public Weapon(string name, int damageBonus) : base(name){
        if (damageBonus < 0){
            throw new ArgumentException("Damage bonus must be greater than 0", nameof(damageBonus));
        }
        DamageBonus = damageBonus;
    }

    public int DamageBonus { get; private set; }

    public override string Describe() => $"A weapon of {Name} that deals {DamageBonus} damage";
}
