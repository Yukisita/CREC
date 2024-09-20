﻿/*
Program
Copyright (c) [2022-2024] [S.Yukisita]
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
                MessageBox.Show(ex.Message, "CREC");
                return false;
            }
            catch(ArgumentException ex)// Pathが不完全
            {
                MessageBox.Show(ex.Message, "CREC");
                return false;
            }
            catch (DirectoryNotFoundException ex)// パスが正しくない
            {
                MessageBox.Show(ex.Message, "CREC");
                return false;
            }
            catch (NotSupportedException ex)// サポート外の形式のパス
            {
                MessageBox.Show(ex.Message, "CREC");
                return false;
            }
            catch (PathTooLongException ex)// パスが長すぎる
            {
                MessageBox.Show(ex.Message, "CREC");
                return false;
            }
            catch (IOException ex)// 使用中のファイル
            {
                MessageBox.Show(ex.Message, "CREC");
                if (DeleteReadOnlyFile(FileFullPath))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(UnauthorizedAccessException ex)// 権限がない
            {
                MessageBox.Show(ex.Message, "CREC");
                if (DeleteReadOnlyFile(FileFullPath))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)// 予期せぬエラー
            {
                MessageBox.Show(ex.Message, "CREC");
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
                MessageBox.Show(ex.Message, "CREC");
                return false;
            }
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "CREC");
                return false;
            }
        }
    }
}