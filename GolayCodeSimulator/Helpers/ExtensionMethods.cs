using System.Collections.Generic;

namespace GolayCodeSimulator.Helpers;

public static class ExtensionMethods
{
    public static List<uint> Transpose(this List<uint> matrix, int columnCount)
    {
        List<uint> transposedMatrix = [];
        
        for (var col = 0; col < columnCount; col++)
        {
            uint transposedRow = 0;
            for (var row = 0; row < matrix.Count; row++)
            {
                transposedRow |= matrix[row].IsBitSet(31 - col) ? (uint)(1 << (31 - row)) : 0;
            }

            transposedMatrix.Add(transposedRow);
        }

        return transposedMatrix;
    }

    public static uint TransposedMatrixMultiply(this List<uint> matrix, uint value)
    {
        uint result = 0;
        for (var i = 0; i < matrix.Count; i++)
        {
            var bit = (value & matrix[i]).Weight() % 2;
            result |= bit << (31 - i);
        }
        
        return result;
    }
    
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
}