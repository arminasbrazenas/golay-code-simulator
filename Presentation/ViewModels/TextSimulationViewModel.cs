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
    private string? _receivedTextWithoutErrorCorrection;
    private string? _receivedTextWithErrorCorrection;

    public TextSimulationViewModel()
    {
        var canSendText = this.WhenAnyValue(
            vm => vm.BitFlipProbability,
            vm => vm.Text,
            (p, t) => BitFlipProbabilityValidator.Validate(p).IsValid && GolayTextValidator.Validate(t).IsValid
        );

        SendTextCommand = ReactiveCommand.Create(SendText, canSendText);
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

    public string ReceivedTextWithoutErrorCorrection
    {
        get => _receivedTextWithoutErrorCorrection ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _receivedTextWithoutErrorCorrection, value);
    }

    public string ReceivedTextWithErrorCorrection
    {
        get => _receivedTextWithErrorCorrection ?? string.Empty;
        set => this.RaiseAndSetIfChanged(ref _receivedTextWithErrorCorrection, value);
    }

    /// <summary>
    /// Sends the text through a binary symmetric channel with and without error correction.
    /// </summary>
    private void SendText()
    {
        var messageBytes = Encoding.UTF8.GetBytes(Text).ToList();
        var bitFlipProbability = BitFlipProbability.ParseDoubleCultureInvariant();
        var seed = Guid.NewGuid().GetHashCode();

        var bytesWithoutErrorCorrection = BinarySymmetricChannel.SimulateSending(messageBytes, bitFlipProbability, seed);
        ReceivedTextWithoutErrorCorrection = Encoding.UTF8.GetString(bytesWithoutErrorCorrection.ToArray());

        var bytesWithErrorCorrection = SimulationManager.SendThroughChannel(messageBytes, bitFlipProbability, seed);
        ReceivedTextWithErrorCorrection = Encoding.UTF8.GetString(bytesWithErrorCorrection.ToArray());
    }
}
