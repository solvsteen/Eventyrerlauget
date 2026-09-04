using Eventyrerlauget.Combat;
using Eventyrerlauget.Dice;
using Eventyrerlauget.Exceptions;
using Eventyrerlauget.Items;

namespace Eventyrerlauget.Characters;

public abstract class Character : IDamageable
{
    public int HP { get; private set; }
    protected int MaxHP;
    public string Name { get; protected set; }
    public int Level { get; protected set; }
    public bool IsAlive
    {
        get
        {
            if(HP > 0)
            {
                return true;
            }
            else
            {
                throw new CharacterIsDefeatedException(Name);
            }
        }
    }

    int IDamageable.MaxHP => MaxHP;

    private List<Item> _inventory;
    public Dictionary<EquipmentSlot, Item> _equippedItems;

    protected Character(string name, int level, int maxHitPoints)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Navn må ikke være tomt.", nameof(name));
        }

        if (maxHitPoints <= 0)
        {
            throw new ArgumentException("Maks. livspoint skal være positivt.", nameof(maxHitPoints));
        }

        Name = name;
        Level = level;
        MaxHP = maxHitPoints;
        HP = maxHitPoints;
    }

    public void Heal(int amount)
    {
        HP += amount;
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
    }

    public void AddItem(Item item)
    {
        _inventory.Add(item);
    }

    public void UsePotion(Potion potion)
    {
        if(_inventory.Contains(potion))
        {
            Heal(potion.HealAmount);
            _inventory.Remove(potion);
        }
    }

    public void Equip(Item item, EquipmentSlot slot)
    {
        if (_inventory.Contains(item))
        {
            _equippedItems[slot] = item;
        }
    }

    public void Attack(IDamageable target, IDiceRoller diceRoller)
    {
        target.TakeDamage(diceRoller.Roll(9));
    }
}