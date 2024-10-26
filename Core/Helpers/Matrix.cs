namespace GolayCodeSimulator.Core.Helpers;

public class Matrix
{
    private readonly List<uint> _matrix;
    private readonly List<uint> _transposedMatrix;

    public Matrix(List<uint> matrix, int columnCount)
    {
        _matrix = matrix;
        _transposedMatrix = Transpose(matrix, columnCount);
    }

    /// <summary>
    /// Multiplies the given value by a matrix.
    /// </summary>
    /// <param name="value">Value to multiply.</param>
    /// <returns>Multiplication result.</returns>
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

    /// <summary>
    /// Finds the first row in the matrix where the weight of the given value's and matrix row's addition is less than or equal to the specified weight.
    /// </summary>
    /// <param name="value">Value to compare against the rows in the matrix.</param>
    /// <param name="weight">Addition weight to check against.</param>
    /// <returns>Index of the matrix row and addition result if a row is found; otherwise, null.</returns>
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

    /// <summary>
    /// Transposes the given matrix with the specified column count.
    /// </summary>
    /// <param name="matrix">Matrix to transpose.</param>
    /// <param name="columnCount">The number of columns in the original matrix.</param>
    /// <returns>Transposed matrix.</returns>
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
