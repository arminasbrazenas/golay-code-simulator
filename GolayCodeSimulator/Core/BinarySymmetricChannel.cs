using System;
using System.Collections.Generic;

namespace GolayCodeSimulator.Core;

public class BinarySymmetricChannel
{
    private static readonly Random Random = new();
    
    public List<byte> SimulateNoise(IEnumerable<byte> message, double bitFlipProbability)
    {
        List<byte> messageFromChannel = [];

        foreach (var messageByte in message)
        {
            var byteFromChannel = messageByte;
            
            for (var j = 1; j <= 8; j++)
            {
                var randomProbability = Random.NextDouble();
                var isError = randomProbability < bitFlipProbability;
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