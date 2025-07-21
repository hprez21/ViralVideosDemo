using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace ViralVideosDemo.Services;

public class ChatService : IChatService
{
    private readonly HttpClient _httpClient;
    private const string AZURE_LLM_ENDPOINT_KEY = "AzureLlmEndpoint";
    private const string AZURE_LLM_API_KEY = "AzureLlmApiKey";
    private const string AZURE_LLM_DEPLOYMENT_KEY = "AzureLlmDeployment";

    public ChatService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<bool> IsConfiguredAsync()
    {
        try
        {
            var endpoint = Preferences.Default.Get(AZURE_LLM_ENDPOINT_KEY, string.Empty);
            var apiKey = Preferences.Default.Get(AZURE_LLM_API_KEY, string.Empty);
            var deployment = Preferences.Default.Get(AZURE_LLM_DEPLOYMENT_KEY, string.Empty);

            var isConfigured = !string.IsNullOrWhiteSpace(endpoint) && 
                              !string.IsNullOrWhiteSpace(apiKey) && 
                              !string.IsNullOrWhiteSpace(deployment);
            
            return Task.FromResult(isConfigured);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task<string> EnhancePromptAsync(string originalIdea)
    {
        try
        {
            if (!await IsConfiguredAsync())
            {
                throw new InvalidOperationException("Azure LLM service is not configured. Please configure it in Settings.");
            }

            if (string.IsNullOrWhiteSpace(originalIdea))
            {
                return originalIdea;
            }

            var enhancementPrompt = $@"
You are an expert in creating viral video content. Please enhance the following video idea to make it more engaging, trendy, and likely to go viral on social media platforms like TikTok, Instagram Reels, and YouTube Shorts.

Original idea: {originalIdea}

Please provide an enhanced version that:
- Incorporates current trends
- Has strong hook potential
- Is optimized for short-form content
- Includes engaging elements like humor, surprise, or emotion
- Is concise and actionable

Enhanced idea:";

            var response = await CallAzureLLMAsync(enhancementPrompt);
            
            // Extract just the enhanced idea from the response
            var enhancedIdea = ExtractEnhancedIdea(response);
            
            return !string.IsNullOrWhiteSpace(enhancedIdea) ? enhancedIdea : originalIdea;
        }
        catch (Exception ex)
        {
            // Log error and return original idea as fallback
            System.Diagnostics.Debug.WriteLine($"Error enhancing prompt: {ex.Message}");
            return originalIdea;
        }
    }

    public async Task<List<VideoPromptResult>> GenerateVideoPromptsAsync(string videoIdea, bool isEnhanced)
    {
        try
        {
            if (!await IsConfiguredAsync())
            {
                // Return fallback prompts if service is not configured
                return GenerateFallbackPrompts(videoIdea);
            }

            var generationPrompt = $@"
You are an expert in creating viral video content. Based on the following video idea, generate 4 specific prompts that will help create a viral video.

Video idea: {videoIdea}
{(isEnhanced ? "(This idea has been AI-enhanced)" : "")}

Please provide exactly 4 prompts in the following categories:
1. Opening Hook - How to start the video to grab attention immediately
2. Main Content - The core content structure and flow
3. Engagement Boost - Elements to increase viewer interaction and engagement
4. Call to Action - How to end the video to maximize shares and follows

Format each prompt as:
Title: [Category Name]
Content: [Detailed prompt instructions]

Prompts:";

            var response = await CallAzureLLMAsync(generationPrompt);
            var prompts = ParseVideoPrompts(response);
            
            return prompts.Any() ? prompts : GenerateFallbackPrompts(videoIdea);
        }
        catch (Exception ex)
        {
            // Log error and return fallback prompts
            System.Diagnostics.Debug.WriteLine($"Error generating prompts: {ex.Message}");
            return GenerateFallbackPrompts(videoIdea);
        }
    }

    private async Task<string> CallAzureLLMAsync(string prompt)
    {
        var endpoint = Preferences.Default.Get(AZURE_LLM_ENDPOINT_KEY, string.Empty);
        var apiKey = Preferences.Default.Get(AZURE_LLM_API_KEY, string.Empty);
        var deployment = Preferences.Default.Get(AZURE_LLM_DEPLOYMENT_KEY, string.Empty);

        if (string.IsNullOrWhiteSpace(endpoint) || string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(deployment))
        {
            throw new InvalidOperationException("Azure LLM configuration is incomplete");
        }

        // Construct the Azure OpenAI endpoint URL
        var url = $"{endpoint.TrimEnd('/')}/openai/deployments/{deployment}/chat/completions?api-version=2024-02-15-preview";

        var requestBody = new
        {
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            max_tokens = 1000,
            temperature = 0.7,
            top_p = 0.95
        };

        var jsonContent = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("api-key", apiKey);

        var response = await _httpClient.PostAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Azure LLM API call failed: {response.StatusCode} - {errorContent}");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        if (jsonResponse.TryGetProperty("choices", out var choices) && 
            choices.GetArrayLength() > 0 &&
            choices[0].TryGetProperty("message", out var message) &&
            message.TryGetProperty("content", out var messageContent))
        {
            return messageContent.GetString() ?? string.Empty;
        }

        throw new InvalidOperationException("Unexpected response format from Azure LLM API");
    }

    private string ExtractEnhancedIdea(string response)
    {
        try
        {
            // Look for content after "Enhanced idea:" or similar patterns
            var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (line.Contains("Enhanced idea:", StringComparison.OrdinalIgnoreCase) ||
                    line.Contains("Enhanced version:", StringComparison.OrdinalIgnoreCase))
                {
                    // Try to get the content after the colon
                    var colonIndex = line.IndexOf(':');
                    if (colonIndex >= 0 && colonIndex < line.Length - 1)
                    {
                        var result = line.Substring(colonIndex + 1).Trim();
                        if (!string.IsNullOrWhiteSpace(result))
                        {
                            return result;
                        }
                    }
                    
                    // If no content after colon, check next lines
                    if (i + 1 < lines.Length)
                    {
                        var nextLine = lines[i + 1].Trim();
                        if (!string.IsNullOrWhiteSpace(nextLine))
                        {
                            return nextLine;
                        }
                    }
                }
            }

            // If no specific pattern found, return the last non-empty line
            var lastLine = lines.LastOrDefault(l => !string.IsNullOrWhiteSpace(l))?.Trim();
            return lastLine ?? response.Trim();
        }
        catch
        {
            return response.Trim();
        }
    }

