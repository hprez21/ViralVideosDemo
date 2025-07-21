using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using ViralVideosDemo.Services;

namespace ViralVideosDemo.ViewModels;

public partial class AddVideoIdeaViewModel : ObservableObject
{
    private readonly IChatService _chatService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanGenerateStory))]
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
    [NotifyPropertyChangedFor(nameof(CanGenerateStory))]
    private bool isGenerating = false;

    // Video Configuration Properties
    [ObservableProperty]
    private string selectedResolution = "720";

    [ObservableProperty]
    private string selectedDuration = "10";

    [ObservableProperty]
    private string selectedOrientation = "Horizontal";

    // Available options for pickers
    public List<string> ResolutionOptions { get; } = ["480", "720", "1080"];
    public List<string> DurationOptions { get; } = ["5", "10", "15", "20"];
    public List<string> OrientationOptions { get; } = ["Horizontal", "Vertical", "Square"];

    // Reference to the page for displaying alerts
    private Page? _page;

    public AddVideoIdeaViewModel(IChatService chatService)
    {
        _chatService = chatService;
        CheckChatServiceConfiguration();
    }

    public void SetPage(Page page)
    {
        _page = page;
    }

    /// <summary>
    /// Checks if chat service is configured and updates UI accordingly
    /// </summary>
    private async void CheckChatServiceConfiguration()
    {
        try
        {
            var isConfigured = await _chatService.IsConfiguredAsync();
            if (!isConfigured)
            {
                // Update UI to indicate that enhancement requires configuration
                EnhancementToggleIcon = "⚙️";
            }
        }
        catch
        {
            // If checking fails, assume not configured
            EnhancementToggleIcon = "⚙️";
        }
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
            string finalVideoIdea = VideoIdea;

            // Check if enhancement is enabled and service is configured
            if (IsEnhancementEnabled)
            {
                var isConfigured = await _chatService.IsConfiguredAsync();
                if (!isConfigured)
                {
                    if (_page != null)
                    {
                        var result = await _page.DisplayAlert(
                            "Service Not Configured", 
                            "AI enhancement requires Azure LLM configuration. Would you like to go to Settings to configure it?", 
                            "Go to Settings", 
                            "Continue without Enhancement");
                        
                        if (result)
                        {
                            await Shell.Current.GoToAsync("//Settings");
                            return;
                        }
                        else
                        {
                            // Continue without enhancement
                            IsEnhancementEnabled = false;
                            await AnimateToggle();
                        }
                    }
                }
                else
                {
                    // Enhance the idea using Azure LLM
                    try
                    {
                        finalVideoIdea = await _chatService.EnhancePromptAsync(VideoIdea);
                        
                        if (_page != null)
                        {
                            await _page.DisplayAlert("Enhanced!", 
                                $"Your idea has been enhanced by AI!\n\nOriginal: {VideoIdea}\n\nEnhanced: {finalVideoIdea}", 
                                "Continue");
                        }
                    }
                    catch (Exception ex)
                    {
                        if (_page != null)
                        {
                            await _page.DisplayAlert("Enhancement Failed", 
                                $"Failed to enhance the idea: {ex.Message}\n\nContinuing with original idea.", 
                                "OK");
                        }
                    }
                }
            }

            // TODO: Store the final video idea and enhancement status for the VideoPromptsPage
            // Pass the data through navigation parameters including video configuration
            var (width, height) = GetVideoDimensions();
            var duration = int.Parse(SelectedDuration);
            
            var navigationParameters = new Dictionary<string, object>
            {
                ["VideoIdea"] = VideoIdea,
                ["EnhancedIdea"] = finalVideoIdea ?? string.Empty,
                ["WasEnhanced"] = (finalVideoIdea != VideoIdea && !string.IsNullOrEmpty(finalVideoIdea)).ToString(),
                ["VideoWidth"] = width.ToString(),
                ["VideoHeight"] = height.ToString(),
                ["VideoDuration"] = duration.ToString(),
                ["VideoOrientation"] = SelectedOrientation
            };

            // Navigate to VideoPromptsPage after processing
            await Shell.Current.GoToAsync("//VideoPrompts", navigationParameters);
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

    /// <summary>
    /// Gets video dimensions based on selected resolution and orientation
    /// Using SORA-supported resolution combinations
    /// </summary>
    /// <returns>Tuple with width and height values</returns>
    private (int width, int height) GetVideoDimensions()
    {
        // SORA supported resolutions: (480, 480), (854, 480), (720, 720), (1280, 720), (1080, 1080), (1920, 1080)
        return SelectedOrientation switch
        {
            "Horizontal" => SelectedResolution switch
            {
                "480" => (854, 480),    // Horizontal 480p
                "720" => (1280, 720),   // Horizontal 720p (HD)
                "1080" => (1920, 1080), // Horizontal 1080p (Full HD)
                _ => (1280, 720)        // Default to 720p horizontal
            },
            "Vertical" => SelectedResolution switch
            {
                "480" => (480, 854),    // Vertical 480p
                "720" => (720, 1280),   // Vertical 720p
                "1080" => (1080, 1920), // Vertical 1080p
                _ => (720, 1280)        // Default to 720p vertical
            },
            "Square" => SelectedResolution switch
            {
                "480" => (480, 480),    // Square 480p
                "720" => (720, 720),    // Square 720p
                "1080" => (1080, 1080), // Square 1080p
                _ => (720, 720)         // Default to 720p square
            },
            _ => (1280, 720)            // Default to horizontal 720p
        };
    }
}
