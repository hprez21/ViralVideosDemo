using ViralVideosDemo.ViewModels;

namespace ViralVideosDemo.Pages;

public partial class VideoDisplayPage : ContentPage
{
    public VideoDisplayPage(VideoDisplayViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        
        // Connect MediaElement to ViewModel
        viewModel.SetMediaElement(VideoPlayer);
    }
}
