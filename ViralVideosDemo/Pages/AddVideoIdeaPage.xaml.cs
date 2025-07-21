using Microsoft.Maui.Controls;
using ViralVideosDemo.ViewModels;

namespace ViralVideosDemo.Pages;

public partial class AddVideoIdeaPage : ContentPage
{
    private readonly AddVideoIdeaViewModel _viewModel;

    public AddVideoIdeaPage(AddVideoIdeaViewModel viewModel)
    {
        InitializeComponent();
        
        // Set the injected ViewModel as BindingContext
        _viewModel = viewModel;
        BindingContext = _viewModel;
        
        // Set the page reference in the ViewModel for alerts
        _viewModel.SetPage(this);
    }
}