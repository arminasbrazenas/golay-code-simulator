using System;
using System.Buffers.Binary;
using System.Collections.Generic;
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
    private List<byte>? _originalImageMetadata;
    private List<byte>? _originalImageData;
    private Bitmap? _originalImage;
    private Bitmap? _receivedImageWithoutErrorCorrection;
    private Bitmap? _receivedImageWithErrorCorrection;

    public ImageSimulationViewModel()
    {
        var canSendImage = this.WhenAnyValue(
            vm => vm.BitFlipProbability,
            vm => vm.OriginalImage,
            (p, img) => BitFlipProbabilityValidator.Validate(p).IsValid && img is not null
        );

        SendImageCommand = ReactiveCommand.Create(SendImage, canSendImage);
    }

    public ICommand SendImageCommand { get; }

    public string BitFlipProbability
    {
        get => _bitFlipProbability ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _bitFlipProbability, value);
    }

    public Bitmap? ReceivedImageWithoutErrorCorrection
    {
        get => _receivedImageWithoutErrorCorrection;
        set => this.RaiseAndSetIfChanged(ref _receivedImageWithoutErrorCorrection, value);
    }

    public Bitmap? ReceivedImageWithErrorCorrection
    {
        get => _receivedImageWithErrorCorrection;
        set => this.RaiseAndSetIfChanged(ref _receivedImageWithErrorCorrection, value);
    }

    public Bitmap? OriginalImage
    {
        get => _originalImage;
        set => this.RaiseAndSetIfChanged(ref _originalImage, value);
    }

    /// <summary>
    /// Loads a BMP image from the given file.
    /// </summary>
    /// <param name="file">File to load the BMP image from.</param>
    public async Task LoadBmpImageFromFile(IStorageFile file)
    {
        var (metadata, data) = await ReadBmpImageFromFile(file);
        _originalImageMetadata = metadata;
        _originalImageData = data;

        OriginalImage = new Bitmap(new MemoryStream(metadata.Concat(data).ToArray()));
        ReceivedImageWithoutErrorCorrection = null;
        ReceivedImageWithErrorCorrection = null;
    }

    /// <summary>
    /// Sends an image through a binary symmetric channel with and without error correction.
    /// </summary>
    private void SendImage()
    {
        var bitFlipProbability = BitFlipProbability.ParseDoubleCultureInvariant();
        var seed = Guid.NewGuid().GetHashCode();

        var dataBytesWithoutErrorCorrection = BinarySymmetricChannel.Simulate(_originalImageData!, bitFlipProbability, seed);
        var imageBytesWithoutErrorCorrection = _originalImageMetadata!.Concat(dataBytesWithoutErrorCorrection).ToArray();
        ReceivedImageWithoutErrorCorrection = new Bitmap(new MemoryStream(imageBytesWithoutErrorCorrection));

        var dataBytesWithErrorCorrection = SimulationManager.SendThroughChannel(_originalImageData!, bitFlipProbability, seed);
        var imageBytesWithErrorCorrection = _originalImageMetadata!.Concat(dataBytesWithErrorCorrection).ToArray();
        ReceivedImageWithErrorCorrection = new Bitmap(new MemoryStream(imageBytesWithErrorCorrection));
    }

    /// <summary>
    /// Reads BMP image contents from the given file.
    /// </summary>
    /// <param name="file">File to read the BMP image from.</param>
    /// <returns>BMP image metadata and data.</returns>
    private static async Task<(List<byte> Metadata, List<byte> Data)> ReadBmpImageFromFile(IStorageFile file)
    {
        await using var stream = await file.OpenReadAsync();
        using var reader = new BinaryReader(stream);

        // Bytes 10-13 contain the offset at which the image data starts in little endian byte ordering.
        var metadata = reader.ReadBytes(14);
        var dataOffset = BinaryPrimitives.ReadInt32LittleEndian(metadata.Skip(10).ToArray());
        var restMetadata = reader.ReadBytes(dataOffset - 14);
        metadata = metadata.Concat(restMetadata).ToArray();

        var data = reader.ReadBytes((int)stream.Length - metadata.Length);

        return (metadata.ToList(), data.ToList());
    }
}
