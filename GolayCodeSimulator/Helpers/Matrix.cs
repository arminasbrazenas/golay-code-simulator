using System.Collections.Generic;

namespace GolayCodeSimulator.Helpers;

public class Matrix
{
    private readonly List<uint> _matrix;
    private readonly List<uint> _transposedMatrix;
    
    public Matrix(List<uint> matrix, int columnCount)
    {
        _matrix = matrix;
        _transposedMatrix = Transpose(matrix, columnCount);
    }
    
    public uint Multiply(uint value)
    {
        uint result = 0;
        for (var i = 0; i < _transposedMatrix.Count; i++)
        {
            var bit = (value & _transposedMatrix[i]).Weight() % 2;
            result |= bit << (31 - i);
        }
        
        return result;
    }

    public (int Index, uint AdditionResult)? FindRowWithLowerOrEqualAdditionWeight(uint value, int weight)
    {
        for (var i = 0; i < _matrix.Count; i++)
        {
            uint additionResult = _matrix[i] ^ value;
            if (additionResult.Weight() <= weight)
            {
                return (i, additionResult);
            }
        }

        return null;
    }
    
    private static List<uint> Transpose(List<uint> matrix, int columnCount)
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
}