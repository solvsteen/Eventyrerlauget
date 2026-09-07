using Eventyrerlauget.Characters;
using Eventyrerlauget.Dice;
using Eventyrerlauget.Exceptions;
using Eventyrerlauget.Items;
using Eventyrerlauget;

namespace Eventyrerlauget.Combat;

/// <summary>
/// A single fight between the adventuring party and a list of monsters.
/// Play() asks the player what each living hero should do, then lets the
/// monsters strike back, until one side is gone.
/// </summary>
internal class Encounter
{
    private readonly Party _party;
    private readonly List<Monster> _monsters;
    private readonly IDiceRoller _dice;

    /// <param name="name">Title shown on the encounter intro (e.g. "A goblin ambush!").</param>
    /// <param name="party">The party participating in the encounter.</param>
    /// <param name="monsters">The monsters participating in the encounter.</param>
    /// <param name="die">The dice roller used for attacks, poison, and random targeting.</param>
    internal Encounter(string name, Party party, List<Monster> monsters, IDiceRoller die)
    {
        ArgumentNullException.ThrowIfNull(party);
        ArgumentNullException.ThrowIfNull(monsters);
        ArgumentNullException.ThrowIfNull(die);

        Name = name;
        _party = party;
        _monsters = monsters;
        _dice = die;
    }

    internal string Name { get; }

    public Party Party => _party;

    /// <summary>
    /// Runs rounds until the party or the monsters are defeated.
    /// Each living hero chooses an action; then each living monster attacks back.
    /// </summary>
    /// <returns><c>true</c> if the party won; <c>false</c> if the party was defeated.</returns>
    internal bool Play()
    {
        Ui.EncounterIntro(_monsters, Name);
        Ui.PromptContinue();

        int round = 1;
        while (!IsOver())
        {
            // Clear so the HUD, log, and prompts fit on one screen (see Ui.RenderCombatStatus).
            Ui.Clear();
            Ui.RoundRule(round);
            Ui.RenderCombatStatus(_party, _monsters);
            Ui.Spacer();

            PlayHeroTurns();

            // Skip monster turns if the last hero just killed the final foe.
            if (!IsOver())
            {
                PlayMonsterTurns();
            }

            Ui.Spacer();
            Ui.RenderCombatStatus(_party, _monsters);
            Ui.Spacer();

            if (!IsOver())
            {
                Ui.PromptContinue();
            }

            round++;
        }

        return !_party.IsDefeated();
    }

    // One pass over the party: poison ticks, then the player picks an action.
    private void PlayHeroTurns()
    {
        foreach (Character hero in _party.Members)
        {
            if (IsOver())
            {
                return;
            }

            if (!hero.IsAlive)
            {
                continue;
            }

            // TickPoison returns "" when the hero is not poisoned; Narrate ignores blanks.
            Ui.Narrate(hero.TickPoison());
            if (!hero.IsAlive)
            {
                continue;
            }

            if (IsOver())
            {
                return;
            }

            PlayHeroTurn(hero);
        }
    }

    // Prompt the player, then run Attack / CastSpell / DrinkPotion on this hero.
    private void PlayHeroTurn(Character hero)
    {
        List<Monster> livingMonsters = LivingMonsters();
        if (livingMonsters.Count == 0)
        {
            return;
        }

        HeroAction action = Ui.ChooseHeroAction(hero);
        try
        {
            switch (action)
            {
                case HeroAction.Attack:
                {
                    Monster target = Ui.ChooseMonster($"Who should {hero.Name} attack?", livingMonsters);
                    Ui.Narrate(hero.Attack(target, _dice));
                    NarrateIfDefeated(target);
                    break;
                }
                case HeroAction.CastSpell:
                {
                    // Pattern matching: only a Sorcerer implements ISpellcaster.
                    if (hero is ISpellcaster caster)
                    {
                        Monster target = Ui.ChooseMonster($"Who should {hero.Name} blast?", livingMonsters);
                        Ui.Narrate(caster.CastSpell(target, _dice));
                        NarrateIfDefeated(target);
                    }
                    break;
                }
                case HeroAction.DrinkPotion:
                {
                    List<Potion> potions = hero.Potions().ToList();
                    if (potions.Count == 0)
                    {
                        Ui.Narrate($"{hero.Name} fumbles in an empty pack and cannot act.");
                        break;
                    }

                    Potion potion = Ui.ChoosePotion(hero, potions);
                    Ui.Narrate(hero.UsePotion(potion));
                    break;
                }
            }
        }
        catch (CharacterIsDefeatedException ex)
        {
            Ui.Narrate(ex.Message);
        }
        catch (InsufficientManaException ex)
        {
            // CastSpell throws this when mana is too low; the hero spends the turn fumbling.
            Ui.Narrate(ex.Message);
        }
    }

    private void PlayMonsterTurns()
    {
        foreach (Monster monster in _monsters)
        {
            if (IsOver())
            {
                return;
            }

            if (!monster.IsAlive)
            {
                continue;
            }

            Character? target = RandomLivingHero();
            if (target is null)
            {
                return;
            }

            try
            {
                Ui.Narrate(monster.Attack(target, _dice));
                NarrateIfDefeated(target);
            }
            catch (CharacterIsDefeatedException ex)
            {
                Ui.Narrate(ex.Message);
            }
        }
    }

    // A second log line so "defeated" / "falls" can be colored red by Ui.Narrate.
    private static void NarrateIfDefeated(IDamageable target)
    {
        if (!target.IsAlive)
        {
            Ui.Narrate($"{target.Name} is defeated and falls.");
        }
    }

    // Pick a living hero at random. Roll is 1..count, so subtract 1 for a list index.
    private Character? RandomLivingHero()
    {
        List<Character> alive = _party.AliveMembers().ToList();
        if (alive.Count == 0)
        {
            return null;
        }

        int index = _dice.Roll(alive.Count) - 1;
        return alive[index];
    }

    private List<Monster> LivingMonsters() => _monsters.Where(monster => monster.IsAlive).ToList();

    /// <summary>
    /// Determines whether the encounter is over.
    /// </summary>
    /// <returns>
    /// <c>true</c> if either the party has no living members or all monsters are defeated;
    /// otherwise, <c>false</c>.
    /// </returns>
    private bool IsOver()
    {
        return !_party.AliveMembers().Any() ||
               !_monsters.Any(monster => monster.IsAlive);
    }
}
