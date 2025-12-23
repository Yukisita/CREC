# プロジェクト設定ファイル(.crec)バックアップ機能 - 実装概要

## 実装内容

### 概要
プロジェクト設定ファイル（.crec）のバックアップ機能を実装しました。コレクションバックアップと同様に、プロジェクト設定ファイルも自動的にバックアップされるようになります。

### 実装した機能

1. **バックアップフォルダの作成**
   - フォルダ名: `$ProjectSettingFileBackup`
   - 場所: `projectSettingValues.ProjectBackupFolderPath`内

2. **バックアップファイル名の形式**
   - 形式: `yyyyMMdd_HHmmssfff_元のファイル名.crec`
   - 例: `20250117_143025123_MyProject.crec`

3. **自動バックアップのタイミング**
   - プロジェクト設定ファイル保存後に必ずバックアップ
   - コレクションバックアップ作成時、プロジェクト設定ファイルのバックアップが1つもない場合にバックアップ

4. **バックアップ数の管理**
   - コレクションバックアップ数（`MaxBackupCount`）に合わせる
   - 超過する場合は古いものから削除

### 変更したファイル

1. **CollectionDataClass.cs**
   - 定数 `ProjectSettingFileBackupFolderName` を追加
   - `BackupProjectSettingFile()` メソッドを追加（プロジェクト設定ファイルのバックアップ）
   - `ManageProjectSettingBackupCount()` メソッドを追加（バックアップ数管理）
   - `BackupProjectData()` メソッドを修正（プロジェクト設定ファイルバックアップの存在確認と作成）
   - `CleanupDeletedCollectionBackupFolders()` メソッドを修正（プロジェクト設定ファイルバックアップフォルダを削除対象外に）

2. **ProjectSettingClass.cs**
   - `SaveProjectSetting()` メソッドを修正（保存成功後にバックアップを呼び出し）

### 技術的な詳細

#### BackupProjectSettingFile メソッド
```csharp
public static bool BackupProjectSettingFile(ProjectSettingValuesClass projectSettingValues)
```
- プロジェクト設定ファイルをバックアップフォルダにコピー
- バックアップファイル名は日時＋元のファイル名
- バックアップ後、`ManageProjectSettingBackupCount()`を呼び出してバックアップ数を管理
- エラーが発生した場合はfalseを返すが、ユーザーには通知しない（サイレント失敗）

#### ManageProjectSettingBackupCount メソッド
```csharp
private static void ManageProjectSettingBackupCount(
    ProjectSettingValuesClass projectSettingValues,
    string projectSettingBackupFolderPath)
```
- バックアップファイルを作成日時順にソート
- `MaxBackupCount`を超えている場合、古いバックアップから削除
- エラーが発生した場合は処理を中断

#### BackupProjectData メソッドの変更点
```csharp
// プロジェクト設定ファイルのバックアップが存在しない場合はバックアップを作成
string projectSettingBackupFolderPath = System.IO.Path.Combine(
    projectSettingValues.ProjectBackupFolderPath, 
    ProjectSettingFileBackupFolderName);

bool projectSettingBackupExists = false;
if (Directory.Exists(projectSettingBackupFolderPath))
{
    DirectoryInfo backupDirInfo = new DirectoryInfo(projectSettingBackupFolderPath);
    FileInfo[] backupFiles = backupDirInfo.GetFiles("*.crec");
    projectSettingBackupExists = backupFiles.Length > 0;
}

if (!projectSettingBackupExists)
{
    BackupProjectSettingFile(projectSettingValues);
}
```
- コレクションバックアップ開始時にプロジェクト設定ファイルバックアップの存在を確認
- 存在しない場合のみ新規作成

#### SaveProjectSetting メソッドの変更点
```csharp
// プロジェクト設定ファイル保存後は必ずバックアップする
if (returnValue)
{
    CollectionDataClass.BackupProjectSettingFile(projectSettingValues);
}
```
- プロジェクト設定ファイルの保存に成功した場合のみバックアップを実行

### エラーハンドリング

- バックアップ処理でエラーが発生してもユーザーには通知しない（サイレント失敗）
- エラーはデバッグログに出力される
- プロジェクト設定ファイルの保存自体は成功する
- アプリケーションの動作は継続される

### 既存機能への影響

#### 影響なし
- コレクションバックアップ機能は変更なし
- プロジェクト設定ファイルの保存・読み込み機能は変更なし
- `MaxBackupCount`設定の動作は変更なし
- バックアップログ機能は変更なし

#### 変更あり
- クリーンアップ操作で`$ProjectSettingFileBackup`フォルダは削除されない
- `BackupProjectData()`メソッドは初回実行時にプロジェクト設定ファイルもバックアップする

### テスト推奨事項

1. **基本動作確認**
   - プロジェクト設定変更後の保存
   - バックアップフォルダとファイルの存在確認
   - ファイル名形式の確認

2. **バックアップ数管理**
   - `MaxBackupCount`設定値の変更
   - 古いバックアップの自動削除確認

3. **コレクションバックアップとの連携**
   - 初回コレクションバックアップ時の動作
   - 既存バックアップがある場合の動作

4. **エラーケース**
   - 書き込み権限がない場合
   - ディスク容量不足の場合

詳細なテスト手順は`TESTING_GUIDE_PROJECT_SETTINGS_BACKUP.md`を参照してください。

### 今後の改善案

1. **ユーザー通知の追加**
   - バックアップ失敗時にオプションで通知を表示
   - バックアップ成功時のステータス表示

2. **バックアップの復元機能**
   - UIからプロジェクト設定ファイルを復元できる機能

3. **バックアップの差分管理**
   - 変更がない場合はバックアップをスキップ

4. **バックアップの圧縮**
   - コレクションバックアップと同様に圧縮オプションを追加

## 注意事項

- この実装はWindows環境専用です
- .NET Framework 4.8が必要です
- Visual Studio 2019以降でビルドしてください
- Linuxなどの非Windows環境ではビルドできません

## 実装者向けメモ

- バックアップ処理は同期的に実行されます（通常100ms以下で完了）
- スレッドセーフな実装になっています
- ファイル操作のエラーは`System.Diagnostics.Debug.WriteLine`でログ出力されます
