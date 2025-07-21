using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ViralVideosDemo.Services;

namespace ViralVideosDemo.ViewModels;

public partial class VideoPromptsViewModel : ObservableObject, IQueryAttributable
{
    private readonly IChatService _chatService;
    private readonly ISoraService _soraService;

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

    // Video configuration properties
    private int _videoWidth = 1280;      // Default horizontal 720p
    private int _videoHeight = 720;
    private int _videoDuration = 10;     // Default 10 seconds
    private string _videoOrientation = "Horizontal";

    public VideoPromptsViewModel(IChatService chatService, ISoraService soraService)
    {
        _chatService = chatService;
        _soraService = soraService;
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

        // Get video configuration parameters
        if (query.ContainsKey("VideoWidth"))
        {
            int.TryParse(query["VideoWidth"].ToString(), out _videoWidth);
        }

        if (query.ContainsKey("VideoHeight"))
        {
            int.TryParse(query["VideoHeight"].ToString(), out _videoHeight);
        }

        if (query.ContainsKey("VideoDuration"))
        {
            int.TryParse(query["VideoDuration"].ToString(), out _videoDuration);
        }

        if (query.ContainsKey("VideoOrientation"))
        {
            _videoOrientation = query["VideoOrientation"].ToString() ?? "Horizontal";
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
            Debug.WriteLine("[VideoPrompts] GenerateVideo started");
            var page = GetCurrentPage();
            
            // Check if SORA service is configured
            var isConfigured = await _soraService.IsConfiguredAsync();
            Debug.WriteLine($"[VideoPrompts] SORA configured: {isConfigured}");
            
            if (!isConfigured)
            {
                Debug.WriteLine("[VideoPrompts] SORA not configured, showing alert");
                if (page != null)
                {
                    await page.DisplayAlert("Configuración requerida", 
                        "Por favor configura las credenciales de SORA en la página de configuración.", 
                        "OK");
                }
                return;
            }

            // Activate loading UI (blocks all interaction)
            IsLoading = true;
            Debug.WriteLine("[VideoPrompts] Loading UI activated - blocking all interaction");

            // Generate the combined prompt from all prompts
            var combinedPrompt = string.Join(". ", Prompts.Select(p => p.Content));
            if (string.IsNullOrWhiteSpace(combinedPrompt))
            {
                combinedPrompt = VideoIdea; // Fallback to original idea
            }

            Debug.WriteLine($"[VideoPrompts] Combined prompt: {combinedPrompt}");
            Debug.WriteLine($"[VideoPrompts] Video configuration: {_videoWidth}x{_videoHeight}, {_videoDuration}s, {_videoOrientation}");
            Debug.WriteLine("[VideoPrompts] Starting SORA video generation...");

            // Generate video using SORA service with custom configuration
            var videoFilePath = await _soraService.GenerateVideoAsync(combinedPrompt, _videoWidth, _videoHeight, _videoDuration);
            
            Debug.WriteLine($"[VideoPrompts] ✅ Video generated successfully: {videoFilePath}");
            Debug.WriteLine($"[VideoPrompts] File exists: {File.Exists(videoFilePath)}");
            Debug.WriteLine($"[VideoPrompts] File size: {new FileInfo(videoFilePath).Length / 1024 / 1024:F2} MB");

            // Prepare navigation parameters
            var navigationParams = new Dictionary<string, object>
            {
                ["VideoTitle"] = VideoIdea,
                ["VideoSource"] = videoFilePath,
                ["OriginalPrompt"] = _originalIdea,
                ["EnhancedPrompt"] = _wasEnhanced ? _enhancedIdea : "",
                ["EnglishContent"] = combinedPrompt,
                ["HasEnhancedPrompt"] = _wasEnhanced.ToString()
            };

            Debug.WriteLine("[VideoPrompts] Navigation parameters:");
            foreach (var kvp in navigationParams)
            {
                Debug.WriteLine($"[VideoPrompts]   {kvp.Key}: {kvp.Value}");
            }

            // Navigate to video display page with real video
            Debug.WriteLine("[VideoPrompts] Navigating to VideoDisplay page...");
            await Shell.Current.GoToAsync("VideoDisplay", navigationParams);

            // Show success message
            if (page != null)
            {
                Debug.WriteLine("[VideoPrompts] Showing success message");
                await page.DisplayAlert("¡Video Generado!", 
                    $"Tu video viral ha sido creado exitosamente.\n\nArchivo: {Path.GetFileName(videoFilePath)}", 
                    "Ver Video");
            }
            
            Debug.WriteLine("[VideoPrompts] GenerateVideo completed successfully");
        }
        catch (InvalidOperationException ex)
        {
            Debug.WriteLine($"[VideoPrompts] Configuration error: {ex.Message}");
            var page = GetCurrentPage();
            if (page != null)
            {
                await page.DisplayAlert("Error de Configuración", ex.Message, "OK");
            }
        }
        catch (TimeoutException)
        {
            Debug.WriteLine("[VideoPrompts] Timeout error");
            var page = GetCurrentPage();
            if (page != null)
            {
                await page.DisplayAlert("Tiempo Agotado", 
                    "La generación del video tomó demasiado tiempo. Por favor intenta de nuevo.", 
                    "OK");
            }
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine($"[VideoPrompts] HTTP error: {ex.Message}");
            var page = GetCurrentPage();
            if (page != null)
            {
                await page.DisplayAlert("Error de Conexión", 
                    $"Error al conectar con el servicio SORA: {ex.Message}", 
                    "OK");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[VideoPrompts] Unexpected error: {ex.Message}");
            Debug.WriteLine($"[VideoPrompts] Exception type: {ex.GetType().Name}");
            Debug.WriteLine($"[VideoPrompts] Stack trace: {ex.StackTrace}");
            var page = GetCurrentPage();
            if (page != null)
            {
                await page.DisplayAlert("Error", $"Error al generar video: {ex.Message}", "OK");
            }
        }
        finally
        {
            // Always deactivate loading UI regardless of success or failure
            IsLoading = false;
            Debug.WriteLine("[VideoPrompts] Loading UI deactivated - interaction restored");
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
