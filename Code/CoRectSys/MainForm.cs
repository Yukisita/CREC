/*
MainForm
Copyright (c) [2022-2024] [S.Yukisita]
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
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
using System.IO;
using File = System.IO.File;
using System.Threading;
using System.Windows.Documents;
using System.IO.Compression;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using ColRECt;
using System.Net;
using System.Net.Http;
using System.Security.Policy;

namespace CoRectSys
{
    public partial class MainForm : Form
    {
        #region 変数の宣言
        // アップデート確認用、Release前に変更忘れずに
        string LatestVersionDownloadLink = "https://github.com/Yukisita/CREC/releases/download/Latest_Release/CREC_v7.09.02.zip";
        // プロジェクトファイル読み込み用変数
        string TargetProjectName = "";// プロジェクト名
        string TargetFolderPath = "";// データ保管場所のフォルダパス
        string TargetCRECPath = "";// 管理ファイル（.crec）のファイルパス
        string TargetIndexPath = "";// Indexの場所
        string TargetDetailsPath = "";// 説明txtのパス
        string TargetBackupPath = "";// バックアップ場所のフォルダパス
        string TargetListOutputPath = "";// 一覧List出力場所のフォルダパス
        bool StartUpBackUp = false;// S、起動時にバックアップ
        bool EditedBackUp = false;// E、編集後にバックアップ
        bool CloseBackUp = false;// C、終了時にバックアップ
        int CompressType = 1;// 圧縮方法
        bool StartUpListOutput = false;// S、起動時に一覧作成
        bool EditedListOutput = false;// E、編集後に一覧作成
        bool CloseListOutput = false;// C、終了時に一覧作成
        bool OpenListAfterOutput = false;// O、一覧作成後に自動で開く
        string ListOutputFormat = "CSV";// List作成時のフォーマット、デフォルトでCSV
        string TargetCreatedDate = "";// 作成日時
        string TargetModifiedDate = "";// 更新日時
        string TargetAccessedDate = "";// 最終アクセス日時
        string ShowObjectNameLabel = "名称";// 名称ラベルの表示名
        bool ShowObjectNameLabelVisible = true;
        string ShowIDLabel = "UUID";// IDラベルの表示名
        bool ShowIDLabelVisible = true;
        string ShowMCLabel = "管理コード";// 管理コードラベルの表示名
        bool ShowMCLabelVisible = true;
        bool AutoMCFill = true;
        string ShowRegistrationDateLabel = "登録日";// 登録日ラベルの表示名
        bool ShowRegistrationDateLabelVisible = true;
        string ShowCategoryLabel = "カテゴリ";// カテゴリラベルの表示名
        bool ShowCategoryLabelVisible = true;
        string Tag1Name = "タグ１";// Tag1の名称
        bool ShowTag1NameVisible = true;
        string Tag2Name = "タグ２";// Tag2の名称
        bool ShowTag2NameVisible = true;
        string Tag3Name = "タグ３";// Tag3の名称
        bool ShowTag3NameVisible = true;
        string ShowRealLocationLabel = "現物保管場所";// 現物保管場所ラベルの表示名
        bool ShowRealLocationLabelVisible = true;
        string ShowDataLocationLabel = "データ保管場所";// データ保管場所ラベルの表示名
        bool ShowDataLocationLabelVisible = true;
        int SearchOptionNumber = 0;// 検索対象設定、デフォルトで0
        int SearchMethodNumber = 0;// 検索方法、デフォルトで0
        string AutoLoadProjectPath = "";// 自動読み込みプロジェクトのパス
        bool OpenLastTimeProject = false;// 前回開いていたプロジェクトを開く
        string[] cols;// List等読み込み用

        // config.sys読み込み用変数
        bool AllowEdit;// 編集可否を設定
        bool AllowEditID = false;// IDの手動設定の可否を設定、デフォルトで禁止
        bool ShowConfidentialData;// 機密情報表示の可否を設定
        bool ShowUserAssistToolTips;// ユーザー補助のポップアップの表示・非表示を設定
        bool AutoSearch;// 検索窓に入力された内容を自動で検索するか設定
        bool RecentShownContents;// 検索候補を表示するか設定
        bool BootUpdateCheck;// 起動時に更新確認するか設定
        string ColorSetting = "Blue";// 色設定

        // 詳細データ読み込み用変数宣言、詳細表示している内容を入れておく
        string TargetContentsPath = "";// 詳細表示するデータのフォルダパス
        string ThisName = "";
        string ThisID = "";
        string ThisMC = "";
        string ThisRegistrationDate = "";
        string ThisCategory = "";
        string ThisTag1 = "";
        string ThisTag2 = "";
        string ThisTag3 = "";
        string ThisRealLocation = "";

        // データ一覧表示関係
        DataTable ContentsDataTable = new DataTable();
        string DataLoadingStatus = "false";// Data非同期読み込みのステータス

        // 表示関係
        double CurrentDPI = 1.0;// 現在のDPI値
        double FirstDPI = 1.0;// 起動時の表示スケール値
        // フォントサイズ
        float extrasmallfontsize = (float)(9);// 最小フォントのサイズ
        float smallfontsize = (float)(14.25);// 小フォントのサイズ
        float mainfontsize = (float)(18.0);// 標準フォントのサイズ
        float bigfontsize = (float)(20.25);// 大フォントのサイズ
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
                    MessageBox.Show("" + ex, "CREC");
                }
                if (StartUpListOutput == true)
                {
                    if (ListOutputFormat == "CSV")
                    {
                        CSVListOutputMethod();
                    }
                    else if (ListOutputFormat == "TSV")
                    {
                        TSVListOutputMethod();
                    }
                    else
                    {
                        MessageBox.Show("値が不正です。", "CREC");
                    }
                }
                if (StartUpBackUp == true)// 自動バックアップ
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
                    MessageBox.Show("" + ex, "CREC");
                }
            }
            if (AutoSearch == true)
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
            SetFormLayout();// レイアウト初期化、DPI反映
            dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);// DataGridViewのセルサイズ調整
            dataGridView1.Columns["TargetPath"].Visible = false;// TargetPathを非表示に
            if (BootUpdateCheck == true)
            {
                CheckLatestVersion();// 更新の確認
            }
        }

        #region メニューバー関係
        private void NewProjectToolStripMenuItem_Click(object sender, EventArgs e)// 新規プロジェクト作成
        {
            if (SaveAndCloseEditButton.Visible == true)// 編集中の場合は警告を表示
            {
                if (CheckEditingContents() == true)// 編集中のファイルへの操作が完了した場合
                {
                    MakeNewProject makenewproject = new MakeNewProject("", ColorSetting);
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
                MakeNewProject makenewproject = new MakeNewProject("", ColorSetting);
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
            if (StartUpListOutput == true)
            {
                if (ListOutputFormat == "CSV")
                {
                    CSVListOutputMethod();
                }
                else if (ListOutputFormat == "TSV")
                {
                    TSVListOutputMethod();
                }
                else
                {
                    MessageBox.Show("値が不正です。", "CREC");
                }
            }
            if (StartUpBackUp == true)// 自動バックアップ
            {
                BackUpMethod();
                MakeBackUpZip();// ZIP圧縮を非同期で開始
            }
        }
        private void OpenProjectMethod()// 既存の在庫管理プロジェクトを読み込むメソッド
        {
            // 開いているプロジェクトがあった場合は内容を保存
            if (TargetContentsPath.Length > 0)
            {
                SaveSearchSettings();
            }

            if (SaveAndCloseEditButton.Visible == true)// 編集中の場合は警告を表示
            {
                if (CheckEditingContents() == true)// 編集中のファイルへの操作が完了した場合
                {
                    OpenFileDialog openFolderDialog = new OpenFileDialog();
                    openFolderDialog.InitialDirectory = Directory.GetCurrentDirectory();
                    openFolderDialog.Title = "ファイルを選択してください";
                    openFolderDialog.Filter = "管理ファイル|*.crec";
                    if (openFolderDialog.ShowDialog() == DialogResult.OK)// ファイル読み込み成功
                    {
                        DataLoadingStatus = "false";
                        TargetCRECPath = openFolderDialog.FileName;
                        openFolderDialog.Dispose();
                        LoadProjectFileMethod();// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
                    }
                    else// ファイル読み込み失敗
                    {
                    }
                }
                else// キャンセルされた場合
                {
                    return;
                }
            }
            else// 編集中データがなかった場合
            {
                OpenFileDialog openFolderDialog = new OpenFileDialog();
                openFolderDialog.InitialDirectory = Directory.GetCurrentDirectory();
                openFolderDialog.Title = "ファイルを選択してください";
                openFolderDialog.Filter = "管理ファイル|*.crec";
                if (openFolderDialog.ShowDialog() == DialogResult.OK)// ファイル読み込み成功
                {
                    DataLoadingStatus = "false";
                    TargetCRECPath = openFolderDialog.FileName;
                    openFolderDialog.Dispose();
                    LoadProjectFileMethod();// プロジェクトファイル(CREC)を読み込むメソッドの呼び出し
                }
                else// ファイル読み込み失敗
                {
                }
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
            IEnumerable<string> tmp = null;
            if (File.Exists(TargetCRECPath))
            {
                try
                {
                    tmp = File.ReadLines(TargetCRECPath, Encoding.GetEncoding("UTF-8"));
                }
                catch
                {
                    MessageBox.Show("プロジェクトファイルの読み込みに失敗しました。", "CREC");
                    return;
                }
            }
            else
            {
                MessageBox.Show("プロジェクトファイルが見つかりませんでした。", "CREC");
                return;
            }
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
            foreach (string line in tmp)
            {
                cols = line.Split(',');
                switch (cols[0])
                {
                    case "projectname":
                        TargetProjectName = cols[1];
                        ShowProjcetNameTextBox.Text = "プロジェクト名：" + cols[1];
                        break;
                    case "projectlocation":
                        TargetFolderPath = cols[1];
                        break;
                    case "backuplocation":
                        TargetBackupPath = cols[1];
                        break;
                    case "autobackup":
                        if (cols[1].Contains("S"))
                        {
                            StartUpBackUp = true;
                        }
                        else
                        {
                            StartUpBackUp = false;
                        }
                        if (cols[1].Contains("C"))
                        {
                            CloseBackUp = true;
                        }
                        else
                        {
                            CloseBackUp = false;
                        }
                        if (cols[1].Contains("E"))
                        {
                            EditedBackUp = true;
                        }
                        else
                        {
                            EditedBackUp = false;
                        }
                        break;
                    case "CompressType":
                        try
                        {
                            CompressType = Convert.ToInt32(cols[1]);
                        }
                        catch (Exception ex)
                        {
                            System.Windows.Forms.MessageBox.Show(ex.Message);
                            CompressType = 1;
                        }
                        break;
                    case "Listoutputlocation":
                        TargetListOutputPath = cols[1];
                        break;
                    case "autoListoutput":
                        if (cols[1].Contains("S"))
                        {
                            StartUpListOutput = true;
                        }
                        else
                        {
                            StartUpListOutput = false;
                        }
                        if (cols[1].Contains("C"))
                        {
                            CloseListOutput = true;
                        }
                        else
                        {
                            CloseListOutput = false;
                        }
                        if (cols[1].Contains("E"))
                        {
                            EditedListOutput = true;
                        }
                        else
                        {
                            EditedListOutput = false;
                        }
                        break;
                    case "openListafteroutput":
                        if (cols[1].Contains("O"))
                        {
                            OpenListAfterOutput = true;
                        }
                        else
                        {
                            OpenListAfterOutput = false;
                        }
                        break;
                    case "ListOutputFormat":
                        if (cols[1] == "CSV")
                        {
                            ListOutputFormat = "CSV";
                        }
                        else if (cols[1] == "TSV")
                        {
                            ListOutputFormat = "TSV";
                        }
                        break;
                    case "created":
                        TargetCreatedDate = cols[1];
                        break;
                    case "modified":
                        TargetModifiedDate = cols[1];
                        break;
                    case "accessed":
                        TargetAccessedDate = cols[1];
                        break;
                    case "ShowObjectNameLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                ShowObjectNameLabel = cols[1];
                            }
                            else
                            {
                                ShowCategoryLabel = "名称";
                            }
                            if (cols[2] == "f")
                            {
                                ShowObjectNameLabelVisible = false;
                                ObjectNameLabel.Visible = false;
                                ShowObjectName.Visible = false;
                            }
                            else
                            {
                                ShowObjectNameLabelVisible = true;
                                ObjectNameLabel.Visible = true;
                                ShowObjectName.Visible = true;
                            }
                        }
                        catch
                        {
                            ShowObjectNameLabelVisible = true;
                            ObjectNameLabel.Visible = true;
                            ShowObjectName.Visible = true;
                        }
                        break;
                    case "ShowIDLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                ShowIDLabel = cols[1];
                            }
                            else
                            {
                                ShowIDLabel = "ID";
                            }
                            if (cols[2] == "f")
                            {
                                ShowIDLabelVisible = false;
                                IDLabel.Visible = false;
                                ShowID.Visible = false;
                            }
                            else
                            {
                                ShowIDLabelVisible = true;
                                IDLabel.Visible = true;
                                ShowID.Visible = true;
                                AllowEditIDButton.Visible = true;
                            }
                        }
                        catch
                        {
                            ShowIDLabelVisible = true;
                            IDLabel.Visible = true;
                            ShowID.Visible = true;
                            AllowEditIDButton.Visible = true;
                        }
                        break;
                    case "ShowMCLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                ShowMCLabel = cols[1];
                            }
                            else
                            {
                                ShowMCLabel = "管理コード";
                            }
                            if (cols[2] == "f")
                            {
                                ShowMCLabelVisible = false;
                                MCLabel.Visible = false;
                                ShowMC.Visible = false;
                            }
                            else
                            {
                                ShowMCLabelVisible = true;
                                MCLabel.Visible = true;
                                ShowMC.Visible = true;
                                CheckSameMCButton.Visible = true;
                            }
                        }
                        catch
                        {
                            ShowMCLabelVisible = true;
                            MCLabel.Visible = true;
                            ShowMC.Visible = true;
                            CheckSameMCButton.Visible = true;
                        }
                        break;
                    case "ShowRegistrationDateLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                ShowRegistrationDateLabel = cols[1];
                            }
                            else
                            {
                                ShowRegistrationDateLabel = "登録日";
                            }
                            if (cols[2] == "f")
                            {
                                ShowRegistrationDateLabelVisible = false;
                                RegistrationDateLabel.Visible = false;
                                ShowRegistrationDate.Visible = false;
                            }
                            else
                            {
                                ShowRegistrationDateLabelVisible = true;
                                RegistrationDateLabel.Visible = true;
                                ShowRegistrationDate.Visible = true;
                            }
                        }
                        catch
                        {
                            ShowRegistrationDateLabelVisible = true;
                            RegistrationDateLabel.Visible = true;
                            ShowRegistrationDate.Visible = true;
                        }
                        break;
                    case "AutoMCFill":
                        try
                        {
                            if (cols[1] == "f")
                            {
                                AutoMCFill = false;
                            }
                            else
                            {
                                AutoMCFill = true;
                            }
                        }
                        catch
                        {
                            AutoMCFill = true;
                        }
                        break;
                    case "ShowCategoryLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                ShowCategoryLabel = cols[1];
                            }
                            else
                            {
                                ShowCategoryLabel = "カテゴリ";
                            }
                            if (cols[2] == "f")
                            {
                                ShowCategoryLabelVisible = false;
                                CategoryLabel.Visible = false;
                                ShowCategory.Visible = false;
                            }
                            else
                            {
                                ShowCategoryLabelVisible = true;
                                CategoryLabel.Visible = true;
                                ShowCategory.Visible = true;
                            }
                        }
                        catch
                        {
                            ShowCategoryLabelVisible = true;
                            CategoryLabel.Visible = true;
                            ShowCategory.Visible = true;
                        }
                        break;
                    case "Tag1Name":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                Tag1Name = cols[1];
                            }
                            else
                            {
                                Tag1Name = "タグ１";
                            }
                            if (cols[2] == "f")
                            {
                                ShowTag1NameVisible = false;
                                Tag1NameLabel.Visible = false;
                                ShowTag1.Visible = false;
                            }
                            else
                            {
                                ShowTag1NameVisible = true;
                                Tag1NameLabel.Visible = true;
                                ShowTag1.Visible = true;
                            }
                        }
                        catch
                        {
                            ShowTag1NameVisible = true;
                            Tag1NameLabel.Visible = true;
                            ShowTag1.Visible = true;
                        }
                        break;
                    case "Tag2Name":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                Tag2Name = cols[1];
                            }
                            else
                            {
                                Tag2Name = "タグ２";
                            }
                            if (cols[2] == "f")
                            {
                                ShowTag2NameVisible = false;
                                Tag2NameLabel.Visible = false;
                                ShowTag2.Visible = false;
                            }
                            else
                            {
                                ShowTag2NameVisible = true;
                                Tag2NameLabel.Visible = true;
                                ShowTag2.Visible = true;
                            }
                        }
                        catch
                        {
                            ShowTag2NameVisible = true;
                            Tag2NameLabel.Visible = true;
                            ShowTag2.Visible = true;
                        }
                        break;
                    case "Tag3Name":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                Tag3Name = cols[1];
                            }
                            else
                            {
                                Tag3Name = "タグ３";
                            }
                            if (cols[2] == "f")
                            {
                                ShowTag3NameVisible = false;
                                Tag3NameLabel.Visible = false;
                                ShowTag3.Visible = false;
                            }
                            else
                            {
                                ShowTag3NameVisible = true;
                                Tag3NameLabel.Visible = true;
                                ShowTag3.Visible = true;
                            }
                        }
                        catch
                        {
                            ShowTag3NameVisible = true;
                            Tag3NameLabel.Visible = true;
                            ShowTag3.Visible = true;
                        }
                        break;
                    case "ShowRealLocationLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                ShowRealLocationLabel = cols[1];
                            }
                            else
                            {
                                ShowRealLocationLabel = "現物保管場所";
                            }
                            if (cols[2] == "f")
                            {
                                ShowRealLocationLabelVisible = false;
                                RealLocationLabel.Visible = false;
                                ShowRealLocation.Visible = false;
                            }
                            else
                            {
                                ShowRealLocationLabelVisible = true;
                                RealLocationLabel.Visible = true;
                                ShowRealLocation.Visible = true;
                            }
                        }
                        catch
                        {
                            ShowRealLocationLabelVisible = true;
                            RealLocationLabel.Visible = true;
                            ShowRealLocation.Visible = true;
                        }
                        break;
                    case "ShowDataLocationLabel":
                        try
                        {
                            if (cols[1].Length > 0)
                            {
                                ShowDataLocationLabel = cols[1];
                            }
                            else
                            {
                                ShowDataLocationLabel = "データ保管場所";
                            }
                            if (cols[2] == "f")
                            {
                                ShowDataLocationLabelVisible = false;
                                ShowDataLocation.Visible = false;
                                OpenDataLocation.Visible = false;
                                CopyDataLocationPath.Visible = false;
                            }
                            else
                            {
                                ShowDataLocationLabelVisible = true;
                                ShowDataLocation.Visible = true;
                                OpenDataLocation.Visible = true;
                                CopyDataLocationPath.Visible = true;
                            }
                        }
                        catch
                        {
                            ShowDataLocationLabelVisible = true;
                            ShowDataLocation.Visible = true;
                            OpenDataLocation.Visible = true;
                            CopyDataLocationPath.Visible = true;
                        }
                        break;
                    case "SearchOptionNumber":
                        SearchOptionNumber = Convert.ToInt32(cols[1]);
                        break;
                    case "SearchMethodNumber":
                        SearchMethodNumber = Convert.ToInt32(cols[1]);
                        break;
                    case "IDListVisible":
                        if (cols[1] == "false")
                        {
                            IDList.Visible = false;
                            IDListVisibleToolStripMenuItem.Checked = false;
                        }
                        else
                        {
                            IDList.Visible = true;
                            IDListVisibleToolStripMenuItem.Checked = true;
                        }
                        break;
                    case "MCListVisible":
                        if (cols[1] == "false")
                        {
                            MCList.Visible = false;
                            MCListVisibleToolStripMenuItem.Checked = false;
                        }
                        else
                        {
                            MCList.Visible = true;
                            MCListVisibleToolStripMenuItem.Checked = true;
                        }
                        break;
                    case "ObjectNameListVisible":
                        if (cols[1] == "false")
                        {
                            ObjectNameList.Visible = false;
                            NameListVisibleToolStripMenuItem.Checked = false;
                        }
                        else
                        {
                            ObjectNameList.Visible = true;
                            NameListVisibleToolStripMenuItem.Checked = true;
                        }
                        break;
                    case "RegistrationDateListVisible":
                        if (cols[1] == "false")
                        {
                            RegistrationDateList.Visible = false;
                            RegistrationDateListVisibleToolStripMenuItem.Checked = false;
                        }
                        else
                        {
                            RegistrationDateList.Visible = true;
                            RegistrationDateListVisibleToolStripMenuItem.Checked = true;
                        }
                        break;
                    case "CategoryListVisible":
                        if (cols[1] == "false")
                        {
                            CategoryList.Visible = false;
                            CategoryListVisibleToolStripMenuItem.Checked = false;
                        }
                        else
                        {
                            CategoryList.Visible = true;
                            CategoryListVisibleToolStripMenuItem.Checked = true;
                        }
                        break;
                    case "Tag1ListVisible":
                        if (cols[1] == "false")
                        {
                            Tag1List.Visible = false;
                            Tag1ListVisibleToolStripMenuItem.Checked = false;
                        }
                        else
                        {
                            Tag1List.Visible = true;
                            Tag1ListVisibleToolStripMenuItem.Checked = true;
                        }
                        break;
                    case "Tag2ListVisible":
                        if (cols[1] == "false")
                        {
                            Tag2List.Visible = false;
                            Tag2ListVisibleToolStripMenuItem.Checked = false;
                        }
                        else
                        {
                            Tag2List.Visible = true;
                            Tag2ListVisibleToolStripMenuItem.Checked = true;
                        }
                        break;
                    case "Tag3ListVisible":
                        if (cols[1] == "false")
                        {
                            Tag3List.Visible = false;
                            Tag3ListVisibleToolStripMenuItem.Checked = false;
                        }
                        else
                        {
                            Tag3List.Visible = true;
                            Tag3ListVisibleToolStripMenuItem.Checked = true;
                        }
                        break;
                    case "InventoryInformationListVisible":
                        if (cols[1] == "false")
                        {
                            InventoryList.Visible = false;
                            InventoryStatusList.Visible = false;
                            InventoryInformationListToolStripMenuItem.Checked = false;
                        }
                        else
                        {
                            InventoryList.Visible = true;
                            InventoryStatusList.Visible = true;
                            InventoryInformationListToolStripMenuItem.Checked = true;
                        }
                        break;
                }
                if (IDListVisibleToolStripMenuItem.Checked == false
                    && MCListVisibleToolStripMenuItem.Checked == false
                    && NameListVisibleToolStripMenuItem.Checked == false
                    && RegistrationDateListVisibleToolStripMenuItem.Checked == false
                    && CategoryListVisibleToolStripMenuItem.Checked == false
                    && Tag1ListVisibleToolStripMenuItem.Checked == false
                    && Tag2ListVisibleToolStripMenuItem.Checked == false
                    && Tag3ListVisibleToolStripMenuItem.Checked == false
                    && InventoryList.Visible == false)
                {
                    MessageBox.Show("全項目が非表示状態に設定されています。システム上IDのみ表示します。", "CREC");
                    IDList.Visible = true;
                    IDListVisibleToolStripMenuItem.Checked = true;
                }
            }
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
            // ラベルの名称を読み込んで詳細表示画面に設定
            ObjectNameLabel.Text = ShowObjectNameLabel + "：";
            IDLabel.Text = ShowIDLabel + "：";
            MCLabel.Text = ShowMCLabel + "：";
            RegistrationDateLabel.Text = ShowRegistrationDateLabel + "：";
            CategoryLabel.Text = ShowCategoryLabel + "：";
            Tag1NameLabel.Text = Tag1Name + "：";
            Tag2NameLabel.Text = Tag2Name + "：";
            Tag3NameLabel.Text = Tag3Name + "：";
            RealLocationLabel.Text = ShowRealLocationLabel + "：";
            ShowDataLocation.Text = ShowDataLocationLabel + "：";
            // ラベルの名称を読み込んで検索ボックスに設定、順番注意
            SearchOptionComboBox.Items.Clear();
            SearchOptionComboBox.Items.Add(ShowIDLabel);
            SearchOptionComboBox.Items.Add(ShowMCLabel);
            SearchOptionComboBox.Items.Add(ShowObjectNameLabel);
            SearchOptionComboBox.Items.Add(ShowCategoryLabel);
            SearchOptionComboBox.Items.Add(Tag1Name);
            SearchOptionComboBox.Items.Add(Tag2Name);
            SearchOptionComboBox.Items.Add(Tag3Name);
            SearchOptionComboBox.Items.Add("在庫状況");
            // ラベルの名称を読み込んでDGVに設定
            dataGridView1.Refresh();
            dataGridView1.Columns["IDList"].HeaderText = ShowIDLabel;
            dataGridView1.Columns["IDList"].Visible = IDListVisibleToolStripMenuItem.Checked;
            dataGridView1.Columns["MCList"].HeaderText = ShowMCLabel;
            dataGridView1.Columns["MCList"].Visible = MCListVisibleToolStripMenuItem.Checked;
            dataGridView1.Columns["ObjectNameList"].HeaderText = ShowObjectNameLabel;
            dataGridView1.Columns["ObjectNameList"].Visible = NameListVisibleToolStripMenuItem.Checked;
            dataGridView1.Columns["RegistrationDateList"].HeaderText = ShowRegistrationDateLabel;
            dataGridView1.Columns["RegistrationDateList"].Visible = RegistrationDateListVisibleToolStripMenuItem.Checked;
            dataGridView1.Columns["CategoryList"].HeaderText = ShowCategoryLabel;
            dataGridView1.Columns["CategoryList"].Visible = CategoryListVisibleToolStripMenuItem.Checked;
            dataGridView1.Columns["Tag1List"].HeaderText = Tag1Name;
            dataGridView1.Columns["Tag1List"].Visible = Tag1ListVisibleToolStripMenuItem.Checked;
            dataGridView1.Columns["Tag2List"].HeaderText = Tag2Name;
            dataGridView1.Columns["Tag2List"].Visible = Tag2ListVisibleToolStripMenuItem.Checked;
            dataGridView1.Columns["Tag3List"].HeaderText = Tag3Name;
            dataGridView1.Columns["Tag3List"].Visible = Tag3ListVisibleToolStripMenuItem.Checked;
            // ラベルの名称を読み込んでDGVのList表示・非表示設定画面に追加
            IDListVisibleToolStripMenuItem.Text = ShowIDLabel;
            MCListVisibleToolStripMenuItem.Text = ShowMCLabel;
            NameListVisibleToolStripMenuItem.Text = ShowObjectNameLabel;
            RegistrationDateListVisibleToolStripMenuItem.Text = ShowRegistrationDateLabel;
            CategoryListVisibleToolStripMenuItem.Text = ShowCategoryLabel;
            Tag1ListVisibleToolStripMenuItem.Text = Tag1Name;
            Tag2ListVisibleToolStripMenuItem.Text = Tag2Name;
            Tag3ListVisibleToolStripMenuItem.Text = Tag3Name;
            // ToolTipsの設定
            SetTagNameToolTips();
            // ListOutputPathの設定
            if (!Directory.Exists(TargetListOutputPath))
            {
                TargetListOutputPath = TargetFolderPath;
            }
            // ComboBoxの初期選択項目を設定
            SearchOptionComboBox.SelectedIndexChanged -= SearchOptionComboBox_SelectedIndexChanged;
            SearchMethodComboBox.SelectedIndexChanged -= SearchMethodComboBox_SelectedIndexChanged;
            SearchOptionComboBox.SelectedIndex = SearchOptionNumber;
            SearchMethodComboBox.SelectedIndex = SearchMethodNumber;
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
                    File.Delete("RecentlyOpenedProjectList.log");
                    StreamWriter streamWriter = new StreamWriter("RecentlyOpenedProjectList.log", true, Encoding.GetEncoding("UTF-8"));
                    streamWriter.WriteLine(TargetProjectName + "," + TargetCRECPath);// 今開いたプロジェクトを書き込み
                    int Count = 0;
                    foreach (string line in RecentlyOpendProjectList)
                    {
                        if (line != TargetProjectName + "," + TargetCRECPath)// 重複を回避
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
                    streamWriter.WriteLine(TargetProjectName + "," + TargetCRECPath);// 今開いたプロジェクトを書き込み
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
                DeleteRecentlyOpendProjectListToolStripMenuItem.Text = "履歴を削除";
                OpenRecentlyOpendProjectToolStripMenuItem.DropDownItems.Add(DeleteRecentlyOpendProjectListToolStripMenuItem);
            }
            else
            {
                ToolStripItem NoRecentlyOpendProjectListToolStripMenuItem = new ToolStripMenuItem();
                NoRecentlyOpendProjectListToolStripMenuItem.Text = "履歴はありません";
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
            if (TargetContentsPath.Length > 0)
            {
                SaveSearchSettings();
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
                File.Delete("RecentlyOpenedProjectList.log");
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
                    File.Delete("RecentlyOpenedProjectList.log");
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
            if (TargetContentsPath.Length == 0)
            {
                MessageBox.Show("先にプロジェクトを開いてください。", "CREC");
                return;
            }
            if (TargetBackupPath.Length == 0)
            {
                MessageBox.Show("バックアップフォルダが設定されていません。", "CREC");
                return;
            }
            try
            {
                System.Diagnostics.Process.Start(TargetBackupPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("フォルダを開けませんでした\n" + ex.Message, "CREC");
            }
        }
        private void OutputListAllContentsToolStripMenuItem_Click(object sender, EventArgs e)// プロジェクトの全データの一覧をListに出力
        {
            if (ListOutputFormat == "CSV")
            {
                CSVListOutputMethod();
            }
            else if (ListOutputFormat == "TSV")
            {
                TSVListOutputMethod();
            }
            else
            {
                MessageBox.Show("値が不正です。", "CREC");
            }
        }
        private void OutputListShownContentsToolStripMenuItem_Click(object sender, EventArgs e)// 一覧に表示中のデータのみ一覧をListに出力
        {
            try
            {
                string tempTargetListOutputPath = "";
                if (Directory.Exists(TargetListOutputPath))
                {
                    tempTargetListOutputPath = TargetListOutputPath;
                }
                else
                {
                    tempTargetListOutputPath = TargetFolderPath;
                }
                if (ListOutputFormat == "CSV")// CSV出力用
                {
                    StreamWriter streamWriter = new StreamWriter(tempTargetListOutputPath + "\\InventoryOutput.csv", false, Encoding.GetEncoding("shift-jis"));
                    streamWriter.WriteLine("データ保存場所のパス,ID,管理コード,名称,登録日,カテゴリー,タグ1,タグ2,タグ3,在庫数,在庫状況");
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(TargetFolderPath);
                    try
                    {
                        IEnumerable<System.IO.DirectoryInfo> subFolders = di.EnumerateDirectories("*");
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            // 変数初期化「List作成処理ないでのみ使用すること」
                            string ListThisName = "";
                            string ListThisID = "";
                            string ListThisMC = "";
                            string ListThisCategory = "";
                            string ListThisTag1 = "";
                            string ListThisTag2 = "";
                            string ListThisTag3 = "";
                            string ListRegistrationDate = "";
                            string ListInventory = "";
                            string ListInventoryStatus = "";
                            int? ListSafetyStock = null;
                            int? ListReorderPoint = null;
                            int? ListMaximumLevel = null;
                            // index読み込み
                            IEnumerable<string> tmp = null;
                            string ListContentsPath = Convert.ToString(dataGridView1.Rows[i].Cells[0].Value);
                            try
                            {
                                tmp = File.ReadLines(ListContentsPath + "\\index.txt", Encoding.GetEncoding("UTF-8"));
                                foreach (string line in tmp)
                                {
                                    cols = line.Split(',');
                                    switch (cols[0])
                                    {
                                        case "名称":
                                            ListThisName = line.Substring(3).Replace(",", "");
                                            break;
                                        case "ID":
                                            ListThisID = line.Substring(3).Replace(",", "");
                                            break;
                                        case "MC":
                                            ListThisMC = line.Substring(3).Replace(",", "");
                                            break;
                                        case "登録日":
                                            ListRegistrationDate = line.Substring(4).Replace(",", "");
                                            break;
                                        case "カテゴリ":
                                            ListThisCategory = line.Substring(5).Replace(",", "");
                                            break;
                                        case "タグ1":
                                            ListThisTag1 = line.Substring(4).Replace(",", "");
                                            break;
                                        case "タグ2":
                                            ListThisTag2 = line.Substring(4).Replace(",", "");
                                            break;
                                        case "タグ3":
                                            ListThisTag3 = line.Substring(4).Replace(",", "");
                                            break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Indexファイルが破損しています。\n" + ex.Message, "CREC");
                                ListThisID = "Status：Indexファイル破損";
                                ListThisName = "Status：Indexファイル破損";
                                ListThisCategory = "　ー　";
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
                            streamWriter.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", ListContentsPath, ListThisID, ListThisMC, ListThisName, ListRegistrationDate, ListThisCategory, ListThisTag1, ListThisTag2, ListThisTag3, ListInventory, ListInventoryStatus);
                        }
                        streamWriter.Close();
                        MessageBox.Show("データ一覧を以下の場所にCSV形式で出力しました。\n" + tempTargetListOutputPath + "\\InventoryOutput.csv", "CREC");
                    }
                    catch (Exception ex)
                    {

                    }
                    if (OpenListAfterOutput == true)
                    {
                        System.Diagnostics.Process process = System.Diagnostics.Process.Start(tempTargetListOutputPath + "\\InventoryOutput.csv");
                    }
                }
                else if (ListOutputFormat == "TSV")
                {
                    StreamWriter streamWriter = new StreamWriter(tempTargetListOutputPath + "\\InventoryOutput.tsv", false, Encoding.GetEncoding("shift-jis"));
                    streamWriter.WriteLine("データ保存場所のパス\tID\t管理コード\t名称\t登録日\tカテゴリー\tタグ1\tタグ2\tタグ3\t在庫数\t在庫状況");
                    System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(TargetFolderPath);
                    try
                    {
                        IEnumerable<System.IO.DirectoryInfo> subFolders = di.EnumerateDirectories("*");
                        for (int i = 0; i < dataGridView1.RowCount; i++)
                        {
                            // 変数初期化「List作成処理ないでのみ使用すること」
                            string ListThisName = "";
                            string ListThisID = "";
                            string ListThisMC = "";
                            string ListThisCategory = "";
                            string ListThisTag1 = "";
                            string ListThisTag2 = "";
                            string ListThisTag3 = "";
                            string ListRegistrationDate = "";
                            string ListInventory = "";
                            string ListInventoryStatus = "";
                            int? ListSafetyStock = null;
                            int? ListReorderPoint = null;
                            int? ListMaximumLevel = null;
                            // index読み込み
                            IEnumerable<string> tmp = null;
                            string ListContentsPath = Convert.ToString(dataGridView1.Rows[i].Cells[0].Value);
                            try
                            {
                                tmp = File.ReadLines(ListContentsPath + "\\index.txt", Encoding.GetEncoding("UTF-8"));
                                foreach (string line in tmp)
                                {
                                    cols = line.Split(',');
                                    switch (cols[0])
                                    {
                                        case "名称":
                                            ListThisName = line.Substring(3);
                                            break;
                                        case "ID":
                                            ListThisID = line.Substring(3);
                                            break;
                                        case "MC":
                                            ListThisMC = line.Substring(3);
                                            break;
                                        case "登録日":
                                            ListRegistrationDate = line.Substring(4);
                                            break;
                                        case "カテゴリ":
                                            ListThisCategory = line.Substring(5);
                                            break;
                                        case "タグ1":
                                            ListThisTag1 = line.Substring(4);
                                            break;
                                        case "タグ2":
                                            ListThisTag2 = line.Substring(4);
                                            break;
                                        case "タグ3":
                                            ListThisTag3 = line.Substring(4);
                                            break;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Indexファイルが破損しています。\n" + ex.Message, "CREC");
                                ListThisID = "Status：Indexファイル破損";
                                ListThisName = "Status：Indexファイル破損";
                                ListThisCategory = "　ー　";
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
                            streamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}", ListContentsPath, ListThisID, ListThisMC, ListThisName, ListRegistrationDate, ListThisCategory, ListThisTag1, ListThisTag2, ListThisTag3, ListInventory, ListInventoryStatus);
                        }
                        streamWriter.Close();
                        MessageBox.Show("データ一覧を以下の場所にTSV形式で出力しました。\n" + tempTargetListOutputPath + "\\InventoryOutput.tsv", "CREC");
                    }
                    catch (Exception ex)
                    {

                    }
                    if (OpenListAfterOutput == true)
                    {
                        System.Diagnostics.Process process = System.Diagnostics.Process.Start(tempTargetListOutputPath + "\\InventoryOutput.tsv");
                    }
                }
                else
                {
                    MessageBox.Show("値が不正です。", "CREC");
                }
            }
            catch (Exception ex) { }
        }
        private void EditConfigSysToolStripMenuItem_Click(object sender, EventArgs e)// 環境設定編集画面
        {
            ConfigForm configform = new ConfigForm(ColorSetting);
            configform.ShowDialog();
            ImportConfig();// 更新したconfigファイルを読み込み
        }
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)// プログラム終了
        {
            this.Close();
        }
        private void RestartToolStripMenuItem_Click(object sender, EventArgs e)// アプリ再起動
        {
            if (SaveAndCloseEditButton.Visible == true)// 編集中のデータがある場合
            {
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("編集中のデータがあります。保存しますか？\n保存されなかったデータは削除されます。", "CREC", System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Warning);
                if (result == System.Windows.MessageBoxResult.Yes)// 保存してアプリを終了
                {
                    // 保存関係の処理、入力内容を確認
                    if (CheckContent() == false)
                    {
                        return;// 不備があった場合は再起動キャンセル
                    }
                    SaveContentsMethod();// データ保存メソッドを呼び出し
                }
                else if (result == System.Windows.MessageBoxResult.No)// 保存せずアプリを終了（一時データを削除）
                {
                    // 編集中タグを削除
                    File.Delete(TargetContentsPath + "\\DED");
                    File.Delete(TargetContentsPath + "\\RED");
                    // サムネ画像が更新されていた場合は一時データをを削除
                    if (File.Exists(TargetContentsPath + "\\pictures\\Thumbnail1.newjpg"))
                    {
                        File.Delete(TargetContentsPath + "\\pictures\\Thumbnail1.newjpg");
                    }
                    // 新規作成の場合はデータを削除
                    if (File.Exists(TargetContentsPath + "\\ADD"))
                    {
                        DeleteContent();
                    }
                    else
                    {
                        File.Delete(TargetContentsPath + "\\ADD");
                    }
                }
                else if (result == System.Windows.MessageBoxResult.Cancel)// アプリ再起動をキャンセル
                {
                    return;
                }
            }
            System.Windows.Forms.Application.Restart();
        }
        private void AddContentsToolStripMenuItem_Click(object sender, EventArgs e)// 新規追加
        {
            AddContentsMethod();// 新規にデータを追加するメソッドを呼び出し
        }
        private void ResetEditingContentsToolStripMenuItem_Click(object sender, EventArgs e)// 編集内容をリセット
        {
            if (SaveAndCloseEditButton.Visible == true)// 編集中の場合は警告を表示
            {
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("編集中のデータを破棄し、編集前の状態に戻しますか？", "CREC", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    // 詳細情報読み込み＆表示
                    StreamReader sr1 = null;
                    try
                    {
                        sr1 = new StreamReader(TargetDetailsPath);
                    }
                    catch
                    {
                        DetailsTextBox.Text = "No Data.";
                    }
                    finally
                    {
                        if (sr1 != null)
                        {
                            DetailsTextBox.Text = sr1.ReadToEnd();
                            sr1.Close();
                        }
                    }
                    // 機密情報を読み込み
                    try
                    {
                        StreamReader sr2 = new StreamReader(TargetContentsPath + "\\confidentialdata.txt");
                        ConfidentialDataTextBox.Text = sr2.ReadToEnd();
                        sr2.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("データの読み込みに失敗しました\n" + ex.Message, "CREC");
                    }
                    EditNameTextBox.Text = ThisName;
                    EditIDTextBox.TextChanged -= IDTextBox_TextChanged;// ID重複確認イベントを停止
                    EditIDTextBox.Text = ThisID;
                    EditIDTextBox.TextChanged += IDTextBox_TextChanged;// ID重複確認イベントを開始
                    AllowEditIDButton.Text = "編集不可";
                    ReissueUUIDToolStripMenuItem.Enabled = false;
                    EditMCTextBox.Text = ThisMC;
                    EditRegistrationDateTextBox.Text = ThisRegistrationDate;
                    EditCategoryTextBox.Text = ThisCategory;
                    EditTag1TextBox.Text = ThisTag1;
                    EditTag2TextBox.Text = ThisTag2;
                    EditTag3TextBox.Text = ThisTag3;
                    EditRealLocationTextBox.Text = ThisRealLocation;
                }
                else if (result == System.Windows.MessageBoxResult.No)
                {
                    // 何もしない
                }
            }
            else
            {
                // 何もしない
            }
        }
        private void AddInventoryModeToolStripMenuItem_Click(object sender, EventArgs e)// 在庫数管理モードを追加
        {
            if (TargetContentsPath.Length == 0)
            {
                MessageBox.Show("表示するデータを選択し、詳細表示してください。", "CREC");
                return;
            }
            if (File.Exists(TargetContentsPath + "\\inventory.inv"))// 在庫数管理モードの表示・非表示
            {
                MessageBox.Show("在庫数管理ファイルは作成済みです。", "CREC");
            }
            else
            {
                FileStream FileStream = File.Create(TargetContentsPath + "\\inventory.inv");
                FileStream.Close();
                StreamWriter InventoryManagementFile = new StreamWriter(TargetContentsPath + "\\inventory.inv");
                InventoryManagementFile.WriteLine("{0},,,", ThisID);
                InventoryManagementFile.Close();
                InventoryManagementModeButton.Visible = true;
            }
        }
        #region データ一覧の表示項目設定
        private void IDVisibleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)// IDの表示・非表示
        {

            if (IDListVisibleToolStripMenuItem.Checked == true)
            {
                IDList.Visible = true;
                dataGridView1.Columns["IDList"].Visible = true;
            }
            else if (IDListVisibleToolStripMenuItem.Checked == false)
            {
                IDList.Visible = false;
                dataGridView1.Columns["IDList"].Visible = false;
            }
            //選択後もMenuItem開いたままにする処理
            ViewToolStripMenuItem.ShowDropDown();
            VisibleListElementsToolStripMenuItem.ShowDropDown();
        }
        private void MCVisibleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)// 管理コードの表示・非表示
        {
            if (MCListVisibleToolStripMenuItem.Checked == true)
            {
                MCList.Visible = true;
                dataGridView1.Columns["MCList"].Visible = true;
            }
            else if (MCListVisibleToolStripMenuItem.Checked == false)
            {
                MCList.Visible = false;
                dataGridView1.Columns["MCList"].Visible = false;
            }
            // 選択後もMenuItem開いたままにする処理
            ViewToolStripMenuItem.ShowDropDown();
            VisibleListElementsToolStripMenuItem.ShowDropDown();
        }
        private void NameVisibleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)// 名称の表示・非表示
        {
            if (NameListVisibleToolStripMenuItem.Checked == true)
            {
                ObjectNameList.Visible = true;
                dataGridView1.Columns["ObjectNameList"].Visible = true;
            }
            else if (NameListVisibleToolStripMenuItem.Checked == false)
            {
                ObjectNameList.Visible = false;
                dataGridView1.Columns["ObjectNameList"].Visible = false;
            }
            // 選択後もMenuItem開いたままにする処理
            ViewToolStripMenuItem.ShowDropDown();
            VisibleListElementsToolStripMenuItem.ShowDropDown();
        }
        private void RegistrationDateVisibleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)// 登録日の表示・非表示
        {
            if (RegistrationDateListVisibleToolStripMenuItem.Checked == true)
            {
                RegistrationDateList.Visible = true;
                dataGridView1.Columns["RegistrationDateList"].Visible = true;
            }
            else if (RegistrationDateListVisibleToolStripMenuItem.Checked == false)
            {
                RegistrationDateList.Visible = false;
                dataGridView1.Columns["RegistrationDateList"].Visible = false;
            }
            // 選択後もMenuItem開いたままにする処理
            ViewToolStripMenuItem.ShowDropDown();
            VisibleListElementsToolStripMenuItem.ShowDropDown();
        }
        private void CategoryVisibleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)// カテゴリの表示・非表示
        {
            if (CategoryListVisibleToolStripMenuItem.Checked == true)
            {
                CategoryList.Visible = true;
                dataGridView1.Columns["CategoryList"].Visible = true;
            }
            else if (CategoryListVisibleToolStripMenuItem.Checked == false)
            {
                CategoryList.Visible = false;
                dataGridView1.Columns["CategoryList"].Visible = false;
            }
            // 選択後もMenuItem開いたままにする処理
            ViewToolStripMenuItem.ShowDropDown();
            VisibleListElementsToolStripMenuItem.ShowDropDown();
        }
        private void Tag1VisibleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)// タグ１の表示・非表示
        {
            if (Tag1ListVisibleToolStripMenuItem.Checked == true)
            {
                Tag1List.Visible = true;
                dataGridView1.Columns["Tag1List"].Visible = true;
            }
            else if (Tag1ListVisibleToolStripMenuItem.Checked == false)
            {
                Tag1List.Visible = false;
                dataGridView1.Columns["Tag1List"].Visible = false;
            }
            // 選択後もMenuItem開いたままにする処理
            ViewToolStripMenuItem.ShowDropDown();
            VisibleListElementsToolStripMenuItem.ShowDropDown();
        }
        private void Tag2VisibleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)// タグ２の表示・非表示
        {
            if (Tag2ListVisibleToolStripMenuItem.Checked == true)
            {
                Tag2List.Visible = true;
                dataGridView1.Columns["Tag2List"].Visible = true;
            }
            else if (Tag2ListVisibleToolStripMenuItem.Checked == false)
            {
                Tag2List.Visible = false;
                dataGridView1.Columns["Tag2List"].Visible = false;
            }
            // 選択後もMenuItem開いたままにする処理
            ViewToolStripMenuItem.ShowDropDown();
            VisibleListElementsToolStripMenuItem.ShowDropDown();
        }
        private void Tag3VisibleToolStripMenuItem_CheckedChanged(object sender, EventArgs e)// タグ３の表示・非表示
        {
            if (Tag3ListVisibleToolStripMenuItem.Checked == true)
            {
                Tag3List.Visible = true;
                dataGridView1.Columns["Tag3List"].Visible = true;
            }
            else if (Tag3ListVisibleToolStripMenuItem.Checked == false)
            {
                Tag3List.Visible = false;
                dataGridView1.Columns["Tag3List"].Visible = false;
            }
            // 選択後もMenuItem開いたままにする処理
            ViewToolStripMenuItem.ShowDropDown();
            VisibleListElementsToolStripMenuItem.ShowDropDown();
        }
        private void InventoryInformationToolStripMenuItem_CheckedChanged(object sender, EventArgs e)// 在庫状況の表示
        {
            if (InventoryInformationListToolStripMenuItem.Checked == true)
            {
                InventoryList.Visible = true;
                dataGridView1.Columns["InventoryList"].Visible = true;
                InventoryStatusList.Visible = true;
                dataGridView1.Columns["InventoryStatusList"].Visible = true;
            }
            else if (InventoryInformationListToolStripMenuItem.Checked == false)
            {
                InventoryList.Visible = false;
                dataGridView1.Columns["InventoryList"].Visible = false;
                InventoryStatusList.Visible = false;
                dataGridView1.Columns["InventoryStatusList"].Visible = false;
            }
            // 選択後もMenuItem開いたままにする処理
            ViewToolStripMenuItem.ShowDropDown();
            VisibleListElementsToolStripMenuItem.ShowDropDown();
        }
        #endregion
        #region 色設定
        private void AliceBlueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorSetting = "Blue";
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
        }
        private void WhiteSmokeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorSetting = "White";
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
        }
        private void LavenderBlushToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorSetting = "Sakura";
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
        }
        private void HoneydewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorSetting = "Green";
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
        private void ZoomInFontToolStripMenuItem_Click(object sender, EventArgs e)// フォントサイズを1Pt大きくする
        {
            extrasmallfontsize += 1;// 最小フォントのサイズ
            smallfontsize += 1;// 小フォントのサイズ
            mainfontsize += 1;// 標準フォントのサイズ
            bigfontsize += 1;// 大フォントのサイズ
            SetFormLayout();
            //選択後もMenuItem開いたままにする処理
            FontSizeToolStripMenuItem.ShowDropDown();
            ZoomInFontToolStripMenuItem.ShowDropDown();
        }
        private void ZoomOutFontToolStripMenuItem_Click(object sender, EventArgs e)// フォントサイズを1Pt小さくする
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
                SetFormLayout();
                //選択後もMenuItem開いたままにする処理
                FontSizeToolStripMenuItem.ShowDropDown();
                ZoomOutFontToolStripMenuItem.ShowDropDown();
            }
        }
        #endregion
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)// バージョン情報表示
        {
            VersionInformation VerInfo = new VersionInformation(ColorSetting, CurrentDPI);
            VerInfo.ShowDialog();
        }
        private void readmeToolStripMenuItem_Click(object sender, EventArgs e)// ReadMe表示
        {
            ReadMe readme = new ReadMe(ColorSetting);
            readme.ShowDialog();
        }
        private void UpdateHistoryToolStripMenuItem_Click(object sender, EventArgs e)// 更新履歴
        {
            UpdateHistory updateHistory = new UpdateHistory(ColorSetting);
            updateHistory.ShowDialog();
        }
        private void AccessLatestReleaseToolStripMenuItem_Click(object sender, EventArgs e)// WebのLatestReleaseにアクセス
        {
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("Webサイトにアクセスします。\n許可しますか？", "CREC", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
            if (result == System.Windows.MessageBoxResult.Yes)// ブラウザでリンクを表示
            {
                System.Diagnostics.Process.Start("https://github.com/Yukisita/CREC/releases/tag/Latest_Release");
            }
        }
        private void UserManualToolStripMenuItem_Click(object sender, EventArgs e)// 利用ガイド
        {
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("Webサイトにアクセスします。\n許可しますか？", "CREC", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
            if (result == System.Windows.MessageBoxResult.Yes)// ブラウザでリンクを表示
            {
                System.Diagnostics.Process.Start("https://github.com/Yukisita/CREC/wiki/%E4%BD%BF%E7%94%A8%E6%96%B9%E6%B3%95");
            }
        }
        private void Form1_Closing(object sender, CancelEventArgs e)// 終了時の処理
        {
            SaveSearchSettings();
            SaveConfig();
            if (SaveAndCloseEditButton.Visible == true)// 編集中のデータがある場合
            {
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("編集中のデータがあります。保存しますか？\n保存されなかったデータは削除されます。", "CREC", System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Warning);
                if (result == System.Windows.MessageBoxResult.Yes)// 保存してアプリを終了
                {
                    // 入力内容を確認
                    if (CheckContent() == false)
                    {
                        return;
                    }
                    // データ保存メソッドを呼び出し
                    SaveContentsMethod();
                    if (CloseListOutput == true)
                    {
                        if (ListOutputFormat == "CSV")
                        {
                            CSVListOutputMethod();
                        }
                        else if (ListOutputFormat == "TSV")
                        {
                            TSVListOutputMethod();
                        }
                        else
                        {
                            MessageBox.Show("値が不正です。", "CREC");
                        }
                    }
                    if (CloseBackUp == true)// 自動バックアップ
                    {
                        BackUpMethod();
                        this.Hide();// メインフォームを消す
                        CloseBackUpForm closeBackUpForm = new CloseBackUpForm(ColorSetting);
                        Task.Run(() => { closeBackUpForm.ShowDialog(); });// 別プロセスでバックアップ中のプログレスバー表示ウインドウを開く
                        DateTime DT = DateTime.Now;
                        if (CompressType == 0)
                        {
                            try
                            {
                                ZipFile.CreateFromDirectory(TargetBackupPath + "\\backuptmp", TargetBackupPath + "\\" + TargetProjectName + "_backup-" + DT.ToString("yyyy年MM月dd日HH時mm分ss秒") + ".zip");

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("バックアップ作成に失敗しました。\n" + ex.Message, "CREC");
                                BackupToolStripMenuItem.Text = "バックアップ作成";
                                BackupToolStripMenuItem.Enabled = true;
                                return;
                            }
                            Directory.Delete(TargetBackupPath + "\\backuptmp", true);
                        }
                        else if (CompressType == 1)
                        {
                            // バックアップ用フォルダ作成
                            Directory.CreateDirectory(TargetBackupPath + "\\" + TargetProjectName + "_backup-" + DT.ToString("yyyy年MM月dd日HH時mm分ss秒"));
                            File.Copy(TargetCRECPath, TargetBackupPath + "\\" + TargetProjectName + "_backup-" + DT.ToString("yyyy年MM月dd日HH時mm分ss秒") + "\\backup.crec", true);// crecファイルをバックアップ
                            try
                            {
                                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(TargetFolderPath);
                                IEnumerable<System.IO.DirectoryInfo> subFolders = di.EnumerateDirectories("*");
                                foreach (System.IO.DirectoryInfo subFolder in subFolders)
                                {
                                    try
                                    {
                                        FileSystem.CopyDirectory(subFolder.FullName, "backuptmp\\" + subFolder.Name + "\\datatemp", Microsoft.VisualBasic.FileIO.UIOption.AllDialogs, Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing);
                                        ZipFile.CreateFromDirectory("backuptmp\\" + subFolder.Name + "\\datatemp", "backuptmp\\" + subFolder.Name + "backupziptemp.zip");// 圧縮
                                        File.Move("backuptmp\\" + subFolder.Name + "backupziptemp.zip", TargetBackupPath + "\\" + TargetProjectName + "_backup-" + DT.ToString("yyyy年MM月dd日HH時mm分ss秒") + "\\" + subFolder.Name + "_backup-" + DT.ToString("yyyy-MM-dd-HH-mm-ss") + ".zip");// 移動
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
                                MessageBox.Show("バックアップ作成に失敗しました。\n" + ex.Message, "CREC");
                                BackupToolStripMenuItem.Text = "バックアップ作成";
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
                    // 編集中タグを削除
                    File.Delete(TargetContentsPath + "\\RED");
                    File.Delete(TargetContentsPath + "\\DED");
                    // サムネ画像が更新されていた場合は一時データをを削除
                    if (File.Exists(TargetContentsPath + "\\pictures\\Thumbnail1.newjpg"))
                    {
                        File.Delete(TargetContentsPath + "\\pictures\\Thumbnail1.newjpg");
                    }
                    if (File.Exists(TargetContentsPath + "\\ADD"))
                    {
                        DeleteContent();
                    }
                    else
                    {
                        File.Delete(TargetContentsPath + "\\ADD");
                    }
                    if (CloseListOutput == true)
                    {
                        if (ListOutputFormat == "CSV")
                        {
                            CSVListOutputMethod();
                        }
                        else if (ListOutputFormat == "TSV")
                        {
                            TSVListOutputMethod();
                        }
                        else
                        {
                            MessageBox.Show("値が不正です。", "CREC");
                        }
                    }
                    if (CloseBackUp == true)// 自動バックアップ
                    {
                        BackUpMethod();
                        this.Hide();// メインフォームを消す
                        CloseBackUpForm closeBackUpForm = new CloseBackUpForm(ColorSetting);
                        Task.Run(() => { closeBackUpForm.ShowDialog(); });// 別プロセスでバックアップ中のプログレスバー表示ウインドウを開く
                        // バックアップ作成
                        DateTime DT = DateTime.Now;
                        if (CompressType == 0)
                        {
                            try
                            {
                                ZipFile.CreateFromDirectory(TargetBackupPath + "\\backuptmp", TargetBackupPath + "\\" + TargetProjectName + "_backup-" + DT.ToString("yyyy年MM月dd日HH時mm分ss秒") + ".zip");

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("バックアップ作成に失敗しました。\n" + ex.Message, "CREC");
                                BackupToolStripMenuItem.Text = "バックアップ作成";
                                BackupToolStripMenuItem.Enabled = true;
                                return;
                            }
                            Directory.Delete(TargetBackupPath + "\\backuptmp", true);
                        }
                        else if (CompressType == 1)
                        {
                            // バックアップ用フォルダ作成
                            Directory.CreateDirectory(TargetBackupPath + "\\" + TargetProjectName + "_backup-" + DT.ToString("yyyy年MM月dd日HH時mm分ss秒"));
                            File.Copy(TargetCRECPath, TargetBackupPath + "\\" + TargetProjectName + "_backup-" + DT.ToString("yyyy年MM月dd日HH時mm分ss秒") + "\\backup.crec", true);// crecファイルをバックアップ
                            try
                            {
                                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(TargetFolderPath);
                                IEnumerable<System.IO.DirectoryInfo> subFolders = di.EnumerateDirectories("*");
                                foreach (System.IO.DirectoryInfo subFolder in subFolders)
                                {
                                    try
                                    {
                                        FileSystem.CopyDirectory(subFolder.FullName, "backuptmp\\" + subFolder.Name + "\\datatemp", Microsoft.VisualBasic.FileIO.UIOption.AllDialogs, Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing);
                                        ZipFile.CreateFromDirectory("backuptmp\\" + subFolder.Name + "\\datatemp", "backuptmp\\" + subFolder.Name + "backupziptemp.zip");// 圧縮
                                        File.Move("backuptmp\\" + subFolder.Name + "backupziptemp.zip", TargetBackupPath + "\\" + TargetProjectName + "_backup-" + DT.ToString("yyyy年MM月dd日HH時mm分ss秒") + "\\" + subFolder.Name + "_backup-" + DT.ToString("yyyy-MM-dd-HH-mm-ss") + ".zip");// 移動
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
                                MessageBox.Show("バックアップ作成に失敗しました。\n" + ex.Message, "CREC");
                                BackupToolStripMenuItem.Text = "バックアップ作成";
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
                if (CloseListOutput == true)
                {
                    if (ListOutputFormat == "CSV")
                    {
                        CSVListOutputMethod();
                    }
                    else if (ListOutputFormat == "TSV")
                    {
                        TSVListOutputMethod();
                    }
                    else
                    {
                        MessageBox.Show("値が不正です。", "CREC");
                    }
                }
                if (CloseBackUp == true)// 自動バックアップ
                {
                    BackUpMethod();
                    this.Hide();// メインフォームを消す
                    CloseBackUpForm closeBackUpForm = new CloseBackUpForm(ColorSetting);
                    Task.Run(() => { closeBackUpForm.ShowDialog(); });// 別プロセスでバックアップ中のプログレスバー表示ウインドウを開く
                    // バックアップ作成
                    DateTime DT = DateTime.Now;
                    if (CompressType == 0)
                    {
                        try
                        {
                            ZipFile.CreateFromDirectory(TargetBackupPath + "\\backuptmp", TargetBackupPath + "\\" + TargetProjectName + "_backup-" + DT.ToString("yyyy年MM月dd日HH時mm分ss秒") + ".zip");

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("バックアップ作成に失敗しました。\n" + ex.Message, "CREC");
                            BackupToolStripMenuItem.Text = "バックアップ作成";
                            BackupToolStripMenuItem.Enabled = true;
                            return;
                        }
                        Directory.Delete(TargetBackupPath + "\\backuptmp", true);
                    }
                    else if (CompressType == 1)
                    {
                        // バックアップ用フォルダ作成
                        Directory.CreateDirectory(TargetBackupPath + "\\" + TargetProjectName + "_backup-" + DT.ToString("yyyy年MM月dd日HH時mm分ss秒"));
                        File.Copy(TargetCRECPath, TargetBackupPath + "\\" + TargetProjectName + "_backup-" + DT.ToString("yyyy年MM月dd日HH時mm分ss秒") + "\\backup.crec", true);// crecファイルをバックアップ
                        try
                        {
                            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(TargetFolderPath);
                            IEnumerable<System.IO.DirectoryInfo> subFolders = di.EnumerateDirectories("*");
                            foreach (System.IO.DirectoryInfo subFolder in subFolders)
                            {
                                try
                                {
                                    FileSystem.CopyDirectory(subFolder.FullName, "backuptmp\\" + subFolder.Name + "\\datatemp", Microsoft.VisualBasic.FileIO.UIOption.AllDialogs, Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing);
                                    ZipFile.CreateFromDirectory("backuptmp\\" + subFolder.Name + "\\datatemp", "backuptmp\\" + subFolder.Name + "backupziptemp.zip");// 圧縮
                                    File.Move("backuptmp\\" + subFolder.Name + "backupziptemp.zip", TargetBackupPath + "\\" + TargetProjectName + "_backup-" + DT.ToString("yyyy年MM月dd日HH時mm分ss秒") + "\\" + subFolder.Name + "_backup-" + DT.ToString("yyyy-MM-dd-HH-mm-ss") + ".zip");// 移動
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
                            MessageBox.Show("バックアップ作成に失敗しました。\n" + ex.Message, "CREC");
                            BackupToolStripMenuItem.Text = "バックアップ作成";
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
        private void DeleteContentToolStripMenuItem_Click(object sender, EventArgs e)// データ完全削除
        {
            if (TargetContentsPath.Length == 0)
            {
                MessageBox.Show("プロジェクトを開いてください。", "CREC");
                return;
            }
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(ThisName + "を削除しますか？\nこの操作は取り消せません。", "CREC", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
            if (result == System.Windows.MessageBoxResult.Yes)// 削除を実行
            {
                DeleteContent();
            }
        }
        private void ReissueUUIDToolStripMenuItem_Click(object sender, EventArgs e)// UUID再割当て
        {
            EditIDTextBox.Text = Convert.ToString(Guid.NewGuid());// UUIDを入力
        }
        private void ForceEditRequestToolStripMenuItem_Click(object sender, EventArgs e)// 編集権限強制取得
        {
            if (AllowEdit == false)
            {
                MessageBox.Show("設定により編集が禁止されています。", "CREC");
            }
            else if (File.Exists(TargetContentsPath + "\\DED"))
            {
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("他の端末で編集中の可能性があります。\n編集権限を強制的に取得しますか？\nなお、本操作によりデータ破損する可能性があります。", "CREC", System.Windows.MessageBoxButton.YesNo);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    // 編集リクエストタグを削除
                    File.Delete(TargetContentsPath + "\\RED");
                    File.Delete(TargetContentsPath + "\\DED");
                    File.Delete(TargetContentsPath + "\\ADD");
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (TargetContentsPath.Length == 0)
                {
                    MessageBox.Show("編集するデータを選択し、詳細表示してください。", "CREC");
                    return;
                }
                StartEditForm();
                // 現時点でのデータを読み込んで表示
                LoadDetails();
                EditNameTextBox.Text = ThisName;
                EditIDTextBox.TextChanged -= IDTextBox_TextChanged;// ID重複確認イベントを停止
                EditIDTextBox.Text = ThisID;
                EditIDTextBox.TextChanged += IDTextBox_TextChanged;// ID重複確認イベントを開始
                EditMCTextBox.Text = ThisMC;
                EditRegistrationDateTextBox.Text = ThisRegistrationDate;
                EditCategoryTextBox.Text = ThisCategory;
                EditTag1TextBox.Text = ThisTag1;
                EditTag2TextBox.Text = ThisTag2;
                EditTag3TextBox.Text = ThisTag3;
                EditRealLocationTextBox.Text = ThisRealLocation;
            }
        }
        private void EditProjectToolStripMenuItem_Click(object sender, EventArgs e)// プロジェクト管理ファイルの編集
        {
            if (TargetCRECPath.Length == 0)
            {
                MessageBox.Show("先にプロジェクトを開いてください。", "CREC");
                return;
            }
            else
            {
                if (SaveAndCloseEditButton.Visible == true)// 編集中の場合は警告を表示
                {
                    if (CheckEditingContents() == true)
                    {
                        MakeNewProject makenewproject = new MakeNewProject(TargetCRECPath, ColorSetting);
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
                    MakeNewProject makenewproject = new MakeNewProject(TargetCRECPath, ColorSetting);
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
                MessageBox.Show("先にプロジェクトを開いてください。", "CREC");
                return;
            }
            else
            {
                ProjectInfoForm projectInfoForm = new ProjectInfoForm(TargetCRECPath, ColorSetting);
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
                CurrentSelectedContentsID = "";
            }
            else
            {
                try
                {
                    CurrentSelectedContentsID = Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value);
                }
                catch (Exception ex)
                {
                    CurrentSelectedContentsID = "";
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
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(TargetFolderPath);
                try
                {
                    IEnumerable<System.IO.DirectoryInfo> subFolders = di.EnumerateDirectories("*");
                    foreach (System.IO.DirectoryInfo subFolder in subFolders)
                    {
                        NoData = false;
                        if (DataLoadingStatus == "stop")
                        {
                            DataLoadingStatus = "false";
                            break;
                        }
                        // 変数初期化「List読み込み内でのみ使用すること」
                        string ListThisName = "";
                        string ListThisID = "";
                        string ListThisMC = "";
                        string ListThisCategory = "";
                        string ListThisTag1 = "";
                        string ListThisTag2 = "";
                        string ListThisTag3 = "";
                        string ListRegistrationDate = "";
                        string ListInventory = "";
                        string ListInventoryStatus = "";
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
                                        ListThisName = line.Substring(3);
                                        break;
                                    case "ID":
                                        ListThisID = line.Substring(3);
                                        break;
                                    case "MC":
                                        ListThisMC = line.Substring(3);
                                        break;
                                    case "登録日":
                                        ListRegistrationDate = line.Substring(4);
                                        break;
                                    case "カテゴリ":
                                        ListThisCategory = line.Substring(5);
                                        break;
                                    case "タグ1":
                                        ListThisTag1 = line.Substring(4);
                                        break;
                                    case "タグ2":
                                        ListThisTag2 = line.Substring(4);
                                        break;
                                    case "タグ3":
                                        ListThisTag3 = line.Substring(4);
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Indexファイルが破損しています。\n" + ex.Message, "CREC");
                            ListThisID = subFolder.Name;
                            ListThisName = "Status：Indexファイル破損";
                            ListThisCategory = "　ー　";
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
                                    ContentsDataTable.Rows.Add(subFolder.FullName, ListThisID, ListThisMC, ListThisName, ListRegistrationDate, ListThisCategory, ListThisTag1, ListThisTag2, ListThisTag3, ListInventory, ListInventoryStatus);
                                }
                            }
                            else
                            {
                                ContentsDataTable.Rows.Add(subFolder.FullName, ListThisID, ListThisMC, ListThisName, ListRegistrationDate, ListThisCategory, ListThisTag1, ListThisTag2, ListThisTag3, ListInventory, ListInventoryStatus);
                            }
                        }
                        else if (SearchFormTextBox.TextLength >= 1)
                        {
                            // SearchOptionの検索方法を追加しておいて
                            switch (SearchOptionComboBox.SelectedIndex)
                            {
                                case 0:
                                    if (SearchMethod(ListThisID) == true)
                                    {
                                        ContentsDataTable.Rows.Add(subFolder.FullName, ListThisID, ListThisMC, ListThisName, ListRegistrationDate, ListThisCategory, ListThisTag1, ListThisTag2, ListThisTag3, ListInventory, ListInventoryStatus);
                                    }
                                    break;
                                case 1:
                                    if (SearchMethod(ListThisMC) == true)
                                    {
                                        ContentsDataTable.Rows.Add(subFolder.FullName, ListThisID, ListThisMC, ListThisName, ListRegistrationDate, ListThisCategory, ListThisTag1, ListThisTag2, ListThisTag3, ListInventory, ListInventoryStatus);
                                    }
                                    break;
                                case 2:
                                    if (SearchMethod(ListThisName) == true)
                                    {
                                        ContentsDataTable.Rows.Add(subFolder.FullName, ListThisID, ListThisMC, ListThisName, ListRegistrationDate, ListThisCategory, ListThisTag1, ListThisTag2, ListThisTag3, ListInventory, ListInventoryStatus);
                                    }
                                    break;
                                case 3:
                                    if (SearchMethod(ListThisCategory) == true)
                                    {
                                        ContentsDataTable.Rows.Add(subFolder.FullName, ListThisID, ListThisMC, ListThisName, ListRegistrationDate, ListThisCategory, ListThisTag1, ListThisTag2, ListThisTag3, ListInventory, ListInventoryStatus);
                                    }
                                    break;
                                case 4:
                                    if (SearchMethod(ListThisTag1) == true)
                                    {
                                        ContentsDataTable.Rows.Add(subFolder.FullName, ListThisID, ListThisMC, ListThisName, ListRegistrationDate, ListThisCategory, ListThisTag1, ListThisTag2, ListThisTag3, ListInventory, ListInventoryStatus);
                                    }
                                    break;
                                case 5:
                                    if (SearchMethod(ListThisTag2) == true)
                                    {
                                        ContentsDataTable.Rows.Add(subFolder.FullName, ListThisID, ListThisMC, ListThisName, ListRegistrationDate, ListThisCategory, ListThisTag1, ListThisTag2, ListThisTag3, ListInventory, ListInventoryStatus);
                                    }
                                    break;
                                case 6:
                                    if (SearchMethod(ListThisTag3) == true)
                                    {
                                        ContentsDataTable.Rows.Add(subFolder.FullName, ListThisID, ListThisMC, ListThisName, ListRegistrationDate, ListThisCategory, ListThisTag1, ListThisTag2, ListThisTag3, ListInventory, ListInventoryStatus);
                                    }
                                    break;
                                case 7:
                                    if (SearchMethod(ListInventoryStatus) == true)
                                    {
                                        ContentsDataTable.Rows.Add(subFolder.FullName, ListThisID, ListThisMC, ListThisName, ListRegistrationDate, ListThisCategory, ListThisTag1, ListThisTag2, ListThisTag3, ListInventory, ListInventoryStatus);
                                    }
                                    break;
                            }
                        }
                        // 更新前に選択されていたデータの行番号を取得
                        if (CurrentSelectedContentsID.Length != 0)
                        {
                            if (CurrentSelectedContentsID == ListThisID)
                            {
                                dataGridView1.ClearSelection();
                                CurrentSelectedContentsRows = dataGridView1.Rows.Count;
                            }
                        }
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
                        dataGridView1.CurrentCell = dataGridView1.Rows[CurrentSelectedContentsRows - 1].Cells[dataGridView1.CurrentCell.ColumnIndex];
                    }
                    catch (Exception ex)
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
            TargetAccessedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            SaveSearchSettings();
            // 一応読み込み終了を宣言
            DataLoadingLabel.Visible = false;
            this.Cursor = Cursors.Default;
            DataLoadingStatus = "false";
            //CheckContentsList();
        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)// 詳細表示
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
        private void OpenDataLocation_Click(object sender, EventArgs e)// ファイルの場所を表示
        {
            try
            {
                System.Diagnostics.Process.Start(TargetContentsPath + "\\data");
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
                Clipboard.SetText(TargetContentsPath + "\\data");
            }
            catch (Exception ex)
            {
                MessageBox.Show("データのパスをクリップボードにコピーできません。\n" + ex.Message, "CREC");
            }
        }
        private void ShowConfidentialDataButton_Click(object sender, EventArgs e)// 機密情報表示・非表示
        {
            Size FormSize = Size;
            if (ShowConfidentialData == false)
            {
                MessageBox.Show("Access Denied.", "CREC");
            }
            else if (ShowConfidentialData == true)
            {
                if (ConfidentialDataTextBox.Visible == false)
                {
                    if (TargetContentsPath.Length == 0)
                    {
                        MessageBox.Show("表示するデータを選択し、詳細表示してください。", "CREC");
                        return;
                    }

                    // フォーム内で機密情報を表示
                    DetailsTextBox.Visible = false;
                    ConfidentialDataTextBox.Visible = true;
                    ShowConfidentialDataButton.Text = "機密情報非表示";
                    DetailsLabel.Text = "機密情報";
                }
                else if (ConfidentialDataTextBox.Visible == true)
                {
                    DetailsTextBox.Visible = true;
                    ConfidentialDataTextBox.Visible = false;
                    ShowConfidentialDataButton.Text = "機密情報表示";
                    DetailsLabel.Text = "詳細情報";
                    SetFormLayout();
                }
            }
        }
        private bool CheckEditingContents()// 編集中に別のデータを開こうとした場合、編集中データを保存するか確認
        {
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("編集中のデータがあります。保存しますか？\n保存されなかったデータは削除されます。", "CREC", System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Warning);
            if (result == System.Windows.MessageBoxResult.Yes)// 保存して編集画面を閉じる
            {
                // 入力内容を確認
                if (CheckContent() == false)
                {
                    return false;
                }
                // データ保存メソッドを呼び出し
                SaveContentsMethod();
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
                // 入力フォームをリセット
                ClearDetailsWindowMethod();
                // 通常画面で必要なものを表示
                EditButton.Visible = true;
                ShowTag1.Visible = true;
                ShowTag2.Visible = true;
                ShowTag3.Visible = true;
                // 通常画面用にラベルを変更
                ShowPicturesButton.Text = "画像を表示";
                // 詳細データおよび機密データを編集不可能に変更
                DetailsTextBox.ReadOnly = true;
                ConfidentialDataTextBox.ReadOnly = true;
                // ID手動設定を不可に変更
                EditIDTextBox.ReadOnly = true;
                AllowEditID = false;
                AllowEditIDButton.Text = "編集不可";
                ReissueUUIDToolStripMenuItem.Enabled = false;
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
                File.Delete(TargetContentsPath + "\\RED");
                File.Delete(TargetContentsPath + "\\DED");
                // サムネ画像が更新されていた場合は一時データをを削除
                if (File.Exists(TargetContentsPath + "\\pictures\\Thumbnail1.newjpg"))
                {
                    File.Delete(TargetContentsPath + "\\pictures\\Thumbnail1.newjpg");
                }
                if (File.Exists(TargetContentsPath + "\\ADD"))// データ追加時は削除
                {
                    DeleteContent();
                }
                else
                {
                    File.Delete(TargetContentsPath + "\\ADD");
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
                // 入力フォームをリセット
                ClearDetailsWindowMethod();
                // 通常画面で必要なものを表示
                EditButton.Visible = true;
                ShowTag1.Visible = true;
                ShowTag2.Visible = true;
                ShowTag3.Visible = true;
                // 通常画面用にラベルを変更
                ShowPicturesButton.Text = "画像を表示";
                // 詳細データおよび機密データを編集不可能に変更
                DetailsTextBox.ReadOnly = true;
                ConfidentialDataTextBox.ReadOnly = true;
                // ID手動設定を不可に変更
                EditIDTextBox.ReadOnly = true;
                AllowEditID = false;
                AllowEditIDButton.Text = "編集不可";
                ReissueUUIDToolStripMenuItem.Enabled = false;
                if (DataLoadingStatus == "true")
                {
                    DataLoadingStatus = "stop";
                }
                LoadGrid();
                return true;
            }
            else if (result == System.Windows.MessageBoxResult.Cancel)// 何もしない
            {
                return false;
            }
            else// 仮置き
            {
                return false;
            }
        }
        private void ShowDetails()// 詳細情報の表示
        {
            // 読み込み中の画面に切り替え
            NoImageLabel.Text = "Loading";
            NoImageLabel.Visible = true;
            Thumbnail.Image = null;
            Thumbnail.BackColor = menuStrip1.BackColor;
            Application.DoEvents();
            LoadDetails();
            // 表示・非表示項目の設定
            AllowEditIDButton.Visible = false;
            CheckSameMCButton.Visible = false;
            ConfidentialDataTextBox.Visible = false;
            DetailsTextBox.Visible = true;
            if (ShowObjectNameLabelVisible == false)
            {
                ShowObjectName.Visible = false;
            }
            else
            {
                ShowObjectName.Visible = true;
            }
            if (ShowIDLabelVisible == false)
            {
                ShowID.Visible = false;
            }
            else
            {
                ShowID.Visible = true;
            }
            if (ShowMCLabelVisible == false)
            {
                ShowMC.Visible = false;
            }
            else
            {
                ShowMC.Visible = true;
            }
            if (ShowRegistrationDateLabelVisible == false)
            {
                ShowRegistrationDate.Visible = false;
            }
            else
            {
                ShowRegistrationDate.Visible = true;
            }
            if (ShowCategoryLabelVisible == false)
            {
                ShowCategory.Visible = false;
            }
            else
            {
                ShowCategory.Visible = true;
            }
            if (ShowTag1NameVisible == false)
            {
                ShowTag1.Visible = false;
            }
            else
            {
                ShowTag1.Visible = true;
            }
            if (ShowTag2NameVisible == false)
            {
                ShowTag2.Visible = false;
            }
            else
            {
                ShowTag2.Visible = true;
            }
            if (ShowTag3NameVisible == false)
            {
                ShowTag3.Visible = false;
            }
            else
            {
                ShowTag3.Visible = true;
            }
            if (ShowRealLocationLabelVisible == false)
            {
                ShowRealLocation.Visible = false;
            }
            else
            {
                ShowRealLocation.Visible = true;
            }
            if (ShowDataLocationLabelVisible == false)
            {
                ShowDataLocation.Visible = false;
            }
            else
            {
                ShowDataLocation.Visible = true;
            }

            if (File.Exists(TargetContentsPath + "\\inventory.inv"))// 在庫数管理モードの表示・非表示
            {
                InventoryManagementModeButton.Visible = true;
            }
            else
            {
                InventoryManagementModeButton.Visible = false;
            }
            DetailsLabel.Text = "詳細情報";
            ShowConfidentialDataButton.Text = "機密情報表示";
            ObjectNameLabel.Text = ShowObjectNameLabel + "：";
            ShowObjectName.Text = ThisName;
            IDLabel.Text = ShowIDLabel + "：";
            ShowID.Text = ThisID;
            MCLabel.Text = ShowMCLabel + "：";
            ShowMC.Text = ThisMC;
            RegistrationDateLabel.Text = ShowRegistrationDateLabel + "：";
            ShowRegistrationDate.Text = ThisRegistrationDate;
            CategoryLabel.Text = ShowCategoryLabel + "：";
            ShowCategory.Text = ThisCategory;
            Tag1NameLabel.Text = Tag1Name + "：";
            ShowTag1.Text = ThisTag1;
            Tag2NameLabel.Text = Tag2Name + "：";
            ShowTag2.Text = ThisTag2;
            Tag3NameLabel.Text = Tag3Name + "：";
            ShowTag3.Text = ThisTag3;
            RealLocationLabel.Text = ShowRealLocationLabel + "：";
            ShowRealLocation.Text = ThisRealLocation;
            ShowDataLocation.Text = ShowDataLocationLabel + "：";
            Thumbnail.ImageLocation = (TargetContentsPath + "\\pictures\\Thumbnail1.jpg");
            TargetDetailsPath = (TargetContentsPath + "\\details.txt");
            if (System.IO.File.Exists(TargetContentsPath + "\\pictures\\Thumbnail1.jpg"))
            {
                NoImageLabel.Text = "NO IMAGE";
                NoImageLabel.Visible = false;
                Thumbnail.BackColor = this.BackColor;
            }
            else
            {
                NoImageLabel.Text = "NO IMAGE";
                NoImageLabel.Visible = true;
                Thumbnail.BackColor = menuStrip1.BackColor;
            }
            ShowPicturesButton.Visible = true;

            // 詳細情報読み込み＆表示
            StreamReader sr1 = null;
            try
            {
                sr1 = new StreamReader(TargetDetailsPath);
            }
            catch (Exception ex)
            {
                DetailsTextBox.Text = "No Data.";
            }
            finally
            {
                if (sr1 != null)
                {
                    DetailsTextBox.Text = sr1.ReadToEnd();
                    sr1.Close();
                }
            }
            // 機密情報を読み込み
            try
            {
                StreamReader sr2 = new StreamReader(TargetContentsPath + "\\confidentialdata.txt");
                ConfidentialDataTextBox.Text = sr2.ReadToEnd();
                sr2.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("データの読み込みに失敗しました\n" + ex.Message, "CREC");
            }
            // 編集ボタンの表示内容設定
            if (AllowEdit == false)
            {
                EditButton.Text = "編集禁止";
                EditButton.ForeColor = Color.Red;
            }
            else
            {
                if (File.Exists(TargetContentsPath + "\\DED"))
                {
                    EditButton.Text = "他端末編集中";
                    EditButton.ForeColor = Color.Blue;
                }
                else
                {
                    EditButton.Text = "編集";
                    EditButton.ForeColor = Color.Black;
                }
            }
            // 最近表示した項目としてUUID、名称を保存<- 未実装項目

        }
        private void LoadDetails()// 詳細情報を読み込み
        {
            if (dataGridView1.CurrentRow == null)
            {
                return;
            }
            try
            {
                TargetContentsPath = Convert.ToString(dataGridView1.CurrentRow.Cells[0].Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show("詳細情報の読み込みに失敗しました。\n" + ex.Message, "CREC");
                return;
            }

            if (TargetContentsPath.Length == 0)
            {
                MessageBox.Show("データが存在しません。", "CREC");
                return;
            }
            // 変数初期化
            ClearDetailsWindowMethod();
            // index読み込み
            TargetIndexPath = TargetContentsPath + "\\index.txt";
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
                            ThisName = line.Substring(3);
                            break;
                        case "ID":
                            ThisID = line.Substring(3);
                            break;
                        case "MC":
                            ThisMC = line.Substring(3);
                            break;
                        case "登録日":
                            ThisRegistrationDate = line.Substring(4);
                            break;
                        case "カテゴリ":
                            ThisCategory = line.Substring(5);
                            break;
                        case "タグ1":
                            ThisTag1 = line.Substring(4);
                            break;
                        case "タグ2":
                            ThisTag2 = line.Substring(4);
                            break;
                        case "タグ3":
                            ThisTag3 = line.Substring(4);
                            break;
                        case "場所1(Real)":
                            ThisRealLocation = cols[1];
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Indexファイルが破損しています。\n" + ex.Message, "CREC");
                ThisName = "　ー　";
                ThisID = Path.GetFileName(TargetContentsPath);
                ThisRegistrationDate = "　ー　";
            }
        }
        private void ClearDetailsWindowMethod()// 表示されている詳細情報・入力フォームの情報を全てリセットするメソッド
        {
            // 詳細表示の表示内容を初期化
            ThisName = "";
            ObjectNameLabel.Text = ShowObjectNameLabel + "：";
            ShowObjectName.Text = "";
            ThisID = "";
            IDLabel.Text = ShowIDLabel + "：";
            ShowID.Text = "";
            ThisMC = "";
            MCLabel.Text = ShowMCLabel + "：";
            ShowMC.Text = "";
            ThisRegistrationDate = "";
            RegistrationDateLabel.Text = ShowRegistrationDateLabel + "：";
            ShowRegistrationDate.Text = "";
            ThisCategory = "";
            CategoryLabel.Text = ShowCategoryLabel + "：";
            ShowCategory.Text = "";
            ThisTag1 = "";
            ShowTag1.Text = "";
            Tag1NameLabel.Text = Tag1Name + "：";
            ThisTag2 = "";
            ShowTag2.Text = "";
            Tag2NameLabel.Text = Tag2Name + "：";
            ThisTag3 = "";
            ShowTag3.Text = "";
            Tag3NameLabel.Text = Tag3Name + "：";
            ThisRealLocation = "";
            RealLocationLabel.Text = ShowRealLocationLabel + "：";
            ShowRealLocation.Text = "";
            ShowDataLocation.Text = ShowDataLocationLabel + "：";
            DetailsTextBox.Text = "";
            ConfidentialDataTextBox.Text = "";
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
        private void ShowPicturesButton_Click(object sender, EventArgs e)// 詳細画像表示・追加ボタン
        {
            if (ShowPicturesButton.Text == "画像を表示")
            {
                ShowPicturesMethod();
            }
            else if (ShowPicturesButton.Text == "画像保存場所")
            {
                try
                {
                    System.Diagnostics.Process.Start("EXPLORER.EXE", TargetContentsPath + "\\pictures");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("フォルダを開けませんでした\n" + ex.Message, "CREC");
                    return;
                }
            }
        }
        private void ShowPicturesMethod()// 詳細画像表示用のメソッド
        {
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(TargetContentsPath + "\\pictures");
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
            ShowPictureFileNameLabel.Text = "";
            ShowPictureFileNameLabel.Visible = false;
            PictureBox1.Image = null;// これやらないと次の物に切り替えた直後に前の物の画像が一瞬表示される
            ClosePicturesButton.Visible = false;
            NextPictureButton.Visible = false;
            PreviousPictureButton.Visible = false;
            NoPicturesLabel.Visible = false;
            if (AutoSearch == false)
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
            if (AllowEdit == false)
            {
                MessageBox.Show("設定により編集が禁止されています。", "CREC");
                return;
            }
            else if (File.Exists(TargetContentsPath + "\\DED"))
            {
                if (File.Exists(TargetContentsPath + "\\RED"))
                {
                    MessageBox.Show("編集中、編集待機中の端末があります。\n閲覧のみ可能です。", "CREC");
                    return;
                }
                System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("他の端末で編集中のため、ロックされています。\n編集権限をリクエストしますか？", "CREC", System.Windows.MessageBoxButton.YesNo);
                if (result == System.Windows.MessageBoxResult.Yes)
                {
                    // 編集リクエストタグを作成
                    FileStream FileStream = File.Create(TargetContentsPath + "\\RED");
                    FileStream.Close();
                    EditButton.Visible = false;
                    EditRequestingButton.Visible = true;
                    AwaitEdit();
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (TargetContentsPath.Length == 0)
                {
                    MessageBox.Show("編集するデータを選択し、詳細表示してください。", "CREC");
                    return;
                }
                StartEditForm();
                // 詳細情報読み込み＆表示
                StreamReader sr1 = null;
                try
                {
                    sr1 = new StreamReader(TargetDetailsPath);
                }
                catch (Exception ex)
                {
                    DetailsTextBox.Text = "No Data.";
                }
                finally
                {
                    if (sr1 != null)
                    {
                        DetailsTextBox.Text = sr1.ReadToEnd();
                        sr1.Close();
                    }
                }
                // 機密情報を読み込み
                try
                {
                    StreamReader sr2 = new StreamReader(TargetContentsPath + "\\confidentialdata.txt");
                    ConfidentialDataTextBox.Text = sr2.ReadToEnd();
                    sr2.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("データの読み込みに失敗しました\n" + ex.Message, "CREC");
                }
                EditNameTextBox.Text = ThisName;
                EditIDTextBox.TextChanged -= IDTextBox_TextChanged;// ID重複確認イベントを停止
                EditIDTextBox.Text = ThisID;
                EditIDTextBox.TextChanged += IDTextBox_TextChanged;// ID重複確認イベントを開始
                AllowEditIDButton.Text = "編集不可";
                ReissueUUIDToolStripMenuItem.Enabled = false;
                EditMCTextBox.Text = ThisMC;
                EditRegistrationDateTextBox.Text = ThisRegistrationDate;
                EditCategoryTextBox.Text = ThisCategory;
                EditTag1TextBox.Text = ThisTag1;
                EditTag2TextBox.Text = ThisTag2;
                EditTag3TextBox.Text = ThisTag3;
                EditRealLocationTextBox.Text = ThisRealLocation;
            }
        }
        private void StartEditForm()// 編集画面に切り替え
        {
            // 編集中タグを作成
            FileStream FileStream = File.Create(TargetContentsPath + "\\DED");
            FileStream.Close();
            AwaitEditRequest();// 編集リクエスト待機非同期処理を開始
            // サムネ用画像変更用データが残っていた場合削除
            if (File.Exists(TargetContentsPath + "\\pictures\\Thumbnail1.newjpg"))
            {
                File.Delete(TargetContentsPath + "\\pictures\\Thumbnail1.newjpg");
            }
            // 編集画面に必要な物を表示
            if (ShowObjectNameLabelVisible == true)
            {
                EditNameTextBox.Visible = true;
            }
            if (ShowIDLabelVisible == true)
            {
                EditIDTextBox.Visible = true;
                AllowEditIDButton.Visible = true;
            }
            if (ShowMCLabelVisible == true)
            {
                EditMCTextBox.Visible = true;
                CheckSameMCButton.Visible = true;
            }
            if (ShowRegistrationDateLabelVisible == true)
            {
                EditRegistrationDateTextBox.Visible = true;
            }
            if (ShowCategoryLabelVisible == true)
            {
                EditCategoryTextBox.Visible = true;
            }
            if (ShowTag1NameVisible == true)
            {
                EditTag1TextBox.Visible = true;
            }
            if (ShowTag2NameVisible == true)
            {
                EditTag2TextBox.Visible = true;
            }
            if (ShowTag3NameVisible == true)
            {
                EditTag3TextBox.Visible = true;
            }
            if (ShowRealLocationLabelVisible == true)
            {
                EditRealLocationTextBox.Visible = true;
            }
            if (ShowDataLocationLabelVisible == true)
            {
            }
            SaveAndCloseEditButton.Visible = true;
            SelectThumbnailButton.Visible = true;
            ShowPicturesButton.Visible = true;
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

            // 各ラベルの表示内容を編集用に変更
            ShowPicturesButton.Text = "画像保存場所";
            AllowEditIDButton.Text = "編集不可";
            ReissueUUIDToolStripMenuItem.Enabled = false;
            // 詳細データおよび機密データを編集可能に変更
            DetailsTextBox.ReadOnly = false;
            ConfidentialDataTextBox.ReadOnly = false;
            // 一応ID重複確認イベントを開始
            EditIDTextBox.TextChanged += IDTextBox_TextChanged;
        }
        private void SelectThumbnailButton_Click(object sender, EventArgs e)// サムネイル選択
        {
            OpenFileDialog openFolderDialog = new OpenFileDialog();
            openFolderDialog.InitialDirectory = TargetContentsPath + "\\pictures";
            openFolderDialog.Title = "画像を選択してください";
            openFolderDialog.Filter = "|*.jpg;*.png";
            if (openFolderDialog.ShowDialog() == DialogResult.OK)// ファイル読み込み成功
            {
                Thumbnail.ImageLocation = (openFolderDialog.FileName);// この時点で選択されたサムネイルを仮表示（未保存状態）
                try
                {
                    File.Copy(openFolderDialog.FileName, TargetContentsPath + "\\pictures\\Thumbnail1.newjpg", true);
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
            // ボタンを無効化
            SaveAndCloseEditButton.Enabled = false;
            SaveAndCloseEditButton.Text = "保存中";
            SaveAndCloseEditButton.Update();
            // 入力内容を確認
            if (CheckContent() == false)
            {
                return;
            }
            // 通常画面用にラベルを変更
            ShowPicturesButton.Text = "画像を表示";
            // データ保存メソッドを呼び出し
            SaveContentsMethod();
            // ボタンを有効化
            SaveAndCloseEditButton.Enabled = true;
            SaveAndCloseEditButton.Text = "保存して終了";
            SaveAndCloseEditButton.Update();
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
            AllowEditID = false;
            AllowEditIDButton.Text = "編集不可";
            ReissueUUIDToolStripMenuItem.Enabled = false;
            // 再度詳細情報を表示
            if (DataLoadingStatus == "true")
            {
                DataLoadingStatus = "stop";
            }
            TargetModifiedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            LoadGrid();
            ShowDetails();
        }
        private void DeleteContent()// データ完全削除用のメソッド
        {
            if (TargetContentsPath.Length == 0)
            {
                MessageBox.Show("プロジェクトを開いてください。", "CREC");
                return;
            }
            try
            {
                Directory.Delete(TargetContentsPath, true);
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
                // 入力フォームをリセット
                ClearDetailsWindowMethod();
                // 通常画面で必要なものを表示
                EditButton.Visible = true;
                ShowTag1.Visible = true;
                ShowTag2.Visible = true;
                ShowTag3.Visible = true;
                // 通常画面用にラベルを変更
                ShowPicturesButton.Text = "画像を表示";
                // 詳細データおよび機密データを編集不可能に変更
                DetailsTextBox.ReadOnly = true;
                ConfidentialDataTextBox.ReadOnly = true;
                // ID手動設定を不可に変更
                EditIDTextBox.ReadOnly = true;
                AllowEditID = false;
                AllowEditIDButton.Text = "編集不可";
                ReissueUUIDToolStripMenuItem.Enabled = false;
                // 再度詳細情報を表示
                if (DataLoadingStatus == "true")
                {
                    DataLoadingStatus = "stop";
                }
            }
            SearchFormTextBox.Text = "";
            SearchOptionComboBox.SelectedIndex = 0;
            MessageBox.Show("削除成功", "CREC");
            TargetModifiedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            LoadGrid();
            ShowDetails();
        }
        private bool CheckContent()// 入力内容の整合性を確認
        {
            if (System.IO.Directory.Exists(TargetFolderPath + "\\" + EditIDTextBox.Text))// ID重複確認
            {
                if (TargetContentsPath != TargetFolderPath + "\\" + EditIDTextBox.Text)
                {
                    AllowEditIDButton.Text = "設定不可";
                    ReissueUUIDToolStripMenuItem.Enabled = false;
                    AllowEditIDButton.ForeColor = Color.Red;
                    MessageBox.Show("入力されたIDは使用済みです。", "CREC");
                    return false;
                }
            }
            return true;
        }
        private void AllowEditIDButton_Click(object sender, EventArgs e)// ID手動設定の可否
        {
            if (AllowEditID == false)
            {
                AllowEditID = true;
                AllowEditIDButton.Text = "編集可";
                EditIDTextBox.ReadOnly = false;
                ReissueUUIDToolStripMenuItem.Enabled = true;
            }
            else if (AllowEditID == true)
            {
                AllowEditID = false;
                AllowEditIDButton.Text = "編集不可";
                EditIDTextBox.ReadOnly = true;
                ReissueUUIDToolStripMenuItem.Enabled = false;
            }
        }
        private void IDTextBox_TextChanged(object sender, EventArgs e)// ID重複確認
        {
            if (ThisID == EditIDTextBox.Text)
            {
                AllowEditIDButton.Text = "変更なし";
                AllowEditIDButton.ForeColor = Color.Black;
            }
            else if (System.IO.Directory.Exists(TargetFolderPath + "\\" + EditIDTextBox.Text))
            {
                AllowEditIDButton.Text = "設定不可";
                AllowEditIDButton.ForeColor = Color.Red;
            }
            else
            {
                AllowEditIDButton.Text = "設定可";
                AllowEditIDButton.ForeColor = Color.Black;
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
            if (AutoSearch == true)
            {
                SearchOptionComboBox.SelectedIndexChanged += SearchOptionComboBox_SelectedIndexChanged;
                SearchMethodComboBox.SelectedIndexChanged += SearchMethodComboBox_SelectedIndexChanged;
                SearchFormTextBox.TextChanged += SearchFormTextBox_TextChanged;
            }
        }
        private void AddContentsMethod()// 新規にデータを追加する処理のメソッド
        {
            if (TargetFolderPath.Length == 0)
            {
                MessageBox.Show("プロジェクトを開いてください。", "CREC");
                return;
            }
            if (SaveAndCloseEditButton.Visible == true)// 編集中の場合は警告を表示
            {
                if (CheckEditingContents() == true)
                {

                }
                else
                {
                    return;
                }
            }
            dataGridView1.ClearSelection();//　List選択解除
            dataGridView1.CurrentCell = null;//　List選択解除
            ThisID = Convert.ToString(Guid.NewGuid());
            EditIDTextBox.Text = ThisID;// UUIDを入力
            DateTime DT = DateTime.Now;
            if(AutoMCFill == true)
            {
                EditMCTextBox.Text = DT.ToString("yyMMddHHmmssf");// MCを自動入力
            }
            EditRegistrationDateTextBox.Text = DT.ToString("yyyy/MM/dd_HH:mm:ss.f");// 日時を自動入力
            TargetContentsPath = TargetFolderPath + "\\" + EditIDTextBox.Text;
            // フォルダ及びファイルを作成
            Directory.CreateDirectory(TargetContentsPath);
            Directory.CreateDirectory(TargetContentsPath + "\\data");
            Directory.CreateDirectory(TargetContentsPath + "\\pictures");
            FileStream FileStream = File.Create(TargetContentsPath + "\\index.txt");
            StreamWriter streamWriter = new StreamWriter(FileStream);
            streamWriter.WriteLine(string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7},\n{8}", "名称," + EditNameTextBox.Text, "ID," + EditIDTextBox.Text, "MC," + EditMCTextBox.Text, "登録日," + EditRegistrationDateTextBox.Text, "カテゴリ," + EditCategoryTextBox.Text, "タグ1," + EditTag1TextBox.Text, "タグ2," + EditTag2TextBox.Text, "タグ3," + EditTag3TextBox.Text, "場所1(Real)," + EditRealLocationTextBox.Text));
            streamWriter.Close();
            FileStream.Close();
            FileStream = File.Create(TargetContentsPath + "\\details.txt");
            FileStream.Close();
            FileStream = File.Create(TargetContentsPath + "\\confidentialdata.txt");
            FileStream.Close();
            // 在庫管理を行うか確認
            DialogResult result = MessageBox.Show("在庫数管理を行いますか。", "CREC", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                FileStream = File.Create(TargetContentsPath + "\\inventory.inv");
                FileStream.Close();
                StreamWriter InventoryManagementFile = new StreamWriter(TargetContentsPath + "\\inventory.inv");
                InventoryManagementFile.WriteLine("{0},,,", EditIDTextBox.Text);
                InventoryManagementFile.Close();
                InventoryManagementModeButton.Visible = true;
            }
            else
            {
                InventoryManagementModeButton.Visible = false;
            }
            // 新規作成タグを作成
            FileStream = File.Create(TargetContentsPath + "\\ADD");
            FileStream.Close();
            // 表示中の内容をクリア
            DetailsTextBox.Text = "";
            ConfidentialDataTextBox.Text = "";
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
        private void SaveContentsMethod()// 入力されたデータを保存する処理のメソッド
        {
            // フォルダを作成
            Directory.CreateDirectory(TargetContentsPath);
            Directory.CreateDirectory(TargetContentsPath + "\\data");
            Directory.CreateDirectory(TargetContentsPath + "\\pictures");
            // 変更前のデータをバックアップ
            try
            {
                File.Copy(TargetContentsPath + "\\index.txt", TargetContentsPath + "\\index_old.txt", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Indexファイルのバックアップ作成に失敗しました。\n" + ex.Message, "CREC");
            }
            try
            {
                File.Copy(TargetContentsPath + "\\details.txt", TargetContentsPath + "\\details_old.txt", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("詳細データのバックアップ作成に失敗しました。\n" + ex.Message, "CREC");
            }
            try
            {
                File.Copy(TargetContentsPath + "\\confidentialdata.txt", TargetContentsPath + "\\confidentialdata_old.txt", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("機密データのバックアップ作成に失敗しました。\n" + ex.Message, "CREC");
            }
            // Indexデータを保存
            StreamWriter Indexfile = new StreamWriter(TargetContentsPath + "\\index.txt", false, Encoding.GetEncoding("UTF-8"));
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
                "名称," + EditNameTextBox.Text.Replace("\r", "").Replace("\n", ""),
                "ID," + EditIDTextBox.Text.Replace("\r", "").Replace("\n", ""),
                "MC," + EditMCTextBox.Text.Replace("\r", "").Replace("\n", ""),
                "登録日," + EditRegistrationDateTextBox.Text.Replace("\r", "").Replace("\n", ""),
                "カテゴリ," + EditCategoryTextBox.Text.Replace("\r", "").Replace("\n", ""),
                "タグ1," + EditTag1TextBox.Text.Replace("\r", "").Replace("\n", ""),
                "タグ2," + EditTag2TextBox.Text.Replace("\r", "").Replace("\n", ""),
                "タグ3," + EditTag3TextBox.Text.Replace("\r", "").Replace("\n", ""),
                "場所1(Real)," + EditRealLocationTextBox.Text.Replace("\r", "").Replace("\n", "")));
            Indexfile.Close();
            // 詳細データの保存
            StreamWriter Detailsfile = new StreamWriter(TargetContentsPath + "\\details.txt", false, Encoding.GetEncoding("UTF-8"));
            Detailsfile.Write(string.Format("{0}", DetailsTextBox.Text));
            Detailsfile.Close();
            // 機密データの保存
            StreamWriter ConfidentialDataFile = new StreamWriter(TargetContentsPath + "\\confidentialdata.txt", false, Encoding.GetEncoding("UTF-8"));
            ConfidentialDataFile.Write(string.Format("{0}", ConfidentialDataTextBox.Text));
            ConfidentialDataFile.Close();
            // サムネ画像が更新されていた場合は上書きしキャッシュを削除
            if (File.Exists(TargetContentsPath + "\\pictures\\Thumbnail1.newjpg"))
            {
                File.Copy(TargetContentsPath + "\\pictures\\Thumbnail1.newjpg", TargetContentsPath + "\\pictures\\Thumbnail1.jpg", true);
                File.Delete(TargetContentsPath + "\\pictures\\Thumbnail1.newjpg");
            }
            if (PictureBox1.Visible == true)// 詳細画像表示されている場合は読み込み
            {
                ShowPicturesMethod();
            }
            // 編集中タグを削除
            File.Delete(TargetContentsPath + "\\DED");
            File.Delete(TargetContentsPath + "\\RED");
            File.Delete(TargetContentsPath + "\\ADD");
            // ID変更処理
            if (TargetContentsPath != TargetFolderPath + "\\" + EditIDTextBox.Text)
            {
                Directory.Move(@TargetContentsPath, @TargetFolderPath + "\\" + EditIDTextBox.Text);
            }
            if (EditedListOutput == true)
            {
                if (ListOutputFormat == "CSV")
                {
                    CSVListOutputMethod();
                }
                else if (ListOutputFormat == "TSV")
                {
                    TSVListOutputMethod();
                }
                else
                {
                    MessageBox.Show("値が不正です。", "CREC");
                }
            }
            if (EditedBackUp == true)
            {
                BackUpMethod();
                MakeBackUpZip();// ZIP圧縮を非同期で開始
            }
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
        private void InventoryManagementModeButton_Click(object sender, EventArgs e)// 在庫管理モード切り替え
        {
            string reader;
            string row;
            string[] cols;
            if (InventoryModeDataGridView.Visible == false)// 在庫管理モードを開始
            {
                if (TargetContentsPath.Length == 0)
                {
                    MessageBox.Show("表示するデータを選択し、詳細表示してください。", "CREC");
                    return;
                }
                //invからデータを読み込んで表示
                try
                {
                    reader = File.ReadAllText(TargetContentsPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
                    rowsIM = reader.Trim().Replace("\r", "").Split('\n');
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
                SearchButton.Visible = false;
                // 必要なものを表示
                InventoryLabel.Visible = true;
                InventoryModeDataGridView.Visible = true;
                OperationOptionComboBox.Visible = true;
                EditQuantityTextBox.Visible = true;
                AddInventoryOperationButton.Visible = true;
                InventoryOperation.Visible = true;
                InputQuantitiy.Visible = true;
                InventoryOperationNote.Visible = true;
                EditInventoryOperationNoteTextBox.Visible = true;
                ProperInventorySettingsComboBox.Visible = true;
                ProperInventorySettingsTextBox.Visible = true;
                SaveProperInventorySettingsButton.Visible = true;
                ProperInventorySettingsNotificationLabel.Visible = true;
                InventoryManagementModeButton.Text = "在庫数管理終了";
                // DataGridView関係
                InventoryModeDataGridView.Rows.Clear();
                InventoryModeDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                InventoryModeDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
                // 行数を確認
                string[] tmp = File.ReadAllLines(TargetContentsPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
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
                    InventoryModeDataGridView.Rows.Add(cols[0], cols[1], cols[2], cols[3]);
                    inventory = inventory + Convert.ToInt32(cols[2]);
                    InventoryLabel.Text = Convert.ToString("在庫数：" + inventory);
                }
                if (inventory < 0)
                {
                    MessageBox.Show("在庫数がマイナスです。\n現在個数を確認してください", "CREC");
                }
                ProperInventorySettingsComboBox.SelectedIndex = 0;
                if (SafetyStock == null)
                {
                    ProperInventorySettingsTextBox.TextChanged -= ProperInventorySettingsTextBox_TextChanged;// 適正在庫管理の入力イベントを停止
                    ProperInventorySettingsTextBox.Text = "未定義";
                    ProperInventorySettingsTextBox.TextChanged += ProperInventorySettingsTextBox_TextChanged;// 適正在庫管理の入力イベントを再開
                }
                else
                {
                    ProperInventorySettingsTextBox.Text = Convert.ToString(SafetyStock);
                }
                ProperInventoryNotification();// 適正在庫設定と比較
            }
            else if (InventoryModeDataGridView.Visible == true)// 在庫管理モードを終了
            {
                CloseInventoryViewMethod();// 在庫管理モードを閉じるメソッドを呼び出し
                // 必要なものを表示
                if (StandardDisplayModeToolStripMenuItem.Checked)
                {
                    dataGridView1.Visible = true;
                    SearchFormTextBox.Visible = true;
                    SearchOptionComboBox.Visible = true;
                    SearchMethodComboBox.Visible = true;
                    SearchFormTextBoxClearButton.Visible = true;
                    AddContentsButton.Visible = true;
                    ListUpdateButton.Visible = true;
                    if(AutoSearch == false)
                    {
                        SearchButton.Visible = true;
                    }
                    if (DataLoadingStatus == "true")
                    {
                        DataLoadingStatus = "stop";
                    }
                    LoadGrid();
                }
                else if (FullDisplayModeToolStripMenuItem.Checked)// 全画面表示モードの時は写真を表示
                {
                    ShowPicturesMethod();
                }
            }
        }
        private void AddInventoryOperationButton_Click(object sender, EventArgs e)// 在庫の増減を保存
        {
            // 行数を確認
            string[] tmp = File.ReadAllLines(TargetContentsPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
            int error = 1;// 入力内容に不備がない場合は0に変更
            switch (OperationOptionComboBox.SelectedIndex)// 入力されたデータの整合性チェック
            {
                case 0:
                    if (Convert.ToInt32(EditQuantityTextBox.Text) > 0)
                    {
                        error = 0;
                    }
                    else
                    {
                        MessageBox.Show("入庫が選択されています。\n0より大きい値を入力してください", "CREC");
                    }
                    break;
                case 1:
                    if (Convert.ToInt32(EditQuantityTextBox.Text) < 0)
                    {
                        error = 0;
                    }
                    else
                    {
                        MessageBox.Show("出庫が選択されています\n0より小さい値を入力してください", "CREC");
                    }
                    break;
                case 2:
                    error = 0;
                    break;
            }
            if (error == 0)// エラーがなかった場合書き込み
            {
                StreamWriter sw = new StreamWriter(TargetContentsPath + "\\inventory.inv", false, Encoding.GetEncoding("UTF-8"));
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
                        sw.WriteLine("{0},{1},{2},{3}", DT.ToString("yyyy/MM/dd hh:mm:ss"), OperationOptionComboBox.Text, EditQuantityTextBox.Text, EditInventoryOperationNoteTextBox.Text);
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
                reader = File.ReadAllText(TargetContentsPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
                rowsIM = reader.Trim().Replace("\r", "").Split('\n');
                // 行数を確認
                tmp = File.ReadAllLines(TargetContentsPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
                // 1行目を読み込み
                row = rowsIM[0];
                inventory = 0;
                for (int i = 1; i <= Convert.ToInt32(tmp.Length) - 1; i++)
                {
                    row = rowsIM[i];
                    cols = row.Split(',');
                    InventoryModeDataGridView.Rows.Add(cols[0], cols[1], cols[2], cols[3]);
                    inventory = inventory + Convert.ToInt32(cols[2]);
                    InventoryLabel.Text = Convert.ToString("在庫数：" + inventory);
                }
                if (inventory < 0)
                {
                    MessageBox.Show("在庫数がマイナスです。\n現在個数を確認してください", "CREC");
                }
                TargetModifiedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                ProperInventoryNotification();// 適正在庫設定と比較
            }
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
                            ProperInventorySettingsTextBox.Text = "未定義";
                        }
                        else if (ProperInventorySettingsTextBox.ReadOnly == false)
                        {
                            ProperInventorySettingsTextBox.Text = "";
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
                            ProperInventorySettingsTextBox.Text = "未定義";
                        }
                        else if (ProperInventorySettingsTextBox.ReadOnly == false)
                        {
                            ProperInventorySettingsTextBox.Text = "";
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
                            ProperInventorySettingsTextBox.Text = "未定義";
                        }
                        else if (ProperInventorySettingsTextBox.ReadOnly == false)
                        {
                            ProperInventorySettingsTextBox.Text = "";
                        }
                    }
                    else
                    {
                        ProperInventorySettingsTextBox.Text = MaximumLevel.ToString();
                    }
                    break;
            }
            ProperInventorySettingsTextBox.TextChanged += ProperInventorySettingsTextBox_TextChanged;// 適正在庫管理の入力イベントを再開
            TargetModifiedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }
        private void SaveProperInventorySettingsButton_Click(object sender, EventArgs e)// 適正在庫の設定変更および保存
        {
            if (ProperInventorySettingsTextBox.ReadOnly == true)
            {
                ProperInventorySettingsTextBox.ReadOnly = false;
                SaveProperInventorySettingsButton.Text = "保存";
                if (ProperInventorySettingsTextBox.Text == "未定義")
                {
                    ProperInventorySettingsTextBox.Text = "";
                }
            }
            else if (ProperInventorySettingsTextBox.ReadOnly == false)
            {
                ProperInventorySettingsTextBox.ReadOnly = true;
                SaveProperInventorySettingsButton.Text = "変更";
                // 行数を確認
                string[] tmp = File.ReadAllLines(TargetContentsPath + "\\inventory.inv", Encoding.GetEncoding("UTF-8"));
                int error = 1;// 入力内容に不備がない場合は0に変更
                StreamWriter sw = new StreamWriter(TargetContentsPath + "\\inventory.inv", false, Encoding.GetEncoding("UTF-8"));
                for (int i = 0; i < Convert.ToInt32(tmp.Length); i++)
                {
                    if (i == 0)// .invのヘッダをコピー
                    {
                        string row;
                        row = rowsIM[i];
                        cols = row.Split(',');
                        sw.WriteLine("{0},{1},{2},{3}", cols[0], SafetyStock, ReorderPoint, MaximumLevel);
                    }
                    else// 既存のデータを順に書き込み
                    {
                        sw.WriteLine(rowsIM[i]);
                    }
                }
                sw.Close();
                ProperInventoryNotification();
            }
            TargetModifiedDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }
        private void ProperInventorySettingsTextBox_TextChanged(object sender, EventArgs e)// 入力された内容をリアルタイムで反映
        {
            switch (ProperInventorySettingsComboBox.SelectedIndex)// コンボボックスの選択内容に合わせて保存先を自動選択
            {
                case 0:
                    if (ProperInventorySettingsTextBox.Text != "")
                    {
                        SafetyStock = Convert.ToInt32(ProperInventorySettingsTextBox.Text);
                    }
                    break;
                case 1:
                    if (ProperInventorySettingsTextBox.Text != "")
                    {
                        ReorderPoint = Convert.ToInt32(ProperInventorySettingsTextBox.Text);
                    }
                    break;
                case 2:
                    if (ProperInventorySettingsTextBox.Text != "")
                    {
                        MaximumLevel = Convert.ToInt32(ProperInventorySettingsTextBox.Text);
                    }
                    break;
            }
        }
        private void ProperInventoryNotification()// 適正在庫設定と比較して通知を表示
        {
            ProperInventorySettingsNotificationLabel.Text = "";
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
        private void CloseInventoryViewMethod()// 在庫表示モードを閉じるメソッド
        {
            InventoryLabel.Visible = false;
            InventoryModeDataGridView.Visible = false;
            OperationOptionComboBox.Visible = false;
            EditQuantityTextBox.Visible = false;
            AddInventoryOperationButton.Visible = false;
            InventoryOperation.Visible = false;
            InputQuantitiy.Visible = false;
            InventoryOperationNote.Visible = false;
            EditInventoryOperationNoteTextBox.Visible = false;
            ProperInventorySettingsComboBox.Visible = false;
            ProperInventorySettingsTextBox.Visible = false;
            SaveProperInventorySettingsButton.Visible = false;
            ProperInventorySettingsNotificationLabel.Visible = false;
            // 表示内容・変数をリセット
            InventoryLabel.ResetText();
            ProperInventorySettingsTextBox.ResetText();
            SafetyStock = null;
            ReorderPoint = null;
            MaximumLevel = null;
            InventoryManagementModeButton.Text = "在庫数管理画面";
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
            if (TargetFolderPath.Length != 0)
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
                SearchMethodComboBox.Items.Add("前方一致");
                SearchMethodComboBox.Items.Add("部分一致");
                SearchMethodComboBox.Items.Add("後方一致");
                SearchMethodComboBox.Items.Add("完全一致");
            }
            SearchMethodComboBox.SelectedIndex = 0;
            SearchMethodComboBox.SelectedIndexChanged += SearchMethodComboBox_SelectedIndexChanged;
            if (SearchOptionComboBox.SelectedIndex == 7)
            {
                //ContentsDataTable.Rows.Clear();// DataGridViewのカラム情報以外を削除
                if (DataLoadingStatus == "true")
                {
                    DataLoadingStatus = "stop";
                }
                LoadGrid();// 再度読み込み
            }
            else if (TargetFolderPath.Length != 0)
            {
                //ContentsDataTable.Rows.Clear();// DataGridViewのカラム情報以外を削除
                if (DataLoadingStatus == "true")
                {
                    DataLoadingStatus = "stop";
                }
                LoadGrid();// 再度読み込み
            }
        }
        private void SearchMethodComboBox_SelectedIndexChanged(object sender, EventArgs e)// 検索メソッドが変更された場合に一覧を読み込んで更新
        {
            if (TargetFolderPath.Length != 0)
            {
                //ContentsDataTable.Rows.Clear();// DataGridViewのカラム情報以外を削除
                if (DataLoadingStatus == "true")
                {
                    DataLoadingStatus = "stop";
                }
                LoadGrid();// 再度読み込み
            }
        }
        private void SearchButton_Click(object sender, EventArgs e)// 検索ボタン、自動検索OFF時に使用
        {
            if (TargetFolderPath.Length != 0)
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
            if (AutoSearch == true)// 自動検索が有効な場合
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
        private void SaveSearchSettings()//現時点での検索対象、検索メソッドの内容、List表示内容をプロジェクトファイルに書き込み
        {
            try
            {
                if (TargetCRECPath.Length > 0)
                {
                    StreamWriter sw = new StreamWriter(TargetCRECPath, false, Encoding.GetEncoding("UTF-8"));
                    sw.WriteLine("{0},{1}", "projectname", TargetProjectName);
                    sw.WriteLine("{0},{1}", "projectlocation", TargetFolderPath);
                    sw.WriteLine("{0},{1}", "backuplocation", TargetBackupPath);
                    sw.Write("autobackup,");
                    if (StartUpBackUp == true)
                    {
                        sw.Write("S");
                    }
                    if (CloseBackUp == true)
                    {
                        sw.Write("C");
                    }
                    if (EditedBackUp == true)
                    {
                        sw.Write("E");
                    }
                    sw.Write('\n');
                    sw.WriteLine("CompressType,{0}", CompressType);
                    sw.WriteLine("{0},{1}", "Listoutputlocation", TargetListOutputPath);
                    sw.Write("autoListoutput,");
                    if (StartUpListOutput == true)
                    {
                        sw.Write("S");
                    }
                    if (CloseListOutput == true)
                    {
                        sw.Write("C");
                    }
                    if (EditedListOutput == true)
                    {
                        sw.Write("E");
                    }
                    sw.Write('\n');
                    sw.Write("openListafteroutput,");
                    if (OpenListAfterOutput == true)
                    {
                        sw.Write("O");
                    }
                    sw.Write('\n');
                    sw.WriteLine("ListOutputFormat,{0}", ListOutputFormat);
                    sw.WriteLine("{0},{1}", "created", TargetCreatedDate);
                    sw.WriteLine("{0},{1}", "modified", TargetModifiedDate);
                    sw.WriteLine("{0},{1}", "accessed", TargetAccessedDate);
                    if (ShowObjectNameLabelVisible == true)
                    {
                        sw.WriteLine("{0},{1},{2}", "ShowObjectNameLabel", ShowObjectNameLabel, "t");
                    }
                    else
                    {
                        sw.WriteLine("{0},{1},{2}", "ShowObjectNameLabel", ShowObjectNameLabel, "f");
                    }
                    if (ShowIDLabelVisible == true)
                    {
                        sw.WriteLine("{0},{1},{2}", "ShowIDLabel", ShowIDLabel, "t");
                    }
                    else
                    {
                        sw.WriteLine("{0},{1},{2}", "ShowIDLabel", ShowIDLabel, "f");
                    }
                    if (ShowMCLabelVisible == true)
                    {
                        sw.WriteLine("{0},{1},{2}", "ShowMCLabel", ShowMCLabel, "t");
                    }
                    else
                    {
                        sw.WriteLine("{0},{1},{2}", "ShowMCLabel", ShowMCLabel, "f");
                    }
                    if (ShowRegistrationDateLabelVisible == true)
                    {
                        sw.WriteLine("{0},{1},{2}", "ShowRegistrationDateLabel", ShowRegistrationDateLabel, "t");
                    }
                    else
                    {
                        sw.WriteLine("{0},{1},{2}", "ShowRegistrationDateLabel", ShowRegistrationDateLabel, "f");
                    }
                    if (AutoMCFill == true)
                    {
                        sw.Write("AutoMCFill,t\n");
                    }
                    else
                    {
                        sw.Write("AutoMCFill,f\n");
                    }
                    if (ShowCategoryLabelVisible == true)
                    {
                        sw.WriteLine("{0},{1},{2}", "ShowCategoryLabel", ShowCategoryLabel, "t");
                    }
                    else
                    {
                        sw.WriteLine("{0},{1},{2}", "ShowCategoryLabel", ShowCategoryLabel, "f");
                    }
                    if (ShowTag1NameVisible == true)
                    {
                        sw.WriteLine("{0},{1},{2}", "Tag1Name", Tag1Name, "t");
                    }
                    else
                    {
                        sw.WriteLine("{0},{1},{2}", "Tag1Name", Tag1Name, "f");
                    }
                    if (ShowTag2NameVisible == true)
                    {
                        sw.WriteLine("{0},{1},{2}", "Tag2Name", Tag2Name, "t");
                    }
                    else
                    {
                        sw.WriteLine("{0},{1},{2}", "Tag2Name", Tag2Name, "f");
                    }
                    if (ShowTag3NameVisible == true)
                    {
                        sw.WriteLine("{0},{1},{2}", "Tag3Name", Tag3Name, "t");
                    }
                    else
                    {
                        sw.WriteLine("{0},{1},{2}", "Tag3Name", Tag3Name, "f");
                    }
                    if (ShowRealLocationLabelVisible == true)
                    {
                        sw.WriteLine("{0},{1},{2}", "ShowRealLocationLabel", ShowRealLocationLabel, "t");
                    }
                    else
                    {
                        sw.WriteLine("{0},{1},{2}", "ShowRealLocationLabel", ShowRealLocationLabel, "f");
                    }
                    if (ShowDataLocationLabelVisible == true)
                    {
                        sw.WriteLine("{0},{1},{2}", "ShowDataLocationLabel", ShowDataLocationLabel, "t");
                    }
                    else
                    {
                        sw.WriteLine("{0},{1},{2}", "ShowDataLocationLabel", ShowDataLocationLabel, "f");
                    }
                    sw.WriteLine("{0},{1}", "SearchOptionNumber", SearchOptionComboBox.SelectedIndex.ToString());
                    sw.WriteLine("{0},{1}", "SearchMethodNumber", SearchMethodComboBox.SelectedIndex.ToString());
                    if (IDList.Visible == true)
                    {
                        sw.WriteLine("IDListVisible,true");
                    }
                    else if (IDList.Visible == false)
                    {
                        sw.WriteLine("IDListVisible,false");
                    }
                    if (MCList.Visible == true)
                    {
                        sw.WriteLine("MCListVisible,true");
                    }
                    else if (MCList.Visible == false)
                    {
                        sw.WriteLine("MCListVisible,false");
                    }
                    if (ObjectNameList.Visible == true)
                    {
                        sw.WriteLine("ObjectNameListVisible,true");
                    }
                    else if (ObjectNameList.Visible == false)
                    {
                        sw.WriteLine("ObjectNameListVisible,false");
                    }
                    if (RegistrationDateList.Visible == true)
                    {
                        sw.WriteLine("RegistrationDateListVisible,true");
                    }
                    else if (RegistrationDateList.Visible == false)
                    {
                        sw.WriteLine("RegistrationDateListVisible,false");
                    }
                    if (CategoryList.Visible == true)
                    {
                        sw.WriteLine("CategoryListVisible,true");
                    }
                    else if (CategoryList.Visible == false)
                    {
                        sw.WriteLine("CategoryListVisible,false");
                    }
                    if (Tag1List.Visible == true)
                    {
                        sw.WriteLine("Tag1ListVisible,true");
                    }
                    else if (Tag1List.Visible == false)
                    {
                        sw.WriteLine("Tag1ListVisible,false");
                    }
                    if (Tag2List.Visible == true)
                    {
                        sw.WriteLine("Tag2ListVisible,true");
                    }
                    else if (Tag2List.Visible == false)
                    {
                        sw.WriteLine("Tag2ListVisible,false");
                    }
                    if (Tag3List.Visible == true)
                    {
                        sw.WriteLine("Tag3ListVisible,true");
                    }
                    else if (Tag3List.Visible == false)
                    {
                        sw.WriteLine("Tag3ListVisible,false");
                    }
                    if (InventoryList.Visible == true)
                    {
                        sw.WriteLine("InventoryInformationListVisible,true");
                    }
                    else if (InventoryList.Visible == false)
                    {
                        sw.WriteLine("InventoryInformationListVisible,false");
                    }
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("プロジェクトファイルの更新に失敗しました。", "CREC");
            }
        }
        #endregion

        #region Config読み込み・保存
        private readonly string ConfigFile = "config.sys";
        private void ImportConfig()// config.sysの読み込み
        {
            if (File.Exists(ConfigFile))
            {
                // 指定されていなかった場合のために初期化
                AllowEdit = true;
                ShowConfidentialData = false;
                ShowUserAssistToolTips = true;
                OpenLastTimeProject = false;
                AutoSearch = true;
                RecentShownContents = false;
                BootUpdateCheck = false;
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
                                AllowEdit = true;
                            }
                            else
                            {
                                AllowEdit = false;
                            }
                            break;
                        case "ShowConfidentialData":
                            if (cols[1] == "true")
                            {
                                ShowConfidentialData = true;
                            }
                            else
                            {
                                ShowConfidentialData = false;
                            }
                            break;
                        case "ShowUserAssistToolTips":
                            if (cols[1] == "true")
                            {
                                ShowUserAssistToolTips = true;
                                SetUserAssistToolTips();
                            }
                            else
                            {
                                ShowUserAssistToolTips = false;
                                SetUserAssistToolTips();
                            }
                            break;
                        case "AutoLoadProject":
                            if (File.Exists(cols[1]))
                            {
                                if (TargetCRECPath.Length == 0)
                                {
                                    TargetCRECPath = cols[1];//CREC起動時のみ読み込み
                                }
                                AutoLoadProjectPath = cols[1];
                            }
                            else if (cols[1].Length == 0)
                            {
                                AutoLoadProjectPath = "";
                            }
                            else
                            {
                                MessageBox.Show("自動読み込み設定されたプロジェクトが見つかりません。", "CREC");
                                TargetCRECPath = "";
                                AutoLoadProjectPath = "";
                            }
                            break;
                        case "OpenLastTimeProject":
                            if (cols[1] == "true")
                            {
                                OpenLastTimeProject = true;
                            }
                            else
                            {
                                OpenLastTimeProject = false;
                            }
                            break;
                        case "AutoSearch":
                            if (cols[1] == "true")
                            {
                                SearchButton.Visible = false;
                                AutoSearch = true;
                                SearchFormTextBox.TextChanged += SearchFormTextBox_TextChanged;
                                SearchOptionComboBox.SelectedIndexChanged += SearchOptionComboBox_SelectedIndexChanged;
                                SearchMethodComboBox.SelectedIndexChanged += SearchMethodComboBox_SelectedIndexChanged;
                            }
                            else
                            {
                                SearchButton.Visible = true;
                                AutoSearch = false;
                                SearchFormTextBox.TextChanged -= SearchFormTextBox_TextChanged;
                                SearchOptionComboBox.SelectedIndexChanged -= SearchOptionComboBox_SelectedIndexChanged;
                                SearchMethodComboBox.SelectedIndexChanged -= SearchMethodComboBox_SelectedIndexChanged;
                            }
                            break;
                        case "RecentShownContents":
                            if (cols[1] == "true")
                            {
                                RecentShownContents = true;
                            }
                            else
                            {
                                RecentShownContents = false;
                            }
                            break;
                        case "BootUpdateCheck":
                            if (cols[1] == "true")
                            {
                                BootUpdateCheck = true;
                            }
                            else
                            {
                                BootUpdateCheck = false;
                            }
                            break;
                        case "ColorSetting":
                            if (cols[1].Length == 0)
                            {
                                ColorSetting = "Blue";
                            }
                            else
                            {
                                ColorSetting = cols[1];
                            }
                            break;
                    }
                }
            }
            else
            {
                MessageBox.Show("設定ファイルが見つかりません。\nデフォルト設定で起動します。", "CREC");
                //Config.sysの作成
                var TargetFile = Directory.GetCurrentDirectory() + "\\config.sys";
                try
                {
                    StreamWriter sw = File.CreateText(TargetFile);
                    sw.WriteLine("AllowEdit,true");
                    sw.WriteLine("ShowConfidentialData,false");
                    sw.WriteLine("ShowUserAssistToolTips,true");
                    sw.WriteLine("AutoLoadProject,");
                    sw.WriteLine("OpenLastTimeProject,false");
                    sw.WriteLine("AutoSearch,true");
                    sw.WriteLine("RecentShownContents,true");
                    sw.WriteLine("BootUpdateCheck,true");
                    sw.WriteLine("ColorSetting,Blue");
                    sw.Close();
                    AllowEdit = true;
                    ShowConfidentialData = false;
                    ShowUserAssistToolTips = true;
                    OpenLastTimeProject = false;
                    AutoSearch = true;
                    RecentShownContents = false;
                    BootUpdateCheck = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Config.sysの作成に失敗しました(´・ω・｀)\n" + ex.Message, "CREC");
                }
            }
        }
        private void SaveConfig()// config.sysの保存
        {
            StreamWriter configfile = new StreamWriter("config.sys", false, Encoding.GetEncoding("UTF-8"));
            if (AllowEdit == true)
            {
                configfile.WriteLine("AllowEdit,true");
            }
            else
            {
                configfile.WriteLine("AllowEdit,false");
            }
            if (ShowConfidentialData == true)
            {
                configfile.WriteLine("ShowConfidentialData,true");
            }
            else
            {
                configfile.WriteLine("ShowConfidentialData,false");
            }
            if (ShowUserAssistToolTips == true)
            {
                configfile.WriteLine("ShowUserAssistToolTips,true");
            }
            else
            {
                configfile.WriteLine("ShowUserAssistToolTips,false");
            }
            if (OpenLastTimeProject == true)
            {
                configfile.WriteLine("AutoLoadProject,{0}", TargetCRECPath);
                configfile.WriteLine("OpenLastTimeProject,true");
            }
            else
            {
                configfile.WriteLine("AutoLoadProject,{0}", AutoLoadProjectPath);
                configfile.WriteLine("OpenLastTimeProject,false");
            }
            if (AutoSearch == true)
            {
                configfile.WriteLine("AutoSearch,true");
            }
            else
            {
                configfile.WriteLine("AutoSearch,false");
            }
            if (RecentShownContents == true)
            {
                configfile.WriteLine("RecentShownContents,true");
            }
            else
            {
                configfile.WriteLine("RecentShownContents,false");
            }
            if (BootUpdateCheck == true)
            {
                configfile.WriteLine("BootUpdateCheck,true");
            }
            else
            {
                configfile.WriteLine("BootUpdateCheck,false");
            }
            configfile.WriteLine("ColorSetting,{0}", ColorSetting);
            configfile.Close();
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
                ShowSelectedItemInformationButton.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + 10 * DpiScale), SearchOptionComboBox.Location.Y);
                ShowSelectedItemInformationButton.Font = new Font(fontname, mainfontsize);
            }
            else if (FullDisplayModeToolStripMenuItem.Checked)
            {
                dataGridView1.Width = Convert.ToInt32(FormSize.Width - 40 * DpiScale);
                dataGridView1BackgroundPictureBox.Width = Convert.ToInt32(FormSize.Width);
                dataGridView1BackgroundPictureBox.Height = Convert.ToInt32(FormSize.Height - 35 * DpiScale);
                dataGridView1BackgroundPictureBox.Location = new System.Drawing.Point(0, Convert.ToInt32(35 * DpiScale));
                ShowSelectedItemInformationButton.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 + 10 * DpiScale), SearchOptionComboBox.Location.Y);
                ShowSelectedItemInformationButton.Font = new Font(fontname, mainfontsize);
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
            SearchOptionComboBox.Font = new Font(fontname, mainfontsize);
            SearchOptionComboBox.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 - 320 * DpiScale), SearchOptionComboBox.Location.Y);
            PictureBox1.Width = Convert.ToInt32(FormSize.Width * 0.5 - 20 * DpiScale);
            PictureBox1.Height = Convert.ToInt32(FormSize.Height - 200 * DpiScale);
            PictureBox1.Location = new Point(Convert.ToInt32(10 * DpiScale), Convert.ToInt32(70 * DpiScale));
            ShowPictureFileNameLabel.Font = new Font(fontname, mainfontsize);
            ShowPictureFileNameLabel.Location = new Point(PictureBox1.Location.X, Convert.ToInt32(30 * DpiScale));
            NoPicturesLabel.Font = new Font(fontname, mainfontsize);
            NoPicturesLabel.Location = new Point(Convert.ToInt32(FormSize.Width * 0.25 - 92 * DpiScale), Convert.ToInt32(FormSize.Height * 0.5));
            ClosePicturesButton.Font = new Font(fontname, mainfontsize);
            ClosePicturesButton.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 - 190 * DpiScale), Convert.ToInt32(FormSize.Height - 120 * DpiScale));
            SearchMethodComboBox.Font = new Font(fontname, mainfontsize);
            SearchMethodComboBox.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 - 160 * DpiScale), SearchOptionComboBox.Location.Y);
            SearchFormTextBox.Font = new Font(fontname, mainfontsize);
            SearchFormTextBox.Width = Convert.ToInt32(FormSize.Width * 0.5 - 340 * DpiScale);
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
            EditButton.Location = new Point(Convert.ToInt32(FormSize.Width - 615 * DpiScale), Convert.ToInt32(FormSize.Height - 90 * DpiScale));
            EditButton.Font = new Font(fontname, mainfontsize);
            EditRequestingButton.Location = new Point(Convert.ToInt32(FormSize.Width - 615 * DpiScale), Convert.ToInt32(FormSize.Height - 90 * DpiScale));
            EditRequestingButton.Font = new Font(fontname, mainfontsize);
            SaveAndCloseEditButton.Location = new Point(Convert.ToInt32(FormSize.Width - 620 * DpiScale), Convert.ToInt32(FormSize.Height - 90 * DpiScale));
            SaveAndCloseEditButton.Font = new Font(fontname, mainfontsize);
            InventoryManagementModeButton.Location = new Point(Convert.ToInt32(FormSize.Width - 445 * DpiScale), Convert.ToInt32(FormSize.Height - 90 * DpiScale));
            InventoryManagementModeButton.Font = new Font(fontname, mainfontsize);
            ShowConfidentialDataButton.Location = new Point(Convert.ToInt32(FormSize.Width - 230 * DpiScale), Convert.ToInt32(FormSize.Height - 90 * DpiScale));
            ShowConfidentialDataButton.Font = new Font(fontname, mainfontsize);
            // 在庫管理モード関係
            InventoryModeDataGridView.Width = Convert.ToInt32(FormSize.Width * 0.5 - 20 * DpiScale);
            InventoryModeDataGridView.Height = Convert.ToInt32(FormSize.Height - 340 * DpiScale);
            InventoryModeDataGridView.Location = new Point(Convert.ToInt32(10 * DpiScale), Convert.ToInt32(140 * DpiScale));
            InventoryModeDataGridView.Font = new Font(fontname, mainfontsize);
            InventoryOperation.Location = new Point(Convert.ToInt32(30 * DpiScale), Convert.ToInt32(FormSize.Height - 195 * DpiScale));
            InventoryOperation.Font = new Font(fontname, mainfontsize);
            InputQuantitiy.Location = new Point(Convert.ToInt32(210 * DpiScale), Convert.ToInt32(FormSize.Height - 195 * DpiScale));
            InputQuantitiy.Font = new Font(fontname, mainfontsize);
            OperationOptionComboBox.Location = new Point(Convert.ToInt32(30 * DpiScale), Convert.ToInt32(FormSize.Height - 160 * DpiScale));
            OperationOptionComboBox.Font = new Font(fontname, mainfontsize);
            EditQuantityTextBox.Location = new Point(Convert.ToInt32(210 * DpiScale), Convert.ToInt32(FormSize.Height - 160 * DpiScale));
            EditQuantityTextBox.Font = new Font(fontname, mainfontsize);
            AddInventoryOperationButton.Location = new Point(Convert.ToInt32(FormSize.Width * 0.5 - 175 * DpiScale), Convert.ToInt32(FormSize.Height - 165 * DpiScale));
            AddInventoryOperationButton.Font = new Font(fontname, mainfontsize);
            InventoryOperationNote.Location = new Point(Convert.ToInt32(30 * DpiScale), Convert.ToInt32(FormSize.Height - 120 * DpiScale));
            InventoryOperationNote.Font = new Font(fontname, mainfontsize);
            EditInventoryOperationNoteTextBox.Width = Convert.ToInt32(FormSize.Width * 0.5 - 40 * DpiScale);
            EditInventoryOperationNoteTextBox.Location = new Point(Convert.ToInt32(30 * DpiScale), Convert.ToInt32(FormSize.Height - 90 * DpiScale));
            EditInventoryOperationNoteTextBox.Font = new Font(fontname, mainfontsize);
            ProperInventorySettingsComboBox.Font = new Font(fontname, mainfontsize);
            ProperInventorySettingsTextBox.Font = new Font(fontname, mainfontsize);
            SaveProperInventorySettingsButton.Font = new Font(fontname, mainfontsize);
            AllowEditIDButton.Location = new Point(Convert.ToInt32(EditIDTextBox.Location.X + EditIDTextBox.Width - 70 * DpiScale), EditIDTextBox.Location.Y + (EditIDTextBox.Height - AllowEditIDButton.Height) / 2);
            AllowEditIDButton.Font = new Font(fontname, extrasmallfontsize);
            CheckSameMCButton.Location = new Point(Convert.ToInt32(EditMCTextBox.Location.X + EditMCTextBox.Width - 95 * DpiScale), EditMCTextBox.Location.Y + (EditMCTextBox.Height - CheckSameMCButton.Height) / 2);
            CheckSameMCButton.Font = new Font(fontname, extrasmallfontsize);
            NoImageLabel.Location = new Point(Convert.ToInt32(Thumbnail.Location.X + (Thumbnail.Width - NoImageLabel.Width) * 0.5), Convert.ToInt32(Thumbnail.Location.Y + (Thumbnail.Height - NoImageLabel.Height) * 0.5));
            ShowPicturesButton.Location = new Point(Convert.ToInt32(Thumbnail.Location.X + Thumbnail.Width * 0.5 - 85 * DpiScale), ShowPicturesButton.Location.Y);
            ShowPicturesButton.Font = new Font(fontname, mainfontsize);
            SelectThumbnailButton.Location = new Point(Convert.ToInt32(Thumbnail.Location.X + Thumbnail.Width * 0.5 - 85 * DpiScale), SelectThumbnailButton.Location.Y);
            SelectThumbnailButton.Font = new Font(fontname, mainfontsize);
            ShowListButton.Width = Convert.ToInt32(130 * DpiScale);
            ShowListButton.Font = new Font(fontname, mainfontsize);
            // dataGridViewContextMenuStripの文字サイズ
            AddContentsContextStripMenuItem.Font = new Font(fontname, mainfontsize);
            ListUpdateContextStripMenuItem.Font = new Font(fontname, mainfontsize);
            OpenProjectContextStripMenuItem.Font = new Font(fontname, mainfontsize);
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
                await Task.Delay(1);
                if (Directory.Exists(TargetContentsPath) == false)
                {
                    MessageBox.Show("IDが変更されました。\n再度読み込みを行ってください。", "CREC");
                    break;
                }
                if (File.Exists(TargetContentsPath + "\\RED"))
                {
                }
                else
                {
                    if (File.Exists(TargetContentsPath + "\\DED"))
                    {
                        EditRequestingButton.Visible = false;
                        EditButton.Visible = true;
                        EditButton.Text = "保存して終了";
                        MessageBox.Show("拒否されました", "CREC");
                        return;
                    }
                    else
                    {
                        EditRequestingButton.Visible = false;
                        EditButton.Visible = true;
                        EditButton.Text = "編集";
                        StartEditForm();
                        // 現時点でのデータを読み込んで表示
                        LoadDetails();
                        // 詳細情報読み込み＆表示
                        StreamReader sr1 = null;
                        try
                        {
                            sr1 = new StreamReader(TargetDetailsPath);
                        }
                        catch (Exception ex)
                        {
                            DetailsTextBox.Text = "No Data.";
                        }
                        finally
                        {
                            if (sr1 != null)
                            {
                                DetailsTextBox.Text = sr1.ReadToEnd();
                                sr1.Close();
                            }
                        }
                        // 機密情報を読み込み
                        try
                        {
                            StreamReader sr2 = new StreamReader(TargetContentsPath + "\\confidentialdata.txt");
                            ConfidentialDataTextBox.Text = sr2.ReadToEnd();
                            sr2.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("データの読み込みに失敗しました\n" + ex.Message, "CREC");
                        }
                        EditNameTextBox.Text = ThisName;
                        EditIDTextBox.TextChanged -= IDTextBox_TextChanged;// ID重複確認イベントを停止
                        EditIDTextBox.Text = ThisID;
                        EditIDTextBox.TextChanged += IDTextBox_TextChanged;// ID重複確認イベントを開始
                        AllowEditIDButton.Text = "編集不可";
                        ReissueUUIDToolStripMenuItem.Enabled = false;
                        EditMCTextBox.Text = ThisMC;
                        EditRegistrationDateTextBox.Text = ThisRegistrationDate;
                        EditCategoryTextBox.Text = ThisCategory;
                        EditTag1TextBox.Text = ThisTag1;
                        EditTag2TextBox.Text = ThisTag2;
                        EditTag3TextBox.Text = ThisTag3;
                        EditRealLocationTextBox.Text = ThisRealLocation;
                        return;
                    }
                }
            }
            return;
        }
        private async void AwaitEditRequest()// 編集リクエストを待機
        {
            while (File.Exists(TargetContentsPath + "\\DED"))// DEDがある（編集中）のみ実施、消えたら終わる
            {
                await Task.Delay(1);
                if (File.Exists(TargetContentsPath + "\\RED"))
                {
                    System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("他の端末から編集権限をリクエストされました。\nリクエストを許可しますか？", "CREC", System.Windows.MessageBoxButton.YesNo);
                    if (result == System.Windows.MessageBoxResult.Yes)// 編集を権限を譲渡
                    {
                        // 保存関係の処理
                        if (TargetContentsPath == TargetFolderPath + "\\" + EditIDTextBox.Text)
                        {
                            SaveContentsMethod();
                        }
                        else if (TargetContentsPath != TargetFolderPath + "\\" + EditIDTextBox.Text)
                        {
                            result = System.Windows.MessageBox.Show("IDを変更した場合、他の端末へ編集権限を譲渡できなくなります。\nIDを変更前のものに戻して保存しますか？", "CREC", System.Windows.MessageBoxButton.YesNo);
                            if (result == System.Windows.MessageBoxResult.Yes)
                            {
                                ThisID = Path.GetFileName(TargetContentsPath);
                                EditIDTextBox.Text = ThisID;
                                MessageBox.Show("IDを" + ThisID + "に戻しました", "CREC");
                                SaveContentsMethod();
                            }
                            else
                            {
                                SaveContentsMethod();
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
                        // 通常画面用にラベルを変更
                        ShowPicturesButton.Text = "画像を表示";
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
                        File.Delete(TargetContentsPath + "\\RED");
                        break;
                    }
                }
            }
            return;
        }
        private async void CheckContentsList()// ContentsListの状態をバックグラウンドで監視
        {
            while (true)
            {
                await Task.Delay(1);
                if (dataGridView1.CurrentRow == null)// セル未選択時は何もしない
                {
                }
                else
                {
                    if (ThisID != Convert.ToString(dataGridView1.CurrentRow.Cells[1].Value))
                    {
                        if (SaveAndCloseEditButton.Visible == true)// 編集中の場合は警告を表示
                        {
                            if (CheckEditingContents() == true)
                            {
                                LoadDetails();
                                ShowDetails();
                            }
                            else
                            {
                                return;
                            }
                        }
                        else
                        {
                            LoadDetails();
                            ShowDetails();
                        }
                    }
                }
            }
        }
        private async void CheckEditing()// 表示中のデータが他の端末で編集中かバックグラウンドで監視
        {
            while (true)
            {
                await Task.Delay(1);
                if (File.Exists(TargetContentsPath + "\\DED"))
                {
                    if (File.Exists(TargetContentsPath + "\\RED"))
                    {
                        EditButton.Text = "閲覧のみ可能";
                        EditButton.ForeColor = Color.Red;
                    }
                    else
                    {
                        if (SaveAndCloseEditButton.Visible == false)
                        {
                            EditButton.Text = "他端末編集中";
                            EditButton.ForeColor = Color.Blue;
                        }
                    }
                }
                else
                {
                    EditButton.Text = "編集";
                    EditButton.ForeColor = Color.Black;
                }
            }
        }
        private async void MakeBackUpZip()// バックアップ処理のうちZIP圧縮の部分
        {
            DateTime DT = DateTime.Now;
            BackupToolStripMenuItem.Text = "バックアップ作成中";
            BackupToolStripMenuItem.Enabled = false;
            // バックアップ作成
            if (CompressType == 0)// 単一ZIPに圧縮
            {
                await Task.Run(() =>
                {
                    try
                    {
                        ZipFile.CreateFromDirectory(TargetBackupPath + "\\backuptmp", TargetBackupPath + "\\" + TargetProjectName + "_backup-" + DT.ToString("yyyy年MM月dd日HH時mm分ss秒") + ".zip");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("バックアップ作成に失敗しました。\n" + ex.Message, "CREC");
                        BackupToolStripMenuItem.Text = "バックアップ作成";
                        BackupToolStripMenuItem.Enabled = true;
                        return;
                    }
                    Directory.Delete(TargetBackupPath + "\\backuptmp", true);
                });
            }
            else if (CompressType == 1)// コンテンツごとに圧縮
            {
                await Task.Run(() =>
                {
                    // バックアップ用フォルダ作成
                    Directory.CreateDirectory(TargetBackupPath + "\\" + TargetProjectName + "_backup-" + DT.ToString("yyyy年MM月dd日HH時mm分ss秒"));
                    File.Copy(TargetCRECPath, TargetBackupPath + "\\" + TargetProjectName + "_backup-" + DT.ToString("yyyy年MM月dd日HH時mm分ss秒") + "\\backup.crec", true);// crecファイルをバックアップ
                    try
                    {
                        System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(TargetFolderPath);
                        IEnumerable<System.IO.DirectoryInfo> subFolders = di.EnumerateDirectories("*");
                        int CountBackupedData = 0;
                        int TotalBackupData = subFolders.Count();
                        foreach (System.IO.DirectoryInfo subFolder in subFolders)
                        {
                            try
                            {
                                FileSystem.CopyDirectory(subFolder.FullName, "backuptmp\\" + subFolder.Name + "\\datatemp", Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing);
                                ZipFile.CreateFromDirectory("backuptmp\\" + subFolder.Name + "\\datatemp", "backuptmp\\" + subFolder.Name + "backupziptemp.zip");// 圧縮
                                File.Move("backuptmp\\" + subFolder.Name + "backupziptemp.zip", TargetBackupPath + "\\" + TargetProjectName + "_backup-" + DT.ToString("yyyy年MM月dd日HH時mm分ss秒") + "\\" + subFolder.Name + "_backup-" + DT.ToString("yyyy-MM-dd-HH-mm-ss") + ".zip");// 移動
                                Directory.Delete("backuptmp\\" + subFolder.Name, true);// 削除
                                CountBackupedData += 1;
                                BackupToolStripMenuItem.Text = "バックアップ作成中：" + Convert.ToString(CountBackupedData) + "/" + Convert.ToString(TotalBackupData);
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
                        MessageBox.Show("バックアップ作成に失敗しました。\n" + ex.Message, "CREC");
                        BackupToolStripMenuItem.Text = "バックアップ作成";
                        BackupToolStripMenuItem.Enabled = true;
                        return;
                    }
                    if (File.Exists("BackupErrorLog.txt"))
                    {
                        MessageBox.Show("いくつかのファイルのバックアップ作成に失敗しました。\nログを確認してください。", "CREC");
                    }
                });
            }

            BackupToolStripMenuItem.Text = "バックアップ作成";
            BackupToolStripMenuItem.Enabled = true;
            MessageBox.Show("バックアップ作成が完了しました。", "CREC");
        }
        private async void CheckLatestVersion()// 更新確認
        {
            HttpClient httpClient = new HttpClient();
            try
            {
                // githubのreleaseにアクセスできるか確認
                HttpResponseMessage httpResponseMessage1 = await httpClient.GetAsync("https://github.com/Yukisita/CREC/releases/tag/Latest_Release");
                if (httpResponseMessage1.IsSuccessStatusCode)// githubへのアクセスができた場合
                {
                    try
                    {
                        // 本バージョンと一致する配布先があるか確認
                        HttpResponseMessage httpResponseMessage2 = await httpClient.GetAsync(LatestVersionDownloadLink);
                        if (httpResponseMessage2.IsSuccessStatusCode)// 配布バージョンと一致した場合
                        {
                            // MessageBox.Show("success");//デバッグ用
                        }
                        else
                        {
                            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show("アプリケーションの更新が見つかりました。\n最新バージョンの配布先にアクセスしますか？", "CREC", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning);
                            if (result == System.Windows.MessageBoxResult.Yes)// ブラウザでリンクを表示
                            {
                                System.Diagnostics.Process.Start("https://github.com/Yukisita/CREC/releases/tag/Latest_Release");
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

        #region ユーザーアシストのToolTip設定
        private void SetUserAssistToolTips()
        {
            if (ShowUserAssistToolTips == true)
            {
                UserAssistToolTip.SetToolTip(SearchFormTextBox, "検索する文字列を入力");
                UserAssistToolTip.SetToolTip(SearchOptionComboBox, "検索内容を選択");
                UserAssistToolTip.SetToolTip(SearchMethodComboBox, "検索方法を選択");
                UserAssistToolTip.SetToolTip(EditIDTextBox, "システムに必要な固有値");
                UserAssistToolTip.SetToolTip(EditMCTextBox, "JAN等の任意で管理コードを入力");
                UserAssistToolTip.SetToolTip(EditQuantityTextBox, "数字を入力");
            }
            else if (ShowUserAssistToolTips == false)
            {
                UserAssistToolTip.RemoveAll();
            }
        }
        #endregion

        #region DataGridView内のContextStripMenuの設定
        private void AddContentsContextStripMenuItem_Click(object sender, EventArgs e)// データ追加
        {
            AddContentsMethod();// 新規にデータを追加するメソッドを呼び出し
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
            if (StartUpListOutput == true)
            {
                if (ListOutputFormat == "CSV")
                {
                    CSVListOutputMethod();
                }
                else if (ListOutputFormat == "TSV")
                {
                    TSVListOutputMethod();
                }
                else
                {
                    MessageBox.Show("値が不正です。", "CREC");
                }
            }
            if (StartUpBackUp == true)// 自動バックアップ
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
                MessageBox.Show("先にプロジェクトを開いてください。", "CREC");
                return;
            }
            // バックアップ場所が設定されているか確認
            if (TargetBackupPath.Length == 0)
            {
                MessageBox.Show("バックアップフォルダが指定されていません。", "CREC");
                return;
            }
            // バックアップフォルダが存在するか確認
            if (Directory.Exists(TargetBackupPath))
            {

            }
            else
            {
                MessageBox.Show("バックアップフォルダが見つかりませんでした", "CREC");
                return;
            }
            BackupToolStripMenuItem.Text = "バックアップ作成中";
            if (CompressType == 0)
            {
                FileSystem.CopyDirectory(TargetFolderPath, TargetBackupPath + "\\backuptmp", Microsoft.VisualBasic.FileIO.UIOption.AllDialogs, Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing);
                File.Copy(TargetCRECPath, TargetBackupPath + "\\backuptmp" + "\\backup.crec", true);
            }
            else if (CompressType == 2)
            {
                DateTime DT = DateTime.Now;
                FileSystem.CopyDirectory(TargetFolderPath, TargetBackupPath + "\\" + TargetProjectName + "_backup-" + DT.ToString("yyyy年MM月dd日HH時mm分ss秒"), Microsoft.VisualBasic.FileIO.UIOption.AllDialogs, Microsoft.VisualBasic.FileIO.UICancelOption.DoNothing);
                File.Copy(TargetCRECPath, TargetBackupPath + "\\" + TargetProjectName + "_backup-" + DT.ToString("yyyy年MM月dd日HH時mm分ss秒") + "\\backup.crec", true);
            }
        }
        #endregion

        #region List一覧出力関係
        private void CSVListOutputMethod()// CSV形式で一覧を出力
        {
            // ファイルが開いているか確認
            if (TargetCRECPath.Length == 0)
            {
                MessageBox.Show("先にプロジェクトを開いてください。", "CREC");
                return;
            }
            try
            {
                string tempTargetListOutputPath = "";
                if (Directory.Exists(TargetListOutputPath))
                {
                    tempTargetListOutputPath = TargetListOutputPath;
                }
                else
                {
                    tempTargetListOutputPath = TargetFolderPath;
                }
                StreamWriter streamWriter = new StreamWriter(tempTargetListOutputPath + "\\InventoryOutput.csv", false, Encoding.GetEncoding("shift-jis"));
                streamWriter.WriteLine("データ保存場所のパス,ID,管理コード,名称,登録日,カテゴリー,タグ1,タグ2,タグ3,在庫数,在庫状況");
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(TargetFolderPath);
                try
                {
                    IEnumerable<System.IO.DirectoryInfo> subFolders = di.EnumerateDirectories("*");
                    foreach (System.IO.DirectoryInfo subFolder in subFolders)
                    {
                        // 変数初期化「ListSA作成処理内でのみ使用すること」
                        string ListThisName = "";
                        string ListThisID = "";
                        string ListThisMC = "";
                        string ListThisCategory = "";
                        string ListThisTag1 = "";
                        string ListThisTag2 = "";
                        string ListThisTag3 = "";
                        string ListRegistrationDate = "";
                        string ListInventory = "";
                        string ListInventoryStatus = "";
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
                                        ListThisName = line.Substring(3).Replace(",", "");
                                        break;
                                    case "ID":
                                        ListThisID = line.Substring(3).Replace(",", "");
                                        break;
                                    case "MC":
                                        ListThisMC = line.Substring(3).Replace(",", "");
                                        break;
                                    case "登録日":
                                        ListRegistrationDate = line.Substring(4).Replace(",", "");
                                        break;
                                    case "カテゴリ":
                                        ListThisCategory = line.Substring(5).Replace(",", "");
                                        break;
                                    case "タグ1":
                                        ListThisTag1 = line.Substring(4).Replace(",", "");
                                        break;
                                    case "タグ2":
                                        ListThisTag2 = line.Substring(4).Replace(",", "");
                                        break;
                                    case "タグ3":
                                        ListThisTag3 = line.Substring(4).Replace(",", "");
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Indexファイルが破損しています。\n" + ex.Message, "CREC");
                            ListThisID = subFolder.Name;
                            ListThisName = "Status：Indexファイル破損";
                            ListThisCategory = "　ー　";
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
                        streamWriter.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", subFolder.FullName, ListThisID, ListThisMC, ListThisName, ListRegistrationDate, ListThisCategory, ListThisTag1, ListThisTag2, ListThisTag3, ListInventory, ListInventoryStatus);
                    }
                    streamWriter.Close();
                    MessageBox.Show("データ一覧を以下の場所にCSV形式で出力しました。\n" + tempTargetListOutputPath + "\\InventoryOutput.csv", "CREC");
                }
                catch (Exception ex)
                {

                }
                if (OpenListAfterOutput == true)
                {
                    if (ListOutputFormat == "CSV")
                    {
                        System.Diagnostics.Process process = System.Diagnostics.Process.Start(tempTargetListOutputPath + "\\InventoryOutput.csv");
                    }
                    else if (ListOutputFormat == "TSV")
                    {
                        System.Diagnostics.Process process = System.Diagnostics.Process.Start(tempTargetListOutputPath + "\\InventoryOutput.tsv");
                    }
                    else
                    {
                        MessageBox.Show("値が不正です。", "CREC");
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private void TSVListOutputMethod()// TSV形式で一覧を出力
        {
            // ファイルが開いているか確認
            if (TargetCRECPath.Length == 0)
            {
                MessageBox.Show("先にプロジェクトを開いてください。", "CREC");
                return;
            }
            try
            {
                string tempTargetListOutputPath = "";
                if (Directory.Exists(TargetListOutputPath))
                {
                    tempTargetListOutputPath = TargetListOutputPath;
                }
                else
                {
                    tempTargetListOutputPath = TargetFolderPath;
                }
                StreamWriter streamWriter = new StreamWriter(tempTargetListOutputPath + "\\InventoryOutput.tsv", false, Encoding.GetEncoding("shift-jis"));
                streamWriter.WriteLine("データ保存場所のパス\tID\t管理コード\t名称\t登録日\tカテゴリー\tタグ1\tタグ2\tタグ3\t在庫数\t在庫状況");
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(TargetFolderPath);
                try
                {
                    IEnumerable<System.IO.DirectoryInfo> subFolders = di.EnumerateDirectories("*");
                    foreach (System.IO.DirectoryInfo subFolder in subFolders)
                    {
                        // 変数初期化「ListSA作成処理内でのみ使用すること」
                        string ListThisName = "";
                        string ListThisID = "";
                        string ListThisMC = "";
                        string ListThisCategory = "";
                        string ListThisTag1 = "";
                        string ListThisTag2 = "";
                        string ListThisTag3 = "";
                        string ListRegistrationDate = "";
                        string ListInventory = "";
                        string ListInventoryStatus = "";
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
                                        ListThisName = line.Substring(3);
                                        break;
                                    case "ID":
                                        ListThisID = line.Substring(3);
                                        break;
                                    case "MC":
                                        ListThisMC = line.Substring(3);
                                        break;
                                    case "登録日":
                                        ListRegistrationDate = line.Substring(4);
                                        break;
                                    case "カテゴリ":
                                        ListThisCategory = line.Substring(5);
                                        break;
                                    case "タグ1":
                                        ListThisTag1 = line.Substring(4);
                                        break;
                                    case "タグ2":
                                        ListThisTag2 = line.Substring(4);
                                        break;
                                    case "タグ3":
                                        ListThisTag3 = line.Substring(4);
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Indexファイルが破損しています。\n" + ex.Message, "CREC");
                            ListThisID = subFolder.Name;
                            ListThisName = "Status：Indexファイル破損";
                            ListThisCategory = "　ー　";
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
                        streamWriter.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}", subFolder.FullName, ListThisID, ListThisMC, ListThisName, ListRegistrationDate, ListThisCategory, ListThisTag1, ListThisTag2, ListThisTag3, ListInventory, ListInventoryStatus);
                    }
                    streamWriter.Close();
                    MessageBox.Show("データ一覧を以下の場所にTSV形式で出力しました。\n" + tempTargetListOutputPath + "\\InventoryOutput.tsv", "CREC");
                }
                catch (Exception ex)
                {

                }
                if (OpenListAfterOutput == true)
                {
                    if (ListOutputFormat == "CSV")
                    {
                        System.Diagnostics.Process process = System.Diagnostics.Process.Start(tempTargetListOutputPath + "\\InventoryOutput.csv");
                    }
                    else if (ListOutputFormat == "TSV")
                    {
                        System.Diagnostics.Process process = System.Diagnostics.Process.Start(tempTargetListOutputPath + "\\InventoryOutput.tsv");
                    }
                    else
                    {
                        MessageBox.Show("値が不正です。", "CREC");
                    }
                }
            }
            catch (Exception ex)
            {

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
            if (SaveAndCloseEditButton.Visible == true)// 編集中に切り替えた場合の処理
            {
                if (InventoryModeDataGridView.Visible == true)// 在庫管理中に切り替わった場合
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
                else// その他の状態から切り替わった場合の処理
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
        private void ShowListButton_Click(object sender, EventArgs e)// List表示モードに戻る
        {
            if (SaveAndCloseEditButton.Visible == true)// 編集中の場合は警告を表示
            {
                if (CheckEditingContents() == true)
                {
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
                }
                else
                {
                    return;
                }
            }
            else
            {
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
            }
        }
        private void SetColorMethod()// 色設定のメソッド
        {
            switch (ColorSetting)
            {
                case "Blue":
                    ColorSetting = "Blue";
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
                case "White":
                    ColorSetting = "White";
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
                case "Sakura":
                    ColorSetting = "Sakura";
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
                case "Green":
                    ColorSetting = "Green";
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
                    ColorSetting = "Blue";
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
        }
    }
}