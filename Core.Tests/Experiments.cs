using System.Diagnostics;
using Xunit.Abstractions;

namespace GolayCodeSimulator.Core.Tests;

public class Experiments
{
    private readonly ITestOutputHelper _testOutputHelper;

    public Experiments(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void CorrectDecodingProbabilityAsBitFlipProbabilityIncreases()
    {
        double bitFlipProbability = 0;
        const double delta = 0.005,
            trialsPerProbability = 1000;
        List<byte> message = [0b0011_1110, 0b1110_0000];

        while (bitFlipProbability < 1)
        {
            var correctDecodingCount = 0;

            for (var i = 0; i < trialsPerProbability; i++)
            {
                var encodedBytes = GolayEncoder.Encode(message);
                var bytesFromChannel = BinarySymmetricChannel.SimulateSending(encodedBytes, bitFlipProbability);
                var decodedBytes = GolayDecoder.Decode(bytesFromChannel);
                var receivedMessage = GolayInformationParser.ParseDecodedMessage(decodedBytes);

                if (message[0] == receivedMessage[0] && message[1] == receivedMessage[1])
                {
                    correctDecodingCount++;
                }
            }

            var correctDecodingChance = correctDecodingCount / trialsPerProbability * 100;
            _testOutputHelper.WriteLine(
                $"Bit flip probability: {bitFlipProbability:F3}; " + $"Chance of correct decoding: {correctDecodingChance:F3}%"
            );

            bitFlipProbability += delta;
        }
    }

    [Fact]
    public void SimulationTimeTakenAsMessageSizeIncreases()
    {
        List<byte> message = [];
        Random random = new();
        var nextBytes = new byte[300_000];
        const double bitFlipProbability = 0.1;

        for (var i = 0; i < 12; i++)
        {
            random.NextBytes(nextBytes);
            message.AddRange(nextBytes);

            var encodingStopwatch = Stopwatch.StartNew();
            var encodedBytes = GolayEncoder.Encode(message);
            encodingStopwatch.Stop();

            var channelStopwatch = Stopwatch.StartNew();
            var bytesFromChannel = BinarySymmetricChannel.SimulateSending(encodedBytes, bitFlipProbability);
            channelStopwatch.Stop();

            var decodingStopwatch = Stopwatch.StartNew();
            GolayDecoder.Decode(bytesFromChannel);
            decodingStopwatch.Stop();

            _testOutputHelper.WriteLine(
                $"Message size: {message.Count} bytes; "
                    + $"Encoding: {encodingStopwatch.ElapsedMilliseconds} ms; "
                    + $"Sending through channel: {channelStopwatch.ElapsedMilliseconds} ms; "
                    + $"Decoding: {channelStopwatch.ElapsedMilliseconds} ms"
            );
        }
    }
}
