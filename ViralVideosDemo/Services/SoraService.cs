using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace ViralVideosDemo.Services;

/// <summary>
/// Service for generating videos using SORA AI
/// </summary>
public class SoraService : ISoraService
{
    private readonly HttpClient _httpClient;
    private string _currentStatus = "Ready";
    
    // Configuration keys for Preferences
    private const string SORA_ENDPOINT_KEY = "SoraEndpoint";
    private const string SORA_API_KEY_KEY = "SoraApiKey";
    private const string SORA_DEPLOYMENT_KEY = "SoraDeployment";
    
    // Default output directory
    private readonly string _outputDirectory;

    public SoraService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        
        // Create default output directory in Documents/ViralVideos
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        _outputDirectory = Path.Combine(documentsPath, "ViralVideos");
        
        // Ensure output directory exists
        if (!Directory.Exists(_outputDirectory))
        {
            Directory.CreateDirectory(_outputDirectory);
        }
    }

    /// <summary>
    /// Checks if SORA service is properly configured
    /// </summary>
    public async Task<bool> IsConfiguredAsync()
    {
        await Task.CompletedTask; // Make it async for consistency
        
        var endpoint = Preferences.Default.Get(SORA_ENDPOINT_KEY, string.Empty);
        var apiKey = Preferences.Default.Get(SORA_API_KEY_KEY, string.Empty);
        
        return !string.IsNullOrWhiteSpace(endpoint) && !string.IsNullOrWhiteSpace(apiKey);
    }

    /// <summary>
    /// Gets the current video generation status
    /// </summary>
    public string GetCurrentStatus()
    {
        return _currentStatus;
    }

    /// <summary>
    /// Generates a video using SORA AI service
    /// </summary>
    public async Task<string> GenerateVideoAsync(string prompt, int width = 480, int height = 480, int nSeconds = 5)
    {
        var start = DateTime.Now;
        _currentStatus = "Initializing video generation...";
        Debug.WriteLine($"[SORA] Starting video generation at {start:yyyy-MM-dd HH:mm:ss}");

        // Validate configuration
        if (!await IsConfiguredAsync())
        {
            var error = "SORA service is not configured. Please set endpoint and API key in settings.";
            Debug.WriteLine($"[SORA] ERROR: {error}");
            throw new InvalidOperationException(error);
        }

        // Get configuration from Preferences
        var soraEndpoint = Preferences.Default.Get(SORA_ENDPOINT_KEY, string.Empty);
        var soraApiKey = Preferences.Default.Get(SORA_API_KEY_KEY, string.Empty);
        var soraModel = Preferences.Default.Get(SORA_DEPLOYMENT_KEY, "sora");

        Debug.WriteLine($"[SORA] Configuration loaded:");
        Debug.WriteLine($"[SORA]   Endpoint: {soraEndpoint}");
        Debug.WriteLine($"[SORA]   Model: {soraModel}");
        Debug.WriteLine($"[SORA]   API Key: {soraApiKey[..Math.Min(8, soraApiKey.Length)]}...");

        // Validate prompt
        if (string.IsNullOrWhiteSpace(prompt))
        {
            var error = "Prompt cannot be empty";
            Debug.WriteLine($"[SORA] ERROR: {error}");
            throw new ArgumentException(error, nameof(prompt));
        }

        Debug.WriteLine($"[SORA] Video parameters:");
        Debug.WriteLine($"[SORA]   Prompt: {prompt}");
        Debug.WriteLine($"[SORA]   Size: {width}x{height}");
        Debug.WriteLine($"[SORA]   Duration: {nSeconds} seconds");

        // Setup HTTP client headers
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("api-key", soraApiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        // Generate unique filename
        var timestamp = DateTime.Now.ToString("ddMMMyyyy_HHmmss");
        var safeSuffix = CreateSafeFilename(prompt, 30);
        var outputFilename = Path.Combine(_outputDirectory, $"sora_{timestamp}_{safeSuffix}.mp4");

        Debug.WriteLine($"[SORA] Output directory: {_outputDirectory}");
        Debug.WriteLine($"[SORA] Output filename: {outputFilename}");

        try
        {
            // Step 1: Create video generation job
            _currentStatus = "Creating video generation job...";
            Debug.WriteLine($"[SORA] Step 1: Creating video generation job...");
            var jobId = await CreateVideoGenerationJobAsync(soraEndpoint, prompt, width, height, nSeconds, soraModel);
            Debug.WriteLine($"[SORA] Job created successfully with ID: {jobId}");

            // Step 2: Poll for job completion
            _currentStatus = "Processing video generation...";
            Debug.WriteLine($"[SORA] Step 2: Polling job status...");
            var statusResponse = await PollJobStatusAsync(soraEndpoint, jobId);
            Debug.WriteLine($"[SORA] Job completed successfully");

            // Step 3: Download generated video
            _currentStatus = "Downloading generated video...";
            Debug.WriteLine($"[SORA] Step 3: Downloading video to {outputFilename}");
            await DownloadGeneratedVideoAsync(soraEndpoint, statusResponse, outputFilename);

            var elapsed = DateTime.Now - start;
            _currentStatus = $"Video generation completed in {elapsed.Minutes}m {elapsed.Seconds}s";
            Debug.WriteLine($"[SORA] ✅ Video generation completed successfully!");
            Debug.WriteLine($"[SORA] Total time: {elapsed.Minutes}m {elapsed.Seconds}s");
            Debug.WriteLine($"[SORA] Video saved to: {outputFilename}");
            Debug.WriteLine($"[SORA] File size: {new FileInfo(outputFilename).Length / 1024 / 1024:F2} MB");
            
            return outputFilename;
        }
        catch (Exception ex)
        {
            _currentStatus = $"Error: {ex.Message}";
            Debug.WriteLine($"[SORA] ❌ ERROR during video generation: {ex.Message}");
            Debug.WriteLine($"[SORA] Exception type: {ex.GetType().Name}");
            Debug.WriteLine($"[SORA] Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// Creates a video generation job
    /// </summary>
    private async Task<string> CreateVideoGenerationJobAsync(string endpoint, string prompt, int width, int height, int nSeconds, string model)
    {
        var apiVersion = "preview";
        var createUrl = $"{endpoint}/openai/v1/video/generations/jobs?api-version={apiVersion}";
        
        Debug.WriteLine($"[SORA] Creating job at URL: {createUrl}");
        
        var requestBody = new
        {
            prompt = prompt,
            width = width,
            height = height,
            n_seconds = nSeconds,
            model = model
        };

        var bodyJson = JsonSerializer.Serialize(requestBody);
        Debug.WriteLine($"[SORA] Request body: {bodyJson}");
        
        var content = new StringContent(bodyJson);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await _httpClient.PostAsync(createUrl, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        Debug.WriteLine($"[SORA] Response status: {response.StatusCode}");
        Debug.WriteLine($"[SORA] Response body: {responseBody}");

        if (!response.IsSuccessStatusCode)
        {
            var error = $"Failed to create video generation job. Status: {response.StatusCode}, Response: {responseBody}";
            Debug.WriteLine($"[SORA] ERROR: {error}");
            throw new HttpRequestException(error);
        }

        using var doc = JsonDocument.Parse(responseBody);
        var jobId = doc.RootElement.GetProperty("id").GetString();
        
        if (string.IsNullOrEmpty(jobId))
        {
            var error = "Failed to get job ID from response";
            Debug.WriteLine($"[SORA] ERROR: {error}");
            throw new InvalidOperationException(error);
        }

        return jobId;
    }

    /// <summary>
    /// Polls job status until completion
    /// </summary>
    private async Task<JsonElement> PollJobStatusAsync(string endpoint, string jobId)
    {
        var apiVersion = "preview";
        var statusUrl = $"{endpoint}/openai/v1/video/generations/jobs/{jobId}?api-version={apiVersion}";
        
        Debug.WriteLine($"[SORA] Polling status at URL: {statusUrl}");
        
        string status;
        JsonElement statusResponse;
        var pollCount = 0;
        const int maxPolls = 120; // 10 minutes timeout (5 seconds * 120)

        do
        {
            if (pollCount >= maxPolls)
            {
                var error = "Video generation timed out after 10 minutes";
                Debug.WriteLine($"[SORA] ERROR: {error}");
                throw new TimeoutException(error);
            }

            await Task.Delay(5000); // Wait 5 seconds between polls
            pollCount++;

            Debug.WriteLine($"[SORA] Poll #{pollCount}/{maxPolls} - Checking job status...");

            var statusResp = await _httpClient.GetAsync(statusUrl);
            var statusJson = await statusResp.Content.ReadAsStringAsync();
            
            Debug.WriteLine($"[SORA] Poll response status: {statusResp.StatusCode}");
            Debug.WriteLine($"[SORA] Poll response body: {statusJson}");
            
            if (!statusResp.IsSuccessStatusCode)
            {
                var error = $"Failed to get job status. Status: {statusResp.StatusCode}, Response: {statusJson}";
                Debug.WriteLine($"[SORA] ERROR: {error}");
                throw new HttpRequestException(error);
            }

            var statusDoc = JsonDocument.Parse(statusJson);
            statusResponse = statusDoc.RootElement;
            status = statusResponse.GetProperty("status").GetString() ?? "unknown";
            
            Debug.WriteLine($"[SORA] Job status: {status}");
            _currentStatus = $"Video generation in progress... Status: {status} (Poll {pollCount}/{maxPolls})";

        } while (status != "succeeded" && status != "failed" && status != "cancelled");

        if (status != "succeeded")
        {
            var error = $"Video generation failed with status: {status}";
            Debug.WriteLine($"[SORA] ERROR: {error}");
            throw new InvalidOperationException(error);
        }

        Debug.WriteLine($"[SORA] ✅ Job completed successfully after {pollCount} polls");
        return statusResponse;
    }

    /// <summary>
    /// Downloads the generated video file
    /// </summary>
    private async Task DownloadGeneratedVideoAsync(string endpoint, JsonElement statusResponse, string outputFilename)
    {
        if (!statusResponse.TryGetProperty("generations", out JsonElement generations) || generations.GetArrayLength() == 0)
        {
            var error = "No video generations found in job result";
            Debug.WriteLine($"[SORA] ERROR: {error}");
            throw new InvalidOperationException(error);
        }

        var generationId = generations[0].GetProperty("id").GetString();
        Debug.WriteLine($"[SORA] Generation ID: {generationId}");
        
        var apiVersion = "preview";
        var videoUrl = $"{endpoint}/openai/v1/video/generations/{generationId}/content/video?api-version={apiVersion}";

        Debug.WriteLine($"[SORA] Downloading video from URL: {videoUrl}");
        Debug.WriteLine($"[SORA] Saving to file: {outputFilename}");

        var videoResponse = await _httpClient.GetAsync(videoUrl);
        
        Debug.WriteLine($"[SORA] Download response status: {videoResponse.StatusCode}");
        Debug.WriteLine($"[SORA] Content length: {videoResponse.Content.Headers.ContentLength} bytes");
        
        if (!videoResponse.IsSuccessStatusCode)
        {
            var error = $"Failed to download video content. Status: {videoResponse.StatusCode}";
            Debug.WriteLine($"[SORA] ERROR: {error}");
            throw new HttpRequestException(error);
        }

        // Ensure output directory exists
        var outputDir = Path.GetDirectoryName(outputFilename);
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir!);
            Debug.WriteLine($"[SORA] Created output directory: {outputDir}");
        }

        using var fileStream = new FileStream(outputFilename, FileMode.Create, FileAccess.Write);
        await videoResponse.Content.CopyToAsync(fileStream);
        
        Debug.WriteLine($"[SORA] ✅ Video downloaded successfully");
        Debug.WriteLine($"[SORA] File size on disk: {new FileInfo(outputFilename).Length} bytes");
        Debug.WriteLine($"[SORA] File exists: {File.Exists(outputFilename)}");
    }

    /// <summary>
    /// Creates a safe filename from the prompt
    /// </summary>
    private static string CreateSafeFilename(string prompt, int maxLength)
    {
        if (string.IsNullOrEmpty(prompt))
            return "video";

        var safeName = prompt.Length > maxLength ? prompt.Substring(0, maxLength) : prompt;
        
        // Replace invalid filename characters
        var invalidChars = Path.GetInvalidFileNameChars();
        foreach (var invalidChar in invalidChars)
        {
            safeName = safeName.Replace(invalidChar, '_');
        }
        
        // Replace common problematic characters
        safeName = safeName
            .Replace(" ", "_")
            .Replace(",", "_")
            .Replace(".", "_")
            .Replace(":", "_")
            .Replace(";", "_");

        return string.IsNullOrEmpty(safeName) ? "video" : safeName;
    }
}
