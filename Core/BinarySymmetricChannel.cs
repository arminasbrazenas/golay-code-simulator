using GolayCodeSimulator.Core.Helpers;

namespace GolayCodeSimulator.Core;

public class BinarySymmetricChannel
{
    private static Random _random = new();

    /// <summary>
    /// Simulates sending a message through a binary symmetric channel.
    /// </summary>
    /// <param name="message">Message to send through the channel.</param>
    /// <param name="bitFlipProbability">Probability that a bit is flipped.</param>
    /// <param name="seed">Seed to use for random number generator.</param>
    /// <returns>Message received from the channel.</returns>
    public static List<byte> Simulate(IEnumerable<byte> message, double bitFlipProbability, int? seed = null)
    {
        if (seed is not null)
        {
            _random = new Random(seed.Value);
        }

        List<byte> messageFromChannel = [];
        foreach (byte messageByte in message)
        {
            byte byteFromChannel = messageByte;

            for (int j = 0; j < 8; j++)
            {
                double randomProbability = _random.NextDouble();
                bool isError = randomProbability < bitFlipProbability;
                if (isError)
                {
                    byteFromChannel = byteFromChannel.FlipBit(j);
                }
            }

            messageFromChannel.Add(byteFromChannel);
        }

        return messageFromChannel;
    }
}
