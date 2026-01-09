# Testing Guide: Project Settings File (.crec) Backup Feature

## Overview
This guide provides step-by-step instructions to test the new project settings file backup functionality implemented in CREC.

## Feature Requirements
1. Create a folder named `$ProjectSettingFileBackup` within the project backup folder
2. Backup filename format: `yyyyMMdd_HHmmssfff_<original_filename>.crec`
3. Always backup after saving the project settings file
4. When creating a collection backup, if no project settings backup exists, create one
5. Backup count matches `MaxBackupCount` setting (same as collection backups)

## Prerequisites
- Windows 10/11 or Windows Server 2016+
- Visual Studio 2019 or newer
- CREC application built from the updated source code

## Test Scenarios

### Test 1: Verify Backup Folder Creation
**Objective**: Confirm that the `$ProjectSettingFileBackup` folder is created correctly.

**Steps**:
1. Build the CREC solution in Visual Studio
2. Launch CREC.exe
3. Create a new project or open an existing project
4. Go to the project settings and modify any setting
5. Save the project settings
6. Navigate to the project's backup folder location

**Expected Result**:
- A folder named `$ProjectSettingFileBackup` should exist in the backup folder
- The folder should contain at least one `.crec` file

### Test 2: Verify Backup Filename Format
**Objective**: Confirm that backup filenames follow the correct format.

**Steps**:
1. Open an existing project
2. Modify project settings (e.g., change project name, backup settings)
3. Save the project settings
4. Navigate to `<backup_folder>/$ProjectSettingFileBackup/`
5. Check the filename of the newly created backup

**Expected Result**:
- Filename format: `yyyyMMdd_HHmmssfff_<projectname>.crec`
- Example: `20250117_143025123_MyProject.crec`
- Date and time should match the current system time (within a few seconds)

### Test 3: Verify Automatic Backup After Save
**Objective**: Confirm that backup is created automatically after each save.

**Steps**:
1. Open an existing project
2. Note the number of backup files in `$ProjectSettingFileBackup` folder
3. Modify project settings multiple times, saving after each modification:
   - Change collection name label
   - Change backup compression type
   - Change maximum backup count
4. After each save, check the `$ProjectSettingFileBackup` folder

**Expected Result**:
- A new backup file is created after each save operation
- The number of backup files should increase by 1 after each save
- Each backup file should have a unique timestamp

### Test 4: Verify Backup During Collection Backup (No Existing Backup)
**Objective**: Confirm that project settings are backed up when creating collection backup if no backup exists.

**Steps**:
1. Create a new project with at least one collection
2. Manually delete the `$ProjectSettingFileBackup` folder (if it exists)
3. Go to File → Backup → Start Backup (or use automatic backup)
4. Wait for backup to complete
5. Check the backup folder

**Expected Result**:
- `$ProjectSettingFileBackup` folder is created
- At least one project settings backup file exists in the folder
- The backup file timestamp should match the collection backup time

### Test 5: Verify Backup During Collection Backup (Existing Backup)
**Objective**: Confirm that existing project settings backup is not overwritten unnecessarily.

**Steps**:
1. Open an existing project that already has project settings backups
2. Note the number of backup files in `$ProjectSettingFileBackup`
3. Trigger a collection backup without modifying project settings
4. Check the `$ProjectSettingFileBackup` folder again

**Expected Result**:
- Number of project settings backup files should remain the same
- No new backup file is created (since backup already exists)

### Test 6: Verify Backup Count Management
**Objective**: Confirm that old backups are deleted when MaxBackupCount is exceeded.

**Steps**:
1. Open an existing project
2. Set MaxBackupCount to a low value (e.g., 3) in project settings
3. Save project settings
4. Modify and save project settings at least 5 more times
5. Check the `$ProjectSettingFileBackup` folder

**Expected Result**:
- Only 3 backup files exist (matching MaxBackupCount)
- The 3 newest backup files are retained
- Oldest backup files are automatically deleted
- Backup files should be sorted by creation time

