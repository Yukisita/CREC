# Project Settings Backup Flow Diagram

## Overview
This document describes the flow of the project settings file backup functionality.

## Backup Trigger Points

### 1. After SaveProjectSetting (Always)
```
User Action: Modify & Save Project Settings
    ↓
ProjectSettingClass.SaveProjectSetting()
    ↓
Write settings to .crec file
    ↓
Success?
    ├─ Yes → CollectionDataClass.BackupProjectSettingFile()
    │           ↓
    │       Create backup folder if needed
    │           ↓
    │       Copy .crec file with timestamp
    │           ↓
    │       ManageProjectSettingBackupCount()
    │           ↓
    │       Delete old backups if count > MaxBackupCount
    │
    └─ No → Return false (no backup created)
```

### 2. During Collection Backup (If No Backup Exists)
```
User Action: Start Collection Backup
    ↓
CollectionDataClass.BackupProjectData()
    ↓
Check: Does $ProjectSettingFileBackup folder exist?
    ├─ Yes → Check: Any .crec files inside?
    │           ├─ Yes → Skip project settings backup
    │           └─ No → Call BackupProjectSettingFile()
    │
    └─ No → Call BackupProjectSettingFile()
    ↓
Continue with collection backups...
```

## Folder Structure

```
ProjectBackupFolderPath/
├── !BackupLog/                          # Backup logs (existing)
│   └── BackupLog_20250117-143025123.txt
│
├── $ProjectSettingFileBackup/           # NEW: Project settings backups
│   ├── 20250117_143025123_MyProject.crec
│   ├── 20250117_145030456_MyProject.crec
│   └── 20250117_150512789_MyProject.crec
│
├── UUID-COLLECTION-1/                   # Collection backups (existing)
│   ├── 20250117_143025123.zip
│   └── 20250117_145030456.zip
│
└── UUID-COLLECTION-2/                   # Collection backups (existing)
    ├── 20250117_143025123.zip
    └── 20250117_145030456.zip
```

## Backup Count Management

```
ManageProjectSettingBackupCount() called
    ↓
Get all .crec files in $ProjectSettingFileBackup/
    ↓
Sort by CreationTime (oldest first)
    ↓
Count > MaxBackupCount?
    ├─ No → Done
    │
    └─ Yes → Delete oldest backup
                ↓
             Remove from list
                ↓
             Repeat until count <= MaxBackupCount
```

## Cleanup Operations

```
CleanupDeletedCollectionBackupFolders() called
    ↓
Get all folders in ProjectBackupFolderPath
    ↓
Exclude these folders:
    - !BackupLog
    - $ProjectSettingFileBackup    # NEW: Also excluded
    ↓
For each remaining folder:
    ↓
Check if collection UUID exists in project
    ├─ Yes → Keep folder
    └─ No → Delete folder
```

## Error Handling Flow

```
Any Backup Operation
    ↓
Try to perform backup
    ↓
Exception caught?
    ├─ Yes → Log error to Debug output
    │           ↓
    │       Return false
    │           ↓
    │       Application continues normally
    │           ↓
    │       No user notification (silent failure)
    │
    └─ No → Return true
```

## Key Characteristics

1. **Non-Blocking**: Backup operations are fast (< 100ms typically) and don't block the UI
2. **Silent Failures**: Backup errors don't interrupt user workflow
3. **Automatic**: No user intervention required
4. **Consistent**: Uses same MaxBackupCount as collection backups
5. **Isolated**: Each project has its own backup folder

## Integration Points

### SaveProjectSetting
- **Location**: `ProjectSettingClass.cs`
- **Trigger**: Every successful save
- **Behavior**: Always creates backup

### BackupProjectData
- **Location**: `CollectionDataClass.cs`
- **Trigger**: Manual or automatic collection backup
- **Behavior**: Creates backup only if none exists

### CleanupDeletedCollectionBackupFolders
- **Location**: `CollectionDataClass.cs`
- **Modification**: Excludes `$ProjectSettingFileBackup` from deletion

## Backup Naming Convention

Format: `yyyyMMdd_HHmmssfff_<original_filename>`

Components:
- `yyyy`: 4-digit year (e.g., 2025)
- `MM`: 2-digit month (01-12)
- `dd`: 2-digit day (01-31)
- `HH`: 2-digit hour (00-23)
- `mm`: 2-digit minute (00-59)
- `ss`: 2-digit second (00-59)
- `fff`: 3-digit millisecond (000-999)
- `<original_filename>`: Original .crec filename

Example: `20250117_143025123_MyProject.crec`
- Created: January 17, 2025 at 14:30:25.123
- Original file: MyProject.crec

## Concurrency Considerations

1. **Thread-Safe**: Backup operations during collection backup are thread-safe
2. **Sequential**: SaveProjectSetting backup is sequential (not parallel)
3. **Atomic**: File operations use atomic copy (File.Copy with overwrite)
4. **No Locks**: No explicit file locking (relies on OS file system)

## Performance Characteristics

- **Backup Time**: < 100ms for typical .crec files (few KB)
- **Disk Space**: Minimal (MaxBackupCount × ~10KB per backup)
- **Memory Usage**: Negligible (file operations only)
- **CPU Usage**: Minimal (file copy operations)

## Limitations

1. **No Compression**: Project settings backups are not compressed (unlike collection backups)
2. **No Restore UI**: Users must manually restore from backup folder
3. **No Differential**: Full copy created each time (no incremental backups)
4. **Silent Errors**: Backup failures are not shown to users
