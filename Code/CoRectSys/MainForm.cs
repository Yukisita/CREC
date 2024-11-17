/*
MainForm
Copyright (c) [2022-2024] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using File = System.IO.File;
using System.Threading;
using System.Windows.Documents;
using System.IO.Compression;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Imaging;

namespace CREC
{
    public partial class MainForm : Form
    {
        // アップデート確認用URLの更新、Release前に変更忘れずに
        #region 定数の宣言
        readonly string LatestVersionDownloadLink = "https://github.com/Yukisita/CREC/releases/download/Latest_Release/CREC_v8.4.3.zip";// アップデート確認用URL
        readonly string GitHubLatestReleaseURL = "https://github.com/Yukisita/CREC/releases/tag/Latest_Release";// 最新安定版の公開場所URL
        #endregion
        #region 変数の宣言
        // プロジェクトファイル読み込み用変数
        string TargetCRECPath = string.Empty;// 管理ファイル（.crec）のファイルパス
        string TargetIndexPath = string.Empty;// Indexの場所
        string TargetDetailsPath = string.Empty;// 説明txtのパス
        ProjectSettingValuesClass CurrentProjectSettingValues = new ProjectSettingValuesClass();// 現在表示中のプロジェクトの設定値

        string[] cols;// List等読み込み用
        ToolStripMenuItem[] LanguageSettingToolStripMenuItems;
        string CurrentLanguageFileName = string.Empty;
        // config.sys読み込み用Class
        ConfigValuesClass ConfigValues = new ConfigValuesClass();
        /// <summary>
        /// 詳細データ読み込み用変数宣言、詳細表示している内容を入れておく
        /// </summary>
        class DataValuesClass
        {
            public string CollectionFolderPath { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string ID { get; set; } = string.Empty;
            public string MC { get; set; } = string.Empty;
            public string RegistrationDate { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
            public string Tag1 { get; set; } = string.Empty;
            public string Tag2 { get; set; } = string.Empty;
            public string Tag3 { get; set; } = string.Empty;
            public string RealLocation { get; set; } = string.Empty;
        }
        List<DataValuesClass> DataList = new List<DataValuesClass>();// 一覧表示しているデータ
        DataValuesClass CurrentShownDataValues = new DataValuesClass();// 詳細表示中のデータ

        // データ一覧表示関係
        DataTable ContentsDataTable = new DataTable();
        string DataLoadingStatus = "false";// Data非同期読み込みのステータス

        // 表示関係
        double CurrentDPI = 1.0;// 現在のDPI値
        double FirstDPI = 1.0;// 起動時の表示スケール値
        // フォントサイズ
        float extrasmallfontsize = (float)(9.0);// 最小フォントのサイズ
        float smallfontsize = (float)(14.25);// 小フォントのサイズ
        float mainfontsize = (float)(18.0);// 標準フォントのサイズ
        float bigfontsize = (float)(20.25);// 大フォントのサイズ

        XElement LanguageFile;// 言語ファイル
        #endregion

        public MainForm()
        {
            // Boot画面を表示
            BootingForm bootingForm = new BootingForm();
            try
            {
                bootingForm.Show(this);
                Application.DoEvents();// これをやらないとBootingFormが真っ白なままになる
            }
            catch (Exception ex)
            {
                MessageBox.Show("アプリケーションの起動に失敗しました。\n" + ex, "CREC");
            }
            bootingForm.BootingProgressLabel.Text = "アプリケーション起動中...おまちください。";
            Application.DoEvents();
            // 起動処理開始
            InitializeComponent();
            // データ一覧のDGVにダブルバッファリングを設定
            Type dgvType = typeof(DataGridView);
            PropertyInfo dgvPropertyInfo = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            dgvPropertyInfo.SetValue(dataGridView1, true, null);
            dataGridView1.Refresh();
            ContentsDataTable.Rows.Clear();
            ContentsDataTable.Columns.Add("TargetPath");
            ContentsDataTable.Columns.Add("IDList");
            ContentsDataTable.Columns.Add("MCList");
            ContentsDataTable.Columns.Add("ObjectNameList");
            ContentsDataTable.Columns.Add("RegistrationDateList");
            ContentsDataTable.Columns.Add("CategoryList");
            ContentsDataTable.Columns.Add("Tag1List");
            ContentsDataTable.Columns.Add("Tag2List");
            ContentsDataTable.Columns.Add("Tag3List");
            ContentsDataTable.Columns.Add("InventoryList");
            ContentsDataTable.Columns.Add("InventoryStatusList");
            // モニタ情報を取得
            int ScreenWidth = System.Windows.Forms.Screen.GetBounds(this).Width;
            int ScreenHeight = System.Windows.Forms.Screen.GetBounds(this).Height;
            float DpiScale = ((new System.Windows.Forms.Form()).CreateGraphics().DpiX) / 96;// DPI取得
            CurrentDPI = ((new System.Windows.Forms.Form()).CreateGraphics().DpiX) / 96;// DPI取得
            FirstDPI = ((new System.Windows.Forms.Form()).CreateGraphics().DpiX) / 96;// 起動時の表示スケール取得
            if (ScreenWidth < 1280 * DpiScale || ScreenHeight < 620 * DpiScale)// 非対応モニタが検出された場合は警告を表示
            {
                MessageBox.Show("このスクリーンでは正常に表示されない場合があります。\n" + "モニタ解像度=" + ScreenWidth + "X" + ScreenHeight + "\n表示スケール=" + DpiScale * 100 + "%");
            }
            bootingForm.BootingProgressLabel.Text = "設定ファイル読み込み中";
            Application.DoEvents();
            ImportConfig();// configファイルの読み込み・自動生成
            // 言語ファイル読み込み
            bootingForm.BootingProgressLabel.Text = "言語ファイル読み込み中";
            Application.DoEvents();
            System.IO.DirectoryInfo directoryInfo = new DirectoryInfo("language");
            if (!directoryInfo.Exists)// 言語フォルダの存在確認
            {
                MessageBox.Show("言語フォルダが見つかりません。デフォルトの言語ファイルを作成します。\nNo Language Folder.", "CREC");
                Directory.CreateDirectory("language");
                if (!LanguageSettingClass.MakeDefaultLanguageFileJP())
                {
                    MessageBox.Show("致命的なエラーが発生しました。アプリケーションを終了します。", "CREC");
                    Environment.FailFast("Default language file can't find.");
                }
                CurrentLanguageFileName = "Japanese";
                SetLanguage("language\\" + CurrentLanguageFileName + ".xml");
                LanguageSettingClass.MakeDefaultLanguageFileEN();
            }
            else
            {
                try
                {
                    SetLanguage("language\\" + CurrentLanguageFileName + ".xml");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("言語の設定に失敗しました。\n" + ex.Message + "\n言語ファイル（日本語）を作成し、起動します。", "CREC");
                    if (!LanguageSettingClass.MakeDefaultLanguageFileJP())
                    {
                        MessageBox.Show("致命的なエラーが発生しました。アプリケーションを終了します。", "CREC");
                        Environment.FailFast("Default language file can't find.");
                    }
                    CurrentLanguageFileName = "Japanese";
                    SetLanguage("language\\" + CurrentLanguageFileName + ".xml");
                }
            }
            SetColorMethod();// 色設定を反映
            SetTagNameToolTips();// ToolTipsの設定
            bootingForm.BootingProgressLabel.Text = "ウインドウレイアウト調整中";
            Application.DoEvents();
            SetFormLayout();
            // バックグラウンド処理の開始
            CheckContentsList();
            CheckEditing();
            // 自動読み込み設定時は開始（例外処理はImportConfig内で実施済み）
            if (TargetCRECPath.Length > 0)
            {
                bootingForm.BootingProgressLabel.Text = "プロジェクト読み込み中";
                LoadProjectFileMethod();// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
                // Boot画面を閉じる
                try
                {
                    bootingForm.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Empty + ex, "CREC");
                }
                if (CurrentProjectSettingValues.StartUpListOutput == true)
                {
                    ListOutputMethod(CurrentProjectSettingValues.ListOutputFormat);
                }
                if (CurrentProjectSettingValues.StartUpBackUp == true)// 自動バックアップ
                {
                    BackUpMethod();
                    MakeBackUpZip();// ZIP圧縮を非同期で開始
                }
            }
            else
            {
                // Boot画面を閉じる
                try
                {
                    bootingForm.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(string.Empty + ex, "CREC");
                }
            }
            if (ConfigValues.AutoSearch == true)
            {
                SearchButton.Visible = false;
            }
            else
            {
                SearchButton.Visible = true;
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)// フォームが開いた直後の処理
        {
            extrasmallfontsize += ConfigValues.FontsizeOffset;// 最小フォントのサイズ
            smallfontsize += ConfigValues.FontsizeOffset;// 小フォントのサイズ
            mainfontsize += ConfigValues.FontsizeOffset;// 標準フォントのサイズ
            bigfontsize += ConfigValues.FontsizeOffset;// 大フォントのサイズ
            SetFormLayout();
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);// DataGridViewのセルサイズ調整
            dataGridView1.Columns["TargetPath"].Visible = false;// TargetPathを非表示に
            if (ConfigValues.BootUpdateCheck == true)
            {
                CheckLatestVersion();// 更新の確認
            }
            // 英語モードだとボタンが表示されなくなる不具合対策、原因は不明
            ShowSelectedItemInformationButton.Visible = false;
        }

        #region メニューバー関係
        private void NewProjectToolStripMenuItem_Click(object sender, EventArgs e)// 新規プロジェクト作成
        {
            if (SaveAndCloseEditButton.Visible == true)// 編集中の場合は警告を表示
            {
                if (CheckEditingContents() == true)// 編集中のファイルへの操作が完了した場合
                {
                    MakeNewProject makenewproject = new MakeNewProject(string.Empty, CurrentProjectSettingValues, LanguageFile);
                    makenewproject.ShowDialog();
                    if (makenewproject.ReturnTargetProject.Length != 0)// 新規作成されずにメインフォームに戻ってきた場合を除外
                    {
                        TargetCRECPath = makenewproject.ReturnTargetProject;
                        LoadProjectFileMethod();// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
                    }
                }
                else// キャンセルされた場合
                {
                    return;
                }
            }
            else// 編集中データがなかった場合
            {
                MakeNewProject makenewproject = new MakeNewProject(string.Empty, CurrentProjectSettingValues, LanguageFile);
                makenewproject.ShowDialog();
                if (makenewproject.ReturnTargetProject.Length != 0)// 新規作成されずにメインフォームに戻ってきた場合を除外
                {
                    TargetCRECPath = makenewproject.ReturnTargetProject;
                    LoadProjectFileMethod();// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
                }
            }
        }
        private void OpenMenu_Click(object sender, EventArgs e)// 在庫管理プロジェクト読み込み、OpenProjectContextStripMenuItem_Clickと同じ
        {
            OpenProjectMethod();// 既存の在庫管理プロジェクトを読み込むメソッドを呼び出し
            if (CurrentProjectSettingValues.StartUpListOutput == true)
            {
                ListOutputMethod(CurrentProjectSettingValues.ListOutputFormat);
            }
            if (CurrentProjectSettingValues.StartUpBackUp == true)// 自動バックアップ
            {
                BackUpMethod();
                MakeBackUpZip();// ZIP圧縮を非同期で開始
            }
        }
        /// <summary>
        /// 既存のプロジェクトを読み込むメソッド
        /// </summary>
        private void OpenProjectMethod()
        {
            if (CurrentShownDataValues.CollectionFolderPath.Length > 0)// 開いているプロジェクトがあった場合は内容を保存
            {
                ProjectSettingClass.SaveProjectSetting(CurrentProjectSettingValues, TargetCRECPath);
            }

            if (SaveAndCloseEditButton.Visible == true && CheckEditingContents() != true)// 編集中の場合は警告を表示
            {
                return;
            }

            OpenFileDialog openFolderDialog = new OpenFileDialog// OpenFileDialog作成
            {
                InitialDirectory = Directory.GetCurrentDirectory(),
                Title = "ファイルを選択してください",
                Filter = "管理ファイル|*.crec"
            };

            if (openFolderDialog.ShowDialog() == DialogResult.OK)// OpenFileDialogでファイルが選択された場合
            {
                DataLoadingStatus = "false";
                TargetCRECPath = openFolderDialog.FileName;
                openFolderDialog.Dispose();
                LoadProjectFileMethod();// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
            }
            else
            {
                openFolderDialog.Dispose();
            }
        }
        private void LoadProjectFileMethod()// CREC読み込み用の処理メソッド
        {
            ClearDetailsWindowMethod();// 詳細表示画面を初期化
            // 画像表示モードを閉じる
            dataGridView1.Visible = true;
            dataGridView1BackgroundPictureBox.Visible = true;
            if (FullDisplayModeToolStripMenuItem.Checked == true)
            {
                ShowSelectedItemInformationButton.Visible = true;
            }
            SearchFormTextBox.Visible = true;
            SearchFormTextBoxClearButton.Visible = true;
            SearchOptionComboBox.Visible = true;
            SearchMethodComboBox.Visible = true;
            AddContentsButton.Visible = true;
            ListUpdateButton.Visible = true;
            ShowListButton.Visible = false;
            ClosePicturesViewMethod();
            // 読み込み処理呼び出し
            if (!ProjectSettingClass.LoadProjectSetting(ref CurrentProjectSettingValues, TargetCRECPath))
            {
                // 読み込み失敗時
                return;
            }

            // 色設定
            SetColorMethod();
            // ラベルの名称を読み込んで詳細表示画面に設定
            ObjectNameLabel.Text = CurrentProjectSettingValues.CollectionNameLabel + "：";
            IDLabel.Text = CurrentProjectSettingValues.UUIDLabel + "：";
            MCLabel.Text = CurrentProjectSettingValues.ManagementCodeLabel + "：";
            RegistrationDateLabel.Text = CurrentProjectSettingValues.RegistrationDateLabel + "：";
            CategoryLabel.Text = CurrentProjectSettingValues.CategoryLabel + "：";
            Tag1NameLabel.Text = CurrentProjectSettingValues.FirstTagLabel + "：";
            Tag2NameLabel.Text = CurrentProjectSettingValues.SecondTagLabel + "：";
            Tag3NameLabel.Text = CurrentProjectSettingValues.ThirdTagLabel + "：";
            RealLocationLabel.Text = CurrentProjectSettingValues.RealLocationLabel + "：";
            ShowDataLocation.Text = CurrentProjectSettingValues.DataLocationLabel + "：";
            // ラベルの名称を読み込んで検索ボックスに設定、順番注意
            SearchOptionComboBox.Items.Clear();
            SearchOptionComboBox.Items.Add(CurrentProjectSettingValues.UUIDLabel);
            SearchOptionComboBox.Items.Add(CurrentProjectSettingValues.ManagementCodeLabel);
            SearchOptionComboBox.Items.Add(CurrentProjectSettingValues.CollectionNameLabel);
            SearchOptionComboBox.Items.Add(CurrentProjectSettingValues.CategoryLabel);
            SearchOptionComboBox.Items.Add(CurrentProjectSettingValues.FirstTagLabel);
            SearchOptionComboBox.Items.Add(CurrentProjectSettingValues.SecondTagLabel);
            SearchOptionComboBox.Items.Add(CurrentProjectSettingValues.ThirdTagLabel);
            SearchOptionComboBox.Items.Add("在庫状況");
            // ラベルの名称を読み込んでDGVに設定
            dataGridView1.Refresh();
            dataGridView1.Columns["IDList"].HeaderText = CurrentProjectSettingValues.UUIDLabel;
            dataGridView1.Columns["MCList"].HeaderText = CurrentProjectSettingValues.ManagementCodeLabel;
            dataGridView1.Columns["ObjectNameList"].HeaderText = CurrentProjectSettingValues.CollectionNameLabel;
            dataGridView1.Columns["RegistrationDateList"].HeaderText = CurrentProjectSettingValues.RegistrationDateLabel;
            dataGridView1.Columns["CategoryList"].HeaderText = CurrentProjectSettingValues.CategoryLabel;
            dataGridView1.Columns["Tag1List"].HeaderText = CurrentProjectSettingValues.FirstTagLabel;
            dataGridView1.Columns["Tag2List"].HeaderText = CurrentProjectSettingValues.SecondTagLabel;
            dataGridView1.Columns["Tag3List"].HeaderText = CurrentProjectSettingValues.ThirdTagLabel;
            // ラベルの名称を読み込んでDGVのList表示・非表示設定画面に追加
            IDListVisibleToolStripMenuItem.Text = CurrentProjectSettingValues.UUIDLabel;
            MCListVisibleToolStripMenuItem.Text = CurrentProjectSettingValues.ManagementCodeLabel;
            NameListVisibleToolStripMenuItem.Text = CurrentProjectSettingValues.CollectionNameLabel;
            RegistrationDateListVisibleToolStripMenuItem.Text = CurrentProjectSettingValues.RegistrationDateLabel;
            CategoryListVisibleToolStripMenuItem.Text = CurrentProjectSettingValues.CategoryLabel;
            Tag1ListVisibleToolStripMenuItem.Text = CurrentProjectSettingValues.FirstTagLabel;
            Tag2ListVisibleToolStripMenuItem.Text = CurrentProjectSettingValues.SecondTagLabel;
            Tag3ListVisibleToolStripMenuItem.Text = CurrentProjectSettingValues.ThirdTagLabel;

            // 表示内容更新
            ShowProjcetNameTextBox.Text = LanguageSettingClass.GetOtherMessage("ProjectNameHeader", "mainform", LanguageFile) + CurrentProjectSettingValues.Name;
            ObjectNameLabel.Visible
                = ShowObjectName.Visible
                = CurrentProjectSettingValues.CollectionNameVisible;
            IDLabel.Visible
                = ShowID.Visible
                = CurrentProjectSettingValues.UUIDVisible;
            MCLabel.Visible
                = ShowMC.Visible
                = CheckSameMCButton.Visible
                = CurrentProjectSettingValues.ManagementCodeVisible;
            RegistrationDateLabel.Visible
                = ShowRegistrationDate.Visible
                = CurrentProjectSettingValues.RegistrationDateVisible;
            CategoryLabel.Visible
                = ShowCategory.Visible
                = CurrentProjectSettingValues.CategoryVisible;
            Tag1NameLabel.Visible
                = ShowTag1.Visible
                = CurrentProjectSettingValues.FirstTagVisible;
            Tag2NameLabel.Visible
                = ShowTag2.Visible
                = CurrentProjectSettingValues.SecondTagVisible;
            Tag3NameLabel.Visible
                = ShowTag3.Visible
                = CurrentProjectSettingValues.ThirdTagVisible;
            RealLocationLabel.Visible
                = ShowRealLocation.Visible
                = CurrentProjectSettingValues.RealLocationVisible;
            ShowDataLocation.Visible
                = OpenDataLocation.Visible
                = CopyDataLocationPath.Visible
                = CurrentProjectSettingValues.DataLocationVisible;

            // 表示内容復元時にToolStripMenuが出たままにならないようイベントハンドラを一時停止
            IDListVisibleToolStripMenuItem.CheckedChanged -= IDVisibleToolStripMenuItem_CheckedChanged;
            MCListVisibleToolStripMenuItem.CheckedChanged -= MCVisibleToolStripMenuItem_CheckedChanged;
            NameListVisibleToolStripMenuItem.CheckedChanged -= NameVisibleToolStripMenuItem_CheckedChanged;
            RegistrationDateListVisibleToolStripMenuItem.CheckedChanged -= RegistrationDateVisibleToolStripMenuItem_CheckedChanged;
            CategoryListVisibleToolStripMenuItem.CheckedChanged -= CategoryVisibleToolStripMenuItem_CheckedChanged;
            Tag1ListVisibleToolStripMenuItem.CheckedChanged -= Tag1VisibleToolStripMenuItem_CheckedChanged;
            Tag2ListVisibleToolStripMenuItem.CheckedChanged -= Tag2VisibleToolStripMenuItem_CheckedChanged;
            Tag3ListVisibleToolStripMenuItem.CheckedChanged -= Tag3VisibleToolStripMenuItem_CheckedChanged;
            InventoryInformationListToolStripMenuItem.CheckedChanged -= InventoryInformationToolStripMenuItem_CheckedChanged;
            ControlCollectionListColumnVisibe();
            // イベントハンドラ再開
            IDListVisibleToolStripMenuItem.CheckedChanged += IDVisibleToolStripMenuItem_CheckedChanged;
            MCListVisibleToolStripMenuItem.CheckedChanged += MCVisibleToolStripMenuItem_CheckedChanged;
            NameListVisibleToolStripMenuItem.CheckedChanged += NameVisibleToolStripMenuItem_CheckedChanged;
            RegistrationDateListVisibleToolStripMenuItem.CheckedChanged += RegistrationDateVisibleToolStripMenuItem_CheckedChanged;
            CategoryListVisibleToolStripMenuItem.CheckedChanged += CategoryVisibleToolStripMenuItem_CheckedChanged;
            Tag1ListVisibleToolStripMenuItem.CheckedChanged += Tag1VisibleToolStripMenuItem_CheckedChanged;
            Tag2ListVisibleToolStripMenuItem.CheckedChanged += Tag2VisibleToolStripMenuItem_CheckedChanged;
            Tag3ListVisibleToolStripMenuItem.CheckedChanged += Tag3VisibleToolStripMenuItem_CheckedChanged;
            InventoryInformationListToolStripMenuItem.CheckedChanged += InventoryInformationToolStripMenuItem_CheckedChanged;

            // ToolTipsの設定
            SetTagNameToolTips();
            // ListOutputPathの設定
            if (!Directory.Exists(CurrentProjectSettingValues.ListOutputPath))
            {
                CurrentProjectSettingValues.ListOutputPath = CurrentProjectSettingValues.ProjectDataFolderPath;
            }
            // ComboBoxの初期選択項目を設定
            SearchOptionComboBox.SelectedIndexChanged -= SearchOptionComboBox_SelectedIndexChanged;
            SearchMethodComboBox.SelectedIndexChanged -= SearchMethodComboBox_SelectedIndexChanged;
            SearchOptionComboBox.SelectedIndex = CurrentProjectSettingValues.SearchOptionNumber;
            SearchMethodComboBox.SelectedIndex = CurrentProjectSettingValues.SearchMethodNumber;
            SearchOptionComboBox.SelectedIndexChanged += SearchOptionComboBox_SelectedIndexChanged;
            SearchMethodComboBox.SelectedIndexChanged += SearchMethodComboBox_SelectedIndexChanged;
            // DataGridViewにデータ読み込み
            if (DataLoadingStatus == "true")
            {
                DataLoadingStatus = "stop";
            }
            // 最近使用したプロジェクトに登録
            try
            {
                IEnumerable<string> RecentlyOpendProjectList = null;
                if (File.Exists("RecentlyOpenedProjectList.log"))// 履歴が既に存在する場合は読み込み
                {
                    RecentlyOpendProjectList = File.ReadAllLines("RecentlyOpenedProjectList.log", Encoding.GetEncoding("UTF-8"));
                    FileOperationClass.DeleteFile("RecentlyOpenedProjectList.log");
                    StreamWriter streamWriter = new StreamWriter("RecentlyOpenedProjectList.log", true, Encoding.GetEncoding("UTF-8"));
                    streamWriter.WriteLine(CurrentProjectSettingValues.Name + "," + TargetCRECPath);// 今開いたプロジェクトを書き込み
                    int Count = 0;
                    foreach (string line in RecentlyOpendProjectList)
                    {
                        if (line != CurrentProjectSettingValues.Name + "," + TargetCRECPath)// 重複を回避
                        {
                            streamWriter.WriteLine(line);
                            Count++;
                        }
                        if (Count > 5)// 登録数を超えたらWhile抜ける
                        {
                            break;
                        }
                    }
                    streamWriter.Close();
                }
                else// 履歴存在しない場合は新規作成
                {
                    StreamWriter streamWriter = new StreamWriter("RecentlyOpenedProjectList.log", true, Encoding.GetEncoding("UTF-8"));
                    streamWriter.WriteLine(CurrentProjectSettingValues.Name + "," + TargetCRECPath);// 今開いたプロジェクトを書き込み
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("最近使用したプロジェクトの登録に失敗しました。\n" + ex.Message, "CREC");
            }
            LoadGrid();
            SetFormLayout();// ラベルの文字に併せてレイアウトを変更
        }
        private void OpenRecentlyOpendProjectToolStripMenuItem_MouseEnter(object sender, EventArgs e)// 最近使用したプロジェクト表示
        {
            OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems.Clear();// 初期化
            string[] RecentlyOpendProjectList = null;
            if (File.Exists("RecentlyOpenedProjectList.log"))// 履歴が既に存在する場合は読み込み
            {
                try
                {
                    RecentlyOpendProjectList = File.ReadAllLines("RecentlyOpenedProjectList.log", Encoding.GetEncoding("UTF-8"));
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("履歴ファイルの読み込みに失敗しました。\n" + ex.Message, "CREC");
                    return;
                }
                for (int i = 0; i < RecentlyOpendProjectList.Length; i++)
                {
                    switch (i)
                    {
                        case 0:
                            ToolStripItem OpenRecentlyOpendProjectToolStripMenuItem0 = new ToolStripMenuItem();
                            OpenRecentlyOpendProjectToolStripMenuItem0.Click += OpenRecentlyOpendProjectToolStripMenuItemSub_Click;
                            OpenRecentlyOpendProjectToolStripMenuItem0.Text = RecentlyOpendProjectList[0].Split(',')[0];
                            OpenRecentlyOpendProjectToolStripMenuItem0.ToolTipText = RecentlyOpendProjectList[0].Split(',')[1];
                            OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems.Add(OpenRecentlyOpendProjectToolStripMenuItem0);
                            break;
                        case 1:
                            ToolStripItem OpenRecentlyOpendProjectToolStripMenuItem1 = new ToolStripMenuItem();
                            OpenRecentlyOpendProjectToolStripMenuItem1.Click += OpenRecentlyOpendProjectToolStripMenuItemSub_Click;
                            OpenRecentlyOpendProjectToolStripMenuItem1.Text = RecentlyOpendProjectList[1].Split(',')[0];
                            OpenRecentlyOpendProjectToolStripMenuItem1.ToolTipText = RecentlyOpendProjectList[1].Split(',')[1];
                            OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems.Add(OpenRecentlyOpendProjectToolStripMenuItem1);
                            break;
                        case 2:
                            ToolStripItem OpenRecentlyOpendProjectToolStripMenuItem2 = new ToolStripMenuItem();
                            OpenRecentlyOpendProjectToolStripMenuItem2.Click += OpenRecentlyOpendProjectToolStripMenuItemSub_Click;
                            OpenRecentlyOpendProjectToolStripMenuItem2.Text = RecentlyOpendProjectList[2].Split(',')[0];
                            OpenRecentlyOpendProjectToolStripMenuItem2.ToolTipText = RecentlyOpendProjectList[2].Split(',')[1];
                            OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems.Add(OpenRecentlyOpendProjectToolStripMenuItem2);
                            break;
                        case 3:
                            ToolStripItem OpenRecentlyOpendProjectToolStripMenuItem3 = new ToolStripMenuItem();
                            OpenRecentlyOpendProjectToolStripMenuItem3.Click += OpenRecentlyOpendProjectToolStripMenuItemSub_Click;
                            OpenRecentlyOpendProjectToolStripMenuItem3.Text = RecentlyOpendProjectList[3].Split(',')[0];
                            OpenRecentlyOpendProjectToolStripMenuItem3.ToolTipText = RecentlyOpendProjectList[3].Split(',')[1];
                            OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems.Add(OpenRecentlyOpendProjectToolStripMenuItem3);
                            break;
                        case 4:
                            ToolStripItem OpenRecentlyOpendProjectToolStripMenuItem4 = new ToolStripMenuItem();
                            OpenRecentlyOpendProjectToolStripMenuItem4.Click += OpenRecentlyOpendProjectToolStripMenuItemSub_Click;
                            OpenRecentlyOpendProjectToolStripMenuItem4.Text = RecentlyOpendProjectList[4].Split(',')[0];
                            OpenRecentlyOpendProjectToolStripMenuItem4.ToolTipText = RecentlyOpendProjectList[4].Split(',')[1];
                            OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems.Add(OpenRecentlyOpendProjectToolStripMenuItem4);
                            break;
                    }
                }
                // 履歴削除を追加
                ToolStripSeparator OpenRecentlyOpendProjectToolStripMenuItemDropDownItemsSeparator = new ToolStripSeparator();
                OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems.Add(OpenRecentlyOpendProjectToolStripMenuItemDropDownItemsSeparator);
                ToolStripItem DeleteRecentlyOpendProjectListToolStripMenuItem = new ToolStripMenuItem();
                DeleteRecentlyOpendProjectListToolStripMenuItem.Click += DeleteRecentlyOpendProjectListToolStripMenuItem_Click;
                DeleteRecentlyOpendProjectListToolStripMenuItem.Text = LanguageSettingClass.GetOtherMessage("DeleteRecentlyOpendProjectList", "mainform", LanguageFile);
                OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems.Add(DeleteRecentlyOpendProjectListToolStripMenuItem);
            }
            else
            {
                ToolStripItem NoRecentlyOpendProjectListToolStripMenuItem = new ToolStripMenuItem();
                NoRecentlyOpendProjectListToolStripMenuItem.Text = LanguageSettingClass.GetOtherMessage("NoRecentlyOpendProjectList", "mainform", LanguageFile);
                OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems.Add(NoRecentlyOpendProjectListToolStripMenuItem);
            }
        }
        private void OpenRecentlyOpendProjectToolStripMenuItemSub_Click(object sender, EventArgs e)// 最近使用したプロジェクト表示（イベント）
        {
            if (SaveAndCloseEditButton.Visible == true)// 編集中の場合は警告を表示
            {
                if (CheckEditingContents() != true)// 編集中のファイルへの操作が完了した場合
                {
                    return;
                }
            }
            // 開いているプロジェクトがあった場合は内容を保存
            if (CurrentShownDataValues.CollectionFolderPath.Length > 0)
            {
                ProjectSettingClass.SaveProjectSetting(CurrentProjectSettingValues, TargetCRECPath);
            }

            TargetCRECPath = OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems[OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems.IndexOf((ToolStripMenuItem)sender)].ToolTipText;
            if (!File.Exists(TargetCRECPath))
            {
                MessageBox.Show("プロジェクトファイルが見つかりませんでした。\nこの項目を「最近使用したプロジェクト」から削除します。", "CREC");
                // 見つからなかったプロジェクトを履歴から削除
                IEnumerable<string> RecentlyOpendProjectList = null;
                try
                {
                    RecentlyOpendProjectList = File.ReadAllLines("RecentlyOpenedProjectList.log", Encoding.GetEncoding("UTF-8"));
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("履歴ファイルの読み込みに失敗しました。\n" + ex.Message, "CREC");
                    return;
                }
                FileOperationClass.DeleteFile("RecentlyOpenedProjectList.log");
                StreamWriter streamWriter = new StreamWriter("RecentlyOpenedProjectList.log", true, Encoding.GetEncoding("UTF-8"));
                foreach (string line in RecentlyOpendProjectList)
                {
                    if (!line.Contains(TargetCRECPath))// 見つからなかったプロジェクト以外は書き込み
                    {
                        streamWriter.WriteLine(line);
                    }
                }
                streamWriter.Close();
                return;
            }
            switch (OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems.IndexOf((ToolStripMenuItem)sender))
            {
                case 0:
                    DataLoadingStatus = "false";
                    TargetCRECPath = OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems[0].ToolTipText;
                    LoadProjectFileMethod();// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
                    break;
                case 1:
                    DataLoadingStatus = "false";
                    TargetCRECPath = OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems[1].ToolTipText;
                    LoadProjectFileMethod();// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
                    break;
                case 2:
                    DataLoadingStatus = "false";
                    TargetCRECPath = OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems[2].ToolTipText;
                    LoadProjectFileMethod();// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
                    break;
                case 3:
                    DataLoadingStatus = "false";
                    TargetCRECPath = OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems[3].ToolTipText;
                    LoadProjectFileMethod();// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
                    break;
                case 4:
                    DataLoadingStatus = "false";
                    TargetCRECPath = OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems[4].ToolTipText;
                    LoadProjectFileMethod();// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
                    break;
            }

        }
        private void DeleteRecentlyOpendProjectListToolStripMenuItem_Click(object sender, EventArgs e)// 最近使用したプロジェクトの履歴を削除（イベント）
        {
            if (File.Exists("RecentlyOpenedProjectList.log"))// 履歴が既に存在する場合は読み込み
            {
                try
                {
                    FileOperationClass.DeleteFile("RecentlyOpenedProjectList.log");
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("履歴削除に失敗しました。\n" + ex.Message, "CREC");
                }
            }
        }
        private void BackupToolStripMenuItem_Click(object sender, EventArgs e)// 手動バックアップ作成
        {
            BackUpMethod();
            MakeBackUpZip();// ZIP圧縮を非同期で開始
        }
        private void OpenBackUpFolderToolStripMenuItem_Click(object sender, EventArgs e)// バックアップ保存場所を開く
        {
            if (CurrentShownDataValues.CollectionFolderPath.Length == 0)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (CurrentProjectSettingValues.ProjectBackupFolderPath.Length == 0)
            {
                MessageBox.Show("バックアップフォルダが設定されていません。", "CREC");
                return;
            }
            try
            {
                System.Diagnostics.Process.Start(CurrentProjectSettingValues.ProjectBackupFolderPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("フォルダを開けませんでした\n" + ex.Message, "CREC");
            }
        }
        private void OutputListAllContentsToolStripMenuItem_Click(object sender, EventArgs e)// プロジェクトの全データの一覧をListに出力
        {
            ListOutputMethod(CurrentProjectSettingValues.ListOutputFormat);
        }
        private void OutputListShownContentsToolStripMenuItem_Click(object sender, EventArgs e)// 一覧に表示中のデータのみ一覧をListに出力
        {
            string tempTargetListOutputPath = string.Empty;
            if (Directory.Exists(CurrentProjectSettingValues.ListOutputPath))
            {
                tempTargetListOutputPath = CurrentProjectSettingValues.ListOutputPath;
            }
            else
            {
                tempTargetListOutputPath = CurrentProjectSettingValues.ProjectDataFolderPath;
            }
            StreamWriter streamWriter;
            if (CurrentProjectSettingValues.ListOutputFormat == ListOutputFormat.CSV)
            {
                streamWriter = new StreamWriter(tempTargetListOutputPath + "\\InventoryOutput.csv", false, Encoding.GetEncoding("shift-jis"));
            }
            else
            {
                streamWriter = new StreamWriter(tempTargetListOutputPath + "\\InventoryOutput.tsv", false, Encoding.GetEncoding("shift-jis"));
            }
            try
            {
                if (CurrentProjectSettingValues.ListOutputFormat == ListOutputFormat.CSV || CurrentProjectSettingValues.ListOutputFormat == ListOutputFormat.TSV)
                {
                    // ヘッダ書き込み
                    if (CurrentProjectSettingValues.ListOutputFormat == ListOutputFormat.CSV)
                    {
                        streamWriter.WriteLine("データ保存場所のパス,ID,管理コード,名称,登録日,カテゴリー,タグ1,タグ2,タグ3,在庫数,在庫状況");
                    }
                    else
                    {
                        streamWriter.WriteLine("データ保存場所のパス\tID\t管理コード\t名称\t登録日\tカテゴリー\tタグ1\tタグ2\tタグ3\t在庫数\t在庫状況");
                    }
                    try
                    {
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            string ListContentsPath = Convert.ToString(dataGridView1.Rows[i].Cells[0].Value);
                            // 変数初期化
                            DataValuesClass thisDataValues = new DataValuesClass();
                            string ListInventory = string.Empty;
                            string ListInventoryStatus = string.Empty;
                            int? ListSafetyStock = null;
                            int? ListReorderPoint = null;
                            int? ListMaximumLevel = null;
                            // index読み込み
                            IEnumerable<string> tmp = null;
                            try
                            {
                                tmp = File.ReadLines(ListContentsPath + "\\index.txt", Encoding.GetEncoding("UTF-8"));
                                foreach (string line in tmp)
                                {
                                    cols = line.Split(',');
                                    switch (cols[0])
                                    {
                                        case "名称":
                                            thisDataValues.Name = line.Substring(3).Replace(",", string.Empty);
                                            break;
                                        case "ID":
                                            thisDataValues.ID = line.Substring(3).Replace(",", string.Empty);
                                            break;
                                        case "MC":
                                            thisDataValues.MC = line.Substring(3).Replace(",", string.Empty);
                                            break;
                                        case "登録日":
                                            thisDataValues.RegistrationDate = line.Substring(4).Replace(",", string.Empty);
                                            break;
                                        case "カテゴリ":
                                            thisDataValues.Category = line.Substring(5).Replace(",", string.Empty);
                                            break;
                                        case "タグ1":
                                            thisDataValues.Tag1 = line.Substring(4).Replace(",", string.Empty);
                                            break;
                                        case "タグ2":
                                            thisDataValues.Tag2 = line.Substring(4).Replace(",", string.Empty);
                                            break;
                                        case "タグ3":
                                            thisDataValues.Tag3 = line.Substring(4).Replace(",", string.Empty);
                                            break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Indexファイルが破損しています。\n" + ex.Message, "CREC");
                                thisDataValues.ID = Path.GetDirectoryName(ListContentsPath);
                                thisDataValues.Name = "Status：Indexファイル破損";
                                thisDataValues.Category = " - ";
                            }
                            // 在庫状態を取得
                            //invからデータを読み込んで表示
                            if (File.Exists(ListContentsPath + "\\inventory.inv"))
                            {
                                try
                                {
                                    tmp = File.ReadLines(ListContentsPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
                                    bool firstline = true;
                                    int count = 0;
                                    foreach (string line in tmp)
                                    {
                                        cols = line.Split(',');
                                        if (firstline == true)
                                        {
                                            if (cols[1].Length != 0)
                                            {
                                                ListSafetyStock = Convert.ToInt32(cols[1]);
                                            }
                                            if (cols[2].Length != 0)
                                            {
                                                ListReorderPoint = Convert.ToInt32(cols[2]);
                                            }
                                            if (cols[3].Length != 0)
                                            {
                                                ListMaximumLevel = Convert.ToInt32(cols[3]);
                                            }
                                            firstline = false;
                                        }
                                        else
                                        {
                                            count = count + Convert.ToInt32(cols[2]);
                                        }
                                    }
                                    ListInventory = Convert.ToString(count);
                                    ListInventoryStatus = "-";
                                    if (0 == count)
                                    {
                                        ListInventoryStatus = "欠品";
                                    }
                                    else if (0 < count && count < ListSafetyStock)
                                    {
                                        ListInventoryStatus = "不足";
                                    }
                                    else if (ListSafetyStock <= count && count <= ListReorderPoint)
                                    {
                                        ListInventoryStatus = "不足";
                                    }
                                    else if (ListReorderPoint <= count && count <= ListMaximumLevel)
                                    {
                                        ListInventoryStatus = "適正";
                                    }
                                    else if (ListMaximumLevel < count)
                                    {
                                        ListInventoryStatus = "過剰";
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ListInventory = "ERROR";
                                    ListInventoryStatus = ex.Message;
                                }
                            }
                            else
                            {
                                ListInventory = "-";
                                ListInventoryStatus = "-";
                            }
                            // ファイル書き込み
                            if (CurrentProjectSettingValues.ListOutputFormat == ListOutputFormat.CSV)
                            {
                                streamWriter.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", ListContentsPath, thisDataValues.ID, thisDataValues.MC, thisDataValues.Name, thisDataValues.RegistrationDate, thisDataValues.Category, thisDataValues.Tag1, thisDataValues.Tag2, thisDataValues.Tag3, ListInventory, ListInventoryStatus);
                            }
                            else
                            {
                                streamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}", ListContentsPath, thisDataValues.ID, thisDataValues.MC, thisDataValues.Name, thisDataValues.RegistrationDate, thisDataValues.Category, thisDataValues.Tag1, thisDataValues.Tag2, thisDataValues.Tag3, ListInventory, ListInventoryStatus);
                            }
                        }
                        streamWriter.Close();
                        // 正常出力完了のメッセージ表示
                        if (CurrentProjectSettingValues.ListOutputFormat == ListOutputFormat.CSV)
                        {
                            MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("CSVOutputFinish", "mainform", LanguageFile) + "\n" + tempTargetListOutputPath + "\\InventoryOutput.csv", "CREC");
                        }
                        else
                        {
                            MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("TSVOutputFinish", "mainform", LanguageFile) + "\n" + tempTargetListOutputPath + "\\InventoryOutput.tsv", "CREC");
                        }
                    }
                    catch (Exception ex)
                    {
                        // エラーメッセージ表示
                        if (CurrentProjectSettingValues.ListOutputFormat == ListOutputFormat.CSV)
                        {
                            MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("CSVOutputError", "mainform", LanguageFile) + "\n" + ex.Message, "CREC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("TSVOutputError", "mainform", LanguageFile) + "\n" + ex.Message, "CREC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    // 出力後の描画処理
                    if (CurrentProjectSettingValues.OpenListAfterOutput == true)
                    {
                        if (CurrentProjectSettingValues.ListOutputFormat == ListOutputFormat.CSV)
                        {
                            System.Diagnostics.Process process = System.Diagnostics.Process.Start(tempTargetListOutputPath + "\\InventoryOutput.csv");
                        }
                        else if (CurrentProjectSettingValues.ListOutputFormat == ListOutputFormat.TSV)
                        {
                            System.Diagnostics.Process process = System.Diagnostics.Process.Start(tempTargetListOutputPath + "\\InventoryOutput.tsv");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("値が不正です。", "CREC");
                }
            }
            catch (Exception ex)
            {
                // エラーメッセージ表示
                if (CurrentProjectSettingValues.ListOutputFormat == ListOutputFormat.CSV)
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("CSVOutputError", "mainform", LanguageFile) + "\n" + ex.Message, "CREC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("TSVOutputError", "mainform", LanguageFile) + "\n" + ex.Message, "CREC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                streamWriter.Close();
            }
        }
        private void EditConfigSysToolStripMenuItem_Click(object sender, EventArgs e)// 環境設定編集画面
        {
            ConfigForm configform = new ConfigForm(CurrentProjectSettingValues.ColorSetting.ToString(), CurrentLanguageFileName, LanguageFile, ConfigValues.FontsizeOffset);
            configform.ShowDialog();
            if (configform.ReturnConfigSaved)// configが保存された場合
            {
                ImportConfig();// 更新したconfigファイルを読み込み
                SetUserAssistToolTips();// ToolTipの表示設定を再読み込み
            }
        }
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)// プログラム終了
        {
            this.Close();
        }
        private void RestartToolStripMenuItem_Click(object sender, EventArgs e)// アプリ再起動
        {
            if (SaveAndCloseEditButton.Visible == true)// 編集中のデータがある場合
            {
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("AskSaveUnsavedData", "mainform", LanguageFile), "CREC", System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Warning);
                if (result == System.Windows.MessageBoxResult.Yes)// 保存してアプリを終了
                {
                    // 保存関係の処理、入力内容を確認
                    if (CheckContent() == false)
                    {
                        return;// 不備があった場合は再起動キャンセル
                    }
                    // データ保存メソッドを呼び出し
                    if (SaveContentsMethod() == false)
                    {
                        return;
                    }
                }
                else if (result == System.Windows.MessageBoxResult.No)// 保存せずアプリを終了（一時データを削除）
                {
                    // 編集中タグを削除・解放をマーク
                    if (!Directory.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData"))
                    {
                        Directory.CreateDirectory(CurrentShownDataValues.CollectionFolderPath + "\\SystemData");
                    }
                    FileOperationClass.AddBlankFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\FREE");
                    FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED");
                    FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED");
                    // サムネ画像が更新されていた場合は一時データをを削除
                    if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\pictures\\Thumbnail1.newjpg"))
                    {
                        FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\pictures\\Thumbnail1.newjpg");
                    }
                    // 新規作成の場合はデータを削除
                    if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\ADD"))
                    {
                        DeleteContent();
                    }
                    else
                    {
                        FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\ADD");
                    }
                }
                else if (result == System.Windows.MessageBoxResult.Cancel)// アプリ再起動をキャンセル
                {
                    return;
                }
            }
            if (TargetCRECPath.Length != 0)
            {
                ProjectSettingClass.SaveProjectSetting(CurrentProjectSettingValues, TargetCRECPath);
            }
            SaveConfig();
            System.Windows.Forms.Application.Restart();
        }
        private void AddContentsToolStripMenuItem_Click(object sender, EventArgs e)// 新規追加
        {
            AddContentsMethod();// 新規にデータを追加するメソッドを呼び出し
        }
        /// <summary>
        /// 編集内容をリセット
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetEditingContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("編集中のデータを破棄し、編集前の状態に戻しますか？", "CREC", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
            if (result == System.Windows.MessageBoxResult.Yes)
            {
                // 詳細情報読み込み＆表示
                StreamReader streamReaderDetailData = null;
                try
                {
                    streamReaderDetailData = new StreamReader(TargetDetailsPath);
                    DetailsTextBox.Text = streamReaderDetailData.ReadToEnd();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("DetailDataLoadError", "mainform", LanguageFile) + "\n" + ex.Message, "CREC");
                    DetailsTextBox.Text = string.Empty;
                }
                finally
                {
                    streamReaderDetailData?.Close();
                }
                // 機密情報を読み込み
                StreamReader streamReaderConfidentialData = null;
                try
                {
                    streamReaderConfidentialData = new StreamReader(CurrentShownDataValues.CollectionFolderPath + "\\confidentialdata.txt");
                    ConfidentialDataTextBox.Text = streamReaderConfidentialData.ReadToEnd();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("RestrictedDataLoadError", "mainform", LanguageFile) + "\n" + ex.Message, "CREC");
                    ConfidentialDataTextBox.Text = string.Empty;
                }
                finally
                {
                    streamReaderConfidentialData?.Close();
                }
                EditNameTextBox.Text = CurrentShownDataValues.Name;
                EditIDTextBox.TextChanged -= IDTextBox_TextChanged;// ID重複確認イベントを停止
                EditIDTextBox.Text = CurrentShownDataValues.ID;
                EditIDTextBox.TextChanged += IDTextBox_TextChanged;// ID重複確認イベントを開始
                AllowEditIDButton.Visible = true;
                UUIDEditStatusLabel.Visible = false;
                EditMCTextBox.Text = CurrentShownDataValues.MC;
                EditRegistrationDateTextBox.Text = CurrentShownDataValues.RegistrationDate;
                EditCategoryTextBox.Text = CurrentShownDataValues.Category;
                EditTag1TextBox.Text = CurrentShownDataValues.Tag1;
                EditTag2TextBox.Text = CurrentShownDataValues.Tag2;
                EditTag3TextBox.Text = CurrentShownDataValues.Tag3;
                EditRealLocationTextBox.Text = CurrentShownDataValues.RealLocation;
            }
        }
        private void AddInventoryModeToolStripMenuItem_Click(object sender, EventArgs e)// 在庫数管理モードを追加
        {
            if (TargetCRECPath.Length == 0)// プロジェクトが開かれていない場合のエラー
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (CurrentShownDataValues.CollectionFolderPath.Length == 0)
            {
                MessageBox.Show("表示するデータを選択し、詳細表示してください。", "CREC");
                return;
            }
            if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\inventory.inv"))// 在庫数管理モードの表示・非表示
            {
                MessageBox.Show("在庫数管理ファイルは作成済みです。", "CREC");
            }
            else
            {
                FileOperationClass.AddBlankFile(CurrentShownDataValues.CollectionFolderPath + "\\inventory.inv");
                StreamWriter InventoryManagementFile = new StreamWriter(CurrentShownDataValues.CollectionFolderPath + "\\inventory.inv");
                InventoryManagementFile.WriteLine("{0},,,", CurrentShownDataValues.ID);
                InventoryManagementFile.Close();
                InventoryManagementModeButton.Visible = true;
                CloseInventoryManagementModeButton.Visible = false;
            }
        }
        #region コレクション一覧の表示項目設定
        /// <summary>
        /// コレクションリストの表示内容をコントロール
        /// </summary>
        private void ControlCollectionListColumnVisibe()
        {
            IDList.Visible
                = IDListVisibleToolStripMenuItem.Checked
                = dataGridView1.Columns["IDList"].Visible
                = CurrentProjectSettingValues.CollectionListUUIDVisible;
            MCList.Visible
                = MCListVisibleToolStripMenuItem.Checked
                = dataGridView1.Columns["MCList"].Visible
                = CurrentProjectSettingValues.CollectionListManagementCodeVisible;
            ObjectNameList.Visible
                = NameListVisibleToolStripMenuItem.Checked
                = dataGridView1.Columns["ObjectNameList"].Visible
                = CurrentProjectSettingValues.CollectionListNameVisible;
            RegistrationDateList.Visible
                = RegistrationDateListVisibleToolStripMenuItem.Checked
                = dataGridView1.Columns["RegistrationDateList"].Visible
                = CurrentProjectSettingValues.CollectionListRegistrationDateVisible;
            CategoryList.Visible
                = CategoryListVisibleToolStripMenuItem.Checked
                = dataGridView1.Columns["CategoryList"].Visible
                = CurrentProjectSettingValues.CollectionListCategoryVisible;
            Tag1List.Visible
                = Tag1ListVisibleToolStripMenuItem.Checked
                = dataGridView1.Columns["Tag1List"].Visible
                = CurrentProjectSettingValues.CollectionListFirstTagVisible;
            Tag2List.Visible
                = Tag2ListVisibleToolStripMenuItem.Checked
                = dataGridView1.Columns["Tag2List"].Visible
                = CurrentProjectSettingValues.CollectionListSecondTagVisible;
            Tag3List.Visible
                = Tag3ListVisibleToolStripMenuItem.Checked
                = dataGridView1.Columns["Tag3List"].Visible
                = CurrentProjectSettingValues.CollectionListThirdTagVisible;
            InventoryList.Visible
                = InventoryStatusList.Visible
                = InventoryInformationListToolStripMenuItem.Checked
                = dataGridView1.Columns["InventoryList"].Visible
                = dataGridView1.Columns["InventoryStatusList"].Visible
                = CurrentProjectSettingValues.CollectionListInventoryInformationVisible;
        }
        private void IDVisibleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)// IDの表示・非表示
        {
            CurrentProjectSettingValues.CollectionListUUIDVisible = IDListVisibleToolStripMenuItem.Checked;
            ProjectSettingClass.CheckListVisibleColumnExist(ref CurrentProjectSettingValues);
            ControlCollectionListColumnVisibe();
            //選択後もMenuItem開いたままにする処理
            ViewToolStripMenuItem.ShowDropDown();
            VisibleListElementsToolStripMenuItem.ShowDropDown();
        }
        private void MCVisibleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)// 管理コードの表示・非表示
        {
            CurrentProjectSettingValues.CollectionListManagementCodeVisible = MCListVisibleToolStripMenuItem.Checked;
            ProjectSettingClass.CheckListVisibleColumnExist(ref CurrentProjectSettingValues);
            ControlCollectionListColumnVisibe();
            // 選択後もMenuItem開いたままにする処理
            ViewToolStripMenuItem.ShowDropDown();
            VisibleListElementsToolStripMenuItem.ShowDropDown();
        }
        private void NameVisibleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)// 名称の表示・非表示
        {
            CurrentProjectSettingValues.CollectionListNameVisible = NameListVisibleToolStripMenuItem.Checked;
            ProjectSettingClass.CheckListVisibleColumnExist(ref CurrentProjectSettingValues);
            ControlCollectionListColumnVisibe();
            // 選択後もMenuItem開いたままにする処理
            ViewToolStripMenuItem.ShowDropDown();
            VisibleListElementsToolStripMenuItem.ShowDropDown();
        }
        private void RegistrationDateVisibleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)// 登録日の表示・非表示
        {
            CurrentProjectSettingValues.CollectionListRegistrationDateVisible = RegistrationDateListVisibleToolStripMenuItem.Checked;
            ProjectSettingClass.CheckListVisibleColumnExist(ref CurrentProjectSettingValues);
            ControlCollectionListColumnVisibe();
            // 選択後もMenuItem開いたままにする処理
            ViewToolStripMenuItem.ShowDropDown();
            VisibleListElementsToolStripMenuItem.ShowDropDown();
        }
        private void CategoryVisibleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)// カテゴリの表示・非表示
        {
            CurrentProjectSettingValues.CollectionListCategoryVisible = CategoryListVisibleToolStripMenuItem.Checked;
            ProjectSettingClass.CheckListVisibleColumnExist(ref CurrentProjectSettingValues);
            ControlCollectionListColumnVisibe();
            // 選択後もMenuItem開いたままにする処理
            ViewToolStripMenuItem.ShowDropDown();
            VisibleListElementsToolStripMenuItem.ShowDropDown();
        }
        private void Tag1VisibleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)// タグ１の表示・非表示
        {
            CurrentProjectSettingValues.CollectionListFirstTagVisible = Tag1ListVisibleToolStripMenuItem.Checked;
            ProjectSettingClass.CheckListVisibleColumnExist(ref CurrentProjectSettingValues);
            ControlCollectionListColumnVisibe();
            // 選択後もMenuItem開いたままにする処理
            ViewToolStripMenuItem.ShowDropDown();
            VisibleListElementsToolStripMenuItem.ShowDropDown();
        }
        private void Tag2VisibleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)// タグ２の表示・非表示
        {
            CurrentProjectSettingValues.CollectionListSecondTagVisible = Tag2ListVisibleToolStripMenuItem.Checked;
            ProjectSettingClass.CheckListVisibleColumnExist(ref CurrentProjectSettingValues);
            ControlCollectionListColumnVisibe();
            // 選択後もMenuItem開いたままにする処理
            ViewToolStripMenuItem.ShowDropDown();
            VisibleListElementsToolStripMenuItem.ShowDropDown();
        }
        private void Tag3VisibleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)// タグ３の表示・非表示
        {
            CurrentProjectSettingValues.CollectionListThirdTagVisible = Tag3ListVisibleToolStripMenuItem.Checked;
            ProjectSettingClass.CheckListVisibleColumnExist(ref CurrentProjectSettingValues);
            ControlCollectionListColumnVisibe();
            // 選択後もMenuItem開いたままにする処理
            ViewToolStripMenuItem.ShowDropDown();
            VisibleListElementsToolStripMenuItem.ShowDropDown();
        }
        private void InventoryInformationToolStripMenuItem_CheckedChanged(object sender, EventArgs e)// 在庫状況の表示
        {
            CurrentProjectSettingValues.CollectionListInventoryInformationVisible = InventoryInformationListToolStripMenuItem.Checked;
            ProjectSettingClass.CheckListVisibleColumnExist(ref CurrentProjectSettingValues);
            ControlCollectionListColumnVisibe();
            // 選択後もMenuItem開いたままにする処理
            ViewToolStripMenuItem.ShowDropDown();
            VisibleListElementsToolStripMenuItem.ShowDropDown();
        }
        #endregion
        #region 色設定
        private void AliceBlueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentProjectSettingValues.ColorSetting = ColorValue.Blue;
            SetColorMethod();
        }
        private void WhiteSmokeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentProjectSettingValues.ColorSetting = ColorValue.White;
            SetColorMethod();
        }
        private void LavenderBlushToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentProjectSettingValues.ColorSetting = ColorValue.Sakura;
            SetColorMethod();
        }
        private void HoneydewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CurrentProjectSettingValues.ColorSetting = ColorValue.Green;
            SetColorMethod();
        }
        private void DarkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Form1.ForeColor = Color.DarkGray;
            AliceBlueToolStripMenuItem.Checked = false;
            HoneydewToolStripMenuItem.Checked = false;
            LavenderBlushToolStripMenuItem.Checked = false;
            WhiteSmokeToolStripMenuItem.Checked = false;
            DarkToolStripMenuItem.Checked = true;
        }
        #endregion
        #region 文字サイズ
        /// <summary>
        /// フォントサイズを1Pt大きくする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomInFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            extrasmallfontsize += 1;// 最小フォントのサイズ
            smallfontsize += 1;// 小フォントのサイズ
            mainfontsize += 1;// 標準フォントのサイズ
            bigfontsize += 1;// 大フォントのサイズ
            ConfigValues.FontsizeOffset += 1;// フォントサイズオフセット量を加算
            SetFormLayout();
            //選択後もMenuItem開いたままにする処理
            FontSizeToolStripMenuItem.ShowDropDown();
            ZoomInFontToolStripMenuItem.ShowDropDown();
            ResetEditingContentsToolStripMenuItem.ShowDropDown();
        }
        /// <summary>
        /// フォントサイズを1Pt小さくする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZoomOutFontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (extrasmallfontsize <= 1)
            {
                MessageBox.Show("これ以上縮小することはできません。", "CREC");
            }
            else
            {
                extrasmallfontsize -= 1;// 最小フォントのサイズ
                smallfontsize -= 1;// 小フォントのサイズ
                mainfontsize -= 1;// 標準フォントのサイズ
                bigfontsize -= 1;// 大フォントのサイズ
                ConfigValues.FontsizeOffset -= 1;// フォントサイズオフセット量を減算
                SetFormLayout();
                //選択後もMenuItem開いたままにする処理
                FontSizeToolStripMenuItem.ShowDropDown();
                ZoomOutFontToolStripMenuItem.ShowDropDown();
                ResetEditingContentsToolStripMenuItem.ShowDropDown();
            }
        }
        /// <summary>
        /// フォントサイズをリセット
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResetFontSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            extrasmallfontsize = (float)9.0;// 最小フォントのサイズ
            smallfontsize = (float)14.25;// 小フォントのサイズ
            mainfontsize = (float)18.0;// 標準フォントのサイズ
            bigfontsize = (float)20.25;// 大フォントのサイズ
            ConfigValues.FontsizeOffset = 0;// フォントサイズオフセット量を加算
            SetFormLayout();
            //選択後もMenuItem開いたままにする処理
            FontSizeToolStripMenuItem.ShowDropDown();
            ZoomInFontToolStripMenuItem.ShowDropDown();
            ResetEditingContentsToolStripMenuItem.ShowDropDown();
        }
        #endregion
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)// バージョン情報表示
        {
            VersionInformation VerInfo = new VersionInformation(CurrentProjectSettingValues.ColorSetting.ToString(), CurrentDPI, LanguageFile);
            VerInfo.ShowDialog();
        }
        private void readmeToolStripMenuItem_Click(object sender, EventArgs e)// ReadMe表示
        {
            ReadMe readme = new ReadMe(CurrentProjectSettingValues.ColorSetting.ToString());
            readme.ShowDialog();
        }
        private void UpdateHistoryToolStripMenuItem_Click(object sender, EventArgs e)// 更新履歴
        {
            UpdateHistory updateHistory = new UpdateHistory(CurrentProjectSettingValues.ColorSetting.ToString());
            updateHistory.ShowDialog();
        }
        private void AccessLatestReleaseToolStripMenuItem_Click(object sender, EventArgs e)// WebのLatestReleaseにアクセス
        {
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("WebAccessCheck", "mainform", LanguageFile), "CREC", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
            if (result == System.Windows.MessageBoxResult.Yes)// ブラウザでリンクを表示
            {
                System.Diagnostics.Process.Start(GitHubLatestReleaseURL);
            }
        }
        private void UserManualToolStripMenuItem_Click(object sender, EventArgs e)// 利用ガイド
        {
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("WebAccessCheck", "mainform", LanguageFile), "CREC", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
            if (result == System.Windows.MessageBoxResult.Yes)// ブラウザでリンクを表示
            {
                System.Diagnostics.Process.Start("https://github.com/Yukisita/CREC/wiki/%E4%BD%BF%E7%94%A8%E6%96%B9%E6%B3%95");
            }
        }
        private void Form1_Closing(object sender, CancelEventArgs e)// 終了時の処理
        {
            SaveConfig();
            if (TargetCRECPath.Length != 0)
            {
                ProjectSettingClass.SaveProjectSetting(CurrentProjectSettingValues, TargetCRECPath);
                if (SaveAndCloseEditButton.Visible == true)// 編集中のデータがある場合
                {
                    System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("AskSaveUnsavedData", "mainform", LanguageFile), "CREC", System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Warning);
                    if (result == System.Windows.MessageBoxResult.Yes)// 保存してアプリを終了
                    {
                        // 入力内容を確認
                        if (CheckContent() == false)
                        {
                            e.Cancel = true;
                            return;
                        }
                        // データ保存メソッドを呼び出し
                        if (SaveContentsMethod() == false)
                        {
                            e.Cancel = true;
                            return;
                        }
                        if (CurrentProjectSettingValues.CloseListOutput == true)
                        {
                            ListOutputMethod(CurrentProjectSettingValues.ListOutputFormat);
                        }
                        if (CurrentProjectSettingValues.CloseBackUp == true)// 自動バックアップ
                        {
                            BackUpMethod();
                            this.Hide();// メインフォームを消す
                            CloseBackUpForm closeBackUpForm = new CloseBackUpForm(CurrentProjectSettingValues.ColorSetting.ToString());
                            Task.Run(() => { closeBackUpForm.ShowDialog(); });// 別プロセスでバックアップ中のプログレスバー表示ウインドウを開く
                            DateTime DT = DateTime.Now;
                            if (CurrentProjectSettingValues.CompressType == CREC.CompressType.SingleFile)
                            {
                                try
                                {
                                    ZipFile.CreateFromDirectory(CurrentProjectSettingValues.ProjectBackupFolderPath + "\\backuptmp", CurrentProjectSettingValues.ProjectBackupFolderPath + "\\" + CurrentProjectSettingValues.Name + "_backup_" + DT.ToString("yyyy-MM-dd_HH-mm-ss") + ".zip");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("BackupFailed", "mainform", LanguageFile) + "\n" + ex.Message, "CREC");
                                    BackupToolStripMenuItem.Text = LanguageSettingClass.GetToolStripMenuItemMessage("BackupToolStripMenuItem", "mainform", LanguageFile);
                                    BackupToolStripMenuItem.Enabled = true;
                                    return;
                                }
                                Directory.Delete(CurrentProjectSettingValues.ProjectBackupFolderPath + "\\backuptmp", true);
                            }
                            else if (CurrentProjectSettingValues.CompressType == CREC.CompressType.ParData)
                            {
                                // バックアップ用フォルダ作成
                                Directory.CreateDirectory(CurrentProjectSettingValues.ProjectBackupFolderPath + "\\" + CurrentProjectSettingValues.Name + "_backup_" + DT.ToString("yyyy-MM-dd_HH-mm-ss"));
                                File.Copy(TargetCRECPath, CurrentProjectSettingValues.ProjectBackupFolderPath + "\\" + CurrentProjectSettingValues.Name + "_backup_" + DT.ToString("yyyy-MM-dd_HH-mm-ss") + "\\backup.crec", true);// crecファイルをバックアップ
                                try
                                {
                                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(CurrentProjectSettingValues.ProjectDataFolderPath);
                                    IEnumerable<System.IO.DirectoryInfo> subFolders = di.EnumerateDirectories("*");
                                    foreach (System.IO.DirectoryInfo subFolder in subFolders)
                                    {
                                        try
                                        {
                                            FileSystem.CopyDirectory(subFolder.FullName, "backuptmp\\" + subFolder.Name + "\\datatemp", Microsoft.VisualBasic.FileIO.UIOption.AllDialogs, Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing);
                                            ZipFile.CreateFromDirectory("backuptmp\\" + subFolder.Name + "\\datatemp", "backuptmp\\" + subFolder.Name + "backupziptemp.zip");// 圧縮
                                            File.Move("backuptmp\\" + subFolder.Name + "backupziptemp.zip", CurrentProjectSettingValues.ProjectBackupFolderPath + "\\" + CurrentProjectSettingValues.Name + "_backup_" + DT.ToString("yyyy-MM-dd_HH-mm-ss") + "\\" + subFolder.Name + "_backup-" + DT.ToString("yyyy-MM-dd-HH-mm-ss") + ".zip");// 移動
                                            Directory.Delete("backuptmp\\" + subFolder.Name, true);// 削除
                                        }
                                        catch// バックアップ失敗時はログに書き込み
                                        {
                                            StreamWriter streamWriter = new StreamWriter("BackupErrorLog.txt", true, Encoding.GetEncoding("UTF-8"));
                                            streamWriter.WriteLine(subFolder.FullName);
                                            streamWriter.Close();
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("BackupFailed", "mainform", LanguageFile) + "\n" + ex.Message, "CREC");
                                    BackupToolStripMenuItem.Text = LanguageSettingClass.GetToolStripMenuItemMessage("BackupToolStripMenuItem", "mainform", LanguageFile);
                                    BackupToolStripMenuItem.Enabled = true;
                                    return;
                                }
                                Directory.Delete("backuptmp", true);// 削除
                                if (File.Exists("BackupErrorLog.txt"))
                                {
                                    MessageBox.Show("いくつかのファイルのバックアップ作成に失敗しました。\nログを確認してください。", "CREC");
                                }
                            }
                        }
                    }
                    else if (result == System.Windows.MessageBoxResult.No)// 保存せずアプリを終了（一時データを削除）
                    {
                        // 編集中タグを削除・解放をマーク
                        if (!Directory.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData"))
                        {
                            Directory.CreateDirectory(CurrentShownDataValues.CollectionFolderPath + "\\SystemData");
                        }
                        FileOperationClass.AddBlankFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\FREE");
                        FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED");
                        FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED");
                        // サムネ画像が更新されていた場合は一時データをを削除
                        if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\pictures\\Thumbnail1.newjpg"))
                        {
                            FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\pictures\\Thumbnail1.newjpg");
                        }
                        if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\ADD"))
                        {
                            DeleteContent();
                        }
                        else
                        {
                            FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\ADD");
                        }
                        if (CurrentProjectSettingValues.CloseListOutput == true)
                        {
                            ListOutputMethod(CurrentProjectSettingValues.ListOutputFormat);
                        }
                        if (CurrentProjectSettingValues.CloseBackUp == true)// 自動バックアップ
                        {
                            BackUpMethod();
                            this.Hide();// メインフォームを消す
                            CloseBackUpForm closeBackUpForm = new CloseBackUpForm(CurrentProjectSettingValues.ColorSetting.ToString());
                            Task.Run(() => { closeBackUpForm.ShowDialog(); });// 別プロセスでバックアップ中のプログレスバー表示ウインドウを開く
                                                                              // バックアップ作成
                            DateTime DT = DateTime.Now;
                            if (CurrentProjectSettingValues.CompressType == CREC.CompressType.SingleFile)
                            {
                                try
                                {
                                    ZipFile.CreateFromDirectory(CurrentProjectSettingValues.ProjectBackupFolderPath + "\\backuptmp", CurrentProjectSettingValues.ProjectBackupFolderPath + "\\" + CurrentProjectSettingValues.Name + "_backup_" + DT.ToString("yyyy-MM-dd_HH-mm-ss") + ".zip");
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("BackupFailed", "mainform", LanguageFile) + "\n" + ex.Message, "CREC");
                                    BackupToolStripMenuItem.Text = LanguageSettingClass.GetToolStripMenuItemMessage("BackupToolStripMenuItem", "mainform", LanguageFile);
                                    BackupToolStripMenuItem.Enabled = true;
                                    return;
                                }
                                Directory.Delete(CurrentProjectSettingValues.ProjectBackupFolderPath + "\\backuptmp", true);
                            }
                            else if (CurrentProjectSettingValues.CompressType == CREC.CompressType.ParData)
                            {
                                // バックアップ用フォルダ作成
                                Directory.CreateDirectory(CurrentProjectSettingValues.ProjectBackupFolderPath + "\\" + CurrentProjectSettingValues.Name + "_backup_" + DT.ToString("yyyy-MM-dd_HH-mm-ss"));
                                File.Copy(TargetCRECPath, CurrentProjectSettingValues.ProjectBackupFolderPath + "\\" + CurrentProjectSettingValues.Name + "_backup_" + DT.ToString("yyyy-MM-dd_HH-mm-ss") + "\\backup.crec", true);// crecファイルをバックアップ
                                try
                                {
                                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(CurrentProjectSettingValues.ProjectDataFolderPath);
                                    IEnumerable<System.IO.DirectoryInfo> subFolders = di.EnumerateDirectories("*");
                                    foreach (System.IO.DirectoryInfo subFolder in subFolders)
                                    {
                                        try
                                        {
                                            FileSystem.CopyDirectory(subFolder.FullName, "backuptmp\\" + subFolder.Name + "\\datatemp", Microsoft.VisualBasic.FileIO.UIOption.AllDialogs, Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing);
                                            ZipFile.CreateFromDirectory("backuptmp\\" + subFolder.Name + "\\datatemp", "backuptmp\\" + subFolder.Name + "backupziptemp.zip");// 圧縮
                                            File.Move("backuptmp\\" + subFolder.Name + "backupziptemp.zip", CurrentProjectSettingValues.ProjectBackupFolderPath + "\\" + CurrentProjectSettingValues.Name + "_backup_" + DT.ToString("yyyy-MM-dd_HH-mm-ss") + "\\" + subFolder.Name + "_backup-" + DT.ToString("yyyy-MM-dd-HH-mm-ss") + ".zip");// 移動
                                            Directory.Delete("backuptmp\\" + subFolder.Name, true);// 削除
                                        }
                                        catch// バックアップ失敗時はログに書き込み
                                        {
                                            StreamWriter streamWriter = new StreamWriter("BackupErrorLog.txt", true, Encoding.GetEncoding("UTF-8"));
                                            streamWriter.WriteLine(subFolder.FullName);
                                            streamWriter.Close();
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("BackupFailed", "mainform", LanguageFile) + "\n" + ex.Message, "CREC");
                                    BackupToolStripMenuItem.Text = LanguageSettingClass.GetToolStripMenuItemMessage("BackupToolStripMenuItem", "mainform", LanguageFile);
                                    BackupToolStripMenuItem.Enabled = true;
                                    return;
                                }
                                Directory.Delete("backuptmp", true);// 削除
                                if (File.Exists("BackupErrorLog.txt"))
                                {
                                    MessageBox.Show("いくつかのファイルのバックアップ作成に失敗しました。\nログを確認してください。", "CREC");
                                }
                            }
                        }
                    }
                    else if (result == System.Windows.MessageBoxResult.Cancel)// アプリ終了をキャンセル
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else
                {
                    if (CurrentProjectSettingValues.CloseListOutput == true)
                    {
                        ListOutputMethod(CurrentProjectSettingValues.ListOutputFormat);
                    }
                    if (CurrentProjectSettingValues.CloseBackUp == true)// 自動バックアップ
                    {
                        BackUpMethod();
                        this.Hide();// メインフォームを消す
                        CloseBackUpForm closeBackUpForm = new CloseBackUpForm(CurrentProjectSettingValues.ColorSetting.ToString());
                        Task.Run(() => { closeBackUpForm.ShowDialog(); });// 別プロセスでバックアップ中のプログレスバー表示ウインドウを開く
                                                                          // バックアップ作成
                        DateTime DT = DateTime.Now;
                        if (CurrentProjectSettingValues.CompressType == CREC.CompressType.SingleFile)
                        {
                            try
                            {
                                ZipFile.CreateFromDirectory(CurrentProjectSettingValues.ProjectBackupFolderPath + "\\backuptmp", CurrentProjectSettingValues.ProjectBackupFolderPath + "\\" + CurrentProjectSettingValues.Name + "_backup_" + DT.ToString("yyyy-MM-dd_HH-mm-ss") + ".zip");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("BackupFailed", "mainform", LanguageFile) + "\n" + ex.Message, "CREC");
                                BackupToolStripMenuItem.Text = LanguageSettingClass.GetToolStripMenuItemMessage("BackupToolStripMenuItem", "mainform", LanguageFile);
                                BackupToolStripMenuItem.Enabled = true;
                                return;
                            }
                            Directory.Delete(CurrentProjectSettingValues.ProjectBackupFolderPath + "\\backuptmp", true);
                        }
                        else if (CurrentProjectSettingValues.CompressType == CREC.CompressType.ParData)
                        {
                            // バックアップ用フォルダ作成
                            Directory.CreateDirectory(CurrentProjectSettingValues.ProjectBackupFolderPath + "\\" + CurrentProjectSettingValues.Name + "_backup_" + DT.ToString("yyyy-MM-dd_HH-mm-ss"));
                            File.Copy(TargetCRECPath, CurrentProjectSettingValues.ProjectBackupFolderPath + "\\" + CurrentProjectSettingValues.Name + "_backup_" + DT.ToString("yyyy-MM-dd_HH-mm-ss") + "\\backup.crec", true);// crecファイルをバックアップ
                            try
                            {
                                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(CurrentProjectSettingValues.ProjectDataFolderPath);
                                IEnumerable<System.IO.DirectoryInfo> subFolders = di.EnumerateDirectories("*");
                                foreach (System.IO.DirectoryInfo subFolder in subFolders)
                                {
                                    try
                                    {
                                        FileSystem.CopyDirectory(subFolder.FullName, "backuptmp\\" + subFolder.Name + "\\datatemp", Microsoft.VisualBasic.FileIO.UIOption.AllDialogs, Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing);
                                        ZipFile.CreateFromDirectory("backuptmp\\" + subFolder.Name + "\\datatemp", "backuptmp\\" + subFolder.Name + "backupziptemp.zip");// 圧縮
                                        File.Move("backuptmp\\" + subFolder.Name + "backupziptemp.zip", CurrentProjectSettingValues.ProjectBackupFolderPath + "\\" + CurrentProjectSettingValues.Name + "_backup_" + DT.ToString("yyyy-MM-dd_HH-mm-ss") + "\\" + subFolder.Name + "_backup-" + DT.ToString("yyyy-MM-dd-HH-mm-ss") + ".zip");// 移動
                                        Directory.Delete("backuptmp\\" + subFolder.Name, true);// 削除
                                    }
                                    catch// バックアップ失敗時はログに書き込み
                                    {
                                        StreamWriter streamWriter = new StreamWriter("BackupErrorLog.txt", true, Encoding.GetEncoding("UTF-8"));
                                        streamWriter.WriteLine(subFolder.FullName);
                                        streamWriter.Close();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("BackupFailed", "mainform", LanguageFile) + "\n" + ex.Message, "CREC");
                                BackupToolStripMenuItem.Text = LanguageSettingClass.GetToolStripMenuItemMessage("BackupToolStripMenuItem", "mainform", LanguageFile);
                                BackupToolStripMenuItem.Enabled = true;
                                return;
                            }
                            Directory.Delete("backuptmp", true);// 削除
                            if (File.Exists("BackupErrorLog.txt"))
                            {
                                MessageBox.Show("いくつかのファイルのバックアップ作成に失敗しました。\nログを確認してください。", "CREC");
                            }
                        }
                    }
                }
            }
        }
        private void DeleteContentToolStripMenuItem_Click(object sender, EventArgs e)// データ完全削除
        {
            if (CurrentShownDataValues.CollectionFolderPath.Length == 0)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(CurrentShownDataValues.Name + "を削除しますか？\nこの操作は取り消せません。", "CREC", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
            if (result == System.Windows.MessageBoxResult.Yes)// 削除を実行
            {
                DeleteContent();
            }
        }
        private void ForceEditRequestToolStripMenuItem_Click(object sender, EventArgs e)// 編集権限強制取得
        {
            if (ConfigValues.AllowEdit == false)
            {
                MessageBox.Show("設定により編集が禁止されています。", "CREC");
            }
            else if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED"))
            {
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("他の端末で編集中の可能性があります。\n編集権限を強制的に取得しますか？\nなお、本操作によりデータ破損する可能性があります。", "CREC", System.Windows.MessageBoxButton.YesNo);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    // 編集リクエストタグを削除・解放を宣言
                    if (!Directory.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData"))
                    {
                        Directory.CreateDirectory(CurrentShownDataValues.CollectionFolderPath + "\\SystemData");
                    }
                    FileOperationClass.AddBlankFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\FREE");
                    FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED");
                    FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED");
                    FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\ADD");
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (TargetCRECPath.Length == 0)// プロジェクトが開かれていない場合のエラー
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (CurrentShownDataValues.CollectionFolderPath.Length == 0)
                {
                    MessageBox.Show("編集するデータを選択し、詳細表示してください。", "CREC");
                    return;
                }
                StartEditForm();
                // 現時点でのデータを読み込んで表示
                LoadDetails();
                EditNameTextBox.Text = CurrentShownDataValues.Name;
                EditIDTextBox.TextChanged -= IDTextBox_TextChanged;// ID重複確認イベントを停止
                EditIDTextBox.Text = CurrentShownDataValues.ID;
                EditIDTextBox.TextChanged += IDTextBox_TextChanged;// ID重複確認イベントを開始
                EditMCTextBox.Text = CurrentShownDataValues.MC;
                EditRegistrationDateTextBox.Text = CurrentShownDataValues.RegistrationDate;
                EditCategoryTextBox.Text = CurrentShownDataValues.Category;
                EditTag1TextBox.Text = CurrentShownDataValues.Tag1;
                EditTag2TextBox.Text = CurrentShownDataValues.Tag2;
                EditTag3TextBox.Text = CurrentShownDataValues.Tag3;
                EditRealLocationTextBox.Text = CurrentShownDataValues.RealLocation;
            }
        }
        private void EditProjectToolStripMenuItem_Click(object sender, EventArgs e)// プロジェクト管理ファイルの編集
        {
            if (TargetCRECPath.Length == 0)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                if (SaveAndCloseEditButton.Visible == true)// 編集中の場合は警告を表示
                {
                    if (CheckEditingContents() == true)
                    {
                        MakeNewProject makenewproject = new MakeNewProject(TargetCRECPath, CurrentProjectSettingValues, LanguageFile);
                        makenewproject.ShowDialog();
                        if (makenewproject.ReturnTargetProject.Length != 0)// メインフォームに戻ってきたときの処理
                        {
                            TargetCRECPath = makenewproject.ReturnTargetProject;
                            LoadProjectFileMethod();// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    MakeNewProject makenewproject = new MakeNewProject(TargetCRECPath, CurrentProjectSettingValues, LanguageFile);
                    makenewproject.ShowDialog();
                    if (makenewproject.ReturnTargetProject.Length != 0)// メインフォームに戻ってきたときの処理
                    {
                        TargetCRECPath = makenewproject.ReturnTargetProject;
                        LoadProjectFileMethod();// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
                    }
                }
            }
        }
        private void ProjectInformationToolStripMenuItem_Click(object sender, EventArgs e)// プロジェクト情報の表示
        {
            if (TargetCRECPath.Length == 0)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                ProjectInfoForm projectInfoForm = new ProjectInfoForm(CurrentProjectSettingValues);
                projectInfoForm.ShowDialog();
            }
        }
        #endregion

        #region データ一覧・詳細表示関係
        private async void LoadGrid()// データを読み込んでリストに表示
        {
            while (DataLoadingStatus != "false")
            {
                await Task.Delay(1);
            }
            DataLoadingStatus = "true";
            DataLoadingLabel.Visible = true;
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();
            // 現時点で選択されている情報を取得
            string CurrentSelectedContentsID;// 現時点で表示されているデータのID
            int CurrentSelectedContentsRows = 1;// 表示されていたデータのリスト更新後の列番号
            if (dataGridView1.CurrentRow == null)
            {
                CurrentSelectedContentsID = string.Empty;
            }
            else
            {
                try
                {
                    CurrentSelectedContentsID = Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value);
                }
                catch (Exception ex)
                {
                    CurrentSelectedContentsID = string.Empty;
                    MessageBox.Show("現時点で表示されているデータのID取得に失敗しました。\n" + ex.Message, "CREC");
                }
            }
            // DataGridView関係
            bool NoData = true;// データが1つも存在しない場合はtrue
            ContentsDataTable.Rows.Clear();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            try
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(CurrentProjectSettingValues.ProjectDataFolderPath);
                try
                {
                    DataList.Clear();// DataListを初期化
                    IEnumerable<System.IO.DirectoryInfo> subFolders = di.EnumerateDirectories("*");
                    foreach (System.IO.DirectoryInfo subFolder in subFolders)
                    {
                        DataValuesClass thisDataValues = new DataValuesClass();
                        NoData = false;
                        if (DataLoadingStatus == "stop")
                        {
                            DataLoadingStatus = "false";
                            break;
                        }
                        // 変数初期化「List読み込み内でのみ使用すること」
                        string ListInventory = string.Empty;
                        string ListInventoryStatus = string.Empty;
                        int? ListSafetyStock = null;
                        int? ListReorderPoint = null;
                        int? ListMaximumLevel = null;
                        // index読み込み
                        IEnumerable<string> tmp = null;
                        try
                        {
                            tmp = File.ReadLines(subFolder.FullName + "\\index.txt", Encoding.GetEncoding("UTF-8"));
                            foreach (string line in tmp)
                            {
                                cols = line.Split(',');
                                switch (cols[0])
                                {
                                    case "名称":
                                        thisDataValues.Name = line.Substring(3);
                                        break;
                                    case "ID":
                                        thisDataValues.ID = line.Substring(3);
                                        break;
                                    case "MC":
                                        thisDataValues.MC = line.Substring(3);
                                        break;
                                    case "登録日":
                                        thisDataValues.RegistrationDate = line.Substring(4);
                                        break;
                                    case "カテゴリ":
                                        thisDataValues.Category = line.Substring(5);
                                        break;
                                    case "タグ1":
                                        thisDataValues.Tag1 = line.Substring(4);
                                        break;
                                    case "タグ2":
                                        thisDataValues.Tag2 = line.Substring(4);
                                        break;
                                    case "タグ3":
                                        thisDataValues.Tag3 = line.Substring(4);
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Indexファイルが破損しています。\n" + ex.Message, "CREC");
                            thisDataValues.ID = subFolder.Name;
                            thisDataValues.Name = "Status：Indexファイル破損";
                            thisDataValues.Category = "　ー　";
                        }
                        // 在庫状態を取得、invからデータを読み込み
                        if (File.Exists(subFolder.FullName + "\\inventory.inv"))
                        {
                            try
                            {
                                tmp = File.ReadLines(subFolder.FullName + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
                                bool firstline = true;
                                int count = 0;
                                foreach (string line in tmp)
                                {
                                    cols = line.Split(',');
                                    if (firstline == true)
                                    {
                                        if (cols[1].Length != 0)
                                        {
                                            ListSafetyStock = Convert.ToInt32(cols[1]);
                                        }
                                        if (cols[2].Length != 0)
                                        {
                                            ListReorderPoint = Convert.ToInt32(cols[2]);
                                        }
                                        if (cols[3].Length != 0)
                                        {
                                            ListMaximumLevel = Convert.ToInt32(cols[3]);
                                        }
                                        firstline = false;
                                    }
                                    else
                                    {
                                        count = count + Convert.ToInt32(cols[2]);
                                    }
                                }
                                ListInventory = Convert.ToString(count);
                                ListInventoryStatus = "　ー　";
                                if (0 == count)
                                {
                                    ListInventoryStatus = "欠品";
                                }
                                else if (0 < count && count < ListSafetyStock)
                                {
                                    ListInventoryStatus = "不足";
                                }
                                else if (ListSafetyStock <= count && count <= ListReorderPoint)
                                {
                                    ListInventoryStatus = "不足";
                                }
                                else if (ListReorderPoint <= count && count <= ListMaximumLevel)
                                {
                                    ListInventoryStatus = "適正";
                                }
                                else if (ListMaximumLevel < count)
                                {
                                    ListInventoryStatus = "過剰";
                                }
                            }
                            catch (Exception ex)
                            {
                                ListInventory = "ERROR";
                                ListInventoryStatus = ex.Message;
                            }
                        }
                        else
                        {
                            ListInventory = "　ー　";
                            ListInventoryStatus = "　ー　";
                        }
                        //dataGridViewに追加、検索欄に文字が入力されている場合は絞り込み
                        if (SearchFormTextBox.TextLength == 0)
                        {
                            if (SearchOptionComboBox.SelectedIndex == 7)
                            {
                                if (SearchMethod(ListInventoryStatus) == true)
                                {
                                    ContentsDataTable.Rows.Add(subFolder.FullName, thisDataValues.ID, thisDataValues.MC, thisDataValues.Name, thisDataValues.RegistrationDate, thisDataValues.Category, thisDataValues.Tag1, thisDataValues.Tag2, thisDataValues.Tag3, ListInventory, ListInventoryStatus);
                                }
                            }
                            else
                            {
                                ContentsDataTable.Rows.Add(subFolder.FullName, thisDataValues.ID, thisDataValues.MC, thisDataValues.Name, thisDataValues.RegistrationDate, thisDataValues.Category, thisDataValues.Tag1, thisDataValues.Tag2, thisDataValues.Tag3, ListInventory, ListInventoryStatus);
                            }
                        }
                        else if (SearchFormTextBox.TextLength >= 1)
                        {
                            // SearchOptionの検索方法を追加しておいて
                            switch (SearchOptionComboBox.SelectedIndex)
                            {
                                case 0:
                                    if (SearchMethod(thisDataValues.ID) == true)
                                    {
                                        ContentsDataTable.Rows.Add(subFolder.FullName, thisDataValues.ID, thisDataValues.MC, thisDataValues.Name, thisDataValues.RegistrationDate, thisDataValues.Category, thisDataValues.Tag1, thisDataValues.Tag2, thisDataValues.Tag3, ListInventory, ListInventoryStatus);
                                    }
                                    break;
                                case 1:
                                    if (SearchMethod(thisDataValues.MC) == true)
                                    {
                                        ContentsDataTable.Rows.Add(subFolder.FullName, thisDataValues.ID, thisDataValues.MC, thisDataValues.Name, thisDataValues.RegistrationDate, thisDataValues.Category, thisDataValues.Tag1, thisDataValues.Tag2, thisDataValues.Tag3, ListInventory, ListInventoryStatus);
                                    }
                                    break;
                                case 2:
                                    if (SearchMethod(thisDataValues.Name) == true)
                                    {
                                        ContentsDataTable.Rows.Add(subFolder.FullName, thisDataValues.ID, thisDataValues.MC, thisDataValues.Name, thisDataValues.RegistrationDate, thisDataValues.Category, thisDataValues.Tag1, thisDataValues.Tag2, thisDataValues.Tag3, ListInventory, ListInventoryStatus);
                                    }
                                    break;
                                case 3:
                                    if (SearchMethod(thisDataValues.Category) == true)
                                    {
                                        ContentsDataTable.Rows.Add(subFolder.FullName, thisDataValues.ID, thisDataValues.MC, thisDataValues.Name, thisDataValues.RegistrationDate, thisDataValues.Category, thisDataValues.Tag1, thisDataValues.Tag2, thisDataValues.Tag3, ListInventory, ListInventoryStatus);
                                    }
                                    break;
                                case 4:
                                    if (SearchMethod(thisDataValues.Tag1) == true)
                                    {
                                        ContentsDataTable.Rows.Add(subFolder.FullName, thisDataValues.ID, thisDataValues.MC, thisDataValues.Name, thisDataValues.RegistrationDate, thisDataValues.Category, thisDataValues.Tag1, thisDataValues.Tag2, thisDataValues.Tag3, ListInventory, ListInventoryStatus);
                                    }
                                    break;
                                case 5:
                                    if (SearchMethod(thisDataValues.Tag2) == true)
                                    {
                                        ContentsDataTable.Rows.Add(subFolder.FullName, thisDataValues.ID, thisDataValues.MC, thisDataValues.Name, thisDataValues.RegistrationDate, thisDataValues.Category, thisDataValues.Tag1, thisDataValues.Tag2, thisDataValues.Tag3, ListInventory, ListInventoryStatus);
                                    }
                                    break;
                                case 6:
                                    if (SearchMethod(thisDataValues.Tag3) == true)
                                    {
                                        ContentsDataTable.Rows.Add(subFolder.FullName, thisDataValues.ID, thisDataValues.MC, thisDataValues.Name, thisDataValues.RegistrationDate, thisDataValues.Category, thisDataValues.Tag1, thisDataValues.Tag2, thisDataValues.Tag3, ListInventory, ListInventoryStatus);
                                    }
                                    break;
                                case 7:
                                    if (SearchMethod(ListInventoryStatus) == true)
                                    {
                                        ContentsDataTable.Rows.Add(subFolder.FullName, thisDataValues.ID, thisDataValues.MC, thisDataValues.Name, thisDataValues.RegistrationDate, thisDataValues.Category, thisDataValues.Tag1, thisDataValues.Tag2, thisDataValues.Tag3, ListInventory, ListInventoryStatus);
                                    }
                                    break;
                            }
                        }
                        // 更新前に選択されていたデータの行番号を取得
                        if (CurrentSelectedContentsID.Length != 0)
                        {
                            if (CurrentSelectedContentsID == thisDataValues.ID)
                            {
                                dataGridView1.ClearSelection();
                                CurrentSelectedContentsRows = dataGridView1.Rows.Count;
                            }
                        }

                        DataList.Add(thisDataValues);// リストに追加
                    }
                    dataGridView1.DataSource = ContentsDataTable;// ここでバインド
                    dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);// セル幅を調整
                }
                catch (Exception ex)
                {
                    MessageBox.Show("プロジェクトフォルダが見つかりませんでした。\n" + ex.Message, "CREC");
                    this.Cursor = Cursors.Default;
                    DataLoadingLabel.Visible = false;
                    DataLoadingStatus = "false";
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CREC");
                DataLoadingLabel.Visible = false;
                this.Cursor = Cursors.Default;
                DataLoadingStatus = "false";
                return;
            }

            // データが存在する/しないで場合分け
            if (NoData == false)// データが存在する場合は更新前に選択されていたデータを復元
            {
                if (DataLoadingStatus == "true") // 更新前に選択されていたデータを選択
                {
                    dataGridView1.ClearSelection();
                    try
                    {
                        dataGridView1.Rows[CurrentSelectedContentsRows - 1].Selected = true;
                        dataGridView1.CurrentCell = dataGridView1.Rows[CurrentSelectedContentsRows - 1].Cells[0];
                    }
                    catch
                    {
                        DataLoadingLabel.Visible = false;
                        this.Cursor = Cursors.Default;
                        DataLoadingStatus = "false";
                        return;
                    }
                    DataLoadingLabel.Visible = false;
                    this.Cursor = Cursors.Default;
                    DataLoadingStatus = "false";
                }
            }
            else if (NoData == true)// データが１つも存在しない場合は新規データ作成するか確認
            {
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("このプロジェクトにはデータがありません。\nデータを作成しますか？", "CREC", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
                if (result == System.Windows.MessageBoxResult.Yes)// データ作成
                {
                    AddContentsMethod();// 新規にデータを追加するメソッドを呼び出し
                }
                else if (result == System.Windows.MessageBoxResult.No)// データ作成しない
                {
                }
                DataLoadingLabel.Visible = false;
                this.Cursor = Cursors.Default;
                DataLoadingStatus = "false";
            }
            // アクセス日時を更新
            CurrentProjectSettingValues.AccessedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            ProjectSettingClass.SaveProjectSetting(CurrentProjectSettingValues, TargetCRECPath);
            // 一応読み込み終了を宣言
            DataLoadingLabel.Visible = false;
            this.Cursor = Cursors.Default;
            DataLoadingStatus = "false";
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)// 詳細表示
        {
            if (SaveAndCloseEditButton.Visible == true)// 編集中の場合は警告を表示
            {
                if (CheckEditingContents() == false)// 処理をキャンセルされた場合
                {
                    return;// 処理を停止する
                }
            }
            ShowDetails();
        }
        private void OpenDataLocation_Click(object sender, EventArgs e)// ファイルの場所を表示
        {
            try
            {
                System.Diagnostics.Process.Start(CurrentShownDataValues.CollectionFolderPath + "\\data");
            }
            catch (Exception ex)
            {
                MessageBox.Show("フォルダを開けませんでした\n" + ex.Message, "CREC");
            }
        }
        private void CopyDataLocationPath_Click(object sender, EventArgs e)// ファイルのパスをクリップボードにコピー
        {
            try
            {
                Clipboard.SetText(CurrentShownDataValues.CollectionFolderPath + "\\data");
            }
            catch (Exception ex)
            {
                MessageBox.Show("データのパスをクリップボードにコピーできません。\n" + ex.Message, "CREC");
            }
        }
        private void ShowConfidentialDataButton_Click(object sender, EventArgs e)// 機密情報表示
        {
            if (ConfigValues.ShowConfidentialData == false)// 機密情報表示が禁止されている場合
            {
                MessageBox.Show("Access Denied.", "CREC");
            }
            else if (ConfigValues.ShowConfidentialData == true)
            {
                if (TargetCRECPath.Length == 0)// プロジェクトが開かれていない場合のエラー
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (CurrentShownDataValues.CollectionFolderPath.Length == 0)
                {
                    MessageBox.Show("表示するデータを選択し、詳細表示してください。", "CREC");
                    return;
                }
                DetailsTextBox.Visible = false;
                ConfidentialDataTextBox.Visible = true;
                ConfidentialDataLabel.Visible = true;
                DetailsLabel.Visible = false;
                HideConfidentialDataButton.Visible = true;
                ShowConfidentialDataButton.Visible = false;
            }
        }
        private void HideConfidentialDataButton_Click(object sender, EventArgs e)// 機密情報非表示
        {
            DetailsTextBox.Visible = true;
            ConfidentialDataTextBox.Visible = false;
            ConfidentialDataLabel.Visible = false;
            DetailsLabel.Visible = true;
            HideConfidentialDataButton.Visible = false;
            ShowConfidentialDataButton.Visible = true;
            SetFormLayout();
        }
        private bool CheckEditingContents()// 編集中に別のデータを開こうとした場合、編集中データを保存するか確認
        {
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("AskSaveUnsavedData", "mainform", LanguageFile), "CREC", System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Warning);
            if (result == System.Windows.MessageBoxResult.Yes)// 保存して編集画面を閉じる
            {
                // 入力内容を確認
                if (CheckContent() == false)
                {
                    return false;
                }
                // データ保存メソッドを呼び出し
                if (SaveContentsMethod() == false)
                {
                    return false;
                }
                // 通常画面に不要な物を非表示に
                EditNameTextBox.Visible = false;
                EditIDTextBox.Visible = false;
                AllowEditIDButton.Visible = false;
                EditMCTextBox.Visible = false;
                CheckSameMCButton.Visible = false;
                EditRegistrationDateTextBox.Visible = false;
                EditCategoryTextBox.Visible = false;
                EditTag1TextBox.Visible = false;
                EditTag2TextBox.Visible = false;
                EditTag3TextBox.Visible = false;
                EditRealLocationTextBox.Visible = false;
                SaveAndCloseEditButton.Visible = false;
                SelectThumbnailButton.Visible = false;
                OpenPictureFolderButton.Visible = false;
                // 入力フォームをリセット
                ClearDetailsWindowMethod();
                // 通常画面で必要なものを表示
                EditButton.Visible = true;
                ShowTag1.Visible = true;
                ShowTag2.Visible = true;
                ShowTag3.Visible = true;
                ShowPicturesButton.Visible = true;
                // 詳細データおよび機密データを編集不可能に変更
                DetailsTextBox.ReadOnly = true;
                ConfidentialDataTextBox.ReadOnly = true;
                // ID手動設定を不可に変更
                EditIDTextBox.ReadOnly = true;
                ConfigValues.AllowEditID = false;
                AllowEditIDButton.Visible = true;
                UUIDEditStatusLabel.Visible = false;
                if (DataLoadingStatus == "true")
                {
                    DataLoadingStatus = "stop";
                }
                LoadGrid();
                return true;
            }
            else if (result == System.Windows.MessageBoxResult.No)// 保存せず編集画面を閉じる
            {
                // 編集中タグを削除
                if (!Directory.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData"))
                {
                    Directory.CreateDirectory(CurrentShownDataValues.CollectionFolderPath + "\\SystemData");
                }
                FileOperationClass.AddBlankFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\FREE");
                FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED");
                FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED");
                // サムネ画像が更新されていた場合は一時データをを削除
                if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\pictures\\Thumbnail1.newjpg"))
                {
                    FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\pictures\\Thumbnail1.newjpg");
                }
                if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\ADD"))// データ追加時は削除
                {
                    DeleteContent();
                }
                else
                {
                    FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\ADD");
                }
                // 通常画面に不要な物を非表示に
                EditNameTextBox.Visible = false;
                EditIDTextBox.Visible = false;
                AllowEditIDButton.Visible = false;
                EditMCTextBox.Visible = false;
                CheckSameMCButton.Visible = false;
                EditRegistrationDateTextBox.Visible = false;
                EditCategoryTextBox.Visible = false;
                EditTag1TextBox.Visible = false;
                EditTag2TextBox.Visible = false;
                EditTag3TextBox.Visible = false;
                EditRealLocationTextBox.Visible = false;
                SaveAndCloseEditButton.Visible = false;
                SelectThumbnailButton.Visible = false;
                OpenPictureFolderButton.Visible = false;
                // 入力フォームをリセット
                ClearDetailsWindowMethod();
                // 通常画面で必要なものを表示
                EditButton.Visible = true;
                ShowTag1.Visible = true;
                ShowTag2.Visible = true;
                ShowTag3.Visible = true;
                ShowPicturesButton.Visible = true;
                // 詳細データおよび機密データを編集不可能に変更
                DetailsTextBox.ReadOnly = true;
                ConfidentialDataTextBox.ReadOnly = true;
                // ID手動設定を不可に変更
                EditIDTextBox.ReadOnly = true;
                ConfigValues.AllowEditID = false;
                AllowEditIDButton.Visible = true;
                UUIDEditStatusLabel.Visible = false;
                if (DataLoadingStatus == "true")
                {
                    DataLoadingStatus = "stop";
                }
                LoadGrid();
                return true;
            }
            // 上記条件以外、処理せずに戻る場合
            return false;
        }
        private void ShowDetails()// 詳細情報の表示
        {
            // 読み込み中の画面に切り替え
            NoImageLabel.Visible = true;
            Thumbnail.Image = null;
            Thumbnail.BackColor = menuStrip1.BackColor;
            if (LoadDetails() != true)
            {
                return;
            }
            AllowEditIDButton.Visible = false;
            CheckSameMCButton.Visible = false;
            ConfidentialDataTextBox.Visible = false;
            DetailsTextBox.Visible = true;
            // サムネイル表示処理を非同期で開始
            LoadThnumbnailPicture();
            // 表示・非表示項目の設定
            ShowObjectName.Visible = CurrentProjectSettingValues.CollectionNameVisible;
            ShowID.Visible = CurrentProjectSettingValues.UUIDVisible;
            ShowMC.Visible = CurrentProjectSettingValues.ManagementCodeVisible;
            ShowRegistrationDate.Visible = CurrentProjectSettingValues.RegistrationDateVisible;
            ShowCategory.Visible = CurrentProjectSettingValues.CategoryVisible;
            ShowTag1.Visible = CurrentProjectSettingValues.FirstTagVisible;
            ShowTag2.Visible = CurrentProjectSettingValues.SecondTagVisible;
            ShowTag3.Visible = CurrentProjectSettingValues.ThirdTagVisible;
            ShowRealLocation.Visible = CurrentProjectSettingValues.RealLocationVisible;
            ShowDataLocation.Visible = CurrentProjectSettingValues.DataLocationVisible;

            if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\inventory.inv"))// 在庫数管理モードの表示・非表示
            {
                if (CloseInventoryManagementModeButton.Visible == false)
                {
                    InventoryManagementModeButton.Visible = true;
                    CloseInventoryManagementModeButton.Visible = false;
                }
            }
            else
            {
                InventoryManagementModeButton.Visible = false;
                CloseInventoryManagementModeButton.Visible = false;
            }
            DetailsLabel.Visible = true;
            ConfidentialDataLabel.Visible = false;
            ShowConfidentialDataButton.Visible = true;
            HideConfidentialDataButton.Visible = false;
            ObjectNameLabel.Text = CurrentProjectSettingValues.CollectionNameLabel + "：";
            ShowObjectName.Text = CurrentShownDataValues.Name;
            IDLabel.Text = CurrentProjectSettingValues.UUIDLabel + "：";
            ShowID.Text = CurrentShownDataValues.ID;
            MCLabel.Text = CurrentProjectSettingValues.ManagementCodeLabel + "：";
            ShowMC.Text = CurrentShownDataValues.MC;
            RegistrationDateLabel.Text = CurrentProjectSettingValues.RegistrationDateLabel + "：";
            ShowRegistrationDate.Text = CurrentShownDataValues.RegistrationDate;
            CategoryLabel.Text = CurrentProjectSettingValues.CategoryLabel + "：";
            ShowCategory.Text = CurrentShownDataValues.Category;
            Tag1NameLabel.Text = CurrentProjectSettingValues.FirstTagLabel + "：";
            ShowTag1.Text = CurrentShownDataValues.Tag1;
            Tag2NameLabel.Text = CurrentProjectSettingValues.SecondTagLabel + "：";
            ShowTag2.Text = CurrentShownDataValues.Tag2;
            Tag3NameLabel.Text = CurrentProjectSettingValues.ThirdTagLabel + "：";
            ShowTag3.Text = CurrentShownDataValues.Tag3;
            RealLocationLabel.Text = CurrentProjectSettingValues.RealLocationLabel + "：";
            ShowRealLocation.Text = CurrentShownDataValues.RealLocation;
            ShowDataLocation.Text = CurrentProjectSettingValues.DataLocationLabel + "：";
            Application.DoEvents();
            ShowPicturesButton.Visible = true;
            OpenPictureFolderButton.Visible = false;

            // 詳細情報読み込み
            TargetDetailsPath = (CurrentShownDataValues.CollectionFolderPath + "\\details.txt");
            StreamReader streamReaderDetailData = null;
            try
            {
                streamReaderDetailData = new StreamReader(TargetDetailsPath);
                DetailsTextBox.Text = streamReaderDetailData.ReadToEnd();
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("DetailDataLoadError", "mainform", LanguageFile) + "\n" + ex.Message, "CREC");
                DetailsTextBox.Text = "No Data.";
            }
            finally
            {
                streamReaderDetailData?.Close();
            }
            // 機密情報を読み込み
            StreamReader streamReaderConfidentialData = null;
            try
            {
                streamReaderConfidentialData = new StreamReader(CurrentShownDataValues.CollectionFolderPath + "\\confidentialdata.txt");
                ConfidentialDataTextBox.Text = streamReaderConfidentialData.ReadToEnd();
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("RestrictedDataLoadError", "mainform", LanguageFile) + "\n" + ex.Message, "CREC");
                ConfidentialDataTextBox.Text = "No Data.";
            }
            finally
            {
                streamReaderConfidentialData?.Close();
            }

            // 編集ボタンの表示内容設定
            if (ConfigValues.AllowEdit == false || File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED"))
            {
                ReadOnlyButton.Visible = true;
                ReadOnlyButton.ForeColor = Color.Red;
                EditButton.Visible = false;
                EditRequestButton.Visible = false;
            }
            else
            {
                if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED"))
                {
                    EditRequestButton.Visible = true;
                    EditRequestButton.ForeColor = Color.Blue;
                    ReadOnlyButton.Visible = false;
                    EditButton.Visible = false;
                }
                else
                {
                    EditButton.Visible = true;
                    ReadOnlyButton.Visible = false;
                    EditRequestButton.Visible = false;
                }
            }
            // 最近表示した項目としてUUID、名称を保存<- 未実装項目

        }
        private bool LoadDetails()// 詳細情報を読み込み
        {
            // 変数初期化
            ClearDetailsWindowMethod();
            if (dataGridView1.CurrentRow == null)
            {
                return false;
            }
            try
            {
                CurrentShownDataValues.CollectionFolderPath = Convert.ToString(dataGridView1.CurrentRow.Cells[0].Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("詳細情報の読み込みに失敗しました。\n" + ex.Message, "CREC");
                return false;
            }

            if (CurrentShownDataValues.CollectionFolderPath.Length == 0)
            {
                MessageBox.Show("データが存在しません。", "CREC");
                return false;
            }
            // index読み込み
            TargetIndexPath = CurrentShownDataValues.CollectionFolderPath + "\\index.txt";
            try
            {
                IEnumerable<string> tmp = File.ReadLines(TargetIndexPath, Encoding.GetEncoding("UTF-8"));
                foreach (string line in tmp)
                {
                    cols = line.Split(',');
                    if (cols[1].Length == 0)
                    {
                        cols[1] = "　ー　";
                    }
                    switch (cols[0])
                    {
                        case "名称":
                            CurrentShownDataValues.Name = line.Substring(3);
                            break;
                        case "ID":
                            CurrentShownDataValues.ID = line.Substring(3);
                            break;
                        case "MC":
                            CurrentShownDataValues.MC = line.Substring(3);
                            break;
                        case "登録日":
                            CurrentShownDataValues.RegistrationDate = line.Substring(4);
                            break;
                        case "カテゴリ":
                            CurrentShownDataValues.Category = line.Substring(5);
                            break;
                        case "タグ1":
                            CurrentShownDataValues.Tag1 = line.Substring(4);
                            break;
                        case "タグ2":
                            CurrentShownDataValues.Tag2 = line.Substring(4);
                            break;
                        case "タグ3":
                            CurrentShownDataValues.Tag3 = line.Substring(4);
                            break;
                        case "場所1(Real)":
                            CurrentShownDataValues.RealLocation = cols[1];
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Indexファイルが破損しています。\n" + ex.Message, "CREC");
                CurrentShownDataValues.Name = " - ";
                CurrentShownDataValues.ID = Path.GetFileName(CurrentShownDataValues.CollectionFolderPath);
                CurrentShownDataValues.RegistrationDate = " - ";
                return false;
            }
            return true;
        }
        private void ClearDetailsWindowMethod()// 表示されている詳細情報・入力フォームの情報を全てリセットするメソッド
        {
            // 詳細表示の表示内容を初期化
            CurrentShownDataValues = new DataValuesClass();
            ObjectNameLabel.Text = CurrentProjectSettingValues.CollectionNameLabel + "：";
            ShowObjectName.Text = string.Empty;
            IDLabel.Text = CurrentProjectSettingValues.UUIDLabel + "：";
            ShowID.Text = string.Empty;
            MCLabel.Text = CurrentProjectSettingValues.ManagementCodeLabel + "：";
            ShowMC.Text = string.Empty;
            RegistrationDateLabel.Text = CurrentProjectSettingValues.RegistrationDateLabel + "：";
            ShowRegistrationDate.Text = string.Empty;
            CategoryLabel.Text = CurrentProjectSettingValues.CategoryLabel + "：";
            ShowCategory.Text = string.Empty;
            ShowTag1.Text = string.Empty;
            Tag1NameLabel.Text = CurrentProjectSettingValues.FirstTagLabel + "：";
            ShowTag2.Text = string.Empty;
            Tag2NameLabel.Text = CurrentProjectSettingValues.SecondTagLabel + "：";
            ShowTag3.Text = string.Empty;
            Tag3NameLabel.Text = CurrentProjectSettingValues.ThirdTagLabel + "：";
            RealLocationLabel.Text = CurrentProjectSettingValues.RealLocationLabel + "：";
            ShowRealLocation.Text = string.Empty;
            ShowDataLocation.Text = CurrentProjectSettingValues.DataLocationLabel + "：";
            DetailsTextBox.Text = string.Empty;
            ConfidentialDataTextBox.Text = string.Empty;

            // 入力フォームをリセット
            EditNameTextBox.ResetText();
            EditIDTextBox.TextChanged -= IDTextBox_TextChanged;// ID重複確認イベントを停止
            EditIDTextBox.ResetText();
            EditIDTextBox.TextChanged += IDTextBox_TextChanged;// ID重複確認イベントを再開
            EditMCTextBox.ResetText();
            EditRegistrationDateTextBox.ResetText();
            EditCategoryTextBox.ResetText();
            EditTag1TextBox.ResetText();
            EditTag2TextBox.ResetText();
            EditTag3TextBox.ResetText();
            EditRealLocationTextBox.ResetText();
            DetailsTextBox.ResetText();
            ConfidentialDataTextBox.ResetText();
        }

        #endregion

        #region 詳細画像表示関係
        List<string> PicturesList = new List<string>();
        int PictureNumber;
        int PictureCount;
        private void ShowPicturesButton_Click(object sender, EventArgs e)// 詳細画像表示ボタン
        {
            ShowPicturesMethod();
        }
        private void OpenPictureFolderButton_Click(object sender, EventArgs e)// 画像保存フォルダを開くボタン
        {
            try
            {
                System.Diagnostics.Process.Start("EXPLORER.EXE", CurrentShownDataValues.CollectionFolderPath + "\\pictures");
            }
            catch (Exception ex)
            {
                MessageBox.Show("フォルダを開けませんでした\n" + ex.Message, "CREC");
                return;
            }
        }
        private void ShowPicturesMethod()// 詳細画像表示用のメソッド
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(CurrentShownDataValues.CollectionFolderPath + "\\pictures");
            System.IO.FileInfo[] files;
            try
            {
                files = di.GetFiles("*.jpg", System.IO.SearchOption.AllDirectories).Concat(di.GetFiles("*.png", System.IO.SearchOption.AllDirectories)).ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show("画像の読み込みに失敗しました。\n" + ex.Message, "CREC");
                return;
            }
            // 必要なものを表示
            PictureBox1.Visible = true;
            ShowPictureFileNameLabel.Visible = true;
            if (StandardDisplayModeToolStripMenuItem.Checked) { ClosePicturesButton.Visible = true; }
            else if (FullDisplayModeToolStripMenuItem.Checked) { ClosePicturesButton.Visible = false; }

            // 不要なものを非表示
            dataGridView1.Visible = false;
            SearchFormTextBox.Visible = false;
            SearchOptionComboBox.Visible = false;
            SearchMethodComboBox.Visible = false;
            SearchButton.Visible = false;
            SearchFormTextBoxClearButton.Visible = false;
            AddContentsButton.Visible = false;
            ListUpdateButton.Visible = false;
            CloseInventoryViewMethod();// 在庫管理モードを閉じるメソッドを呼び出し
            foreach (System.IO.FileInfo f in files)
            {
                if (f.Name != "Thumbnail1.jpg")// サムネイル画像は除外する
                {
                    PicturesList.Add(f.FullName);
                }
            }
            PictureCount = PicturesList.Count;
            PictureNumber = 0;
            try
            {
                PictureBox1.ImageLocation = PicturesList[PictureNumber];
                ShowPictureFileNameLabel.Text = Path.GetFileName(PicturesList[PictureNumber].ToString());
                NextPictureButton.Visible = true;
                PreviousPictureButton.Visible = true;
                NoPicturesLabel.Visible = false;
            }
            catch
            {
                NextPictureButton.Visible = false;
                PreviousPictureButton.Visible = false;
                NoPicturesLabel.Visible = true;
            }
        }
        private void ClosePicturesButton_Click(object sender, EventArgs e)// 詳細画像非表示ボタン
        {
            ClosePicturesViewMethod();// 画像表示モードを閉じるメソッドを呼び出し
            // 一覧表示モードに戻る
            dataGridView1.Visible = true;
            SearchFormTextBox.Visible = true;
            SearchOptionComboBox.Visible = true;
            SearchMethodComboBox.Visible = true;
            SearchFormTextBoxClearButton.Visible = true;
            AddContentsButton.Visible = true;
            ListUpdateButton.Visible = true;
        }
        private void ClosePicturesViewMethod()// 画像表示モードを閉じるメソッド
        {
            PictureBox1.Visible = false;
            ShowPictureFileNameLabel.Text = string.Empty;
            ShowPictureFileNameLabel.Visible = false;
            PictureBox1.Image = null;// これやらないと次の物に切り替えた直後に前の物の画像が一瞬表示される
            ClosePicturesButton.Visible = false;
            NextPictureButton.Visible = false;
            PreviousPictureButton.Visible = false;
            NoPicturesLabel.Visible = false;
            if (ConfigValues.AutoSearch == false)
            {
                SearchButton.Visible = true;
            }
            PicturesList.Clear();
        }
        private void NextPictreButton_Click(object sender, EventArgs e)// 次の画像を表示
        {
            PictureNumber++;
            if (PictureNumber >= PicturesList.Count)
            {
                PictureNumber = 0;
            }
            PictureBox1.ImageLocation = PicturesList[PictureNumber];
            ShowPictureFileNameLabel.Text = Path.GetFileName(PicturesList[PictureNumber].ToString());
        }
        private void PreviousPictureButton_Click(object sender, EventArgs e)// 前の画像を表示
        {
            PictureNumber--;
            if (PictureNumber < 0)
            {
                PictureNumber = PictureCount - 1;
            }
            PictureBox1.ImageLocation = PicturesList[PictureNumber];
            ShowPictureFileNameLabel.Text = Path.GetFileName(PicturesList[PictureNumber].ToString());
        }
        private void OpenPicturewithAppToolStripMenuItem_Click(object sender, EventArgs e)// 画像をアプリケーションで開く
        {
            if (PicturesList.Count > 0)
            {
                try
                {
                    var pInfo = new ProcessStartInfo
                    {
                        FileName = PicturesList[PictureNumber]
                    };
                    Process.Start(pInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("画像の表示に失敗しました。\n" + ex, "CREC");
                }
            }
            else
            {
                MessageBox.Show("表示可能な画像が見つかりません。", "CREC");
            }
        }
        #endregion

        #region 編集・データ追加・データ削除関係
        private void EditButton_Click(object sender, EventArgs e)// 編集画面表示
        {
            // 状態を確認、必要に応じてボタンを更新
            if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED") && File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED"))
            {
                ReadOnlyButton.Visible = true;
                EditButton.Visible = false;
                EditRequestButton.Visible = false;
                return;
            }
            else if (!File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED") && !File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED"))
            {
                if (TargetCRECPath.Length == 0)// プロジェクトが開かれていない場合のエラー
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (CurrentShownDataValues.CollectionFolderPath.Length == 0)
                {
                    MessageBox.Show("編集するデータを選択し、詳細表示してください。", "CREC");
                    return;
                }
                StartEditForm();
                // 詳細情報読み込み＆表示
                StreamReader streamReaderDetailData = null;
                try
                {
                    streamReaderDetailData = new StreamReader(TargetDetailsPath);
                    DetailsTextBox.Text = streamReaderDetailData.ReadToEnd();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("DetailDataLoadError", "mainform", LanguageFile) + "\n" + ex.Message, "CREC");
                    DetailsTextBox.Text = string.Empty;
                }
                finally
                {
                    streamReaderDetailData?.Close();
                }
                // 機密情報を読み込み
                StreamReader streamReaderConfidentialData = null;
                try
                {
                    streamReaderConfidentialData = new StreamReader(CurrentShownDataValues.CollectionFolderPath + "\\confidentialdata.txt");
                    ConfidentialDataTextBox.Text = streamReaderConfidentialData.ReadToEnd();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("RestrictedDataLoadError", "mainform", LanguageFile) + "\n" + ex.Message, "CREC");
                    ConfidentialDataTextBox.Text = string.Empty;
                }
                finally
                {
                    streamReaderConfidentialData?.Close();
                }
                EditNameTextBox.Text = CurrentShownDataValues.Name;
                EditIDTextBox.TextChanged -= IDTextBox_TextChanged;// ID重複確認イベントを停止
                EditIDTextBox.Text = CurrentShownDataValues.ID;
                EditIDTextBox.TextChanged += IDTextBox_TextChanged;// ID重複確認イベントを開始
                EditMCTextBox.Text = CurrentShownDataValues.MC;
                EditRegistrationDateTextBox.Text = CurrentShownDataValues.RegistrationDate;
                EditCategoryTextBox.Text = CurrentShownDataValues.Category;
                EditTag1TextBox.Text = CurrentShownDataValues.Tag1;
                EditTag2TextBox.Text = CurrentShownDataValues.Tag2;
                EditTag3TextBox.Text = CurrentShownDataValues.Tag3;
                EditRealLocationTextBox.Text = CurrentShownDataValues.RealLocation;
            }
            else if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED") && !File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED"))
            {
                ReadOnlyButton.Visible = false;
                EditButton.Visible = false;
                EditRequestButton.Visible = true;
                return;
            }
        }
        private void EditRequestButton_Click(object sender, EventArgs e)// 編集権限リクエスト
        {
            // 状態を確認、必要に応じてボタンを更新
            if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED") && File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED"))
            {
                ReadOnlyButton.Visible = true;
                EditButton.Visible = false;
                EditRequestButton.Visible = false;
                return;
            }
            else if (!File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED") && !File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED"))
            {
                ReadOnlyButton.Visible = false;
                EditButton.Visible = true;
                EditRequestButton.Visible = false;
                return;
            }
            else if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED") && !File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED"))
            {
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("他の端末で編集中のため、ロックされています。\n編集権限をリクエストしますか？", "CREC", System.Windows.MessageBoxButton.YesNo);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    // 編集リクエストタグを作成
                    if (!Directory.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData"))
                    {
                        Directory.CreateDirectory(CurrentShownDataValues.CollectionFolderPath + "\\SystemData");
                    }
                    FileOperationClass.AddBlankFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED");
                    EditRequestButton.Visible = false;
                    EditRequestingButton.Visible = true;
                    AwaitEdit();
                }
            }
        }
        private void ReadOnlyButton_Click(object sender, EventArgs e)// 編集不可
        {
            // 状態を確認、必要に応じてボタンを更新
            if (ConfigValues.AllowEdit == false)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoEditingPermissions", "mainform", LanguageFile), "CREC");
                return;
            }
            else if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED") && File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED"))
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("EditWaitingUserExists", "mainform", LanguageFile), "CREC");
                return;
            }
            else if (!File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED") && !File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED"))
            {
                ReadOnlyButton.Visible = false;
                EditButton.Visible = true;
                EditRequestButton.Visible = false;
                return;
            }
            else if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED") && !File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED"))
            {
                ReadOnlyButton.Visible = false;
                EditButton.Visible = false;
                EditRequestButton.Visible = true;
                return;
            }
        }
        private void StartEditForm()// 編集画面に切り替え
        {
            // 編集中の端末がいないか再確認
            if (!File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\FREE") && !File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\ADD"))
            {
                DialogResult result = MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("AskStartEditWithoutCheckOtherEditing", "mainform", LanguageFile), "CREC", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }
            }
            else
            {
                FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\FREE");
            }
            // 編集中タグを作成
            if (!Directory.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData"))
            {
                Directory.CreateDirectory(CurrentShownDataValues.CollectionFolderPath + "\\SystemData");
            }
            FileOperationClass.AddBlankFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED");
            AwaitEditRequest();// 編集リクエスト待機非同期処理を開始
            // サムネ用画像変更用データが残っていた場合削除
            if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\pictures\\Thumbnail1.newjpg"))
            {
                FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\pictures\\Thumbnail1.newjpg");
            }
            // 編集画面に必要な物を表示
            EditNameTextBox.Visible = CurrentProjectSettingValues.CollectionNameVisible;
            EditIDTextBox.Visible = CurrentProjectSettingValues.UUIDVisible;
            AllowEditIDButton.Visible = CurrentProjectSettingValues.UUIDVisible;
            UUIDEditStatusLabel.Visible = false;
            EditMCTextBox.Visible = CurrentProjectSettingValues.ManagementCodeVisible;
            CheckSameMCButton.Visible = CurrentProjectSettingValues.ManagementCodeVisible;
            EditRegistrationDateTextBox.Visible = CurrentProjectSettingValues.RegistrationDateVisible;
            EditCategoryTextBox.Visible = CurrentProjectSettingValues.CategoryVisible;
            EditTag1TextBox.Visible = CurrentProjectSettingValues.FirstTagVisible;
            EditTag2TextBox.Visible = CurrentProjectSettingValues.SecondTagVisible;
            EditTag3TextBox.Visible = CurrentProjectSettingValues.ThirdTagVisible;
            EditRealLocationTextBox.Visible = CurrentProjectSettingValues.RealLocationVisible;
            SaveAndCloseEditButton.Visible = true;
            SelectThumbnailButton.Visible = true;
            OpenPictureFolderButton.Visible = true;
            // 編集画面で不要なものを非表示
            EditButton.Visible = false;
            ShowObjectName.Visible = false;
            ShowID.Visible = false;
            ShowMC.Visible = false;
            ShowRegistrationDate.Visible = false;
            ShowCategory.Visible = false;
            ShowTag1.Visible = false;
            ShowTag2.Visible = false;
            ShowTag3.Visible = false;
            ShowRealLocation.Visible = false;
            ShowPicturesButton.Visible = false;

            // 詳細データおよび機密データを編集可能に変更
            DetailsTextBox.ReadOnly = false;
            ConfidentialDataTextBox.ReadOnly = false;
            // 一応ID重複確認イベントを開始
            EditIDTextBox.TextChanged += IDTextBox_TextChanged;
        }
        private void SelectThumbnailButton_Click(object sender, EventArgs e)// サムネイル選択
        {
            OpenFileDialog openFolderDialog = new OpenFileDialog();
            openFolderDialog.InitialDirectory = CurrentShownDataValues.CollectionFolderPath + "\\pictures";
            openFolderDialog.Title = "画像を選択してください";
            openFolderDialog.Filter = "|*.jpg;*.png";
            if (openFolderDialog.ShowDialog() == DialogResult.OK)// ファイル読み込み成功
            {
                Thumbnail.ImageLocation = (openFolderDialog.FileName);// この時点で選択されたサムネイルを仮表示（未保存状態）
                try
                {
                    // ここに画像サイズ変更処理を追加
                    int TargetWidth = 400;
                    int TargetHeight = 400;
                    Bitmap TargetBitmap = new Bitmap(openFolderDialog.FileName);
                    // 画素数設定
                    if (Math.Max(TargetBitmap.Width, TargetBitmap.Height) < 400)// 画像サイズが元々規定サイズ以内だった場合
                    {
                        TargetWidth = TargetBitmap.Width;
                        TargetHeight = TargetBitmap.Height;
                    }
                    else// 画像圧縮処理
                    {
                        // 長辺を取得してサイズを決定
                        if (TargetBitmap.Width > TargetBitmap.Height)// 横長画像
                        {
                            TargetWidth = 400;
                            TargetHeight = (int)(400.0 * TargetBitmap.Height / TargetBitmap.Width);
                        }
                        else if (TargetBitmap.Width < TargetBitmap.Height)// 縦長画像
                        {
                            TargetHeight = 400;
                            TargetWidth = (int)(400.0 * TargetBitmap.Width / TargetBitmap.Height);
                        }
                    }
                    Bitmap ConvertedBitmap = new Bitmap(TargetWidth, TargetHeight);
                    // DPI設定
                    if (Math.Max(TargetBitmap.HorizontalResolution, TargetBitmap.VerticalResolution) <= 72)
                    {
                        ConvertedBitmap.SetResolution(TargetBitmap.HorizontalResolution, TargetBitmap.VerticalResolution);
                    }
                    else
                    {
                        ConvertedBitmap.SetResolution(72.0F, 72.0F);
                    }
                    // エンコーダ設定
                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.ColorDepth, 24L);
                    ImageCodecInfo imageCodecInfos = null;
                    foreach (ImageCodecInfo imageCodecInfo in ImageCodecInfo.GetImageEncoders())
                    {
                        if (imageCodecInfo.FormatID == ImageFormat.Jpeg.Guid)
                        {
                            imageCodecInfos = imageCodecInfo;
                            break;
                        }
                    }
                    // 変換＆保存
                    using (Graphics g = Graphics.FromImage(ConvertedBitmap))
                    {
                        g.DrawImage(TargetBitmap, 0, 0, TargetWidth, TargetHeight);
                    }
                    ConvertedBitmap.Save(CurrentShownDataValues.CollectionFolderPath + "\\pictures\\Thumbnail1.newjpg", imageCodecInfos, encoderParameters);
                    ConvertedBitmap.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("サムネイルの設定に失敗しました。\n" + ex.Message, "CREC");
                }
                NoImageLabel.Visible = false;
            }
        }
        private void SaveAndCloseEditButton_Click(object sender, EventArgs e)// 編集画面の終了
        {
            SaveAndCloseEditButton.Visible = false;
            SavingLabel.Visible = true;
            Application.DoEvents();
            // 入力内容を確認
            if (CheckContent() == false)
            {
                SaveAndCloseEditButton.Visible = true;
                SavingLabel.Visible = false;
                EditRequestButton.Visible = false;
                return;
            }
            // データ保存メソッドを呼び出し
            if (SaveContentsMethod() == false)
            {
                SaveAndCloseEditButton.Visible = true;
                SavingLabel.Visible = false;
                EditRequestButton.Visible = false;
                return;
            }
            // 通常画面用にラベルを変更
            ShowPicturesButton.Visible = true;
            SavingLabel.Visible = false;
            // 通常画面に不要な物を非表示に
            EditNameTextBox.Visible = false;
            EditIDTextBox.Visible = false;
            AllowEditIDButton.Visible = false;
            EditMCTextBox.Visible = false;
            CheckSameMCButton.Visible = false;
            EditRegistrationDateTextBox.Visible = false;
            EditCategoryTextBox.Visible = false;
            EditTag1TextBox.Visible = false;
            EditTag2TextBox.Visible = false;
            EditTag3TextBox.Visible = false;
            EditRealLocationTextBox.Visible = false;
            SelectThumbnailButton.Visible = false;
            OpenPictureFolderButton.Visible = false;
            // 再表示時に編集したデータを表示するための処理
            SearchFormTextBox.TextChanged -= SearchFormTextBox_TextChanged;
            SearchOptionComboBox.SelectedIndexChanged -= SearchOptionComboBox_SelectedIndexChanged;
            SearchFormTextBox.Text = EditIDTextBox.Text;
            SearchOptionComboBox.SelectedIndex = 0;
            SearchFormTextBox.TextChanged += SearchFormTextBox_TextChanged;
            SearchOptionComboBox.SelectedIndexChanged += SearchOptionComboBox_SelectedIndexChanged;
            // 入力フォームをリセット
            ClearDetailsWindowMethod();
            // 通常画面で必要なものを表示
            EditButton.Visible = true;
            // 詳細データおよび機密データを編集不可能に変更
            DetailsTextBox.ReadOnly = true;
            ConfidentialDataTextBox.ReadOnly = true;
            // ID手動設定を不可に変更
            EditIDTextBox.ReadOnly = true;
            ConfigValues.AllowEditID = false;
            AllowEditIDButton.Visible = true;
            UUIDEditStatusLabel.Visible = false;
            // 再度詳細情報を表示
            if (DataLoadingStatus == "true")
            {
                DataLoadingStatus = "stop";
            }
            CurrentProjectSettingValues.ModifiedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            LoadGrid();
            ShowDetails();
        }
        private void DeleteContent()// データ完全削除用のメソッド
        {
            if (CurrentShownDataValues.CollectionFolderPath.Length == 0)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                Directory.Delete(CurrentShownDataValues.CollectionFolderPath, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("データの削除に失敗しました。\n" + ex.Message, "CREC");
                return;
            }
            if (DataLoadingStatus == "true")
            {
                DataLoadingStatus = "stop";
            }
            if (SaveAndCloseEditButton.Visible == true)// 編集中のデータを削除した場合
            {
                // 通常画面に不要な物を非表示に
                EditNameTextBox.Visible = false;
                EditIDTextBox.Visible = false;
                AllowEditIDButton.Visible = false;
                EditMCTextBox.Visible = false;
                CheckSameMCButton.Visible = false;
                EditRegistrationDateTextBox.Visible = false;
                EditCategoryTextBox.Visible = false;
                EditTag1TextBox.Visible = false;
                EditTag2TextBox.Visible = false;
                EditTag3TextBox.Visible = false;
                EditRealLocationTextBox.Visible = false;
                SaveAndCloseEditButton.Visible = false;
                SelectThumbnailButton.Visible = false;
                OpenPictureFolderButton.Visible = false;
                // 入力フォームをリセット
                ClearDetailsWindowMethod();
                // 通常画面で必要なものを表示
                EditButton.Visible = true;
                ShowTag1.Visible = true;
                ShowTag2.Visible = true;
                ShowTag3.Visible = true;
                ShowPicturesButton.Visible = true;
                // 詳細データおよび機密データを編集不可能に変更
                DetailsTextBox.ReadOnly = true;
                ConfidentialDataTextBox.ReadOnly = true;
                // ID手動設定を不可に変更
                EditIDTextBox.ReadOnly = true;
                ConfigValues.AllowEditID = false;
                AllowEditIDButton.Visible = true;
                UUIDEditStatusLabel.Visible = false;
                // 再度詳細情報を表示
                if (DataLoadingStatus == "true")
                {
                    DataLoadingStatus = "stop";
                }
            }
            SearchFormTextBox.Text = string.Empty;
            SearchOptionComboBox.SelectedIndex = 0;
            MessageBox.Show("削除成功", "CREC");
            CurrentProjectSettingValues.ModifiedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            LoadGrid();
            ShowDetails();
        }
        private bool CheckContent()// 入力内容の整合性を確認
        {
            if (System.IO.Directory.Exists(CurrentProjectSettingValues.ProjectDataFolderPath + "\\" + EditIDTextBox.Text))// ID重複確認
            {
                if (CurrentShownDataValues.CollectionFolderPath != CurrentProjectSettingValues.ProjectDataFolderPath + "\\" + EditIDTextBox.Text)
                {
                    UUIDEditStatusLabel.Text = LanguageSettingClass.GetOtherMessage("UUIDChangeNG", "mainform", LanguageFile);
                    UUIDEditStatusLabel.ForeColor = Color.Red;
                    MessageBox.Show("入力されたIDは使用済みです。", "CREC");
                    return false;
                }
            }
            return true;
        }
        private void AllowEditIDButton_Click(object sender, EventArgs e)// ID手動設定の可否
        {
            ConfigValues.AllowEditID = true;
            AllowEditIDButton.Visible = false;
            UUIDEditStatusLabel.Visible = true;
            UUIDEditStatusLabel.Text = LanguageSettingClass.GetOtherMessage("UUIDNoChange", "mainform", LanguageFile);
            UUIDEditStatusLabel.ForeColor = Color.Black;
            EditIDTextBox.ReadOnly = false;
        }
        private void IDTextBox_TextChanged(object sender, EventArgs e)// ID重複確認
        {
            if (CurrentShownDataValues.ID == EditIDTextBox.Text)
            {
                UUIDEditStatusLabel.Text = LanguageSettingClass.GetOtherMessage("UUIDNoChange", "mainform", LanguageFile);
                UUIDEditStatusLabel.ForeColor = Color.Black;
            }
            else if (System.IO.Directory.Exists(CurrentProjectSettingValues.ProjectDataFolderPath + "\\" + EditIDTextBox.Text))
            {
                UUIDEditStatusLabel.Text = LanguageSettingClass.GetOtherMessage("UUIDChangeNG", "mainform", LanguageFile);
                UUIDEditStatusLabel.ForeColor = Color.Red;
            }
            else
            {
                UUIDEditStatusLabel.Text = LanguageSettingClass.GetOtherMessage("UUIDChangeOK", "mainform", LanguageFile);
                UUIDEditStatusLabel.ForeColor = Color.Blue;
            }
        }
        private void CheckSameMCButton_Click(object sender, EventArgs e)// 同一コードを検索
        {
            SearchOptionComboBox.SelectedIndexChanged -= SearchOptionComboBox_SelectedIndexChanged;
            SearchMethodComboBox.SelectedIndexChanged -= SearchMethodComboBox_SelectedIndexChanged;
            SearchFormTextBox.TextChanged -= SearchFormTextBox_TextChanged;
            SearchOptionComboBox.SelectedIndex = 1;
            SearchMethodComboBox.SelectedIndex = 3;
            SearchFormTextBox.Text = EditMCTextBox.Text;
            LoadGrid();
            if (ConfigValues.AutoSearch == true)
            {
                SearchOptionComboBox.SelectedIndexChanged += SearchOptionComboBox_SelectedIndexChanged;
                SearchMethodComboBox.SelectedIndexChanged += SearchMethodComboBox_SelectedIndexChanged;
                SearchFormTextBox.TextChanged += SearchFormTextBox_TextChanged;
            }
            // 同一IDの検索結果を表示
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("SameMCNotFound", "mainform", LanguageFile), "CREC");
            }
            else
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("SameMCExist", "mainform", LanguageFile), "CREC");
            }
        }
        private void AddContentsMethod()// 新規にデータを追加する処理のメソッド
        {
            if (CurrentProjectSettingValues.ProjectDataFolderPath.Length == 0)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (SaveAndCloseEditButton.Visible == true)// 編集中の場合は警告を表示
            {
                if (CheckEditingContents() == false)// キャンセルされた場合
                {
                    return;// 編集モードに切り替えず処理を終了
                }
            }
            dataGridView1.ClearSelection();//　List選択解除
            dataGridView1.CurrentCell = null;//　List選択解除
            CurrentShownDataValues.ID = Convert.ToString(Guid.NewGuid());
            EditIDTextBox.Text = CurrentShownDataValues.ID;// UUIDを入力
            DateTime DT = DateTime.Now;
            if (CurrentProjectSettingValues.ManagementCodeAutoFill == true)
            {
                EditMCTextBox.Text = DT.ToString("yyMMddHHmmssf");// MCを自動入力
            }
            EditRegistrationDateTextBox.Text = DT.ToString("yyyy/MM/dd_HH:mm:ss.f");// 日時を自動入力
            CurrentShownDataValues.CollectionFolderPath = CurrentProjectSettingValues.ProjectDataFolderPath + "\\" + EditIDTextBox.Text;
            // フォルダ及びファイルを作成
            Directory.CreateDirectory(CurrentShownDataValues.CollectionFolderPath);
            Directory.CreateDirectory(CurrentShownDataValues.CollectionFolderPath + "\\data");
            Directory.CreateDirectory(CurrentShownDataValues.CollectionFolderPath + "\\pictures");
            Directory.CreateDirectory(CurrentShownDataValues.CollectionFolderPath + "\\SystemData");
            FileOperationClass.AddBlankFile(CurrentShownDataValues.CollectionFolderPath + "\\index.txt");
            StreamWriter streamWriter = new StreamWriter(CurrentShownDataValues.CollectionFolderPath + "\\index.txt");
            streamWriter.WriteLine(string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7},\n{8}", "名称," + EditNameTextBox.Text, "ID," + EditIDTextBox.Text, "MC," + EditMCTextBox.Text, "登録日," + EditRegistrationDateTextBox.Text, "カテゴリ," + EditCategoryTextBox.Text, "タグ1," + EditTag1TextBox.Text, "タグ2," + EditTag2TextBox.Text, "タグ3," + EditTag3TextBox.Text, "場所1(Real)," + EditRealLocationTextBox.Text));
            streamWriter.Close();
            FileOperationClass.AddBlankFile(CurrentShownDataValues.CollectionFolderPath + "\\details.txt");
            FileOperationClass.AddBlankFile(CurrentShownDataValues.CollectionFolderPath + "\\confidentialdata.txt");
            // 在庫管理を行うか確認
            DialogResult result = MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("AskMakeInventoryManagementFile", "mainform", LanguageFile), "CREC", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                FileOperationClass.AddBlankFile(CurrentShownDataValues.CollectionFolderPath + "\\inventory.inv");
                StreamWriter InventoryManagementFile = new StreamWriter(CurrentShownDataValues.CollectionFolderPath + "\\inventory.inv");
                InventoryManagementFile.WriteLine("{0},,,", EditIDTextBox.Text);
                InventoryManagementFile.Close();
                InventoryManagementModeButton.Visible = true;
                CloseInventoryManagementModeButton.Visible = false;
            }
            else
            {
                InventoryManagementModeButton.Visible = false;
                CloseInventoryManagementModeButton.Visible = false;
            }
            // 新規作成タグを作成
            FileOperationClass.AddBlankFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\ADD");
            // 表示中の内容をクリア
            DetailsTextBox.Text = string.Empty;
            ConfidentialDataTextBox.Text = string.Empty;
            Thumbnail.Image = null;
            NoImageLabel.Visible = true;
            Thumbnail.BackColor = menuStrip1.BackColor;
            StartEditForm();
            if (FullDisplayModeToolStripMenuItem.Checked)// 全画面表示モードの時の処理
            {
                dataGridView1.Visible = false;
                dataGridView1BackgroundPictureBox.Visible = false;
                ShowSelectedItemInformationButton.Visible = false;
                SearchFormTextBox.Visible = false;
                SearchFormTextBoxClearButton.Visible = false;
                SearchOptionComboBox.Visible = false;
                SearchMethodComboBox.Visible = false;
                ShowListButton.Visible = true;
                ShowPicturesMethod();
            }
        }
        private bool SaveContentsMethod()// 入力されたデータを保存する処理のメソッド
        {
            // ID変更処理が必要な場合
            if (CurrentShownDataValues.CollectionFolderPath != CurrentProjectSettingValues.ProjectDataFolderPath + "\\" + EditIDTextBox.Text)
            {
                if (FileOperationClass.MoveFolder(CurrentShownDataValues.CollectionFolderPath, CurrentProjectSettingValues.ProjectDataFolderPath + "\\" + EditIDTextBox.Text))
                {
                    CurrentShownDataValues.CollectionFolderPath = CurrentProjectSettingValues.ProjectDataFolderPath + "\\" + EditIDTextBox.Text;// 移動先のパスで保存処理を続行
                }
                else// 移動に失敗した場合
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("UUIDChangeError", "mainform", LanguageFile), "CREC");
                    return false;
                }
            }
            // フォルダを作成
            Directory.CreateDirectory(CurrentShownDataValues.CollectionFolderPath);
            Directory.CreateDirectory(CurrentShownDataValues.CollectionFolderPath + "\\data");
            Directory.CreateDirectory(CurrentShownDataValues.CollectionFolderPath + "\\pictures");
            // 変更前のデータをバックアップ
            try
            {
                File.Copy(CurrentShownDataValues.CollectionFolderPath + "\\index.txt", CurrentShownDataValues.CollectionFolderPath + "\\index_old.txt", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Indexファイルのバックアップ作成に失敗しました。\n" + ex.Message, "CREC");
            }
            try
            {
                File.Copy(CurrentShownDataValues.CollectionFolderPath + "\\details.txt", CurrentShownDataValues.CollectionFolderPath + "\\details_old.txt", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("詳細データのバックアップ作成に失敗しました。\n" + ex.Message, "CREC");
            }
            try
            {
                File.Copy(CurrentShownDataValues.CollectionFolderPath + "\\confidentialdata.txt", CurrentShownDataValues.CollectionFolderPath + "\\confidentialdata_old.txt", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("機密データのバックアップ作成に失敗しました。\n" + ex.Message, "CREC");
            }
            // Indexデータを保存
            StreamWriter Indexfile = new StreamWriter(CurrentShownDataValues.CollectionFolderPath + "\\index.txt", false, Encoding.GetEncoding("UTF-8"));
            if (EditNameTextBox.Text.Length == 0)
            {
                EditNameTextBox.Text = "　ー　";
            }
            if (EditIDTextBox.Text.Length == 0)
            {
                EditIDTextBox.Text = "　ー　";
            }
            if (EditMCTextBox.Text.Length == 0)
            {
                EditMCTextBox.Text = "　ー　";
            }
            if (EditRegistrationDateTextBox.Text.Length == 0)
            {
                EditRegistrationDateTextBox.Text = "　ー　";
            }
            if (EditCategoryTextBox.Text.Length == 0)
            {
                EditCategoryTextBox.Text = "　ー　";
            }
            if (EditTag1TextBox.Text.Length == 0)
            {
                EditTag1TextBox.Text = "　ー　";
            }
            if (EditTag2TextBox.Text.Length == 0)
            {
                EditTag2TextBox.Text = "　ー　";
            }
            if (EditTag3TextBox.Text.Length == 0)
            {
                EditTag3TextBox.Text = "　ー　";
            }
            if (EditRealLocationTextBox.Text.Length == 0)
            {
                EditRealLocationTextBox.Text = "　ー　";
            }
            Indexfile.WriteLine(string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}",
                "名称," + EditNameTextBox.Text.Replace("\r", string.Empty).Replace("\n", string.Empty),
                "ID," + EditIDTextBox.Text.Replace("\r", string.Empty).Replace("\n", string.Empty),
                "MC," + EditMCTextBox.Text.Replace("\r", string.Empty).Replace("\n", string.Empty),
                "登録日," + EditRegistrationDateTextBox.Text.Replace("\r", string.Empty).Replace("\n", string.Empty),
                "カテゴリ," + EditCategoryTextBox.Text.Replace("\r", string.Empty).Replace("\n", string.Empty),
                "タグ1," + EditTag1TextBox.Text.Replace("\r", string.Empty).Replace("\n", string.Empty),
                "タグ2," + EditTag2TextBox.Text.Replace("\r", string.Empty).Replace("\n", string.Empty),
                "タグ3," + EditTag3TextBox.Text.Replace("\r", string.Empty).Replace("\n", string.Empty),
                "場所1(Real)," + EditRealLocationTextBox.Text.Replace("\r", string.Empty).Replace("\n", string.Empty)));
            Indexfile.Close();
            // 詳細データの保存
            StreamWriter Detailsfile = new StreamWriter(CurrentShownDataValues.CollectionFolderPath + "\\details.txt", false, Encoding.GetEncoding("UTF-8"));
            Detailsfile.Write(string.Format("{0}", DetailsTextBox.Text));
            Detailsfile.Close();
            // 機密データの保存
            StreamWriter ConfidentialDataFile = new StreamWriter(CurrentShownDataValues.CollectionFolderPath + "\\confidentialdata.txt", false, Encoding.GetEncoding("UTF-8"));
            ConfidentialDataFile.Write(string.Format("{0}", ConfidentialDataTextBox.Text));
            ConfidentialDataFile.Close();
            // サムネ画像が更新されていた場合は上書きしキャッシュを削除
            if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\pictures\\Thumbnail1.newjpg"))
            {
                File.Copy(CurrentShownDataValues.CollectionFolderPath + "\\pictures\\Thumbnail1.newjpg", CurrentShownDataValues.CollectionFolderPath + "\\pictures\\Thumbnail1.jpg", true);
                FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\pictures\\Thumbnail1.newjpg");
            }
            if (PictureBox1.Visible == true)// 詳細画像表示されている場合は読み込み
            {
                ShowPicturesMethod();
            }
            // 編集中タグを削除・解放をマーク
            if (!Directory.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData"))
            {
                Directory.CreateDirectory(CurrentShownDataValues.CollectionFolderPath + "\\SystemData");
            }
            FileOperationClass.AddBlankFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\FREE");
            FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED");
            FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED");
            FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\ADD");

            if (CurrentProjectSettingValues.EditListOutput == true)
            {
                ListOutputMethod(CurrentProjectSettingValues.ListOutputFormat);
            }
            if (CurrentProjectSettingValues.EditBackUp == true)
            {
                BackUpMethod();
                MakeBackUpZip();// ZIP圧縮を非同期で開始
            }
            return true;
        }
        private void AddContentsButton_Click(object sender, EventArgs e)// データ追加ボタン
        {
            AddContentsMethod();// 新規にデータを追加するメソッドを呼び出し
        }
        private void ListUpdateButton_Click(object sender, EventArgs e)// 一覧更新ボタン
        {
            if (DataLoadingStatus == "true")
            {
                DataLoadingStatus = "stop";
            }
            LoadGrid();// 一覧を再度読み込み
        }
        #endregion

        #region 在庫数・適正在庫数管理関係
        string[] rowsIM;// .invを1行ごとに読み込んだ内容（在庫管理用）
        int? SafetyStock;// 安全在庫数、未定義時はnullを使用
        int? ReorderPoint;// 発注点、未定義時はnullを使用
        int? MaximumLevel;// 最大在庫数、未定義時はnullを使用
        int inventory = 0;// 在庫数計算用
        private void InventoryManagementModeButton_Click(object sender, EventArgs e)// 在庫管理モード開始
        {
            string reader;
            string row;
            string[] cols;
            if (TargetCRECPath.Length == 0)// プロジェクトが開かれていない場合のエラー
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (CurrentShownDataValues.CollectionFolderPath.Length == 0)
            {
                MessageBox.Show("表示するデータを選択し、詳細表示してください。", "CREC");
                return;
            }
            //invからデータを読み込んで表示
            try
            {
                reader = File.ReadAllText(CurrentShownDataValues.CollectionFolderPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
                rowsIM = reader.Trim().Replace("\r", string.Empty).Split('\n');
            }
            catch (Exception ex)
            {
                MessageBox.Show("在庫管理データの読み込みに失敗しました。\n" + ex.Message, "CREC");
                return;
            }
            // 不要なものを非表示に
            ClosePicturesViewMethod();// 画像表示モードを閉じるメソッドを呼び出し
            dataGridView1.Visible = false;
            SearchFormTextBox.Visible = false;
            SearchOptionComboBox.Visible = false;
            SearchMethodComboBox.Visible = false;
            SearchFormTextBoxClearButton.Visible = false;
            AddContentsButton.Visible = false;
            ListUpdateButton.Visible = false;
            InventoryManagementModeButton.Visible = false;
            SearchButton.Visible = false;
            // 必要なものを表示
            InventoryLabel.Visible = true;
            InventoryModeDataGridView.Visible = true;
            OperationOptionComboBox.Visible = true;
            EditQuantityTextBox.Visible = true;
            AddInventoryOperationButton.Visible = true;
            InventoryOperationLabel.Visible = true;
            InputQuantitiyLabel.Visible = true;
            AddQuantityButton.Visible = true;
            SubtractQuantityButton.Visible = true;
            InventoryOperationNoteLabel.Visible = true;
            EditInventoryOperationNoteTextBox.Visible = true;
            ProperInventorySettingsComboBox.Visible = true;
            ProperInventorySettingsTextBox.Visible = true;
            SetProperInventorySettingsButton.Visible = true;
            ProperInventorySettingsNotificationLabel.Visible = true;
            CloseInventoryManagementModeButton.Visible = true;
            // DataGridView関係
            InventoryModeDataGridView.Rows.Clear();
            InventoryModeDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            InventoryModeDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            // 行数を確認
            string[] tmp = File.ReadAllLines(CurrentShownDataValues.CollectionFolderPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
            // 1行目を読み込み
            row = rowsIM[0];
            cols = row.Split(',');
            if (cols[1].Length != 0)
            {
                SafetyStock = Convert.ToInt32(cols[1]);
            }
            if (cols[2].Length != 0)
            {
                ReorderPoint = Convert.ToInt32(cols[2]);
            }
            if (cols[3].Length != 0)
            {
                MaximumLevel = Convert.ToInt32(cols[3]);
            }
            inventory = 0;// 初期化
            for (int i = 1; i <= Convert.ToInt32(tmp.Length) - 1; i++)// 2行目以降を読み込み
            {
                row = rowsIM[i];
                cols = row.Split(',');
                string InventoryOperation = string.Empty;
                switch (cols[1])
                {
                    case "EntoryOperation":
                    case "入庫"://後方互換用
                        InventoryOperation = LanguageSettingClass.GetOtherMessage("EntoryOperation", "mainform", LanguageFile);
                        break;
                    case "ExitOperation":
                    case "出庫"://後方互換用
                        InventoryOperation = LanguageSettingClass.GetOtherMessage("ExitOperation", "mainform", LanguageFile);
                        break;
                    case "Stocktaking":
                    case "棚卸"://後方互換用
                        InventoryOperation = LanguageSettingClass.GetOtherMessage("Stocktaking", "mainform", LanguageFile);
                        break;
                }
                InventoryModeDataGridView.Rows.Add(cols[0], InventoryOperation, cols[2], cols[3]);
                inventory = inventory + Convert.ToInt32(cols[2]);
                InventoryLabel.Text = Convert.ToString(LanguageSettingClass.GetOtherMessage("Inventory", "mainform", LanguageFile) + " : " + inventory);
            }
            if (inventory < 0)
            {
                MessageBox.Show("在庫数がマイナスです。\n現在個数を確認してください", "CREC");
            }
            ProperInventorySettingsComboBox.SelectedIndex = 0;
            if (SafetyStock == null)
            {
                ProperInventorySettingsTextBox.TextChanged -= ProperInventorySettingsTextBox_TextChanged;// 適正在庫管理の入力イベントを停止
                ProperInventorySettingsTextBox.Text = " - ";
                ProperInventorySettingsTextBox.TextChanged += ProperInventorySettingsTextBox_TextChanged;// 適正在庫管理の入力イベントを再開
            }
            else
            {
                ProperInventorySettingsTextBox.Text = Convert.ToString(SafetyStock);
            }
            ProperInventoryNotification();// 適正在庫設定と比較
        }
        private void CloseInventoryManagementModeButton_Click(object sender, EventArgs e)// 在庫管理モード終了
        {

            CloseInventoryViewMethod();// 在庫管理モードを閉じるメソッドを呼び出し
            // 必要なものを表示
            InventoryManagementModeButton.Visible = true;
            if (StandardDisplayModeToolStripMenuItem.Checked)
            {
                dataGridView1.Visible = true;
                SearchFormTextBox.Visible = true;
                SearchOptionComboBox.Visible = true;
                SearchMethodComboBox.Visible = true;
                SearchFormTextBoxClearButton.Visible = true;
                AddContentsButton.Visible = true;
                ListUpdateButton.Visible = true;
                if (ConfigValues.AutoSearch == false)
                {
                    SearchButton.Visible = true;
                }
                if (DataLoadingStatus == "true")
                {
                    DataLoadingStatus = "stop";
                }
            }
            else if (FullDisplayModeToolStripMenuItem.Checked)// 全画面表示モードの時は写真を表示
            {
                ShowPicturesMethod();
            }
        }
        private void AddInventoryOperationButton_Click(object sender, EventArgs e)// 在庫の増減を保存
        {
            if (EditQuantityTextBox.Text.Length < 1 || EditQuantityTextBox.Text == "-")// 数量が未入力の場合
            {
                MessageBox.Show("数量が入力されていません。", "CREC");
                return;
            }
            if (OperationOptionComboBox.SelectedIndex == -1)// 操作内容が未選択の場合
            {
                MessageBox.Show("作業内容が入力されていません。", "CREC");
                return;
            }
            // 行数を確認
            string[] tmp = File.ReadAllLines(CurrentShownDataValues.CollectionFolderPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
            string InventoryOperation = string.Empty;
            switch (OperationOptionComboBox.SelectedIndex)// 入力されたデータの整合性チェック
            {
                case 0:
                    if (Convert.ToInt32(EditQuantityTextBox.Text) > 0)
                    {
                        InventoryOperation = "EntoryOperation";
                    }
                    else
                    {
                        MessageBox.Show("入庫が選択されています。\n0より大きい値を入力してください", "CREC");
                        return;
                    }
                    break;
                case 1:
                    if (Convert.ToInt32(EditQuantityTextBox.Text) < 0)
                    {
                        InventoryOperation = "ExitOperation";
                    }
                    else
                    {
                        MessageBox.Show("出庫が選択されています\n0より小さい値を入力してください", "CREC");
                        return;
                    }
                    break;
                case 2:
                    InventoryOperation = "Stocktaking";
                    break;
            }
            StreamWriter sw = new StreamWriter(CurrentShownDataValues.CollectionFolderPath + "\\inventory.inv", false, Encoding.GetEncoding("UTF-8"));
            for (int i = 0; i <= Convert.ToInt32(tmp.Length); i++)
            {
                if (i == 0)// .invのヘッダをコピー
                {
                    sw.WriteLine(rowsIM[0]);
                }
                else if (i == 1)// 追加されたデータを先頭に追加
                {
                    // 現在時刻を取得 
                    DateTime DT = DateTime.Now;
                    sw.WriteLine("{0},{1},{2},{3}", DT.ToString("yyyy/MM/dd hh:mm:ss"), InventoryOperation, EditQuantityTextBox.Text, EditInventoryOperationNoteTextBox.Text);
                }
                else// 既存のデータを順に書き込み
                {
                    sw.WriteLine(rowsIM[i - 1]);
                }
            }
            sw.Close();
            OperationOptionComboBox.Refresh();
            EditQuantityTextBox.ResetText();
            EditInventoryOperationNoteTextBox.ResetText();
            //invからデータを読み込んで再表示
            InventoryModeDataGridView.Rows.Clear();
            string reader;
            string row;
            string[] cols;
            reader = File.ReadAllText(CurrentShownDataValues.CollectionFolderPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
            rowsIM = reader.Trim().Replace("\r", string.Empty).Split('\n');
            // 行数を確認
            tmp = File.ReadAllLines(CurrentShownDataValues.CollectionFolderPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
            // 1行目を読み込み
            row = rowsIM[0];
            inventory = 0;
            for (int i = 1; i <= Convert.ToInt32(tmp.Length) - 1; i++)
            {
                row = rowsIM[i];
                cols = row.Split(',');
                switch (cols[1])
                {
                    case "EntoryOperation":
                    case "入庫"://後方互換用
                        InventoryOperation = LanguageSettingClass.GetOtherMessage("EntoryOperation", "mainform", LanguageFile);
                        break;
                    case "ExitOperation":
                    case "出庫"://後方互換用
                        InventoryOperation = LanguageSettingClass.GetOtherMessage("ExitOperation", "mainform", LanguageFile);
                        break;
                    case "Stocktaking":
                    case "棚卸"://後方互換用
                        InventoryOperation = LanguageSettingClass.GetOtherMessage("Stocktaking", "mainform", LanguageFile);
                        break;
                }
                InventoryModeDataGridView.Rows.Add(cols[0], InventoryOperation, cols[2], cols[3]);
                inventory = inventory + Convert.ToInt32(cols[2]);
                InventoryLabel.Text = Convert.ToString(LanguageSettingClass.GetOtherMessage("Inventory", "mainform", LanguageFile) + " : " + inventory);
            }
            if (inventory < 0)
            {
                MessageBox.Show("在庫数がマイナスです。\n現在個数を確認してください", "CREC");
            }
            CurrentProjectSettingValues.ModifiedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            ProperInventoryNotification();// 適正在庫設定と比較
        }
        private void ProperInventorySettingsComboBox_SelectedIndexChanged(object sender, EventArgs e)// 適正在庫の表示項目を選択および表示
        {
            ProperInventorySettingsTextBox.TextChanged -= ProperInventorySettingsTextBox_TextChanged;// 適正在庫管理の入力イベントを停止
            switch (ProperInventorySettingsComboBox.SelectedIndex)
            {
                case 0:
                    if (SafetyStock == null)
                    {
                        if (ProperInventorySettingsTextBox.ReadOnly == true)
                        {
                            ProperInventorySettingsTextBox.Text = " - ";
                        }
                        else if (ProperInventorySettingsTextBox.ReadOnly == false)
                        {
                            ProperInventorySettingsTextBox.Text = string.Empty;
                        }
                    }
                    else
                    {
                        ProperInventorySettingsTextBox.Text = SafetyStock.ToString();
                    }
                    break;
                case 1:
                    if (ReorderPoint == null)
                    {
                        if (ProperInventorySettingsTextBox.ReadOnly == true)
                        {
                            ProperInventorySettingsTextBox.Text = " - ";
                        }
                        else if (ProperInventorySettingsTextBox.ReadOnly == false)
                        {
                            ProperInventorySettingsTextBox.Text = string.Empty;
                        }
                    }
                    else
                    {
                        ProperInventorySettingsTextBox.Text = ReorderPoint.ToString();
                    }
                    break;
                case 2:
                    if (MaximumLevel == null)
                    {
                        if (ProperInventorySettingsTextBox.ReadOnly == true)
                        {
                            ProperInventorySettingsTextBox.Text = " - ";
                        }
                        else if (ProperInventorySettingsTextBox.ReadOnly == false)
                        {
                            ProperInventorySettingsTextBox.Text = string.Empty;
                        }
                    }
                    else
                    {
                        ProperInventorySettingsTextBox.Text = MaximumLevel.ToString();
                    }
                    break;
            }
            ProperInventorySettingsTextBox.TextChanged += ProperInventorySettingsTextBox_TextChanged;// 適正在庫管理の入力イベントを再開
            CurrentProjectSettingValues.ModifiedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }
        private void SaveProperInventorySettingsButton_Click(object sender, EventArgs e)// 適正在庫の設定保存
        {
            ProperInventorySettingsTextBox.ReadOnly = true;
            SaveProperInventorySettingsButton.Visible = false;
            SetProperInventorySettingsButton.Visible = true;
            // 行数を確認
            string[] tmp = File.ReadAllLines(CurrentShownDataValues.CollectionFolderPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
            StreamWriter streamWriter;
            streamWriter = new StreamWriter(CurrentShownDataValues.CollectionFolderPath + "\\inventory.inv", false, Encoding.GetEncoding("UTF-8"));
            try
            {
                for (int i = 0; i < Convert.ToInt32(tmp.Length); i++)
                {
                    if (i == 0)// .invのヘッダをコピー
                    {
                        string row;
                        row = rowsIM[i];
                        cols = row.Split(',');
                        streamWriter.WriteLine("{0},{1},{2},{3}", cols[0], SafetyStock, ReorderPoint, MaximumLevel);
                    }
                    else// 既存のデータを順に書き込み
                    {
                        streamWriter.WriteLine(rowsIM[i]);
                    }
                }
                ProperInventoryNotification();
                CurrentProjectSettingValues.ModifiedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CREC");
            }
            finally
            {
                streamWriter.Close();
            }
        }
        private void SetProperInventorySettingsButton_Click(object sender, EventArgs e)// 適正在庫の設定変更モード開始
        {
            ProperInventorySettingsTextBox.ReadOnly = false;
            SaveProperInventorySettingsButton.Visible = true;
            SetProperInventorySettingsButton.Visible = false;
            if (ProperInventorySettingsTextBox.Text == "未定義" || ProperInventorySettingsTextBox.Text == " - ")// 未定義は多言語対応前の後方互換用
            {
                ProperInventorySettingsTextBox.Text = string.Empty;
            }
            CurrentProjectSettingValues.ModifiedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }
        private void ProperInventorySettingsTextBox_TextChanged(object sender, EventArgs e)// 入力された内容をリアルタイムで反映
        {
            switch (ProperInventorySettingsComboBox.SelectedIndex)// コンボボックスの選択内容に合わせて保存先を自動選択
            {
                case 0:
                    if (ProperInventorySettingsTextBox.Text != string.Empty)
                    {
                        SafetyStock = Convert.ToInt32(ProperInventorySettingsTextBox.Text);
                    }
                    break;
                case 1:
                    if (ProperInventorySettingsTextBox.Text != string.Empty)
                    {
                        ReorderPoint = Convert.ToInt32(ProperInventorySettingsTextBox.Text);
                    }
                    break;
                case 2:
                    if (ProperInventorySettingsTextBox.Text != string.Empty)
                    {
                        MaximumLevel = Convert.ToInt32(ProperInventorySettingsTextBox.Text);
                    }
                    break;
            }
        }
        private void ProperInventoryNotification()// 適正在庫設定と比較して通知を表示
        {
            ProperInventorySettingsNotificationLabel.Text = string.Empty;
            if (inventory < SafetyStock)
            {
                MessageBox.Show("安全在庫数を下回っています。", "CREC");
                ProperInventorySettingsNotificationLabel.Text = "安全在庫数を下回っています。";
            }
            if (SafetyStock <= inventory && inventory <= ReorderPoint)
            {
                MessageBox.Show("在庫数が発注点以下です。", "CREC");
                ProperInventorySettingsNotificationLabel.Text = "在庫数が発注点以下です。";
            }
            if (MaximumLevel < inventory)
            {
                MessageBox.Show("過剰在庫状態です。", "CREC");
                ProperInventorySettingsNotificationLabel.Text = "過剰在庫状態です。";
            }
        }
        private void ProperInventorySettingsTextBox_KeyPress(object sender, KeyPressEventArgs e)// 適正在庫入力用TextBoxを数字のみの入力に制限
        {
            if (e.KeyChar == '\b')
            {
                return;
            }
            else if ((e.KeyChar < '0' || '9' < e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void EditQuantityTextBox_KeyPress(object sender, KeyPressEventArgs e)// 出入庫個数入力用TextBoxを数字のみの入力に制限
        {
            if (e.KeyChar == '\b')
            {
                return;
            }
            else if (e.KeyChar == '-')
            {
                return;
            }
            else if ((e.KeyChar < '0' || '9' < e.KeyChar))
            {
                e.Handled = true;
            }
        }
        /// <summary>
        /// 在庫操作数を＋１する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddQuantityButton_Click(object sender, EventArgs e)
        {
            AddSubtractQuantity(true);
        }
        /// <summary>
        /// 在庫操作数を-１する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubtractQuantityButton_Click(object sender, EventArgs e)
        {
            AddSubtractQuantity(false);
        }
        /// <summary>
        /// 在庫操作数を加算・減算する処理
        /// </summary>
        /// <param name="Add">加算するときはtrue</param>
        private void AddSubtractQuantity(bool Add)
        {
            // 現在の在庫操作数を取得
            long CurrentQuantity = 0;
            if (EditQuantityTextBox.Text != string.Empty)// TextBoxに値が入っている場合は取得
            {
                try
                {
                    CurrentQuantity = Convert.ToInt64(EditQuantityTextBox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "CREC");
                }
            }
            // 加算・減算処理
            try
            {
                CurrentQuantity += (int)Math.Pow(-1, 1 + Convert.ToInt32(Add));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CREC");
            }
            // TextBoxに入れる
            EditQuantityTextBox.Text = Convert.ToString(CurrentQuantity);
        }
        private void CloseInventoryViewMethod()// 在庫表示モードを閉じるメソッド
        {
            CloseInventoryManagementModeButton.Visible = false;
            InventoryLabel.Visible = false;
            InventoryModeDataGridView.Visible = false;
            OperationOptionComboBox.Visible = false;
            EditQuantityTextBox.Visible = false;
            AddInventoryOperationButton.Visible = false;
            InventoryOperationLabel.Visible = false;
            InputQuantitiyLabel.Visible = false;
            AddQuantityButton.Visible = false;
            SubtractQuantityButton.Visible = false;
            InventoryOperationNoteLabel.Visible = false;
            EditInventoryOperationNoteTextBox.Visible = false;
            ProperInventorySettingsComboBox.Visible = false;
            ProperInventorySettingsTextBox.Visible = false;
            SaveProperInventorySettingsButton.Visible = false;
            SetProperInventorySettingsButton.Visible = false;
            ProperInventorySettingsNotificationLabel.Visible = false;
            if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\inventory.inv"))
            {
                InventoryManagementModeButton.Visible = true;
            }
            // 表示内容・変数をリセット
            InventoryLabel.ResetText();
            ProperInventorySettingsTextBox.ResetText();
            SafetyStock = null;
            ReorderPoint = null;
            MaximumLevel = null;
        }
        #endregion

        #region 検索関係
        private void SetTagNameToolTips()// タグのToolTip(説明)
        {
            TagNameToolTip.SetToolTip(Tag1NameLabel, Tag1NameLabel.Text);
            TagNameToolTip.SetToolTip(Tag2NameLabel, Tag2NameLabel.Text);
            TagNameToolTip.SetToolTip(Tag3NameLabel, Tag3NameLabel.Text);
        }
        private void SearchFormTextBox_TextChanged(object sender, EventArgs e)// 検索窓が更新されるごとに再度一覧を読み込んで更新
        {
            if (CurrentProjectSettingValues.ProjectDataFolderPath.Length != 0)
            {
                // 現時点での文字列を保存

                // 検索
                if (DataLoadingStatus == "true")
                {
                    DataLoadingStatus = "stop";
                }
                LoadGrid();// 再度読み込み
            }
        }
        private void SearchOptionComboBox_SelectedIndexChanged(object sender, EventArgs e)// 検索対象が変更された場合に一覧を読み込んで更新
        {
            CurrentProjectSettingValues.SearchOptionNumber = SearchOptionComboBox.SelectedIndex;
            SearchMethodComboBox.SelectedIndexChanged -= SearchMethodComboBox_SelectedIndexChanged;
            SearchMethodComboBox.Items.Clear();
            if (SearchOptionComboBox.SelectedIndex == 7)
            {
                SearchMethodComboBox.Items.Add("欠品");
                SearchMethodComboBox.Items.Add("不足");
                SearchMethodComboBox.Items.Add("適正");
                SearchMethodComboBox.Items.Add("過剰");
                SearchFormTextBox.Clear();
            }
            else
            {
                SearchMethodComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("SearchMethodForwardMatch", "mainform", LanguageFile));
                SearchMethodComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("SearchMethodBroadMatch", "mainform", LanguageFile));
                SearchMethodComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("SearchMethodBackwardMatch", "mainform", LanguageFile));
                SearchMethodComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("SearchMethodExactMatch", "mainform", LanguageFile));
            }
            SearchMethodComboBox.SelectedIndex = 0;
            CurrentProjectSettingValues.SearchMethodNumber = SearchMethodComboBox.SelectedIndex;
            SearchMethodComboBox.SelectedIndexChanged += SearchMethodComboBox_SelectedIndexChanged;
            if (SearchOptionComboBox.SelectedIndex == 7)
            {
                if (DataLoadingStatus == "true")
                {
                    DataLoadingStatus = "stop";
                }
                LoadGrid();// 再度読み込み
            }
            else if (CurrentProjectSettingValues.ProjectDataFolderPath.Length != 0)
            {
                if (DataLoadingStatus == "true")
                {
                    DataLoadingStatus = "stop";
                }
                LoadGrid();// 再度読み込み
            }
        }
        private void SearchMethodComboBox_SelectedIndexChanged(object sender, EventArgs e)// 検索メソッドが変更された場合に一覧を読み込んで更新
        {
            CurrentProjectSettingValues.SearchMethodNumber = SearchMethodComboBox.SelectedIndex;
            if (CurrentProjectSettingValues.ProjectDataFolderPath.Length != 0)
            {
                if (DataLoadingStatus == "true")
                {
                    DataLoadingStatus = "stop";
                }
                LoadGrid();// 再度読み込み
            }
        }
        private void SearchButton_Click(object sender, EventArgs e)// 検索ボタン、自動検索OFF時に使用
        {
            if (CurrentProjectSettingValues.ProjectDataFolderPath.Length != 0)
            {
                // 検索
                if (DataLoadingStatus == "true")
                {
                    DataLoadingStatus = "stop";
                }
                LoadGrid();// 再度読み込み
            }
        }
        private void SearchFormTextBoxClearButton_Click(object sender, EventArgs e)// 検索窓の入力内容をクリア
        {
            SearchFormTextBox.TextChanged -= SearchFormTextBox_TextChanged;
            SearchFormTextBox.Clear();
            if (ConfigValues.AutoSearch == true)// 自動検索が有効な場合
            {
                SearchFormTextBox.TextChanged += SearchFormTextBox_TextChanged;
                LoadGrid();// 再度読み込み
            }
        }
        private bool SearchMethod(string Keywords)// 検索窓の入力内容とキーワードが指定の検索方法で一致するか判定
        {
            switch (SearchMethodComboBox.SelectedIndex)
            {
                case 0:
                    if (SearchOptionComboBox.SelectedIndex == 7)
                    {
                        if (Keywords == "欠品")
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Keywords.StartsWith(SearchFormTextBox.Text))// 前方一致
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case 1:
                    if (SearchOptionComboBox.SelectedIndex == 7)
                    {
                        if (Keywords == "不足")
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Keywords.Contains(SearchFormTextBox.Text))// 部分一致
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case 2:
                    if (SearchOptionComboBox.SelectedIndex == 7)
                    {
                        if (Keywords == "適正")
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Keywords.EndsWith(SearchFormTextBox.Text))// 後方一致
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case 3:
                    if (SearchOptionComboBox.SelectedIndex == 7)
                    {
                        if (Keywords == "過剰")
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (Keywords == SearchFormTextBox.Text)// 完全一致
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                default:
                    return false;
            }
        }
        #endregion

        #region Config読み込み・保存
        private readonly string ConfigFile = "config.sys";
        private void ImportConfig()// config.sysの読み込み
        {
            // 指定されていなかった場合のために初期化
            CurrentLanguageFileName = "Japanese";
            if (File.Exists(ConfigFile))
            {
                IEnumerable<string> tmp = null;
                tmp = File.ReadLines(ConfigFile, Encoding.GetEncoding("UTF-8"));
                foreach (string line in tmp)
                {
                    cols = line.Split(',');
                    switch (cols[0])
                    {
                        case "AllowEdit":
                            if (cols[1] == "true")
                            {
                                ConfigValues.AllowEdit = true;
                            }
                            else
                            {
                                ConfigValues.AllowEdit = false;
                            }
                            break;
                        case "ShowConfidentialData":
                            if (cols[1] == "true")
                            {
                                ConfigValues.ShowConfidentialData = true;
                            }
                            else
                            {
                                ConfigValues.ShowConfidentialData = false;
                            }
                            break;
                        case "ShowUserAssistToolTips":
                            if (cols[1] == "true")
                            {
                                ConfigValues.ShowUserAssistToolTips = true;
                            }
                            else
                            {
                                ConfigValues.ShowUserAssistToolTips = false;
                            }
                            break;
                        case "AutoLoadProject":
                            if (File.Exists(cols[1]))
                            {
                                if (TargetCRECPath.Length == 0)
                                {
                                    TargetCRECPath = cols[1];//CREC起動時のみ読み込み
                                }
                                ConfigValues.AutoLoadProjectPath = cols[1];
                            }
                            else if (cols[1].Length == 0)
                            {
                                ConfigValues.AutoLoadProjectPath = string.Empty;
                            }
                            else
                            {
                                MessageBox.Show("自動読み込み設定されたプロジェクトが見つかりません。", "CREC");
                                TargetCRECPath = string.Empty;
                                ConfigValues.AutoLoadProjectPath = string.Empty;
                            }
                            break;
                        case "OpenLastTimeProject":
                            if (cols[1] == "true")
                            {
                                ConfigValues.OpenLastTimeProject = true;
                            }
                            else
                            {
                                ConfigValues.OpenLastTimeProject = false;
                            }
                            break;
                        case "AutoSearch":
                            if (cols[1] == "true")
                            {
                                SearchButton.Visible = false;
                                ConfigValues.AutoSearch = true;
                                SearchFormTextBox.TextChanged += SearchFormTextBox_TextChanged;
                                SearchOptionComboBox.SelectedIndexChanged += SearchOptionComboBox_SelectedIndexChanged;
                                SearchMethodComboBox.SelectedIndexChanged += SearchMethodComboBox_SelectedIndexChanged;
                            }
                            else
                            {
                                SearchButton.Visible = true;
                                ConfigValues.AutoSearch = false;
                                SearchFormTextBox.TextChanged -= SearchFormTextBox_TextChanged;
                                SearchOptionComboBox.SelectedIndexChanged -= SearchOptionComboBox_SelectedIndexChanged;
                                SearchMethodComboBox.SelectedIndexChanged -= SearchMethodComboBox_SelectedIndexChanged;
                            }
                            break;
                        case "RecentShownContents":
                            if (cols[1] == "true")
                            {
                                ConfigValues.RecentShownContents = true;
                            }
                            else
                            {
                                ConfigValues.RecentShownContents = false;
                            }
                            break;
                        case "BootUpdateCheck":
                            if (cols[1] == "true")
                            {
                                ConfigValues.BootUpdateCheck = true;
                            }
                            else
                            {
                                ConfigValues.BootUpdateCheck = false;
                            }
                            break;
                        case "Language":
                            if (cols[1].Length == 0)
                            {
                                CurrentLanguageFileName = "Japanese.xml";
                            }
                            else
                            {
                                CurrentLanguageFileName = cols[1];
                            }
                            break;
                        case "FontsizeOffset":
                            if (cols[1].Length == 0)
                            {
                                ConfigValues.FontsizeOffset = 0;
                            }
                            else
                            {
                                ConfigValues.FontsizeOffset = Convert.ToInt32(cols[1]);
                            }
                            break;
                    }
                }
            }
            else
            {
                MessageBox.Show("設定ファイルが見つかりません。\nデフォルト設定で起動します。", "CREC");
                //Config.sysの作成
                SaveConfig();
            }
        }
        private void SaveConfig()// config.sysの保存
        {
            StreamWriter configfile = new StreamWriter(Directory.GetCurrentDirectory() + "\\config.sys", false, Encoding.GetEncoding("UTF-8"));
            try
            {
                if (ConfigValues.AllowEdit == true)
                {
                    configfile.WriteLine("AllowEdit,true");
                }
                else
                {
                    configfile.WriteLine("AllowEdit,false");
                }
                if (ConfigValues.ShowConfidentialData == true)
                {
                    configfile.WriteLine("ShowConfidentialData,true");
                }
                else
                {
                    configfile.WriteLine("ShowConfidentialData,false");
                }
                if (ConfigValues.ShowUserAssistToolTips == true)
                {
                    configfile.WriteLine("ShowUserAssistToolTips,true");
                }
                else
                {
                    configfile.WriteLine("ShowUserAssistToolTips,false");
                }
                if (ConfigValues.OpenLastTimeProject == true)
                {
                    configfile.WriteLine("AutoLoadProject,{0}", TargetCRECPath);
                    configfile.WriteLine("OpenLastTimeProject,true");
                }
                else
                {
                    configfile.WriteLine("AutoLoadProject,{0}", ConfigValues.AutoLoadProjectPath);
                    configfile.WriteLine("OpenLastTimeProject,false");
                }
                if (ConfigValues.AutoSearch == true)
                {
                    configfile.WriteLine("AutoSearch,true");
                }
                else
                {
                    configfile.WriteLine("AutoSearch,false");
                }
                if (ConfigValues.RecentShownContents == true)
                {
                    configfile.WriteLine("RecentShownContents,true");
                }
                else
                {
                    configfile.WriteLine("RecentShownContents,false");
                }
                if (ConfigValues.BootUpdateCheck == true)
                {
                    configfile.WriteLine("BootUpdateCheck,true");
                }
                else
                {
                    configfile.WriteLine("BootUpdateCheck,false");
                }
                configfile.WriteLine("Language,{0}", CurrentLanguageFileName);
                configfile.WriteLine("FontsizeOffset,{0}", ConfigValues.FontsizeOffset);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Config.sysの作成に失敗しました(´・ω・｀)\n" + ex.Message, "CREC");
            }
            finally
            {
                configfile.Close();
            }
        }
        #endregion

        #region 画面サイズ変更時のコントロールサイズ更新処理
        private void Form1_SizeChanged(object sender, EventArgs e)// 画面サイズ変更時にコントロールサイズ更新処理を実行
        {
            SetFormLayout();
        }
        private void SetFormLayout()// コントロールサイズ更新処理
        {
            // 処理がダサい。インスタンスを使うと良いらしい（調べておく）
            string fontname = "Meiryo UI";// フォント指定 
            float DpiScale = (float)CurrentDPI;// DPI取得
            Size FormSize = Size;// フォームサイズを取得
            // フォームの最小サイズを変更
            this.MinimumSize = new Size(Convert.ToInt32(1280 * DpiScale), Convert.ToInt32(640 * DpiScale));
            if (StandardDisplayModeToolStripMenuItem.Checked)// 通常表示モードの時は非表示
            {
                dataGridView1.Width = Convert.ToInt32(FormSize.Width * 0.5 - 20 * DpiScale);
                dataGridView1BackgroundPictureBox.Width = 0;
                dataGridView1BackgroundPictureBox.Height = 0;
                dataGridView1BackgroundPictureBox.Location = new System.Drawing.Point(0, Convert.ToInt32(35 * DpiScale));
                ShowSelectedItemInformationButton.Location = new Point(Convert.ToInt32(ListUpdateButton.Location.X + ListUpdateButton.Width + 30 * DpiScale), ListUpdateButton.Location.Y);
                ShowSelectedItemInformationButton.Font = new Font(fontname, smallfontsize);
                ShowSelectedItemInformationButton.Height = ListUpdateButton.Height;
            }
            else if (FullDisplayModeToolStripMenuItem.Checked)
            {
                dataGridView1.Width = Convert.ToInt32(FormSize.Width - 40 * DpiScale);
                dataGridView1BackgroundPictureBox.Width = Convert.ToInt32(FormSize.Width);
                dataGridView1BackgroundPictureBox.Height = Convert.ToInt32(FormSize.Height - 35 * DpiScale);
                dataGridView1BackgroundPictureBox.Location = new System.Drawing.Point(0, Convert.ToInt32(35 * DpiScale));
                ShowSelectedItemInformationButton.Location = new Point(Convert.ToInt32(ListUpdateButton.Location.X + ListUpdateButton.Width + 30 * DpiScale), ListUpdateButton.Location.Y);
                ShowSelectedItemInformationButton.Font = new Font(fontname, smallfontsize);
                ShowSelectedItemInformationButton.Height = ListUpdateButton.Height;
            }
            // 編集TextBox関係
            if (FormSize.Width < 1600 * DpiScale)
            {
                Thumbnail.Width = Convert.ToInt32(FormSize.Width * 0.4 - 312 * DpiScale);
                Thumbnail.Location = new Point(Convert.ToInt32(FormSize.Width - Thumbnail.Width - 32 * DpiScale), Thumbnail.Location.Y);
                EditNameTextBox.Width = Convert.ToInt32(260 * DpiScale);
                EditIDTextBox.Width = Convert.ToInt32(260 * DpiScale);
                EditMCTextBox.Width = Convert.ToInt32(260 * DpiScale);
                EditRegistrationDateTextBox.Width = Convert.ToInt32(260 * DpiScale);
                EditCategoryTextBox.Width = Convert.ToInt32(260 * DpiScale);
                EditTag1TextBox.Width = Convert.ToInt32(240 * DpiScale);
                EditTag2TextBox.Width = Convert.ToInt32(240 * DpiScale);
                EditTag3TextBox.Width = Convert.ToInt32(240 * DpiScale);
                EditRealLocationTextBox.Width = Convert.ToInt32(190 * DpiScale);
            }
            else
            {
                Thumbnail.Width = Convert.ToInt32(328 * DpiScale);
                Thumbnail.Location = new Point(Convert.ToInt32(FormSize.Width - 360 * DpiScale), Thumbnail.Location.Y);
                Thumbnail.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                EditNameTextBox.Width = Convert.ToInt32(FormSize.Width - 1080 * DpiScale) / 2;
                EditIDTextBox.Width = Convert.ToInt32(FormSize.Width - 1080 * DpiScale) / 2;
                EditMCTextBox.Width = Convert.ToInt32(FormSize.Width - 1080 * DpiScale) / 2;
                EditRegistrationDateTextBox.Width = Convert.ToInt32(FormSize.Width - 1080 * DpiScale) / 2;
                EditCategoryTextBox.Width = Convert.ToInt32(FormSize.Width - 1080 * DpiScale) / 2;
                EditTag1TextBox.Width = Convert.ToInt32(FormSize.Width - 1120 * DpiScale) / 2;
                EditTag2TextBox.Width = Convert.ToInt32(FormSize.Width - 1120 * DpiScale) / 2;
                EditTag3TextBox.Width = Convert.ToInt32(FormSize.Width - 1120 * DpiScale) / 2;
                EditRealLocationTextBox.Width = Convert.ToInt32(FormSize.Width - 1220 * DpiScale) / 2;
            }
            DataLoadingLabel.Font = new Font(fontname, mainfontsize);
            DataLoadingLabel.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 - DataLoadingLabel.Width - 10 * DpiScale), DataLoadingLabel.Location.Y);
            dataGridView1.Font = new Font(fontname, mainfontsize);
            dataGridView1.Height = Convert.ToInt32(FormSize.Height - 200 * DpiScale);
            dataGridView1.Location = new Point(Convert.ToInt32(10 * DpiScale), Convert.ToInt32(140 * DpiScale));
            SearchOptionComboBox.Font = new Font(fontname, smallfontsize);
            SearchOptionComboBox.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 - 365 * DpiScale), SearchOptionComboBox.Location.Y);
            PictureBox1.Width = Convert.ToInt32(FormSize.Width * 0.5 - 20 * DpiScale);
            PictureBox1.Height = Convert.ToInt32(FormSize.Height - 200 * DpiScale);
            PictureBox1.Location = new Point(Convert.ToInt32(10 * DpiScale), Convert.ToInt32(70 * DpiScale));
            ShowPictureFileNameLabel.Font = new Font(fontname, mainfontsize);
            ShowPictureFileNameLabel.Location = new Point(PictureBox1.Location.X, Convert.ToInt32(30 * DpiScale));
            NoPicturesLabel.Font = new Font(fontname, mainfontsize);
            NoPicturesLabel.Location = new Point(Convert.ToInt32(FormSize.Width * 0.25 - 92 * DpiScale), Convert.ToInt32(FormSize.Height * 0.5));
            ClosePicturesButton.Font = new Font(fontname, mainfontsize);
            ClosePicturesButton.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 - 190 * DpiScale), Convert.ToInt32(FormSize.Height - 120 * DpiScale));
            SearchMethodComboBox.Font = new Font(fontname, smallfontsize);
            SearchMethodComboBox.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 - 205 * DpiScale), SearchOptionComboBox.Location.Y);
            SearchFormTextBox.Font = new Font(fontname, mainfontsize);
            SearchFormTextBox.Width = Convert.ToInt32(FormSize.Width * 0.5 - 385 * DpiScale);
            SearchButton.Font = new Font(fontname, mainfontsize);
            SearchButton.Location = new Point(Convert.ToInt32(SearchFormTextBox.Location.X + SearchFormTextBox.Width - SearchFormTextBoxClearButton.Width - (SearchFormTextBox.Height - SearchFormTextBoxClearButton.Height) * 0.5 - 40 * DpiScale), Convert.ToInt32(SearchFormTextBox.Location.Y + (SearchFormTextBox.Height - SearchFormTextBoxClearButton.Height) * 0.5));
            SearchFormTextBoxClearButton.Font = new Font(fontname, mainfontsize);
            SearchFormTextBoxClearButton.Location = new Point(Convert.ToInt32(SearchFormTextBox.Location.X + SearchFormTextBox.Width - SearchFormTextBoxClearButton.Width - (SearchFormTextBox.Height - SearchFormTextBoxClearButton.Height) * 0.5), Convert.ToInt32(SearchFormTextBox.Location.Y + (SearchFormTextBox.Height - SearchFormTextBoxClearButton.Height) * 0.5));
            DetailsTextBox.Font = new Font(fontname, mainfontsize);
            DetailsTextBox.Width = Convert.ToInt32(FormSize.Width * 0.5 - 50 * DpiScale);
            DetailsTextBox.Height = Convert.ToInt32(FormSize.Height - 600 * DpiScale);
            DetailsTextBox.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + 15 * DpiScale), Convert.ToInt32(500 * DpiScale));
            ConfidentialDataTextBox.Font = new Font(fontname, mainfontsize);
            ConfidentialDataTextBox.Width = Convert.ToInt32(FormSize.Width * 0.5 - 50 * DpiScale);
            ConfidentialDataTextBox.Height = Convert.ToInt32(FormSize.Height - 600 * DpiScale);
            ConfidentialDataTextBox.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + 15 * DpiScale), Convert.ToInt32(500 * DpiScale));
            NextPictureButton.Font = new Font(fontname, mainfontsize);
            NextPictureButton.Location = new Point(Convert.ToInt32(FormSize.Width * 0.25 - 90 * DpiScale), Convert.ToInt32(FormSize.Height - 120 * DpiScale));
            PreviousPictureButton.Font = new Font(fontname, mainfontsize);
            PreviousPictureButton.Location = new Point(Convert.ToInt32(10 * DpiScale), Convert.ToInt32(FormSize.Height - 120 * DpiScale));
            CenterLine.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5), Convert.ToInt32(54 * DpiScale));
            CenterLine.Height = Convert.ToInt32(FormSize.Height - 100 * DpiScale);
            ObjectNameLabel.Font = new Font(fontname, bigfontsize);
            ObjectNameLabel.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + 10 * DpiScale), ShowObjectName.Location.Y);
            ShowObjectName.Font = new Font(fontname, bigfontsize);
            ShowObjectName.Location = new Point(Convert.ToInt32(ObjectNameLabel.Location.X + ObjectNameLabel.Width), ShowObjectName.Location.Y);
            EditNameTextBox.Font = new Font(fontname, mainfontsize);
            EditNameTextBox.Location = new Point(Convert.ToInt32(ObjectNameLabel.Location.X + ObjectNameLabel.Width), ShowObjectName.Location.Y);
            EditNameTextBox.Width = Convert.ToInt32(Thumbnail.Location.X - EditNameTextBox.Location.X - 5 * DpiScale);
            IDLabel.Font = new Font(fontname, smallfontsize);
            IDLabel.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + 10 * DpiScale), ShowID.Location.Y);
            ShowID.Font = new Font(fontname, smallfontsize);
            ShowID.Location = new Point(Convert.ToInt32(IDLabel.Location.X + IDLabel.Width), ShowID.Location.Y);
            EditIDTextBox.Font = new Font(fontname, smallfontsize);
            EditIDTextBox.Location = new Point(Convert.ToInt32(IDLabel.Location.X + IDLabel.Width), ShowID.Location.Y);
            EditIDTextBox.Width = Convert.ToInt32(Thumbnail.Location.X - EditIDTextBox.Location.X - 5 * DpiScale);
            MCLabel.Font = new Font(fontname, mainfontsize);
            MCLabel.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + 10 * DpiScale), ShowMC.Location.Y);
            ShowMC.Font = new Font(fontname, mainfontsize);
            ShowMC.Location = new Point(Convert.ToInt32(MCLabel.Location.X + MCLabel.Width), ShowMC.Location.Y);
            EditMCTextBox.Font = new Font(fontname, mainfontsize);
            EditMCTextBox.Location = new Point(Convert.ToInt32(MCLabel.Location.X + MCLabel.Width), ShowMC.Location.Y);
            EditMCTextBox.Width = Convert.ToInt32(Thumbnail.Location.X - EditMCTextBox.Location.X - 5 * DpiScale);
            RegistrationDateLabel.Font = new Font(fontname, mainfontsize);
            RegistrationDateLabel.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + 10 * DpiScale), RegistrationDateLabel.Location.Y);
            ShowRegistrationDate.Font = new Font(fontname, mainfontsize);
            ShowRegistrationDate.Location = new Point(Convert.ToInt32(RegistrationDateLabel.Location.X + RegistrationDateLabel.Width), RegistrationDateLabel.Location.Y);
            EditRegistrationDateTextBox.Font = new Font(fontname, mainfontsize);
            EditRegistrationDateTextBox.Location = new Point(Convert.ToInt32(RegistrationDateLabel.Location.X + RegistrationDateLabel.Width), RegistrationDateLabel.Location.Y);
            EditRegistrationDateTextBox.Width = Convert.ToInt32(Thumbnail.Location.X - EditRegistrationDateTextBox.Location.X - 5 * DpiScale);
            CategoryLabel.Font = new Font(fontname, mainfontsize);
            CategoryLabel.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + 10 * DpiScale), CategoryLabel.Location.Y);
            ShowCategory.Font = new Font(fontname, mainfontsize);
            ShowCategory.Location = new Point(Convert.ToInt32(CategoryLabel.Location.X + CategoryLabel.Width), CategoryLabel.Location.Y);
            EditCategoryTextBox.Font = new Font(fontname, mainfontsize);
            EditCategoryTextBox.Location = new Point(Convert.ToInt32(CategoryLabel.Location.X + CategoryLabel.Width), CategoryLabel.Location.Y);
            EditCategoryTextBox.Width = Convert.ToInt32(Thumbnail.Location.X - EditCategoryTextBox.Location.X - 5 * DpiScale);
            Tag1NameLabel.Font = new Font(fontname, mainfontsize);
            Tag1NameLabel.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + 20 * DpiScale), Tag1NameLabel.Location.Y);
            ShowTag1.Font = new Font(fontname, mainfontsize);
            ShowTag1.Location = new Point(Convert.ToInt32(Tag1NameLabel.Location.X + Tag1NameLabel.Width), Tag1NameLabel.Location.Y);
            EditTag1TextBox.Font = new Font(fontname, mainfontsize);
            EditTag1TextBox.Location = new Point(Convert.ToInt32(Tag1NameLabel.Location.X + +Tag1NameLabel.Width), Tag1NameLabel.Location.Y);
            EditTag1TextBox.Width = Convert.ToInt32(Thumbnail.Location.X - EditTag1TextBox.Location.X - 5 * DpiScale);
            Tag2NameLabel.Font = new Font(fontname, mainfontsize);
            Tag2NameLabel.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + 20 * DpiScale), Tag2NameLabel.Location.Y);
            ShowTag2.Font = new Font(fontname, mainfontsize);
            ShowTag2.Location = new Point(Convert.ToInt32(Tag2NameLabel.Location.X + Tag2NameLabel.Width), Tag2NameLabel.Location.Y);
            EditTag2TextBox.Font = new Font(fontname, mainfontsize);
            EditTag2TextBox.Location = new Point(Convert.ToInt32(Tag2NameLabel.Location.X + Tag2NameLabel.Width), Tag2NameLabel.Location.Y);
            EditTag2TextBox.Width = Convert.ToInt32(Thumbnail.Location.X - EditTag2TextBox.Location.X - 5 * DpiScale);
            Tag3NameLabel.Font = new Font(fontname, mainfontsize);
            Tag3NameLabel.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + 20 * DpiScale), Tag3NameLabel.Location.Y);
            ShowTag3.Font = new Font(fontname, mainfontsize);
            ShowTag3.Location = new Point(Convert.ToInt32(Tag3NameLabel.Location.X + Tag3NameLabel.Width), Tag3NameLabel.Location.Y);
            EditTag3TextBox.Font = new Font(fontname, mainfontsize);
            EditTag3TextBox.Location = new Point(Convert.ToInt32(Tag3NameLabel.Location.X + Tag3NameLabel.Width), Tag3NameLabel.Location.Y);
            EditTag3TextBox.Width = Convert.ToInt32(Thumbnail.Location.X - EditTag3TextBox.Location.X - 5 * DpiScale);
            RealLocationLabel.Font = new Font(fontname, mainfontsize);
            RealLocationLabel.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + 5 * DpiScale), ShowRealLocation.Location.Y);
            ShowRealLocation.Font = new Font(fontname, mainfontsize);
            ShowRealLocation.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + RealLocationLabel.Width), ShowRealLocation.Location.Y);
            EditRealLocationTextBox.Font = new Font(fontname, mainfontsize);
            EditRealLocationTextBox.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + RealLocationLabel.Width), ShowRealLocation.Location.Y);
            EditRealLocationTextBox.Width = Convert.ToInt32(Thumbnail.Location.X - EditRealLocationTextBox.Location.X - 5 * DpiScale);
            ShowDataLocation.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + 5 * DpiScale), ShowDataLocation.Location.Y);
            ShowDataLocation.Font = new Font(fontname, mainfontsize);
            OpenDataLocation.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + ShowDataLocation.Width), ShowDataLocation.Location.Y);
            OpenDataLocation.Font = new Font(fontname, smallfontsize);
            CopyDataLocationPath.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + ShowDataLocation.Width + OpenDataLocation.Width + 5 * DpiScale), ShowDataLocation.Location.Y);
            CopyDataLocationPath.Font = new Font(fontname, smallfontsize);
            DetailsLabel.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + 5 * DpiScale), Convert.ToInt32(DetailsTextBox.Location.Y - DetailsLabel.Height - 5 * DpiScale));
            DetailsLabel.Font = new Font(fontname, mainfontsize);
            ConfidentialDataLabel.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + 5 * DpiScale), Convert.ToInt32(DetailsTextBox.Location.Y - ConfidentialDataLabel.Height - 5 * DpiScale));
            ConfidentialDataLabel.Font = new Font(fontname, mainfontsize);
            EditButton.Location = new Point(Convert.ToInt32(FormSize.Width - 615 * DpiScale), Convert.ToInt32(FormSize.Height - 90 * DpiScale));
            EditButton.Font = new Font(fontname, mainfontsize);
            EditRequestButton.Location = new Point(Convert.ToInt32(FormSize.Width - 630 * DpiScale), Convert.ToInt32(FormSize.Height - 90 * DpiScale));
            EditRequestButton.Font = new Font(fontname, mainfontsize);
            ReadOnlyButton.Location = new Point(Convert.ToInt32(FormSize.Width - 615 * DpiScale), Convert.ToInt32(FormSize.Height - 90 * DpiScale));
            ReadOnlyButton.Font = new Font(fontname, mainfontsize);
            EditRequestingButton.Location = new Point(Convert.ToInt32(FormSize.Width - 615 * DpiScale), Convert.ToInt32(FormSize.Height - 90 * DpiScale));
            EditRequestingButton.Font = new Font(fontname, mainfontsize);
            SaveAndCloseEditButton.Location = new Point(Convert.ToInt32(FormSize.Width - 620 * DpiScale), Convert.ToInt32(FormSize.Height - 90 * DpiScale));
            SaveAndCloseEditButton.Font = new Font(fontname, mainfontsize);
            SavingLabel.Location = new Point(Convert.ToInt32(FormSize.Width - 620 * DpiScale), Convert.ToInt32(FormSize.Height - 90 * DpiScale));
            SavingLabel.Font = new Font(fontname, mainfontsize);
            InventoryManagementModeButton.Location = new Point(Convert.ToInt32(FormSize.Width - 445 * DpiScale), Convert.ToInt32(FormSize.Height - 90 * DpiScale));
            InventoryManagementModeButton.Font = new Font(fontname, mainfontsize);
            CloseInventoryManagementModeButton.Location = new Point(Convert.ToInt32(FormSize.Width - 445 * DpiScale), Convert.ToInt32(FormSize.Height - 90 * DpiScale));
            CloseInventoryManagementModeButton.Font = new Font(fontname, mainfontsize);
            ShowConfidentialDataButton.Location = new Point(Convert.ToInt32(FormSize.Width - 230 * DpiScale), Convert.ToInt32(FormSize.Height - 90 * DpiScale));
            ShowConfidentialDataButton.Font = new Font(fontname, mainfontsize);
            HideConfidentialDataButton.Location = new Point(Convert.ToInt32(FormSize.Width - 230 * DpiScale), Convert.ToInt32(FormSize.Height - 90 * DpiScale));
            HideConfidentialDataButton.Font = new Font(fontname, mainfontsize);
            // 在庫管理モード関係
            InventoryModeDataGridView.Width = Convert.ToInt32(FormSize.Width * 0.5 - 20 * DpiScale);
            InventoryModeDataGridView.Height = Convert.ToInt32(FormSize.Height - 340 * DpiScale);
            InventoryModeDataGridView.Location = new Point(Convert.ToInt32(10 * DpiScale), Convert.ToInt32(140 * DpiScale));
            InventoryModeDataGridView.Font = new Font(fontname, mainfontsize);
            InventoryOperationLabel.Location = new Point(Convert.ToInt32(30 * DpiScale), Convert.ToInt32(FormSize.Height - 195 * DpiScale));
            InventoryOperationLabel.Font = new Font(fontname, mainfontsize);
            InputQuantitiyLabel.Location = new Point(Convert.ToInt32(210 * DpiScale), Convert.ToInt32(FormSize.Height - 195 * DpiScale));
            InputQuantitiyLabel.Font = new Font(fontname, mainfontsize);
            OperationOptionComboBox.Location = new Point(Convert.ToInt32(30 * DpiScale), Convert.ToInt32(FormSize.Height - 160 * DpiScale));
            OperationOptionComboBox.Font = new Font(fontname, mainfontsize);
            EditQuantityTextBox.Location = new Point(Convert.ToInt32(210 * DpiScale), Convert.ToInt32(FormSize.Height - 160 * DpiScale));
            EditQuantityTextBox.Font = new Font(fontname, mainfontsize);
            AddQuantityButton.Location = new Point(Convert.ToInt32(380 * DpiScale), Convert.ToInt32(FormSize.Height - 195 * DpiScale));
            AddQuantityButton.Font = new Font(fontname, mainfontsize);
            SubtractQuantityButton.Location = new Point(Convert.ToInt32(380 * DpiScale), Convert.ToInt32(FormSize.Height - 150 * DpiScale));
            SubtractQuantityButton.Font = new Font(fontname, mainfontsize);
            AddInventoryOperationButton.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 - 175 * DpiScale), Convert.ToInt32(FormSize.Height - 165 * DpiScale));
            AddInventoryOperationButton.Font = new Font(fontname, mainfontsize);
            InventoryOperationNoteLabel.Location = new Point(Convert.ToInt32(30 * DpiScale), Convert.ToInt32(FormSize.Height - 120 * DpiScale));
            InventoryOperationNoteLabel.Font = new Font(fontname, mainfontsize);
            EditInventoryOperationNoteTextBox.Width = Convert.ToInt32(FormSize.Width * 0.5 - 40 * DpiScale);
            EditInventoryOperationNoteTextBox.Location = new Point(Convert.ToInt32(30 * DpiScale), Convert.ToInt32(FormSize.Height - 90 * DpiScale));
            EditInventoryOperationNoteTextBox.Font = new Font(fontname, mainfontsize);
            ProperInventorySettingsComboBox.Font = new Font(fontname, mainfontsize);
            ProperInventorySettingsTextBox.Location = new Point(Convert.ToInt32(ProperInventorySettingsComboBox.Location.X + ProperInventorySettingsComboBox.Width + 15 * DpiScale), ProperInventorySettingsTextBox.Location.Y);
            ProperInventorySettingsTextBox.Font = new Font(fontname, mainfontsize);
            SetProperInventorySettingsButton.Location = new Point(Convert.ToInt32(ProperInventorySettingsTextBox.Location.X + ProperInventorySettingsTextBox.Width + 15 * DpiScale), SetProperInventorySettingsButton.Location.Y);
            SetProperInventorySettingsButton.Font = new Font(fontname, mainfontsize);
            SaveProperInventorySettingsButton.Location = new Point(Convert.ToInt32(ProperInventorySettingsTextBox.Location.X + ProperInventorySettingsTextBox.Width + 15 * DpiScale), SaveProperInventorySettingsButton.Location.Y);
            SaveProperInventorySettingsButton.Font = new Font(fontname, mainfontsize);
            AllowEditIDButton.Location = new Point(Convert.ToInt32(EditIDTextBox.Location.X + EditIDTextBox.Width - AllowEditIDButton.Width - 10 * DpiScale), EditIDTextBox.Location.Y + (EditIDTextBox.Height - AllowEditIDButton.Height) / 2);
            AllowEditIDButton.Font = new Font(fontname, extrasmallfontsize);
            UUIDEditStatusLabel.Location = new Point(Convert.ToInt32(EditIDTextBox.Location.X + EditIDTextBox.Width - UUIDEditStatusLabel.Width - 10 * DpiScale), EditIDTextBox.Location.Y + (EditIDTextBox.Height - UUIDEditStatusLabel.Height) / 2);
            UUIDEditStatusLabel.Font = new Font(fontname, extrasmallfontsize);
            CheckSameMCButton.Location = new Point(Convert.ToInt32(EditMCTextBox.Location.X + EditMCTextBox.Width - CheckSameMCButton.Width - 5 * DpiScale), EditMCTextBox.Location.Y + (EditMCTextBox.Height - CheckSameMCButton.Height) / 2);
            CheckSameMCButton.Font = new Font(fontname, extrasmallfontsize);
            NoImageLabel.Location = new Point(Convert.ToInt32(Thumbnail.Location.X + (Thumbnail.Width - NoImageLabel.Width) * 0.5), Convert.ToInt32(Thumbnail.Location.Y + (Thumbnail.Height - NoImageLabel.Height) * 0.5));
            ShowPicturesButton.Location = new Point(Convert.ToInt32(Thumbnail.Location.X + Thumbnail.Width * 0.5 - 85 * DpiScale), ShowPicturesButton.Location.Y);
            ShowPicturesButton.Font = new Font(fontname, mainfontsize);
            OpenPictureFolderButton.Location = new Point(Convert.ToInt32(Thumbnail.Location.X + Thumbnail.Width * 0.5 - 85 * DpiScale), OpenPictureFolderButton.Location.Y);
            OpenPictureFolderButton.Font = new Font(fontname, mainfontsize);
            SelectThumbnailButton.Location = new Point(Convert.ToInt32(Thumbnail.Location.X + Thumbnail.Width * 0.5 - 85 * DpiScale), SelectThumbnailButton.Location.Y);
            SelectThumbnailButton.Font = new Font(fontname, mainfontsize);
            ShowListButton.Width = Convert.ToInt32(130 * DpiScale);
            ShowListButton.Font = new Font(fontname, mainfontsize);
            // dataGridViewContextMenuStripの文字サイズ
            AddContentsContextStripMenuItem.Font = new Font(fontname, smallfontsize);
            CopyAndAddContentsContextToolStripMenuItem.Font = new Font(fontname, smallfontsize);
            ListUpdateContextStripMenuItem.Font = new Font(fontname, smallfontsize);
            OpenProjectContextStripMenuItem.Font = new Font(fontname, smallfontsize);
            ShowSelectedItemInformationToolStripMenuItem.Font = new Font(fontname, smallfontsize);
            // PictureBox1ContextMenuStripの文字サイズ
            OpenPicturewithAppToolStripMenuItem.Font = new Font(fontname, mainfontsize);
            AddContentsButton.Font = new Font(fontname, smallfontsize);
            ListUpdateButton.Font = new Font(fontname, smallfontsize);
            // ToolStorip関係
            ShowListButton.Font = new Font(fontname, smallfontsize);
            foreach (var toolStripMenuItem1 in menuStrip1.Items)
            {
                if (toolStripMenuItem1 is ToolStripMenuItem)
                {
                    ((ToolStripMenuItem)toolStripMenuItem1).Font = new Font(fontname, smallfontsize);
                    foreach (var toolStripMenuItem2 in ((ToolStripMenuItem)toolStripMenuItem1).DropDownItems)
                    {
                        if (toolStripMenuItem2 is ToolStripMenuItem)
                        {
                            ((ToolStripMenuItem)toolStripMenuItem2).Font = new Font(fontname, smallfontsize);
                            foreach (var toolStripMenuItem3 in ((ToolStripMenuItem)toolStripMenuItem2).DropDownItems)
                            {
                                if (toolStripMenuItem3 is ToolStripMenuItem)
                                {
                                    ((ToolStripMenuItem)toolStripMenuItem3).Font = new Font(fontname, smallfontsize);
                                }
                            }
                        }
                    }
                }
            }
        }
        private void MainForm_DpiChanged(object sender, DpiChangedEventArgs e)// DPIの変更を取得および文字サイズの計算
        {
            CurrentDPI = e.DeviceDpiNew / 96.0;
            extrasmallfontsize = (float)(9 * CurrentDPI / FirstDPI);// 最小フォントのサイズ
            smallfontsize = (float)(14.25 * CurrentDPI / FirstDPI);// 小フォントのサイズ
            mainfontsize = (float)(18.0 * CurrentDPI / FirstDPI);// 標準フォントのサイズ
            bigfontsize = (float)(20.25 * CurrentDPI / FirstDPI);// 大フォントのサイズ
        }
        private void MainForm_ResizeEnd(object sender, EventArgs e)// ウインドウサイズの変更・移動を取得
        {
            SetFormLayout();
        }
        #endregion

        #region 非同期処理置き場
        private async void AwaitEdit()// 編集許可を待機
        {
            while (true)
            {
                await Task.Delay(CurrentProjectSettingValues.DataCheckInterval);
                if (Directory.Exists(CurrentShownDataValues.CollectionFolderPath) == false)
                {
                    MessageBox.Show("IDが変更されました。\n再度読み込みを行ってください。", "CREC");
                    break;
                }
                if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED"))
                {
                }
                else
                {
                    if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED"))
                    {
                        EditRequestingButton.Visible = false;
                        EditButton.Visible = true;
                        MessageBox.Show("拒否されました", "CREC");
                        return;
                    }
                    else if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\FREE"))
                    {
                        EditRequestButton.Visible = false;
                        EditRequestingButton.Visible = false;
                        EditButton.Visible = false;
                        ReadOnlyButton.Visible = false;
                        EditRequestButton.Visible = false;
                        //EditButton.Text = "編集";
                        StartEditForm();
                        // 現時点でのデータを読み込んで表示
                        LoadDetails();
                        // 詳細情報読み込み＆表示
                        StreamReader streamReaderDetailData = null;
                        try
                        {
                            streamReaderDetailData = new StreamReader(TargetDetailsPath);
                            DetailsTextBox.Text = streamReaderDetailData.ReadToEnd();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("DetailDataLoadError", "mainform", LanguageFile) + "\n" + ex.Message, "CREC");
                            DetailsTextBox.Text = "No Data.";
                        }
                        finally
                        {
                            streamReaderDetailData?.Close();
                        }
                        // 機密情報を読み込み
                        StreamReader streamReaderConfidentialData = null;
                        try
                        {
                            streamReaderConfidentialData = new StreamReader(CurrentShownDataValues.CollectionFolderPath + "\\confidentialdata.txt");
                            ConfidentialDataTextBox.Text = streamReaderConfidentialData.ReadToEnd();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("RestrictedDataLoadError", "mainform", LanguageFile) + "\n" + ex.Message, "CREC");
                            ConfidentialDataTextBox.Text = "No Data.";
                        }
                        finally
                        {
                            streamReaderConfidentialData?.Close();
                        }
                        EditNameTextBox.Text = CurrentShownDataValues.Name;
                        EditIDTextBox.TextChanged -= IDTextBox_TextChanged;// ID重複確認イベントを停止
                        EditIDTextBox.Text = CurrentShownDataValues.ID;
                        EditIDTextBox.TextChanged += IDTextBox_TextChanged;// ID重複確認イベントを開始
                        AllowEditIDButton.Visible = true;
                        UUIDEditStatusLabel.Visible = false;
                        EditMCTextBox.Text = CurrentShownDataValues.MC;
                        EditRegistrationDateTextBox.Text = CurrentShownDataValues.RegistrationDate;
                        EditCategoryTextBox.Text = CurrentShownDataValues.Category;
                        EditTag1TextBox.Text = CurrentShownDataValues.Tag1;
                        EditTag2TextBox.Text = CurrentShownDataValues.Tag2;
                        EditTag3TextBox.Text = CurrentShownDataValues.Tag3;
                        EditRealLocationTextBox.Text = CurrentShownDataValues.RealLocation;
                        return;
                    }
                }
            }
        }
        private async void AwaitEditRequest()// 編集リクエストを待機
        {
            while (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED"))// DEDがある（編集中）のみ実施、消えたら終わる
            {
                await Task.Delay(CurrentProjectSettingValues.DataCheckInterval);
                if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED"))
                {
                    System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("他の端末から編集権限をリクエストされました。\nリクエストを許可しますか？", "CREC", System.Windows.MessageBoxButton.YesNo);
                    if (result == System.Windows.MessageBoxResult.Yes)// 編集を権限を譲渡
                    {
                        // 保存関係の処理
                        if (CurrentShownDataValues.CollectionFolderPath == CurrentProjectSettingValues.ProjectDataFolderPath + "\\" + EditIDTextBox.Text)
                        {
                            if (SaveContentsMethod() == false)
                            {
                                return;
                            }
                        }
                        else if (CurrentShownDataValues.CollectionFolderPath != CurrentProjectSettingValues.ProjectDataFolderPath + "\\" + EditIDTextBox.Text)
                        {
                            result = System.Windows.MessageBox.Show("IDを変更した場合、他の端末へ編集権限を譲渡できなくなります。\nIDを変更前のものに戻して保存しますか？", "CREC", System.Windows.MessageBoxButton.YesNo);
                            if (result == System.Windows.MessageBoxResult.Yes)
                            {
                                CurrentShownDataValues.ID = Path.GetFileName(CurrentShownDataValues.CollectionFolderPath);
                                EditIDTextBox.Text = CurrentShownDataValues.ID;
                                MessageBox.Show("IDを" + CurrentShownDataValues.ID + "に戻しました", "CREC");
                                if (SaveContentsMethod() == false)
                                {
                                    return;
                                }
                            }
                            else
                            {
                                if (SaveContentsMethod() == false)
                                {
                                    return;
                                }
                            }

                        }
                        // 通常画面に不要な物を非表示に
                        EditNameTextBox.Visible = false;
                        EditIDTextBox.Visible = false;
                        AllowEditIDButton.Visible = false;
                        EditMCTextBox.Visible = false;
                        CheckSameMCButton.Visible = false;
                        EditRegistrationDateTextBox.Visible = false;
                        EditCategoryTextBox.Visible = false;
                        EditTag1TextBox.Visible = false;
                        EditTag2TextBox.Visible = false;
                        EditTag3TextBox.Visible = false;
                        EditRealLocationTextBox.Visible = false;
                        SaveAndCloseEditButton.Visible = false;
                        SelectThumbnailButton.Visible = false;
                        OpenPictureFolderButton.Visible = false;
                        //　再表示時に編集したデータを表示するための処理
                        SearchFormTextBox.Text = EditIDTextBox.Text;
                        SearchOptionComboBox.SelectedIndex = 0;
                        // 入力フォームをリセット
                        EditNameTextBox.ResetText();
                        EditIDTextBox.TextChanged -= IDTextBox_TextChanged;// ID重複確認イベントを停止
                        EditIDTextBox.ResetText();
                        EditIDTextBox.TextChanged += IDTextBox_TextChanged;// ID重複確認イベントを再開
                        EditMCTextBox.ResetText();
                        EditRegistrationDateTextBox.ResetText();
                        EditCategoryTextBox.ResetText();
                        EditTag1TextBox.ResetText();
                        EditTag2TextBox.ResetText();
                        EditTag3TextBox.ResetText();
                        EditRealLocationTextBox.ResetText();
                        DetailsTextBox.ResetText();
                        ConfidentialDataTextBox.ResetText();
                        // 通常画面で必要なものを表示
                        EditButton.Visible = true;
                        ShowTag1.Visible = true;
                        ShowTag2.Visible = true;
                        ShowTag3.Visible = true;
                        ShowPicturesButton.Visible = true;
                        // 詳細データおよび機密データを編集不可能に変更
                        DetailsTextBox.ReadOnly = true;
                        ConfidentialDataTextBox.ReadOnly = true;
                        // 再度詳細情報を表示
                        if (DataLoadingStatus == "true")
                        {
                            DataLoadingStatus = "stop";
                        }
                        LoadGrid();
                        ShowDetails();
                        break;
                    }
                    else
                    {
                        FileOperationClass.DeleteFile(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED");
                        break;
                    }
                }
            }
        }
        private async void CheckContentsList()// ContentsListの状態をバックグラウンドで監視
        {
            while (true)
            {
                await Task.Delay(100);
                if (dataGridView1.CurrentRow != null)// セル未選択時は何もしない
                {
                    if (CurrentShownDataValues.ID != Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value))
                    {
                        if (SaveAndCloseEditButton.Visible == true)// 編集中の場合は警告を表示
                        {
                            if (CheckEditingContents() == true)
                            {
                                ShowDetails();
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            ShowDetails();
                        }
                    }
                }
            }
        }
        private async void CheckEditing()// 表示中のデータが他の端末で編集中かバックグラウンドで監視
        {
            bool activeForm = false;
            while (true)
            {
                await Task.Delay(CurrentProjectSettingValues.DataCheckInterval);
                switch (CurrentProjectSettingValues.SleepMode)
                {
                    case SleepMode.Deep:
                        if (Form.ActiveForm == this)
                        {
                            activeForm = true;
                        }
                        else
                        {
                            activeForm = false;
                            try// 終了時にエラーが出るのでCatchするが、微妙
                            {
                                ShowProjcetNameTextBox.Text = "Deep Sleep...";
                            }
                            catch
                            { }
                            for (int i = 0; i < 10; i++)
                            {
                                await Task.Delay(CurrentProjectSettingValues.DataCheckInterval);
                                if (Form.ActiveForm == this)
                                {
                                    activeForm = true;
                                    break;
                                }
                            }
                        }
                        break;
                    case SleepMode.Normal:
                        if (Form.ActiveForm == this)
                        {
                            activeForm = true;
                        }
                        else
                        {
                            activeForm = true;
                            try// 終了時にエラーが出るのでCatchするが、微妙
                            {
                                ShowProjcetNameTextBox.Text = "Sleep...";
                            }
                            catch
                            { }
                            for (int i = 0; i < 10; i++)
                            {
                                await Task.Delay(CurrentProjectSettingValues.DataCheckInterval);
                                if (Form.ActiveForm == this)
                                {
                                    activeForm = true;
                                    break;
                                }
                            }
                        }
                        break;
                    case SleepMode.Disable:
                        activeForm = true;
                        break;
                }

                if (activeForm == true)
                {
                    try// 終了時にエラーが出るのでCatchするが、微妙
                    {
                        if (ShowProjcetNameTextBox.Text.Contains("Sleep..."))
                        {
                            ShowProjcetNameTextBox.Text = LanguageSettingClass.GetOtherMessage("ProjectNameHeader", "mainform", LanguageFile) + CurrentProjectSettingValues.Name;
                        }
                    }
                    catch { }
                    if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\DED"))
                    {
                        if (File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\SystemData\\RED"))
                        {
                            if (EditRequestingButton.Visible == false)
                            {
                                ReadOnlyButton.Visible = true;
                                ReadOnlyButton.ForeColor = Color.Red;
                            }
                            else
                            {
                                ReadOnlyButton.Visible = false;
                            }
                            EditButton.Visible = false;
                            EditRequestButton.Visible = false;
                        }
                        else
                        {
                            if (SaveAndCloseEditButton.Visible == false)
                            {
                                EditRequestButton.Visible = true;
                                EditRequestButton.ForeColor = Color.Blue;
                                EditButton.Visible = false;
                                ReadOnlyButton.Visible = false;
                            }
                        }
                    }
                    else
                    {
                        if (ConfigValues.AllowEdit == true)
                        {
                            EditButton.Visible = true;
                            ReadOnlyButton.Visible = false;
                            EditRequestButton.Visible = false;
                        }
                        else
                        {
                            EditButton.Visible = false;
                            ReadOnlyButton.Visible = true;
                            EditRequestButton.Visible = false;
                        }
                    }
                }
            }
        }
        private async void MakeBackUpZip()// バックアップ処理のうちZIP圧縮の部分
        {
            DateTime DT = DateTime.Now;
            BackupToolStripMenuItem.Text = LanguageSettingClass.GetOtherMessage("BackupToolStripMenuItemBackupInProgressMessage", "mainform", LanguageFile);
            BackupToolStripMenuItem.Enabled = false;
            // バックアップ作成
            if (CurrentProjectSettingValues.CompressType == CREC.CompressType.SingleFile)// 単一ZIPに圧縮
            {
                await Task.Run(() =>
                {
                    try
                    {
                        ZipFile.CreateFromDirectory(CurrentProjectSettingValues.ProjectBackupFolderPath + "\\backuptmp", CurrentProjectSettingValues.ProjectBackupFolderPath + "\\" + CurrentProjectSettingValues.Name + "_backup_" + DT.ToString("yyyy-MM-dd_HH-mm-ss") + ".zip");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("BackupFailed", "mainform", LanguageFile) + "\n" + ex.Message, "CREC");
                        BackupToolStripMenuItem.Text = LanguageSettingClass.GetToolStripMenuItemMessage("BackupToolStripMenuItem", "mainform", LanguageFile);
                        BackupToolStripMenuItem.Enabled = true;
                        return;
                    }
                    Directory.Delete(CurrentProjectSettingValues.ProjectBackupFolderPath + "\\backuptmp", true);
                });
            }
            else if (CurrentProjectSettingValues.CompressType == CREC.CompressType.ParData)// コンテンツごとに圧縮
            {
                await Task.Run(() =>
                {
                    // バックアップ用フォルダ作成
                    Directory.CreateDirectory(CurrentProjectSettingValues.ProjectBackupFolderPath + "\\" + CurrentProjectSettingValues.Name + "_backup_" + DT.ToString("yyyy-MM-dd_HH-mm-ss"));
                    File.Copy(TargetCRECPath, CurrentProjectSettingValues.ProjectBackupFolderPath + "\\" + CurrentProjectSettingValues.Name + "_backup_" + DT.ToString("yyyy-MM-dd_HH-mm-ss") + "\\backup.crec", true);// crecファイルをバックアップ
                    try
                    {
                        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(CurrentProjectSettingValues.ProjectDataFolderPath);
                        IEnumerable<System.IO.DirectoryInfo> subFolders = di.EnumerateDirectories("*");
                        int CountBackupedData = 0;
                        int TotalBackupData = subFolders.Count();
                        foreach (System.IO.DirectoryInfo subFolder in subFolders)
                        {
                            try
                            {
                                FileSystem.CopyDirectory(subFolder.FullName, "backuptmp\\" + subFolder.Name + "\\datatemp", Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing);
                                ZipFile.CreateFromDirectory("backuptmp\\" + subFolder.Name + "\\datatemp", "backuptmp\\" + subFolder.Name + "backupziptemp.zip");// 圧縮
                                File.Move("backuptmp\\" + subFolder.Name + "backupziptemp.zip", CurrentProjectSettingValues.ProjectBackupFolderPath + "\\" + CurrentProjectSettingValues.Name + "_backup_" + DT.ToString("yyyy-MM-dd_HH-mm-ss") + "\\" + subFolder.Name + "_backup-" + DT.ToString("yyyy-MM-dd-HH-mm-ss") + ".zip");// 移動
                                Directory.Delete("backuptmp\\" + subFolder.Name, true);// 削除
                                CountBackupedData += 1;
                                BackupToolStripMenuItem.Text = LanguageSettingClass.GetOtherMessage("BackupToolStripMenuItemBackupInProgressMessage", "mainform", LanguageFile) + " : " + Convert.ToString(CountBackupedData) + "/" + Convert.ToString(TotalBackupData);
                                Application.DoEvents();
                            }
                            catch// バックアップ失敗時はログに書き込み
                            {
                                StreamWriter streamWriter = new StreamWriter("BackupErrorLog.txt", true, Encoding.GetEncoding("UTF-8"));
                                streamWriter.WriteLine(subFolder.FullName);
                                streamWriter.Close();
                            }
                        }
                        Directory.Delete("backuptmp", true);// 削除
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("BackupFailed", "mainform", LanguageFile) + "\n" + ex.Message, "CREC");
                        BackupToolStripMenuItem.Text = LanguageSettingClass.GetToolStripMenuItemMessage("BackupToolStripMenuItem", "mainform", LanguageFile);
                        BackupToolStripMenuItem.Enabled = true;
                        return;
                    }
                    if (File.Exists("BackupErrorLog.txt"))
                    {
                        MessageBox.Show("いくつかのファイルのバックアップ作成に失敗しました。\nログを確認してください。", "CREC");
                    }
                });
            }

            BackupToolStripMenuItem.Text = LanguageSettingClass.GetToolStripMenuItemMessage("BackupToolStripMenuItem", "mainform", LanguageFile);
            BackupToolStripMenuItem.Enabled = true;
            MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("BackupCompleted", "mainform", LanguageFile), "CREC");
        }
        /// <summary>
        /// サムネイル読み込み処理
        /// </summary>
        private async void LoadThnumbnailPicture()
        {
            NoImageLabel.Text = "Loading";
            await Task.Run(() =>
            {
                Thumbnail.ImageLocation = (CurrentShownDataValues.CollectionFolderPath + "\\pictures\\Thumbnail1.jpg");
            });

            if (System.IO.File.Exists(CurrentShownDataValues.CollectionFolderPath + "\\pictures\\Thumbnail1.jpg"))
            {
                NoImageLabel.Visible = false;
                Thumbnail.BackColor = this.BackColor;
            }
            else
            {
                NoImageLabel.Text = "NO IMAGE";
                NoImageLabel.Visible = true;
                Thumbnail.BackColor = menuStrip1.BackColor;
            }
        }
        /// <summary>
        /// 更新確認
        /// </summary>
        private async void CheckLatestVersion()
        {
            HttpClient httpClient = new HttpClient();
            try
            {
                // githubのreleaseにアクセスできるか確認
                HttpResponseMessage httpResponseMessage1 = await httpClient.GetAsync(GitHubLatestReleaseURL);
                if (httpResponseMessage1.IsSuccessStatusCode)// githubへのアクセスができた場合
                {
                    try
                    {
                        // 本バージョンと一致する配布先があるか確認
                        HttpResponseMessage httpResponseMessage2 = await httpClient.GetAsync(LatestVersionDownloadLink);
                        if (!httpResponseMessage2.IsSuccessStatusCode)// 配布バージョンと一致しない場合
                        {
                            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("UpdateNotification", "mainform", LanguageFile), "CREC", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
                            if (result == System.Windows.MessageBoxResult.Yes)// ブラウザでリンクを表示
                            {
                                System.Diagnostics.Process.Start(GitHubLatestReleaseURL);
                            }
                        }
                    }
                    catch
                    {
                        return;
                    }
                }
            }
            catch
            {
                return;
            }
        }
        #endregion

        #region DataGridView内のContextStripMenuの設定
        /// <summary>
        /// 全体表示モードで詳細表示画面に切り替える
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowSelectedItemInformationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Visible = false;
            dataGridView1BackgroundPictureBox.Visible = false;
            ShowSelectedItemInformationButton.Visible = false;
            SearchFormTextBox.Visible = false;
            SearchFormTextBoxClearButton.Visible = false;
            SearchOptionComboBox.Visible = false;
            SearchMethodComboBox.Visible = false;
            AddContentsButton.Visible = false;
            ShowListButton.Visible = true;
            ShowPicturesMethod();
        }
        private void AddContentsContextStripMenuItem_Click(object sender, EventArgs e)// データ追加
        {
            AddContentsMethod();// 新規にデータを追加するメソッドを呼び出し
        }
        private void CopyAndAddContentsToolStripMenuItem_Click(object sender, EventArgs e)// 表示中の内容をコピーして新規追加
        {
            // 現在の表示内容を記録
            string copyName = ShowObjectName.Text;// 名前
            string copyCategory = ShowCategory.Text;// カテゴリ
            string copyTag1 = ShowTag1.Text;// タグ1
            string copyTag2 = ShowTag2.Text;// タグ2
            string copyTag3 = ShowTag3.Text;// タグ3
            string copyRealLocation = ShowRealLocation.Text;// 現物保管場所

            AddContentsMethod();// 新規にデータを追加するメソッドを呼び出し

            // 記録した内容を複製
            EditNameTextBox.Text = copyName;
            EditCategoryTextBox.Text = copyCategory;
            EditTag1TextBox.Text = copyTag1;
            EditTag2TextBox.Text = copyTag2;
            EditTag3TextBox.Text = copyTag3;
            EditRealLocationTextBox.Text = copyRealLocation;
        }
        private void ListUpdateContextStripMenuItem_Click(object sender, EventArgs e)// 一覧を更新
        {
            if (DataLoadingStatus == "true")
            {
                DataLoadingStatus = "stop";
            }
            LoadGrid();// 再度一覧を読み込み
        }
        private void OpenProjectContextStripMenuItem_Click(object sender, EventArgs e)// プロジェクトを開く、OpenMenu_Clickと同じ
        {
            OpenProjectMethod();// 既存の在庫管理プロジェクトを読み込むメソッドを呼び出し
            if (CurrentProjectSettingValues.StartUpListOutput == true)
            {
                ListOutputMethod(CurrentProjectSettingValues.ListOutputFormat);
            }
            if (CurrentProjectSettingValues.StartUpBackUp == true)// 自動バックアップ
            {
                BackUpMethod();
                MakeBackUpZip();// ZIP圧縮を非同期で開始
            }
        }
        #endregion

        #region バックアップ関連
        private void BackUpMethod()// バックアップ作成のメソッド、対象データをバックアップ
        {
            // ファイルが開いているか確認
            if (TargetCRECPath.Length == 0)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // バックアップ場所が設定されているか確認
            if (CurrentProjectSettingValues.ProjectBackupFolderPath.Length == 0)
            {
                MessageBox.Show("バックアップフォルダが指定されていません。", "CREC");
                return;
            }
            // バックアップフォルダが存在するか確認
            if (!Directory.Exists(CurrentProjectSettingValues.ProjectBackupFolderPath))
            {
                MessageBox.Show("バックアップフォルダが見つかりませんでした", "CREC");
                return;
            }
            BackupToolStripMenuItem.Text = LanguageSettingClass.GetOtherMessage("BackupToolStripMenuItemBackupInProgressMessage", "mainform", LanguageFile);
            if (CurrentProjectSettingValues.CompressType == CREC.CompressType.SingleFile)
            {
                FileSystem.CopyDirectory(CurrentProjectSettingValues.ProjectDataFolderPath, CurrentProjectSettingValues.ProjectBackupFolderPath + "\\backuptmp", Microsoft.VisualBasic.FileIO.UIOption.AllDialogs, Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing);
                File.Copy(TargetCRECPath, CurrentProjectSettingValues.ProjectBackupFolderPath + "\\backuptmp" + "\\backup.crec", true);
            }
            else if (CurrentProjectSettingValues.CompressType == CREC.CompressType.NoCompress)
            {
                DateTime DT = DateTime.Now;
                FileSystem.CopyDirectory(CurrentProjectSettingValues.ProjectDataFolderPath, CurrentProjectSettingValues.ProjectBackupFolderPath + "\\" + CurrentProjectSettingValues.Name + "_backup_" + DT.ToString("yyyy-MM-dd_HH-mm-ss"), Microsoft.VisualBasic.FileIO.UIOption.AllDialogs, Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing);
                File.Copy(TargetCRECPath, CurrentProjectSettingValues.ProjectBackupFolderPath + "\\" + CurrentProjectSettingValues.Name + "_backup_" + DT.ToString("yyyy-MM-dd_HH-mm-ss") + "\\backup.crec", true);
            }
        }
        #endregion

        #region 一覧出力関係
        /// <summary>
        /// 一覧を出力
        /// </summary>
        /// <param name="ListOutputFormat">一覧出力先フォルダのパス</param>
        private void ListOutputMethod(CREC.ListOutputFormat listOutputFormat)
        {
            // ファイルが開いているか確認
            if (TargetCRECPath.Length == 0)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            // ファイル出力先を設定
            string tempTargetListOutputPath;
            if (Directory.Exists(CurrentProjectSettingValues.ListOutputPath))
            {
                tempTargetListOutputPath = CurrentProjectSettingValues.ListOutputPath;
            }
            else
            {
                tempTargetListOutputPath = CurrentProjectSettingValues.ProjectDataFolderPath;
            }
            // 出力形式に合わせて出力
            if (listOutputFormat == CREC.ListOutputFormat.CSV || listOutputFormat == CREC.ListOutputFormat.TSV)
            {
                ListOutputMethod(tempTargetListOutputPath, listOutputFormat);
            }
            else
            {
                MessageBox.Show("値が不正です。", "CREC");
                return;
            }
            // 出力後の描画処理
            if (CurrentProjectSettingValues.OpenListAfterOutput == true)
            {
                if (listOutputFormat == CREC.ListOutputFormat.CSV)
                {
                    System.Diagnostics.Process process = System.Diagnostics.Process.Start(tempTargetListOutputPath + "\\InventoryOutput.csv");
                }
                else if (listOutputFormat == CREC.ListOutputFormat.TSV)
                {
                    System.Diagnostics.Process process = System.Diagnostics.Process.Start(tempTargetListOutputPath + "\\InventoryOutput.tsv");
                }
            }
        }
        /// <summary>
        /// 一覧出力する処理
        /// </summary>
        /// <param name="tempTargetListOutputPath">出力パス</param>
        /// <param name="ListOutputFormat">フォーマット</param>
        private void ListOutputMethod(string tempTargetListOutputPath, CREC.ListOutputFormat listOutputFormat)
        {
            StreamWriter streamWriter;
            // ヘッダ書き込み
            if (listOutputFormat == CREC.ListOutputFormat.CSV)
            {
                streamWriter = new StreamWriter(tempTargetListOutputPath + "\\InventoryOutput.csv", false, Encoding.GetEncoding("shift-jis"));
                streamWriter.WriteLine("データ保存場所のパス,ID,管理コード,名称,登録日,カテゴリー,タグ1,タグ2,タグ3,在庫数,在庫状況");
            }
            else
            {
                streamWriter = new StreamWriter(tempTargetListOutputPath + "\\InventoryOutput.tsv", false, Encoding.GetEncoding("shift-jis"));
                streamWriter.WriteLine("データ保存場所のパス\tID\t管理コード\t名称\t登録日\tカテゴリー\tタグ1\tタグ2\tタグ3\t在庫数\t在庫状況");
            }

            try
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(CurrentProjectSettingValues.ProjectDataFolderPath);
                IEnumerable<System.IO.DirectoryInfo> subFolders = di.EnumerateDirectories("*");
                foreach (System.IO.DirectoryInfo subFolder in subFolders)
                {
                    // 変数初期化
                    DataValuesClass thisDataValues = new DataValuesClass();
                    string ListInventory = string.Empty;
                    string ListInventoryStatus = string.Empty;
                    int? ListSafetyStock = null;
                    int? ListReorderPoint = null;
                    int? ListMaximumLevel = null;
                    // index読み込み
                    IEnumerable<string> tmp = null;
                    try
                    {
                        tmp = File.ReadLines(subFolder.FullName + "\\index.txt", Encoding.GetEncoding("UTF-8"));
                        foreach (string line in tmp)
                        {
                            cols = line.Split(',');
                            switch (cols[0])
                            {
                                case "名称":
                                    thisDataValues.Name = line.Substring(3).Replace(",", string.Empty);
                                    break;
                                case "ID":
                                    thisDataValues.ID = line.Substring(3).Replace(",", string.Empty);
                                    break;
                                case "MC":
                                    thisDataValues.MC = line.Substring(3).Replace(",", string.Empty);
                                    break;
                                case "登録日":
                                    thisDataValues.RegistrationDate = line.Substring(4).Replace(",", string.Empty);
                                    break;
                                case "カテゴリ":
                                    thisDataValues.Category = line.Substring(5).Replace(",", string.Empty);
                                    break;
                                case "タグ1":
                                    thisDataValues.Tag1 = line.Substring(4).Replace(",", string.Empty);
                                    break;
                                case "タグ2":
                                    thisDataValues.Tag2 = line.Substring(4).Replace(",", string.Empty);
                                    break;
                                case "タグ3":
                                    thisDataValues.Tag3 = line.Substring(4).Replace(",", string.Empty);
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Indexファイルが破損しています。\n" + ex.Message, "CREC");
                        thisDataValues.ID = subFolder.Name;
                        thisDataValues.Name = "Status：Indexファイル破損";
                        thisDataValues.Category = " - ";
                    }
                    // 在庫状態を取得
                    //invからデータを読み込んで表示
                    if (File.Exists(subFolder.FullName + "\\inventory.inv"))
                    {
                        try
                        {
                            tmp = File.ReadLines(subFolder.FullName + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
                            bool firstline = true;
                            int count = 0;
                            foreach (string line in tmp)
                            {
                                cols = line.Split(',');
                                if (firstline == true)
                                {
                                    if (cols[1].Length != 0)
                                    {
                                        ListSafetyStock = Convert.ToInt32(cols[1]);
                                    }
                                    if (cols[2].Length != 0)
                                    {
                                        ListReorderPoint = Convert.ToInt32(cols[2]);
                                    }
                                    if (cols[3].Length != 0)
                                    {
                                        ListMaximumLevel = Convert.ToInt32(cols[3]);
                                    }
                                    firstline = false;
                                }
                                else
                                {
                                    count = count + Convert.ToInt32(cols[2]);
                                }
                            }
                            ListInventory = Convert.ToString(count);
                            ListInventoryStatus = "-";
                            if (0 == count)
                            {
                                ListInventoryStatus = "欠品";
                            }
                            else if (0 < count && count < ListSafetyStock)
                            {
                                ListInventoryStatus = "不足";
                            }
                            else if (ListSafetyStock <= count && count <= ListReorderPoint)
                            {
                                ListInventoryStatus = "不足";
                            }
                            else if (ListReorderPoint <= count && count <= ListMaximumLevel)
                            {
                                ListInventoryStatus = "適正";
                            }
                            else if (ListMaximumLevel < count)
                            {
                                ListInventoryStatus = "過剰";
                            }
                        }
                        catch (Exception ex)
                        {
                            ListInventory = "ERROR";
                            ListInventoryStatus = ex.Message;
                        }
                    }
                    else
                    {
                        ListInventory = "-";
                        ListInventoryStatus = "-";
                    }
                    // ファイル書き込み
                    if (listOutputFormat == CREC.ListOutputFormat.CSV)
                    {
                        streamWriter.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", subFolder.FullName, thisDataValues.ID, thisDataValues.MC, thisDataValues.Name, thisDataValues.RegistrationDate, thisDataValues.Category, thisDataValues.Tag1, thisDataValues.Tag2, thisDataValues.Tag3, ListInventory, ListInventoryStatus);
                    }
                    else
                    {
                        streamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}", subFolder.FullName, thisDataValues.ID, thisDataValues.MC, thisDataValues.Name, thisDataValues.RegistrationDate, thisDataValues.Category, thisDataValues.Tag1, thisDataValues.Tag2, thisDataValues.Tag3, ListInventory, ListInventoryStatus);
                    }
                }
                // 正常出力完了のメッセージ表示
                if (listOutputFormat == CREC.ListOutputFormat.CSV)
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("CSVOutputFinish", "mainform", LanguageFile) + "\n" + tempTargetListOutputPath + "\\InventoryOutput.csv", "CREC");
                }
                else
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("TSVOutputFinish", "mainform", LanguageFile) + "\n" + tempTargetListOutputPath + "\\InventoryOutput.tsv", "CREC");
                }
            }
            catch (Exception ex)
            {
                // エラーメッセージ表示
                if (listOutputFormat == CREC.ListOutputFormat.CSV)
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("CSVOutputError", "mainform", LanguageFile) + "\n" + ex.Message, "CREC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("TSVOutputError", "mainform", LanguageFile) + "\n" + ex.Message, "CREC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                streamWriter.Close();
            }
        }
        #endregion

        #region 表示モード設定
        private void StandardDisplayModeToolStripMenuItem_Click(object sender, EventArgs e)// 標準モード
        {
            StandardDisplayModeToolStripMenuItem.Checked = true;
            FullDisplayModeToolStripMenuItem.Checked = false;
            ShowSelectedItemInformationButton.Visible = false;
            ShowListButton.Visible = false;
            ShowSelectedItemInformationToolStripMenuItem.Visible = false;
            ShowSelectedItemInformationToolStripMenuItemSeparator.Visible = false;
            if (PictureBox1.Visible == true)// 画像が表示されている場合は閉じるボタンを表示
            {
                ClosePicturesButton.Visible = true;
            }
            SetFormLayout();
        }
        private void FullDisplayModeToolStripMenuItem_Click(object sender, EventArgs e)// 拡大モード変更中
        {
            FullDisplayModeToolStripMenuItem.Checked = true;
            StandardDisplayModeToolStripMenuItem.Checked = false;
            ShowSelectedItemInformationToolStripMenuItem.Visible = true;
            ShowSelectedItemInformationToolStripMenuItemSeparator.Visible = true;
            if (SaveAndCloseEditButton.Visible == true)// 編集中に切り替えた場合の処理
            {
                dataGridView1.Visible = false;
                dataGridView1BackgroundPictureBox.Visible = false;
                ShowSelectedItemInformationButton.Visible = false;
                SearchFormTextBox.Visible = false;
                SearchFormTextBoxClearButton.Visible = false;
                SearchOptionComboBox.Visible = false;
                SearchMethodComboBox.Visible = false;
                ShowListButton.Visible = true;
                if (InventoryModeDataGridView.Visible == false)// 在庫管理以外の画面状態で切り替わった場合
                {
                    ShowPicturesMethod();// 画像を表示
                }
            }
            else
            {
                if (PictureBox1.Visible == true)// 画像表示中に切り替わった場合の処理
                {
                    dataGridView1.Visible = false;
                    dataGridView1BackgroundPictureBox.Visible = false;
                    ShowSelectedItemInformationButton.Visible = false;
                    SearchFormTextBox.Visible = false;
                    SearchFormTextBoxClearButton.Visible = false;
                    SearchOptionComboBox.Visible = false;
                    SearchMethodComboBox.Visible = false;
                    ShowListButton.Visible = true;
                    ShowPicturesMethod();
                }
                else if (InventoryModeDataGridView.Visible == true)// 在庫管理中に切り替わった場合
                {
                    dataGridView1.Visible = false;
                    dataGridView1BackgroundPictureBox.Visible = false;
                    ShowSelectedItemInformationButton.Visible = false;
                    SearchFormTextBox.Visible = false;
                    SearchFormTextBoxClearButton.Visible = false;
                    SearchOptionComboBox.Visible = false;
                    SearchMethodComboBox.Visible = false;
                    ShowListButton.Visible = true;
                }
                else// その他
                {
                    ShowSelectedItemInformationButton.Visible = true;
                    dataGridView1BackgroundPictureBox.Visible = true;
                    ShowListButton.Visible = false;
                }
            }
            SetFormLayout();
        }
        private void ShowSelectedItemInformationButton_Click(object sender, EventArgs e)// 拡大モード時に詳細情報を表示
        {
            dataGridView1.Visible = false;
            dataGridView1BackgroundPictureBox.Visible = false;
            ShowSelectedItemInformationButton.Visible = false;
            SearchFormTextBox.Visible = false;
            SearchFormTextBoxClearButton.Visible = false;
            SearchOptionComboBox.Visible = false;
            SearchMethodComboBox.Visible = false;
            AddContentsButton.Visible = false;
            ShowListButton.Visible = true;
            ShowPicturesMethod();
        }
        /// <summary>
        /// 全体表示画面状態で、詳細表示画面からList表示画面に戻る
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowListButton_Click(object sender, EventArgs e)
        {
            if (SaveAndCloseEditButton.Visible == true)// 編集中の場合は警告を表示
            {
                if (CheckEditingContents() != true)// 編集終了をキャンセルされた場合
                {
                    return;// 何もせず終了
                }
            }
            dataGridView1.Visible = true;
            dataGridView1BackgroundPictureBox.Visible = true;
            ShowSelectedItemInformationButton.Visible = true;
            SearchFormTextBox.Visible = true;
            SearchFormTextBoxClearButton.Visible = true;
            SearchOptionComboBox.Visible = true;
            SearchMethodComboBox.Visible = true;
            AddContentsButton.Visible = true;
            ListUpdateButton.Visible = true;
            ShowListButton.Visible = false;
            ClosePicturesViewMethod();// 画像表示モードを閉じるメソッドを呼び出し
            CloseInventoryViewMethod();// 在庫管理モードを閉じるメソッドを呼び出し
        }
        private void SetColorMethod()// 色設定のメソッド
        {
            switch (CurrentProjectSettingValues.ColorSetting)
            {
                case ColorValue.Blue:
                    this.BackColor = Color.AliceBlue;
                    ShowListButton.BackColor = Color.AliceBlue;
                    menuStrip1.BackColor = SystemColors.InactiveCaption;
                    ShowProjcetNameTextBox.BackColor = SystemColors.InactiveCaption;
                    dataGridView1.BackgroundColor = SystemColors.InactiveCaption;
                    InventoryModeDataGridView.BackgroundColor = SystemColors.InactiveCaption;
                    Thumbnail.BackColor = SystemColors.InactiveCaption;
                    NoImageLabel.BackColor = SystemColors.InactiveCaption;
                    AliceBlueToolStripMenuItem.Checked = true;
                    HoneydewToolStripMenuItem.Checked = false;
                    LavenderBlushToolStripMenuItem.Checked = false;
                    WhiteSmokeToolStripMenuItem.Checked = false;
                    DarkToolStripMenuItem.Checked = false;
                    break;
                case ColorValue.White:
                    this.BackColor = Color.WhiteSmoke;
                    ShowListButton.BackColor = Color.WhiteSmoke;
                    menuStrip1.BackColor = Color.Gainsboro;
                    ShowProjcetNameTextBox.BackColor = Color.Gainsboro;
                    dataGridView1.BackgroundColor = SystemColors.ControlDark;
                    InventoryModeDataGridView.BackgroundColor = SystemColors.ControlDark;
                    Thumbnail.BackColor = Color.Gainsboro;
                    NoImageLabel.BackColor = Color.Gainsboro;
                    WhiteSmokeToolStripMenuItem.Checked = true;
                    LavenderBlushToolStripMenuItem.Checked = false;
                    AliceBlueToolStripMenuItem.Checked = false;
                    HoneydewToolStripMenuItem.Checked = false;
                    DarkToolStripMenuItem.Checked = false;
                    break;
                case ColorValue.Sakura:
                    this.BackColor = Color.LavenderBlush;
                    ShowListButton.BackColor = Color.LavenderBlush;
                    menuStrip1.BackColor = Color.LightPink;
                    ShowProjcetNameTextBox.BackColor = Color.LightPink;
                    dataGridView1.BackgroundColor = Color.LightPink;
                    InventoryModeDataGridView.BackgroundColor = Color.LightPink;
                    Thumbnail.BackColor = Color.LightPink;
                    NoImageLabel.BackColor = Color.LightPink;
                    LavenderBlushToolStripMenuItem.Checked = true;
                    AliceBlueToolStripMenuItem.Checked = false;
                    HoneydewToolStripMenuItem.Checked = false;
                    WhiteSmokeToolStripMenuItem.Checked = false;
                    DarkToolStripMenuItem.Checked = false;
                    break;
                case ColorValue.Green:
                    this.BackColor = Color.Honeydew;
                    ShowListButton.BackColor = Color.Honeydew;
                    menuStrip1.BackColor = Color.FromArgb(192, 255, 192);
                    ShowProjcetNameTextBox.BackColor = Color.FromArgb(192, 255, 192);
                    dataGridView1.BackgroundColor = Color.FromArgb(192, 255, 192);
                    InventoryModeDataGridView.BackgroundColor = Color.FromArgb(192, 255, 192);
                    Thumbnail.BackColor = Color.FromArgb(192, 255, 192);
                    NoImageLabel.BackColor = Color.FromArgb(192, 255, 192);
                    HoneydewToolStripMenuItem.Checked = true;
                    AliceBlueToolStripMenuItem.Checked = false;
                    LavenderBlushToolStripMenuItem.Checked = false;
                    WhiteSmokeToolStripMenuItem.Checked = false;
                    DarkToolStripMenuItem.Checked = false;
                    break;
                default:
                    this.BackColor = Color.AliceBlue;
                    ShowListButton.BackColor = Color.AliceBlue;
                    menuStrip1.BackColor = SystemColors.InactiveCaption;
                    ShowProjcetNameTextBox.BackColor = SystemColors.InactiveCaption;
                    dataGridView1.BackgroundColor = SystemColors.InactiveCaption;
                    InventoryModeDataGridView.BackgroundColor = SystemColors.InactiveCaption;
                    Thumbnail.BackColor = SystemColors.InactiveCaption;
                    NoImageLabel.BackColor = SystemColors.InactiveCaption;
                    AliceBlueToolStripMenuItem.Checked = true;
                    HoneydewToolStripMenuItem.Checked = false;
                    LavenderBlushToolStripMenuItem.Checked = false;
                    WhiteSmokeToolStripMenuItem.Checked = false;
                    DarkToolStripMenuItem.Checked = false;
                    break;
            }
        }
        #endregion

        private void RecentShownContentsToolStripMenuItem_Click(object sender, EventArgs e)// 最近表示した項目
        {
            // 実装検討中
        }

        #region 言語設定
        private void LanguageSettingToolStripMenuItem_MouseEnter(object sender, EventArgs e)// 表示言語リストを表示
        {
            LanguageSettingToolStripMenuItem.DropDownItems.Clear();
            int count = -1;
            try
            {
                System.IO.DirectoryInfo directoryInfo = new DirectoryInfo("language");
                if (!directoryInfo.Exists)
                {
                    MessageBox.Show("言語フォルダが見つかりません。デフォルトの言語ファイルを作成します。\nNo Language Folder.", "CREC");
                    Directory.CreateDirectory("language");
                    if (!LanguageSettingClass.MakeDefaultLanguageFileJP())
                    {
                        MessageBox.Show("致命的なエラーが発生しました。アプリケーションを終了します。", "CREC");
                        Environment.FailFast("Default language file can't find.");
                    }
                    CurrentLanguageFileName = "Japanese";
                    SetLanguage("language\\" + CurrentLanguageFileName + ".xml");
                    LanguageSettingClass.MakeDefaultLanguageFileEN();
                    return;
                }
                System.IO.FileInfo[] fileInfos = directoryInfo.GetFiles("*.xml");
                if (fileInfos.Length > 0)
                {
                    LanguageSettingToolStripMenuItems = new ToolStripMenuItem[fileInfos.Length];
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        LanguageSettingToolStripMenuItem.Enabled = true;
                        count++;
                        LanguageSettingToolStripMenuItems[count] = new ToolStripMenuItem();
                        LanguageFile = XElement.Load(fileInfo.FullName);
                        IEnumerable<string> strings = from item in LanguageFile.Elements("metadata").Elements("name") select item.Value;
                        foreach (string s in strings)
                        {
                            LanguageSettingToolStripMenuItems[count].Text = s;
                            LanguageSettingToolStripMenuItems[count].ToolTipText = fileInfo.FullName;
                            LanguageSettingToolStripMenuItem.DropDownItems.Add(LanguageSettingToolStripMenuItems[count]);
                            LanguageSettingToolStripMenuItems[count].Click += new EventHandler(LanguageSettingToolStripMenuItems_Click);
                            if (CurrentLanguageFileName == Path.GetFileNameWithoutExtension(fileInfo.FullName))
                            {
                                LanguageSettingToolStripMenuItems[count].Checked = true;
                            }
                        }
                    }
                }
                else
                {
                    LanguageSettingToolStripMenuItem.Text = "言語ファイルが見つかりません。デフォルトの言語ファイルを作成します。";
                    if (!LanguageSettingClass.MakeDefaultLanguageFileJP())
                    {
                        MessageBox.Show("致命的なエラーが発生しました。アプリケーションを終了します。", "CREC");
                        Environment.FailFast("Default language file can't find.");
                    }
                    CurrentLanguageFileName = "Japanese";
                    SetLanguage("language\\" + CurrentLanguageFileName + ".xml");
                    LanguageSettingClass.MakeDefaultLanguageFileEN();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("言語ファイル取得中にエラーが発生しました。\n" + ex.Message, "CREC");
            }
        }
        private void LanguageSettingToolStripMenuItems_Click(object sender, EventArgs e)// 表示言語がクリックされた時のイベント
        {
            int? SelectedRow = null;
            if (dataGridView1.RowCount > 0)// リストにコンテンツが表示されているか確認
            {
                SelectedRow = dataGridView1.SelectedRows[0].Index;// 表示中のコンテンツのIDを取得
            }
            CurrentLanguageFileName = Path.GetFileNameWithoutExtension(((ToolStripItem)sender).ToolTipText);
            SetLanguage(((ToolStripItem)sender).ToolTipText);
            int TargetColumn = 1;
            if (IDListVisibleToolStripMenuItem.Checked) { TargetColumn = 1; }
            else if (MCListVisibleToolStripMenuItem.Checked) { TargetColumn = 2; }
            else if (NameListVisibleToolStripMenuItem.Checked) { TargetColumn = 3; }
            else if (RegistrationDateListVisibleToolStripMenuItem.Checked) { TargetColumn = 4; }
            else if (CategoryListVisibleToolStripMenuItem.Checked) { TargetColumn = 5; }
            else if (Tag1ListVisibleToolStripMenuItem.Checked) { TargetColumn = 6; }
            else if (Tag2ListVisibleToolStripMenuItem.Checked) { TargetColumn = 7; }
            else if (Tag3ListVisibleToolStripMenuItem.Checked) { TargetColumn = 8; }
            else if (InventoryInformationListToolStripMenuItem.Checked) { TargetColumn = 9; }
            if (SelectedRow != null)
            {
                dataGridView1.CurrentCell = dataGridView1[TargetColumn, (int)SelectedRow];// 言語設定前に表示されている項目を復元
            }
            if (AddInventoryOperationButton.Visible)// 在庫管理モードだった場合はボタンを変更
            {
                InventoryManagementModeButton.Visible = false;
                CloseInventoryManagementModeButton.Visible = true;
            }
            if (!SaveAndCloseEditButton.Visible)// 編集モードでない場合は再読み込み
            {
                ShowDetails();
            }
        }
        private void SetLanguage(string targetLanguageFilePath)// 言語ファイル（xml）を読み込んで表示する処理
        {
            LanguageFile = XElement.Load(targetLanguageFilePath);
            this.Text = LanguageSettingClass.GetOtherMessage("FormName", "mainform", LanguageFile);
            IEnumerable<XElement> buttonItemDataList = from item in LanguageFile.Elements("mainform").Elements("Button").Elements("item") select item;
            foreach (XElement itemData in buttonItemDataList)
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
            IEnumerable<XElement> labelItemDataList = from item in LanguageFile.Elements("mainform").Elements("Label").Elements("item") select item;
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
            IEnumerable<XElement> toolStripMenuItemItemDataList = from item in LanguageFile.Elements("mainform").Elements("ToolStripMenuItem").Elements("item") select item;
            foreach (XElement itemData in toolStripMenuItemItemDataList)
            {
                try
                {
                    foreach (var toolStripMenuItem1 in menuStrip1.Items)
                    {
                        if (toolStripMenuItem1 is ToolStripMenuItem)
                        {
                            if (((ToolStripMenuItem)toolStripMenuItem1).Name == itemData.Element("itemname").Value)
                            {
                                ((ToolStripMenuItem)toolStripMenuItem1).Text = itemData.Element("itemtext").Value;
                                break;
                            }
                            foreach (var toolStripMenuItem2 in ((ToolStripMenuItem)toolStripMenuItem1).DropDownItems)
                            {
                                if (toolStripMenuItem2 is ToolStripMenuItem)
                                {
                                    if (((ToolStripMenuItem)toolStripMenuItem2).Name == itemData.Element("itemname").Value)
                                    {
                                        ((ToolStripMenuItem)toolStripMenuItem2).Text = itemData.Element("itemtext").Value;
                                        break;
                                    }
                                    foreach (var toolStripMenuItem3 in ((ToolStripMenuItem)toolStripMenuItem2).DropDownItems)
                                    {
                                        if (toolStripMenuItem3 is ToolStripMenuItem)
                                        {
                                            if (((ToolStripMenuItem)toolStripMenuItem3).Name == itemData.Element("itemname").Value)
                                            {
                                                ((ToolStripMenuItem)toolStripMenuItem3).Text = itemData.Element("itemtext").Value;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                try
                {
                    foreach (var toolStripMenuItem1 in dataGridViewContextMenuStrip.Items)
                    {
                        if (toolStripMenuItem1 is ToolStripMenuItem)
                        {
                            if (((ToolStripMenuItem)toolStripMenuItem1).Name == itemData.Element("itemname").Value)
                            {
                                ((ToolStripMenuItem)toolStripMenuItem1).Text = itemData.Element("itemtext").Value;
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            SetUserAssistToolTips();
            ShowProjcetNameTextBox.Text = LanguageSettingClass.GetOtherMessage("ProjectNameHeader", "mainform", LanguageFile) + CurrentProjectSettingValues.Name;
            ShowListButton.Text = LanguageSettingClass.GetOtherMessage("ShowListButton", "mainform", LanguageFile);
            // 検索方法の表示更新
            SearchMethodComboBox.SelectedIndexChanged -= SearchMethodComboBox_SelectedIndexChanged;
            int CurrentSelectedIndex = SearchMethodComboBox.SelectedIndex;
            SearchMethodComboBox.Items.Clear();
            if (SearchOptionComboBox.SelectedIndex == 7)
            {
                SearchMethodComboBox.Items.Add("欠品");
                SearchMethodComboBox.Items.Add("不足");
                SearchMethodComboBox.Items.Add("適正");
                SearchMethodComboBox.Items.Add("過剰");
                SearchFormTextBox.Clear();
            }
            else
            {
                SearchMethodComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("SearchMethodForwardMatch", "mainform", LanguageFile));
                SearchMethodComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("SearchMethodBroadMatch", "mainform", LanguageFile));
                SearchMethodComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("SearchMethodBackwardMatch", "mainform", LanguageFile));
                SearchMethodComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("SearchMethodExactMatch", "mainform", LanguageFile));
            }
            if (CurrentSelectedIndex != -1)// 表示中の項目を変更
            {
                switch (CurrentSelectedIndex)
                {
                    case 0:
                        SearchMethodComboBox.Text = (LanguageSettingClass.GetOtherMessage("SearchMethodForwardMatch", "mainform", LanguageFile));
                        break;
                    case 1:
                        SearchMethodComboBox.Text = (LanguageSettingClass.GetOtherMessage("SearchMethodBroadMatch", "mainform", LanguageFile));
                        break;
                    case 2:
                        SearchMethodComboBox.Text = (LanguageSettingClass.GetOtherMessage("SearchMethodBackwardMatch", "mainform", LanguageFile));
                        break;
                    case 3:
                        SearchMethodComboBox.Text = (LanguageSettingClass.GetOtherMessage("SearchMethodExactMatch", "mainform", LanguageFile));
                        break;
                }
            }
            SearchMethodComboBox.SelectedIndex = CurrentSelectedIndex;
            SearchMethodComboBox.SelectedIndexChanged += SearchMethodComboBox_SelectedIndexChanged;
            // 在庫管理モード
            int CurrentSelectedProperInventorySettingsComboBox = (int)ProperInventorySettingsComboBox.SelectedIndex;
            ProperInventorySettingsComboBox.Items.Clear();
            ProperInventorySettingsComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("SafetyStock", "mainform", LanguageFile));
            ProperInventorySettingsComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("ReorderPoint", "mainform", LanguageFile));
            ProperInventorySettingsComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("MaximumLevel", "mainform", LanguageFile));
            ProperInventorySettingsComboBox.SelectedIndex = CurrentSelectedProperInventorySettingsComboBox;
            int CurrentSelectedOperationOptionComboBox = (int)OperationOptionComboBox.SelectedIndex;
            OperationOptionComboBox.Items.Clear();
            OperationOptionComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("EntoryOperation", "mainform", LanguageFile));
            OperationOptionComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("ExitOperation", "mainform", LanguageFile));
            OperationOptionComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("Stocktaking", "mainform", LanguageFile));
            OperationOptionComboBox.SelectedIndex = CurrentSelectedOperationOptionComboBox;
            InventoryLabel.Text = Convert.ToString(LanguageSettingClass.GetOtherMessage("Inventory", "mainform", LanguageFile)
                + " : " + InventoryLabel.Text.Split(new string[] { " : " }, StringSplitOptions.None).Last());
            InventoryModeDataGridView.Columns["date"].HeaderText = LanguageSettingClass.GetOtherMessage("InventoryListColumsHeaderDate", "mainform", LanguageFile);
            InventoryModeDataGridView.Columns["operation"].HeaderText = LanguageSettingClass.GetOtherMessage("InventoryListColumsHeaderOperation", "mainform", LanguageFile);
            InventoryModeDataGridView.Columns["quantity"].HeaderText = LanguageSettingClass.GetOtherMessage("InventoryListColumsHeaderQuantity", "mainform", LanguageFile);
            InventoryModeDataGridView.Columns["note"].HeaderText = LanguageSettingClass.GetOtherMessage("InventoryListColumsHeaderNote", "mainform", LanguageFile);
        }
        private void SetUserAssistToolTips()// ユーザーアシストのToolTip設定
        {
            if (ConfigValues.ShowUserAssistToolTips == true)
            {
                UserAssistToolTip.SetToolTip(SearchFormTextBox, LanguageSettingClass.GetToolTipMessage("SearchFormTextBox", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(SearchOptionComboBox, LanguageSettingClass.GetToolTipMessage("SearchOptionComboBox", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(SearchMethodComboBox, LanguageSettingClass.GetToolTipMessage("SearchMethodComboBox", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(EditIDTextBox, LanguageSettingClass.GetToolTipMessage("EditIDTextBox", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(EditMCTextBox, LanguageSettingClass.GetToolTipMessage("EditMCTextBox", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(AddContentsButton, LanguageSettingClass.GetToolTipMessage("AddContentsButton", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(ListUpdateButton, LanguageSettingClass.GetToolTipMessage("ListUpdateButton", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(SearchButton, LanguageSettingClass.GetToolTipMessage("SearchButton", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(SearchFormTextBoxClearButton, LanguageSettingClass.GetToolTipMessage("SearchFormTextBoxClearButton", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(OpenDataLocation, LanguageSettingClass.GetToolTipMessage("OpenDataLocation", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(CopyDataLocationPath, LanguageSettingClass.GetToolTipMessage("CopyDataLocationPath", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(ShowPicturesButton, LanguageSettingClass.GetToolTipMessage("ShowPicturesButton", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(OpenPictureFolderButton, LanguageSettingClass.GetToolTipMessage("OpenPictureFolderButton", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(SelectThumbnailButton, LanguageSettingClass.GetToolTipMessage("SelectThumbnailButton", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(EditButton, LanguageSettingClass.GetToolTipMessage("EditButton", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(InventoryManagementModeButton, LanguageSettingClass.GetToolTipMessage("InventoryManagementModeButton", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(CloseInventoryManagementModeButton, LanguageSettingClass.GetToolTipMessage("CloseInventoryManagementModeButton", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(ShowConfidentialDataButton, LanguageSettingClass.GetToolTipMessage("ShowConfidentialDataButton", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(HideConfidentialDataButton, LanguageSettingClass.GetToolTipMessage("HideConfidentialDataButton", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(SetProperInventorySettingsButton, LanguageSettingClass.GetToolTipMessage("SetProperInventorySettingsButton", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(SaveProperInventorySettingsButton, LanguageSettingClass.GetToolTipMessage("SaveProperInventorySettingsButton", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(OperationOptionComboBox, LanguageSettingClass.GetToolTipMessage("OperationOptionComboBox", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(EditQuantityTextBox, LanguageSettingClass.GetToolTipMessage("EditQuantityTextBox", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(EditInventoryOperationNoteTextBox, LanguageSettingClass.GetToolTipMessage("EditInventoryOperationNoteTextBox", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(AddInventoryOperationButton, LanguageSettingClass.GetToolTipMessage("AddInventoryOperationButton", "mainform", LanguageFile));
                UserAssistToolTip.SetToolTip(SaveAndCloseEditButton, LanguageSettingClass.GetToolTipMessage("SaveAndCloseEditButton", "mainform", LanguageFile));

            }
            else if (ConfigValues.ShowUserAssistToolTips == false)
            {
                UserAssistToolTip.RemoveAll();
            }
        }
        #endregion

    }
}