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
        
        SendMessageCommand = ReactiveCommand.Create(HandleSendMessageCommand, canSendMessage);
        DecodeMessageCommand = ReactiveCommand.Create(HandleDecodeMessageCommand, canDecodeMessage);
        ErrorPositionsMessage = this.WhenAnyValue(
            vm => vm.EncodedMessage,
            vm => vm.MessageFromChannel,
            CreateErrorPositionsMessage);
    }
    
    public ICommand SendMessageCommand { get; }

    public ICommand DecodeMessageCommand { get; }
    
    public IObservable<string?> ErrorPositionsMessage { get; }

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
    
    private void HandleSendMessageCommand()
    {
        var bitFlipProbability = BitFlipProbability.ParseDoubleCultureInvariant();
        var messageBytes = BinaryStringConverter.ToBytes(Message);
        
        var encodedBytes = GolayEncoder.Encode(messageBytes);
        EncodedMessage = BinaryStringConverter.FromBytes(encodedBytes, Constants.CodewordLength);
        
        var bytesFromChannel = BinarySymmetricChannel.SimulateNoise(encodedBytes, bitFlipProbability);
        MessageFromChannel = BinaryStringConverter.FromBytes(bytesFromChannel, Constants.CodewordLength);
    }

    private void HandleDecodeMessageCommand()
    {
        var bytesFromChannel = BinaryStringConverter.ToBytes(MessageFromChannel);
        
        var decodedBytes = GolayDecoder.Decode(bytesFromChannel);
        DecodedMessage = BinaryStringConverter.FromBytes(decodedBytes, Constants.CodewordLength);
        
        var informationBytes = GolayInformationParser.ParseDecodedMessage(decodedBytes);
        DecodedMessageInformation = BinaryStringConverter.FromBytes(informationBytes, Constants.InformationLength);
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