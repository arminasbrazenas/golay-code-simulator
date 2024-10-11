namespace GolayCode.Core.Tests;

public class GolayEncoderTests
{
    [Theory]
    [MemberData(nameof(EncodeTestCaseData))]
    public void Encode_GivenMessage_EncodesCorrectly(List<byte> inputMessage, List<byte> expectedEncodedMessage)
    {
        var encoder = new GolayEncoder();

        var actualEncodedMessage = encoder.Encode(inputMessage);

        Assert.Equal(expectedEncodedMessage, actualEncodedMessage);
    }

    public static IEnumerable<object[]> EncodeTestCaseData()
    {
        yield return 
        [
            new List<byte> { 0b0010_0101, 0b1111_0000 },
            new List<byte> { 0b0010_0101, 0b1111_1010, 0b1010_1000 }
        ];
        yield return
        [
            new List<byte> { 0b0011_1110, 0b1110_0000 },
            new List<byte> { 0b0011_1110, 0b1110_0100, 0b1001_0010 }
        ];
        yield return
        [
            new List<byte> {0b0000_1100, 0b0111_0000 },
            new List<byte> { 0b0000_1100, 0b0111_0110, 0b1000_0000 }
        ];
        yield return
        [
            new List<byte> { 0b0010_0100, 0b0000_0000 },
            new List<byte> { 0b0010_0100, 0b0000_1111, 0b1010_0000 }
        ];
        yield return 
        [
            new List<byte> { 0b0010_0101, 0b1111_0010, 0b0101_1111 },
            new List<byte> { 0b0010_0101, 0b1111_1010, 0b1010_1000, 0b0100_1011, 0b1111_0101, 0b0101_0000 }
        ];
        yield return 
        [
            new List<byte> { 0b0010_0101, 0b1111_0010, 0b0101_1111, 0b0011_1110, 0b1110_0000 },
            new List<byte> { 0b0010_0101, 0b1111_1010, 0b1010_1000, 0b0100_1011, 0b1111_0101, 0b0101_0000, 0b1111_1011, 0b1001_0010, 0b0100_1000 }
        ];
        yield return 
        [
            new List<byte> { 0b0010_0101, 0b1111_0010, 0b0101_1111, 0b0011_1110, 0b1110_0010, 0b0100_0000 },
            new List<byte> { 0b0010_0101, 0b1111_1010, 0b1010_1000, 0b0100_1011, 0b1111_0101, 0b0101_0000, 0b1111_1011, 0b1001_0010, 0b0100_1001, 0b0010_0000, 0b0111_1101, 0b0000_0000 }
        ];
    }
}