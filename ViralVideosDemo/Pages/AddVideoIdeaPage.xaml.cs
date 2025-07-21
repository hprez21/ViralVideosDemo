using Microsoft.Maui.Controls;

namespace ViralVideosDemo.Pages;

public partial class AddVideoIdeaPage : ContentPage
{
    private bool _isEnhancementEnabled = false;

    public AddVideoIdeaPage()
    {
        InitializeComponent();
        
        // Subscribe to text changed event to update character count
        VideoIdeaEditor.TextChanged += OnVideoIdeaTextChanged;
        
        // Subscribe to button click event (placeholder for future implementation)
        GenerateViralStoryButton.Clicked += OnGenerateViralStoryClicked;
    }

    /// <summary>
    /// Handles text changes in the video idea editor to update character count
    /// </summary>
    private void OnVideoIdeaTextChanged(object? sender, TextChangedEventArgs e)
    {
        var characterCount = string.IsNullOrEmpty(e.NewTextValue) ? 0 : e.NewTextValue.Length;
        CharacterCountLabel.Text = $"{characterCount} characters";
    }

    /// <summary>
    /// Handles the AI Enhancement toggle tap
    /// </summary>
    private async void OnEnhancementToggleTapped(object? sender, EventArgs e)
    {
        _isEnhancementEnabled = !_isEnhancementEnabled;
        
        // Animate the toggle
        await AnimateToggle();
        
        // Update button text based on enhancement state
        UpdateGenerateButtonText();
    }

    /// <summary>
    /// Animates the enhancement toggle switch
    /// </summary>
    private Task AnimateToggle()
    {
        var completionSource = new TaskCompletionSource<bool>();
        var animation = new Animation();
        
        if (_isEnhancementEnabled)
        {
            // Enable animation - move circle to right and change colors
            animation.Add(0, 1, new Animation(v => EnhancementToggleCircle.TranslationX = v, 0, 20));
            animation.Add(0, 1, new Animation(v => 
            {
                var color = Color.FromRgba(
                    (int)(0x66 + (0x00 - 0x66) * v), // Red component
                    (int)(0x66 + (0xFF - 0x66) * v), // Green component  
                    (int)(0x66 + (0x33 - 0x66) * v), // Blue component
                    255);
                EnhancementToggleBorder.BackgroundColor = color;
            }));
            
            // Update circle content
            if (EnhancementToggleCircle.Content is Label circleLabel)
            {
                circleLabel.Text = "✨";
                circleLabel.Opacity = 1.0;
            }
        }
        else
        {
            // Disable animation - move circle to left and reset colors
            animation.Add(0, 1, new Animation(v => EnhancementToggleCircle.TranslationX = v, 20, 0));
            animation.Add(0, 1, new Animation(v => 
            {
                var grayValue = (int)(0x66 + (0xCC - 0x66) * v);
                var color = Color.FromRgb(grayValue, grayValue, grayValue);
                EnhancementToggleBorder.BackgroundColor = color;
            }));
            
            // Update circle content
            if (EnhancementToggleCircle.Content is Label circleLabel)
            {
                circleLabel.Text = "🤖";
                circleLabel.Opacity = 0.5;
            }
        }
        
        animation.Commit(this, "ToggleAnimation", 16, 250, Easing.CubicOut, 
            (v, c) => completionSource.SetResult(true));
        
        return completionSource.Task;
    }

    /// <summary>
    /// Updates the generate button text based on enhancement state
    /// </summary>
    private void UpdateGenerateButtonText()
    {
        if (_isEnhancementEnabled)
        {
            GenerateViralStoryButton.Text = "✨ Enhance & Generate";
        }
        else
        {
            GenerateViralStoryButton.Text = "Generate Viral Story";
        }
    }

    /// <summary>
    /// Handles the Generate Viral Story button click event
    /// TODO: Implement the logic to generate viral story based on user input
    /// </summary>
    private async void OnGenerateViralStoryClicked(object? sender, EventArgs e)
    {
        // Placeholder for future implementation
        // This is where the viral story generation logic will be added
        
        var userIdea = VideoIdeaEditor.Text?.Trim();
        
        if (string.IsNullOrEmpty(userIdea))
        {
            await DisplayAlert("Empty Idea", "Please enter your video idea before generating a viral story.", "OK");
            return;
        }
        
        // Show different message based on enhancement setting
        var message = _isEnhancementEnabled 
            ? "AI will enhance and generate viral story from your idea!" 
            : "Viral story will be generated from your idea!";
        
        // TODO: Add logic to process the user's idea and generate viral story
        // TODO: Consider enhancement setting when calling AI service
        await DisplayAlert("Coming Soon", message, "OK");
    }
}
