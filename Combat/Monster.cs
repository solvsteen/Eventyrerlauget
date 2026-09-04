using Eventyrerlauget.Dice;
using Eventyrerlauget.Exceptions;

namespace Eventyrerlauget.Combat;


public class Monster : IDamageable
{
    public string Name { get; private set; }
    public int HP { get; private set; }
    public int MaxHP { get; private set; }
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

    public int PoisonChancePercent { get; }
    public bool IsPoisonous => PoisonChancePercent > 0;

    public Monster(string name, int maxHitPoints, int poisonChancePercent = 0)
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
        MaxHP = maxHitPoints;
        HP = maxHitPoints;
        PoisonChancePercent = poisonChancePercent;
    }

    public void Heal(int amount)
    {
        HP += amount;
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
    }

    public void Attack(IDamageable target, IDiceRoller diceRoller)
    {
        int damage = diceRoller.Roll(6);
        target.TakeDamage(damage * PoisonChancePercent);
    }
}