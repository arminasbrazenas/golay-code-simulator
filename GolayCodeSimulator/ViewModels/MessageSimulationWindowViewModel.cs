using System.Linq;
using System.Windows.Input;
using Avalonia.Data;
using GolayCodeSimulator.Core;
using GolayCodeSimulator.Utilities;
using ReactiveUI;

namespace GolayCodeSimulator.ViewModels;

public partial class MessageSimulationWindowViewModel : ViewModelBase
{
    private string? _rawBitFlipProbability;
    private double? _bitFlipProbability;
    private string? _rawMessage;
    private string? _message;
    private string? _encodedMessage;
    private string? _messageFromChannel;
    private string? _rawMessageFromChannel;

    private readonly GolayEncoder _golayEncoder;
    private readonly BinarySymmetricChannel _binarySymmetricChannel;
    
    public MessageSimulationWindowViewModel()
    {
        _golayEncoder = new GolayEncoder();
        _binarySymmetricChannel = new BinarySymmetricChannel();
        SendMessageCommand = ReactiveCommand.Create(HandleSendMessageCommand);
        DecodeMessageCommand = ReactiveCommand.Create(HandleDecodeMessageCommand);
    }
    
    public ICommand SendMessageCommand { get; }

    public ICommand DecodeMessageCommand { get; }

    public string RawBitFlipProbability
    {
        get => _rawBitFlipProbability ?? string.Empty;
        set
        {
            this.RaiseAndSetIfChanged(ref _rawBitFlipProbability, value);
            ValidateAndSetBitFlipProbability(value);
        }
    }

    public string RawMessage
    {
        get => _rawMessage ?? string.Empty;
        set
        {
            this.RaiseAndSetIfChanged(ref _rawMessage, value);
            ValidateAndSetMessage(value);
        }
    }

    public string EncodedMessage
    {
        get => _encodedMessage ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _encodedMessage, value);
    }

    public string RawMessageFromChannel
    {
        get => _rawMessageFromChannel ?? string.Empty;
        set
        {
            this.RaiseAndSetIfChanged(ref _rawMessageFromChannel, value);
            ValidateAndSetMessageFromChannel(value);
        }
    }

    private void HandleSendMessageCommand()
    {
        if (!_bitFlipProbability.HasValue || _message is null)
        {
            return;
        }
        
        var messageBytes = BinaryStringConverter.ToBytes(_message);
        var encodedMessageBytes = _golayEncoder.Encode(messageBytes);
        EncodedMessage = BinaryStringConverter.FromBytes(encodedMessageBytes);
        
        var messageFromChannelBytes = _binarySymmetricChannel.SimulateNoise(encodedMessageBytes, _bitFlipProbability.Value);
        var messageFromChannel = BinaryStringConverter.FromBytes(messageFromChannelBytes);
        RawMessageFromChannel = messageFromChannel;
    }

    private void HandleDecodeMessageCommand()
    {
    }

    private void ValidateAndSetBitFlipProbability(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _bitFlipProbability = null;
            throw new DataValidationException("Bit flip probability is required.");
        }
            
        if (!double.TryParse(value, out var probability))
        {
            _bitFlipProbability = null;
            throw new DataValidationException("Bit flip probability must be a number.");
        }

        if (probability < 0 || probability > 1)
        {
            _bitFlipProbability = null;
            throw new DataValidationException("Bit flip probability must be between 0 and 1.");
        }

        _bitFlipProbability = probability;
    }

    private void ValidateAndSetMessage(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _message = null;
            throw new DataValidationException("Message is required.");
        }
            
        if (value.Any(x => x != '0' && x != '1'))
        {
            _message = null;
            throw new DataValidationException("Message must be binary.");
        }
            
        if (value.Length % 12 != 0)
        {
            _message = null;
            throw new DataValidationException($"Message length must be a multiple of 12. Current length is {value.Length}");
        }

        _message = value;
    }

    private void ValidateAndSetMessageFromChannel(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _messageFromChannel = null;
            throw new DataValidationException("Message from channel is required.");
        }
            
        if (value.Any(x => x != '0' && x != '1'))
        {
            _messageFromChannel = null;
            throw new DataValidationException("Message from channel must be binary.");
        }
            
        if (value.Length % 23 != 0)
        {
            _messageFromChannel = null;
            throw new DataValidationException($"Message from channel length must be a multiple of 23. Current length is {value.Length}");
        }

        _messageFromChannel = value;
    }
}