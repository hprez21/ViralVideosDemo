using Microsoft.Extensions.Logging;
using ViralVideosDemo.Services;
using ViralVideosDemo.Pages;
using ViralVideosDemo.ViewModels;

namespace ViralVideosDemo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register services for dependency injection
            builder.Services.AddSingleton<HttpClient>();
            builder.Services.AddSingleton<IChatService, ChatService>();
            
            // Register pages
            builder.Services.AddTransient<AddVideoIdeaPage>();
            builder.Services.AddTransient<VideoPromptsPage>();
            builder.Services.AddTransient<SettingsPage>();

            // Register ViewModels
            builder.Services.AddTransient<AddVideoIdeaViewModel>();
            builder.Services.AddTransient<VideoPromptsViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
