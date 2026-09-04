namespace Eventyrerlauget.Dice;

/// <summary>
/// Provides random dice rolls using a random number generator.
/// </summary>
internal class RandomDiceRoller : IDiceRoller
{
    private readonly Random _random = new Random();

    /// <summary>
    /// Rolls a die with the specified number of sides.
    /// </summary>
    /// <param name="sides">The number of sides on the die.</param>
    /// <returns>A random number between 1 and the number of sides.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the number of sides is less than or equal to zero.
    /// </exception>
    public int Roll(int sides)
    {
        if (sides <= 0)
        {
            throw new ArgumentException(
                "The number of sides must be greater than 0.");
            /*
             Error handling suggestion for where Roll is called:
            int sides = 0;
            while (sides<=0)
            {
            Console.Write("Please input number of sides on the die: ");
            if (int.TryParse(Console.ReadLine(), out sides)) sides = sidesInput;
            }
            IDiceRoller dice = new RandomDiceRoller();

            int result = dice.Roll(sides);
             */
        }
        int min = 1;
        int max = sides + 1;
        return _random.Next(min, max);
    }
}
