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

    // Set by a poisonous monster hit. TickPoison() at the start of this hero's turn.
    public bool IsPoisoned { get; private set; }

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

    // OfType<Potion>() keeps only potions from the mixed backpack (weapons and armor stay put).
    public IEnumerable<Potion> Potions() => _inventory.OfType<Potion>();

    // Only drink the potion if it is actually in the backpack, then remove it.
    // Returns a line for the combat log (the UI colors it from words like "drinks" / "heals").
    public string UsePotion(Potion potion)
    {
        if (!_inventory.Contains(potion))
        {
            return $"{Name} fumbles in the backpack and cannot act.";
        }

        int healthBefore = HP;
        Heal(potion.HealAmount);
        // restored can be less than HealAmount when the hero was already close to MaxHP.
        int restored = HP - healthBefore;
        _inventory.Remove(potion);
        return $"{Name} drinks {potion.Name} and heals {restored}.";
    }

    // Equip does nothing unless the item is already in the inventory.
    public void Equip(Item item, EquipmentSlot slot)
    {
        if (_inventory.Contains(item))
        {
            _equippedItems[slot] = item;
        }
    }

    // A poisonous bite sets this flag. Dead heroes cannot be poisoned.
    public void ApplyPoison()
    {
        if (IsAlive)
        {
            IsPoisoned = true;
        }
    }

    // Used between fights (CatchBreath) so poison does not last the whole adventure.
    public void CurePoison() => IsPoisoned = false;

    // Called at the start of this hero's turn. Empty string means nothing happened.
    public string TickPoison()
    {
        if (!IsPoisoned || !IsAlive)
        {
            return string.Empty;
        }

        const int poisonDamage = 2;
        TakeDamage(poisonDamage);
        if (!IsAlive)
        {
            IsPoisoned = false;
            return $"{Name} takes {poisonDamage} poison damage and falls.";
        }

        return $"{Name} takes {poisonDamage} poison damage.";
    }

    // Armor reduces weapon hits. Poison and other effects call TakeDamage directly.
    public int MitigateDamage(int incomingDamage)
    {
        int defense = 0;
        if (_equippedItems.TryGetValue(EquipmentSlot.Armor, out Item? item) && item is Armor armor)
        {
            defense = armor.DefenseBonus;
        }

        return Math.Max(0, incomingDamage - defense);
    }

    // Defeated characters cannot act. Query IsAlive to test; this throw is for illegal actions.
    // Returns a combat-log line. 1 on a d9 misses; 9 is a critical (double damage).
    public string Attack(IDamageable target, IDiceRoller diceRoller)
    {
        if (!IsAlive)
        {
            throw new CharacterIsDefeatedException(Name);
        }

        int roll = diceRoller.Roll(9);
        if (roll == 1)
        {
            return $"{Name} misses {target.Name}.";
        }

        int weaponBonus = 0;
        if (_equippedItems.TryGetValue(EquipmentSlot.Weapon, out Item? item) && item is Weapon weapon)
        {
            weaponBonus = weapon.DamageBonus;
        }

        int damage = roll + weaponBonus;
        // A 9 on a d9 doubles the total (dice + weapon). Armor is not applied here:
        // heroes hit monsters, and monsters have no armor slot.
        bool isCritical = roll == 9;
        if (isCritical)
        {
            damage *= 2;
        }

        target.TakeDamage(damage);

        if (isCritical)
        {
            return $"{Name} lands a critical hit on {target.Name} for {damage} damage.";
        }

        return $"{Name} hits {target.Name} for {damage} damage.";
    }
}
