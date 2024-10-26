namespace GolayCodeSimulator.Core.Tests;

public class BinarySymmetricChannelTests
{
    [Fact]
    public void SimulateSending_GivenHighErrorProbability_FlipsAtLeastOneBit()
    {
        List<byte> message = [0b0010_0110];
        const double probability = 0.5;
        var messageFromChannel = BinarySymmetricChannel.SimulateSending(message, probability);

        Assert.Equal(message.Count, messageFromChannel.Count);
        Assert.NotEqual(message, messageFromChannel);
    }

    [Fact]
    public void SimulateSending_GivenZeroErrorProbability_DoesNotFlipAnyBit()
    {
        List<byte> message = [0b0010_0110];
        const double probability = 0;

        var messageFromChannel = BinarySymmetricChannel.SimulateSending(message, probability);

        Assert.Equal(message, messageFromChannel);
    }

    [Fact]
    public void SimulateSending_GivenCertainErrorProbability_FlipsAllBits()
    {
        List<byte> message = [0b0010_0110];
        List<byte> expectedMessageFromChannel = [0b1101_1001];
        const double probability = 1;

        var messageFromChannel = BinarySymmetricChannel.SimulateSending(message, probability);

        Assert.Equal(expectedMessageFromChannel, messageFromChannel);
    }
}
