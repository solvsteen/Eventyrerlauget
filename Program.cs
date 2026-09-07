using Eventyrerlauget.Characters;
using Eventyrerlauget.Combat;
using Eventyrerlauget.Dice;
using Eventyrerlauget.Items;

namespace Eventyrerlauget;

class Program
{
    static void Main(string[] args)
    {
        // do/while: run one full adventure, then ask whether to start a new one
        // with a fresh party. "Venture forth again?" is only shown after a wipe
        // or after the last fight — not after each encounter.
        do
        {
            Ui.TitleScreen();

            Party party = CreateParty();
            IDiceRoller dice = new RandomDiceRoller();
            Ui.DiceRollerBadge("random");
            Ui.RenderParty(party);
            Ui.PromptContinue("The woods grow quiet. Press any key to continue...");

            PlayAdventure(party, dice);

            Ui.Spacer();
            Ui.RenderParty(party);
        } while (Ui.Confirm("Venture forth again?"));
    }

    // Walks the same party through every fight in order. HP, mana, backpack,
    // and fallen heroes carry from one encounter to the next.
    static void PlayAdventure(Party party, IDiceRoller dice)
    {
        // Func<List<Monster>> = a function that creates the monsters when called.
        // We do not build them all up front; each fight gets a fresh list.
        (string Name, Func<List<Monster>> Monsters)[] encounters =
        [
            ("A goblin ambush!", () =>
            [
                new Monster("Goblin", 8, poisonChancePercent: 25),
                new Monster("Wolf", 10),
            ]),
            ("Bandits on the road", () =>
            [
                new Monster("Bandit", 14),
                new Monster("Cutpurse", 11),
            ]),
            ("The spider's den", () =>
            [
                new Monster("Giant Spider", 18, poisonChancePercent: 40),
                new Monster("Spiderling", 8, poisonChancePercent: 20),
            ]),
            ("The ogre's clearing", () =>
            [
                new Monster("Ogre", 24),
                new Monster("Goblin Brute", 12, poisonChancePercent: 15),
            ]),
        ];

        for (int i = 0; i < encounters.Length; i++)
        {
            bool isLastEncounter = i == encounters.Length - 1;
            // Monsters() runs the lambda and returns a new List<Monster> for this fight.
            var encounter = new Encounter(encounters[i].Name, party, encounters[i].Monsters(), dice);

            // Play() returns false when the whole party is down — the adventure ends.
            if (!encounter.Play())
            {
                Ui.Spacer();
                Ui.DefeatBanner();
                return;
            }

            // Big VICTORY banner only after the last fight, not after every skirmish.
            if (isLastEncounter)
            {
                Ui.Spacer();
                Ui.VictoryBanner();
                return;
            }

            Ui.Spacer();
            Ui.SectionRule("The path continues", "springgreen2");
            CatchBreath(party);
            Ui.RenderParty(party);
            Ui.PromptContinue("Press any key to march on...");
        }
    }

    // A short rest between fights: poison fades, living heroes recover a little,
    // and someone finds a potion. Dead companions stay down. HP and gear carry on.
    static void CatchBreath(Party party)
    {
        foreach (Character hero in party.Members)
        {
            if (!hero.IsAlive)
            {
                continue;
            }

            hero.CurePoison();
            hero.Heal(4);
            // Pattern matching: only a Sorcerer is an ISpellcaster, so only they regain mana.
            if (hero is ISpellcaster caster)
            {
                caster.RestoreMana(5);
            }
        }

        // OrderBy(...).FirstOrDefault() = the living hero with the lowest HP,
        // or null if nobody is left (should not happen if we just won the fight).
        Character? looter = party.AliveMembers()
            .OrderBy(hero => hero.HP)
            .FirstOrDefault();
        if (looter is not null)
        {
            looter.AddItem(new Potion("Healing", 8));
            Ui.Narrate("The party catches its breath. Wounds close a little.");
            Ui.Narrate($"{looter.Name} finds a Healing potion.");
        }
    }

    static Party CreateParty()
    {
        // AskName offers a default (Bo / Luna / Ash). Press Enter to keep it.
        string fighterName = Ui.AskName("Fighter", "Bo");
        string sorcererName = Ui.AskName("Sorcerer", "Luna");
        string rangerName = Ui.AskName("Ranger", "Ash");

        // Arguments: name, level, max HP (Sorcerer also takes max mana).
        var fighter = new Fighter(fighterName, 1, 20);
        var sorcerer = new Sorcerer(sorcererName, 1, 12, 15);
        var ranger = new Ranger(rangerName, 1, 16);

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
        return party;
    }
}
