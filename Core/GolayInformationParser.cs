using GolayCodeSimulator.Core.Helpers;

namespace GolayCodeSimulator.Core;

public static class GolayInformationParser
{
    /// <summary>
    /// Parses information bytes from the decoded message.
    /// </summary>
    /// <param name="decodedMessage">Decoded message to parse information bytes from.</param>
    /// <returns>Parsed information bytes.</returns>
    public static List<byte> ParseDecodedMessage(List<byte> decodedMessage)
    {
        BitReader bitReader = new(decodedMessage, blockSize: Constants.CodewordLength);
        BitWriter bitWriter = new(blockSize: Constants.InformationLength);

        while (true)
        {
            uint? codeword = bitReader.ReadNextBlock();
            if (codeword is null)
            {
                bitWriter.Flush();
                return bitWriter.Bytes;
            }

            bitWriter.WriteBlock(codeword.Value);
        }
    }
}
