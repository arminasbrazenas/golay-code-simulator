using System.Collections.Generic;

namespace GolayCodeSimulator.Core;

public class GolayEncoder
{
    private const int CodewordLength = 23;
    private uint[] _generatorMatrix;

    public GolayEncoder()
    {
        _generatorMatrix = ComputeGeneratorMatrix();
    }
    
    private static readonly uint[] GeneratorMatrix = 
    [
        0b1101_1100_0101_0000_0000_0000_0000_0000,
        0b1011_1000_1011_0000_0000_0000_0000_0000,
        0b0111_0001_0111_0000_0000_0000_0000_0000,
        0b1110_0010_1101_0000_0000_0000_0000_0000,
        0b1100_0101_1011_0000_0000_0000_0000_0000,
        0b1000_1011_0111_0000_0000_0000_0000_0000,
        0b0001_0110_1111_0000_0000_0000_0000_0000,
        0b0010_1101_1101_0000_0000_0000_0000_0000,
        0b0101_1011_1001_0000_0000_0000_0000_0000,
        0b1011_0111_0001_0000_0000_0000_0000_0000,
        0b0110_1110_0011_0000_0000_0000_0000_0000
    ];
    
    public IList<byte> Encode(IList<byte> message)
    {
        List<byte> encodedMessage = [];

        uint buffer = 0;
        int byteOffset = 0, bufferSize = 0;
        var isOddBlock = true;

        while (byteOffset + 1 < message.Count)
        {
            var informationBits = GetInformationBits(message, byteOffset, isOddBlock);
            var checkBits = CalculateCheckBits(informationBits);
            
            var codeword = informationBits | checkBits;
            buffer |= codeword >> bufferSize;

            uint newBuffer = 0;
            var newBufferSize = 0;
            if (bufferSize + CodewordLength <= 32)
            {
                bufferSize += CodewordLength;
            }
            else
            {
                var bitsWritten = 32 - bufferSize;
                bufferSize = 32;
                newBuffer = codeword << bitsWritten;
                newBufferSize = CodewordLength - bitsWritten;
            }

            if (bufferSize == 32)
            {
                FlushBuffer(encodedMessage, buffer, bufferSize);
                buffer = newBuffer;
                bufferSize = newBufferSize;
            }

            byteOffset += isOddBlock ? 1 : 2;
            isOddBlock = !isOddBlock;
        }
        
        FlushBuffer(encodedMessage, buffer, bufferSize);
        
        return encodedMessage;
    }

    private static uint GetInformationBits(IList<byte> message, int byteOffset, bool isOddBlock)
    {
        if (isOddBlock)
        {
            return ((uint)message[byteOffset] << 24) | ((uint)(message[byteOffset + 1] & 0xF0) << 16);
        }
        
        return ((uint)(message[byteOffset] & 0x0F) << 28) | ((uint)message[byteOffset + 1] << 20);
    }

    private static uint CalculateCheckBits(uint informationBits)
    {
        uint checkBits = 0;
        for (var i = 0; i < GeneratorMatrix.Length; i++)
        {
            var checkBit = CalculateCheckBit(informationBits, GeneratorMatrix[i]);
            checkBits |= checkBit << (19 - i);
        }

        return checkBits;
    }

    private static uint CalculateCheckBit(uint informationBits, uint generatorBits)
    {
        return Utilities.CountSetBits(informationBits & generatorBits) % 2;
    }
    
    private static void FlushBuffer(List<byte> encodedMessage, uint buffer, int bufferSize)
    {
        var shift = 24;
        while (bufferSize > 0 && shift >= 0)
        {
            encodedMessage.Add((byte)(buffer >> shift));
            bufferSize -= 8;
            shift -= 8;
        }
    }

    private static uint[] ComputeGeneratorMatrix()
    {
        var matrix = new uint[GolayConstants.MessageLength];
        for (var i = 0; i < GolayConstants.MessageLength; i++)
        {
            matrix[i] = GolayConstants.IMatrix[i] | (GolayConstants.BMatrix[i] >> GolayConstants.MessageLength);
        }
        
        return matrix;
    }
}