namespace GolayCodeSimulator.Core.Helpers;

public static class ExtensionMethods
{
    /// <summary>
    /// Counts the number of bits that are set in the given value
    /// using Brian Kernighan’s algorithm.
    /// </summary>
    /// <param name="value">Value to count set bits for.</param>
    /// <returns>The number of set bits.</returns>
    public static uint Weight(this uint value)
    {
        uint count = 0;
        while (value > 0)
        {
            value &= value - 1;
            count += 1;
        }

        return count;
    }

    public static bool IsBitSet(this uint value, int bitIndex) =>
        (value & (1 << bitIndex)) != 0;
    
    public static uint ClearBit(this uint value, int bitIndex) =>
        (uint)(value & ~(1 << bitIndex));

    public static byte FlipBit(this byte value, int bitIndex) =>
        (byte)(value ^ (1 << (7 - bitIndex)));
}