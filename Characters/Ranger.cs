namespace Eventyrerlauget.Characters;

// Ranged/skirmish hero. Same combat rules as Fighter; the demo party gives this class the potion.
public class Ranger : Character
{
    public Ranger(string name, int level, int maxHitPoints)
        : base(name, level, maxHitPoints)
    {
    }
}
