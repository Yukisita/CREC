# Index File Format - JSON Migration

## Overview
The index file format has been updated from `index.txt` to `index.json` for web application compatibility.

## File Location
- **Old format**: `{CollectionFolder}\index.txt`
- **New format**: `{CollectionFolder}\SystemData\index.json`

## JSON Format

```json
{
	"systemData": {
		"id": "2adf98de-9bbd-47cc-8a2e-47cd194045a3",
		"systemCreateDate": "2024-11-08T18:13:37.0000000+00:00"
	},
	"values": {
		"name": "hoge",
		"managementCode": "hoge",
		"registrationDate": "2024-11-08T18:13:37.0000000+00:00",
		"category": "hoge",
		"firstTag": "hoge",
		"secondTag": "hoge",
		"thirdTag": "hoge",
		"location": "hoge"
	}
}
```

## Field Mapping

| index.txt Field | index.json Field | Description |
|-----------------|------------------|-------------|
| 名称 | name | Collection name |
| ID | id | Collection ID (also in systemData) |
| MC | managementCode | Management code |
| 登録日 | registrationDate | Registration date |
| カテゴリ | category | Category |
| タグ1 | firstTag | First tag |
| タグ2 | secondTag | Second tag |
| タグ3 | thirdTag | Third tag |
| 場所1(Real) | location | Physical location |

## Migration Process

### Automatic Migration
When the application detects an `index.txt` file without a corresponding `index.json`:

1. A message box appears: "index.txtファイルが見つかりました。JSON形式（index.json）に変換します。"
2. User is asked: "変換後、元のindex.txtファイルを削除しますか？"
   - Yes: Old file is deleted after successful conversion
   - No: Old file is kept for reference
3. Success message: "index.jsonへの変換に成功しました。"
4. On failure: Falls back to reading index.txt for backward compatibility

### New Collections
- All new collections automatically create `index.json` in the `SystemData` folder
- No `index.txt` file is created

### Backup Files
- **JSON format**: `{CollectionFolder}\SystemData\index_old.json`
- **Text format** (legacy): `{CollectionFolder}\index_old.txt`

## Date Format
- All dates use ISO8601 format with UTC offset
- Example: `2024-11-08T18:13:37.0000000+00:00`
- Generated using: `DateTimeOffset.Now.ToString("o")`

## Backward Compatibility
- The application can still read `index.txt` files
- Migration is automatic and user-controlled
- Failed migrations fall back to reading the original text format

## Testing Checklist

### Test 1: New Collection Creation
1. Create a new collection
2. Verify `{CollectionFolder}\SystemData\index.json` exists
3. Verify JSON structure matches specification
4. Verify systemCreateDate is in ISO8601 format with offset

### Test 2: Migration from index.txt
1. Open a project with existing `index.txt` files
2. Verify migration dialog appears
3. Test "Yes" option: Verify old file is deleted
4. Test "No" option: Verify old file is kept
5. Verify data integrity after migration

### Test 3: Data Preservation
1. Load a collection with existing data
2. Edit and save the collection
3. Verify all fields are preserved correctly
4. Verify systemData.id and systemCreateDate remain unchanged on save

### Test 4: Backup and Recovery
1. Create/edit a collection (triggers backup)
2. Verify `index_old.json` is created
3. Delete `index.json`
4. Reload collection to trigger recovery
5. Verify data is restored from backup

### Test 5: Backward Compatibility
1. Manually create an `index.txt` file
2. Load the collection
3. Verify migration dialog appears
4. Verify data loads correctly after migration

## Implementation Notes

### Classes Added
- `IndexJsonSystemData`: Contains system metadata (id, systemCreateDate)
- `IndexJsonValues`: Contains user-editable values
- `IndexJsonFormat`: Root JSON structure

### Methods Modified
- `LoadCollectionIndexData`: Now supports both JSON and TXT formats with auto-migration
- `SaveCollectionIndexData`: Saves in JSON format to SystemData folder
- `BackupCollectionIndexData`: Handles both JSON and TXT backups
- `CollectionIndexRecovery_IndexFileNotFound`: Supports recovery from both formats

### Methods Added
- `LoadIndexJsonFile`: Reads JSON using DataContractJsonSerializer
- `SaveIndexJsonFile`: Writes JSON with manual formatting for precise control
- `MigrateIndexTxtToJson`: Converts old format to new with user confirmation
- `EscapeJsonString`: Properly escapes special characters in JSON strings

## Language Support
New message keys added to both Japanese.xml and English.xml:
- `IndexFileMigrationToJson`: Migration notification
- `IndexFileMigrationDeleteOldFile`: Confirmation for deleting old file
- `IndexFileMigrationSuccess`: Success message
- `IndexFileMigrationFailed`: Failure message
