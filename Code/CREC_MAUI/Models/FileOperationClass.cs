/*
Program
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using File = System.IO.File;

namespace CREC
{
    internal class FileOperationClass
    {
        /// <summary>
        /// 指定されたファイルを削除する処理
        /// </summary>
        /// <param name="FileFullPath">削除するファイルのパス（拡張子含む）</param>
        /// <returns>削除成功：true、削除失敗：false</returns>
        public static bool DeleteFile(string FileFullPath)
        {
            try
            {
                File.Delete(FileFullPath);
                return true;
            }
            catch (ArgumentNullException ex)// PathがNullぽ
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            catch (ArgumentException ex)// Pathが不完全
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            catch (DirectoryNotFoundException ex)// パスが正しくない
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            catch (NotSupportedException ex)// サポート外の形式のパス
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            catch (PathTooLongException ex)// パスが長すぎる
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            catch (IOException ex)// 使用中のファイル
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                if (DeleteReadOnlyFile(FileFullPath))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (UnauthorizedAccessException ex)// 権限がない
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                if (DeleteReadOnlyFile(FileFullPath))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)// 予期せぬエラー
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// ファイルを強制削除
        /// </summary>
        /// <param name="FileFullPath"></param>
        /// <returns>削除成功：true、削除失敗：false</returns>
        private static bool DeleteReadOnlyFile(string FileFullPath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(FileFullPath)
                {
                    Attributes = FileAttributes.Normal
                };
                File.Delete(FileFullPath);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// ファイルを指定の場所に移動
        /// </summary>
        /// <param name="CurrentFileFullPath">移動元のファイルパス</param>
        /// <param name="NewFileFullPath">移動先のファイルパス</param>
        /// <param name="Overwriting">上書き許可 true:上書き可, false:上書き禁止</param>
        /// <param name="deleteCurrentFile">移動元のファイルを削除する true:削除, false:残す</param>
        /// <returns>移動成功：true、移動失敗：false</returns>
        public static bool MoveFile(string CurrentFileFullPath, string NewFileFullPath, bool Overwriting, bool deleteCurrentFile)
        {
            // ファイルを移動
            try
            {
                File.Copy(CurrentFileFullPath, NewFileFullPath, Overwriting);
            }
            catch (ArgumentNullException ex)// PathがNullぽ
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            catch (ArgumentException ex)// Pathに不備がある
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            catch (UnauthorizedAccessException ex)// 権限がない
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            catch (PathTooLongException ex)// パスが長すぎる
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            catch (DirectoryNotFoundException ex)// ディレクトリが見つからない
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            catch (FileNotFoundException ex)// コピー元ファイルが見つからない
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            catch (IOException ex)// コピー先のファイルが存在し、上書き許可がない
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            catch (NotSupportedException ex)// パスの名称がサポート外
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            catch (Exception ex)// 予期せぬエラー
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            // 移動元のファイルを削除
            if (deleteCurrentFile)
            {
                return DeleteFile(CurrentFileFullPath);
            }
            return true;
        }

        /// <summary>
        /// フォルダを指定の場所に移動
        /// </summary>
        /// <param name="CurrentFolderFullPath">移動元のフォルダパス</param>
        /// <param name="NewFolderFullPath">移動先のフォルダパス</param>
        /// <param name="overwriteExistingFolder">既存フォルダの上書き許可 true:上書き可, false:上書き禁止</param>
        /// <param name="cloneFolder">フォルダ内容を同一にする</param>
        /// <param name="overwriteExistingFiles">既存ファイルの上書き許可 true:上書き可, false:上書き禁止</param>
        /// <param name="deleteCurrentFolder">移動元のフォルダを削除する true:削除, false:残す</param>
        /// <returns>移動成功：true、移動失敗：false</returns>
        public static bool MoveFolder(string CurrentFolderFullPath, string NewFolderFullPath, bool overwriteExistingFolder, bool cloneFolder, bool overwriteExistingFiles, bool deleteCurrentFolder)
        {
            if (Directory.Exists(NewFolderFullPath) && !overwriteExistingFolder)// フォルダ上書き禁止
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {("指定されたフォルダは既に存在します。"}");
                return false;
            }

            if (cloneFolder)// フォルダをクローンする場合
            {
                // 移動先のフォルダが存在する場合は削除
                if (Directory.Exists(NewFolderFullPath))
                {
                    try
                    {
                        Directory.Delete(NewFolderFullPath, true);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"[CREC] {ex.Message}");
                        return false;
                    }
                }
            }

            try
            {
                CopyDirectory(CurrentFolderFullPath, NewFolderFullPath, overwriteExistingFiles);
            }
            catch (UnauthorizedAccessException ex)// アクセス許可がない
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {ex.Message}");
                return false;
            }
            catch (ArgumentNullException ex)// 移動前、または移動先のフォルダパスが未指定
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            catch (ArgumentException ex)// 移動前、または移動先のフォルダパスに無効な文字が含まれる
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            catch (PathTooLongException ex)// 移動前、または移動先のフォルダパスがシステムの最大長以上
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            catch (DirectoryNotFoundException ex)// 移動前のフォルダが見つからない
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            catch (IOException ex)// 移動前と移動先のフォルダパスが同一、使用中のフォルダまたはファイルがある、など
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }
            catch (Exception ex)// 予期せぬエラー
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {(ex.Message}");
                return false;
            }

            if (deleteCurrentFolder)// 移動元のフォルダを削除
            {
                try
                {
                    Directory.Delete(CurrentFolderFullPath, true);
                    return true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[CREC] {ex.Message}");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 指定された空ファイルを作成する処理
        /// </summary>
        /// <param name="FileFullPath">作成する空ファイルのパス（拡張子含む）</param>
        /// <returns>作成成功：true、作成失敗：false</returns>
        public static bool AddBlankFile(string FileFullPath)
        {
            try
            {
                File.Create(FileFullPath).Close();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CREC] {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Copy directory recursively
        /// </summary>
        /// <param name="sourceDir">Source directory</param>
        /// <param name="destDir">Destination directory</param>
        /// <param name="overwriteFiles">Whether to overwrite existing files</param>
        private static void CopyDirectory(string sourceDir, string destDir, bool overwriteFiles)
        {
            if (!Directory.Exists(destDir))
            {
                Directory.CreateDirectory(destDir);
            }

            // Copy files
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destDir, fileName);
                File.Copy(file, destFile, overwriteFiles);
            }

            // Copy subdirectories
            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(subDir);
                string destSubDir = Path.Combine(destDir, dirName);
                CopyDirectory(subDir, destSubDir, overwriteFiles);
            }
        }
    }
}
