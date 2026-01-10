using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EVEMon.Common.Abstractions.Services;
using EVEMon.Common.Models;

namespace EVEMon.Avalonia.Services;

/// <summary>
/// Avalonia implementation of the navigation service.
/// </summary>
public class AvaloniaNavigationService : INavigationService
{
    #region Character Navigation

    public void NavigateToCharacter(Character character)
    {
        // In the Avalonia app, character navigation is handled by selecting
        // the character in the main window's character list
        // The MainWindowViewModel handles this through SelectedCharacter property
        Trace.WriteLine($"NavigateToCharacter: {character?.Name}");
    }

    public void NavigateToSkillPlanner(Character character, Plan? plan = null)
    {
        // TODO: Implement skill planner window in Avalonia
        Trace.WriteLine($"NavigateToSkillPlanner: {character?.Name}, Plan: {plan?.Name}");
    }

    #endregion

    #region Window Navigation

    public void OpenSettings()
    {
        // TODO: Implement settings window in Avalonia
        Trace.WriteLine("OpenSettings");
    }

    public void OpenAbout()
    {
        // TODO: Implement about window in Avalonia
        Trace.WriteLine("OpenAbout");
    }

    public void OpenAddCharacter()
    {
        // TODO: Implement add character wizard in Avalonia
        Trace.WriteLine("OpenAddCharacter");
    }

    public void OpenPlanEditor(Plan plan)
    {
        // TODO: Implement plan editor window in Avalonia
        Trace.WriteLine($"OpenPlanEditor: {plan?.Name}");
    }

    public void OpenSkillBrowser(Character character)
    {
        // TODO: Implement skill browser in Avalonia
        Trace.WriteLine($"OpenSkillBrowser: {character?.Name}");
    }

    public void OpenShipBrowser(Character character)
    {
        // TODO: Implement ship browser in Avalonia
        Trace.WriteLine($"OpenShipBrowser: {character?.Name}");
    }

    public void OpenItemBrowser(Character character)
    {
        // TODO: Implement item browser in Avalonia
        Trace.WriteLine($"OpenItemBrowser: {character?.Name}");
    }

    public void OpenBlueprintBrowser(Character character)
    {
        // TODO: Implement blueprint browser in Avalonia
        Trace.WriteLine($"OpenBlueprintBrowser: {character?.Name}");
    }

    #endregion

    #region External Navigation

    public void OpenInBrowser(string url)
    {
        if (string.IsNullOrEmpty(url))
            return;

        OpenInBrowser(new Uri(url));
    }

    public void OpenInBrowser(Uri uri)
    {
        if (uri == null)
            return;

        try
        {
            // Cross-platform URL opening
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = uri.ToString(),
                    UseShellExecute = true
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", uri.ToString());
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", uri.ToString());
            }
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Failed to open URL: {uri} - {ex.Message}");
        }
    }

    #endregion

    #region Tab Navigation

    public void NavigateToTab(string tabName)
    {
        // Tab navigation is handled by the view's TabControl
        // The view model can expose a SelectedTab property if needed
        Trace.WriteLine($"NavigateToTab: {tabName}");
    }

    #endregion
}
