using System.Linq;
using System.Text;
using System.Windows.Input;
using GolayCodeSimulator.Core;
using GolayCodeSimulator.Helpers;
using GolayCodeSimulator.Validators;
using ReactiveUI;

namespace GolayCodeSimulator.ViewModels;

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
        
        SendTextCommand = ReactiveCommand.Create(HandleSendMessageCommand, canSendText);
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
    
    private void HandleSendMessageCommand()
    {
        var bitFlipProbability = BitFlipProbability.ParseDoubleCultureInvariant();
        var messageBytes = Encoding.UTF8.GetBytes(Text).ToList();
        
        var messageFromChannelWithoutEncodingBytes = BinarySymmetricChannel.SimulateNoise(messageBytes, bitFlipProbability);
        ReceivedTextWithoutEncoding = Encoding.UTF8.GetString(messageFromChannelWithoutEncodingBytes.ToArray());

        var isZeroPaddingNeeded = (messageBytes.Count * 8) % Constants.InformationLength != 0;
        if (isZeroPaddingNeeded)
        {
            messageBytes.Add(0);
        }
        
        var encodedMessageBytes = GolayEncoder.Encode(messageBytes);
        var messageFromChannel = BinarySymmetricChannel.SimulateNoise(encodedMessageBytes, bitFlipProbability);
        var decodedMessageBytes = GolayDecoder.Decode(messageFromChannel);
        var informationBytes = GolayInformationParser.ParseDecodedMessage(decodedMessageBytes);

        if (messageBytes.Last() == 0)
        {
            informationBytes.RemoveAt(informationBytes.Count - 1);
        }
        
        ReceivedTextWithEncoding = Encoding.UTF8.GetString(informationBytes.ToArray());
    }
}