using System.Collections.Generic;
using GolayCodeSimulator.Core;
using ReactiveUI;

namespace GolayCodeSimulator.Presentation.ViewModels;

public abstract class ViewModelBase : ReactiveObject
{
    protected static List<byte> SendThroughChannelWithZeroPaddingIfNeeded(
        List<byte> bytes,
        double bitFlipProbability,
        int seed)
    {
        var isZeroPaddingNeeded = (bytes.Count * 8) % Constants.InformationLength != 0;
        if (isZeroPaddingNeeded)
        {
            bytes.Add(0);
        }
        
        var encodedBytes = GolayEncoder.Encode(bytes);
        var bytesFromChannel = BinarySymmetricChannel.SimulateNoise(encodedBytes, bitFlipProbability, seed);
        var decodedBytes = GolayDecoder.Decode(bytesFromChannel);
        var informationBytes = GolayInformationParser.ParseDecodedMessage(decodedBytes);

        if (isZeroPaddingNeeded)
        {
            informationBytes.RemoveAt(informationBytes.Count - 1);
            bytes.RemoveAt(bytes.Count - 1);
        }

        return informationBytes;
    }
}