using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GolayCodeSimulator.Core;
using GolayCodeSimulator.Helpers;
using GolayCodeSimulator.Validators;
using ReactiveUI;

namespace GolayCodeSimulator.ViewModels;

public partial class MessageSimulationViewModel : ViewModelBase
{
    private string? _bitFlipProbability;
    private string? _message;
    private string? _encodedMessage;
    private string? _messageFromChannel;
    private string? _decodedMessage;
    private string? _decodedMessageInformation;

    public MessageSimulationViewModel()
    {
        var canSendMessage = this.WhenAnyValue(
            vm => vm.BitFlipProbability,
            vm => vm.Message,
            (p, m) => BitFlipProbabilityValidator.Validate(p).IsValid && GolayMessageValidator.Validate(m).IsValid);

        var canDecodeMessage = this.WhenAnyValue(
            vm => vm.MessageFromChannel,
            m => GolayEncodedMessageValidator.Validate(m).IsValid);

        ErrorPositionsMessage = this.WhenAnyValue(
            vm => vm.EncodedMessage,
            vm => vm.MessageFromChannel,
            CreateErrorPositionsMessage);
        
        SendMessageCommand = ReactiveCommand.Create(HandleSendMessageCommand, canSendMessage);
        DecodeMessageCommand = ReactiveCommand.Create(HandleDecodeMessageCommand, canDecodeMessage);
    }
    
    public ICommand SendMessageCommand { get; }

    public ICommand DecodeMessageCommand { get; }

    public string BitFlipProbability
    {
        get => _bitFlipProbability ?? string.Empty;
        set
        {
            this.RaiseAndSetIfChanged(ref _bitFlipProbability, value);
            BitFlipProbabilityValidator.Validate(value).ThrowOnFailure();
        }
    }
    
    public string Message
    {
        get => _message ?? string.Empty;
        set
        {
            this.RaiseAndSetIfChanged(ref _message, value);
            GolayMessageValidator.Validate(value).ThrowOnFailure();
        }
    }

    public string EncodedMessage
    {
        get => _encodedMessage ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _encodedMessage, value);
    }

    public string MessageFromChannel
    {
        get => _messageFromChannel ?? string.Empty;
        set
        {
            this.RaiseAndSetIfChanged(ref _messageFromChannel, value);
            GolayEncodedMessageValidator.Validate(value).ThrowOnFailure();
        }
    }

    public string DecodedMessage
    {
        get => _decodedMessage ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _decodedMessage, value);
    }

    public string DecodedMessageInformation
    {
        get => _decodedMessageInformation ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _decodedMessageInformation, value);
    }
    
    public IObservable<string?> ErrorPositionsMessage { get; }

    private void HandleSendMessageCommand()
    {
        var bitFlipProbability = BitFlipProbability.ParseDoubleCultureInvariant();
        var messageBytes = BinaryStringConverter.ToBytes(Message);
        
        var encodedMessageBytes = GolayEncoder.Encode(messageBytes);
        EncodedMessage = BinaryStringConverter.FromBytes(encodedMessageBytes, Constants.CodewordLength);
        
        var messageFromChannelBytes = BinarySymmetricChannel.SimulateNoise(encodedMessageBytes, bitFlipProbability);
        MessageFromChannel = BinaryStringConverter.FromBytes(messageFromChannelBytes, Constants.CodewordLength);
    }

    private void HandleDecodeMessageCommand()
    {
        var messageFromChannelBytes = BinaryStringConverter.ToBytes(MessageFromChannel);
        
        var decodedMessageBytes = GolayDecoder.Decode(messageFromChannelBytes);
        DecodedMessage = BinaryStringConverter.FromBytes(decodedMessageBytes, Constants.CodewordLength);
        
        var decodedInformation = GolayInformationParser.ParseDecodedMessage(decodedMessageBytes);
        DecodedMessageInformation = BinaryStringConverter.FromBytes(decodedInformation, Constants.InformationLength);
    }

    private static string? CreateErrorPositionsMessage(string encodedMessage, string messageFromChannel)
    {
        if (!GolayEncodedMessageValidator.Validate(messageFromChannel).IsValid || encodedMessage.Length != messageFromChannel.Length)
        {
            return null;
        }
                
        var errorPositions = GetErrorPositions(encodedMessage, messageFromChannel);
        return FormatMessageFromErrorPositions(errorPositions);
    }
    
    private static List<int> GetErrorPositions(string encodedMessage, string messageFromChannel)
    {
        List<int> errorPositions = [];
        for (var i = 0; i < encodedMessage.Length; i++)
        {
            if (encodedMessage[i] != messageFromChannel[i])
            {
                errorPositions.Add(i + 1);
            }
        }

        return errorPositions;
    }

    private static string FormatMessageFromErrorPositions(List<int> errorPositions) => errorPositions.Count switch
    {
        0 => "No errors occurred while sending through channel.",
        1 => $"1 error occurred while sending through channel at position {errorPositions.First()}.",
        _ => $"{errorPositions.Count} errors occurred while sending through channel at positions {string.Join(", ", errorPositions)}."
    };
}