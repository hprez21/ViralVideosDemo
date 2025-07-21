using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ViralVideosDemo.Services;

namespace ViralVideosDemo.ViewModels;

public partial class VideoPromptsViewModel : ObservableObject, IQueryAttributable
{
    private readonly IChatService _chatService;

    [ObservableProperty]
    private ObservableCollection<VideoPrompt> prompts = new();

    [ObservableProperty]
    private int totalPrompts = 0;

    [ObservableProperty]
    private int editedPrompts = 0;

    [ObservableProperty]
    private string videoIdea = string.Empty;

    [ObservableProperty]
    private bool isLoading = false;

    // Store original and enhanced ideas
    private string _originalIdea = string.Empty;
    private string _enhancedIdea = string.Empty;
    private bool _wasEnhanced = false;

    public VideoPromptsViewModel(IChatService chatService)
    {
        _chatService = chatService;
        // Don't load sample data by default anymore
    }

    // IQueryAttributable implementation
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("VideoIdea"))
        {
            _originalIdea = query["VideoIdea"].ToString() ?? "";
        }

        if (query.ContainsKey("EnhancedIdea"))
        {
            _enhancedIdea = query["EnhancedIdea"].ToString() ?? "";
            _wasEnhanced = !string.IsNullOrEmpty(_enhancedIdea);
        }

        if (query.ContainsKey("WasEnhanced"))
        {
            _wasEnhanced = bool.Parse(query["WasEnhanced"].ToString() ?? "false");
        }

        // Set the display idea (enhanced if available, otherwise original)
        VideoIdea = _wasEnhanced && !string.IsNullOrEmpty(_enhancedIdea) ? _enhancedIdea : _originalIdea;

        // Generate prompts based on the video idea
        _ = GeneratePromptsFromAI();
    }

    private async Task GeneratePromptsFromAI()
    {
        if (string.IsNullOrWhiteSpace(VideoIdea))
        {
            LoadSampleData(); // Fallback to sample data
            return;
        }

        IsLoading = true;
        Prompts.Clear();

        try
        {
            var isConfigured = await _chatService.IsConfiguredAsync();
            if (!isConfigured)
            {
                LoadSampleData(); // Fallback to sample data
                return;
            }

            // Generate prompts using AI
            var generatedPrompts = await _chatService.GenerateVideoPromptsAsync(VideoIdea, _wasEnhanced);
            
            int promptId = 1;
            foreach (var prompt in generatedPrompts)
            {
                Prompts.Add(new VideoPrompt
                {
                    Id = promptId++,
                    Title = prompt.Title,
                    Content = prompt.Content,
                    IsEdited = false
                });
            }

            UpdateStatistics();
        }
        catch (Exception)
        {
            // If AI generation fails, fall back to sample data
            LoadSampleData();
        }
        finally
        {
            IsLoading = false;
        }
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
            // Simulate video generation process
            var page = GetCurrentPage();
            if (page != null)
            {
                await page.DisplayAlert(
                    "Generating Video", 
                    "Creating your viral video with AI. This may take a few moments...", 
                    "OK");
            }

            // Navigate to video display page
            await Shell.Current.GoToAsync("VideoDisplay", new Dictionary<string, object>
            {
                ["VideoTitle"] = VideoIdea,
                ["OriginalPrompt"] = VideoIdea,
                ["EnhancedPrompt"] = string.Join("; ", Prompts.Select(p => p.Content)),
                ["EnglishContent"] = $"AI-generated viral video based on: {VideoIdea}. This video includes {TotalPrompts} carefully crafted scenes designed to maximize engagement and virality."
            });
        }
        catch (Exception ex)
        {
            var page = GetCurrentPage();
            if (page != null)
            {
                await page.DisplayAlert("Error", $"Failed to generate video: {ex.Message}", "OK");
            }
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
