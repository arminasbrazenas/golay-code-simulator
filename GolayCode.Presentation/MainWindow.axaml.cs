using Avalonia.Controls;
using GolayCode.Core.Channel;

namespace GolayCode.Presentation;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        IChannel channel = new BinarySymmetricChannel();

        byte[] message = [255, 255, 255];
        channel.SimulateNoise(message, 0.5);
    }
}