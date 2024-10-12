using GolayCodeSimulator.Core;

namespace GolayCodeSimulator.Tests;

public class BinarySymmetricChannelTests
{
    [Fact]
    public void SimulateNoise_GivenHighErrorProbability_FlipsAtLeastOneBit()
    {
        List<byte> message = [0b0010_0110];
        var probability = 0.5;
        var channel = new BinarySymmetricChannel();
        
        var messageFromChannel = channel.SimulateNoise(message, probability);
        
        Assert.Equal(message.Count, messageFromChannel.Count);
        Assert.NotEqual(message, messageFromChannel);
    }
    
    [Fact]
    public void SimulateNoise_GivenZeroErrorProbability_DoesNotFlipAnyBit()
    {
        List<byte> message = [0b0010_0110];
        var probability = 0;
        var channel = new BinarySymmetricChannel();
        
        var messageFromChannel = channel.SimulateNoise(message, probability);
        
        Assert.Equal(message, messageFromChannel);
    }
}