using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace ViralVideosDemo.ViewModels;

public partial class AddVideoIdeaViewModel : ObservableObject
{
    [ObservableProperty]
    private string videoIdea = string.Empty;

    [ObservableProperty]
    private string characterCountText = "0 characters";

    [ObservableProperty]
    private bool isEnhancementEnabled = false;

    [ObservableProperty]
    private string generateButtonText = "Generate Viral Story";

    [ObservableProperty]
    private string enhancementToggleIcon = "🤖";

    [ObservableProperty]
    private double enhancementToggleOpacity = 0.5;

    [ObservableProperty]
    private double enhancementToggleTranslationX = 0.0;

    [ObservableProperty]
    private Color enhancementToggleBorderColor = Colors.Gray;

    [ObservableProperty]
    private bool isGenerating = false;

    // Reference to the page for displaying alerts
    private Page? _page;

    public void SetPage(Page page)
    {
        _page = page;
    }

    /// <summary>
    /// Updates character count when video idea text changes
    /// </summary>
    partial void OnVideoIdeaChanged(string value)
    {
        var characterCount = string.IsNullOrEmpty(value) ? 0 : value.Length;
        CharacterCountText = $"{characterCount} characters";
    }

    /// <summary>
    /// Updates UI elements when enhancement toggle state changes
    /// </summary>
    partial void OnIsEnhancementEnabledChanged(bool value)
    {
        UpdateGenerateButtonText();
        UpdateToggleAppearance();
    }

    /// <summary>
    /// Command to toggle AI enhancement feature
    /// </summary>
    [RelayCommand]
    private async Task ToggleEnhancement()
    {
        IsEnhancementEnabled = !IsEnhancementEnabled;
        await AnimateToggle();
    }

    /// <summary>
    /// Command to generate viral story from user's idea
    /// </summary>
    [RelayCommand]
    private async Task GenerateViralStory()
    {
        if (string.IsNullOrWhiteSpace(VideoIdea))
        {
            if (_page != null)
            {
                await _page.DisplayAlert("Empty Idea", "Please enter your video idea before generating a viral story.", "OK");
            }
            return;
        }

        IsGenerating = true;

        try
        {
            // Show different message based on enhancement setting
            var message = IsEnhancementEnabled 
                ? "AI will enhance and generate viral story from your idea!" 
                : "Viral story will be generated from your idea!";

            // TODO: Add logic to process the user's idea and generate viral story
            // TODO: Consider enhancement setting when calling AI service
            if (_page != null)
            {
                await _page.DisplayAlert("Coming Soon", message, "OK");
            }

            // TODO: Navigate to VideoPromptsPage with generated prompts
            // await Shell.Current.GoToAsync($"///{nameof(VideoPromptsPage)}");
        }
        finally
        {
            IsGenerating = false;
        }
    }

    /// <summary>
    /// Updates the generate button text based on enhancement state
    /// </summary>
    private void UpdateGenerateButtonText()
    {
        GenerateButtonText = IsEnhancementEnabled 
            ? "✨ Enhance & Generate" 
            : "Generate Viral Story";
    }

    /// <summary>
    /// Updates toggle visual appearance based on current state
    /// </summary>
    private void UpdateToggleAppearance()
    {
        if (IsEnhancementEnabled)
        {
            EnhancementToggleIcon = "✨";
            EnhancementToggleOpacity = 1.0;
            EnhancementToggleTranslationX = 20.0;
            EnhancementToggleBorderColor = Color.FromRgb(0x00, 0xFF, 0x33); // Green
        }
        else
        {
            EnhancementToggleIcon = "🤖";
            EnhancementToggleOpacity = 0.5;
            EnhancementToggleTranslationX = 0.0;
            EnhancementToggleBorderColor = Colors.Gray;
        }
    }

    /// <summary>
    /// Animates the enhancement toggle switch
    /// </summary>
    private async Task AnimateToggle()
    {
        var animation = new Animation();
        var targetTranslation = IsEnhancementEnabled ? 20.0 : 0.0;
        var startTranslation = IsEnhancementEnabled ? 0.0 : 20.0;

        // Translation animation
        animation.Add(0, 1, new Animation(v => 
        {
            EnhancementToggleTranslationX = v;
        }, startTranslation, targetTranslation));

        // Color animation
        animation.Add(0, 1, new Animation(v => 
        {
            if (IsEnhancementEnabled)
            {
                var color = Color.FromRgba(
                    (int)(0x66 + (0x00 - 0x66) * v), // Red component
                    (int)(0x66 + (0xFF - 0x66) * v), // Green component  
                    (int)(0x66 + (0x33 - 0x66) * v), // Blue component
                    255);
                EnhancementToggleBorderColor = color;
            }
            else
            {
                var grayValue = (int)(0x66 + (0xCC - 0x66) * v);
                EnhancementToggleBorderColor = Color.FromRgb(grayValue, grayValue, grayValue);
            }
        }, 0, 1));

        var completionSource = new TaskCompletionSource<bool>();

        if (_page != null)
        {
            animation.Commit(_page, "ToggleAnimation", 16, 250, 
                Easing.CubicOut, (v, c) => completionSource.SetResult(true));
        }
        else
        {
            completionSource.SetResult(true);
        }

        await completionSource.Task;
    }

    /// <summary>
    /// Validates if the current idea is ready for generation
    /// </summary>
    public bool CanGenerateStory => !string.IsNullOrWhiteSpace(VideoIdea) && !IsGenerating;
}
