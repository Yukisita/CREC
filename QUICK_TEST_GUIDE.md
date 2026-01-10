# Quick Testing Guide - Index JSON Migration

## Prerequisites
- Windows 10/11
- Visual Studio 2019 or newer
- .NET Framework 4.8 Developer Pack

## Quick Start

### 1. Build the Project
```
1. Open Code/CREC.sln in Visual Studio
2. Build → Build Solution (Ctrl+Shift+B)
3. Wait 5-10 minutes for build to complete
4. Output: Code/CoRectSys/bin/Debug/CREC.exe
```

### 2. Test New Collection (JSON Format)
```
1. Run CREC.exe
2. Create or open a project
3. Right-click list → Add New Collection
4. Fill in collection details
5. Save
6. Verify: {CollectionFolder}\SystemData\index.json exists
7. Open index.json and verify format matches specification
```

Expected JSON format:
```json
{
	"systemData": {
		"id": "...",
		"systemCreateDate": "2024-11-08T18:13:37.0000000+00:00"
	},
	"values": {
		"name": "...",
		"managementCode": "...",
		...
	}
}
```

### 3. Test Migration (TXT → JSON)
```
1. Manually create an index.txt file in a collection folder:
   {CollectionFolder}\index.txt
   
   Content:
   名称,TestName
   ID,TestID
   MC,TestMC
   登録日,2024/01/01
   カテゴリ,TestCategory
   タグ1,Tag1
   タグ2,Tag2
   タグ3,Tag3
   場所1(Real),TestLocation

2. Run CREC.exe and open the collection
3. Dialog appears: "index.txtファイルが見つかりました..."
4. Click OK
5. Dialog asks: "変換後、元のindex.txtファイルを削除しますか？"
6. Click Yes or No
7. Success message: "index.jsonへの変換に成功しました。"
8. Verify: {CollectionFolder}\SystemData\index.json exists
9. Verify: All data migrated correctly
10. If Yes: index.txt deleted
    If No: index.txt still exists
```

### 4. Test Data Preservation
```
1. Open an existing collection (with index.json)
2. Edit some fields
3. Save
4. Close and reopen CREC
5. Open the same collection
6. Verify: All changes preserved
7. Verify: systemCreateDate unchanged in JSON
```

### 5. Test Backup & Recovery
```
1. Open a collection
2. Edit and save (triggers backup)
3. Verify: {CollectionFolder}\SystemData\index_old.json created
4. Manually delete index.json
5. Reopen collection
6. Dialog: "Indexファイルが見つかりません..."
7. Dialog: "バックアップデータからの復元に成功しました。"
8. Verify: Data restored correctly
```

## Expected Behavior

### Migration Dialog Flow
1. Detection message (Japanese): "index.txtファイルが見つかりました。JSON形式（index.json）に変換します。"
2. Detection message (English): "Found index.txt file. Converting to JSON format (index.json)."
3. Delete confirmation (Japanese): "変換後、元のindex.txtファイルを削除しますか？"
4. Delete confirmation (English): "Do you want to delete the original index.txt file after conversion?"
5. Success (Japanese): "index.jsonへの変換に成功しました。"
6. Success (English): "Successfully converted to index.json."

### File Structure
```
CollectionFolder/
├── SystemData/
│   ├── index.json          ← New format (primary)
│   ├── index_old.json      ← Backup
│   └── ADD                 ← System tag
├── data/
├── pictures/
├── details.txt
├── confidentialdata.txt
└── index.txt               ← Old format (only if kept during migration)
```

## Verification Checklist

After each test:
- [ ] JSON file created in correct location
- [ ] JSON structure matches specification
- [ ] Date format is ISO8601 with offset
- [ ] All fields mapped correctly
- [ ] No exceptions or errors
- [ ] Data loads correctly after save
- [ ] Language messages display correctly

## Common Issues

### Build Fails
- Ensure .NET Framework 4.8 Developer Pack is installed
- Check Windows SDK is available
- Restore NuGet packages

### Migration Dialog Doesn't Appear
- Verify index.txt exists in collection root
- Verify index.json doesn't already exist in SystemData

### JSON Format Incorrect
- Check the actual file: {CollectionFolder}\SystemData\index.json
- Verify tabs and structure match specification
- Check date format includes offset

## Quick Verification Commands

### Check JSON File
```powershell
# PowerShell
Get-Content "{CollectionFolder}\SystemData\index.json"
```

### Verify Date Format
JSON should contain dates like:
```
"2024-11-08T18:13:37.0000000+00:00"
```
Not like:
```
"2024-11-08T18:13:37Z"  ← Missing .0000000
"2024-11-08 18:13:37"   ← Wrong format
```

## Questions or Issues?

Refer to detailed documentation:
- JSON_FORMAT_EXAMPLE.md - Detailed format and testing guide
- IMPLEMENTATION_SUMMARY.md - Complete implementation details

## Success Criteria

✅ New collections create index.json  
✅ Old collections migrate with user confirmation  
✅ Data preserved across save/load cycles  
✅ Backup and recovery work correctly  
✅ No exceptions or crashes  
✅ Both languages work correctly  
