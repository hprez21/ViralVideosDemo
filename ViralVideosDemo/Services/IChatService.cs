namespace ViralVideosDemo.Services;

public interface IChatService
{
    /// <summary>
    /// Checks if the Azure LLM service is configured properly
    /// </summary>
    /// <returns>True if the service is configured and ready to use</returns>
    Task<bool> IsConfiguredAsync();

    /// <summary>
    /// Enhances a video idea prompt using Azure LLM
    /// </summary>
    /// <param name="originalIdea">The original video idea from the user</param>
    /// <returns>Enhanced prompt or the original idea if enhancement fails</returns>
    Task<string> EnhancePromptAsync(string originalIdea);

    /// <summary>
    /// Generates viral video prompts based on the provided idea
    /// </summary>
    /// <param name="videoIdea">The video idea to generate prompts for</param>
    /// <param name="isEnhanced">Whether the idea was enhanced by AI</param>
    /// <returns>A list of generated prompts</returns>
    Task<List<VideoPromptResult>> GenerateVideoPromptsAsync(string videoIdea, bool isEnhanced);
}

public class VideoPromptResult
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}
