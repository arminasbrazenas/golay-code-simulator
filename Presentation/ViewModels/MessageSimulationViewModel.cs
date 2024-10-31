using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using GolayCodeSimulator.Core;
using GolayCodeSimulator.Presentation.Helpers;
using GolayCodeSimulator.Presentation.Validation;
using ReactiveUI;

namespace GolayCodeSimulator.Presentation.ViewModels;

public class MessageSimulationViewModel : ViewModelBase
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
            (p, m) => BitFlipProbabilityValidator.Validate(p).IsValid && GolayMessageValidator.Validate(m).IsValid
        );

        var canDecodeMessage = this.WhenAnyValue(vm => vm.MessageFromChannel, m => GolayEncodedMessageValidator.Validate(m).IsValid);

        SendMessageCommand = ReactiveCommand.Create(SendMessage, canSendMessage);
        DecodeMessageCommand = ReactiveCommand.Create(DecodeMessage, canDecodeMessage);
        ErrorPositionsMessage = this.WhenAnyValue(vm => vm.EncodedMessage, vm => vm.MessageFromChannel, CreateErrorPositionsMessage);
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

    /// <summary>
    /// Encodes the message using Golay code and sends it through a binary symmetric channel.
    /// </summary>
    private void SendMessage()
    {
        var bitFlipProbability = BitFlipProbability.ParseDoubleCultureInvariant();
        var messageBytes = BinaryStringConverter.ToBytes(Message);

        var encodedBytes = GolayEncoder.Encode(messageBytes);
        EncodedMessage = BinaryStringConverter.FromBytes(encodedBytes, multipleOf: Constants.CodewordLength);

        var bytesFromChannel = BinarySymmetricChannel.Send(encodedBytes, bitFlipProbability);
        MessageFromChannel = BinaryStringConverter.FromBytes(bytesFromChannel, multipleOf: Constants.CodewordLength);
    }

    /// <summary>
    /// Decodes the message received from channel using Golay code.
    /// </summary>
    private void DecodeMessage()
    {
        var bytesFromChannel = BinaryStringConverter.ToBytes(MessageFromChannel);

        var decodedBytes = GolayDecoder.Decode(bytesFromChannel);
        DecodedMessage = BinaryStringConverter.FromBytes(decodedBytes, multipleOf: Constants.CodewordLength);

        var informationBytes = GolayInformationParser.ParseDecodedMessage(decodedBytes);
        DecodedMessageInformation = BinaryStringConverter.FromBytes(informationBytes, multipleOf: Constants.InformationLength);
    }

    /// <summary>
    /// Creates error position message by comparing encoded message and message received from channel.
    /// </summary>
    /// <param name="encodedMessage">Encoded message.</param>
    /// <param name="messageFromChannel">Message received from channel</param>
    /// <returns>Error position message if encoded message and message from channel are valid; otherwise, null.</returns>
    private static string? CreateErrorPositionsMessage(string encodedMessage, string messageFromChannel)
    {
        if (!GolayEncodedMessageValidator.Validate(messageFromChannel).IsValid || encodedMessage.Length != messageFromChannel.Length)
        {
            return null;
        }

        var errorPositions = CalculateErrorPositions(encodedMessage, messageFromChannel);
        return FormatMessageFromErrorPositions(errorPositions);
    }

    /// <summary>
    /// Calculates positions at which errors occurred while sending through channel.
    /// </summary>
    /// <param name="encodedMessage">Encoded message.</param>
    /// <param name="messageFromChannel">Message received from channel.</param>
    /// <returns>Positions at which errors occurred.</returns>
    private static List<int> CalculateErrorPositions(string encodedMessage, string messageFromChannel)
    {
        if (encodedMessage.Length != messageFromChannel.Length)
        {
            return [];
        }

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

    /// <summary>
    /// Formats a message to display to the user about the errors that occurred while sending the message through channel.
    /// </summary>
    /// <param name="errorPositions">Positions at which errors occurred.</param>
    /// <returns>Error position message.</returns>
    private static string FormatMessageFromErrorPositions(List<int> errorPositions) =>
        errorPositions.Count switch
        {
            0 => "No errors occurred while sending through channel.",
            1 => $"1 error occurred while sending through channel at position {errorPositions.First()}.",
            _ => $"{errorPositions.Count} errors occurred while sending through channel at positions {string.Join(", ", errorPositions)}.",
        };
}
