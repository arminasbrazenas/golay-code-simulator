using GolayCodeSimulator.Core.Helpers;

namespace GolayCodeSimulator.Core;

public class GolayEncoder
{
    private static readonly Matrix GeneratorMatrix = new(CreateGeneratorMatrix(), columnCount: Constants.CodewordLength);

    public static List<byte> Encode(List<byte> message)
    {
        BitReader bitReader = new(message, Constants.InformationLength);
        BitWriter bitWriter = new(Constants.CodewordLength);

        while (true)
        {
            uint? informationBits = bitReader.NextBlock();
            if (informationBits is null)
            {
                bitWriter.Flush();
                return bitWriter.Bytes;
            }
            
            uint codeword = GeneratorMatrix.Multiply(informationBits.Value);
            bitWriter.WriteBlock(codeword);
        }
    }

    private static List<uint> CreateGeneratorMatrix()
    {
        List<uint> matrix = [];
        for (int i = 0; i < Constants.InformationLength; i++)
        {
            var row = Constants.IdentityMatrix[i] | (Constants.BMatrix[i] >> Constants.InformationLength);
            row = row.ClearBit(31 - Constants.CodewordLength); // last bit of B matrix is not used during perfect Golay code encoding
            matrix.Add(row);
        }

        return matrix;
    }
}