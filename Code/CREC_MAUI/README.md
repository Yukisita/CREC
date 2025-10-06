# CREC MAUI Migration

This folder contains the MAUI version of the CREC inventory management system, migrated from Windows Forms to .NET MAUI for cross-platform compatibility.

## Migration Status

### ✅ Completed
- Basic MAUI project structure with multi-platform targeting (Android, iOS, macOS, Windows)
- Core business logic classes migrated with Windows Forms dependencies removed
- MVVM architecture implemented with CommunityToolkit.Mvvm
- Basic UI pages created with XAML
- Dependency injection setup with services
- Configuration and project management services
- Basic styling and resource structure

### 🚧 In Progress
- File I/O operations adaptation for MAUI
- Image processing functionality (requires different approach for MAUI)
- Platform-specific file/folder dialogs
- Complete data compatibility testing

### ⏳ Planned
- Full feature parity with Windows Forms version
- Platform-specific optimizations
- Mobile-friendly UI adaptations
- Testing on all target platforms

## Key Changes from Windows Forms Version

### Architecture
- **Windows Forms → MAUI**: Multi-platform UI framework
- **Direct UI manipulation → MVVM**: Proper separation of concerns
- **Static classes → Dependency Injection**: Better testability and maintainability
- **MessageBox → Debug output**: Platform-agnostic logging (will be enhanced)

### Dependencies Removed
- `System.Windows.Forms`
- `Microsoft.VisualBasic.FileIO`
- `System.Drawing` (partially - needs MAUI-specific image handling)

### Dependencies Added
- `Microsoft.Maui.Controls`
- `CommunityToolkit.Mvvm`
- `Microsoft.Extensions.Logging.Debug`

## Building and Running

### Prerequisites
- .NET 9 SDK
- MAUI workload installed: `dotnet workload install maui`
- Platform-specific SDKs (Android SDK, iOS SDK, etc.)

### Build Commands
```bash
# Restore packages
dotnet restore

# Build for Windows
dotnet build -f net9.0-windows10.0.19041.0

# Build for Android
dotnet build -f net9.0-android

# Run on Windows
dotnet run -f net9.0-windows10.0.19041.0
```

## Project Structure

```
CREC_MAUI/
├── Models/                   # Business logic (migrated from original)
│   ├── CollectionDataClass.cs
│   ├── ConfigValuesClass.cs
│   ├── ProjectSettingClass.cs
│   └── ...
├── Services/                 # Data and configuration services
│   ├── IConfigurationService.cs
│   ├── IProjectService.cs
│   └── ...
├── ViewModels/              # MVVM view models
│   ├── MainViewModel.cs
│   ├── ProjectSettingsViewModel.cs
│   └── ...
├── Views/                   # XAML pages
│   ├── MainPage.xaml
│   ├── ProjectSettingsPage.xaml
│   └── ...
├── Resources/               # App resources
│   ├── Styles/
│   ├── Images/
│   ├── Fonts/
│   └── Raw/
└── Platforms/               # Platform-specific code
    └── Windows/
```

## Data Compatibility

The MAUI version is designed to maintain compatibility with existing .crec project files and data structures from the Windows Forms version. The core business logic classes have been preserved with minimal changes to ensure data can be shared between versions.

## Known Limitations

1. **Image Processing**: The original image processing code using System.Drawing needs to be replaced with MAUI-compatible image handling.
2. **File Dialogs**: Platform-specific file/folder picker implementations needed.
3. **Some Advanced Features**: Complex UI interactions may need platform-specific implementations.

## Development Notes

- The migration prioritizes maintaining the core functionality while adapting to MAUI patterns
- MessageBox calls have been replaced with Debug output - proper dialog implementation should be added
- FileSystem operations from Microsoft.VisualBasic have been replaced with standard .NET methods
- The project maintains the same copyright and licensing as the original