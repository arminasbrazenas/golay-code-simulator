using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace GolayCodeSimulator.Views;

public partial class ImageSimulationView : UserControl
{
    public ImageSimulationView()
    {
        InitializeComponent();
    }

    private async void OpenImageSelectDialog(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this)!;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select an image",
            AllowMultiple = false,
            FileTypeFilter = [FilePickerFileTypes.ImageAll]
        });

        var a = 5;
    }
}