using ViralVideosDemo.ViewModels;

namespace ViralVideosDemo.Pages;

public partial class VideoPromptsPage : ContentPage
{
    public VideoPromptsPage()
    {
        InitializeComponent();
        BindingContext = new VideoPromptsViewModel();
    }
}