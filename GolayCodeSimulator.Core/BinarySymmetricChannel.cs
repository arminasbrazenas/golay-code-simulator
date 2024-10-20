using GolayCodeSimulator.Core.Helpers;

namespace GolayCodeSimulator.Core;

public class BinarySymmetricChannel
{
    private static Random _random = new();
    
    public static List<byte> SimulateNoise(
        IEnumerable<byte> message,
        double bitFlipProbability,
        int? seed = null)
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