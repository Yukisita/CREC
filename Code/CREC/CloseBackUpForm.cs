/*
CloseBackupForm
Copyright (c) [2022-2026] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CREC
{
    public partial class CloseBackUpForm : Form
    {
        readonly XElement LanguageFile;// 言語ファイル
        ProjectSettingValuesClass CurrentProjectSettingValues = new ProjectSettingValuesClass();// 終了時に開いていたプロジェクトの設定値

        public CloseBackUpForm(ProjectSettingValuesClass projectSettingValues, XElement languageFile)
        {
            InitializeComponent();
            CurrentProjectSettingValues = projectSettingValues;
            LanguageFile = languageFile;
            SetColorMethod();
        }

        /// <summary>
        /// 色設定のメソッド
        /// </summary>
        private void SetColorMethod()
        {
            switch (CurrentProjectSettingValues.ColorSetting)
            {
                case ColorValue.Blue:
                    this.BackColor = Color.AliceBlue;
                    break;
                case ColorValue.White:
                    this.BackColor = Color.WhiteSmoke;
                    break;
                case ColorValue.Sakura:
                    this.BackColor = Color.LavenderBlush;
                    break;
                case ColorValue.Green:
                    this.BackColor = Color.Honeydew;
                    break;
                default:
                    this.BackColor = Color.AliceBlue;
                    break;
            }
        }

        /// <summary>
        /// フォームが表示されたときに呼び出されるイベントハンドラー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseBackUpForm_Shown(object sender, EventArgs e)
        {
            // バックアップ処理の進捗表示のみを行うフォームのため、この中で処理を記述する
            SetLanguage(); // 言語設定を適用

            // 進捗報告用のProgressオブジェクトを作成
            var backUpProgressReport = new Progress<(int completed, int total)>(backUpData =>
            {
                // CloseBackUpProgressBar呼び出してプログレスバーを更新
                CloseBackUpProgressBar.Maximum = backUpData.total;
                CloseBackUpProgressBar.Value = backUpData.completed;
            });

            // プロジェクトのバックアップ処理を開始
            Task<bool> task = CollectionDataClass.BackupProjectDataAsync(
                CurrentProjectSettingValues,
                backUpProgressReport,
                LanguageFile);

            // タスクの完了を待機し、結果に応じてメッセージを表示
            task.ContinueWith(t =>
            {
                this.Invoke((Action)(() =>
                {
                    if (t.Result == true)
                    {
                        // バックアップ完了メッセージを表示
                        backUpStatusLabel.Text = "バックアップが終了しました。";
                    }
                    else
                    {
                        // バックアップ失敗メッセージを表示
                        backUpStatusLabel.Text = "一部または全てのデータのバックアップに失敗しました。";
                    }

                    // チェックボックスがチェックされている場合、フォームを自動で閉じてアプリケーションを終了する
                    if (CloseAftertheBackupFinished.Checked)
                    {
                        this.Close();
                    }
                }));
            });
        }

        #region 言語設定
        private void SetLanguage()// 言語ファイル（xml）を読み込んで表示する処理
        {
            try
            {
                IEnumerable<XElement> labelItemDataList = from item in LanguageFile.Elements("CloseBackupForm").Elements("Label").Elements("item") select item;
                foreach (XElement itemData in labelItemDataList)
                {
                    try
                    {
                        Control[] targetContrl = Controls.Find(itemData.Element("itemname").Value, true);
                        if (targetContrl.Length > 0)
                        {
                            targetContrl[0].Text = itemData.Element("itemtext").Value;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CREC");
            }
        }
        #endregion
    }
}
