# Video Configuration Features

## Overview
The app now includes comprehensive video configuration controls that allow users to customize their AI-generated videos according to their preferences.

## Configuration Options

### Resolution Options
Users can select from three standard resolutions:
- **480p**: Basic quality, faster generation
- **720p**: High definition (default)
- **1080p**: Full HD, highest quality but slower generation

### Duration Options
Available video durations in seconds:
- **5 seconds**: Quick clips
- **10 seconds**: Standard length (default)
- **15 seconds**: Medium length
- **20 seconds**: Longer content

### Orientation Options
Three aspect ratio choices with SORA-supported dimensions:
- **Horizontal**: 16:9 aspect ratio (854x480, 1280x720, 1920x1080)
- **Vertical**: 9:16 aspect ratio (480x854, 720x1280, 1080x1920)
- **Square**: 1:1 aspect ratio (480x480, 720x720, 1080x1080)

## Technical Implementation

### UI Components
- Added video configuration section in `AddVideoIdeaPage.xaml`
- Three picker controls for resolution, duration, and orientation
- Modern, user-friendly design with descriptions
- Proper theming support for light/dark modes

### ViewModel Properties
New properties in `AddVideoIdeaViewModel.cs`:
```csharp
[ObservableProperty]
private string selectedResolution = "720";

[ObservableProperty]
private string selectedDuration = "10";

[ObservableProperty]
private string selectedOrientation = "Horizontal";

// Lists for picker binding
public List<string> ResolutionOptions { get; } = ["480", "720", "1080"];
public List<string> DurationOptions { get; } = ["5", "10", "15", "20"];
public List<string> OrientationOptions { get; } = ["Horizontal", "Vertical", "Square"];
```

### Dimension Calculation
The `GetVideoDimensions()` method uses SORA-supported resolution combinations:

**Supported SORA Resolutions:**
- (480, 480), (854, 480), (720, 720), (1280, 720), (1080, 1080), (1920, 1080)

**Resolution Mapping:**
- 480p Horizontal: 854x480
- 480p Vertical: 480x854  
- 480p Square: 480x480
- 720p Horizontal: 1280x720
- 720p Vertical: 720x1280
- 720p Square: 720x720
- 1080p Horizontal: 1920x1080
- 1080p Vertical: 1080x1920
- 1080p Square: 1080x1080

### Navigation Parameters
Configuration is passed to `VideoPromptsViewModel` via navigation:
```csharp
var navigationParameters = new Dictionary<string, object>
{
    ["VideoWidth"] = width.ToString(),
    ["VideoHeight"] = height.ToString(),
    ["VideoDuration"] = duration.ToString(),
    ["VideoOrientation"] = SelectedOrientation
};
```

### SORA Integration
The configuration is used when calling the SORA video generation service:
```csharp
var videoFilePath = await _soraService.GenerateVideoAsync(
    combinedPrompt, 
    _videoWidth, 
    _videoHeight, 
    _videoDuration
);
```

## User Experience
1. **Visual Feedback**: Each option includes helpful descriptions
2. **Smart Defaults**: Pre-selected with commonly used settings (720p, 10s, Horizontal)
3. **Responsive Design**: Works across all platforms and screen sizes
4. **Real-time Updates**: Configuration flows seamlessly through the app

## Benefits
- **Customization**: Users can tailor videos to their specific needs
- **Platform Optimization**: Different orientations for different social platforms
- **Quality Control**: Balance between quality and generation time
- **Professional Output**: Support for various professional video standards
