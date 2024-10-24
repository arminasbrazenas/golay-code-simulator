using System;
using System.Collections.Generic;
using System.Windows.Input;
using ReactiveUI;

namespace GolayCodeSimulator.Presentation.ViewModels;

public class MainViewModel : ViewModelBase
{
    private static readonly Dictionary<string, ViewModelBase> CachedViewModels = new();

    private ViewModelBase _currentView;

    public MainViewModel()
    {
        _currentView = GetViewModelByName("MessageSimulation");
        UpdateViewCommand = ReactiveCommand.Create<string>(HandleUpdateViewCommand);
    }

    public ICommand UpdateViewCommand { get; }

    public ViewModelBase CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    private void HandleUpdateViewCommand(string viewName)
    {
        CurrentView = GetViewModelByName(viewName);
    }

    private static ViewModelBase GetViewModelByName(string viewName)
    {
        if (CachedViewModels.TryGetValue(viewName, out var viewModel))
        {
            return viewModel;
        }

        viewModel = CreateViewModelByName(viewName);
        CachedViewModels[viewName] = viewModel;

        return viewModel;
    }

    private static ViewModelBase CreateViewModelByName(string viewName) =>
        viewName switch
        {
            "MessageSimulation" => new MessageSimulationViewModel(),
            "TextSimulation" => new TextSimulationViewModel(),
            "ImageSimulation" => new ImageSimulationViewModel(),
            _ => throw new ArgumentOutOfRangeException(nameof(viewName), viewName, null),
        };
}
