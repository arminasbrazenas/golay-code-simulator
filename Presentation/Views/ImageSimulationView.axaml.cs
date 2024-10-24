using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using GolayCodeSimulator.Presentation.ViewModels;

namespace GolayCodeSimulator.Presentation.Views;

public partial class ImageSimulationView : UserControl
{
    public ImageSimulationView()
    {
        InitializeComponent();
    }

    private async void OpenImageSelectDialog(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this)!;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = "Select a BMP image",
                AllowMultiple = false,
                FileTypeFilter = [new FilePickerFileType("BMP image") { Patterns = ["*.bmp"] }],
            }
        );

        if (files.Count != 1 || !files[0].Name.EndsWith(".bmp"))
        {
            return;
        }

        var dataContext = (ImageSimulationViewModel)DataContext!;
        await dataContext.LoadBmpImageFromFile(files[0]);
    }
}
