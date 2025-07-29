# 🎬 Viral Videos Demo

A cross-platform mobile application built with .NET MAUI 9.0 that leverages AI to create viral video content using Azure OpenAI's SORA and LLM services.

https://github.com/user-attachments/assets/a020c54c-b8cd-4df2-83f7-5b098cb72996

## 📱 Overview

Viral Videos Demo is an innovative mobile app that transforms user ideas into viral video content through the power of artificial intelligence. The app combines Azure OpenAI's language models for prompt enhancement and SORA API for video generation, providing users with a seamless experience to create engaging video content.

## ✨ Key Features

### 🧠 AI-Powered Prompt Enhancement
- Transform basic ideas into optimized viral video prompts
- Azure OpenAI LLM integration for intelligent content creation
- Real-time prompt improvement and optimization

### 🎥 Video Generation with SORA
- Generate high-quality videos using Azure OpenAI's SORA API
- Multiple resolution options (480p, 720p, 1080p)
- Customizable video duration (5-20 seconds)
- Support for different orientations (horizontal, vertical, square)

### 🎨 User-Friendly Interface
- Intuitive XAML-based UI with modern design
- Comprehensive loading indicators for better UX
- Fullscreen blocking loaders for heavy operations
- Dark/Light theme support

### ⚙️ Advanced Configuration
- Complete video configuration controls
- Secure credential management through device preferences
- Real-time service status validation

## 🏗️ Technical Architecture

### Platform Support
- **Android** 21.0+
- **iOS** 15.0+
- **macOS** (Mac Catalyst) 15.0+
- **Windows** 10.0.17763.0+

### Technology Stack
- **.NET 9.0** - Latest .NET framework
- **MAUI 9.0** - Cross-platform UI framework
- **MVVM Toolkit 8.4.0** - Modern MVVM implementation
- **CommunityToolkit.Maui.MediaElement 6.1.1** - Video playback
- **Azure OpenAI Services** - AI processing and video generation

### Key Components

#### Services
- **IChatService** - Azure OpenAI LLM integration for prompt enhancement
- **ISoraService** - Azure OpenAI SORA API for video generation
- **Dependency Injection** - Clean service architecture

#### ViewModels
- **AddVideoIdeaViewModel** - Manages video idea input and enhancement
- **VideoPromptsViewModel** - Handles prompt generation and video creation
- **VideoDisplayViewModel** - Controls video playback and metadata
- **SettingsViewModel** - Manages app configuration and credentials

#### Pages
- **AddVideoIdeaPage** - User input and AI enhancement interface
- **VideoPromptsPage** - Generated prompts editing and video creation
- **VideoDisplayPage** - Video playback and sharing
- **SettingsPage** - App configuration and service setup

## 🚀 Getting Started

### Prerequisites
- Visual Studio 2022 17.8+ or Visual Studio Code
- .NET 9.0 SDK
- Azure OpenAI access with SORA API availability
- Platform-specific development tools (Android SDK, Xcode, etc.)

### Configuration
1. **Clone the repository**
   ```bash
   git clone https://github.com/hprez21/ViralVideosDemo.git
   cd ViralVideosDemo
   ```

2. **Set up Azure OpenAI Services**
   - Create an Azure OpenAI resource
   - Deploy SORA model
   - Deploy a language model (GPT-4, etc.)
   - Obtain endpoint URLs and API keys

3. **Configure the app**
   - Launch the app and navigate to Settings
   - Enter your Azure OpenAI credentials:
     - **LLM Endpoint**: `https://your-resource.openai.azure.com`
     - **LLM API Key**: Your Azure OpenAI API key
     - **LLM Deployment**: Your model deployment name
     - **SORA Endpoint**: `https://your-sora-resource.openai.azure.com`
     - **SORA API Key**: Your SORA API key
     - **SORA Deployment**: Your SORA deployment name (usually "sora")

4. **Build and run**
   ```bash
   dotnet build
   dotnet run
   ```

## 📋 Usage

### Creating Viral Videos

1. **Enter Your Idea**
   - Open the app and navigate to "Add Video Idea"
   - Enter your creative video concept
   - Optionally enable AI enhancement for improved prompts

2. **Configure Video Settings**
   - Select resolution (480p/720p/1080p)
   - Choose duration (5-20 seconds)
   - Pick orientation (horizontal/vertical/square)

3. **Generate Enhanced Prompts**
   - Tap "Generate Viral Story"
   - AI will enhance your idea and create optimized prompts
   - Review and edit generated prompts as needed

4. **Create Your Video**
   - Tap "Generate Video" to start SORA processing
   - Wait for the AI to generate your video (may take several minutes)
   - View your generated video in the player

5. **Share and Enjoy**
   - Watch your AI-generated viral video
   - Share with your audience

## 🎯 Key Features in Detail

### Video Configuration
- **Resolution Options**: 480p (854x480), 720p (1280x720), 1080p (1920x1080)
- **Duration Flexibility**: 5, 10, 15, or 20-second videos
- **Orientation Support**: Horizontal, vertical, and square formats
- **SORA Compatibility**: Exact resolution mapping for API compliance

