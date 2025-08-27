# CREC - Inventory Management System

CREC is a Japanese/English bilingual inventory and data management system built with C# Windows Forms and .NET Framework 4.8. It provides comprehensive features for managing physical items and digital data with search, tagging, backup, and multi-terminal access capabilities.

**ALWAYS reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.**

## Critical Limitations

**WINDOWS-ONLY APPLICATION**: CREC is designed exclusively for Windows environments and CANNOT be built or run on Linux/macOS. Do not attempt to build this project on non-Windows systems.

- Requires Windows with Visual Studio 2017 or newer
- Uses .NET Framework 4.8 (not .NET Core/.NET 5+)
- Contains Windows-specific APIs and Windows Forms GUI components
- Uses Microsoft.VisualBasic.FileIO for enhanced file operations
- References PresentationFramework for some UI components
- Build attempts on Linux/macOS will fail due to missing Windows assemblies and C# language features

### Key Dependencies
- System.Windows.Forms (Windows GUI framework)
- System.IO.Compression (backup compression)
- System.Management (system information gathering)
- Microsoft.VisualBasic.FileIO (enhanced file operations)
- System.Drawing (image processing and thumbnails)

## Working Effectively

### Prerequisites
- Windows 10/11 or Windows Server 2016+
- Visual Studio 2019 or newer (Community Edition sufficient)
- .NET Framework 4.8 Developer Pack
- Git for Windows

### Building the Application
- **NEVER CANCEL**: Build takes 2-5 minutes depending on hardware. Set timeout to 10+ minutes.
- Open `Code/CREC.sln` in Visual Studio
- Build → Build Solution (or Ctrl+Shift+B)
- Output: `Code/CoRectSys/bin/Debug/CREC.exe` (Debug) or `Code/CoRectSys/bin/Release/CREC.exe` (Release)

### Command Line Build (if needed)
```bash
# From repository root
cd Code
msbuild CREC.sln /p:Configuration=Release /p:Platform="Any CPU"
```
- **NEVER CANCEL**: Command line build takes 3-7 minutes. Set timeout to 15+ minutes.
- Requires MSBuild tools or Visual Studio Build Tools

### Running the Application
- Double-click the generated CREC.exe file
- Or run from Visual Studio (F5 for debug, Ctrl+F5 for release)
- First launch will show a project creation dialog in Japanese or English
- Application requires Windows desktop environment (cannot run headless)

### Development Workflow Timing
- **Full rebuild**: 5-10 minutes (NEVER CANCEL - set 20+ minute timeout)
- **Incremental build**: 30 seconds - 2 minutes
- **Application startup**: 3-10 seconds on modern hardware
- **Project loading**: 1-30 seconds depending on project size
- **Backup operation**: 2-15 minutes for typical projects (NEVER CANCEL - set 30+ minute timeout)

## Validation

**CRITICAL**: Always manually validate changes by running complete user scenarios after making modifications.

### Required Validation Scenarios
After making any code changes, ALWAYS perform these validation steps:

1. **Application Startup**:
   - Launch CREC.exe
   - Verify splash screen appears and disappears correctly
   - Confirm main window opens without errors
   - Check that language settings are preserved from last session

2. **Project Management**:
   - Create a new project: File → New Project
   - Set project name, data location, and backup location
   - Verify project creates successfully with proper folder structure
   - Test opening existing projects from recent projects list

3. **Data Management**:
   - Add a new collection item: Right-click list → Add New Collection
   - Fill in required fields (ID, Name, Category)
   - Add optional tags (Tag1, Tag2, Tag3) and comments
   - Add thumbnail image if available
   - Save the item and verify it appears in the list with correct information

4. **Search Functionality**:
   - Use the search box to find the created item
   - Test different search methods: partial match, exact match, start match
   - Try "All fields" search option
   - Test tag-based searching
   - Verify search results are accurate and complete

5. **Backup System**:
   - Go to File → Backup → Start Backup
   - **NEVER CANCEL**: Backup can take 5-15 minutes for large projects. Set timeout to 30+ minutes.
   - Verify backup completes without errors
   - Check that backup files are created in the backup location
   - Test backup restoration functionality

6. **Inventory Management** (if applicable):
   - Switch to inventory mode if project supports it
   - Test stock quantity tracking
   - Verify inventory alerts and status indicators
   - Test stock in/out operations

7. **Multi-language Support**:
   - Switch between Japanese and English: Settings → Language
   - Verify all UI elements display correctly in both languages
   - Test that language preference is saved

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

When modifying UI text:
1. Add new entries to both language files
2. Use `LanguageSettingClass.GetMessageBoxMessage()` or `LanguageSettingClass.GetOtherMessage()` to retrieve text
3. Test both language modes after changes

### Key Project Structure
```
Code/
├── CREC.sln                    # Main Visual Studio solution
└── CoRectSys/
    ├── CREC.csproj            # Main project file (.NET Framework 4.8)
    ├── MainForm.cs            # Primary UI logic (4,961 lines)
    ├── MainForm.Designer.cs   # Auto-generated UI layout
    ├── Program.cs             # Application entry point
    ├── CollectionDataClass.cs # Core data management (1,900+ lines)
    ├── ProjectSettingClass.cs # Project configuration
    ├── LanguageSettingClass.cs # Internationalization
    ├── ConfigValuesClass.cs   # App-wide settings
    ├── CREC.ico              # Application icon
    ├── app.manifest          # Windows compatibility settings
    ├── language/             # Localization files
    │   ├── Japanese.xml
    │   └── English.xml
    ├── Properties/
    │   └── AssemblyInfo.cs   # Version info (currently 9.4.0.0)
    └── Resources/            # Embedded images and resources
```

