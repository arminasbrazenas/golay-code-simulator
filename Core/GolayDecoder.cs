using GolayCodeSimulator.Core.Helpers;

namespace GolayCodeSimulator.Core;

public class GolayDecoder
{
    private static readonly Matrix ParityCheckMatrix =
        new(Constants.IdentityMatrix.Concat(Constants.BMatrix).ToList(), columnCount: Constants.InformationLength);

    private static readonly Matrix BMatrix = new(Constants.BMatrix.ToList(), columnCount: Constants.InformationLength);

    /// <summary>
    /// Decodes the given message which was encoded using Golay code.
    /// </summary>
    /// <param name="encodedMessage">Message to decode.</param>
    /// <returns>The decoded message.</returns>
    public static List<byte> Decode(List<byte> encodedMessage)
    {
        BitReader bitReader = new(encodedMessage, blockSize: Constants.CodewordLength);
        BitWriter bitWriter = new(blockSize: Constants.CodewordLength);

        while (true)
        {
            uint? word = bitReader.ReadNextBlock();
            if (!word.HasValue)
            {
                bitWriter.Flush();
                return bitWriter.Bytes;
            }

            uint oddWeightWord = FormOddWeightWord(word.Value);
            uint errorVector = CalculateErrorVector(oddWeightWord);
            uint decodedCodeword = oddWeightWord ^ errorVector;

            bitWriter.WriteBlock(decodedCodeword);
        }
    }

    /// <summary>
    /// Calculates the error vector given an odd weight word.
    /// </summary>
    /// <param name="oddWeightWord">Odd weight word.</param>
    /// <returns>The error vector.</returns>
    private static uint CalculateErrorVector(uint oddWeightWord)
    {
        // Step 1: Compute the first syndrome
        uint firstSyndrome = ParityCheckMatrix.Multiply(oddWeightWord);

        // Step 2: Check if the first syndrome's weight is <= 3
        if (firstSyndrome.Weight() <= 3)
        {
            // The error vector is [first syndrome; 0]
            return firstSyndrome;
        }

        // Step 3: Find B matrix row with first syndrome's addition weight <= 2
        var row = BMatrix.FindRowWithLowerOrEqualAdditionWeight(firstSyndrome, 2);
        if (row.HasValue)
        {
            // The error vector is [addition result; zeros with row position error bit]
            int errorBitIndex = 31 - Constants.InformationLength - row.Value.Index;
            return row.Value.AdditionResult | (uint)(1 << errorBitIndex);
        }

        // Step 4: Compute the second syndrome
        uint secondSyndrome = BMatrix.Multiply(firstSyndrome);

        // Step 5: Check if the second syndrome's weight is <= 3
        if (secondSyndrome.Weight() <= 3)
        {
            // The error vector is [0; second syndrome]
            return secondSyndrome >> Constants.InformationLength;
        }

        // Step 6: Find B matrix row with second syndrome's addition weight <= 2
        row = BMatrix.FindRowWithLowerOrEqualAdditionWeight(secondSyndrome, 2);
        if (row.HasValue)
        {
            // The error vector is [zeros with row position error bit; addition result]
            int errorBitIndex = 31 - row.Value.Index;
            return (uint)(1 << errorBitIndex) | (row.Value.AdditionResult >> Constants.InformationLength);
        }

        return 0;
    }

    /// <summary>
    /// Appends the digit 0 or 1 to the given word, so that the resulting word has an odd weight.
    /// </summary>
    /// <param name="word">Word to append the digit to.</param>
    /// <returns>The formed word with an odd weight.</returns>
    private static uint FormOddWeightWord(uint word) => word.Weight() % 2 != 0 ? word : word | (1 << (31 - Constants.CodewordLength));
}
