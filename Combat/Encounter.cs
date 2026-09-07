using Eventyrerlauget.Characters;
using Eventyrerlauget.Dice;
using Eventyrerlauget.Exceptions;

namespace Eventyrerlauget.Combat;

/// <summary>
/// Represents a combat encounter between an adventuring party and monsters.
/// </summary>
internal class Encounter
{
    //TODO: statusEffects?

    private readonly Party _party;
    private readonly List<Monster> _monsters;
    private readonly IDiceRoller _dice;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="party">The party participating in the encounter.</param>
    /// <param name="monsters">The monsters participating in the encounter.</param>
    /// <param name="die">The dice roller used in the encounter.</param>
    internal Encounter(string name, Party party, List<Monster> monsters, IDiceRoller die)
    {
        _party = party;
        _monsters = monsters;
        _dice = die;
    }

    public Party Party => _party;

    /// <summary>
    /// Starts the encounter and continues until one side is defeated.
    /// </summary>
    internal void Play()
    {
        while (!IsDefeated())
        {
            PlayRound();
        }
    }

    /// <summary>
    /// Executes one round of combat. Each living party member attacks a living
    /// monster, and each surviving target monster immediately attacks back.
    /// </summary>
    internal void PlayRound()
    {
        foreach (Character character in _party.Members)
        {
            if (!character.IsAlive) continue;

            Monster? targetMonster = _monsters.FirstOrDefault(monster => monster.IsAlive);

            if (targetMonster == null) return;

            try
            {
                //party member attacks monster
                character.Attack(targetMonster, _dice);

                //monster hits back if still alive
                if (targetMonster.IsAlive)
                {
                    targetMonster.Attack(character, _dice);
                }
            }
            catch (CharacterIsDefeatedException ex)
            {
                Console.WriteLine($"Warning: {ex.Message}");
            }
            catch (InsufficientManaException ex)
            {
                Console.WriteLine($"Warning: {ex.Message}");
            }
        }

        //monster turns
        foreach (Monster monster in _monsters)
        {
            if (!monster.IsAlive) continue;

            Character? targetCharacter = _party.AliveMembers().FirstOrDefault();

            if (targetCharacter == null) return;

            monster.Attack(targetCharacter, _dice);

            if (targetCharacter.IsAlive)
            {
                targetCharacter.Attack(monster, _dice);
            }
        }
    }

    /// <summary>
    /// Determines whether the encounter is over.
    /// </summary>
    /// <returns>
    /// <c>true</c> if either the party has no living members or all monsters are defeated;
    /// otherwise, <c>false</c>.
    /// </returns>
    private bool IsDefeated()
    {
        return !Party.AliveMembers().Any() ||
               !_monsters.Any(monster => monster.IsAlive);
    }

}
