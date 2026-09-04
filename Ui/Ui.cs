using Spectre.Console;
using Eventyrerlauget.Characters;
using Eventyrerlauget.Combat;
using Eventyrerlauget.Items;
using Spectre.Console.Rendering;


namespace Eventyrerlauget;

// Spectre.Console helpers for party cards, combat HUD, and colored log lines.
// Lives in Eventyrerlauget (not Eventyrerlauget.Ui) so Program can call Ui.Method
// without C# treating "Ui" as a nested namespace instead of this class.
public static class Ui
{
    // Spectre markup color name, used inside "[bold gold1]...[/]" strings.
    private const string GoldMarkup = "gold1";

    public static void Spacer() => AnsiConsole.WriteLine();

    // Small label so the player can see whether dice are random or fixed.
    // Markup.Escape stops a description like "x[/]" from breaking the color tags.
    public static void DiceRollerBadge(string description)
    {
        AnsiConsole.MarkupLine($"[grey58]Dice:[/] [bold]{Markup.Escape(description)}[/]");
    }

    // A horizontal rule with a colored, centered title. colorMarkup is a Spectre
    // color name (e.g. "red3"), not player text, so it is not escaped.
    public static void SectionRule(string title, string colorMarkup)
    {
        AnsiConsole.Write(new Rule($"[bold {colorMarkup}]{Markup.Escape(title)}[/]")
            .RuleStyle(new Style(Color.Grey37))
            .Centered());
    }

    public static void RoundRule(int round)
    {
        AnsiConsole.Write(new Rule($"[bold {GoldMarkup}]Round {round}[/]")
            .RuleStyle(new Style(Color.Grey37))
            .Centered());
    }

    // One character card per hero: HP/mana bar, gear, and backpack in a colored box.
    // Columns lays the cards side by side. Expand() stretches them to fill the width.
    public static void RenderParty(Party party)
    {
        SectionRule(party.Name, GoldMarkup);
        AnsiConsole.Write(new Columns(party.Members.Select(CharacterCard)).Expand());
        AnsiConsole.WriteLine();
    }

    public static void RenderMonsters(IEnumerable<Monster> monsters)
    {
        // ToList() so we can Count without walking the sequence twice.
        List<Monster> list = monsters.ToList();
        if (list.Count == 0)
        {
            return;
        }

        AnsiConsole.Write(new Columns(list.Select(MonsterCard)).Expand());
        AnsiConsole.WriteLine();
    }

    public static void EncounterIntro(IEnumerable<Monster> monsters)
    {
        SectionRule("A battle begins!", "red3");
        RenderMonsters(monsters);
    }

    // Compact status used inside the combat loop: HP/mana only, heroes and monsters
    // side by side, so a full round (rule + log + status + "press to continue")
    // can fit on screen without scrolling.
    // IRenderable is Spectre's "anything that can be drawn" (here: Panel).
    public static void RenderCombatStatus(Party party, IEnumerable<Monster> monsters)
    {
        var cards = new List<IRenderable>();
        cards.AddRange(party.Members.Select(CompactCharacterCard));
        cards.AddRange(monsters.Select(CompactMonsterCard));
        AnsiConsole.Write(new Columns(cards).Expand());
    }

    // Color a single combat log line from characteristic words in the text that
    // Character/Monster/Encounter return. The domain knows nothing about colors;
    // this is display-only.
    public static void Narrate(string line)
    {
        string escaped = Markup.Escape(line);
        string? style = ClassifyNarration(line);
        // No match => plain indented text. Match => wrap in [style]...[/].
        AnsiConsole.MarkupLine(style is null ? $"  {escaped}" : $"  [{style}]{escaped}[/]");
    }

    // FigletText is the big ASCII-art banner at the end of a fight.
    public static void VictoryBanner()
    {
        AnsiConsole.Write(new FigletText("VICTORY!").Centered().Color(Color.SpringGreen2));
    }

    public static void DefeatBanner()
    {
        AnsiConsole.Write(new FigletText("DEFEAT").Centered().Color(Color.Red3));
    }