    private List<VideoPromptResult> ParseVideoPrompts(string response)
    {
        var prompts = new List<VideoPromptResult>();
        
        try
        {
            var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            VideoPromptResult? currentPrompt = null;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                if (trimmedLine.StartsWith("Title:", StringComparison.OrdinalIgnoreCase))
                {
                    // Save previous prompt if exists
                    if (currentPrompt != null && !string.IsNullOrWhiteSpace(currentPrompt.Content))
                    {
                        prompts.Add(currentPrompt);
                    }
                    
                    // Start new prompt
                    currentPrompt = new VideoPromptResult
                    {
                        Title = trimmedLine.Substring(6).Trim()
                    };
                }
                else if (trimmedLine.StartsWith("Content:", StringComparison.OrdinalIgnoreCase) && currentPrompt != null)
                {
                    currentPrompt.Content = trimmedLine.Substring(8).Trim();
                }
                else if (currentPrompt != null && !string.IsNullOrWhiteSpace(trimmedLine) && string.IsNullOrWhiteSpace(currentPrompt.Content))
                {
                    // If we have a title but no content yet, this line might be the content
                    currentPrompt.Content = trimmedLine;
                }
            }

            // Add the last prompt
            if (currentPrompt != null && !string.IsNullOrWhiteSpace(currentPrompt.Content))
            {
                prompts.Add(currentPrompt);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error parsing video prompts: {ex.Message}");
        }

        return prompts;
    }

    private List<VideoPromptResult> GenerateFallbackPrompts(string videoIdea)
    {
        return new List<VideoPromptResult>
        {
            new VideoPromptResult
            {
                Title = "Opening Hook",
                Content = $"Start with an attention-grabbing opening that immediately showcases the main concept: {videoIdea}",
                Category = "Hook"
            },
            new VideoPromptResult
            {
                Title = "Main Content",
                Content = $"Create engaging content around: {videoIdea}. Keep it fast-paced and visually interesting for maximum retention.",
                Category = "Content"
            },
            new VideoPromptResult
            {
                Title = "Engagement Boost",
                Content = "Add interactive elements like questions, challenges, or calls for comments to increase viewer engagement and algorithm performance.",
                Category = "Engagement"
            },
            new VideoPromptResult
            {
                Title = "Call to Action",
                Content = "End with a strong call to action encouraging likes, follows, and shares. Use trending hashtags and encourage user-generated content.",
                Category = "CTA"
            }
        };
    }
}
