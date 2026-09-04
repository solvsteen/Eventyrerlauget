using Eventyrerlauget.Combat;
using Eventyrerlauget.Dice;
using Eventyrerlauget.Exceptions;
using Eventyrerlauget.Items;

namespace Eventyrerlauget.Characters;

// Abstract base for every hero (Fighter, Sorcerer, Ranger).
// Abstract = you cannot write "new Character(...)"; you create a subclass instead.
// IDamageable = the combat system can Heal / TakeDamage this object the same way as a Monster.
public abstract class Character : IDamageable
{
    // Current hit points. private set = only this class can change HP (Heal / TakeDamage).
    public int HP { get; private set; }

    // Upper limit for HP. The UI reads this to draw the bar as current/max.
    public int MaxHP { get; private set; }

    // protected set = this class and subclasses can rename a hero; callers outside cannot.
    public string Name { get; protected set; }
    public int Level { get; protected set; }

    // A boolean the UI and Party can check without throwing.
    // => means "this is always computed as HP > 0"; we do not store a separate field.
    public bool IsAlive => HP > 0;

    // The real backpack. private = other classes cannot Add/Remove directly; they use AddItem().
    // readonly = this field always points at the same list object (the contents can still change).
    // [] is a collection expression: start with an empty list.
    private readonly List<Item> _inventory = [];

    // Weapon/armor currently worn, keyed by slot. Same privacy idea as _inventory.
    private readonly Dictionary<EquipmentSlot, Item> _equippedItems = [];

    // Read-only views for the UI: it can Count and foreach, but cannot mutate the collections.
    public IReadOnlyList<Item> Inventory => _inventory;
    public IReadOnlyDictionary<EquipmentSlot, Item> EquippedItems => _equippedItems;

    protected Character(string name, int level, int maxHitPoints)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be empty.", nameof(name));
        }

        if (maxHitPoints <= 0)
        {
            throw new ArgumentException("Max hit points must be positive.", nameof(maxHitPoints));
        }

        Name = name;
        Level = level;
        MaxHP = maxHitPoints;
        // A new character starts at full health.
        HP = maxHitPoints;
    }

    // Math.Min keeps HP from going above MaxHP (a big potion cannot overheal).
    public void Heal(int amount)
    {
        HP = Math.Min(MaxHP, HP + amount);
    }

    // Math.Max keeps HP from going below 0 (the UI can then show 0/max instead of negative HP).
    public void TakeDamage(int damage)
    {
        HP = Math.Max(0, HP - damage);
    }

    public void AddItem(Item item)
    {
        _inventory.Add(item);
    }

    // Only drink the potion if it is actually in the backpack, then remove it.
    public void UsePotion(Potion potion)
    {
        if (_inventory.Contains(potion))
        {
            Heal(potion.HealAmount);
            _inventory.Remove(potion);
        }
    }

    // Equip does nothing unless the item is already in the inventory.
    public void Equip(Item item, EquipmentSlot slot)
    {
        if (_inventory.Contains(item))
        {
            _equippedItems[slot] = item;
        }
    }

    // Defeated characters cannot act. Query IsAlive to test; this throw is for illegal actions.
    public void Attack(IDamageable target, IDiceRoller diceRoller)
    {
        if (!IsAlive)
        {
            throw new CharacterIsDefeatedException(Name);
        }

        target.TakeDamage(diceRoller.Roll(9));
    }
}
