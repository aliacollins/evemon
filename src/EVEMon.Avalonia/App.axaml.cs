using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EVEMon.Avalonia.Services;
using EVEMon.Avalonia.Views;
using EVEMon.Common;
using EVEMon.Common.Abstractions;
using EVEMon.Common.Abstractions.Services;
using EVEMon.Common.Services;
using EVEMon.Common.ViewModels;

namespace EVEMon.Avalonia;

/// <summary>
/// EVEMon Avalonia application.
/// </summary>
public partial class App : Application
{
    private MainWindowViewModel? _mainViewModel;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override async void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Initialize EVEMon core services
            await InitializeEVEMonServicesAsync();

            // Create the main ViewModel
            _mainViewModel = new MainWindowViewModel();

            // Create and show main window
            var mainWindow = new MainWindow
            {
                DataContext = _mainViewModel
            };
            desktop.MainWindow = mainWindow;
            mainWindow.Show();
            mainWindow.Activate();

            // Handle shutdown
            desktop.ShutdownRequested += OnShutdownRequested;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async Task InitializeEVEMonServicesAsync()
    {
        // Initialize file system paths first (sets EVEMonDataDir, creates cache directories)
        EveMonClient.InitializeFileSystemPaths();

        // Initialize EveMonClient (loads settings, static data, etc.)
        EveMonClient.Initialize();

        // Start the dispatcher and timer (required for SecondTick events)
        EveMonClient.Run(Thread.CurrentThread);

        // Load static datafiles (skills, items, certificates, masteries, etc.)
        await EVEMon.Common.Collections.Global.GlobalDatafileCollection.LoadAsync();

        // Register Avalonia-specific UI services
        RegisterUIServices();

        // Load settings (which may load existing characters)
        Settings.Initialize();

        // Import character data from settings into EveMonClient collections
        await Settings.ImportDataAsync();
    }

    private void RegisterUIServices()
    {
        // Register Avalonia-specific services
        if (ServiceLocator.IsInitialized)
        {
            var dialogService = new AvaloniaDialogService();
            var navigationService = new AvaloniaNavigationService();

            ServiceBootstrapper.RegisterUIServices(dialogService, navigationService);
        }
    }

    private void OnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        // Dispose the main ViewModel
        _mainViewModel?.Dispose();

        // Shutdown EVEMon services
        try
        {
            EveMonClient.Shutdown();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine($"EVEMon.Avalonia: Error during shutdown: {ex.Message}");
        }
    }
}
