using System.Collections.Generic;

namespace GolayCodeSimulator.Core;

public static class Utilities
{
    /// <summary>
    /// Counts the number of bits that are set in the given value
    /// using Brian Kernighan’s algorithm.
    /// </summary>
    /// <param name="value">Value to count set bits for.</param>
    /// <returns>The number of set bits.</returns>
    public static uint CountSetBits(uint value)
    {
        uint count = 0;
        while (value > 0)
        {
            value &= value - 1;
            count += 1;
        }

        return count;
    }
}