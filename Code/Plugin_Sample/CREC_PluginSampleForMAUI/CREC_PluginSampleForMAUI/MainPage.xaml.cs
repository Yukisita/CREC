/*
MainForm
Copyright (c) [2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/

namespace CREC_PluginSampleForMAUI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            ShowCurrentDirectoryLabel.Text = "Collection Folder Path (Current Directory)\n"+Environment.CurrentDirectory;
        }

        private void OnOpenCollectionDataFolderButtonClicked(object? sender, EventArgs e)
        {
            // Environment.CurrentDirectoryを開く
            try
            {
                var currentDirectory = Environment.CurrentDirectory;
                if (Directory.Exists(currentDirectory))
                {
                    // Windowsの場合、explorerで開く
                    if (OperatingSystem.IsWindows())
                    {
                        System.Diagnostics.Process.Start("explorer.exe", currentDirectory);
                    }
                    // Macの場合、openで開く
                    else if (OperatingSystem.IsMacOS())
                    {
                        System.Diagnostics.Process.Start("open", currentDirectory);
                    }
                    // Linuxの場合、xdg-openで開く
                    else if (OperatingSystem.IsLinux())
                    {
                        System.Diagnostics.Process.Start("xdg-open", currentDirectory);
                    }
                    else
                    {
                        DisplayAlert("Error", "Unsupported OS", "OK");
                    }
                }
                else
                {
                    DisplayAlert("Error", $"Directory does not exist: {currentDirectory}", "OK");
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Error", ex.Message, "OK");
            }

        }
    }
}
