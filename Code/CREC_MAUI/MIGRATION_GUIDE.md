# CREC MAUI Migration - Next Steps

This document outlines the remaining work needed to complete the MAUI migration.

## Completed Work

The basic MAUI project structure has been created with:
- ✅ Business logic classes migrated (Models folder)
- ✅ Windows Forms dependencies removed
- ✅ MVVM architecture implemented
- ✅ Basic UI pages created
- ✅ Dependency injection setup
- ✅ Cross-platform project configuration

## High Priority - Required for Basic Functionality

### 1. Platform-Specific File/Folder Pickers

The original Windows Forms application uses `OpenFileDialog` and `FolderBrowserDialog`. These need platform-specific implementations.

**Windows (WinUI):**
```csharp
// In Platforms/Windows/Services/PlatformService.cs
var picker = new Windows.Storage.Pickers.FileOpenPicker();
picker.FileTypeFilter.Add(".crec");
var file = await picker.PickSingleFileAsync();
```

**Android:**
```csharp
// Use Android file picker or Microsoft.Maui.Authentication for file access
var result = await FilePicker.PickAsync();
```

### 2. Alert/Dialog Services

Replace the stubbed alert methods in `PlatformService` with proper implementations:

**Cross-Platform:**
```csharp
public async Task ShowAlertAsync(string title, string message)
{
    await Application.Current.MainPage.DisplayAlert(title, message, "OK");
}

public async Task<bool> ShowConfirmAsync(string title, string message)
{
    return await Application.Current.MainPage.DisplayAlert(title, message, "Yes", "No");
}
```

### 3. Image Processing

The original `MakeThumbnailPicture` method needs to be rewritten using Microsoft.Maui.Graphics:

```xml
<!-- Add to CREC_MAUI.csproj -->
<PackageReference Include="Microsoft.Maui.Graphics" Version="9.0.5" />
<PackageReference Include="Microsoft.Maui.Graphics.Skia" Version="9.0.5" />
```

```csharp
// New implementation using Microsoft.Maui.Graphics
public static async Task<bool> MakeThumbnailPictureAsync(string collectionFolderPath)
{
    var platformService = ServiceHelper.GetService<IPlatformService>();
    var imageFile = await platformService.PickFileAsync(new[] { ".jpg", ".jpeg", ".png", ".gif" });
    
    if (imageFile != null)
    {
        using var stream = File.OpenRead(imageFile);
        using var image = PlatformImage.FromStream(stream);
        var resized = image.Resize(400, 400, ResizeMode.Max);
        var outputPath = Path.Combine(collectionFolderPath, "thumbnail.jpg");
        await resized.SaveAsync(outputPath);
        return true;
    }
    return false;
}
```

## Medium Priority - Enhanced Functionality

### 4. Enhanced UI for Touch/Mobile

- Add mobile-friendly layouts
- Implement swipe gestures for navigation
- Add responsive design for different screen sizes
- Consider using CollectionView with pull-to-refresh

### 5. Platform-Specific Optimizations

**Windows:**
- File association for .crec files
- Jump lists for recent projects
- Integration with Windows 11 context menus

**Mobile:**
- Share functionality
- Camera integration for thumbnail capture
- Offline/cloud sync options

### 6. Data Migration Tool

Create a utility to help users migrate from Windows Forms to MAUI version:
- Verify .crec file compatibility
- Update file paths for cross-platform compatibility
- Convert absolute paths to relative paths where possible

## Low Priority - Nice to Have

### 7. Advanced Features

- Plugin system adaptation for MAUI
- Backup encryption
- Multi-language font support for mobile devices
- Dark mode support
- Accessibility improvements

### 8. Testing

- Unit tests for business logic
- UI tests for critical workflows
- Platform-specific testing

## Implementation Guide

### Building the Project

1. Install MAUI workload:
   ```bash
   dotnet workload install maui
   ```

2. Build for specific platforms:
   ```bash
   dotnet build -f net9.0-windows10.0.19041.0  # Windows
   dotnet build -f net9.0-android              # Android
   dotnet build -f net9.0-ios                  # iOS (requires Mac)
   ```

### Testing Strategy

1. Start with Windows platform (closest to original)
2. Test basic project creation and data loading
3. Verify data compatibility with existing .crec files
4. Test on mobile platforms with simplified UI

### Estimated Timeline

- **Platform Services Implementation**: 1-2 weeks
- **Image Processing**: 1 week
- **UI Polish and Mobile Optimization**: 2-3 weeks
- **Testing and Bug Fixes**: 1-2 weeks

Total estimated effort: 5-8 weeks for a complete migration.

## Key Files to Modify

1. `Services/PlatformService.cs` - Core platform functionality
2. `ViewModels/MainViewModel.cs` - Add platform service usage
3. `Views/MainPage.xaml` - Enhance UI for mobile
4. `Models/CollectionDataClass.cs` - Complete image processing migration
5. Platform-specific folders for native implementations

This migration provides a solid foundation for a cross-platform CREC application while maintaining compatibility with existing data files.