    // First matching keyword wins, so keep the more specific phrases above the
    // generic ones. Returns a Spectre style string, or null for default (unstyled).
    private static string? ClassifyNarration(string line)
    {
        if (line.Contains("critical", StringComparison.OrdinalIgnoreCase))
        {
            return $"bold {GoldMarkup}";
        }

        if (line.Contains("defeated", StringComparison.OrdinalIgnoreCase)
            || line.Contains("fallen", StringComparison.OrdinalIgnoreCase)
            || line.Contains("falls", StringComparison.OrdinalIgnoreCase))
        {
            return "bold red3";
        }

        if (line.Contains("poison", StringComparison.OrdinalIgnoreCase))
        {
            return "mediumpurple3";
        }

        if (line.Contains("heals", StringComparison.OrdinalIgnoreCase)
            || line.Contains("drinks", StringComparison.OrdinalIgnoreCase)
            || line.Contains("revived", StringComparison.OrdinalIgnoreCase))
        {
            return "springgreen3";
        }

        if (line.Contains("misses", StringComparison.OrdinalIgnoreCase)
            || line.Contains("glances past", StringComparison.OrdinalIgnoreCase)
            || line.Contains("finds no opening", StringComparison.OrdinalIgnoreCase))
        {
            return "grey58";
        }

        if (line.Contains("enough mana", StringComparison.OrdinalIgnoreCase)
            || line.Contains("fumbles", StringComparison.OrdinalIgnoreCase)
            || line.Contains("cannot act", StringComparison.OrdinalIgnoreCase))
        {
            return "grey70";
        }

        return null;
    }

    // Full hero card shown before combat. ClassColor is a Spectre Color for the
    // border; ClassColorMarkup is the same hue as a markup tag for the header.
    private static Panel CharacterCard(Character hero)
    {
        Color accent = ClassColor(hero);
        string accentMarkup = ClassColorMarkup(hero);
        var lines = new List<string>
        {
            $"Level {hero.Level}",
            $"HP  {Bar(hero.HP, hero.MaxHP, HpBarColor(hero.HP, hero.MaxHP))} {hero.HP}/{hero.MaxHP}",
        };

        // Pattern matching: only spellcasters (Sorcerer) have a mana bar.
        if (hero is ISpellcaster caster)
        {
            lines.Add($"MP  {Bar(caster.CurrentMana, caster.MaxMana, "deepskyblue1")} {caster.CurrentMana}/{caster.MaxMana}");
        }

        lines.Add(string.Empty);
        lines.Add($"Weapon: {DescribeEquipped(hero, EquipmentSlot.Weapon)}");
        lines.Add($"Armor: {DescribeEquipped(hero, EquipmentSlot.Armor)}");
        // Item names are escaped so a name containing [ or ] cannot inject markup.
        lines.Add(hero.Inventory.Count == 0
            ? "Backpack: empty"
            : $"Backpack: {Markup.Escape(string.Join(", ", hero.Inventory.Select(item => item.Name)))}");

        if (!hero.IsAlive)
        {
            lines.Insert(0, "[bold red3]DEFEATED[/]");
        }

        var body = new Markup(string.Join('\n', lines));
        string header = $"[bold {accentMarkup}]{Markup.Escape(hero.Name)}[/] [grey58]({Markup.Escape(hero.GetType().Name)})[/]";

        // Rounded border = standing. Double grey border = defeated.
        return new Panel(body)
            .Header(header)
            .Border(hero.IsAlive ? BoxBorder.Rounded : BoxBorder.Double)
            .BorderColor(hero.IsAlive ? accent : Color.Grey)
            .Padding(1, 0)
            .Expand();
    }

    private static Panel MonsterCard(Monster monster)
    {
        var lines = new List<string>
        {
            $"HP  {Bar(monster.HP, monster.MaxHP, HpBarColor(monster.HP, monster.MaxHP))} {monster.HP}/{monster.MaxHP}",
        };

        if (monster.IsPoisonous)
        {
            lines.Add($"[mediumpurple3]Poisonous bite ({monster.PoisonChancePercent}% chance)[/]");
        }

        if (!monster.IsAlive)
        {
            lines.Insert(0, "[bold grey58]SLAIN[/]");
        }

        var body = new Markup(string.Join('\n', lines));
        string header = $"[bold red3]{Markup.Escape(monster.Name)}[/]";

        return new Panel(body)
            .Header(header)
            .Border(monster.IsAlive ? BoxBorder.Rounded : BoxBorder.Double)
            .BorderColor(monster.IsAlive ? Color.Red3 : Color.Grey)
            .Padding(1, 0)
            .Expand();
    }

