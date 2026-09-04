namespace Eventyrerlauget.Dice;

/// <summary>
/// Provides a dice roller that always returns a predetermined value.
/// Useful for deterministic testing.
/// </summary>
public class FixedDiceRoller : IDiceRoller
{
    private readonly int _fixedValue;
    
    /// <summary>
    /// Creates a fixed dice roller with the specified value.
    /// </summary>
    /// <param name="fixedValue">The value that will be returned when the die is rolled.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the fixed value is less than or equal to zero.
    /// </exception>
    public FixedDiceRoller(int fixedValue) {
        if (fixedValue <= 0)
        {
            throw new ArgumentException("The fixed die value must be greater than 0.");
        }
        _fixedValue = fixedValue;
    }

    /// <summary>
    /// Returns the predetermined dice value.
    /// </summary>
    /// <param name="sides">The number of sides on the die.</param>
    /// <returns>The predetermined dice value.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the number of sides is less than or equal to zero,
    /// or when the fixed value is greater than the number of sides.
    /// </exception>
    public int Roll(int sides)
    {
        if (sides <= 0)
        {
            throw new ArgumentException(
                "The number of sides must be greater than 0.");
        }

        if (_fixedValue > sides)
        {
            throw new ArgumentException(
                $"The fixed value ({_fixedValue}) cannot be used on a {sides}-sided die.");
        }
        return _fixedValue;
    }
}
