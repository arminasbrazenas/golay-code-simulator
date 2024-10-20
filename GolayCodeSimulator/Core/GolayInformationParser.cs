using System.Collections.Generic;
using GolayCodeSimulator.Helpers;

namespace GolayCodeSimulator.Core;

public static class GolayInformationParser
{
    public static List<byte> ParseDecodedMessage(List<byte> decodedMessage)
    {
        BitReader bitReader = new(decodedMessage, Constants.CodewordLength);
        BitWriter bitWriter = new(Constants.InformationLength);

        while (true)
        {
            uint? codeword = bitReader.NextBlock();
            if (codeword is null)
            {
                bitWriter.Flush();
                return bitWriter.Bytes;
            }
            
            bitWriter.WriteBlock(codeword.Value);
        }
    }
}