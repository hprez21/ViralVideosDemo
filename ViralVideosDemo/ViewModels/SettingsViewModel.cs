using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace ViralVideosDemo.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    // Constants for Preferences keys
    private const string AZURE_LLM_ENDPOINT_KEY = "AzureLlmEndpoint";
    private const string AZURE_LLM_API_KEY = "AzureLlmApiKey";
    private const string AZURE_LLM_DEPLOYMENT_KEY = "AzureLlmDeployment";
    private const string SORA_ENDPOINT_KEY = "SoraEndpoint";
    private const string SORA_API_KEY = "SoraApiKey";
    private const string SORA_DEPLOYMENT_KEY = "SoraDeployment";

    [ObservableProperty]
    private string azureLlmEndpoint = string.Empty;

    [ObservableProperty]
    private string azureLlmApiKey = string.Empty;

    [ObservableProperty]
    private string azureLlmDeployment = string.Empty;

    [ObservableProperty]
    private string soraEndpoint = string.Empty;

    [ObservableProperty]
    private string soraApiKey = string.Empty;

    [ObservableProperty]
    private string soraDeployment = string.Empty;

    [ObservableProperty]
    private bool isAzureLlmApiKeyVisible = false;

    [ObservableProperty]
    private bool isSoraApiKeyVisible = false;

    [ObservableProperty]
    private string azureLlmApiKeyToggleIcon = "👁️";

    [ObservableProperty]
    private string soraApiKeyToggleIcon = "👁️";

    [ObservableProperty]
    private bool isSaving = false;

    // Reference to the page for displaying alerts
    private Page? _page;

    public SettingsViewModel()
    {
        LoadSettings();
    }

    public void SetPage(Page page)
    {
        _page = page;
    }

    /// <summary>
    /// Loads saved settings from Preferences
    /// </summary>
    public void LoadSettings()
    {
        AzureLlmEndpoint = Preferences.Default.Get(AZURE_LLM_ENDPOINT_KEY, string.Empty);
        AzureLlmApiKey = Preferences.Default.Get(AZURE_LLM_API_KEY, string.Empty);
        AzureLlmDeployment = Preferences.Default.Get(AZURE_LLM_DEPLOYMENT_KEY, string.Empty);
        
        SoraEndpoint = Preferences.Default.Get(SORA_ENDPOINT_KEY, string.Empty);
        SoraApiKey = Preferences.Default.Get(SORA_API_KEY, string.Empty);
        SoraDeployment = Preferences.Default.Get(SORA_DEPLOYMENT_KEY, string.Empty);
    }

    /// <summary>
    /// Command to toggle visibility of Azure LLM API key
    /// </summary>
    [RelayCommand]
    private void ToggleAzureLlmApiKeyVisibility()
    {
        IsAzureLlmApiKeyVisible = !IsAzureLlmApiKeyVisible;
        AzureLlmApiKeyToggleIcon = IsAzureLlmApiKeyVisible ? "🙈" : "👁️";
    }

    /// <summary>
    /// Command to toggle visibility of SORA API key
    /// </summary>
    [RelayCommand]
    private void ToggleSoraApiKeyVisibility()
    {
        IsSoraApiKeyVisible = !IsSoraApiKeyVisible;
        SoraApiKeyToggleIcon = IsSoraApiKeyVisible ? "🙈" : "👁️";
    }

    /// <summary>
    /// Command to save all settings
    /// </summary>
    [RelayCommand]
    private async Task SaveSettings()
    {
        IsSaving = true;

        try
        {
            // Validate settings before saving
            if (!ValidateSettings())
            {
                return;
            }

            // Save Azure LLM settings
            Preferences.Default.Set(AZURE_LLM_ENDPOINT_KEY, AzureLlmEndpoint.Trim());
            Preferences.Default.Set(AZURE_LLM_API_KEY, AzureLlmApiKey.Trim());
            Preferences.Default.Set(AZURE_LLM_DEPLOYMENT_KEY, AzureLlmDeployment.Trim());

            // Save SORA settings
            Preferences.Default.Set(SORA_ENDPOINT_KEY, SoraEndpoint.Trim());
            Preferences.Default.Set(SORA_API_KEY, SoraApiKey.Trim());
            Preferences.Default.Set(SORA_DEPLOYMENT_KEY, SoraDeployment.Trim());

            if (_page != null)
            {
                await _page.DisplayAlert("Settings Saved", "All settings have been saved successfully!", "OK");
            }
        }
        catch (Exception ex)
        {
            if (_page != null)
            {
                await _page.DisplayAlert("Error", $"Failed to save settings: {ex.Message}", "OK");
            }
        }
        finally
        {
            IsSaving = false;
        }
    }

    /// <summary>
    /// Command to clear all settings
    /// </summary>
    [RelayCommand]
    private async Task ClearSettings()
    {
        if (_page != null)
        {
            bool confirmed = await _page.DisplayAlert(
                "Clear Settings",
                "Are you sure you want to clear all settings? This action cannot be undone.",
                "Clear",
                "Cancel");

            if (!confirmed)
                return;
        }

        try
        {
            // Clear from Preferences
            Preferences.Default.Remove(AZURE_LLM_ENDPOINT_KEY);
            Preferences.Default.Remove(AZURE_LLM_API_KEY);
            Preferences.Default.Remove(AZURE_LLM_DEPLOYMENT_KEY);
            Preferences.Default.Remove(SORA_ENDPOINT_KEY);
            Preferences.Default.Remove(SORA_API_KEY);
            Preferences.Default.Remove(SORA_DEPLOYMENT_KEY);

            // Clear from UI
            AzureLlmEndpoint = string.Empty;
            AzureLlmApiKey = string.Empty;
            AzureLlmDeployment = string.Empty;
            SoraEndpoint = string.Empty;
            SoraApiKey = string.Empty;
            SoraDeployment = string.Empty;

            if (_page != null)
            {
                await _page.DisplayAlert("Settings Cleared", "All settings have been cleared successfully!", "OK");
            }
        }
        catch (Exception ex)
        {
            if (_page != null)
            {
                await _page.DisplayAlert("Error", $"Failed to clear settings: {ex.Message}", "OK");
            }
        }
    }

    /// <summary>
    /// Command to test Azure LLM connection
    /// </summary>
    [RelayCommand]
    private async Task TestAzureLlmConnection()
    {
        if (string.IsNullOrWhiteSpace(AzureLlmEndpoint) || 
            string.IsNullOrWhiteSpace(AzureLlmApiKey) || 
            string.IsNullOrWhiteSpace(AzureLlmDeployment))
        {
            if (_page != null)
            {
                await _page.DisplayAlert("Incomplete Configuration", 
                    "Please fill in all Azure LLM fields before testing.", "OK");
            }
            return;
        }

        // TODO: Implement actual connection test
        if (_page != null)
        {
            await _page.DisplayAlert("Test Connection", 
                "Azure LLM connection test functionality will be implemented soon!", "OK");
        }
    }

    /// <summary>
    /// Command to test SORA connection
    /// </summary>
    [RelayCommand]
    private async Task TestSoraConnection()
    {
        if (string.IsNullOrWhiteSpace(SoraEndpoint) || 
            string.IsNullOrWhiteSpace(SoraApiKey) || 
            string.IsNullOrWhiteSpace(SoraDeployment))
        {
            if (_page != null)
            {
                await _page.DisplayAlert("Incomplete Configuration", 
                    "Please fill in all SORA fields before testing.", "OK");
            }
            return;
        }

        // TODO: Implement actual connection test
        if (_page != null)
        {
            await _page.DisplayAlert("Test Connection", 
                "SORA connection test functionality will be implemented soon!", "OK");
        }
    }

    /// <summary>
    /// Validates all settings before saving
    /// </summary>
    private bool ValidateSettings()
    {
        var errors = new List<string>();

        // Validate Azure LLM settings
        if (string.IsNullOrWhiteSpace(AzureLlmEndpoint))
            errors.Add("Azure LLM Endpoint is required");
        else if (!Uri.TryCreate(AzureLlmEndpoint, UriKind.Absolute, out _))
            errors.Add("Azure LLM Endpoint must be a valid URL");

        if (string.IsNullOrWhiteSpace(AzureLlmApiKey))
            errors.Add("Azure LLM API Key is required");

        if (string.IsNullOrWhiteSpace(AzureLlmDeployment))
            errors.Add("Azure LLM Deployment name is required");

        // Validate SORA settings
        if (string.IsNullOrWhiteSpace(SoraEndpoint))
            errors.Add("SORA Endpoint is required");
        else if (!Uri.TryCreate(SoraEndpoint, UriKind.Absolute, out _))
            errors.Add("SORA Endpoint must be a valid URL");

        if (string.IsNullOrWhiteSpace(SoraApiKey))
            errors.Add("SORA API Key is required");

        if (string.IsNullOrWhiteSpace(SoraDeployment))
            errors.Add("SORA Deployment name is required");

        if (errors.Count > 0 && _page != null)
        {
            Task.Run(async () => await _page.DisplayAlert("Validation Error", 
                string.Join("\n", errors), "OK"));
            return false;
        }

        return true;
    }

    /// <summary>
    /// Checks if all required settings are configured
    /// </summary>
    public bool AreSettingsComplete =>
        !string.IsNullOrWhiteSpace(AzureLlmEndpoint) &&
        !string.IsNullOrWhiteSpace(AzureLlmApiKey) &&
        !string.IsNullOrWhiteSpace(AzureLlmDeployment) &&
        !string.IsNullOrWhiteSpace(SoraEndpoint) &&
        !string.IsNullOrWhiteSpace(SoraApiKey) &&
        !string.IsNullOrWhiteSpace(SoraDeployment);

    /// <summary>
    /// Checks if Azure LLM settings are complete
    /// </summary>
    public bool IsAzureLlmConfigComplete =>
        !string.IsNullOrWhiteSpace(AzureLlmEndpoint) &&
        !string.IsNullOrWhiteSpace(AzureLlmApiKey) &&
        !string.IsNullOrWhiteSpace(AzureLlmDeployment);

    /// <summary>
    /// Checks if SORA settings are complete
    /// </summary>
    public bool IsSoraConfigComplete =>
        !string.IsNullOrWhiteSpace(SoraEndpoint) &&
        !string.IsNullOrWhiteSpace(SoraApiKey) &&
        !string.IsNullOrWhiteSpace(SoraDeployment);
}