### Loading Experience
- **Smart Loading States**: Different loaders for different operations
- **Fullscreen Blocking**: UI blocking during video generation
- **Progress Feedback**: Clear messaging about operation status
- **Error Handling**: Comprehensive error management with user-friendly messages

### AI Integration
- **Prompt Enhancement**: Intelligent improvement of user ideas
- **Multi-step Processing**: AI enhancement → Prompt generation → Video creation
- **Quality Optimization**: SORA-specific prompt optimization for better videos

## 📁 Project Structure

```
ViralVideosDemo/
├── Pages/                      # XAML UI pages
│   ├── AddVideoIdeaPage.xaml   # Video idea input
│   ├── VideoPromptsPage.xaml   # Prompt editing and generation
│   ├── VideoDisplayPage.xaml   # Video playback
│   └── SettingsPage.xaml       # App configuration
├── ViewModels/                 # MVVM ViewModels
│   ├── AddVideoIdeaViewModel.cs
│   ├── VideoPromptsViewModel.cs
│   ├── VideoDisplayViewModel.cs
│   └── SettingsViewModel.cs
├── Services/                   # Business logic services
│   ├── IChatService.cs         # LLM service interface
│   ├── ChatService.cs          # Azure OpenAI LLM implementation
│   ├── ISoraService.cs         # SORA service interface
│   └── SoraService.cs          # Azure SORA implementation
├── Resources/                  # App resources
│   ├── Images/                 # Image assets
│   ├── Fonts/                  # Custom fonts
│   └── Styles/                 # XAML styles
└── Platforms/                  # Platform-specific code
    ├── Android/
    ├── iOS/
    ├── MacCatalyst/
    └── Windows/
```

## 🔧 Configuration Details

### Azure OpenAI Setup
The app requires two main Azure OpenAI services:

1. **Language Model Service** (for prompt enhancement)
   - Deployment: GPT-4 or similar
   - Used for improving user ideas into viral video prompts

2. **SORA Service** (for video generation)
   - Deployment: SORA model
   - Used for generating actual videos from prompts

### Supported SORA Resolutions
- **(480, 480)** - Square 480p
- **(854, 480)** - Horizontal 480p
- **(720, 720)** - Square 720p
- **(1280, 720)** - Horizontal 720p (HD)
- **(1080, 1080)** - Square 1080p
- **(1920, 1080)** - Horizontal 1080p (Full HD)

## 🎨 UI/UX Features

### Modern Design
- Material Design principles
- Consistent visual hierarchy
- Responsive layouts for all screen sizes
- Smooth animations and transitions

### Loading States
- **AddVideoIdeaPage**: Enhanced prompt generation loader
- **VideoPromptsPage**: Fullscreen blocking loader for video generation
- **VideoDisplayPage**: Media loading indicators
- **SettingsPage**: Configuration saving feedback

### Error Handling
- Network connectivity issues
- API service unavailability
- Invalid configurations
- Video generation failures

## 📊 Performance Optimizations

### Efficient Architecture
- MVVM pattern with proper separation of concerns
- Dependency injection for loose coupling
- Async/await patterns for responsive UI
- Proper memory management

### Resource Management
- Automatic video file organization
- Efficient media element usage
- Background task handling
- Battery optimization considerations

## 🔒 Security Features

### Credential Management
- Secure storage using device preferences
- No hardcoded API keys
- Runtime configuration validation
- Secure HTTP communications

### Privacy
- Local video storage
- No unauthorized data collection
- User-controlled data sharing

## 🐛 Troubleshooting

### Common Issues

**"Service Not Configured" Error**
- Ensure all Azure OpenAI credentials are entered in Settings
- Verify endpoint URLs are correct
- Check API key validity

**"Resolution Not Supported" Error**
- App automatically maps to supported SORA resolutions
- Use provided resolution options in configuration

**Video Generation Timeout**
- SORA generation can take several minutes
- Ensure stable internet connection
- Check Azure service status

**App Crashes on Video Playback**
- Verify MediaElement dependencies
- Check video file integrity
- Ensure sufficient device storage

### Debug Logging
The app includes comprehensive logging for troubleshooting:
- Service initialization logs
- API call tracking
- Error state logging
- Performance monitoring

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- **Azure OpenAI Team** - For providing powerful AI services
- **Microsoft MAUI Team** - For the excellent cross-platform framework
- **CommunityToolkit** - For essential MAUI extensions
- **MVVM Toolkit** - For modern MVVM implementation

## 📞 Support

For support, questions, or feature requests:
- Create an issue in this repository
- Contact the development team
- Check the documentation files in the project

---

## 📈 Roadmap

### Upcoming Features
- [ ] Video sharing capabilities
- [ ] Advanced prompt templates
- [ ] Batch video generation
- [ ] Cloud storage integration
- [ ] Social media export
- [ ] Analytics and insights

### Recent Updates
- ✅ Fullscreen blocking loaders
- ✅ Enhanced prompt generation experience
- ✅ SORA resolution compatibility fixes
- ✅ Comprehensive loading indicators
- ✅ Video configuration controls

---

**Built with ❤️ using .NET MAUI and Azure OpenAI**
