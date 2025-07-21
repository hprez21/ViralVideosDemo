using Microsoft.Maui.Controls;

namespace ViralVideosDemo.Pages;

public partial class AddVideoIdeaPage : ContentPage
{
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
    private void OnVideoIdeaTextChanged(object sender, TextChangedEventArgs e)
    {
        var characterCount = string.IsNullOrEmpty(e.NewTextValue) ? 0 : e.NewTextValue.Length;
        CharacterCountLabel.Text = $"{characterCount} characters";
    }

    /// <summary>
    /// Handles the Generate Viral Story button click event
    /// TODO: Implement the logic to generate viral story based on user input
    /// </summary>
    private async void OnGenerateViralStoryClicked(object sender, EventArgs e)
    {
        // Placeholder for future implementation
        // This is where the viral story generation logic will be added
        
        var userIdea = VideoIdeaEditor.Text?.Trim();
        
        if (string.IsNullOrEmpty(userIdea))
        {
            await DisplayAlert("Empty Idea", "Please enter your video idea before generating a viral story.", "OK");
            return;
        }
        
        // TODO: Add logic to process the user's idea and generate viral story
        await DisplayAlert("Coming Soon", "Viral story generation feature will be implemented soon!", "OK");
    }
}
