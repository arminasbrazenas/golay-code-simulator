namespace GolayCodeSimulator.Core.Helpers;

public static class Utilities
{
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