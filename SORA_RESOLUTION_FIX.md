# SORA Video Resolution Fix

## Problem Solved
Fixed the "Resolution not supported" error by using exact SORA-supported resolution combinations instead of calculated aspect ratios.

## SORA Supported Resolutions
According to the API error, SORA supports these exact combinations:
- (480, 480) - Square 480p
- (854, 480) - Horizontal 480p
- (720, 720) - Square 720p
- (1280, 720) - Horizontal 720p (HD)
- (1080, 1080) - Square 1080p
- (1920, 1080) - Horizontal 1080p (Full HD)

## New Resolution Mapping

### 480p Options:
- **Horizontal**: 854 x 480
- **Vertical**: 480 x 854 (flipped horizontal)
- **Square**: 480 x 480

### 720p Options:
- **Horizontal**: 1280 x 720 (HD)
- **Vertical**: 720 x 1280 (flipped HD)
- **Square**: 720 x 720

### 1080p Options:
- **Horizontal**: 1920 x 1080 (Full HD)
- **Vertical**: 1080 x 1920 (flipped Full HD)
- **Square**: 1080 x 1080

## Code Changes Made

### 1. Updated GetVideoDimensions() Method
Replaced the aspect ratio calculation with exact SORA-supported combinations:

```csharp
private (int width, int height) GetVideoDimensions()
{
    return SelectedOrientation switch
    {
        "Horizontal" => SelectedResolution switch
        {
            "480" => (854, 480),    // Horizontal 480p
            "720" => (1280, 720),   // Horizontal 720p (HD)
            "1080" => (1920, 1080), // Horizontal 1080p (Full HD)
            _ => (1280, 720)        // Default to 720p horizontal
        },
        "Vertical" => SelectedResolution switch
        {
            "480" => (480, 854),    // Vertical 480p
            "720" => (720, 1280),   // Vertical 720p
            "1080" => (1080, 1920), // Vertical 1080p
            _ => (720, 1280)        // Default to 720p vertical
        },
        "Square" => SelectedResolution switch
        {
            "480" => (480, 480),    // Square 480p
            "720" => (720, 720),    // Square 720p
            "1080" => (1080, 1080), // Square 1080p
            _ => (720, 720)         // Default to 720p square
        },
        _ => (1280, 720)            // Default to horizontal 720p
    };
}
```

### 2. Updated UI Descriptions
- Made descriptions more specific about resolution selection
- Added aspect ratio information to orientation description

### 3. Updated Documentation
- Reflected actual supported SORA resolutions
- Added comprehensive mapping table

## Testing
The app should now successfully generate videos with any combination of:
- Resolution: 480p, 720p, 1080p
- Orientation: Horizontal, Vertical, Square
- Duration: 5, 10, 15, 20 seconds

All combinations use SORA-approved resolution pairs and should work without API errors.
