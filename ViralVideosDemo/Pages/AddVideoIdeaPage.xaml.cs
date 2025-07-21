using Microsoft.Maui.Controls;
using ViralVideosDemo.ViewModels;

namespace ViralVideosDemo.Pages;

public partial class AddVideoIdeaPage : ContentPage
{
    private readonly AddVideoIdeaViewModel _viewModel;

    public AddVideoIdeaPage()
    {
        InitializeComponent();
        
        // Initialize ViewModel and set as BindingContext
        _viewModel = new AddVideoIdeaViewModel();
        BindingContext = _viewModel;
        
        // Set the page reference in the ViewModel for alerts
        _viewModel.SetPage(this);
    }
}
