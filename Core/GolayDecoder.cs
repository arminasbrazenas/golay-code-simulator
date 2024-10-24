using GolayCodeSimulator.Core.Helpers;

namespace GolayCodeSimulator.Core;

public class GolayDecoder
{
    private static readonly Matrix ParityCheckMatrix =
        new(Constants.IdentityMatrix.Concat(Constants.BMatrix).ToList(), columnCount: Constants.InformationLength);

    private static readonly Matrix BMatrix = new(Constants.BMatrix.ToList(), columnCount: Constants.InformationLength);

    public static List<byte> Decode(List<byte> encodedMessage)
    {
        BitReader bitReader = new(encodedMessage, blockSize: Constants.CodewordLength);
        BitWriter bitWriter = new(blockSize: Constants.CodewordLength);

        while (true)
        {
            uint? codeword = bitReader.ReadNextBlock();
            if (!codeword.HasValue)
            {
                bitWriter.Flush();
                return bitWriter.Bytes;
            }

            uint oddWeightWord = CalculateOddWeightWord(codeword.Value);
            uint errorVector = CalculateErrorVector(oddWeightWord);
            uint decodedCodeword = oddWeightWord ^ errorVector;
            bitWriter.WriteBlock(decodedCodeword);
        }
    }

    private static uint CalculateErrorVector(uint oddWeightWord)
    {
        uint firstSyndrome = ParityCheckMatrix.Multiply(oddWeightWord);
        if (firstSyndrome.Weight() <= 3)
        {
            return firstSyndrome;
        }

        var row = BMatrix.FindRowWithLowerOrEqualAdditionWeight(firstSyndrome, 2);
        if (row.HasValue)
        {
            int errorBitIndex = 31 - Constants.InformationLength - row.Value.Index;
            return row.Value.AdditionResult | (uint)(1 << errorBitIndex);
        }

        uint secondSyndrome = BMatrix.Multiply(firstSyndrome);
        if (secondSyndrome.Weight() <= 3)
        {
            return secondSyndrome >> Constants.InformationLength;
        }

        row = BMatrix.FindRowWithLowerOrEqualAdditionWeight(secondSyndrome, 2);
        if (row.HasValue)
        {
            int errorBitIndex = 31 - row.Value.Index;
            return (uint)(1 << errorBitIndex) | (row.Value.AdditionResult >> Constants.InformationLength);
        }

        return 0;
    }

    private static uint CalculateOddWeightWord(uint codeword) =>
        codeword.Weight() % 2 != 0 ? codeword : codeword | (1 << (31 - Constants.CodewordLength));
}
