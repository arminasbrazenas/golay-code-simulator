namespace GolayCode.Core;

public interface IChannel
{
    IList<byte> SimulateNoise(IEnumerable<byte> message, double errorProbability);
}

public class BinarySymmetricChannel : IChannel
{
    private static readonly Random Random = new();
    
    public IList<byte> SimulateNoise(IEnumerable<byte> message, double errorProbability)
    {
        List<byte> messageFromChannel = [];

        foreach (var messageByte in message)
        {
            var byteFromChannel = messageByte;
            
            for (var j = 1; j <= 8; j++)
            {
                var randomProbability = Random.NextDouble();
                var isError = randomProbability < errorProbability;
                if (isError)
                {
                    byteFromChannel = FlipBitInByte(byteFromChannel, j);
                }
            }
            
            messageFromChannel.Add(byteFromChannel);
        }

        return messageFromChannel;
    }

    private static byte FlipBitInByte(byte value, int position)
    {
        return (byte)(value ^ (1 << (8 - position)));
    }
}