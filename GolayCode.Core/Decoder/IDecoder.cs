namespace GolayCode.Core.Decoder;

public interface IDecoder
{
    byte[] Decode(byte[] message);
}