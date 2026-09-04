namespace Eventyrerlauget.Items;

public abstract class Item
{
    public string Name { get; private set; }

    public abstract string Describe();

    public override string ToString() => Describe();

    protected Item(string name)
    {
        if (string.IsNullOrEmpty(name)){
            throw new ArgumentException("Name cannot be null or empty");
        }
        Name = name;
    }
}