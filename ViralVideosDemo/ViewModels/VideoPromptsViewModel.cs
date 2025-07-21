using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ViralVideosDemo.ViewModels;

public partial class VideoPromptsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<VideoPrompt> prompts = new();

    [ObservableProperty]
    private int totalPrompts = 0;

    [ObservableProperty]
    private int editedPrompts = 0;

    [ObservableProperty]
    private string videoIdea = string.Empty;

    public VideoPromptsViewModel()
    {
        LoadSampleData();
    }

    private void LoadSampleData()
    {
        VideoIdea = "Dancing cat compilation with trending music";
        
        Prompts.Add(new VideoPrompt
        {
            Id = 1,
            Title = "Opening Hook",
            Content = "Start with the most energetic cat dance move synchronized to the beat drop",
            IsEdited = false
        });

        Prompts.Add(new VideoPrompt
        {
            Id = 2,
            Title = "Main Content",
            Content = "Sequence of 5-7 different cats dancing to trending audio, each for 3-4 seconds",
            IsEdited = false
        });

        Prompts.Add(new VideoPrompt
        {
            Id = 3,
            Title = "Engagement Boost",
            Content = "Add text overlay: 'Which cat has the best moves? 👇 Comment below!'",
            IsEdited = false
        });

        Prompts.Add(new VideoPrompt
        {
            Id = 4,
            Title = "Call to Action",
            Content = "End with 'Follow for more dancing pets!' with heart and dancing emojis",
            IsEdited = false
        });

        UpdateStatistics();
    }

    [RelayCommand]
    private async Task EditPrompt(VideoPrompt prompt)
    {
        try
        {
            var page = GetCurrentPage();
            if (page == null) return;

            var result = await page.DisplayPromptAsync(
                "Edit Prompt", 
                $"Edit the {prompt.Title}:", 
                "Save", 
                "Cancel", 
                placeholder: prompt.Content, 
                initialValue: prompt.Content);

            if (!string.IsNullOrEmpty(result) && result != prompt.Content)
            {
                if (!prompt.IsEdited)
                {
                    prompt.IsEdited = true;
                    EditedPrompts++;
                }
                prompt.Content = result;
            }
        }
        catch
        {
            // Handle any navigation errors silently
        }
    }

    [RelayCommand]
    private void DeletePrompt(VideoPrompt prompt)
    {
        if (prompt.IsEdited)
        {
            EditedPrompts--;
        }
        Prompts.Remove(prompt);
        UpdateStatistics();
    }

    [RelayCommand]
    private async Task RegeneratePrompt(VideoPrompt prompt)
    {
        // Simulate regeneration
        await Task.Delay(1000);
        
        var regeneratedContent = prompt.Title switch
        {
            "Opening Hook" => "Begin with an unexpected cat appearing from behind furniture, dancing immediately",
            "Main Content" => "Create a montage of cats from different breeds, each showcasing unique dance styles",
            "Engagement Boost" => "Add interactive text: 'Tap ❤️ if your cat can dance too!'",
            "Call to Action" => "Conclude with 'Tag a friend who loves dancing cats!' with music note emojis",
            _ => "Regenerated prompt content"
        };

        if (!prompt.IsEdited)
        {
            prompt.IsEdited = true;
            EditedPrompts++;
        }
        prompt.Content = regeneratedContent;
    }

    [RelayCommand]
    private async Task GenerateVideo()
    {
        try
        {
            var page = GetCurrentPage();
            if (page != null)
            {
                await page.DisplayAlert(
                    "Generate Video", 
                    $"Ready to generate video with {TotalPrompts} prompts ({EditedPrompts} edited)!", 
                    "OK");
            }
        }
        catch
        {
            // Handle any navigation errors silently
        }
    }

    private Page? GetCurrentPage()
    {
        try
        {
            return Application.Current?.Windows?.FirstOrDefault()?.Page;
        }
        catch
        {
            return null;
        }
    }

    private void UpdateStatistics()
    {
        TotalPrompts = Prompts.Count;
        EditedPrompts = Prompts.Count(p => p.IsEdited);
    }
}

public partial class VideoPrompt : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string content = string.Empty;

    [ObservableProperty]
    private bool isEdited;
}
