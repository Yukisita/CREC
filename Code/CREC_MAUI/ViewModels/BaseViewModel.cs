/*
Base ViewModel
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using CommunityToolkit.Mvvm.ComponentModel;

namespace CREC.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _title = string.Empty;

    public virtual async Task InitializeAsync()
    {
        await Task.CompletedTask;
    }
}