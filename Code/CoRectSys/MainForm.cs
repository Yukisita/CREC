/*
MainForm
Copyright (c) [2022-2025] [S.Yukisita]
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
using System.Windows.Markup.Localizer;

namespace CREC
{
    public partial class MainForm : Form
    {
        // アップデート確認用URLの更新、Release前に変更忘れずに
        #region 定数の宣言
        readonly string LatestVersionDownloadLink = "https://github.com/Yukisita/CREC/releases/download/Latest_Release/CREC_v9.0.1.0.zip";// アップデート確認用URL
        readonly string GitHubLatestReleaseURL = "https://github.com/Yukisita/CREC/releases/tag/Latest_Release";// 最新安定版の公開場所URL
        
        // Windows メッセージ定数（水平スクロール対応用）
        private const int WM_MOUSEHWHEEL = 0x020E;
        #endregion
        #region 変数の宣言
        // プロジェクトファイル読み込み用変数
        ConfigValuesClass ConfigValues = new ConfigValuesClass();// config.sys読み込み用Class
        ProjectSettingValuesClass CurrentProjectSettingValues = new ProjectSettingValuesClass();// 現在表示中のプロジェクトの設定値
        List<CollectionDataValuesClass> allCollectionList = new List<CollectionDataValuesClass>();// 全データのコレクションリスト
        List<CollectionDataValuesClass> searchedCollectionList = new List<CollectionDataValuesClass>();// 検索結果のコレクションリスト
        CollectionDataValuesClass CurrentShownCollectionData = new CollectionDataValuesClass();// 詳細表示中のコレクション
        CollectionOperationStatus CurrentShownCollectionOperationStatus = new CollectionOperationStatus();// 詳細表示中のコレクションの操作状況

        string[] cols;// List等読み込み用
        ToolStripMenuItem[] LanguageSettingToolStripMenuItems;// 言語リスト
        ToolStripMenuItem[] RecentShownCollectionsToolStripMenuItems;// 最近使用したコレクションのリスト

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
            // カレントディレクトリを実行ファイルのディレクトリに変更
            Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
            // コマンドライン引数を取得
            string commandLineProjectFile = string.Empty;
            var commandLineArgument = System.Environment.GetCommandLineArgs();
            try// コマンドライン引数2つ目から選択されたプロジェクトのパスを取得
            {
                commandLineProjectFile = commandLineArgument[1];
            }
            catch// 取得できなかったら諦める
            { }
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
            // 水平スクロール対応のためのマウスホイールイベントハンドラを追加
            // Shift+マウスホイール または Ctrl+マウスホイール で水平スクロールが可能
            // タッチパッドの水平スクロールジェスチャーも自動的に検出して対応
            dataGridView1.MouseWheel += DataGridView1_MouseWheel;
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
            // configファイルの読み込み・自動生成
            ImportConfig();
            // 言語ファイル読み込み
            bootingForm.BootingProgressLabel.Text = "言語ファイル読み込み中";
            Application.DoEvents();
            System.IO.DirectoryInfo directoryInfo = new DirectoryInfo(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\language");
            if (!directoryInfo.Exists)// 言語フォルダの存在確認
            {
                MessageBox.Show("言語フォルダが見つかりません。デフォルトの言語ファイルを作成します。\nNo Language Folder.", "CREC");
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\language");
                if (!LanguageSettingClass.MakeDefaultLanguageFileJP())
                {
                    MessageBox.Show("致命的なエラーが発生しました。アプリケーションを終了します。", "CREC");
                    Environment.FailFast("Default language file can't find.");
                }
                ConfigValues.LanguageFileName = "Japanese";
                SetLanguage(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\language\\" + ConfigValues.LanguageFileName + ".xml");
                LanguageSettingClass.MakeDefaultLanguageFileEN();
            }
            else
            {
                try
                {
                    SetLanguage(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\language\\" + ConfigValues.LanguageFileName + ".xml");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("言語の設定に失敗しました。\n" + ex.Message + "\n言語ファイル（日本語）を作成し、起動します。", "CREC");
                    if (!LanguageSettingClass.MakeDefaultLanguageFileJP())
                    {
                        MessageBox.Show("致命的なエラーが発生しました。アプリケーションを終了します。", "CREC");
                        Environment.FailFast("Default language file can't find.");
                    }
                    ConfigValues.LanguageFileName = "Japanese";
                    SetLanguage(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\language\\" + ConfigValues.LanguageFileName + ".xml");
                }
            }
            SetColorMethod();// 色設定を反映
            SetTagNameToolTips();// ToolTipsの設定
            bootingForm.BootingProgressLabel.Text = "ウインドウレイアウト調整中";
            Application.DoEvents();
            SetFormLayout();
            CheckContentsList(CheckContentsListCancellationTokenSource.Token);// 表示内容整合性確認処理を開始
            // コマンドライン引数で開くプロジェクトが指定されている場合はそちらを優先
            if (commandLineProjectFile != string.Empty && File.Exists(commandLineProjectFile))
            {
                CurrentProjectSettingValues.ProjectSettingFilePath = commandLineProjectFile;
            }
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

        /// <summary>
        /// Windowsメッセージを処理してタッチパッドの水平スクロールを検出
        /// </summary>
        /// <param name="m">メッセージ</param>
        protected override void WndProc(ref Message m)
        {
            // 水平マウスホイール（タッチパッドの水平スクロール）を検出
            if (m.Msg == WM_MOUSEHWHEEL)
            {
                // DataGridViewが表示されていて、フォーカスされている場合のみ処理
                if (dataGridView1.Visible && dataGridView1.Focused)
                {
                    // WPARAMから水平スクロール量を取得
                    int delta = (short)((m.WParam.ToInt32() >> 16) & 0xFFFF);
                    
                    // 水平スクロールを実行（タッチパッドでは左右が逆になることがあるので調整）
                    PerformHorizontalScroll(dataGridView1, -delta);
                    
                    // メッセージを処理済みとしてマーク
                    return;
                }
            }
            
            // その他のメッセージは通常通り処理
            base.WndProc(ref m);
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
            // 自動読み込み設定時は開始（例外処理はImportConfig内で実施済み）
            if (CurrentProjectSettingValues.ProjectSettingFilePath.Length > 0)// 自動読み込みプロジェクトが指定されている場合
            {
                LoadProjectFileMethod(CurrentProjectSettingValues.ProjectSettingFilePath);// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
                if (CurrentProjectSettingValues.StartUpListOutput == true)
                {
                    CollectionListClass.OutputCollectionList(CurrentProjectSettingValues, allCollectionList, LanguageFile);// 一覧を出力
                }
                if (CurrentProjectSettingValues.StartUpBackUp == true)// 自動バックアップ
                {
                    BackUpMethod();
                }
            }
            // 英語モードだとボタンが表示されなくなる不具合対策、原因は不明
            ShowSelectedItemInformationButton.Visible = false;
        }

        #region メニューバー関係
        private void NewProjectToolStripMenuItem_Click(object sender, EventArgs e)// 新規プロジェクト作成
        {
            if (SaveAndCloseEditButton.Visible == true)// 編集中の場合は警告を表示
            {
                if (!CheckEditingContents() == true)// キャンセルされた場合
                {
                    return;
                }
            }
            var previousProjectSettinValues = CurrentProjectSettingValues;// 現在開いているプロジェクトの設定値保持
            CurrentProjectSettingValues = new ProjectSettingValuesClass();
            MakeNewProject makenewproject = new MakeNewProject(CurrentProjectSettingValues, LanguageFile);
            makenewproject.ShowDialog();
            if (makenewproject.ReturnTargetProject.Length != 0)// 新規作成されずにメインフォームに戻ってきた場合を除外
            {
                LoadProjectFileMethod(makenewproject.ReturnTargetProject);// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
            }
            else
            {
                LoadProjectFileMethod(previousProjectSettinValues.ProjectSettingFilePath);// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
            }
        }
        private void OpenMenu_Click(object sender, EventArgs e)// 在庫管理プロジェクト読み込み、OpenProjectContextStripMenuItem_Clickと同じ
        {
            OpenProjectMethod();// 既存の在庫管理プロジェクトを読み込むメソッドを呼び出し
        }
        /// <summary>
        /// 既存のプロジェクトを読み込むメソッド
        /// </summary>
        private void OpenProjectMethod()
        {
            if (CurrentShownCollectionData.CollectionFolderPath.Length > 0)// 開いているプロジェクトがあった場合は内容を保存
            {
                ProjectSettingClass.SaveProjectSetting(CurrentProjectSettingValues, LanguageFile);
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
                openFolderDialog.Dispose();
                LoadProjectFileMethod(openFolderDialog.FileName);// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
                if (CurrentProjectSettingValues.StartUpListOutput == true)// 自動リスト出力
                {
                    CollectionListClass.OutputCollectionList(CurrentProjectSettingValues, allCollectionList, LanguageFile);// 一覧を出力
                }
                if (CurrentProjectSettingValues.StartUpBackUp == true)// 自動バックアップ
                {
                    BackUpMethod();
                }
            }
            else
            {
                openFolderDialog.Dispose();
            }
        }
        private void LoadProjectFileMethod(string targetProjectSettingFilePath)// CREC読み込み用の処理メソッド
        {
            var previousProjectSettinValues = CurrentProjectSettingValues; // 現在のプロジェクト設定値を保持
            CurrentProjectSettingValues = new ProjectSettingValuesClass();// プロジェクト設定値を初期化
            CurrentProjectSettingValues.ProjectSettingFilePath = targetProjectSettingFilePath;// プロジェクトファイルのパスを設定
            ClearDetailsWindowMethod();// 詳細表示画面を初期化
            SearchFormTextBox.Text = string.Empty;// 検索ボックスを初期化
            // 最近開いたコレクションを初期化
            RecentShownCollectionsToolStripMenuItems = new ToolStripMenuItem[10];
            RecentShownContentsToolStripMenuItem.DropDownItems.Clear();

            CollectionListIsShowing(true);// リスト表示
            ShowListButton.Visible = false;
            dataGridView1BackgroundPictureBox.Visible = true;
            ClosePicturesViewMethod();// 画像表示モードを閉じる
            // 読み込み処理呼び出し
            if (!ProjectSettingClass.LoadProjectSetting(ref CurrentProjectSettingValues))
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
            SearchMethodComboBox.Items.Clear();
            if (SearchOptionComboBox.SelectedIndex == 7)
            {
                SearchMethodComboBox.Items.Add(CollectionDataClass.InventoryStatusToString(InventoryStatus.StockOut, LanguageFile));
                SearchMethodComboBox.Items.Add(CollectionDataClass.InventoryStatusToString(InventoryStatus.UnderStocked, LanguageFile));
                SearchMethodComboBox.Items.Add(CollectionDataClass.InventoryStatusToString(InventoryStatus.Appropriate, LanguageFile));
                SearchMethodComboBox.Items.Add(CollectionDataClass.InventoryStatusToString(InventoryStatus.OverStocked, LanguageFile));
                SearchFormTextBox.Clear();
            }
            else
            {
                SearchMethodComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("SearchMethodForwardMatch", "mainform", LanguageFile));
                SearchMethodComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("SearchMethodBroadMatch", "mainform", LanguageFile));
                SearchMethodComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("SearchMethodBackwardMatch", "mainform", LanguageFile));
                SearchMethodComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("SearchMethodExactMatch", "mainform", LanguageFile));
            }
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
                if (File.Exists(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\RecentlyOpenedProjectList.log"))// 履歴が既に存在する場合は読み込み
                {
                    RecentlyOpendProjectList = File.ReadAllLines(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\RecentlyOpenedProjectList.log", Encoding.GetEncoding("UTF-8"));
                    FileOperationClass.DeleteFile(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\RecentlyOpenedProjectList.log");
                    StreamWriter streamWriter = new StreamWriter(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\RecentlyOpenedProjectList.log", true, Encoding.GetEncoding("UTF-8"));
                    streamWriter.WriteLine(CurrentProjectSettingValues.Name + "," + CurrentProjectSettingValues.ProjectSettingFilePath);// 今開いたプロジェクトを書き込み
                    int Count = 0;
                    foreach (string line in RecentlyOpendProjectList)
                    {
                        if (line != CurrentProjectSettingValues.Name + "," + CurrentProjectSettingValues.ProjectSettingFilePath)// 重複を回避
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
                    StreamWriter streamWriter = new StreamWriter(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\RecentlyOpenedProjectList.log", true, Encoding.GetEncoding("UTF-8"));
                    streamWriter.WriteLine(CurrentProjectSettingValues.Name + "," + CurrentProjectSettingValues.ProjectSettingFilePath);// 今開いたプロジェクトを書き込み
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
            if (File.Exists(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\RecentlyOpenedProjectList.log"))// 履歴が既に存在する場合は読み込み
            {
                try
                {
                    RecentlyOpendProjectList = File.ReadAllLines(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\RecentlyOpenedProjectList.log", Encoding.GetEncoding("UTF-8"));
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
            if (CurrentShownCollectionData.CollectionFolderPath.Length > 0)
            {
                ProjectSettingClass.SaveProjectSetting(CurrentProjectSettingValues, LanguageFile);
            }
            // 次に読み込むプロジェクトファイルのパスを設定
            string nextProjectSettingFilePath = OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems[OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems.IndexOf((ToolStripMenuItem)sender)].ToolTipText;
            if (!File.Exists(nextProjectSettingFilePath))// プロジェクトファイルが存在しない場合
            {
                MessageBox.Show("プロジェクトファイルが見つかりませんでした。\nこの項目を「最近使用したプロジェクト」から削除します。", "CREC");
                // 見つからなかったプロジェクトを履歴から削除
                IEnumerable<string> RecentlyOpendProjectList;
                try
                {
                    RecentlyOpendProjectList = File.ReadAllLines(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\RecentlyOpenedProjectList.log", Encoding.GetEncoding("UTF-8"));
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("履歴ファイルの読み込みに失敗しました。\n" + ex.Message, "CREC");
                    return;
                }
                FileOperationClass.DeleteFile(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\RecentlyOpenedProjectList.log");
                StreamWriter streamWriter = new StreamWriter(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\RecentlyOpenedProjectList.log", true, Encoding.GetEncoding("UTF-8"));
                foreach (string line in RecentlyOpendProjectList)
                {
                    if (!line.Contains(nextProjectSettingFilePath))// 見つからなかったプロジェクト以外は書き込み
                    {
                        streamWriter.WriteLine(line);
                    }
                }
                streamWriter.Close();
                return;
            }
            // 選択されたプロジェクトを読み込み
            DataLoadingStatus = "false";
            LoadProjectFileMethod(nextProjectSettingFilePath);// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
        }
        private void DeleteRecentlyOpendProjectListToolStripMenuItem_Click(object sender, EventArgs e)// 最近使用したプロジェクトの履歴を削除（イベント）
        {
            if (File.Exists(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\RecentlyOpenedProjectList.log"))// 履歴が既に存在する場合は読み込み
            {
                try
                {
                    FileOperationClass.DeleteFile(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\RecentlyOpenedProjectList.log");
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
        }
        private void OpenBackUpFolderToolStripMenuItem_Click(object sender, EventArgs e)// バックアップ保存場所を開く
        {
            if (CurrentShownCollectionData.CollectionFolderPath.Length == 0)
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
            CollectionListClass.OutputCollectionList(CurrentProjectSettingValues, allCollectionList, LanguageFile);// 一覧を出力
        }
        private void OutputListShownContentsToolStripMenuItem_Click(object sender, EventArgs e)// 一覧に表示中のデータのみ一覧をListに出力
        {
            CollectionListClass.OutputCollectionList(CurrentProjectSettingValues, searchedCollectionList, LanguageFile);// 一覧を出力
        }
        private void EditConfigSysToolStripMenuItem_Click(object sender, EventArgs e)// 環境設定編集画面
        {
            ConfigForm configform = new ConfigForm(ConfigValues, CurrentProjectSettingValues.ColorSetting, LanguageFile);
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
                    if (!Directory.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData"))
                    {
                        Directory.CreateDirectory(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData");
                    }
                    FileOperationClass.AddBlankFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\FREE");
                    FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\DED");
                    FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED");
                    // サムネ画像が更新されていた場合は一時データをを削除
                    if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\NewThumbnail.png"))
                    {
                        FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\NewThumbnail.png");
                    }
                    // 新規作成の場合はデータを削除
                    if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\ADD"))
                    {
                        DeleteContent();
                    }
                    else
                    {
                        FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\ADD");
                    }
                }
                else if (result == System.Windows.MessageBoxResult.Cancel)// アプリ再起動をキャンセル
                {
                    return;
                }
            }
            if (CurrentProjectSettingValues.ProjectSettingFilePath.Length != 0)
            {
                ProjectSettingClass.SaveProjectSetting(CurrentProjectSettingValues, LanguageFile);
            }
            ConfigClass.SaveConfigValues(ConfigValues, CurrentProjectSettingValues.ProjectSettingFilePath);
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
                    streamReaderDetailData = new StreamReader(CurrentShownCollectionData.CollectionFolderPath + "\\details.txt");
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
                    streamReaderConfidentialData = new StreamReader(CurrentShownCollectionData.CollectionFolderPath + "\\confidentialdata.txt");
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
                EditNameTextBox.Text = CurrentShownCollectionData.CollectionName;
                EditIDTextBox.TextChanged -= IDTextBox_TextChanged;// ID重複確認イベントを停止
                EditIDTextBox.Text = CurrentShownCollectionData.CollectionID;
                EditIDTextBox.TextChanged += IDTextBox_TextChanged;// ID重複確認イベントを開始
                AllowEditIDButton.Visible = true;
                UUIDEditStatusLabel.Visible = false;
                EditMCTextBox.Text = CurrentShownCollectionData.CollectionMC;
                EditRegistrationDateTextBox.Text = CurrentShownCollectionData.CollectionRegistrationDate;
                EditCategoryTextBox.Text = CurrentShownCollectionData.CollectionCategory;
                EditTag1TextBox.Text = CurrentShownCollectionData.CollectionTag1;
                EditTag2TextBox.Text = CurrentShownCollectionData.CollectionTag2;
                EditTag3TextBox.Text = CurrentShownCollectionData.CollectionTag3;
                EditRealLocationTextBox.Text = CurrentShownCollectionData.CollectionRealLocation;
            }
        }
        private void AddInventoryModeToolStripMenuItem_Click(object sender, EventArgs e)// 在庫数管理モードを追加
        {
            if (CurrentProjectSettingValues.ProjectSettingFilePath.Length == 0)// プロジェクトが開かれていない場合のエラー
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (CurrentShownCollectionData.CollectionFolderPath.Length == 0)
            {
                MessageBox.Show("表示するデータを選択し、詳細表示してください。", "CREC");
                return;
            }
            if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\inventory.inv"))// 在庫数管理モードの表示・非表示
            {
                MessageBox.Show("在庫数管理ファイルは作成済みです。", "CREC");
            }
            else
            {
                FileOperationClass.AddBlankFile(CurrentShownCollectionData.CollectionFolderPath + "\\inventory.inv");
                StreamWriter InventoryManagementFile = new StreamWriter(CurrentShownCollectionData.CollectionFolderPath + "\\inventory.inv");
                InventoryManagementFile.WriteLine("{0},,,", CurrentShownCollectionData.CollectionID);
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
            VersionInformation VerInfo = new VersionInformation(CurrentProjectSettingValues, CurrentDPI, LanguageFile);
            VerInfo.ShowDialog();
        }
        private void ReadmeToolStripMenuItem_Click(object sender, EventArgs e)// ReadMe表示
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
            ConfigClass.SaveConfigValues(ConfigValues, CurrentProjectSettingValues.ProjectSettingFilePath);
            if (CurrentProjectSettingValues.ProjectSettingFilePath.Length != 0)
            {
                ProjectSettingClass.SaveProjectSetting(CurrentProjectSettingValues, LanguageFile);
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
                    }
                    else if (result == System.Windows.MessageBoxResult.No)// 保存せずアプリを終了（一時データを削除）
                    {
                        // 編集中タグを削除・解放をマーク
                        if (!Directory.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData"))
                        {
                            Directory.CreateDirectory(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData");
                        }
                        FileOperationClass.AddBlankFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\FREE");
                        FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED");
                        FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\DED");
                        // サムネ画像が更新されていた場合は一時データをを削除
                        if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\NewThumbnail.png"))
                        {
                            FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\NewThumbnail.png");
                        }
                        if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\ADD"))
                        {
                            DeleteContent();
                        }
                        else
                        {
                            FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\ADD");
                        }
                    }
                    else if (result == System.Windows.MessageBoxResult.Cancel)// アプリ終了をキャンセル
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                CollectionEditStatusWatcherStop();// データの監視を停止
                collectionEditStatusWatcher.Dispose();// データの監視を破棄
                CheckContentsListCancellationTokenSource.Dispose();// データ監視用のCancellationTokenSourceを破棄
                // コレクション一覧を出力
                if (CurrentProjectSettingValues.CloseListOutput == true)
                {
                    CollectionListClass.OutputCollectionList(CurrentProjectSettingValues, allCollectionList, LanguageFile);// 一覧を出力
                }
                if (CurrentProjectSettingValues.CloseBackUp == true)
                {
                    this.Hide();// メインフォームを消す
                    CloseBackUpForm closeBackUpForm = new CloseBackUpForm(CurrentProjectSettingValues.ColorSetting.ToString());
                    Task.Run(() => { closeBackUpForm.ShowDialog(); });// 別プロセスでバックアップ中のプログレスバー表示ウインドウを開く
                    // バックアップを実行し、その終了を待機
                    CollectionDataClass.BackupProjectData(
                        CurrentProjectSettingValues,
                        LanguageFile);
                }
            }
        }
        private void DeleteContentToolStripMenuItem_Click(object sender, EventArgs e)// データ完全削除
        {
            if (CurrentShownCollectionData.CollectionFolderPath.Length == 0)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(CurrentShownCollectionData.CollectionName + "を削除しますか？\nこの操作は取り消せません。", "CREC", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
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
            else if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\DED"))
            {
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("他の端末で編集中の可能性があります。\n編集権限を強制的に取得しますか？\nなお、本操作によりデータ破損する可能性があります。", "CREC", System.Windows.MessageBoxButton.YesNo);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    // 編集リクエストタグを削除・解放を宣言
                    if (!Directory.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData"))
                    {
                        Directory.CreateDirectory(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData");
                    }
                    FileOperationClass.AddBlankFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\FREE");
                    FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED");
                    FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\DED");
                    FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\ADD");
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (CurrentProjectSettingValues.ProjectSettingFilePath.Length == 0)// プロジェクトが開かれていない場合のエラー
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (CurrentShownCollectionData.CollectionFolderPath.Length == 0)
                {
                    MessageBox.Show("編集するデータを選択し、詳細表示してください。", "CREC");
                    return;
                }
                StartEditForm();
                // 現時点でのデータを読み込んで表示
                LoadDetails();
                EditNameTextBox.Text = CurrentShownCollectionData.CollectionName;
                EditIDTextBox.TextChanged -= IDTextBox_TextChanged;// ID重複確認イベントを停止
                EditIDTextBox.Text = CurrentShownCollectionData.CollectionID;
                EditIDTextBox.TextChanged += IDTextBox_TextChanged;// ID重複確認イベントを開始
                EditMCTextBox.Text = CurrentShownCollectionData.CollectionMC;
                EditRegistrationDateTextBox.Text = CurrentShownCollectionData.CollectionRegistrationDate;
                EditCategoryTextBox.Text = CurrentShownCollectionData.CollectionCategory;
                EditTag1TextBox.Text = CurrentShownCollectionData.CollectionTag1;
                EditTag2TextBox.Text = CurrentShownCollectionData.CollectionTag2;
                EditTag3TextBox.Text = CurrentShownCollectionData.CollectionTag3;
                EditRealLocationTextBox.Text = CurrentShownCollectionData.CollectionRealLocation;
            }
        }
        private void EditProjectToolStripMenuItem_Click(object sender, EventArgs e)// プロジェクト管理ファイルの編集
        {
            if (CurrentProjectSettingValues.ProjectSettingFilePath.Length == 0)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                if (SaveAndCloseEditButton.Visible == true)// 編集中の場合は警告を表示
                {
                    if (CheckEditingContents() != true)
                    {
                        return;
                    }
                }
                MakeNewProject makenewproject = new MakeNewProject(CurrentProjectSettingValues, LanguageFile);
                makenewproject.ShowDialog();
                if (makenewproject.ReturnTargetProject.Length != 0)// メインフォームに戻ってきたときの処理
                {
                    LoadProjectFileMethod(makenewproject.ReturnTargetProject);// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
                }
            }
        }
        private void ProjectInformationToolStripMenuItem_Click(object sender, EventArgs e)// プロジェクト情報の表示
        {
            if (CurrentProjectSettingValues.ProjectSettingFilePath.Length == 0)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                ProjectInfoForm projectInfoForm = new ProjectInfoForm(CurrentProjectSettingValues, LanguageFile);
                projectInfoForm.ShowDialog();
            }
        }
        #endregion

        #region データ一覧・詳細表示関係
        private void CollectionListIsShowing(bool status)
        {
            // 一覧表示中に表示する項目
            AddContentsButton.Visible = status;
            dataGridView1.Visible = status;
            ListUpdateButton.Visible = status;
            SearchFormTextBox.Visible = status;
            SearchFormTextBoxClearButton.Visible = status;
            SearchMethodComboBox.Visible = status;
            SearchOptionComboBox.Visible = status;
            if (ConfigValues.AutoSearch == false && status == true)
            {
                SearchButton.Visible = true;// 自動検索OFFかつ一覧表示中は検索ボタンを表示
            }
            else
            {
                SearchButton.Visible = false;
            }
            if (FullDisplayModeToolStripMenuItem.Checked == true && status == true)
            {
                ShowSelectedItemInformationButton.Visible = true;// 全画面表示中かつ一覧表示中は詳細表示ボタンを表示
            }
            else
            {
                ShowSelectedItemInformationButton.Visible = false;
            }
        }
        private async void LoadGrid()// データを読み込んでリストに表示
        {
            // 表示内容整合性確認処理を停止
            CheckContentsListCancellationTokenSource.Cancel();
            CheckContentsListCancellationTokenSource = new CancellationTokenSource();

            while (DataLoadingStatus != "false")
            {
                await Task.Delay(1);
            }
            DataLoadingStatus = "true";
            DataLoadingLabel.Visible = true;
            this.Cursor = Cursors.WaitCursor;

            // 一覧読み込み中に禁止する操作
            AddContentsButton.Enabled = false;// コレクション追加ボタンを無効化
            AddContentsButton.BackColor = System.Drawing.SystemColors.Control;// ボタンを無効状態の色に変更
            Application.DoEvents();

            string currentSelectedCollectionID = CurrentShownCollectionData.CollectionID;// 現時点で表示されているデータのID
            // DataGridView関係
            ContentsDataTable.Rows.Clear();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            try
            {
                try
                {
                    // プロジェクトフォルダ内のコレクションを全検索
                    CollectionListClass.LoadCollectionList(CurrentProjectSettingValues, ref allCollectionList, LanguageFile);

                    // 検索処理
                    searchedCollectionList = allCollectionList;
                    CollectionListClass.SearchCollectionFromList(ref searchedCollectionList, SearchFormTextBox.Text, SearchOptionComboBox.SelectedIndex, SearchMethodComboBox.SelectedIndex, LanguageFile);

                    // ContentsDataTable.Rowsに追加
                    foreach (var thisCollectionDataValues in searchedCollectionList)
                    {
                        ContentsDataTable.Rows.Add(
                            thisCollectionDataValues.CollectionFolderPath,
                            thisCollectionDataValues.CollectionID,
                            thisCollectionDataValues.CollectionMC,
                            thisCollectionDataValues.CollectionName,
                            thisCollectionDataValues.CollectionRegistrationDate,
                            thisCollectionDataValues.CollectionCategory,
                            thisCollectionDataValues.CollectionTag1,
                            thisCollectionDataValues.CollectionTag2,
                            thisCollectionDataValues.CollectionTag3,
                            thisCollectionDataValues.CollectionCurrentInventory,
                            CollectionDataClass.InventoryStatusToString(thisCollectionDataValues.CollectionInventoryStatus, LanguageFile));
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

            // 一覧読み込み中に禁止した操作を復元
            Application.DoEvents();// 一覧読み込み中に発生していたイベントを削除
            AddContentsButton.Enabled = true;// コレクション追加ボタンを有効化
            AddContentsButton.UseVisualStyleBackColor = true;// ボタンを通常状態の色に変更

            // データが存在する/しないで場合分け
            if (allCollectionList.Count > 0)// データが存在する場合は更新前に選択されていたデータを復元
            {
                if (searchedCollectionList.Count > 0)// 検索結果にコレクションが含まれている場合
                {
                    if (DataLoadingStatus == "true") // 更新前にコレクションが選択されていた場合は、フォーカスを復元
                    {
                        dataGridView1.ClearSelection();
                        // dataGridView1で表示中の内容から、UUIDが一致するコレクションの場所を取得
                        int currentSelectedCollectionRows = 0;// 表示されていたデータのリスト更新後の列番号
                        foreach (DataGridViewRow collection in dataGridView1.Rows)
                        {
                            // 更新前に選択されていたコレクションの行番号を取得
                            if (currentSelectedCollectionID.Length != 0)
                            {
                                if (currentSelectedCollectionID == Convert.ToString(collection.Cells[1].Value))
                                {
                                    currentSelectedCollectionRows = collection.Index;
                                }
                            }
                        }

                        try// 表示されている一番左のセルの番号を取得して、コレクションのフォーカスを復元
                        {
                            dataGridView1.Rows[currentSelectedCollectionRows].Selected = true;
                            dataGridView1.CurrentCell =
                                dataGridView1.Rows[currentSelectedCollectionRows].
                                Cells[dataGridView1.FirstDisplayedCell.ColumnIndex];
                        }
                        catch (Exception ex)// 失敗した場合
                        {
                            MessageBox.Show(ex.Message, "CREC");
                        }

                        DataLoadingLabel.Visible = false;
                        this.Cursor = Cursors.Default;
                        DataLoadingStatus = "false";
                    }
                }
                else// 検索結果にコレクションが存在しない場合
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("SearchedCollectionNofFoundMessage", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else// データが１つも存在しない場合は新規データ作成するか確認
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
            ProjectSettingClass.SaveProjectSetting(CurrentProjectSettingValues, LanguageFile);
            // 一応読み込み終了を宣言
            DataLoadingLabel.Visible = false;
            this.Cursor = Cursors.Default;
            DataLoadingStatus = "false";
            CheckContentsList(CheckContentsListCancellationTokenSource.Token);// 表示内容整合性確認処理を再開
        }
        private void DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)// 詳細表示
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

        private void DataGridView1_MouseWheel(object sender, MouseEventArgs e)// 水平スクロール対応
        {
            DataGridView dgv = sender as DataGridView;
            if (dgv == null) return;

            // Shiftキーが押されているか、またはCtrlキーが押されている場合に水平スクロールを実行
            if (ModifierKeys == Keys.Shift || ModifierKeys == Keys.Control)
            {
                PerformHorizontalScroll(dgv, e.Delta);
                // イベントを処理済みとしてマークし、縦スクロールを防ぐ
                ((HandledMouseEventArgs)e).Handled = true;
            }
        }

        /// <summary>
        /// 水平スクロールを実行するヘルパーメソッド
        /// </summary>
        /// <param name="dgv">対象のDataGridView</param>
        /// <param name="delta">スクロール量（正の値で左、負の値で右）</param>
        private void PerformHorizontalScroll(DataGridView dgv, int delta)
        {
            // 水平スクロール量を計算 (マウスホイールの方向を考慮)
            int scrollAmount = delta > 0 ? -1 : 1; // より細かいスクロール制御
            
            // 現在の表示されている最初の列のインデックスを取得
            int currentColumnIndex = dgv.FirstDisplayedScrollingColumnIndex;
            
            // 新しい列インデックスを計算
            int newColumnIndex = currentColumnIndex + scrollAmount;
            
            // 範囲チェック: 表示可能な列の範囲内に制限
            if (newColumnIndex < 0)
            {
                newColumnIndex = 0;
            }
            else if (newColumnIndex >= dgv.ColumnCount)
            {
                newColumnIndex = dgv.ColumnCount - 1;
            }
            
            // 水平スクロールを実行
            try
            {
                if (newColumnIndex != currentColumnIndex && newColumnIndex >= 0 && newColumnIndex < dgv.ColumnCount)
                {
                    // 指定した列が見える状態であることを確認
                    var targetColumn = dgv.Columns[newColumnIndex];
                    if (targetColumn != null && targetColumn.Visible)
                    {
                        dgv.FirstDisplayedScrollingColumnIndex = newColumnIndex;
                    }
                    else
                    {
                        // 非表示列をスキップして次の表示列を探す
                        int direction = scrollAmount > 0 ? 1 : -1;
                        for (int i = newColumnIndex; i >= 0 && i < dgv.ColumnCount; i += direction)
                        {
                            if (dgv.Columns[i].Visible)
                            {
                                dgv.FirstDisplayedScrollingColumnIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                // スクロール操作でエラーが発生した場合は無視
            }
        }
        private void OpenDataLocation_Click(object sender, EventArgs e)// ファイルの場所を表示
        {
            CollectionDataClass.OpenCollectionDataFolder(CurrentShownCollectionData, LanguageFile);
        }
        private void CopyDataLocationPath_Click(object sender, EventArgs e)// ファイルのパスをクリップボードにコピー
        {
            try
            {
                Clipboard.SetText(CurrentShownCollectionData.CollectionFolderPath + "\\data");
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
                if (CurrentProjectSettingValues.ProjectSettingFilePath.Length == 0)// プロジェクトが開かれていない場合のエラー
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (CurrentShownCollectionData.CollectionFolderPath.Length == 0)
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
                if (!Directory.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData"))
                {
                    Directory.CreateDirectory(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData");
                }
                FileOperationClass.AddBlankFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\FREE");
                FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED");
                FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\DED");
                // サムネ画像が更新されていた場合は一時データをを削除
                if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\NewThumbnail.png"))
                {
                    FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\NewThumbnail.png");
                }
                if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\ADD"))// データ追加時は削除
                {
                    DeleteContent();
                }
                else
                {
                    FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\ADD");
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
            else if (result == System.Windows.MessageBoxResult.Cancel)// コレクションのフォーカスを解除して、そのまま続行
            {
                dataGridView1.ClearSelection();
                return false;
            }
            // 上記条件以外、エラー発生時
            return false;
        }
        private void ShowDetails()// 詳細情報の表示
        {
            // 読み込み中の画面に切り替え
            NoImageLabel.Visible = true;
            Thumbnail.Image = null;
            Thumbnail.BackColor = menuStrip1.BackColor;

            // 詳細情報読み込み
            if (LoadDetails() != true)
            {
                ObjectNameLabel.Text = CurrentProjectSettingValues.CollectionNameLabel + "：";
                ShowObjectName.Text = CurrentShownCollectionData.CollectionName;
                IDLabel.Text = CurrentProjectSettingValues.UUIDLabel + "：";
                ShowID.Text = CurrentShownCollectionData.CollectionID;
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

            // 在庫管理データ読み込み
            if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\inventory.inv"))// 在庫数管理モードの表示・非表示
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
            ShowObjectName.Text = CurrentShownCollectionData.CollectionName;
            IDLabel.Text = CurrentProjectSettingValues.UUIDLabel + "：";
            ShowID.Text = CurrentShownCollectionData.CollectionID;
            MCLabel.Text = CurrentProjectSettingValues.ManagementCodeLabel + "：";
            ShowMC.Text = CurrentShownCollectionData.CollectionMC;
            RegistrationDateLabel.Text = CurrentProjectSettingValues.RegistrationDateLabel + "：";
            ShowRegistrationDate.Text = CurrentShownCollectionData.CollectionRegistrationDate;
            CategoryLabel.Text = CurrentProjectSettingValues.CategoryLabel + "：";
            ShowCategory.Text = CurrentShownCollectionData.CollectionCategory;
            Tag1NameLabel.Text = CurrentProjectSettingValues.FirstTagLabel + "：";
            ShowTag1.Text = CurrentShownCollectionData.CollectionTag1;
            Tag2NameLabel.Text = CurrentProjectSettingValues.SecondTagLabel + "：";
            ShowTag2.Text = CurrentShownCollectionData.CollectionTag2;
            Tag3NameLabel.Text = CurrentProjectSettingValues.ThirdTagLabel + "：";
            ShowTag3.Text = CurrentShownCollectionData.CollectionTag3;
            RealLocationLabel.Text = CurrentProjectSettingValues.RealLocationLabel + "：";
            ShowRealLocation.Text = CurrentShownCollectionData.CollectionRealLocation;
            ShowDataLocation.Text = CurrentProjectSettingValues.DataLocationLabel + "：";
            Application.DoEvents();
            ShowPicturesButton.Visible = true;
            OpenPictureFolderButton.Visible = false;

            // 詳細情報読み込み
            StreamReader streamReaderDetailData = null;
            try
            {
                streamReaderDetailData = new StreamReader(CurrentShownCollectionData.CollectionFolderPath + "\\details.txt");
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
                streamReaderConfidentialData = new StreamReader(CurrentShownCollectionData.CollectionFolderPath + "\\confidentialdata.txt");
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

            CurrentShownCollectionOperationStatus = CollectionOperationStatus.Watching;// 現在の操作ステータスを監視中に設定
            CollectionOperationStatusManager();// 編集ボタンの表示内容設定
            AddRecentShownCollectiontoToolStripMenuItem();// 最近表示した項目としてUUID、名称を保存
        }
        private bool LoadDetails()// 詳細情報を読み込み
        {
            // 変数初期化
            ClearDetailsWindowMethod();
            // dataGridViewで選択されている項目があるか確認
            if (dataGridView1.CurrentRow == null)
            {
                return false;
            }
            try
            {
                CurrentShownCollectionData.CollectionFolderPath = Convert.ToString(dataGridView1.CurrentRow.Cells[0].Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("詳細情報の読み込みに失敗しました。\n" + ex.Message, "CREC");
                return false;
            }
            CollectionEditStatusWatcherStart(ref CurrentShownCollectionData);// 編集監視スレッドの開始
            // index読み込み
            return CollectionDataClass.LoadCollectionIndexData(CurrentShownCollectionData.CollectionFolderPath, ref CurrentShownCollectionData, LanguageFile);
        }
        private void ClearDetailsWindowMethod()// 表示されている詳細情報・入力フォームの情報を全てリセットするメソッド
        {
            // 詳細表示の表示内容を初期化
            CurrentShownCollectionData = new CollectionDataValuesClass();
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
                System.Diagnostics.Process.Start("EXPLORER.EXE", CurrentShownCollectionData.CollectionFolderPath + "\\pictures");
            }
            catch (Exception ex)
            {
                MessageBox.Show("フォルダを開けませんでした\n" + ex.Message, "CREC");
                return;
            }
        }
        private void ShowPicturesMethod()// 詳細画像表示用のメソッド
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(CurrentShownCollectionData.CollectionFolderPath + "\\pictures");
            System.IO.FileInfo[] fileInfos;
            try
            {
                fileInfos = di.GetFiles("*.jpg", System.IO.SearchOption.AllDirectories).
                    Concat(di.GetFiles("*.jpeg", System.IO.SearchOption.AllDirectories)).
                    Concat(di.GetFiles("*.jfif", System.IO.SearchOption.AllDirectories)).
                    Concat(di.GetFiles("*.jpe", System.IO.SearchOption.AllDirectories)).
                    Concat(di.GetFiles("*.png", System.IO.SearchOption.AllDirectories)).
                    Concat(di.GetFiles("*.gif", System.IO.SearchOption.AllDirectories)).
                    Concat(di.GetFiles("*.ico", System.IO.SearchOption.AllDirectories)).ToArray();
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
            CollectionListIsShowing(false);
            CloseInventoryViewMethod();// 在庫管理モードを閉じるメソッドを呼び出し
            foreach (System.IO.FileInfo fileInfo in fileInfos)
            {
                PicturesList.Add(fileInfo.FullName);
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
            CollectionListIsShowing(true);// 一覧表示モードに戻る
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
            if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\DED") && File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED"))
            {
                ReadOnlyButton.Visible = true;
                EditButton.Visible = false;
                EditRequestButton.Visible = false;
                return;
            }
            else if (!File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\DED") && !File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED"))
            {
                if (CurrentProjectSettingValues.ProjectSettingFilePath.Length == 0)// プロジェクトが開かれていない場合のエラー
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (CurrentShownCollectionData.CollectionFolderPath.Length == 0)
                {
                    MessageBox.Show("編集するデータを選択し、詳細表示してください。", "CREC");
                    return;
                }
                StartEditForm();
                // 詳細情報読み込み＆表示
                StreamReader streamReaderDetailData = null;
                try
                {
                    streamReaderDetailData = new StreamReader(CurrentShownCollectionData.CollectionFolderPath + "\\details.txt");
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
                    streamReaderConfidentialData = new StreamReader(CurrentShownCollectionData.CollectionFolderPath + "\\confidentialdata.txt");
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
                EditNameTextBox.Text = CurrentShownCollectionData.CollectionName;
                EditIDTextBox.TextChanged -= IDTextBox_TextChanged;// ID重複確認イベントを停止
                EditIDTextBox.Text = CurrentShownCollectionData.CollectionID;
                EditIDTextBox.TextChanged += IDTextBox_TextChanged;// ID重複確認イベントを開始
                EditMCTextBox.Text = CurrentShownCollectionData.CollectionMC;
                EditRegistrationDateTextBox.Text = CurrentShownCollectionData.CollectionRegistrationDate;
                EditCategoryTextBox.Text = CurrentShownCollectionData.CollectionCategory;
                EditTag1TextBox.Text = CurrentShownCollectionData.CollectionTag1;
                EditTag2TextBox.Text = CurrentShownCollectionData.CollectionTag2;
                EditTag3TextBox.Text = CurrentShownCollectionData.CollectionTag3;
                EditRealLocationTextBox.Text = CurrentShownCollectionData.CollectionRealLocation;
            }
            else if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\DED") && !File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED"))
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
            if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\DED") && File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED"))
            {
                ReadOnlyButton.Visible = true;
                EditButton.Visible = false;
                EditRequestButton.Visible = false;
                return;
            }
            else if (!File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\DED") && !File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED"))
            {
                ReadOnlyButton.Visible = false;
                EditButton.Visible = true;
                EditRequestButton.Visible = false;
                return;
            }
            else if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\DED") && !File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED"))
            {
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("他の端末で編集中のため、ロックされています。\n編集権限をリクエストしますか？", "CREC", System.Windows.MessageBoxButton.YesNo);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    // 編集リクエストタグを作成
                    if (!Directory.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData"))
                    {
                        Directory.CreateDirectory(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData");
                    }
                    FileOperationClass.AddBlankFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED");
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
            else if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\DED") && File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED"))
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("EditWaitingUserExists", "mainform", LanguageFile), "CREC");
                return;
            }
            else if (!File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\DED") && !File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED"))
            {
                ReadOnlyButton.Visible = false;
                EditButton.Visible = true;
                EditRequestButton.Visible = false;
                return;
            }
            else if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\DED") && !File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED"))
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
            if (!File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\FREE") && !File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\ADD"))
            {
                DialogResult result = MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("AskStartEditWithoutCheckOtherEditing", "mainform", LanguageFile), "CREC", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }
            }
            else
            {
                FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\FREE");
            }
            // 編集中タグを作成
            if (!Directory.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData"))
            {
                Directory.CreateDirectory(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData");
            }
            FileOperationClass.AddBlankFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\DED");
            CurrentShownCollectionOperationStatus = CollectionOperationStatus.Editing;// 現在の操作ステータスを編集中に設定
            // サムネ用画像変更用データが残っていた場合削除
            if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\NewThumbnail.png"))
            {
                FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\NewThumbnail.png");
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
            EditRequestButton.Visible = false;

            // 詳細データおよび機密データを編集可能に変更
            DetailsTextBox.ReadOnly = false;
            ConfidentialDataTextBox.ReadOnly = false;
            // 一応ID重複確認イベントを開始
            EditIDTextBox.TextChanged += IDTextBox_TextChanged;
        }
        /// <summary>
        /// サムネイル選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectThumbnailButton_Click(object sender, EventArgs e)
        {
            if (CollectionDataClass.MakeTumbnailPicture(CurrentShownCollectionData.CollectionFolderPath, LanguageFile))// サムネイル作成メソッドを呼び出し
            {
                // 成功した場合
                Thumbnail.ImageLocation = (CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\NewThumbnail.png");// 新たなサムネイルを表示
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
            // 編集したデータを表示するための処理
            ContentsDataTable.Rows.Clear();// 表示内容をクリア
            CollectionDataValuesClass thisCollectionDataValues = new CollectionDataValuesClass();
            if (DataLoadingStatus == "stop")
            {
                DataLoadingStatus = "false";
            }
            // index読み込み
            CollectionDataClass.LoadCollectionIndexData(CurrentShownCollectionData.CollectionFolderPath, ref thisCollectionDataValues, LanguageFile);
            // 在庫状況読み込み
            CollectionDataClass.LoadCollectionInventoryData(CurrentShownCollectionData.CollectionFolderPath, ref thisCollectionDataValues, LanguageFile);
            ContentsDataTable.Rows.Add(
                CurrentShownCollectionData.CollectionFolderPath,
                thisCollectionDataValues.CollectionID,
                thisCollectionDataValues.CollectionMC,
                thisCollectionDataValues.CollectionName,
                thisCollectionDataValues.CollectionRegistrationDate,
                thisCollectionDataValues.CollectionCategory,
                thisCollectionDataValues.CollectionTag1,
                thisCollectionDataValues.CollectionTag2,
                thisCollectionDataValues.CollectionTag3,
                thisCollectionDataValues.CollectionCurrentInventory,
                CollectionDataClass.InventoryStatusToString(thisCollectionDataValues.CollectionInventoryStatus, LanguageFile));
            dataGridView1.DataSource = ContentsDataTable;// ここでバインド
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);// セル幅を調整
            CheckContentsListCancellationTokenSource.Cancel();
            CheckContentsListCancellationTokenSource = new CancellationTokenSource();
            SearchOptionComboBox.SelectedIndexChanged -= SearchOptionComboBox_SelectedIndexChanged;
            SearchOptionComboBox.SelectedIndex = 0;
            SearchOptionComboBox.SelectedIndexChanged += SearchOptionComboBox_SelectedIndexChanged;
            SearchMethodComboBox.SelectedIndexChanged -= SearchMethodComboBox_SelectedIndexChanged;
            SearchMethodComboBox.SelectedIndexChanged += SearchMethodComboBox_SelectedIndexChanged;
            SearchFormTextBox.TextChanged -= SearchFormTextBox_TextChanged;
            SearchFormTextBox.Text = thisCollectionDataValues.CollectionID;
            SearchFormTextBox.TextChanged += SearchFormTextBox_TextChanged;
            // 入力フォームをリセット
            ClearDetailsWindowMethod();
            CheckContentsList(CheckContentsListCancellationTokenSource.Token);// 表示内容整合性確認処理を再開
            ShowDetails();
        }
        private void DeleteContent()// データ完全削除用のメソッド
        {
            if (CurrentShownCollectionData.CollectionFolderPath.Length == 0)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                Directory.Delete(CurrentShownCollectionData.CollectionFolderPath, true);
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
                if (CurrentShownCollectionData.CollectionFolderPath != CurrentProjectSettingValues.ProjectDataFolderPath + "\\" + EditIDTextBox.Text)
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
            System.Windows.MessageBoxResult result =
                System.Windows.MessageBox.Show(
                    "UUIDの変更は、不具合の原因となる可能性があります。\n変更作業を中止しますか？",
                    "CREC",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning);
            if (result == System.Windows.MessageBoxResult.Yes)
            {
                return;
            }
            ConfigValues.AllowEditID = true;
            AllowEditIDButton.Visible = false;
            UUIDEditStatusLabel.Visible = true;
            UUIDEditStatusLabel.Text = LanguageSettingClass.GetOtherMessage("UUIDNoChange", "mainform", LanguageFile);
            UUIDEditStatusLabel.ForeColor = Color.Black;
            EditIDTextBox.ReadOnly = false;
        }
        private void IDTextBox_TextChanged(object sender, EventArgs e)// ID重複確認
        {
            if (CurrentShownCollectionData.CollectionID == EditIDTextBox.Text)
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
            CurrentShownCollectionData.CollectionID = Convert.ToString(Guid.NewGuid());
            EditIDTextBox.Text = CurrentShownCollectionData.CollectionID;// UUIDを入力
            DateTime DT = DateTime.Now;
            if (CurrentProjectSettingValues.ManagementCodeAutoFill == true)
            {
                EditMCTextBox.Text = DT.ToString("yyMMddHHmmssf");// MCを自動入力
            }
            EditRegistrationDateTextBox.Text = DT.ToString("yyyy/MM/dd_HH:mm:ss.f");// 日時を自動入力
            CurrentShownCollectionData.CollectionFolderPath = CurrentProjectSettingValues.ProjectDataFolderPath + "\\" + EditIDTextBox.Text;
            // フォルダ及びファイルを作成
            Directory.CreateDirectory(CurrentShownCollectionData.CollectionFolderPath);
            Directory.CreateDirectory(CurrentShownCollectionData.CollectionFolderPath + "\\data");
            Directory.CreateDirectory(CurrentShownCollectionData.CollectionFolderPath + "\\pictures");
            Directory.CreateDirectory(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData");
            FileOperationClass.AddBlankFile(CurrentShownCollectionData.CollectionFolderPath + "\\index.txt");
            StreamWriter streamWriter = new StreamWriter(CurrentShownCollectionData.CollectionFolderPath + "\\index.txt");
            streamWriter.WriteLine(string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7},\n{8}", "名称," + EditNameTextBox.Text, "ID," + EditIDTextBox.Text, "MC," + EditMCTextBox.Text, "登録日," + EditRegistrationDateTextBox.Text, "カテゴリ," + EditCategoryTextBox.Text, "タグ1," + EditTag1TextBox.Text, "タグ2," + EditTag2TextBox.Text, "タグ3," + EditTag3TextBox.Text, "場所1(Real)," + EditRealLocationTextBox.Text));
            streamWriter.Close();
            FileOperationClass.AddBlankFile(CurrentShownCollectionData.CollectionFolderPath + "\\details.txt");
            FileOperationClass.AddBlankFile(CurrentShownCollectionData.CollectionFolderPath + "\\confidentialdata.txt");
            // 在庫管理を行うか確認
            DialogResult result = MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("AskMakeInventoryManagementFile", "mainform", LanguageFile), "CREC", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                FileOperationClass.AddBlankFile(CurrentShownCollectionData.CollectionFolderPath + "\\inventory.inv");
                StreamWriter InventoryManagementFile = new StreamWriter(CurrentShownCollectionData.CollectionFolderPath + "\\inventory.inv");
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
            FileOperationClass.AddBlankFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\ADD");
            // 表示中の内容をクリア
            DetailsTextBox.Text = string.Empty;
            ConfidentialDataTextBox.Text = string.Empty;
            Thumbnail.Image = null;
            NoImageLabel.Visible = true;
            Thumbnail.BackColor = menuStrip1.BackColor;
            StartEditForm();
            if (FullDisplayModeToolStripMenuItem.Checked)// 全画面表示モードの時の処理
            {
                CollectionListIsShowing(false);
                ShowListButton.Visible = true;
                dataGridView1BackgroundPictureBox.Visible = false;
                ShowPicturesMethod();
            }
        }
        private bool SaveContentsMethod()// 入力されたデータを保存する処理のメソッド
        {
            CollectionEditStatusWatcherStop();// 既存の監視を停止
            // 表示内容整合性確認処理を停止し、キャンセルトークンをリセット
            CheckContentsListCancellationTokenSource.Cancel();
            CheckContentsListCancellationTokenSource = new CancellationTokenSource();
            // ID変更処理が必要な場合
            if (CurrentShownCollectionData.CollectionFolderPath != CurrentProjectSettingValues.ProjectDataFolderPath + "\\" + EditIDTextBox.Text)
            {
                if (FileOperationClass.MoveFolder(
                    CurrentShownCollectionData.CollectionFolderPath,
                    CurrentProjectSettingValues.ProjectDataFolderPath + "\\" + EditIDTextBox.Text,
                    false, false, false, true))
                {
                    CurrentShownCollectionData.CollectionFolderPath = CurrentProjectSettingValues.ProjectDataFolderPath + "\\" + EditIDTextBox.Text;// 移動先のパスで保存処理を続行
                    CurrentShownCollectionData.CollectionID = EditIDTextBox.Text;
                }
                else// 移動に失敗した場合
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("UUIDChangeError", "mainform", LanguageFile), "CREC");
                    CollectionEditStatusWatcherStart(ref CurrentShownCollectionData);// 編集監視スレッドの再開
                    return false;
                }
            }
            // フォルダを作成
            Directory.CreateDirectory(CurrentShownCollectionData.CollectionFolderPath);
            Directory.CreateDirectory(CurrentShownCollectionData.CollectionFolderPath + "\\data");
            Directory.CreateDirectory(CurrentShownCollectionData.CollectionFolderPath + "\\pictures");
            // 変更前のデータをバックアップ
            try
            {
                File.Copy(CurrentShownCollectionData.CollectionFolderPath + "\\details.txt", CurrentShownCollectionData.CollectionFolderPath + "\\details_old.txt", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("詳細データのバックアップ作成に失敗しました。\n" + ex.Message, "CREC");
            }
            try
            {
                File.Copy(CurrentShownCollectionData.CollectionFolderPath + "\\confidentialdata.txt", CurrentShownCollectionData.CollectionFolderPath + "\\confidentialdata_old.txt", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("機密データのバックアップ作成に失敗しました。\n" + ex.Message, "CREC");
            }
            // Indexデータをバックアップ
            CollectionDataClass.BackupCollectionIndexData(CurrentShownCollectionData.CollectionFolderPath, CurrentShownCollectionData, LanguageFile);
            // Indexデータを保存
            CurrentShownCollectionData.CollectionName = EditNameTextBox.Text;
            CurrentShownCollectionData.CollectionID = EditIDTextBox.Text;
            CurrentShownCollectionData.CollectionMC = EditMCTextBox.Text;
            CurrentShownCollectionData.CollectionRegistrationDate = EditRegistrationDateTextBox.Text;
            CurrentShownCollectionData.CollectionCategory = EditCategoryTextBox.Text;
            CurrentShownCollectionData.CollectionTag1 = EditTag1TextBox.Text;
            CurrentShownCollectionData.CollectionTag2 = EditTag2TextBox.Text;
            CurrentShownCollectionData.CollectionTag3 = EditTag3TextBox.Text;
            CurrentShownCollectionData.CollectionRealLocation = EditRealLocationTextBox.Text;
            if (!CollectionDataClass.SaveCollectionIndexData(CurrentShownCollectionData.CollectionFolderPath, CurrentShownCollectionData, LanguageFile))
            {
                return false;
            }

            // 詳細データの保存
            StreamWriter Detailsfile = new StreamWriter(CurrentShownCollectionData.CollectionFolderPath + "\\details.txt", false, Encoding.GetEncoding("UTF-8"));
            Detailsfile.Write(string.Format("{0}", DetailsTextBox.Text));
            Detailsfile.Close();
            // 機密データの保存
            StreamWriter ConfidentialDataFile = new StreamWriter(CurrentShownCollectionData.CollectionFolderPath + "\\confidentialdata.txt", false, Encoding.GetEncoding("UTF-8"));
            ConfidentialDataFile.Write(string.Format("{0}", ConfidentialDataTextBox.Text));
            ConfidentialDataFile.Close();
            // サムネ画像が更新されていた場合は上書きしキャッシュを削除
            if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\NewThumbnail.png"))
            {
                File.Copy(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\NewThumbnail.png", CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\Thumbnail.png", true);
                FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\NewThumbnail.png");
            }
            if (PictureBox1.Visible == true)// 詳細画像表示されている場合は読み込み
            {
                ShowPicturesMethod();
            }
            // 編集中タグを削除・解放をマーク
            if (!Directory.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData"))
            {
                Directory.CreateDirectory(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData");
            }
            FileOperationClass.AddBlankFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\FREE");
            FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\DED");
            FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED");
            FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\ADD");

            // リスト出力
            if (CurrentProjectSettingValues.EditListOutput == true)
            {
                CollectionListClass.OutputCollectionList(CurrentProjectSettingValues, allCollectionList, LanguageFile);// 一覧を出力
            }
            // バックアップ
            if (CurrentProjectSettingValues.EditBackUp == true)
            {
                // バックアップメソッドを呼び出し
                CollectionDataClass.BackupCollection(
                    CurrentProjectSettingValues,// プロジェクト設定値
                    CurrentShownCollectionData, // 現在表示中のコレクションデータ
                    LanguageFile// 言語ファイル
                    );
            }
            CollectionEditStatusWatcherStart(ref CurrentShownCollectionData);// 編集監視スレッドの再開
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
        // 在庫管理モードの表示状態を変更する処理
        public void InventoryManagementModeIsShowing(bool status)// 在庫管理モード関係の表示・非表示設定処理
        {
            // 在庫管理モードで表示する項目
            AddInventoryOperationButton.Visible = status;
            AddQuantityButton.Visible = status;
            CloseInventoryManagementModeButton.Visible = status;
            EditInventoryOperationNoteTextBox.Visible = status;
            EditQuantityTextBox.Visible = status;
            InputQuantitiyLabel.Visible = status;
            InventoryLabel.Visible = status;
            InventoryModeDataGridView.Visible = status;
            InventoryOperationLabel.Visible = status;
            InventoryOperationNoteLabel.Visible = status;
            OperationOptionComboBox.Visible = status;
            ProperInventorySettingsComboBox.Visible = status;
            ProperInventorySettingsTextBox.Visible = status;
            ProperInventorySettingsNotificationLabel.Visible = status;
            SetProperInventorySettingsButton.Visible = status;
            SubtractQuantityButton.Visible = status;

            // 在庫管理モードで非表示にする項目
            InventoryManagementModeButton.Visible = !status;

            // 在庫管理モードで使用するがデフォルトで非表示の項目
            SaveProperInventorySettingsButton.Visible = false;
        }
        private void InventoryManagementModeButton_Click(object sender, EventArgs e)// 在庫管理モード開始
        {
            string reader;
            string row;
            string[] cols;
            if (CurrentProjectSettingValues.ProjectSettingFilePath.Length == 0)// プロジェクトが開かれていない場合のエラー
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("NoProjectOpendError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (CurrentShownCollectionData.CollectionFolderPath.Length == 0)
            {
                MessageBox.Show("表示するデータを選択し、詳細表示してください。", "CREC");
                return;
            }
            //invからデータを読み込んで表示
            try
            {
                reader = File.ReadAllText(CurrentShownCollectionData.CollectionFolderPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
                rowsIM = reader.Trim().Replace("\r", string.Empty).Split('\n');
            }
            catch (Exception ex)
            {
                MessageBox.Show("在庫管理データの読み込みに失敗しました。\n" + ex.Message, "CREC");
                return;
            }

            // 画面設定
            InventoryManagementModeIsShowing(true);
            // 不要なものを非表示に
            ClosePicturesViewMethod();// 画像表示モードを閉じるメソッドを呼び出し
            CollectionListIsShowing(false);
            // DataGridView関係
            InventoryModeDataGridView.Rows.Clear();
            InventoryModeDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            InventoryModeDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            // 行数を確認
            string[] tmp = File.ReadAllLines(CurrentShownCollectionData.CollectionFolderPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
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
                inventory += Convert.ToInt32(cols[2]);
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
            if (StandardDisplayModeToolStripMenuItem.Checked)
            {
                CollectionListIsShowing(true);
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
            string[] tmp = File.ReadAllLines(CurrentShownCollectionData.CollectionFolderPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
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
            StreamWriter sw = new StreamWriter(CurrentShownCollectionData.CollectionFolderPath + "\\inventory.inv", false, Encoding.GetEncoding("UTF-8"));
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
            reader = File.ReadAllText(CurrentShownCollectionData.CollectionFolderPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
            rowsIM = reader.Trim().Replace("\r", string.Empty).Split('\n');
            // 行数を確認
            tmp = File.ReadAllLines(CurrentShownCollectionData.CollectionFolderPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
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
                inventory += Convert.ToInt32(cols[2]);
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
            string[] tmp = File.ReadAllLines(CurrentShownCollectionData.CollectionFolderPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
            StreamWriter streamWriter;
            streamWriter = new StreamWriter(CurrentShownCollectionData.CollectionFolderPath + "\\inventory.inv", false, Encoding.GetEncoding("UTF-8"));
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
        private void AddSubtractQuantity(bool add)
        {
            // 現在の在庫操作数を取得
            long currentQuantity = 0;
            if (EditQuantityTextBox.Text != string.Empty)// TextBoxに値が入っている場合は取得
            {
                try
                {
                    currentQuantity = Convert.ToInt64(EditQuantityTextBox.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "CREC");
                }
            }
            // 加算・減算処理
            try
            {
                currentQuantity += (int)Math.Pow(-1, 1 + Convert.ToInt32(add));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CREC");
            }
            // TextBoxに入れる
            EditQuantityTextBox.Text = Convert.ToString(currentQuantity);
        }
        private void CloseInventoryViewMethod()// 在庫表示モードを閉じるメソッド
        {
            InventoryManagementModeIsShowing(false);
            if (!File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\inventory.inv"))
            {
                InventoryManagementModeButton.Visible = false;
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
                SearchMethodComboBox.Items.Add(CollectionDataClass.InventoryStatusToString(InventoryStatus.StockOut, LanguageFile));
                SearchMethodComboBox.Items.Add(CollectionDataClass.InventoryStatusToString(InventoryStatus.UnderStocked, LanguageFile));
                SearchMethodComboBox.Items.Add(CollectionDataClass.InventoryStatusToString(InventoryStatus.Appropriate, LanguageFile));
                SearchMethodComboBox.Items.Add(CollectionDataClass.InventoryStatusToString(InventoryStatus.OverStocked, LanguageFile));
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
        #endregion

        #region Config読み込み
        /// <summary>
        /// config.sysを読み込み、表示内容を更新
        /// </summary>
        private void ImportConfig()
        {
            ConfigClass.LoadConfigValues(ref ConfigValues, ref CurrentProjectSettingValues);// config.sysを読み込み、ConfigValuesに格納
            // 開始済みのイベントを停止
            SearchOptionComboBox.SelectedIndexChanged -= SearchOptionComboBox_SelectedIndexChanged;
            SearchMethodComboBox.SelectedIndexChanged -= SearchMethodComboBox_SelectedIndexChanged;
            SearchFormTextBox.TextChanged -= SearchFormTextBox_TextChanged;

            if (ConfigValues.AutoSearch == true)// 自動検索が有効な場合は検索窓の入力内容が変更された時に自動で検索を行う
            {
                SearchButton.Visible = false;
                SearchFormTextBox.TextChanged += SearchFormTextBox_TextChanged;
                SearchOptionComboBox.SelectedIndexChanged += SearchOptionComboBox_SelectedIndexChanged;
                SearchMethodComboBox.SelectedIndexChanged += SearchMethodComboBox_SelectedIndexChanged;
            }
            else// 自動検索が無効な場合は検索ボタンを表示
            {
                SearchButton.Visible = true;
                SearchButton.BringToFront();// SearchButtonを最前面に移動
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
            string fontName = "Meiryo UI";// フォント指定 
            float dpiScale = (float)CurrentDPI;// DPI取得
            Size formSize = Size;// フォームサイズを取得
            // フォームの最小サイズを変更
            this.MinimumSize = new Size(Convert.ToInt32(1280 * dpiScale), Convert.ToInt32(640 * dpiScale));
            if (StandardDisplayModeToolStripMenuItem.Checked)// 通常表示モードの時は非表示
            {
                dataGridView1.Width = Convert.ToInt32(formSize.Width * 0.5 - 20 * dpiScale);
                dataGridView1BackgroundPictureBox.Width = 0;
                dataGridView1BackgroundPictureBox.Height = 0;
                dataGridView1BackgroundPictureBox.Location = new System.Drawing.Point(0, Convert.ToInt32(35 * dpiScale));
                ShowSelectedItemInformationButton.Location = new Point(Convert.ToInt32(ListUpdateButton.Location.X + ListUpdateButton.Width + 30 * dpiScale), ListUpdateButton.Location.Y);
                ShowSelectedItemInformationButton.Font = new Font(fontName, smallfontsize);
                ShowSelectedItemInformationButton.Height = ListUpdateButton.Height;
            }
            else if (FullDisplayModeToolStripMenuItem.Checked)
            {
                dataGridView1.Width = Convert.ToInt32(formSize.Width - 40 * dpiScale);
                dataGridView1BackgroundPictureBox.Width = Convert.ToInt32(formSize.Width);
                dataGridView1BackgroundPictureBox.Height = Convert.ToInt32(formSize.Height - 35 * dpiScale);
                dataGridView1BackgroundPictureBox.Location = new System.Drawing.Point(0, Convert.ToInt32(35 * dpiScale));
                ShowSelectedItemInformationButton.Location = new Point(Convert.ToInt32(ListUpdateButton.Location.X + ListUpdateButton.Width + 30 * dpiScale), ListUpdateButton.Location.Y);
                ShowSelectedItemInformationButton.Font = new Font(fontName, smallfontsize);
                ShowSelectedItemInformationButton.Height = ListUpdateButton.Height;
            }
            // 編集TextBox関係
            if (formSize.Width < 1600 * dpiScale)
            {
                Thumbnail.Width = Convert.ToInt32(formSize.Width * 0.4 - 312 * dpiScale);
                Thumbnail.Location = new Point(Convert.ToInt32(formSize.Width - Thumbnail.Width - 32 * dpiScale), Thumbnail.Location.Y);
                EditNameTextBox.Width = Convert.ToInt32(260 * dpiScale);
                EditIDTextBox.Width = Convert.ToInt32(260 * dpiScale);
                EditMCTextBox.Width = Convert.ToInt32(260 * dpiScale);
                EditRegistrationDateTextBox.Width = Convert.ToInt32(260 * dpiScale);
                EditCategoryTextBox.Width = Convert.ToInt32(260 * dpiScale);
                EditTag1TextBox.Width = Convert.ToInt32(240 * dpiScale);
                EditTag2TextBox.Width = Convert.ToInt32(240 * dpiScale);
                EditTag3TextBox.Width = Convert.ToInt32(240 * dpiScale);
                EditRealLocationTextBox.Width = Convert.ToInt32(190 * dpiScale);
            }
            else
            {
                Thumbnail.Width = Convert.ToInt32(328 * dpiScale);
                Thumbnail.Location = new Point(Convert.ToInt32(formSize.Width - 360 * dpiScale), Thumbnail.Location.Y);
                Thumbnail.Anchor = (AnchorStyles.Top | AnchorStyles.Right);
                EditNameTextBox.Width = Convert.ToInt32(formSize.Width - 1080 * dpiScale) / 2;
                EditIDTextBox.Width = Convert.ToInt32(formSize.Width - 1080 * dpiScale) / 2;
                EditMCTextBox.Width = Convert.ToInt32(formSize.Width - 1080 * dpiScale) / 2;
                EditRegistrationDateTextBox.Width = Convert.ToInt32(formSize.Width - 1080 * dpiScale) / 2;
                EditCategoryTextBox.Width = Convert.ToInt32(formSize.Width - 1080 * dpiScale) / 2;
                EditTag1TextBox.Width = Convert.ToInt32(formSize.Width - 1120 * dpiScale) / 2;
                EditTag2TextBox.Width = Convert.ToInt32(formSize.Width - 1120 * dpiScale) / 2;
                EditTag3TextBox.Width = Convert.ToInt32(formSize.Width - 1120 * dpiScale) / 2;
                EditRealLocationTextBox.Width = Convert.ToInt32(formSize.Width - 1220 * dpiScale) / 2;
            }
            DataLoadingLabel.Font = new Font(fontName, mainfontsize);
            DataLoadingLabel.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 - DataLoadingLabel.Width - 10 * dpiScale), DataLoadingLabel.Location.Y);
            dataGridView1.Font = new Font(fontName, mainfontsize);
            dataGridView1.Height = Convert.ToInt32(formSize.Height - 200 * dpiScale);
            dataGridView1.Location = new Point(Convert.ToInt32(10 * dpiScale), Convert.ToInt32(140 * dpiScale));
            SearchOptionComboBox.Font = new Font(fontName, smallfontsize);
            SearchOptionComboBox.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 - 365 * dpiScale), SearchOptionComboBox.Location.Y);
            PictureBox1.Width = Convert.ToInt32(formSize.Width * 0.5 - 20 * dpiScale);
            PictureBox1.Height = Convert.ToInt32(formSize.Height - 200 * dpiScale);
            PictureBox1.Location = new Point(Convert.ToInt32(10 * dpiScale), Convert.ToInt32(70 * dpiScale));
            ShowPictureFileNameLabel.Font = new Font(fontName, mainfontsize);
            ShowPictureFileNameLabel.Location = new Point(PictureBox1.Location.X, Convert.ToInt32(30 * dpiScale));
            NoPicturesLabel.Font = new Font(fontName, mainfontsize);
            NoPicturesLabel.Location = new Point(Convert.ToInt32(formSize.Width * 0.25 - 92 * dpiScale), Convert.ToInt32(formSize.Height * 0.5));
            ClosePicturesButton.Font = new Font(fontName, mainfontsize);
            ClosePicturesButton.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 - 190 * dpiScale), Convert.ToInt32(formSize.Height - 120 * dpiScale));
            SearchMethodComboBox.Font = new Font(fontName, smallfontsize);
            SearchMethodComboBox.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 - 205 * dpiScale), SearchOptionComboBox.Location.Y);
            SearchFormTextBox.Font = new Font(fontName, mainfontsize);
            SearchFormTextBox.Width = Convert.ToInt32(formSize.Width * 0.5 - 385 * dpiScale);
            SearchButton.Font = new Font(fontName, mainfontsize);
            SearchButton.Location = new Point(Convert.ToInt32(SearchFormTextBox.Location.X + SearchFormTextBox.Width - SearchFormTextBoxClearButton.Width - (SearchFormTextBox.Height - SearchFormTextBoxClearButton.Height) * 0.5 - 40 * dpiScale), Convert.ToInt32(SearchFormTextBox.Location.Y + (SearchFormTextBox.Height - SearchFormTextBoxClearButton.Height) * 0.5));
            SearchFormTextBoxClearButton.Font = new Font(fontName, mainfontsize);
            SearchFormTextBoxClearButton.Location = new Point(Convert.ToInt32(SearchFormTextBox.Location.X + SearchFormTextBox.Width - SearchFormTextBoxClearButton.Width - (SearchFormTextBox.Height - SearchFormTextBoxClearButton.Height) * 0.5), Convert.ToInt32(SearchFormTextBox.Location.Y + (SearchFormTextBox.Height - SearchFormTextBoxClearButton.Height) * 0.5));
            DetailsTextBox.Font = new Font(fontName, mainfontsize);
            DetailsTextBox.Width = Convert.ToInt32(formSize.Width * 0.5 - 50 * dpiScale);
            DetailsTextBox.Height = Convert.ToInt32(formSize.Height - 600 * dpiScale);
            DetailsTextBox.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 + 15 * dpiScale), Convert.ToInt32(500 * dpiScale));
            ConfidentialDataTextBox.Font = new Font(fontName, mainfontsize);
            ConfidentialDataTextBox.Width = Convert.ToInt32(formSize.Width * 0.5 - 50 * dpiScale);
            ConfidentialDataTextBox.Height = Convert.ToInt32(formSize.Height - 600 * dpiScale);
            ConfidentialDataTextBox.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 + 15 * dpiScale), Convert.ToInt32(500 * dpiScale));
            NextPictureButton.Font = new Font(fontName, mainfontsize);
            NextPictureButton.Location = new Point(Convert.ToInt32(formSize.Width * 0.25 - 90 * dpiScale), Convert.ToInt32(formSize.Height - 120 * dpiScale));
            PreviousPictureButton.Font = new Font(fontName, mainfontsize);
            PreviousPictureButton.Location = new Point(Convert.ToInt32(10 * dpiScale), Convert.ToInt32(formSize.Height - 120 * dpiScale));
            CenterLine.Location = new Point(Convert.ToInt32(formSize.Width * 0.5), Convert.ToInt32(54 * dpiScale));
            CenterLine.Height = Convert.ToInt32(formSize.Height - 100 * dpiScale);
            ObjectNameLabel.Font = new Font(fontName, bigfontsize);
            ObjectNameLabel.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 + 10 * dpiScale), ShowObjectName.Location.Y);
            ShowObjectName.Font = new Font(fontName, bigfontsize);
            ShowObjectName.Location = new Point(Convert.ToInt32(ObjectNameLabel.Location.X + ObjectNameLabel.Width), ShowObjectName.Location.Y);
            EditNameTextBox.Font = new Font(fontName, mainfontsize);
            EditNameTextBox.Location = new Point(Convert.ToInt32(ObjectNameLabel.Location.X + ObjectNameLabel.Width), ShowObjectName.Location.Y);
            EditNameTextBox.Width = Convert.ToInt32(Thumbnail.Location.X - EditNameTextBox.Location.X - 5 * dpiScale);
            IDLabel.Font = new Font(fontName, smallfontsize);
            IDLabel.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 + 10 * dpiScale), ShowID.Location.Y);
            ShowID.Font = new Font(fontName, smallfontsize);
            ShowID.Location = new Point(Convert.ToInt32(IDLabel.Location.X + IDLabel.Width), ShowID.Location.Y);
            EditIDTextBox.Font = new Font(fontName, smallfontsize);
            EditIDTextBox.Location = new Point(Convert.ToInt32(IDLabel.Location.X + IDLabel.Width), ShowID.Location.Y);
            EditIDTextBox.Width = Convert.ToInt32(Thumbnail.Location.X - EditIDTextBox.Location.X - 5 * dpiScale);
            MCLabel.Font = new Font(fontName, mainfontsize);
            MCLabel.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 + 10 * dpiScale), ShowMC.Location.Y);
            ShowMC.Font = new Font(fontName, mainfontsize);
            ShowMC.Location = new Point(Convert.ToInt32(MCLabel.Location.X + MCLabel.Width), ShowMC.Location.Y);
            EditMCTextBox.Font = new Font(fontName, mainfontsize);
            EditMCTextBox.Location = new Point(Convert.ToInt32(MCLabel.Location.X + MCLabel.Width), ShowMC.Location.Y);
            EditMCTextBox.Width = Convert.ToInt32(Thumbnail.Location.X - EditMCTextBox.Location.X - 5 * dpiScale);
            RegistrationDateLabel.Font = new Font(fontName, mainfontsize);
            RegistrationDateLabel.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 + 10 * dpiScale), RegistrationDateLabel.Location.Y);
            ShowRegistrationDate.Font = new Font(fontName, mainfontsize);
            ShowRegistrationDate.Location = new Point(Convert.ToInt32(RegistrationDateLabel.Location.X + RegistrationDateLabel.Width), RegistrationDateLabel.Location.Y);
            EditRegistrationDateTextBox.Font = new Font(fontName, mainfontsize);
            EditRegistrationDateTextBox.Location = new Point(Convert.ToInt32(RegistrationDateLabel.Location.X + RegistrationDateLabel.Width), RegistrationDateLabel.Location.Y);
            EditRegistrationDateTextBox.Width = Convert.ToInt32(Thumbnail.Location.X - EditRegistrationDateTextBox.Location.X - 5 * dpiScale);
            CategoryLabel.Font = new Font(fontName, mainfontsize);
            CategoryLabel.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 + 10 * dpiScale), CategoryLabel.Location.Y);
            ShowCategory.Font = new Font(fontName, mainfontsize);
            ShowCategory.Location = new Point(Convert.ToInt32(CategoryLabel.Location.X + CategoryLabel.Width), CategoryLabel.Location.Y);
            EditCategoryTextBox.Font = new Font(fontName, mainfontsize);
            EditCategoryTextBox.Location = new Point(Convert.ToInt32(CategoryLabel.Location.X + CategoryLabel.Width), CategoryLabel.Location.Y);
            EditCategoryTextBox.Width = Convert.ToInt32(Thumbnail.Location.X - EditCategoryTextBox.Location.X - 5 * dpiScale);
            Tag1NameLabel.Font = new Font(fontName, mainfontsize);
            Tag1NameLabel.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 + 20 * dpiScale), Tag1NameLabel.Location.Y);
            ShowTag1.Font = new Font(fontName, mainfontsize);
            ShowTag1.Location = new Point(Convert.ToInt32(Tag1NameLabel.Location.X + Tag1NameLabel.Width), Tag1NameLabel.Location.Y);
            EditTag1TextBox.Font = new Font(fontName, mainfontsize);
            EditTag1TextBox.Location = new Point(Convert.ToInt32(Tag1NameLabel.Location.X + +Tag1NameLabel.Width), Tag1NameLabel.Location.Y);
            EditTag1TextBox.Width = Convert.ToInt32(Thumbnail.Location.X - EditTag1TextBox.Location.X - 5 * dpiScale);
            Tag2NameLabel.Font = new Font(fontName, mainfontsize);
            Tag2NameLabel.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 + 20 * dpiScale), Tag2NameLabel.Location.Y);
            ShowTag2.Font = new Font(fontName, mainfontsize);
            ShowTag2.Location = new Point(Convert.ToInt32(Tag2NameLabel.Location.X + Tag2NameLabel.Width), Tag2NameLabel.Location.Y);
            EditTag2TextBox.Font = new Font(fontName, mainfontsize);
            EditTag2TextBox.Location = new Point(Convert.ToInt32(Tag2NameLabel.Location.X + Tag2NameLabel.Width), Tag2NameLabel.Location.Y);
            EditTag2TextBox.Width = Convert.ToInt32(Thumbnail.Location.X - EditTag2TextBox.Location.X - 5 * dpiScale);
            Tag3NameLabel.Font = new Font(fontName, mainfontsize);
            Tag3NameLabel.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 + 20 * dpiScale), Tag3NameLabel.Location.Y);
            ShowTag3.Font = new Font(fontName, mainfontsize);
            ShowTag3.Location = new Point(Convert.ToInt32(Tag3NameLabel.Location.X + Tag3NameLabel.Width), Tag3NameLabel.Location.Y);
            EditTag3TextBox.Font = new Font(fontName, mainfontsize);
            EditTag3TextBox.Location = new Point(Convert.ToInt32(Tag3NameLabel.Location.X + Tag3NameLabel.Width), Tag3NameLabel.Location.Y);
            EditTag3TextBox.Width = Convert.ToInt32(Thumbnail.Location.X - EditTag3TextBox.Location.X - 5 * dpiScale);
            RealLocationLabel.Font = new Font(fontName, mainfontsize);
            RealLocationLabel.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 + 5 * dpiScale), ShowRealLocation.Location.Y);
            ShowRealLocation.Font = new Font(fontName, mainfontsize);
            ShowRealLocation.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 + RealLocationLabel.Width), ShowRealLocation.Location.Y);
            EditRealLocationTextBox.Font = new Font(fontName, mainfontsize);
            EditRealLocationTextBox.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 + RealLocationLabel.Width), ShowRealLocation.Location.Y);
            EditRealLocationTextBox.Width = Convert.ToInt32(Thumbnail.Location.X - EditRealLocationTextBox.Location.X - 5 * dpiScale);
            ShowDataLocation.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 + 5 * dpiScale), ShowDataLocation.Location.Y);
            ShowDataLocation.Font = new Font(fontName, mainfontsize);
            OpenDataLocation.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 + ShowDataLocation.Width), ShowDataLocation.Location.Y);
            OpenDataLocation.Font = new Font(fontName, smallfontsize);
            CopyDataLocationPath.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 + ShowDataLocation.Width + OpenDataLocation.Width + 5 * dpiScale), ShowDataLocation.Location.Y);
            CopyDataLocationPath.Font = new Font(fontName, smallfontsize);
            DetailsLabel.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 + 5 * dpiScale), Convert.ToInt32(DetailsTextBox.Location.Y - DetailsLabel.Height - 5 * dpiScale));
            DetailsLabel.Font = new Font(fontName, mainfontsize);
            ConfidentialDataLabel.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 + 5 * dpiScale), Convert.ToInt32(DetailsTextBox.Location.Y - ConfidentialDataLabel.Height - 5 * dpiScale));
            ConfidentialDataLabel.Font = new Font(fontName, mainfontsize);
            EditButton.Location = new Point(Convert.ToInt32(formSize.Width - 615 * dpiScale), Convert.ToInt32(formSize.Height - 90 * dpiScale));
            EditButton.Font = new Font(fontName, mainfontsize);
            EditRequestButton.Location = new Point(Convert.ToInt32(formSize.Width - 630 * dpiScale), Convert.ToInt32(formSize.Height - 90 * dpiScale));
            EditRequestButton.Font = new Font(fontName, mainfontsize);
            ReadOnlyButton.Location = new Point(Convert.ToInt32(formSize.Width - 615 * dpiScale), Convert.ToInt32(formSize.Height - 90 * dpiScale));
            ReadOnlyButton.Font = new Font(fontName, mainfontsize);
            EditRequestingButton.Location = new Point(Convert.ToInt32(formSize.Width - 615 * dpiScale), Convert.ToInt32(formSize.Height - 90 * dpiScale));
            EditRequestingButton.Font = new Font(fontName, mainfontsize);
            SaveAndCloseEditButton.Location = new Point(Convert.ToInt32(formSize.Width - 620 * dpiScale), Convert.ToInt32(formSize.Height - 90 * dpiScale));
            SaveAndCloseEditButton.Font = new Font(fontName, mainfontsize);
            SavingLabel.Location = new Point(Convert.ToInt32(formSize.Width - 620 * dpiScale), Convert.ToInt32(formSize.Height - 90 * dpiScale));
            SavingLabel.Font = new Font(fontName, mainfontsize);
            InventoryManagementModeButton.Location = new Point(Convert.ToInt32(formSize.Width - 445 * dpiScale), Convert.ToInt32(formSize.Height - 90 * dpiScale));
            InventoryManagementModeButton.Font = new Font(fontName, mainfontsize);
            CloseInventoryManagementModeButton.Location = new Point(Convert.ToInt32(formSize.Width - 445 * dpiScale), Convert.ToInt32(formSize.Height - 90 * dpiScale));
            CloseInventoryManagementModeButton.Font = new Font(fontName, mainfontsize);
            ShowConfidentialDataButton.Location = new Point(Convert.ToInt32(formSize.Width - 230 * dpiScale), Convert.ToInt32(formSize.Height - 90 * dpiScale));
            ShowConfidentialDataButton.Font = new Font(fontName, mainfontsize);
            HideConfidentialDataButton.Location = new Point(Convert.ToInt32(formSize.Width - 230 * dpiScale), Convert.ToInt32(formSize.Height - 90 * dpiScale));
            HideConfidentialDataButton.Font = new Font(fontName, mainfontsize);
            // 在庫管理モード関係
            InventoryModeDataGridView.Width = Convert.ToInt32(formSize.Width * 0.5 - 20 * dpiScale);
            InventoryModeDataGridView.Height = Convert.ToInt32(formSize.Height - 340 * dpiScale);
            InventoryModeDataGridView.Location = new Point(Convert.ToInt32(10 * dpiScale), Convert.ToInt32(140 * dpiScale));
            InventoryModeDataGridView.Font = new Font(fontName, mainfontsize);
            InventoryOperationLabel.Location = new Point(Convert.ToInt32(30 * dpiScale), Convert.ToInt32(formSize.Height - 195 * dpiScale));
            InventoryOperationLabel.Font = new Font(fontName, mainfontsize);
            InputQuantitiyLabel.Location = new Point(Convert.ToInt32(210 * dpiScale), Convert.ToInt32(formSize.Height - 195 * dpiScale));
            InputQuantitiyLabel.Font = new Font(fontName, mainfontsize);
            OperationOptionComboBox.Location = new Point(Convert.ToInt32(30 * dpiScale), Convert.ToInt32(formSize.Height - 160 * dpiScale));
            OperationOptionComboBox.Font = new Font(fontName, mainfontsize);
            EditQuantityTextBox.Location = new Point(Convert.ToInt32(210 * dpiScale), Convert.ToInt32(formSize.Height - 160 * dpiScale));
            EditQuantityTextBox.Font = new Font(fontName, mainfontsize);
            AddQuantityButton.Location = new Point(Convert.ToInt32(380 * dpiScale), Convert.ToInt32(formSize.Height - 195 * dpiScale));
            AddQuantityButton.Font = new Font(fontName, mainfontsize);
            SubtractQuantityButton.Location = new Point(Convert.ToInt32(380 * dpiScale), Convert.ToInt32(formSize.Height - 150 * dpiScale));
            SubtractQuantityButton.Font = new Font(fontName, mainfontsize);
            AddInventoryOperationButton.Location = new Point(Convert.ToInt32(formSize.Width * 0.5 - 175 * dpiScale), Convert.ToInt32(formSize.Height - 165 * dpiScale));
            AddInventoryOperationButton.Font = new Font(fontName, mainfontsize);
            InventoryOperationNoteLabel.Location = new Point(Convert.ToInt32(30 * dpiScale), Convert.ToInt32(formSize.Height - 120 * dpiScale));
            InventoryOperationNoteLabel.Font = new Font(fontName, mainfontsize);
            EditInventoryOperationNoteTextBox.Width = Convert.ToInt32(formSize.Width * 0.5 - 40 * dpiScale);
            EditInventoryOperationNoteTextBox.Location = new Point(Convert.ToInt32(30 * dpiScale), Convert.ToInt32(formSize.Height - 90 * dpiScale));
            EditInventoryOperationNoteTextBox.Font = new Font(fontName, mainfontsize);
            ProperInventorySettingsComboBox.Font = new Font(fontName, mainfontsize);
            ProperInventorySettingsTextBox.Location = new Point(Convert.ToInt32(ProperInventorySettingsComboBox.Location.X + ProperInventorySettingsComboBox.Width + 15 * dpiScale), ProperInventorySettingsTextBox.Location.Y);
            ProperInventorySettingsTextBox.Font = new Font(fontName, mainfontsize);
            SetProperInventorySettingsButton.Location = new Point(Convert.ToInt32(ProperInventorySettingsTextBox.Location.X + ProperInventorySettingsTextBox.Width + 15 * dpiScale), SetProperInventorySettingsButton.Location.Y);
            SetProperInventorySettingsButton.Font = new Font(fontName, mainfontsize);
            SaveProperInventorySettingsButton.Location = new Point(Convert.ToInt32(ProperInventorySettingsTextBox.Location.X + ProperInventorySettingsTextBox.Width + 15 * dpiScale), SaveProperInventorySettingsButton.Location.Y);
            SaveProperInventorySettingsButton.Font = new Font(fontName, mainfontsize);
            AllowEditIDButton.Location = new Point(Convert.ToInt32(EditIDTextBox.Location.X + EditIDTextBox.Width - AllowEditIDButton.Width - 10 * dpiScale), EditIDTextBox.Location.Y + (EditIDTextBox.Height - AllowEditIDButton.Height) / 2);
            AllowEditIDButton.Font = new Font(fontName, extrasmallfontsize);
            UUIDEditStatusLabel.Location = new Point(Convert.ToInt32(EditIDTextBox.Location.X + EditIDTextBox.Width - UUIDEditStatusLabel.Width - 10 * dpiScale), EditIDTextBox.Location.Y + (EditIDTextBox.Height - UUIDEditStatusLabel.Height) / 2);
            UUIDEditStatusLabel.Font = new Font(fontName, extrasmallfontsize);
            CheckSameMCButton.Location = new Point(Convert.ToInt32(EditMCTextBox.Location.X + EditMCTextBox.Width - CheckSameMCButton.Width - 5 * dpiScale), EditMCTextBox.Location.Y + (EditMCTextBox.Height - CheckSameMCButton.Height) / 2);
            CheckSameMCButton.Font = new Font(fontName, extrasmallfontsize);
            NoImageLabel.Location = new Point(Convert.ToInt32(Thumbnail.Location.X + (Thumbnail.Width - NoImageLabel.Width) * 0.5), Convert.ToInt32(Thumbnail.Location.Y + (Thumbnail.Height - NoImageLabel.Height) * 0.5));
            ShowPicturesButton.Location = new Point(Convert.ToInt32(Thumbnail.Location.X + Thumbnail.Width * 0.5 - 85 * dpiScale), ShowPicturesButton.Location.Y);
            ShowPicturesButton.Font = new Font(fontName, mainfontsize);
            OpenPictureFolderButton.Location = new Point(Convert.ToInt32(Thumbnail.Location.X + Thumbnail.Width * 0.5 - 85 * dpiScale), OpenPictureFolderButton.Location.Y);
            OpenPictureFolderButton.Font = new Font(fontName, mainfontsize);
            SelectThumbnailButton.Location = new Point(Convert.ToInt32(Thumbnail.Location.X + Thumbnail.Width * 0.5 - 85 * dpiScale), SelectThumbnailButton.Location.Y);
            SelectThumbnailButton.Font = new Font(fontName, mainfontsize);
            ShowListButton.Width = Convert.ToInt32(130 * dpiScale);
            ShowListButton.Font = new Font(fontName, mainfontsize);
            // dataGridViewContextMenuStripの文字サイズ
            AddContentsContextStripMenuItem.Font = new Font(fontName, smallfontsize);
            CopyAndAddContentsContextToolStripMenuItem.Font = new Font(fontName, smallfontsize);
            ListUpdateContextStripMenuItem.Font = new Font(fontName, smallfontsize);
            OpenProjectContextStripMenuItem.Font = new Font(fontName, smallfontsize);
            ShowSelectedItemInformationToolStripMenuItem.Font = new Font(fontName, smallfontsize);
            OpenCollectionDataLocationToolStripMenuItem.Font = new Font(fontName, smallfontsize);
            // PictureBox1ContextMenuStripの文字サイズ
            OpenPicturewithAppToolStripMenuItem.Font = new Font(fontName, mainfontsize);
            AddContentsButton.Font = new Font(fontName, smallfontsize);
            ListUpdateButton.Font = new Font(fontName, smallfontsize);
            // ToolStorip関係
            ShowListButton.Font = new Font(fontName, smallfontsize);
            foreach (var toolStripMenuItem1 in menuStrip1.Items)
            {
                if (toolStripMenuItem1 is ToolStripMenuItem)
                {
                    ((ToolStripMenuItem)toolStripMenuItem1).Font = new Font(fontName, smallfontsize);
                    foreach (var toolStripMenuItem2 in ((ToolStripMenuItem)toolStripMenuItem1).DropDownItems)
                    {
                        if (toolStripMenuItem2 is ToolStripMenuItem)
                        {
                            ((ToolStripMenuItem)toolStripMenuItem2).Font = new Font(fontName, smallfontsize);
                            foreach (var toolStripMenuItem3 in ((ToolStripMenuItem)toolStripMenuItem2).DropDownItems)
                            {
                                if (toolStripMenuItem3 is ToolStripMenuItem)
                                {
                                    ((ToolStripMenuItem)toolStripMenuItem3).Font = new Font(fontName, smallfontsize);
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

        #region コレクションの編集状態を監視する処理
        static FileSystemWatcher collectionEditStatusWatcher = new FileSystemWatcher();
        delegate void DelegateProcess();//delegateを宣言
        CancellationTokenSource CheckContentsListCancellationTokenSource = new CancellationTokenSource();// CheckContentsListのキャンセルトークン
        /// <summary>
        /// コレクションの編集状態を監視するメソッド
        /// </summary>
        /// <param name="targetPath">監視対象のコレクションパス</param>
        private void CollectionEditStatusWatcherStart(ref CollectionDataValuesClass CollectionDataValues)
        {
            collectionEditStatusWatcher.EnableRaisingEvents = false; // 既存の監視を停止
            if (Directory.Exists(CollectionDataValues.CollectionFolderPath + "\\SystemData") == false)
            {
                return;
            }
            collectionEditStatusWatcher.Path = CollectionDataValues.CollectionFolderPath + "\\SystemData";
            collectionEditStatusWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size;
            collectionEditStatusWatcher.Filter = "*.*";
            // イベントハンドラを登録
            collectionEditStatusWatcher.Created += CollectionEditStatusOnChanged;
            collectionEditStatusWatcher.Changed += CollectionEditStatusOnChanged;
            collectionEditStatusWatcher.Deleted += CollectionEditStatusOnChanged;
            collectionEditStatusWatcher.EnableRaisingEvents = true; // 新しいフォルダの監視を開始
        }
        /// <summary>
        /// コレクションの編集状態を監視するメソッドの停止
        /// </summary>
        private void CollectionEditStatusWatcherStop()
        {
            collectionEditStatusWatcher.EnableRaisingEvents = false;// イベントの監視を停止
            // イベントハンドラを解除
            collectionEditStatusWatcher.Created -= CollectionEditStatusOnChanged;
            collectionEditStatusWatcher.Changed -= CollectionEditStatusOnChanged;
            collectionEditStatusWatcher.Deleted -= CollectionEditStatusOnChanged;
        }
        /// <summary>
        /// コレクションフォルダの変更を検知する処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollectionEditStatusOnChanged(object sender, FileSystemEventArgs e)
        {
            DelegateProcess delegateProcess = new DelegateProcess(CollectionOperationStatusManager);//delegateにChangeTextBoxを登録
            this.Invoke(delegateProcess);//DelegateProcessを実行
        }
        /// <summary>
        /// コレクションの編集状態を取得し描画を変更する処理
        /// </summary>
        private void CollectionOperationStatusManager()
        {
            switch (CurrentShownCollectionOperationStatus)
            {
                case CollectionOperationStatus.Watching:
                    // 編集中端末が存在する場合
                    if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\DED"))
                    {
                        if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED"))
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
                            EditRequestButton.Visible = true;
                            EditRequestButton.ForeColor = Color.Blue;
                            EditButton.Visible = false;
                            ReadOnlyButton.Visible = false;
                        }
                    }
                    // 編集中端末が存在しない場合
                    else
                    {
                        EditButton.Visible = ConfigValues.AllowEdit;
                        ReadOnlyButton.Visible = !ConfigValues.AllowEdit;
                        EditRequestButton.Visible = false;
                    }
                    SaveAndCloseEditButton.Visible = false;
                    break;
                case CollectionOperationStatus.Editing:
                    // 編集リクエストされた場合
                    if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED"))
                    {
                        System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("他の端末から編集権限をリクエストされました。\nリクエストを許可しますか？", "CREC", System.Windows.MessageBoxButton.YesNo);

                        // 権限を譲渡しない場合は抜ける
                        if (result == System.Windows.MessageBoxResult.No || result == System.Windows.MessageBoxResult.None)
                        {
                            FileOperationClass.DeleteFile(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED");
                            break;
                        }

                        // 保存関係の処理
                        if (CurrentShownCollectionData.CollectionFolderPath == CurrentProjectSettingValues.ProjectDataFolderPath + "\\" + EditIDTextBox.Text)
                        {
                            if (SaveContentsMethod() == false)
                            {
                                return;
                            }
                        }
                        else if (CurrentShownCollectionData.CollectionFolderPath != CurrentProjectSettingValues.ProjectDataFolderPath + "\\" + EditIDTextBox.Text)
                        {
                            result = System.Windows.MessageBox.Show("IDを変更した場合、他の端末へ編集権限を譲渡できなくなります。\nIDを変更前のものに戻して保存しますか？", "CREC", System.Windows.MessageBoxButton.YesNo);
                            if (result == System.Windows.MessageBoxResult.Yes)
                            {
                                CurrentShownCollectionData.CollectionID = Path.GetFileName(CurrentShownCollectionData.CollectionFolderPath);
                                EditIDTextBox.Text = CurrentShownCollectionData.CollectionID;
                                MessageBox.Show("IDを" + CurrentShownCollectionData.CollectionID + "に戻しました", "CREC");
                            }
                            if (SaveContentsMethod() == false)
                            {
                                return;
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
                        SaveAndCloseEditButton.Visible = true;
                        EditButton.Visible = false;
                        EditRequestButton.Visible = false;
                    }
                    break;
                case CollectionOperationStatus.EditRequesting:
                    break;
            }
        }
        #endregion

        #region 非同期処理置き場
        private async void CheckContentsList(CancellationToken cancellationToken)// ContentsListの選択と詳細表示内容の整合性をバックグラウンドで監視
        {
            while (true)
            {
                await Task.Delay(CurrentProjectSettingValues.DataCheckInterval);

                // キャンセルトークンが要求された場合はループを抜ける
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                // セル未選択時は何もしない
                if (dataGridView1.CurrentRow == null)
                {
                    continue;
                }

                // 表示中コレクションのUUID（フォルダ）が変更されたときは再読み込み
                if (Directory.Exists(CurrentShownCollectionData.CollectionFolderPath) == false
                    && CurrentShownCollectionData.CollectionFolderPath != string.Empty)
                {
                    MessageBox.Show("コレクションのUUIDが変更されました。\nリストを更新します。", "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGrid();// リストを更新
                    ShowDetails();
                    continue;
                }

                // 表示中コレクションと選択が一致する場合は続行
                if (CurrentShownCollectionData.CollectionID == Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value))
                {
                    continue;
                }

                // 差分がある場合なので、リストで選択されている内容の表示に変更する
                if (SaveAndCloseEditButton.Visible == true)// 編集中の場合は警告を表示
                {
                    if (CheckEditingContents() != true)
                    {
                        continue;
                    }
                }
                ShowDetails();
            }
        }
        private async void AwaitEdit()// 編集許可を待機
        {
            while (true)
            {
                await Task.Delay(CurrentProjectSettingValues.DataCheckInterval);
                if (Directory.Exists(CurrentShownCollectionData.CollectionFolderPath) == false)
                {
                    MessageBox.Show("IDが変更されました。\n再度読み込みを行ってください。", "CREC");
                    break;
                }
                if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\RED"))
                {
                }
                else
                {
                    if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\DED"))
                    {
                        EditRequestingButton.Visible = false;
                        EditButton.Visible = true;
                        MessageBox.Show("拒否されました", "CREC");
                        return;
                    }
                    else if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\FREE"))
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
                            streamReaderDetailData = new StreamReader(CurrentShownCollectionData.CollectionFolderPath + "\\details.txt");
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
                            streamReaderConfidentialData = new StreamReader(CurrentShownCollectionData.CollectionFolderPath + "\\confidentialdata.txt");
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
                        EditNameTextBox.Text = CurrentShownCollectionData.CollectionName;
                        EditIDTextBox.TextChanged -= IDTextBox_TextChanged;// ID重複確認イベントを停止
                        EditIDTextBox.Text = CurrentShownCollectionData.CollectionID;
                        EditIDTextBox.TextChanged += IDTextBox_TextChanged;// ID重複確認イベントを開始
                        AllowEditIDButton.Visible = true;
                        UUIDEditStatusLabel.Visible = false;
                        EditMCTextBox.Text = CurrentShownCollectionData.CollectionMC;
                        EditRegistrationDateTextBox.Text = CurrentShownCollectionData.CollectionRegistrationDate;
                        EditCategoryTextBox.Text = CurrentShownCollectionData.CollectionCategory;
                        EditTag1TextBox.Text = CurrentShownCollectionData.CollectionTag1;
                        EditTag2TextBox.Text = CurrentShownCollectionData.CollectionTag2;
                        EditTag3TextBox.Text = CurrentShownCollectionData.CollectionTag3;
                        EditRealLocationTextBox.Text = CurrentShownCollectionData.CollectionRealLocation;
                        return;
                    }
                }
            }
        }
        /// <summary>
        /// サムネイル読み込み処理
        /// </summary>
        private async void LoadThnumbnailPicture()
        {
            NoImageLabel.Text = "Loading";

            await Task.Run(() =>
            {
                Thumbnail.ImageLocation = (CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\Thumbnail.png");
            });

            if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\Thumbnail.png"))
            {
                NoImageLabel.Visible = false;
                Thumbnail.BackColor = this.BackColor;
            }
            else
            {
                NoImageLabel.Text = "NO IMAGE";
                NoImageLabel.Visible = true;
                Thumbnail.BackColor = menuStrip1.BackColor;
                // 後方互換性(v8.4.3以前)のため、従来のJPGE形式のサムネイルを確認
                if (File.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\pictures\\Thumbnail1.jpg"))
                {
                    if (!Directory.Exists(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData"))// SystemDataフォルダが存在しない場合は作成
                    {
                        Directory.CreateDirectory(CurrentShownCollectionData.CollectionFolderPath + "\\SystemData");
                    }
                    if (!FileOperationClass.MoveFile(
                        CurrentShownCollectionData.CollectionFolderPath + "\\pictures\\Thumbnail1.jpg",
                        CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\Thumbnail.png",
                        true, true))// サムネイルを移動
                    {
                        MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("ThumbnailVersionMigrationError", "mainform", LanguageFile), "CREC", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    // サムネイル再読み込み
                    Thumbnail.ImageLocation = (CurrentShownCollectionData.CollectionFolderPath + "\\SystemData\\Thumbnail.png");
                    NoImageLabel.Visible = false;
                    Thumbnail.BackColor = this.BackColor;
                }
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
            CollectionListIsShowing(false);
            ShowListButton.Visible = true;
            dataGridView1BackgroundPictureBox.Visible = false;
            ShowPicturesMethod();
        }
        /// <summary>
        /// 選択中のコレクションのデータフォルダを開く
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenCollectionDataLocationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CollectionDataClass.OpenCollectionDataFolder(CurrentShownCollectionData, LanguageFile);
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
        }
        #endregion

        #region バックアップ関連
        /// <summary>
        /// 全データバックアップ処理（非同期、画面表示あり）
        /// </summary>
        private void BackUpMethod()
        {
            // プロジェクトが開いているか確認
            if (CurrentProjectSettingValues.ProjectSettingFilePath.Length == 0)
            {
                // プロジェクトが開いていない場合はエラーメッセージを表示
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage(
                    "NoProjectOpendError",
                    "mainform",
                    LanguageFile),
                    "CREC",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            // BackupToolStripMenuItemのTextをバックアップ中のメッセージに変更
            BackupToolStripMenuItem.Text = LanguageSettingClass.GetOtherMessage(
                "BackupToolStripMenuItemBackupInProgressMessage",
                "mainform",
                LanguageFile);
            BackupToolStripMenuItem.Enabled = false;// バックアップ中は無効化
            // プロジェクトのバックアップ処理を開始
            Task<bool> task = CollectionDataClass.BackupProjectDataAsync(
                CurrentProjectSettingValues,
                LanguageFile);
            // タスクの完了を待機し、結果に応じてメッセージを表示
            task.ContinueWith(t =>
            {
                this.Invoke((Action)(() =>
                {
                    if (t.Result == true)
                    {
                        // バックアップ完了メッセージを表示
                        MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage(
                            "BackupCompleted",
                            "mainform",
                            LanguageFile),
                            "CREC",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        // バックアップ失敗メッセージを表示
                        MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage(
                            "BackupFailed",
                            "mainform",
                            LanguageFile),
                            "CREC",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                    // BackupToolStripMenuItemのTextを元に戻す
                    this.Invoke((Action)(() =>
                    {
                        BackupToolStripMenuItem.Text = LanguageSettingClass.GetToolStripMenuItemMessage(
                        "BackupToolStripMenuItem",
                        "mainform",
                        LanguageFile);
                    }));
                    BackupToolStripMenuItem.Enabled = true;// バックアップ完了後は有効化
                }));
            });
        }
        #endregion

        #region 表示モード設定
        private void StandardDisplayModeToolStripMenuItem_Click(object sender, EventArgs e)// 標準モード
        {
            StandardDisplayModeToolStripMenuItem.Checked = true;
            FullDisplayModeToolStripMenuItem.Checked = false;
            ShowSelectedItemInformationToolStripMenuItem.Visible = false;
            ShowSelectedItemInformationButton.Visible = false;
            ShowListButton.Visible = false;
            ClosePicturesButton.Visible = PictureBox1.Visible;// 画像が表示されている場合は閉じるボタンを表示
            SetFormLayout();
        }
        private void FullDisplayModeToolStripMenuItem_Click(object sender, EventArgs e)// 拡大モード変更中
        {
            StandardDisplayModeToolStripMenuItem.Checked = false;
            FullDisplayModeToolStripMenuItem.Checked = true;
            ShowSelectedItemInformationToolStripMenuItem.Visible = true;
            if (SaveAndCloseEditButton.Visible == true)// 編集中に切り替えた場合の処理
            {
                CollectionListIsShowing(false);
                ShowListButton.Visible = true;
                dataGridView1BackgroundPictureBox.Visible = false;
                if (InventoryModeDataGridView.Visible == false)// 在庫管理以外の画面状態で切り替わった場合
                {
                    ShowPicturesMethod();// 画像を表示
                }
            }
            else
            {
                if (PictureBox1.Visible == true)// 画像表示中に切り替わった場合の処理
                {
                    CollectionListIsShowing(false);
                    ShowListButton.Visible = true;
                    dataGridView1BackgroundPictureBox.Visible = false;
                    ShowPicturesMethod();
                }
                else if (InventoryModeDataGridView.Visible == true)// 在庫管理中に切り替わった場合
                {
                    CollectionListIsShowing(false);
                    ShowListButton.Visible = true;
                    dataGridView1BackgroundPictureBox.Visible = false;
                }
                else// その他
                {
                    ShowListButton.Visible = false;
                    dataGridView1BackgroundPictureBox.Visible = true;
                    ShowSelectedItemInformationButton.Visible = true;
                }
            }
            SetFormLayout();
        }
        private void ShowSelectedItemInformationButton_Click(object sender, EventArgs e)// 拡大モード時に詳細情報を表示
        {
            CollectionListIsShowing(false);
            ShowListButton.Visible = true;
            dataGridView1BackgroundPictureBox.Visible = false;
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
            CollectionListIsShowing(true);
            ShowListButton.Visible = false;
            dataGridView1BackgroundPictureBox.Visible = true;
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

        #region 最近表示した項目関連
        /// <summary>
        /// 最近表示した項目に追加する
        /// </summary>
        private void AddRecentShownCollectiontoToolStripMenuItem()
        {
            try
            {
                ToolStripMenuItem[] OriginalRecentShownCollectionsToolStripMenuItems = RecentShownCollectionsToolStripMenuItems;// 既存の最近表示した項目を保存
                RecentShownCollectionsToolStripMenuItems = new ToolStripMenuItem[11];// 一旦初期化
                // 最近表示した項目を追加
                RecentShownCollectionsToolStripMenuItems[0] = new ToolStripMenuItem();
                RecentShownCollectionsToolStripMenuItems[0].Text = CurrentShownCollectionData.CollectionName;
                RecentShownCollectionsToolStripMenuItems[0].ToolTipText = CurrentShownCollectionData.CollectionFolderPath;
                RecentShownCollectionsToolStripMenuItems[0].Click += new EventHandler(RecentShownCollectionsToolStripMenuItems_Click);
                // 既存の最近表示した項目を追加
                if (OriginalRecentShownCollectionsToolStripMenuItems != null)
                {
                    int count = 0;
                    for (int i = 0; i < RecentShownCollectionsToolStripMenuItems.Length - 1; i++)
                    {
                        if (OriginalRecentShownCollectionsToolStripMenuItems[i] == null)
                        {
                            break;
                        }
                        else if (OriginalRecentShownCollectionsToolStripMenuItems[i].ToolTipText == RecentShownCollectionsToolStripMenuItems[0].ToolTipText)// 重複する場合は追加しない
                        {
                            continue;
                        }
                        else
                        {
                            count++;
                            RecentShownCollectionsToolStripMenuItems[count] = new ToolStripMenuItem();
                            RecentShownCollectionsToolStripMenuItems[count] = OriginalRecentShownCollectionsToolStripMenuItems[i];
                            RecentShownCollectionsToolStripMenuItems[count].Click += new EventHandler(RecentShownCollectionsToolStripMenuItems_Click);
                        }
                    }
                }
                // ToolStripMenuItemに追加
                RecentShownContentsToolStripMenuItem.DropDownItems.Clear();// 表示済みのMenuItemを削除
                for (int i = 0; i < RecentShownCollectionsToolStripMenuItems.Length - 1; i++)
                {
                    if (RecentShownCollectionsToolStripMenuItems[i] == null)
                    {
                        break;
                    }
                    else
                    {
                        RecentShownContentsToolStripMenuItem.DropDownItems.Add(RecentShownCollectionsToolStripMenuItems[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("RecentlyOpenedCollectionGetError", "mainform", LanguageFile) + ex.Message, "CREC");
            }
        }
        /// <summary>
        /// 最近表示した項目がクリックされた時のイベント
        /// </summary>
        private void RecentShownCollectionsToolStripMenuItems_Click(object sender, EventArgs e)
        {
            if (SaveAndCloseEditButton.Visible == true)// 編集中の場合は警告を表示
            {
                if (CheckEditingContents() != true)// 編集終了をキャンセルされた場合
                {
                    return;// 何もせず終了
                }
            }
            if (InventoryModeDataGridView.Visible == true)// 在庫管理モードの場合は閉じる
            {
                CloseInventoryViewMethod();
            }
            if (PictureBox1.Visible == true)// 画像表示モードの場合は閉じる
            {
                ClosePicturesViewMethod();
            }
            if (CurrentShownCollectionData.CollectionFolderPath != ((ToolStripItem)sender).ToolTipText)// 表示中の項目と選択された項目が異なる場合
            {
                try
                {
                    if (!Directory.Exists(((ToolStripItem)sender).ToolTipText))// 選択されたコレクションが存在しない場合
                    {
                        // 存在しないコレクションをToolStripMenuItemから削除
                        foreach (var item in RecentShownCollectionsToolStripMenuItems)
                        {
                            if (item != null)
                            {
                                if (item.ToolTipText == ((ToolStripItem)sender).ToolTipText)
                                {
                                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("CantFindRecentlyOpenedCollection", "mainform", LanguageFile) + ((ToolStripItem)sender).Text, "CREC");
                                    RecentShownContentsToolStripMenuItem.DropDownItems.Remove((ToolStripItem)sender);
                                }
                            }
                        }
                        ToolStripMenuItem[] originalRecentShownCollectionsToolStripMenuItems = RecentShownCollectionsToolStripMenuItems;// 既存の最近表示した項目を保存
                        RecentShownCollectionsToolStripMenuItems = new ToolStripMenuItem[11];// 一旦初期化
                        int count = 0;
                        for (int i = 0; i < RecentShownCollectionsToolStripMenuItems.Length; i++)
                        {
                            if (originalRecentShownCollectionsToolStripMenuItems[i] == null)
                            {
                                break;
                            }
                            else if (originalRecentShownCollectionsToolStripMenuItems[i].ToolTipText != ((ToolStripItem)sender).ToolTipText)// 削除対象以外のコレクションを追加
                            {
                                RecentShownCollectionsToolStripMenuItems[count] = originalRecentShownCollectionsToolStripMenuItems[i];
                                count++;
                            }
                        }
                        // ToolStripMenuItemに追加
                        RecentShownContentsToolStripMenuItem.DropDownItems.Clear();// 表示済みのMenuItemを削除
                        for (int i = 0; i < RecentShownCollectionsToolStripMenuItems.Length - 1; i++)
                        {
                            if (RecentShownCollectionsToolStripMenuItems[i] == null)
                            {
                                break;
                            }
                            else
                            {
                                RecentShownContentsToolStripMenuItem.DropDownItems.Add(RecentShownCollectionsToolStripMenuItems[i]);
                            }
                        }
                    }
                    else
                    {
                        CurrentShownCollectionData.CollectionFolderPath = ((ToolStripItem)sender).ToolTipText;
                        CurrentShownCollectionData.CollectionID = Path.GetFileName(((ToolStripItem)sender).ToolTipText);
                        SearchFormTextBox.TextChanged -= SearchFormTextBox_TextChanged;
                        SearchOptionComboBox.SelectedIndexChanged -= SearchOptionComboBox_SelectedIndexChanged;
                        SearchFormTextBox.Text = CurrentShownCollectionData.CollectionID;
                        SearchOptionComboBox.SelectedIndex = 0;
                        SearchFormTextBox.TextChanged += SearchFormTextBox_TextChanged;
                        SearchOptionComboBox.SelectedIndexChanged += SearchOptionComboBox_SelectedIndexChanged;
                        LoadGrid();
                        ShowDetails();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("RecentlyOpenedCollectionSetError", "mainform", LanguageFile) + ex.Message, "CREC");
                }
            }
        }
        #endregion

        #region 言語設定
        private void LanguageSettingToolStripMenuItem_MouseEnter(object sender, EventArgs e)// 表示言語リストを表示
        {
            LanguageSettingToolStripMenuItem.DropDownItems.Clear();
            int count = -1;
            try
            {
                System.IO.DirectoryInfo directoryInfo = new DirectoryInfo(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\language");
                if (!directoryInfo.Exists)
                {
                    MessageBox.Show("言語フォルダが見つかりません。デフォルトの言語ファイルを作成します。\nNo Language Folder.", "CREC");
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\language");
                    if (!LanguageSettingClass.MakeDefaultLanguageFileJP())
                    {
                        MessageBox.Show("致命的なエラーが発生しました。アプリケーションを終了します。", "CREC");
                        Environment.FailFast("Default language file can't find.");
                    }
                    ConfigValues.LanguageFileName = "Japanese";
                    SetLanguage(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\language\\" + ConfigValues.LanguageFileName + ".xml");
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
                        IEnumerable<string> strings = from item in XElement.Load(fileInfo.FullName).Elements("metadata").Elements("name") select item.Value;
                        foreach (string s in strings)
                        {
                            LanguageSettingToolStripMenuItems[count].Text = s;
                            LanguageSettingToolStripMenuItems[count].ToolTipText = fileInfo.FullName;
                            LanguageSettingToolStripMenuItem.DropDownItems.Add(LanguageSettingToolStripMenuItems[count]);
                            LanguageSettingToolStripMenuItems[count].Click += new EventHandler(LanguageSettingToolStripMenuItems_Click);
                            if (ConfigValues.LanguageFileName == Path.GetFileNameWithoutExtension(fileInfo.FullName))
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
                    ConfigValues.LanguageFileName = "Japanese";
                    SetLanguage(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\language\\" + ConfigValues.LanguageFileName + ".xml");
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
            int? selectedRow = null;
            if (dataGridView1.RowCount > 0)// リストにコンテンツが表示されているか確認
            {
                selectedRow = dataGridView1.SelectedRows[0].Index;// 表示中のコンテンツのIDを取得
            }
            ConfigValues.LanguageFileName = Path.GetFileNameWithoutExtension(((ToolStripItem)sender).ToolTipText);
            SetLanguage(((ToolStripItem)sender).ToolTipText);
            int targetColumn = 1;
            if (IDListVisibleToolStripMenuItem.Checked) { targetColumn = 1; }
            else if (MCListVisibleToolStripMenuItem.Checked) { targetColumn = 2; }
            else if (NameListVisibleToolStripMenuItem.Checked) { targetColumn = 3; }
            else if (RegistrationDateListVisibleToolStripMenuItem.Checked) { targetColumn = 4; }
            else if (CategoryListVisibleToolStripMenuItem.Checked) { targetColumn = 5; }
            else if (Tag1ListVisibleToolStripMenuItem.Checked) { targetColumn = 6; }
            else if (Tag2ListVisibleToolStripMenuItem.Checked) { targetColumn = 7; }
            else if (Tag3ListVisibleToolStripMenuItem.Checked) { targetColumn = 8; }
            else if (InventoryInformationListToolStripMenuItem.Checked) { targetColumn = 9; }
            if (selectedRow != null)
            {
                dataGridView1.CurrentCell = dataGridView1[targetColumn, (int)selectedRow];// 言語設定前に表示されている項目を復元
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
                SearchMethodComboBox.Items.Add(CollectionDataClass.InventoryStatusToString(InventoryStatus.StockOut, LanguageFile));
                SearchMethodComboBox.Items.Add(CollectionDataClass.InventoryStatusToString(InventoryStatus.UnderStocked, LanguageFile));
                SearchMethodComboBox.Items.Add(CollectionDataClass.InventoryStatusToString(InventoryStatus.Appropriate, LanguageFile));
                SearchMethodComboBox.Items.Add(CollectionDataClass.InventoryStatusToString(InventoryStatus.OverStocked, LanguageFile));
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
