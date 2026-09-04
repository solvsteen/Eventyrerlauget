namespace Eventyrerlauget.Dice;
/// <summary>
/// Represents an object that can roll a die.
/// </summary>
public interface IDiceRoller
{
    /// <summary>
    /// Rolls a die with the specified number of sides.
    /// </summary>
    /// <param name="sides">The number of sides on the die.</param>
    /// <returns>A random or predetermined number between 1 and the specified number of sides</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the number of sides is less than or equal to zero.
    /// </exception>
    int Roll(int sides);
}