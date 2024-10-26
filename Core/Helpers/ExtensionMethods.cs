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

    /// <summary>
    /// Checks if a specific bit is set in the given value.
    /// </summary>
    /// <param name="value">Value in which to check the bit.</param>
    /// <param name="bitIndex">Zero-based index of the bit to check (0 is the least significant bit).</param>
    /// <returns>True if the specified bit is set; otherwise, false.</returns>
    public static bool IsBitSet(this uint value, int bitIndex) => (value & (1 << bitIndex)) != 0;

    /// <summary>
    /// Clears a specific bit in the given value.
    /// </summary>
    /// <param name="value">Value in which to clear the bit.</param>
    /// <param name="bitIndex">Zero-based index of the bit to clear (0 is the least significant bit).</param>
    /// <returns>Value with the specified bit cleared.</returns>
    public static uint ClearBit(this uint value, int bitIndex) => (uint)(value & ~(1 << bitIndex));

    /// <summary>
    /// Flips a specific bit in the given value.
    /// </summary>
    /// <param name="value">Byte in which to flip te bit.</param>
    /// <param name="bitIndex">Zero-based index of the bit to flip (0 is the least significant bit).</param>
    /// <returns>Byte with the specified bit flipped.</returns>
    public static byte FlipBit(this byte value, int bitIndex) => (byte)(value ^ (1 << (7 - bitIndex)));
}
