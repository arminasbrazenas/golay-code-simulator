namespace GolayCode.Core.Encoder;

public interface IEncoder
{
    byte[] Encode(byte[] message);
}