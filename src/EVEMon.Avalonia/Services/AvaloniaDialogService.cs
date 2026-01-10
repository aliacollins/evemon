using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using EVEMon.Common.Abstractions.Services;

namespace EVEMon.Avalonia.Services;

/// <summary>
/// Avalonia implementation of the dialog service.
/// </summary>
public class AvaloniaDialogService : IDialogService
{
    #region Helper Methods

    private static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }
        return null;
    }

    private static TopLevel? GetTopLevel()
    {
        var window = GetMainWindow();
        return window != null ? TopLevel.GetTopLevel(window) : null;
    }

    #endregion

    #region Message Dialogs

    public void ShowInformation(string title, string message)
    {
        ShowMessageBoxAsync(title, message, "info").GetAwaiter().GetResult();
    }

    public void ShowWarning(string title, string message)
    {
        ShowMessageBoxAsync(title, message, "warning").GetAwaiter().GetResult();
    }

    public void ShowError(string title, string message)
    {
        ShowMessageBoxAsync(title, message, "error").GetAwaiter().GetResult();
    }

    public bool ShowConfirmation(string title, string message)
    {
        return ShowConfirmationAsync(title, message).GetAwaiter().GetResult();
    }

    public async Task<bool> ShowConfirmationAsync(string title, string message)
    {
        // For now, use a simple approach - in production, use a proper dialog library
        // or create a custom dialog window
        var window = GetMainWindow();
        if (window == null)
            return false;

        // Create a simple confirmation dialog
        var dialog = new Window
        {
            Title = title,
            Width = 400,
            Height = 180,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        bool result = false;

        var panel = new StackPanel
        {
            Margin = new Thickness(20),
            Spacing = 20
        };

        panel.Children.Add(new TextBlock
        {
            Text = message,
            TextWrapping = global::Avalonia.Media.TextWrapping.Wrap
        });

        var buttonPanel = new StackPanel
        {
            Orientation = global::Avalonia.Layout.Orientation.Horizontal,
            HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Right,
            Spacing = 10
        };

        var yesButton = new Button { Content = "Yes", Width = 80 };
        yesButton.Click += (s, e) => { result = true; dialog.Close(); };

        var noButton = new Button { Content = "No", Width = 80 };
        noButton.Click += (s, e) => { result = false; dialog.Close(); };

        buttonPanel.Children.Add(yesButton);
        buttonPanel.Children.Add(noButton);
        panel.Children.Add(buttonPanel);

        dialog.Content = panel;
        await dialog.ShowDialog(window);

        return result;
    }

    private static Task ShowMessageBoxAsync(string title, string message, string type)
    {
        var window = GetMainWindow();
        if (window == null)
            return Task.CompletedTask;

        var dialog = new Window
        {
            Title = title,
            Width = 400,
            Height = 160,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        var panel = new StackPanel
        {
            Margin = new Thickness(20),
            Spacing = 20
        };

        panel.Children.Add(new TextBlock
        {
            Text = message,
            TextWrapping = global::Avalonia.Media.TextWrapping.Wrap
        });

        var okButton = new Button
        {
            Content = "OK",
            Width = 80,
            HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Right
        };
        okButton.Click += (s, e) => dialog.Close();
        panel.Children.Add(okButton);

        dialog.Content = panel;
        return dialog.ShowDialog(window);
    }

    #endregion

    #region File Dialogs

    public string ShowOpenFileDialog(string title, string filter, string? initialDirectory = null)
    {
        return ShowOpenFileDialogAsync(title, filter, initialDirectory).GetAwaiter().GetResult();
    }

    public async Task<string> ShowOpenFileDialogAsync(string title, string filter, string? initialDirectory = null)
    {
        var topLevel = GetTopLevel();
        if (topLevel == null)
            return null!;

        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = ParseFilter(filter)
        };

        if (!string.IsNullOrEmpty(initialDirectory))
        {
            options.SuggestedStartLocation = await topLevel.StorageProvider.TryGetFolderFromPathAsync(initialDirectory);
        }

        var result = await topLevel.StorageProvider.OpenFilePickerAsync(options);
        return result.FirstOrDefault()?.Path.LocalPath!;
    }

    public string ShowSaveFileDialog(string title, string filter, string? defaultFileName = null, string? initialDirectory = null)
    {
        return ShowSaveFileDialogAsync(title, filter, defaultFileName, initialDirectory).GetAwaiter().GetResult();
    }

    public async Task<string> ShowSaveFileDialogAsync(string title, string filter, string? defaultFileName = null, string? initialDirectory = null)
    {
        var topLevel = GetTopLevel();
        if (topLevel == null)
            return null!;

        var options = new FilePickerSaveOptions
        {
            Title = title,
            SuggestedFileName = defaultFileName,
            FileTypeChoices = ParseFilter(filter)
        };

        if (!string.IsNullOrEmpty(initialDirectory))
        {
            options.SuggestedStartLocation = await topLevel.StorageProvider.TryGetFolderFromPathAsync(initialDirectory);
        }

        var result = await topLevel.StorageProvider.SaveFilePickerAsync(options);
        return result?.Path.LocalPath!;
    }

    public string ShowFolderBrowserDialog(string title, string? initialDirectory = null)
    {
        return ShowFolderBrowserDialogAsync(title, initialDirectory).GetAwaiter().GetResult();
    }

    public async Task<string> ShowFolderBrowserDialogAsync(string title, string? initialDirectory = null)
    {
        var topLevel = GetTopLevel();
        if (topLevel == null)
            return null!;

        var options = new FolderPickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        };

        if (!string.IsNullOrEmpty(initialDirectory))
        {
            options.SuggestedStartLocation = await topLevel.StorageProvider.TryGetFolderFromPathAsync(initialDirectory);
        }

        var result = await topLevel.StorageProvider.OpenFolderPickerAsync(options);
        return result.FirstOrDefault()?.Path.LocalPath!;
    }

    private static List<FilePickerFileType> ParseFilter(string filter)
    {
        var fileTypes = new List<FilePickerFileType>();

        if (string.IsNullOrEmpty(filter))
        {
            fileTypes.Add(FilePickerFileTypes.All);
            return fileTypes;
        }

        // Parse WinForms-style filter: "Description|*.ext|Description2|*.ext2"
        var parts = filter.Split('|');
        for (int i = 0; i < parts.Length - 1; i += 2)
        {
            var name = parts[i].Trim();
            var pattern = parts[i + 1].Trim();
            var patterns = pattern.Split(';').Select(p => p.Trim()).ToList();

            fileTypes.Add(new FilePickerFileType(name)
            {
                Patterns = patterns
            });
        }

        if (fileTypes.Count == 0)
        {
            fileTypes.Add(FilePickerFileTypes.All);
        }

        return fileTypes;
    }

    #endregion

    #region Input Dialogs

    public string ShowInputDialog(string title, string prompt, string? defaultValue = null)
    {
        return ShowInputDialogAsync(title, prompt, defaultValue).GetAwaiter().GetResult();
    }

    public async Task<string> ShowInputDialogAsync(string title, string prompt, string? defaultValue = null)
    {
        var window = GetMainWindow();
        if (window == null)
            return null!;

        var dialog = new Window
        {
            Title = title,
            Width = 400,
            Height = 180,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        string? result = null;

        var panel = new StackPanel
        {
            Margin = new Thickness(20),
            Spacing = 10
        };

        panel.Children.Add(new TextBlock { Text = prompt });

        var textBox = new TextBox { Text = defaultValue ?? string.Empty };
        panel.Children.Add(textBox);

        var buttonPanel = new StackPanel
        {
            Orientation = global::Avalonia.Layout.Orientation.Horizontal,
            HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Right,
            Spacing = 10,
            Margin = new Thickness(0, 10, 0, 0)
        };

        var okButton = new Button { Content = "OK", Width = 80 };
        okButton.Click += (s, e) => { result = textBox.Text; dialog.Close(); };

        var cancelButton = new Button { Content = "Cancel", Width = 80 };
        cancelButton.Click += (s, e) => { result = null; dialog.Close(); };

        buttonPanel.Children.Add(okButton);
        buttonPanel.Children.Add(cancelButton);
        panel.Children.Add(buttonPanel);

        dialog.Content = panel;
        await dialog.ShowDialog(window);

        return result!;
    }

    #endregion
}
