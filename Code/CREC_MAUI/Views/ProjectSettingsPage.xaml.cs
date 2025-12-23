/*
ProjectSettingsPage
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using CREC.ViewModels;

namespace CREC.Views;

public partial class ProjectSettingsPage : ContentPage
{
    public ProjectSettingsPage(ProjectSettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (BindingContext is ProjectSettingsViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }
}