### Test 7: Verify Cleanup Operations Exclusion
**Objective**: Confirm that project settings backup folder is not deleted during cleanup.

**Steps**:
1. Open an existing project with backups
2. Delete a collection from the project
3. Go to File → Cleanup Backup Data
4. Execute the cleanup operation
5. Check the backup folder

**Expected Result**:
- `$ProjectSettingFileBackup` folder still exists
- Project settings backup files are not deleted
- Only deleted collection backup folders are removed
- `!BackupLog` folder also remains intact

### Test 8: Test MaxBackupCount Change
**Objective**: Verify that changing MaxBackupCount affects project settings backups.

**Steps**:
1. Open project with MaxBackupCount = 10 and 10 existing backups
2. Change MaxBackupCount to 5
3. Save project settings
4. Check `$ProjectSettingFileBackup` folder

**Expected Result**:
- After save, folder should contain 5 backup files (the newest 5)
- Oldest 5 backup files should be deleted
- New backup from this save is included in the count

### Test 9: Test Error Handling
**Objective**: Verify graceful error handling when backup fails.

**Steps**:
1. Open an existing project
2. Make the backup folder read-only (remove write permissions)
3. Modify and save project settings
4. Check application behavior

**Expected Result**:
- Application should not crash
- Project settings save should succeed
- Backup failure should be logged but not shown to user (silent failure)
- Application should continue to function normally

### Test 10: Test Multiple Projects
**Objective**: Verify that backups are properly isolated per project.

**Steps**:
1. Create or open Project A
2. Save project settings multiple times
3. Note the backup files in Project A's `$ProjectSettingFileBackup` folder
4. Close Project A
5. Create or open Project B (different backup location)
6. Save project settings multiple times
7. Check both projects' backup folders

**Expected Result**:
- Each project has its own `$ProjectSettingFileBackup` folder
- Project A's backups are in its backup folder
- Project B's backups are in its backup folder
- No cross-contamination between projects

## Verification Checklist

Use this checklist to track test completion:

- [ ] Test 1: Backup folder creation
- [ ] Test 2: Backup filename format
- [ ] Test 3: Automatic backup after save
- [ ] Test 4: Backup during collection backup (no existing)
- [ ] Test 5: Backup during collection backup (existing)
- [ ] Test 6: Backup count management
- [ ] Test 7: Cleanup operations exclusion
- [ ] Test 8: MaxBackupCount change
- [ ] Test 9: Error handling
- [ ] Test 10: Multiple projects

## Common Issues and Solutions

### Issue: Backup folder not created
**Solution**: Check that ProjectBackupFolderPath is set in project settings

### Issue: Too many backup files
**Solution**: Check MaxBackupCount setting value

### Issue: Backup files with invalid timestamps
**Solution**: Verify system date/time settings

### Issue: Permission denied errors
**Solution**: Ensure backup folder has write permissions

## Notes for Developers

1. **Silent Failures**: Project settings backup failures are logged but not shown to users to avoid interrupting workflow
2. **Thread Safety**: Backup operations during collection backup are thread-safe
3. **Performance**: Backup is synchronous but should complete quickly (< 100ms for typical .crec files)
4. **Backup Timing**: Backup occurs after successful save, before SaveProjectSetting returns

## Code Review Points

Key implementation files:
- `Code/CREC/CollectionDataClass.cs`: Backup methods
- `Code/CREC/ProjectSettingClass.cs`: Save integration

Key methods:
- `BackupProjectSettingFile()`: Main backup logic
- `ManageProjectSettingBackupCount()`: Backup count management
- `SaveProjectSetting()`: Integration point for automatic backup

## Regression Testing

Ensure existing functionality still works:
- [ ] Collection backups work as before
- [ ] Project settings save/load correctly
- [ ] MaxBackupCount still applies to collection backups
- [ ] Cleanup operations work correctly
- [ ] Backup log functionality unchanged
