using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using EVEMon.Common;
using EVEMon.Common.CustomEventArgs;
using EVEMon.Common.Models;
using EVEMon.Common.Service;

namespace EVEMon.Avalonia.Views.Dialogs;

/// <summary>
/// Dialog for adding a new EVE Online character via ESI SSO authentication.
/// Implements the full OAuth2 flow with local HTTP callback listener.
/// </summary>
public partial class AddCharacterWindow : Window
{
    // UI Elements
    private StackPanel? _loginPage;
    private StackPanel? _waitingPage;
    private StackPanel? _resultPage;
    private StackPanel? _errorPage;
    private TextBlock? _waitingText;
    private TextBlock? _characterNameText;
    private TextBlock? _characterIdText;
    private TextBlock? _tokenExpiresText;
    private TextBlock? _errorText;
    private Button? _loginButton;
    private Button? _importButton;

    // SSO Components
    private SSOWebServerHttpListener? _server;
    private SSOAuthenticationService? _authService;
    private string _state = string.Empty;

    // Auth results
    private AccessResponse? _accessResponse;
    private ESIKeyCreationEventArgs? _creationArgs;

    public AddCharacterWindow()
    {
        InitializeComponent();

        // Find UI elements
        _loginPage = this.FindControl<StackPanel>("LoginPage");
        _waitingPage = this.FindControl<StackPanel>("WaitingPage");
        _resultPage = this.FindControl<StackPanel>("ResultPage");
        _errorPage = this.FindControl<StackPanel>("ErrorPage");
        _waitingText = this.FindControl<TextBlock>("WaitingText");
        _characterNameText = this.FindControl<TextBlock>("CharacterNameText");
        _characterIdText = this.FindControl<TextBlock>("CharacterIdText");
        _tokenExpiresText = this.FindControl<TextBlock>("TokenExpiresText");
        _errorText = this.FindControl<TextBlock>("ErrorText");
        _loginButton = this.FindControl<Button>("LoginButton");
        _importButton = this.FindControl<Button>("ImportButton");

        // Initialize SSO components
        try
        {
            _server = new SSOWebServerHttpListener();
            _authService = SSOAuthenticationService.GetInstance();
            _state = DateTime.UtcNow.ToFileTime().ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize SSO components: {ex.Message}");
            ShowError($"Failed to initialize SSO: {ex.Message}");
        }
    }

    /// <summary>
    /// Shows the specified page and hides all others.
    /// </summary>
    private void ShowPage(string pageName)
    {
        if (_loginPage != null) _loginPage.IsVisible = pageName == "login";
        if (_waitingPage != null) _waitingPage.IsVisible = pageName == "waiting";
        if (_resultPage != null) _resultPage.IsVisible = pageName == "result";
        if (_errorPage != null) _errorPage.IsVisible = pageName == "error";
    }

    /// <summary>
    /// Shows the error page with the specified message.
    /// </summary>
    private void ShowError(string message)
    {
        if (_errorText != null)
            _errorText.Text = message;
        ShowPage("error");
    }

    /// <summary>
    /// Handles the Login button click - starts the SSO flow.
    /// </summary>
    private void OnLoginClick(object? sender, RoutedEventArgs e)
    {
        if (_server == null || _authService == null)
        {
            ShowError("SSO components not initialized. Please restart the application.");
            return;
        }

        try
        {
            // Generate new state for XSRF protection
            _state = DateTime.UtcNow.ToFileTime().ToString();

            // Show waiting page
            ShowPage("waiting");
            if (_waitingText != null)
                _waitingText.Text = "Starting authentication server...";

            // Start the HTTP listener
            _server.Start();

            if (_waitingText != null)
                _waitingText.Text = "Opening browser for EVE Online login...";

            // Open browser to EVE SSO
            _authService.SpawnBrowserForLogin(_state, SSOWebServerHttpListener.PORT);

            if (_waitingText != null)
                _waitingText.Text = "Waiting for EVE Online login...";

            // Wait for the callback with auth code
            _server.BeginWaitForCode(_state, OnAuthCodeReceived);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting SSO flow: {ex.Message}");
            StopServer();
            ShowError($"Failed to start authentication: {ex.Message}");
        }
    }

    /// <summary>
    /// Called when the auth code is received from the HTTP callback.
    /// </summary>
    private void OnAuthCodeReceived(Task<string> task)
    {
        // This is called on the UI thread via Dispatcher.Invoke in SSOWebServerHttpListener
        try
        {
            StopServer();

            if (task.IsFaulted)
            {
                ShowError($"Authentication failed: {task.Exception?.InnerException?.Message ?? "Unknown error"}");
                return;
            }

            string authCode = task.Result;
            if (string.IsNullOrEmpty(authCode))
            {
                ShowError("No authorization code received. Please try again.");
                return;
            }

            if (_waitingText != null)
                _waitingText.Text = "Exchanging authorization code for tokens...";

            // Exchange auth code for access/refresh tokens
            _authService?.VerifyAuthCode(authCode, OnTokensReceived);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing auth code: {ex.Message}");
            ShowError($"Error processing authentication: {ex.Message}");
        }
    }

