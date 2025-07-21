using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using System.IO;

namespace ViralVideosDemo.ViewModels
{
    public partial class VideoDisplayViewModel : ObservableObject, IQueryAttributable
    {
        private MediaElement? _mediaElement;

        [ObservableProperty]
        private string videoTitle = "Generated Viral Video";

        [ObservableProperty]
        private string videoSource = "";

        [ObservableProperty]
        private string videoDescription = "AI-generated viral video content";

        [ObservableProperty]
        private string videoDuration = "Loading...";

        [ObservableProperty]
        private string videoStatus = "Ready";

        [ObservableProperty]
        private string originalPrompt = "";

        [ObservableProperty]
        private string enhancedPrompt = "";

        [ObservableProperty]
        private string englishContent = "";

        [ObservableProperty]
        private bool hasEnhancedPrompt = false;

        [ObservableProperty]
        private bool isLoading = true;

        public VideoDisplayViewModel()
        {
            // Sample data for demonstration
            VideoTitle = "AI Generated Viral Video";
            VideoDescription = "This video was generated using AI based on your creative prompt and enhanced with advanced algorithms.";
            OriginalPrompt = "A cat playing with a ball of yarn";
            EnhancedPrompt = "A playful orange tabby cat with bright green eyes gracefully pouncing and rolling around with a large, fluffy ball of rainbow-colored yarn in a cozy, sunlit living room with wooden floors and comfortable furniture.";
            EnglishContent = "Watch as this adorable cat brings joy and entertainment through its playful interaction with colorful yarn. Perfect for social media sharing and guaranteed to make viewers smile!";
            HasEnhancedPrompt = !string.IsNullOrEmpty(EnhancedPrompt);
            
            // For demo purposes, you can use a sample video URL or local file
            // VideoSource = "https://sample-videos.com/zip/10/mp4/SampleVideo_1280x720_1mb.mp4";
        }

        public void SetMediaElement(MediaElement mediaElement)
        {
            _mediaElement = mediaElement;
            
            // Subscribe to MediaElement events
            if (_mediaElement != null)
            {
                _mediaElement.MediaOpened += OnMediaOpened;
                _mediaElement.MediaEnded += OnMediaEnded;
                _mediaElement.MediaFailed += OnMediaFailed;
            }
        }

        private void OnMediaOpened(object? sender, EventArgs e)
        {
            if (_mediaElement != null)
            {
                VideoDuration = _mediaElement.Duration.ToString(@"mm\:ss");
                VideoStatus = "Ready to play";
                IsLoading = false; // Stop loading when media is ready
            }
        }

        private void OnMediaEnded(object? sender, EventArgs e)
        {
            VideoStatus = "Playback completed";
        }

        private void OnMediaFailed(object? sender, EventArgs e)
        {
            VideoStatus = "Error loading video";
            IsLoading = false; // Stop loading on error
        }

        [RelayCommand]
        private void Play()
        {
            try
            {
                _mediaElement?.Play();
                VideoStatus = "Playing";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error playing video: {ex.Message}");
                VideoStatus = "Error playing video";
            }
        }

        [RelayCommand]
        private void Pause()
        {
            try
            {
                _mediaElement?.Pause();
                VideoStatus = "Paused";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error pausing video: {ex.Message}");
            }
        }

