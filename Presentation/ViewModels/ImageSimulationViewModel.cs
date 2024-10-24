using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using GolayCodeSimulator.Core;
using GolayCodeSimulator.Presentation.Helpers;
using GolayCodeSimulator.Presentation.Validation;
using ReactiveUI;

namespace GolayCodeSimulator.Presentation.ViewModels;

public class ImageSimulationViewModel : ViewModelBase
{
    private string? _bitFlipProbability;
    private byte[]? _originalImageMetadata;
    private byte[]? _originalImageData;
    private Bitmap? _originalImage;
    private Bitmap? _receivedImageWithoutEncoding;
    private Bitmap? _receivedImageWithEncoding;
    
    public ImageSimulationViewModel()
    {
        var canSendImage = this.WhenAnyValue(
            vm => vm.BitFlipProbability,
            vm => vm.OriginalImage,
            (p, img) => BitFlipProbabilityValidator.Validate(p).IsValid && img is not null);

        SendImageCommand = ReactiveCommand.Create(HandleSendImageCommand, canSendImage);
    }
    
    public ICommand SendImageCommand { get; }
    
    public string BitFlipProbability
    {
        get => _bitFlipProbability ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _bitFlipProbability, value);
    }
    
    public Bitmap? ReceivedImageWithoutEncoding
    {
        get => _receivedImageWithoutEncoding;
        set => this.RaiseAndSetIfChanged(ref _receivedImageWithoutEncoding, value);
    }

    public Bitmap? ReceivedImageWithEncoding
    {
        get => _receivedImageWithEncoding;
        set => this.RaiseAndSetIfChanged(ref _receivedImageWithEncoding, value);
    }

    public Bitmap? OriginalImage
    {
        get => _originalImage;
        set => this.RaiseAndSetIfChanged(ref _originalImage, value);
    }

    public async Task LoadBmpImageFromFile(IStorageFile file)
    {
        var (metadata, data) = await ReadBmpImageFromFile(file);
        _originalImageMetadata = metadata;
        _originalImageData = data;
        
        OriginalImage = new Bitmap(new MemoryStream(metadata.Concat(data).ToArray()));
        ReceivedImageWithoutEncoding = null;
        ReceivedImageWithEncoding = null;
    }
    
    private void HandleSendImageCommand()
    {
        var bitFlipProbability = BitFlipProbability.ParseDoubleCultureInvariant();
        var seed = Guid.NewGuid().GetHashCode();
        
        var receivedBytesWithoutEncoding = BinarySymmetricChannel.SimulateNoise(_originalImageData!, bitFlipProbability, seed);
        ReceivedImageWithoutEncoding = new Bitmap(new MemoryStream(_originalImageMetadata!.Concat(receivedBytesWithoutEncoding).ToArray()));

        var receivedBytesWithEncoding = SendThroughChannelWithZeroPaddingIfNeeded(_originalImageData!.ToList(), bitFlipProbability, seed).ToArray();
        ReceivedImageWithEncoding = new Bitmap(new MemoryStream(_originalImageMetadata!.Concat(receivedBytesWithEncoding).ToArray()));
    }
    
    private static async Task<(byte[], byte[])> ReadBmpImageFromFile(IStorageFile file)
    {
        await using var stream = await file.OpenReadAsync();
        using var reader = new BinaryReader(stream);
        
        var metadata = reader.ReadBytes(14);
        var dataOffset = BinaryPrimitives.ReadInt32LittleEndian(metadata.Skip(10).ToArray());
        var restMetadata = reader.ReadBytes(dataOffset - 14);
        metadata = metadata.Concat(restMetadata).ToArray();
        
        var data = reader.ReadBytes((int)stream.Length - metadata.Length);
        
        return (metadata, data);
    }
}