using System.Collections.Generic;
using GolayCodeSimulator.Helpers;

namespace GolayCodeSimulator.Core;

public class GolayEncoder
{
    private static readonly List<uint> TransposedGeneratorMatrix = 
        CreateGeneratorMatrix().Transpose(columnCount: Constants.CodewordLength);

    public static List<byte> Encode(List<byte> message)
    {
        var bitWriter = new BitWriter(Constants.CodewordLength);
        var byteOffset = 0;
        var isOddBlock = true;

        while (byteOffset + 1 < message.Count)
        {
            var informationBits = GetInformationBits(message, byteOffset, isOddBlock);
            var codeword = TransposedGeneratorMatrix.TransposedMatrixMultiply(informationBits);
            bitWriter.WriteBlock(codeword);
            
            byteOffset += isOddBlock ? 1 : 2;
            isOddBlock = !isOddBlock;
        }
        
        bitWriter.Flush();
        
        return bitWriter.Bytes;
    }

    private static uint GetInformationBits(List<byte> message, int byteOffset, bool isOddBlock)
    {
        if (isOddBlock)
        {
            return ((uint)message[byteOffset] << 24) | ((uint)(message[byteOffset + 1] & 0xF0) << 16);
        }
        
        return ((uint)(message[byteOffset] & 0x0F) << 28) | ((uint)message[byteOffset + 1] << 20);
    }

    private static List<uint> CreateGeneratorMatrix()
    {
        List<uint> matrix = [];
        for (var i = 0; i < Constants.InformationLength; i++)
        {
            var row = Constants.IdentityMatrix[i] | (Constants.BMatrix[i] >> Constants.InformationLength);
            matrix.Add(row.ClearBit(8)); // last bit of B matrix is not used when using perfect Golay code
        }

        return matrix;
    }
}