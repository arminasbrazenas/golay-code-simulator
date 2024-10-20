using System;
using System.Collections.Generic;
using GolayCodeSimulator.Helpers;

namespace GolayCodeSimulator.Core;

public class BinarySymmetricChannel
{
    private static readonly Random Random = new();
    
    public static List<byte> SimulateNoise(IEnumerable<byte> message, double bitFlipProbability)
    {
        List<byte> messageFromChannel = [];

        foreach (byte messageByte in message)
        {
            byte byteFromChannel = messageByte;
            
            for (int j = 0; j < 8; j++)
            {
                double randomProbability = Random.NextDouble();
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