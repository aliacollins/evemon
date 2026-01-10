using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using EVEMon.Avalonia.Views.Dialogs;

namespace EVEMon.Avalonia.Views;

/// <summary>
/// Main window for EVEMon Avalonia.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void OnAddCharacterClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var dialog = new AddCharacterWindow();
            await dialog.ShowDialog(this);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error opening Add Character dialog: {ex.Message}");
        }
    }

    private void OnExitClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
