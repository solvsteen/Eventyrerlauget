using Eventyrerlauget.Characters;
using Eventyrerlauget.Combat;
using Eventyrerlauget.Items;

namespace Eventyrerlauget;

class Program
{
    static void Main(string[] args)
    {
        // One hero of each class. Arguments: name, level, max HP (Sorcerer also takes max mana).
        var fighter = new Fighter("Bo", 1, 20);
        var sorcerer = new Sorcerer("Luna", 1, 12, 15);
        var ranger = new Ranger("Ash", 1, 16);

        var sword = new Weapon("Longsword", 3);
        var hide = new Armor("Leather armor", 2);
        var potion = new Potion("Healing", 8);

        // Items must be in the backpack before Equip will put them in a slot.
        fighter.AddItem(sword);
        fighter.AddItem(hide);
        fighter.Equip(sword, EquipmentSlot.Weapon);
        fighter.Equip(hide, EquipmentSlot.Armor);

        ranger.AddItem(potion);

        var party = new Party("Eventyrerlauget");
        party.AddMember(fighter);
        party.AddMember(sorcerer);
        party.AddMember(ranger);

        // Full character cards: HP/mana bars, equipped gear, and backpack.
        Ui.DiceRollerBadge("random");
        Ui.RenderParty(party);

        // poisonChancePercent: 25 = this goblin can apply poison; the wolf has the default 0.
        var goblin = new Monster("Goblin", 8, poisonChancePercent: 25);
        var wolf = new Monster("Wolf", 10);
        Ui.EncounterIntro([goblin, wolf]);

        // Fake a round of hits so the compact HUD can show wounded / defeated / slain.
        fighter.TakeDamage(14);
        sorcerer.TakeDamage(12);
        wolf.TakeDamage(10);

        Ui.RoundRule(1);
        // Narrate colors lines by keywords in the text (e.g. "defeated", "heals").
        Ui.Narrate("The goblin hits Bo for 14 damage.");
        Ui.Narrate("Luna is defeated and falls.");
        Ui.Narrate("Ash drinks a potion and heals 8.");
        // Compact row used inside a fight: heroes and monsters side by side, HP only.
        Ui.RenderCombatStatus(party, [goblin, wolf]);
    }
}
