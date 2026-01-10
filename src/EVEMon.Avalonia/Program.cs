using System;
using Avalonia;

namespace EVEMon.Avalonia;

/// <summary>
/// Application entry point for EVEMon Avalonia.
/// </summary>
internal sealed class Program
{
    /// <summary>
    /// Initialization code. Don't use any Avalonia, third-party APIs or any
    /// SynchronizationContext-reliant code before AppMain is called.
    /// </summary>
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    /// <summary>
    /// Avalonia configuration. Don't remove; also used by visual designer.
    /// </summary>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