    // Narrower bars (width: 10) so many fighters fit in one combat-status row.
    private static Panel CompactCharacterCard(Character hero)
    {
        Color accent = ClassColor(hero);
        string accentMarkup = ClassColorMarkup(hero);
        var lines = new List<string>
        {
            $"HP {Bar(hero.HP, hero.MaxHP, HpBarColor(hero.HP, hero.MaxHP), width: 10)} {hero.HP}/{hero.MaxHP}",
        };

        if (hero is ISpellcaster caster)
        {
            lines.Add($"MP {Bar(caster.CurrentMana, caster.MaxMana, "deepskyblue1", width: 10)} {caster.CurrentMana}/{caster.MaxMana}");
        }

        string status = hero.IsAlive ? string.Empty : " [bold red3](defeated)[/]";
        string header = $"[bold {accentMarkup}]{Markup.Escape(hero.Name)}[/]{status}";
        var body = new Markup(string.Join('\n', lines));

        return new Panel(body)
            .Header(header)
            .Border(hero.IsAlive ? BoxBorder.Rounded : BoxBorder.Double)
            .BorderColor(hero.IsAlive ? accent : Color.Grey)
            .Padding(1, 0)
            .Expand();
    }

    private static Panel CompactMonsterCard(Monster monster)
    {
        string hpLine = $"HP {Bar(monster.HP, monster.MaxHP, HpBarColor(monster.HP, monster.MaxHP), width: 10)} {monster.HP}/{monster.MaxHP}";
        string status = monster.IsAlive ? string.Empty : " [bold grey58](slain)[/]";
        string header = $"[bold red3]{Markup.Escape(monster.Name)}[/]{status}";
        var body = new Markup(hpLine);

        return new Panel(body)
            .Header(header)
            .Border(monster.IsAlive ? BoxBorder.Rounded : BoxBorder.Double)
            .BorderColor(monster.IsAlive ? Color.Red3 : Color.Grey)
            .Padding(1, 0)
            .Expand();
    }

    // TryGetValue returns false when that slot is empty; we show an em dash instead.
    private static string DescribeEquipped(Character hero, EquipmentSlot slot)
    {
        return hero.EquippedItems.TryGetValue(slot, out Item? item)
            ? Markup.Escape(item.Describe())
            : "—";
    }

    // Border color as a Spectre Color value (Panel.BorderColor needs this type).
    private static Color ClassColor(Character hero) => hero switch
    {
        Fighter => Color.Red3,
        Sorcerer => Color.SkyBlue1,
        Ranger => Color.SpringGreen3,
        _ => Color.Gold1,
    };

    // Same hues as ClassColor, but as markup names for strings like "[bold red3]Bo[/]".
    private static string ClassColorMarkup(Character hero) => hero switch
    {
        Fighter => "red3",
        Sorcerer => "skyblue1",
        Ranger => "springgreen3",
        _ => GoldMarkup,
    };

    // e.g. "██████████░░░░░░░░░░" — a simple colored bar without pulling in a
    // whole widget dependency for something that is just a number with a color.
    private static string Bar(int current, int max, string colorName, int width = 16)
    {
        // Clamp so a weird HP value cannot request more filled blocks than width.
        int clampedCurrent = Math.Clamp(current, 0, Math.Max(max, 0));
        int filled = max <= 0 ? 0 : (int)Math.Round(width * (double)clampedCurrent / max);
        filled = Math.Clamp(filled, 0, width);

        string filledPart = new string('█', filled);
        string emptyPart = new string('░', width - filled);
        return $"[{colorName}]{filledPart}[/][grey19]{emptyPart}[/]";
    }

    // Green when healthy, gold when wounded, red when critical, grey when down.
    private static string HpBarColor(int current, int max)
    {
        if (max <= 0 || current <= 0)
        {
            return "grey37";
        }

        double ratio = (double)current / max;
        return ratio switch
        {
            < 0.3 => "red3",
            < 0.6 => "gold1",
            _ => "springgreen3",
        };
    }
}
