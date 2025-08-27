# CREC - Inventory Management System

CREC is a Japanese/English bilingual inventory and data management system built with C# Windows Forms and .NET Framework 4.8. It provides comprehensive features for managing physical items and digital data with search, tagging, backup, and multi-terminal access capabilities.

**ALWAYS reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.**

## Critical Limitations

**WINDOWS-ONLY APPLICATION**: CREC is designed exclusively for Windows environments and CANNOT be built or run on Linux/macOS. Do not attempt to build this project on non-Windows systems.

- Requires Windows with Visual Studio 2017 or newer
- Uses .NET Framework 4.8 (not .NET Core/.NET 5+)
- Contains Windows-specific APIs and Windows Forms GUI components
- Build attempts on Linux/macOS will fail due to missing Windows assemblies and C# language features

## Working Effectively

### Prerequisites
- Windows 10/11 or Windows Server 2016+
- Visual Studio 2019 or newer (Community Edition sufficient)
- .NET Framework 4.8 Developer Pack
- Git for Windows

### Building the Application
- **NEVER CANCEL**: Build takes 2-5 minutes. Set timeout to 10+ minutes.
- Open `Code/CREC.sln` in Visual Studio
- Build → Build Solution (or Ctrl+Shift+B)
- Output: `Code/CoRectSys/bin/Debug/CREC.exe` (Debug) or `Code/CoRectSys/bin/Release/CREC.exe` (Release)

### Command Line Build (if needed)
```bash
# From repository root
cd Code
msbuild CREC.sln /p:Configuration=Release
```
- **NEVER CANCEL**: Command line build takes 3-7 minutes. Set timeout to 15+ minutes.

### Running the Application
- Double-click the generated CREC.exe file
- Or run from Visual Studio (F5 for debug, Ctrl+F5 for release)
- First launch will show a project creation dialog

## Validation

**CRITICAL**: Always manually validate changes by running complete user scenarios after making modifications.

### Required Validation Scenarios
After making any code changes, ALWAYS perform these validation steps:

1. **Application Startup**:
   - Launch CREC.exe
   - Verify splash screen appears and disappears
   - Confirm main window opens without errors

2. **Project Management**:
   - Create a new project: File → New Project
   - Set project name, data location, and backup location
   - Verify project creates successfully

3. **Data Management**:
   - Add a new collection item: Right-click list → Add New Collection
   - Fill in required fields (ID, Name, Category)
   - Add optional tags and comments
   - Save the item and verify it appears in the list

4. **Search Functionality**:
   - Use the search box to find the created item
   - Test different search methods (partial match, exact match)
   - Verify search results are accurate

5. **Backup System**:
   - Go to File → Backup → Start Backup
   - Verify backup completes without errors
   - Check that backup files are created in the backup location

### UI Testing
- Test window resizing and DPI scaling (100%, 125%, 150%)
- Verify all buttons and menus are functional
- Check that Japanese/English language switching works
- Test inventory management mode if applicable

## Common Tasks

### Language Files
The application supports Japanese and English:
- `Code/CoRectSys/language/Japanese.xml` - Japanese translations
- `Code/CoRectSys/language/English.xml` - English translations

### Key Project Structure
```
Code/
├── CREC.sln                    # Main Visual Studio solution
└── CoRectSys/
    ├── CREC.csproj            # Main project file
    ├── MainForm.cs            # Primary application window
    ├── Program.cs             # Application entry point
    ├── CollectionDataClass.cs # Core data management
    ├── language/              # Localization files
    └── Properties/
        └── AssemblyInfo.cs    # Version info (currently 9.4.0.0)
```

### Configuration Files
- `App.config` - Application configuration including DPI awareness
- `app.manifest` - Windows manifest for compatibility and permissions

### Build Configurations
- **Debug**: Includes debug symbols, larger executable
- **Release**: Optimized for distribution, smaller executable

## Testing Infrastructure

**NO AUTOMATED TESTS**: This project currently has no unit tests, integration tests, or automated testing infrastructure.

When making changes:
- Manual testing is REQUIRED for all modifications
- Focus on the validation scenarios listed above
- Test on different Windows versions if possible
- Verify both Japanese and English language modes

## Development Guidelines

### Code Modifications
- The application uses modern C# features (tuples, pattern matching)
- Main business logic is in `CollectionDataClass.cs` and `MainForm.cs`
- UI definitions are in `.Designer.cs` files (auto-generated)
- Resource files (`.resx`) contain UI layout and embedded resources

### Database/Storage
- Uses file-based storage (no external database required)
- Project files use `.crec` extension
- Data files stored in user-specified project folders
- Backup system creates compressed archives

### Known Working Environment
- Windows 10/11 with Visual Studio 2019/2022
- .NET Framework 4.8
- Japanese and English locales tested
- Supports high-DPI displays

## Troubleshooting

### Build Errors
- Ensure .NET Framework 4.8 Developer Pack is installed
- Verify Windows SDK is available
- Check that all NuGet packages are restored

### Runtime Issues
- Verify project folder permissions are correct
- Check that required fonts are available for Japanese text
- Ensure backup folder location is writable

### DPI/Scaling Issues
- Application includes DPI awareness settings
- Test on multiple display scales
- UI layouts may need adjustment for extreme scaling (>200%)

## Version Information
- Current version: 9.4.0.0 (check `Properties/AssemblyInfo.cs`)
- Released under MIT License
- Developed by S.Yukisita (2022-2025)
- Wiki documentation: https://github.com/Yukisita/CREC/wiki

## Additional Resources
- [Installation Guide](https://github.com/Yukisita/CREC/wiki/2.-%E3%82%A4%E3%83%B3%E3%82%B9%E3%83%88%E3%83%BC%E3%83%AB%E3%83%BB%E3%82%A2%E3%83%B3%E3%82%A4%E3%83%B3%E3%82%B9%E3%83%88%E3%83%BC%E3%83%AB#%E3%82%A4%E3%83%B3%E3%82%B9%E3%83%88%E3%83%BC%E3%83%AB%E6%96%B9%E6%B3%95)
- [Usage Guide](https://github.com/Yukisita/CREC/wiki)
- Update history in `UpdateHistory.txt`