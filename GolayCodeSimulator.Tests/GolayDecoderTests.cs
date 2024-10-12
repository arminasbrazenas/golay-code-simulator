using GolayCodeSimulator.Core;

namespace GolayCodeSimulator.Tests;

public class GolayDecoderTests
{
    [Theory]
    [MemberData(nameof(DecodeTestCaseData))]
    public void Decode_GivenEncodedMessage_DecodesCorrectly(List<byte> encodedMessage, List<byte> expectedDecodedMessage)
    {
        var decoder = new GolayDecoder();

        var actualDecodedMessage = decoder.Decode(encodedMessage);

        Assert.Equal(expectedDecodedMessage, actualDecodedMessage);
    }
    
    public static IEnumerable<object[]> DecodeTestCaseData()
    {
        yield return 
        [
            new List<byte> { 0b0010_0101, 0b1111_1010, 0b1010_1000 },
            new List<byte> { 0b0010_0101, 0b1111_0000 }
        ];
    }
}