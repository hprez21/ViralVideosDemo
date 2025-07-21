using System;
using System.Threading.Tasks;

namespace ViralVideosDemo.Services;

/// <summary>
/// Interface for SORA video generation service
/// 
/// Configuration is read from Preferences:
/// - SoraEndpoint: Azure SORA endpoint URL
/// - SoraApiKey: Azure SORA API key
/// - SoraDeployment: SORA model deployment name (default: "sora")
/// 
/// Generated videos are saved to: Documents/ViralVideos/
/// </summary>
public interface ISoraService
{
    /// <summary>
    /// Checks if SORA service is properly configured
    /// </summary>
    /// <returns>True if configured, false otherwise</returns>
    Task<bool> IsConfiguredAsync();

    /// <summary>
    /// Generates a video using SORA AI service
    /// </summary>
    /// <param name="prompt">The text prompt describing the video content</param>
    /// <param name="width">Video width in pixels (default: 480)</param>
    /// <param name="height">Video height in pixels (default: 480)</param>
    /// <param name="nSeconds">Video duration in seconds (default: 5)</param>
    /// <returns>The file path of the generated video</returns>
    /// <exception cref="InvalidOperationException">Thrown when service is not configured</exception>
    /// <exception cref="HttpRequestException">Thrown when API request fails</exception>
    /// <exception cref="TimeoutException">Thrown when video generation times out</exception>
    Task<string> GenerateVideoAsync(string prompt, int width = 480, int height = 480, int nSeconds = 5);

    /// <summary>
    /// Gets the current video generation status
    /// </summary>
    /// <returns>Status message describing current operation</returns>
    string GetCurrentStatus();
}
