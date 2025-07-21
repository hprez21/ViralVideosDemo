using ViralVideosDemo.ViewModels;

namespace ViralVideosDemo.Pages;

public partial class VideoPromptsPage : ContentPage
{
    public VideoPromptsPage(VideoPromptsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}