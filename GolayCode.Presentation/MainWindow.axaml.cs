using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using GolayCode.Core;

namespace GolayCode.Presentation;

public partial class MainWindow : Window
{
    private string? _message;
    
    public string? Message
    {
        get => _message;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(Message), "Message is required.");
            }

            if (value.Length % 12 != 0)
            {
                throw new ArgumentException("Message length must be a multiple of 12.", nameof(Message));
            }
        }
    }
    
    public MainWindow()
    {
        InitializeComponent();

        var encoder = new GolayEncoder();
        var encodedMessage = encoder.Encode(
            [0b0010_0101, 0b1111_0010, 0b0101_1111, 0b0011_1110, 0b1110_0010, 0b0100_0000, 0b0010_0101, 0b1111_0000]);

        var decoder = new GolayDecoder();
        var decodedMessage = decoder.Decode(encodedMessage);

        var a = 5;
    }

    public void OnSendMessageClicked(object sender, RoutedEventArgs args)
    {
        var messageTextBox = this.GetControl<TextBox>("MessageTextBox");
        var message = messageTextBox.Text?.Trim();
        
        if (string.IsNullOrWhiteSpace(message))
        {
            SetMessageTextBoxError("Message cannot be empty.");
        }
        else if (message.Any(x => x != '0' && x != '1'))
        {
            SetMessageTextBoxError("Message must be binary.");
        }
        else if (message.Length % 12 != 0)
        {
            SetMessageTextBoxError("Message length must be a multiple of 12.");
        }
        else
        {
            SetMessageTextBoxError(null);
        }
    }

    public void OnDecodeMessageClicked(object sender, RoutedEventArgs args)
    {
        
    }

    private void SetMessageTextBoxError(string? errorMessage)
    {
        var messageTextBoxError = this.GetControl<TextBlock>("MessageTextBoxError");
        messageTextBoxError.Text = errorMessage;
        messageTextBoxError.IsVisible = errorMessage is not null;
    }
}