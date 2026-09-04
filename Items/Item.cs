namespace Eventyrerlauget.Items;

// Abstract = you cannot write "new Item(...)".
// You can only create a Potion, Weapon, or Armor.
public abstract class Item
{
    // Shared by every item. private set = only this class can change it,
    // (the constructor sets it once).
    public string Name { get; private set; }

    // Abstract = every subclass must implement this.
    public abstract string Describe();

    // When you print an item (Console.WriteLine(item)), C# calls ToString().
    // We reuse Describe() so printing and describing stay the same.
    public override string ToString() => Describe();

    // protected = only Item and its subclasses can call this constructor.
    protected Item(string name)
    {
        if (string.IsNullOrEmpty(name)){
            throw new ArgumentException("Name cannot be null or empty");
        }
        Name = name;
    }
}