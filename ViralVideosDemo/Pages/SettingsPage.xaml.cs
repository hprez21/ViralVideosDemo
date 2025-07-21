using Microsoft.Maui.Controls;
using ViralVideosDemo.ViewModels;

namespace ViralVideosDemo.Pages;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _viewModel;

    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
        
        // Set the page reference in the ViewModel for alerts
        _viewModel.SetPage(this);
    }

    /// <summary>
    /// Called when the page appears - reload settings to ensure fresh data
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.LoadSettings();
    }
}
