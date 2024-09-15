namespace GolayCode.Core.Channel;

public class BinarySymmetricChannel : IChannel
{
    private static readonly Random Random = new();
    
    public byte[] SimulateNoise(byte[] message, double errorProbability)
    {
        for (var i = 0; i < message.Length; i++)
        {
            for (var j = 1; j <= 8; j++)
            {
                var randomProbability = Random.NextDouble();
                var isError = randomProbability < errorProbability;

                if (isError)
                {
                    message[i] = FlipBit(message[i], j);
                }
            }
        }

        return message;
    }

    private static byte FlipBit(byte value, int position)
    {
        return (byte)(value ^ (1 << (8 - position)));
    }
}