### Configuration Files
- `App.config` - Application configuration including DPI awareness
- `app.manifest` - Windows manifest for compatibility and permissions
- Project files (`.crec`) store project-specific settings
- `config.sys` (generated at runtime) stores user preferences

### Build Configurations
- **Debug**: Includes debug symbols, larger executable, easier debugging
- **Release**: Optimized for distribution, smaller executable, better performance

### Common Code Modification Patterns
- **Adding new UI elements**: Modify `.Designer.cs` files (via Visual Studio designer)
- **Adding business logic**: Usually in `MainForm.cs` event handlers or `CollectionDataClass.cs` methods
- **Adding new settings**: Update `ConfigValuesClass.cs` and project setting files
- **Modifying data storage**: Update file I/O methods in `CollectionDataClass.cs`

## Testing Infrastructure

**NO AUTOMATED TESTS**: This project currently has no unit tests, integration tests, or automated testing infrastructure.

When making changes:
- Manual testing is REQUIRED for all modifications
- Focus on the validation scenarios listed above
- Test on different Windows versions if possible
- Verify both Japanese and English language modes

## Development Guidelines

### Code Modifications
- The application uses modern C# features (tuples, pattern matching, LINQ)
- Main business logic is in `CollectionDataClass.cs` (1,900+ lines) and `MainForm.cs` (4,961 lines)
- UI definitions are in `.Designer.cs` files (auto-generated)
- Resource files (`.resx`) contain UI layout and embedded resources
- Total codebase: ~15,000 lines of C# code

### Key Classes and Their Responsibilities
- `MainForm.cs` - Primary UI logic, event handling, user interactions
- `CollectionDataClass.cs` - Core data management, backup, file operations
- `ProjectSettingClass.cs` - Project configuration management
- `LanguageSettingClass.cs` - Internationalization support
- `ConfigValuesClass.cs` - Application-wide configuration
- `CollectionListClass.cs` - List management and display logic

### Database/Storage Architecture
- Uses file-based storage (no external database required)
- Project files use `.crec` extension
- Data files stored in user-specified project folders
- Index files track collections and metadata
- Backup system creates compressed archives (.zip format)
- Supports automatic backup on startup/shutdown/edit events
- Image thumbnails stored as separate compressed files

### Search and Filtering
- Multiple search modes: exact match, partial match, start match
- Full-text search across all visible fields
- Tag-based filtering system
- Real-time search updates (can be disabled for performance)
- Supports Japanese and English search terms

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
- If tuple syntax errors occur, ensure C# 7.0+ language features are enabled

### Runtime Issues
- Verify project folder permissions are correct (read/write access required)
- Check that required fonts are available for Japanese text display
- Ensure backup folder location is writable
- If application crashes on startup, check Windows Event Viewer for .NET errors
- Verify sufficient disk space for backup operations

### Performance Issues
- Large projects (>1000 items) may have slower search performance
- Disable automatic search updates for better responsiveness
- Consider adjusting data monitoring intervals in settings
- Backup operations are CPU-intensive and may appear to hang

### DPI/Scaling Issues
- Application includes DPI awareness settings in app.manifest
- Test on multiple display scales (100%, 125%, 150%, 200%)
- UI layouts may need adjustment for extreme scaling (>200%)
- Some controls may not scale perfectly on mixed-DPI setups

### Common Error Scenarios
- **Index file corruption**: Application can auto-restore from backup
- **Permission denied**: Check folder permissions for project and backup locations
- **Language switching fails**: Verify XML language files are not corrupted
- **Images not displaying**: Check file path length limits and supported formats

## Version Information
- Current version: 9.4.0.0 (check `Properties/AssemblyInfo.cs`)
- Released under MIT License
- Developed by S.Yukisita (2022-2025)
- Wiki documentation: https://github.com/Yukisita/CREC/wiki

## Additional Resources
- [Installation Guide](https://github.com/Yukisita/CREC/wiki/2.-%E3%82%A4%E3%83%B3%E3%82%B9%E3%83%88%E3%83%BC%E3%83%AB%E3%83%BB%E3%82%A2%E3%83%B3%E3%82%A4%E3%83%B3%E3%82%B9%E3%83%88%E3%83%BC%E3%83%AB#%E3%82%A4%E3%83%B3%E3%82%B9%E3%83%88%E3%83%BC%E3%83%AB%E6%96%B9%E6%B3%95)
- [Usage Guide](https://github.com/Yukisita/CREC/wiki)
- Update history in `UpdateHistory.txt`

## Quick Reference Commands

### Frequently Used Code Patterns
```csharp
// Language-aware message display
MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("MessageKey", "FormName", LanguageFile), "CREC");

// Collection data access
foreach (CollectionDataValuesClass collection in allCollectionList)
{
    // Process collection data
}

// Backup operations with logging
List<(string id, bool isSuccess)> backupLog = CollectionDataClass.AllCollectionBackUp(...);

// Safe file operations
using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.GetEncoding("UTF-8")))
{
    // File writing operations
}
```

### Common File Locations During Development
- Generated executable: `Code/CoRectSys/bin/Debug/CREC.exe`
- Project settings: User-specified `.crec` files
- Language files: `Code/CoRectSys/language/*.xml`
- Application settings: `%USERPROFILE%/config.sys` (runtime generated)
- Recent projects: `%USERPROFILE%/RecentlyOpenedProjectList.log`

### Expected Behavior Notes
- Application auto-saves user preferences and window positions
- Multi-terminal access requires file locking coordination
- Image thumbnails are automatically generated and compressed
- Search performance degrades with projects >1000 items
- Backup operations create timestamped compressed archives
- Application supports command-line project file opening