    /// <summary>
    /// Called when the access/refresh tokens are received.
    /// </summary>
    private void OnTokensReceived(AccessResponse? response)
    {
        // This is called on the UI thread via Dispatcher.Invoke in SSOAuthenticationService
        try
        {
            if (response == null || string.IsNullOrEmpty(response.AccessToken) || string.IsNullOrEmpty(response.RefreshToken))
            {
                ShowError("Failed to retrieve access tokens. Please try again.");
                return;
            }

            _accessResponse = response;

            if (_waitingText != null)
                _waitingText.Text = "Fetching character information...";

            // Generate a new ID for this ESI key
            long newId = EveMonClient.ESIKeys.Count > 0
                ? EveMonClient.ESIKeys.Max(k => k.ID) + 1
                : 1;

            // Fetch character info using the access token
            ESIKey.TryAddOrUpdateAsync(newId, response, OnCharacterInfoReceived);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing tokens: {ex.Message}");
            ShowError($"Error processing tokens: {ex.Message}");
        }
    }

    /// <summary>
    /// Called when the character information is received.
    /// </summary>
    private void OnCharacterInfoReceived(object? sender, ESIKeyCreationEventArgs e)
    {
        // This is called on the UI thread
        try
        {
            if (e.CCPError != null)
            {
                ShowError($"Failed to fetch character information: {e.CCPError.ErrorMessage ?? "Unknown error"}");
                return;
            }

            _creationArgs = e;

            // Update UI with character info
            if (_characterNameText != null)
                _characterNameText.Text = e.Identity?.CharacterName ?? "Unknown";

            if (_characterIdText != null)
                _characterIdText.Text = e.Identity?.CharacterID.ToString() ?? "Unknown";

            if (_tokenExpiresText != null && _accessResponse != null)
                _tokenExpiresText.Text = _accessResponse.ExpiryUTC.ToString("g") + " UTC";

            // Show the result page
            ShowPage("result");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error displaying character info: {ex.Message}");
            ShowError($"Error displaying character: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the Import button click - creates the ESI key and character.
    /// </summary>
    private void OnImportClick(object? sender, RoutedEventArgs e)
    {
        if (_creationArgs == null)
        {
            ShowError("No character data available. Please authenticate again.");
            return;
        }

        try
        {
            Console.WriteLine($"Import: Starting import for character ID {_creationArgs.ID}");
            Console.WriteLine($"Import: Identity is {(_creationArgs.Identity != null ? "set" : "NULL")}");
            Console.WriteLine($"Import: Identity name: {_creationArgs.Identity?.CharacterName ?? "NULL"}");
            Console.WriteLine($"Import: RefreshToken length: {_creationArgs.RefreshToken?.Length ?? -1}");

            // Check if Identity is null (happens when token verification failed)
            if (_creationArgs.Identity == null)
            {
                ShowError("Character identity not available. The authentication may have failed. Please try again.");
                return;
            }

            // Create or update the ESI key
            var esiKey = _creationArgs.CreateOrUpdate();

            Console.WriteLine($"Import: ESIKey created: {(esiKey != null ? "yes" : "no")}");

            if (esiKey != null)
            {
                Console.WriteLine($"Successfully imported character: {_creationArgs.Identity?.CharacterName}");

                // Close the dialog with success
                Close(true);
            }
            else
            {
                ShowError("Failed to create ESI key. Please try again.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error importing character: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            ShowError($"Error importing character: {ex.Message}");
        }
    }

    /// <summary>
    /// Handles the Cancel Auth button click - stops waiting for callback.
    /// </summary>
    private void OnCancelAuthClick(object? sender, RoutedEventArgs e)
    {
        StopServer();
        ShowPage("login");
    }

    /// <summary>
    /// Handles the Try Again button click - returns to login page.
    /// </summary>
    private void OnTryAgainClick(object? sender, RoutedEventArgs e)
    {
        // Reset state
        _accessResponse = null;
        _creationArgs = null;
        _state = DateTime.UtcNow.ToFileTime().ToString();

        // Recreate server if needed
        if (_server == null)
        {
            try
            {
                _server = new SSOWebServerHttpListener();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to recreate SSO server: {ex.Message}");
            }
        }

        ShowPage("login");
    }

    /// <summary>
    /// Handles the Close button click.
    /// </summary>
    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        StopServer();
        Close(false);
    }

    /// <summary>
    /// Stops the HTTP listener server.
    /// </summary>
    private void StopServer()
    {
        try
        {
            _server?.Stop();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error stopping server: {ex.Message}");
        }
    }

    /// <summary>
    /// Clean up when the window is closed.
    /// </summary>
    protected override void OnClosed(EventArgs e)
    {
        StopServer();
        _server?.Dispose();
        _server = null;

        base.OnClosed(e);
    }
}
