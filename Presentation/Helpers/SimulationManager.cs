using System.Collections.Generic;
using GolayCodeSimulator.Core;

namespace GolayCodeSimulator.Presentation.Helpers;

public static class SimulationManager
{
    /// <summary>
    /// Encodes information bytes using Golay code, sends it through a binary symmetric channel
    /// and decodes the received information bytes.
    /// </summary>
    /// <param name="bytes">Information to send through channel.</param>
    /// <param name="bitFlipProbability">Probability of a bit flip.</param>
    /// <param name="seed">Seed to use for random number generator in the channel.</param>
    /// <returns>Information bytes received from channel.</returns>
    public static List<byte> SendThroughChannel(List<byte> bytes, double bitFlipProbability, int seed)
    {
        // An additional zero is needed at the end if message's length does not divide evenly.
        // This ensures that the last byte is not lost when encoding.
        var isZeroPaddingNeeded = (bytes.Count * 8) % Constants.InformationLength != 0;
        if (isZeroPaddingNeeded)
        {
            bytes.Add(0);
        }

        var encodedBytes = GolayEncoder.Encode(bytes);
        var bytesFromChannel = BinarySymmetricChannel.Simulate(encodedBytes, bitFlipProbability, seed);
        var decodedBytes = GolayDecoder.Decode(bytesFromChannel);
        var informationBytes = GolayInformationParser.ParseDecodedMessage(decodedBytes);

        if (isZeroPaddingNeeded)
        {
            informationBytes.RemoveAt(informationBytes.Count - 1);
            bytes.RemoveAt(bytes.Count - 1);
        }

        return informationBytes;
    }
}
