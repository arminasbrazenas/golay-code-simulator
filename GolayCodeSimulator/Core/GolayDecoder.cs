using System;
using System.Collections.Generic;
using System.Linq;
using GolayCodeSimulator.Helpers;

namespace GolayCodeSimulator.Core;

public class GolayDecoder
{
    private static readonly List<uint> TransposedParityCheckMatrix =
        CalculateParityCheckMatrix().Transpose(columnCount: Constants.InformationLength);

    private static readonly List<uint> TransposedBMatrix =
        Constants.BMatrix.ToList().Transpose(columnCount: Constants.InformationLength);

    public static List<byte> Decode(List<byte> encodedMessage)
    {
        var bitReader = new BitReader(encodedMessage, Constants.CodewordLength);
        var bitWriter = new BitWriter(Constants.CodewordLength);

        while (true)
        {
            uint? codeword = bitReader.NextBlock();
            if (!codeword.HasValue)
            {
                break;
            }

            uint oddWeightWord = CalculateOddWeightWord(codeword.Value);
            uint? errorVector = CalculateErrorVector(oddWeightWord);

            if (errorVector.HasValue)
            {
                uint decodedCodeword = oddWeightWord ^ errorVector.Value;
                bitWriter.WriteBlock(decodedCodeword);
            }
            else
            {
                throw new InvalidOperationException($"Failed to decode: {codeword}");
            }
        }
        
        bitWriter.Flush();
        
        return bitWriter.Bytes;
    }

    private static uint? CalculateErrorVector(uint oddWeightWord)
    {
        uint firstSyndrome = TransposedParityCheckMatrix.TransposedMatrixMultiply(oddWeightWord);
        if (firstSyndrome.Weight() <= 3)
        {
            return firstSyndrome;
        }
        
        int? rowIndex = FindBMatrixRowIndexWithAdditionWeightLowerOrEqualToTwo(firstSyndrome);
        if (rowIndex.HasValue)
        {
            return (firstSyndrome ^ Constants.BMatrix[rowIndex.Value]) | (uint)(1 << (31 - Constants.InformationLength - rowIndex.Value));
        }
        
        uint secondSyndrome = TransposedBMatrix.TransposedMatrixMultiply(firstSyndrome);
        if (secondSyndrome.Weight() <= 3)
        {
            return secondSyndrome >> Constants.InformationLength;
        }
        
        rowIndex = FindBMatrixRowIndexWithAdditionWeightLowerOrEqualToTwo(secondSyndrome);
        if (rowIndex.HasValue)
        {
            return (uint)(1 << (31 - rowIndex.Value)) | ((secondSyndrome ^ Constants.BMatrix[rowIndex.Value]) >> Constants.InformationLength);
        }

        return null;
    }

    private static uint CalculateOddWeightWord(uint codeword)
    {
        return codeword.Weight() % 2 == 0 ? codeword | (1 << 8) : codeword;
    }
    
    private static int? FindBMatrixRowIndexWithAdditionWeightLowerOrEqualToTwo(uint vector)
    {
        for (var i = 0; i < Constants.BMatrix.Length; i++)
        {
            uint addedVectors = Constants.BMatrix[i] ^ vector;
            if (addedVectors.Weight() <= 2)
            {
                return i;
            }
        }

        return null;
    }

    private static List<uint> CalculateParityCheckMatrix()
    {
        return Constants.IdentityMatrix
            .Concat(Constants.BMatrix)
            .ToList();
    }
}