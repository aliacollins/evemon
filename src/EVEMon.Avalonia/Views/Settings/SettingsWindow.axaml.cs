using Avalonia.Controls;
using Avalonia.Interactivity;
using EVEMon.Common.ViewModels.Settings;

namespace EVEMon.Avalonia.Views.Settings;

/// <summary>
/// Settings window for configuring EVEMon options.
/// </summary>
public partial class SettingsWindow : Window
{
    private readonly SettingsViewModel _viewModel;

    public SettingsWindow()
    {
        InitializeComponent();
        _viewModel = new SettingsViewModel();
        DataContext = _viewModel;
    }

    private void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        _viewModel.SaveToSettings();
        Close(true);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }

    private void OnResetClick(object? sender, RoutedEventArgs e)
    {
        _viewModel.ResetToDefaults();
    }
}
