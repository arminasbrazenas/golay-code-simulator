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
    public void ProbabilityOfCorrectDecodingAsBitFlipProbabilityIncreases()
    {
        double bitFlipProbability = 0;
        const double delta = 0.005;
        const double trialsPerProbability = 5000;
        List<byte> message = [0b0011_1110, 0b1110_0000];

        while (bitFlipProbability < 1)
        {
            var correctDecodingCount = 0;

            for (var i = 0; i < trialsPerProbability; i++)
            {
                var encodedBytes = GolayEncoder.Encode(message);
                var bytesFromChannel = BinarySymmetricChannel.Send(encodedBytes, bitFlipProbability);
                var decodedBytes = GolayDecoder.Decode(bytesFromChannel);
                var receivedMessage = GolayInformationParser.ParseDecodedMessage(decodedBytes);

                if (message[0] == receivedMessage[0] && message[1] == receivedMessage[1])
                {
                    correctDecodingCount++;
                }
            }

            var correctDecodingProbability = correctDecodingCount / trialsPerProbability;
            _testOutputHelper.WriteLine(
                $"Bit flip probability: {bitFlipProbability:F3}; " + $"Probability of correct decoding: {correctDecodingProbability:F3}"
            );

            bitFlipProbability += delta;
        }
    }

    [Fact]
    public void SimulationTimeTakenAsMessageSizeIncreases()
    {
        List<byte> message = [];
        Random random = new();
        var nextBytes = new byte[30000];
        const double bitFlipProbability = 0.1;
        const double trialsPerMessageSize = 10;

        for (var i = 0; i < 12; i++)
        {
            random.NextBytes(nextBytes);
            message.AddRange(nextBytes);

            double encodingMilliseconds = 0;
            double decodingMilliseconds = 0;

            for (var j = 0; j < trialsPerMessageSize; j++)
            {
                var encodingStopwatch = Stopwatch.StartNew();
                var encodedBytes = GolayEncoder.Encode(message);
                encodingStopwatch.Stop();

                var bytesFromChannel = BinarySymmetricChannel.Send(encodedBytes, bitFlipProbability);

                var decodingStopwatch = Stopwatch.StartNew();
                GolayDecoder.Decode(bytesFromChannel);
                decodingStopwatch.Stop();

                encodingMilliseconds += encodingStopwatch.ElapsedMilliseconds;
                decodingMilliseconds += decodingStopwatch.ElapsedMilliseconds;
            }

            _testOutputHelper.WriteLine(
                $"Message size: {message.Count} bytes; "
                    + $"Encoding: {encodingMilliseconds / trialsPerMessageSize} ms; "
                    + $"Decoding: {decodingMilliseconds / trialsPerMessageSize} ms"
            );
        }
    }
}
