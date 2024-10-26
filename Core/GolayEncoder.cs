using GolayCodeSimulator.Core.Helpers;

namespace GolayCodeSimulator.Core;

public class GolayEncoder
{
    private static readonly Matrix GeneratorMatrix = new(CreateGeneratorMatrix(), columnCount: Constants.CodewordLength);

    /// <summary>
    /// Encodes the given message using Golay code.
    /// </summary>
    /// <param name="message">Message to encode.</param>
    /// <returns>Encoded message.</returns>
    public static List<byte> Encode(List<byte> message)
    {
        BitReader bitReader = new(message, blockSize: Constants.InformationLength);
        BitWriter bitWriter = new(blockSize: Constants.CodewordLength);

        while (true)
        {
            uint? informationBits = bitReader.ReadNextBlock();
            if (informationBits is null)
            {
                bitWriter.Flush();
                return bitWriter.Bytes;
            }

            uint codeword = GeneratorMatrix.Multiply(informationBits.Value);

            bitWriter.WriteBlock(codeword);
        }
    }

    /// <summary>
    /// Creates Golay code's generator matrix.
    /// </summary>
    /// <returns>The generator matrix.</returns>
    private static List<uint> CreateGeneratorMatrix()
    {
        List<uint> matrix = [];
        for (int i = 0; i < Constants.InformationLength; i++)
        {
            var row = Constants.IdentityMatrix[i] | (Constants.BMatrix[i] >> Constants.InformationLength);
            row = row.ClearBit(31 - Constants.CodewordLength); // last bit of B matrix is not used during perfect binary Golay code encoding
            matrix.Add(row);
        }

        return matrix;
    }
}
