namespace Eventyrerlauget.Items;

// Weapon is an Item. The extra data is a damage bonus.
public class Weapon : Item
{
    public Weapon(string name, int damageBonus) : base(name){
        if (damageBonus < 0){
            // nameof(damageBonus) becomes the string "damageBonus",
            // so the exception shows which argument was invalid.
            throw new ArgumentException("Damage bonus must be greater than 0", nameof(damageBonus));
        }
        DamageBonus = damageBonus;
    }


    // How much damage this weapon adds to your attacks. Cannot be changed after the constructor.
    // { get; } with no setter = read-only after the constructor.
    public int DamageBonus { get; private set; }

    // override = this is Weapon's version of Item.Describe().
    public override string Describe() => $"A weapon of {Name} that deals {DamageBonus} damage";
}
