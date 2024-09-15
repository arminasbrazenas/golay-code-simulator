namespace GolayCode.Core.Channel;

public interface IChannel
{
    byte[] SimulateNoise(byte[] message, double errorProbability);
}