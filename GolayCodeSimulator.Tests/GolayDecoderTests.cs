using GolayCodeSimulator.Core;

namespace GolayCodeSimulator.Tests;

public class GolayDecoderTests
{
    [Theory]
    [MemberData(nameof(DecodeTestCaseData))]
    public void Decode_GivenEncodedMessage_DecodesCorrectly(List<byte> encodedMessage, List<byte> expectedDecodedMessage)
    {
        var actualDecodedMessage = GolayDecoder.Decode(encodedMessage);
        Assert.Equal(expectedDecodedMessage, actualDecodedMessage);
    }
    
    public static IEnumerable<object[]> DecodeTestCaseData()
    {
        yield return 
        [
            new List<byte> { 0b0010_0100, 0b0000_1111, 0b1010_0000 },
            new List<byte> { 0b0010_0100, 0b0000_1111, 0b1010_0000 }
        ];
        yield return 
        [
            new List<byte> { 0b0010_0100, 0b1001_1111, 0b1110_0000 },
            new List<byte> { 0b0010_0100, 0b0000_1111, 0b1010_0000 }
        ];
        yield return
        [
            new List<byte> { 0b1001_1011, 0b0110_0110, 0b1010_0100 },
            new List<byte> { 0b1011_1011, 0b0110_0110, 0b1000_0100 }
        ];
    }
}