        [RelayCommand]
        private void Stop()
        {
            try
            {
                _mediaElement?.Stop();
                VideoStatus = "Stopped";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error stopping video: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task ShareVideo()
        {
            try
            {
                if (!string.IsNullOrEmpty(VideoSource))
                {
                    await Shell.Current.DisplayAlert("Share Video", $"Sharing: {VideoTitle}\n\nCheck out this AI-generated viral video!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Info", "No video available to share", "OK");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sharing video: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Unable to share video", "OK");
            }
        }

        [RelayCommand]
        private async Task Download()
        {
            try
            {
                // In a real app, this would download the video file
                await Shell.Current.DisplayAlert("Info", "Download functionality will be implemented with actual video generation service", "OK");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error downloading video: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Unable to download video", "OK");
            }
        }

        [RelayCommand]
        private async Task Back()
        {
            try
            {
                // Stop video before navigating back
                _mediaElement?.Stop();
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error navigating back: {ex.Message}");
            }
        }

        // Method to set video data from external source
        public void SetVideoData(string title, string source, string originalPrompt, string enhancedPrompt = "", string englishContent = "")
        {
            VideoTitle = title;
            VideoSource = source;
            OriginalPrompt = originalPrompt;
            EnhancedPrompt = enhancedPrompt;
            EnglishContent = englishContent;
            HasEnhancedPrompt = !string.IsNullOrEmpty(enhancedPrompt);
        }

        // IQueryAttributable implementation
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Debug.WriteLine("[VideoDisplay] ApplyQueryAttributes called");
            Debug.WriteLine($"[VideoDisplay] Received {query.Count} parameters:");
            
            // Start loading when receiving new video parameters
            IsLoading = true;
            
            foreach (var kvp in query)
            {
                Debug.WriteLine($"[VideoDisplay]   {kvp.Key}: {kvp.Value}");
            }

            if (query.ContainsKey("VideoTitle"))
            {
                VideoTitle = query["VideoTitle"].ToString() ?? "";
                Debug.WriteLine($"[VideoDisplay] Set VideoTitle: {VideoTitle}");
            }

            if (query.ContainsKey("VideoSource"))
            {
                var videoPath = query["VideoSource"].ToString() ?? "";
                Debug.WriteLine($"[VideoDisplay] Received VideoSource: {videoPath}");
                
                // Validate that the file exists
                if (File.Exists(videoPath))
                {
                    VideoSource = videoPath;
                    Debug.WriteLine($"[VideoDisplay] ✅ Video file exists, set VideoSource: {VideoSource}");
                }
                else
                {
                    Debug.WriteLine($"[VideoDisplay] ❌ Video file not found: {videoPath}");
                    // Fallback to sample video for demo
                    VideoSource = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
                    Debug.WriteLine($"[VideoDisplay] Using fallback video: {VideoSource}");
                }
            }
            else
            {
                Debug.WriteLine("[VideoDisplay] No VideoSource parameter received, using fallback");
                VideoSource = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
            }

            if (query.ContainsKey("OriginalPrompt"))
            {
                OriginalPrompt = query["OriginalPrompt"].ToString() ?? "";
                Debug.WriteLine($"[VideoDisplay] Set OriginalPrompt: {OriginalPrompt}");
            }

            if (query.ContainsKey("EnhancedPrompt"))
            {
                EnhancedPrompt = query["EnhancedPrompt"].ToString() ?? "";
                HasEnhancedPrompt = !string.IsNullOrEmpty(EnhancedPrompt);
                Debug.WriteLine($"[VideoDisplay] Set EnhancedPrompt: {EnhancedPrompt}");
                Debug.WriteLine($"[VideoDisplay] HasEnhancedPrompt: {HasEnhancedPrompt}");
            }

            if (query.ContainsKey("HasEnhancedPrompt"))
            {
                if (bool.TryParse(query["HasEnhancedPrompt"].ToString(), out bool hasEnhanced))
                {
                    HasEnhancedPrompt = hasEnhanced;
                    Debug.WriteLine($"[VideoDisplay] Set HasEnhancedPrompt from parameter: {HasEnhancedPrompt}");
                }
            }

            if (query.ContainsKey("EnglishContent"))
            {
                EnglishContent = query["EnglishContent"].ToString() ?? "";
                Debug.WriteLine($"[VideoDisplay] Set EnglishContent: {EnglishContent}");
            }

            VideoDescription = $"AI-generated viral video: {VideoTitle}";
            Debug.WriteLine($"[VideoDisplay] Set VideoDescription: {VideoDescription}");
            Debug.WriteLine("[VideoDisplay] ApplyQueryAttributes completed");
        }
    }
}
