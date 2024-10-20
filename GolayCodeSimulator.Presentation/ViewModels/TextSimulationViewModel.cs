using System;
using System.Linq;
using System.Text;
using System.Windows.Input;
using GolayCodeSimulator.Core;
using GolayCodeSimulator.Presentation.Helpers;
using GolayCodeSimulator.Presentation.Validation;
using ReactiveUI;

namespace GolayCodeSimulator.Presentation.ViewModels;

public class TextSimulationViewModel : ViewModelBase
{
    private string? _bitFlipProbability;
    private string? _text;
    private string? _receivedTextWithoutEncoding;
    private string? _receivedTextWithEncoding;
    
    public TextSimulationViewModel()
    {
        var canSendText = this.WhenAnyValue(
            vm => vm.BitFlipProbability,
            vm => vm.Text,
            (p, t) => BitFlipProbabilityValidator.Validate(p).IsValid && GolayTextValidator.Validate(t).IsValid);
        
        SendTextCommand = ReactiveCommand.Create(HandleSendTextCommand, canSendText);
    }
    
    public ICommand SendTextCommand { get; }
    
    public string BitFlipProbability
    {
        get => _bitFlipProbability ?? string.Empty;
        set
        {
            this.RaiseAndSetIfChanged(ref _bitFlipProbability, value);
            BitFlipProbabilityValidator.Validate(value).ThrowOnFailure();
        }
    }

    public string Text
    {
        get => _text ?? string.Empty;
        set
        {
            this.RaiseAndSetIfChanged(ref _text, value);
            GolayTextValidator.Validate(value).ThrowOnFailure();
        }
    }

    public string ReceivedTextWithoutEncoding
    {
        get => _receivedTextWithoutEncoding ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _receivedTextWithoutEncoding, value);
    }

    public string ReceivedTextWithEncoding
    {
        get => _receivedTextWithEncoding ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _receivedTextWithEncoding, value);
    }
    
    private void HandleSendTextCommand()
    {
        var messageBytes = Encoding.UTF8.GetBytes(Text).ToList();
        var bitFlipProbability = BitFlipProbability.ParseDoubleCultureInvariant();
        var seed = Guid.NewGuid().GetHashCode();
        
        var receivedBytesWithoutEncoding = BinarySymmetricChannel.SimulateNoise(messageBytes, bitFlipProbability, seed);
        ReceivedTextWithoutEncoding = Encoding.UTF8.GetString(receivedBytesWithoutEncoding.ToArray());
        
        var receivedBytesWithEncoding = SendThroughChannelWithZeroPaddingIfNeeded(messageBytes, bitFlipProbability, seed);
        ReceivedTextWithEncoding = Encoding.UTF8.GetString(receivedBytesWithEncoding.ToArray());
    }
}