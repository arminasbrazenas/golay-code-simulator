namespace GolayCodeSimulator.Core.Helpers;

public static class Utilities
{
    /// <summary>
    /// Calculates a mask, where the most significant bits of the given block size are set to 1, and the rest are cleared to 0.
    /// </summary>
    /// <param name="blockSize">The number of bits to keep set in the mask, starting from the most significant bit.</param>
    /// <returns>The calculated mask.</returns>
    public static uint CalculateBlockMask(int blockSize)
    {
        var bitsToClear = 32 - blockSize;
        uint mask = 0xFF_FF_FF_FF;
        for (int i = 0; i < bitsToClear; i++)
        {
            mask = mask.ClearBit(i);
        }

        return mask;
    }
}
