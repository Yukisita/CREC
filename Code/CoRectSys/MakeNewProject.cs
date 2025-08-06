/*
MakeNewProjectForm
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CREC
{
    public partial class MakeNewProject : Form
    {
        //string TargetCRECPath = string.Empty;//管理ファイル（.crec）のファイルパス
        readonly XElement LanguageFile;// 言語ファイル
        ProjectSettingValuesClass CurrentProjectSettingValues = new ProjectSettingValuesClass();// 現在編集中のプロジェクトの設定値
        public string ReturnTargetProject { get; private set; } = string.Empty;
        public MakeNewProject(ProjectSettingValuesClass projectSettingValues, XElement languageFile)
        {
            InitializeComponent();
            CurrentProjectSettingValues = projectSettingValues;
            SetColorMethod();
            LanguageFile = languageFile;
        }
        private void MakeNewProject_Load(object sender, EventArgs e)// 既存の.crecを読み込み or 新規作成
        {
            // 言語設定
            SetLanguage();
            // ComboBoxの選択肢を設定
            SleepModeComboBox.Items.Clear();
            foreach (SleepMode sleepMode in Enum.GetValues(typeof(SleepMode)))
            {
                SleepModeComboBox.Items.Add(sleepMode.ToString());
            }

            // 必要な項目を初期化
            CompressTypeComboBox.SelectedIndex = 0;
            FileFormatComboBox.SelectedIndex = 0;// 追加の場合はデフォルト値を各ラベルから取得してTextBoxに入れる
            if (CurrentProjectSettingValues.ProjectSettingFilePath.Length == 0)// 新規作成の場合はプロジェクト設定値を初期化
            {
                CurrentProjectSettingValues = new ProjectSettingValuesClass();
                CurrentProjectSettingValues.CollectionNameLabel = ObjectNameLabel.Text;
                CurrentProjectSettingValues.UUIDLabel = UUIDLabel.Text;
                CurrentProjectSettingValues.ManagementCodeLabel = MCLabel.Text;
                CurrentProjectSettingValues.RegistrationDateLabel = RegistrationDateLabel.Text;
                CurrentProjectSettingValues.CategoryLabel = CategoryLabel.Text;
                CurrentProjectSettingValues.FirstTagLabel = Tag1NameLabel.Text;
                CurrentProjectSettingValues.SecondTagLabel = Tag2NameLabel.Text;
                CurrentProjectSettingValues.ThirdTagLabel = Tag3NameLabel.Text;
                CurrentProjectSettingValues.RealLocationLabel = RealLocationLabel.Text;
                CurrentProjectSettingValues.DataLocationLabel = DataLocationLabel.Text;
                // 現在時刻を取得 
                DateTime dateTime = DateTime.Now;
                CurrentProjectSettingValues.CreatedDate = dateTime.ToString("yyyy/MM/dd hh:mm:ss");
            }
            // 現在の内容を表示
            EditProjectNameTextBox.Text = CurrentProjectSettingValues.Name;
            EditProjectLocationTextBox.Text = CurrentProjectSettingValues.ProjectDataFolderPath;
            EditBackupLocationTextBox.Text = CurrentProjectSettingValues.ProjectBackupFolderPath;
            StartUpBackUpCheckBox.Checked = CurrentProjectSettingValues.StartUpBackUp;
            CloseBackUpCheckBox.Checked = CurrentProjectSettingValues.CloseBackUp;
            EditedBackUpCheckBox.Checked = CurrentProjectSettingValues.EditBackUp;
            CompressTypeComboBox.SelectedIndex = (int)CurrentProjectSettingValues.BackupCompressionType;
            // backupの並列処理
            switch (CurrentProjectSettingValues.MaxDegreeOfBackUpProcessParallelism)
            {
                case null:
                case 0:
                    MaxDegreeOfBackUpProcessParallelismComboBox.SelectedIndex = 0;
                    break;
                default:
                    try
                    {
                        MaxDegreeOfBackUpProcessParallelismComboBox.SelectedIndex
                            = Convert.ToInt32(CurrentProjectSettingValues.MaxDegreeOfBackUpProcessParallelism);
                    }
                    catch
                    {
                        MaxDegreeOfBackUpProcessParallelismComboBox.SelectedIndex = 0;
                    }
                    break;
            }
            EditListOutputLocationTextBox.Text = CurrentProjectSettingValues.ListOutputPath;
            StartUpListOutputCheckBox.Checked = CurrentProjectSettingValues.StartUpListOutput;
            CloseListOutputCheckBox.Checked = CurrentProjectSettingValues.CloseListOutput;
            EditedListOutputCheckBox.Checked = CurrentProjectSettingValues.EditListOutput;
            OpenListAfterOutputCheckBox.Checked = CurrentProjectSettingValues.OpenListAfterOutput;
            FileFormatComboBox.SelectedIndex = (int)CurrentProjectSettingValues.ListOutputFormat;
            EditObjectNameLabelTextBox.Text = CurrentProjectSettingValues.CollectionNameLabel;
            ShowObjectNameLabelVisibleCheckBox.Checked = CurrentProjectSettingValues.CollectionNameVisible;
            EditUUIDLabelTextBox.Text = CurrentProjectSettingValues.UUIDLabel;
            ShowIDLabelVisibleCheckBox.Checked = CurrentProjectSettingValues.UUIDVisible;
            EditMCLabelTextBox.Text = CurrentProjectSettingValues.ManagementCodeLabel;
            ShowMCLabelVisibleCheckBox.Checked = CurrentProjectSettingValues.ManagementCodeVisible;
            AutoMCFillCheckBox.Checked = CurrentProjectSettingValues.ManagementCodeAutoFill;
            EditRegistrationDateLabelTextBox.Text = CurrentProjectSettingValues.RegistrationDateLabel;
            ShowRegistrationDateLabelVisibleCheckBox.Checked = CurrentProjectSettingValues.RegistrationDateVisible;
            EditCategoryLabelTextBox.Text = CurrentProjectSettingValues.CategoryLabel;
            ShowCategoryLabelVisibleCheckBox.Checked = CurrentProjectSettingValues.CategoryVisible;
            EditTag1NameTextBox.Text = CurrentProjectSettingValues.FirstTagLabel;
            Tag1NameVisibleCheckBox.Checked = CurrentProjectSettingValues.FirstTagVisible;
            EditTag2NameTextBox.Text = CurrentProjectSettingValues.SecondTagLabel;
            Tag2NameVisibleCheckBox.Checked = CurrentProjectSettingValues.SecondTagVisible;
            EditTag3NameTextBox.Text = CurrentProjectSettingValues.ThirdTagLabel;
            Tag3NameVisibleCheckBox.Checked = CurrentProjectSettingValues.ThirdTagVisible;
            EditRealLocationLabelTextBox.Text = CurrentProjectSettingValues.RealLocationLabel;
            ShowRealLocationLabelVisibleCheckBox.Checked = CurrentProjectSettingValues.RealLocationVisible;
            EditDataLocationLabelTextBox.Text = CurrentProjectSettingValues.DataLocationLabel;
            ShowDataLocationLabelVisibleCheckBox.Checked = CurrentProjectSettingValues.DataLocationVisible;
            SleepModeComboBox.SelectedIndex = (int)CurrentProjectSettingValues.SleepMode;
            DataCheckIntervalTextBox.Text = Convert.ToString(CurrentProjectSettingValues.DataCheckInterval);
            switch (CurrentProjectSettingValues.DataCheckInterval)
            {
                case 100:
                    DataCheckIntervalComboBox.SelectedIndex = 0;
                    break;
                case 500:
                    DataCheckIntervalComboBox.SelectedIndex = 1;
                    break;
                case 1000:
                    DataCheckIntervalComboBox.SelectedIndex = 2;
                    break;
                default:
                    DataCheckIntervalComboBox.SelectedIndex = 3;
                    break;
            }
            MaxBackupCountTextBox.Text = Convert.ToString(CurrentProjectSettingValues.MaxBackupCount);
            CollectionListAutoUpdateCheckBox.Checked = CurrentProjectSettingValues.CollectionListAutoUpdate;
        }
        private void MakeNewProjectButton_Click(object sender, EventArgs e)// 保存してプロジェクト編集画面を閉じる
        {
            // 内容を確認
            int error = 0;// 入力内容に不備がない場合は0、不備を発見した場合は1に変更
            if (EditProjectNameTextBox.TextLength == 0)
            {
                error = 1;
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("EnterProjectNameError", "MakeNewProject", LanguageFile), "CREC");
            }
            if (EditProjectLocationTextBox.TextLength == 0)
            {
                error = 1;
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("EnterProjectFolderPathError", "MakeNewProject", LanguageFile), "CREC");
            }

            if (error == 0)// 記入内容に問題がなかった場合は.crecファイルを作成
            {
                // 現在時刻を取得 
                DateTime dateTime = DateTime.Now;
                CurrentProjectSettingValues.ModifiedDate = dateTime.ToString("yyyy/MM/dd hh:mm:ss");
                // 編集内容をプロジェクト設定値に反映
                CurrentProjectSettingValues.Name = EditProjectNameTextBox.Text;
                CurrentProjectSettingValues.ProjectDataFolderPath = EditProjectLocationTextBox.Text;
                CurrentProjectSettingValues.ProjectBackupFolderPath = EditBackupLocationTextBox.Text;
                CurrentProjectSettingValues.StartUpBackUp = StartUpBackUpCheckBox.Checked;
                CurrentProjectSettingValues.CloseBackUp = CloseBackUpCheckBox.Checked;
                CurrentProjectSettingValues.EditBackUp = EditedBackUpCheckBox.Checked;
                CurrentProjectSettingValues.BackupCompressionType = (BackupCompressionType)CompressTypeComboBox.SelectedIndex;
                if (MaxDegreeOfBackUpProcessParallelismComboBox.SelectedIndex == 0)
                {
                    CurrentProjectSettingValues.MaxDegreeOfBackUpProcessParallelism = null;
                }
                else
                {
                    CurrentProjectSettingValues.MaxDegreeOfBackUpProcessParallelism = MaxDegreeOfBackUpProcessParallelismComboBox.SelectedIndex;
                }
                CurrentProjectSettingValues.ListOutputPath = EditListOutputLocationTextBox.Text;
                CurrentProjectSettingValues.StartUpListOutput = StartUpListOutputCheckBox.Checked;
                CurrentProjectSettingValues.CloseListOutput = CloseListOutputCheckBox.Checked;
                CurrentProjectSettingValues.EditListOutput = EditedListOutputCheckBox.Checked;
                CurrentProjectSettingValues.OpenListAfterOutput = OpenListAfterOutputCheckBox.Checked;
                CurrentProjectSettingValues.ListOutputFormat = (ListOutputFormat)FileFormatComboBox.SelectedIndex;
                CurrentProjectSettingValues.CollectionNameLabel = EditObjectNameLabelTextBox.Text;
                CurrentProjectSettingValues.CollectionNameVisible = ShowObjectNameLabelVisibleCheckBox.Checked;
                CurrentProjectSettingValues.UUIDLabel = EditUUIDLabelTextBox.Text;
                CurrentProjectSettingValues.UUIDVisible = ShowIDLabelVisibleCheckBox.Checked;
                CurrentProjectSettingValues.ManagementCodeLabel = EditMCLabelTextBox.Text;
                CurrentProjectSettingValues.ManagementCodeVisible = ShowMCLabelVisibleCheckBox.Checked;
                CurrentProjectSettingValues.ManagementCodeAutoFill = AutoMCFillCheckBox.Checked;
                CurrentProjectSettingValues.RegistrationDateLabel = EditRegistrationDateLabelTextBox.Text;
                CurrentProjectSettingValues.RegistrationDateVisible = ShowRegistrationDateLabelVisibleCheckBox.Checked;
                CurrentProjectSettingValues.CategoryLabel = EditCategoryLabelTextBox.Text;
                CurrentProjectSettingValues.CategoryVisible = ShowCategoryLabelVisibleCheckBox.Checked;
                CurrentProjectSettingValues.FirstTagLabel = EditTag1NameTextBox.Text;
                CurrentProjectSettingValues.FirstTagVisible = Tag1NameVisibleCheckBox.Checked;
                CurrentProjectSettingValues.SecondTagLabel = EditTag2NameTextBox.Text;
                CurrentProjectSettingValues.SecondTagVisible = Tag2NameVisibleCheckBox.Checked;
                CurrentProjectSettingValues.ThirdTagLabel = EditTag3NameTextBox.Text;
                CurrentProjectSettingValues.ThirdTagVisible = Tag3NameVisibleCheckBox.Checked;
                CurrentProjectSettingValues.RealLocationLabel = EditRealLocationLabelTextBox.Text;
                CurrentProjectSettingValues.RealLocationVisible = ShowRealLocationLabelVisibleCheckBox.Checked;
                CurrentProjectSettingValues.DataLocationLabel = EditDataLocationLabelTextBox.Text;
                CurrentProjectSettingValues.DataLocationVisible = ShowDataLocationLabelVisibleCheckBox.Checked;
                CurrentProjectSettingValues.SleepMode = (SleepMode)SleepModeComboBox.SelectedIndex;
                try
                {
                    CurrentProjectSettingValues.DataCheckInterval = Convert.ToInt32(DataCheckIntervalTextBox.Text);
                }
                catch
                {
                    MessageBox.Show("データ監視間隔の値が正しく入力されていません。\n1以上の整数値を入力してください。", "CREC");
                }
                try
                {
                    int maxBackupCount = Convert.ToInt32(MaxBackupCountTextBox.Text);
                    CurrentProjectSettingValues.MaxBackupCount = maxBackupCount >= 1 ? maxBackupCount : 256;
                }
                catch
                {
                    MessageBox.Show("バックアップ保持数の値が正しく入力されていません。\n1以上の整数値を入力してください。", "CREC");
                    return;
                }
                CurrentProjectSettingValues.CollectionListAutoUpdate = CollectionListAutoUpdateCheckBox.Checked;
                // プロジェクトデータ保管場所が存在するか判定し、作成
                if (CurrentProjectSettingValues.ProjectSettingFilePath.Length == 0)// 新規プロジェクト作成の場合
                {
                    if (Directory.Exists(EditProjectLocationTextBox.Text))
                    {
                        MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("ProjectDirectoryAlreadyExist", "MakeNewProject", LanguageFile), "CREC");
                    }
                    else
                    {
                        try
                        {
                            Directory.CreateDirectory(EditProjectLocationTextBox.Text);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("プロジェクトフォルダの作成に失敗しました。\n" + ex.Message, "CREC");
                            return;
                        }
                    }
                }
                else// 既存プロジェクト編集の場合
                {
                    if (Directory.Exists(EditProjectLocationTextBox.Text))
                    {
                        MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("ProjectDirectoryAlreadyExist", "MakeNewProject", LanguageFile), "CREC");
                    }
                    else
                    {
                        MessageBox.Show("指定されたプロジェクトフォルダが見つかりませんでした。", "CREC");
                    }
                }
                if (EditBackupLocationTextBox.Text.Length != 0)// バックアップ場所の作成
                {
                    if (Directory.Exists(EditBackupLocationTextBox.Text))
                    {
                        MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("ProjectBackupDirectoryAlreadyExist", "MakeNewProject", LanguageFile), "CREC");
                    }
                    else
                    {
                        try
                        {
                            Directory.CreateDirectory(EditBackupLocationTextBox.Text);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("バックアップトフォルダの作成に失敗しました。\n" + ex.Message, "CREC");
                            return;
                        }
                    }
                }
                if (CurrentProjectSettingValues.ProjectSettingFilePath.Length > 0)// 編集時は既存の.crecを削除
                {
                    try
                    {
                        File.Delete(CurrentProjectSettingValues.ProjectSettingFilePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("既存の管理ファイルの削除に失敗しました。\n新規に管理ファイルを作成します。\n" + ex.Message, "CREC");
                    }
                }
                else
                {
                    CurrentProjectSettingValues.ProjectSettingFilePath = System.Environment.CurrentDirectory + "\\" + EditProjectNameTextBox.Text + ".crec";
                }
                ProjectSettingClass.SaveProjectSetting(CurrentProjectSettingValues, LanguageFile);
                ReturnTargetProject = CurrentProjectSettingValues.ProjectSettingFilePath;
                this.Close();
            }
        }
        private void ProjectLocationReferenceButton_Click(object sender, EventArgs e)// プロジェクトの場所を参照
        {
            if (EditProjectNameTextBox.Text.Length == 0)
            {
                MessageBox.Show("先にプロジェクト名を入力してください。", "CREC");
                return;
            }
            OpenFileDialog openFolderDialog = new OpenFileDialog();
            openFolderDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFolderDialog.Title = "フォルダを選択してください";
            openFolderDialog.Filter = "Folder|.";
            openFolderDialog.FileName = "SelectFolder";
            openFolderDialog.CheckFileExists = false;
            if (openFolderDialog.ShowDialog() == DialogResult.OK)// ファイル読み込み成功
            {
                if (CurrentProjectSettingValues.ProjectSettingFilePath.Length == 0)
                {
                    EditProjectLocationTextBox.Text = Path.GetDirectoryName(openFolderDialog.FileName) + "\\" + EditProjectNameTextBox.Text;
                }
                else
                {
                    EditProjectLocationTextBox.Text = Path.GetDirectoryName(openFolderDialog.FileName);
                }
                openFolderDialog.Dispose();
            }
            if (EditProjectLocationTextBox.Text == EditBackupLocationTextBox.Text)
            {
                MessageBox.Show("プロジェクト場所とバックアップは同じフォルダに設定できません。", "CREC");
                EditProjectLocationTextBox.ResetText();
            }
        }
        private void BackupLocationReferenceButton_Click(object sender, EventArgs e)// バックアップ先を参照
        {
            if (EditProjectNameTextBox.Text.Length == 0)
            {
                MessageBox.Show("先にプロジェクト名を入力してください。", "CREC");
                return;
            }
            OpenFileDialog openFolderDialog = new OpenFileDialog();
            openFolderDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFolderDialog.Title = "フォルダを選択してください";
            openFolderDialog.Filter = "Folder|.";
            openFolderDialog.FileName = "SelectFolder";
            openFolderDialog.CheckFileExists = false;
            if (openFolderDialog.ShowDialog() == DialogResult.OK)// フォルダ読み込み成功
            {
                EditBackupLocationTextBox.Text = Path.GetDirectoryName(openFolderDialog.FileName) + "\\" + EditProjectNameTextBox.Text + "_Backup";
                openFolderDialog.Dispose();
            }
            if (EditProjectLocationTextBox.Text == EditBackupLocationTextBox.Text)
            {
                MessageBox.Show("プロジェクト場所とバックアップは同じフォルダに設定できません。", "CREC");
                EditBackupLocationTextBox.ResetText();
            }
        }
        private void ListOutputLocationReferenceButton_Click(object sender, EventArgs e)// データ一覧作成先を参照
        {
            OpenFileDialog openFolderDialog = new OpenFileDialog();
            openFolderDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFolderDialog.Title = "フォルダを選択してください";
            openFolderDialog.Filter = "Folder|.";
            openFolderDialog.FileName = "SelectFolder";
            openFolderDialog.CheckFileExists = false;
            if (openFolderDialog.ShowDialog() == DialogResult.OK)// フォルダ読み込み成功
            {
                EditListOutputLocationTextBox.Text = Path.GetDirectoryName(openFolderDialog.FileName);
                openFolderDialog.Dispose();
            }
        }
        private void SetColorMethod()// 色設定のメソッド
        {
            switch (CurrentProjectSettingValues.ColorSetting.ToString())
            {
                case "Blue":
                    this.BackColor = Color.AliceBlue;
                    break;
                case "White":
                    this.BackColor = Color.WhiteSmoke;
                    break;
                case "Sakura":
                    this.BackColor = Color.LavenderBlush;
                    break;
                case "Green":
                    this.BackColor = Color.Honeydew;
                    break;
                default:
                    this.BackColor = Color.AliceBlue;
                    CurrentProjectSettingValues.ColorSetting = 0;
                    break;
            }
        }

        private void EditUUIDLabelTextBox_Click(object sender, EventArgs e)// UUIDのラベルを変更可能に切り替える
        {
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("AskChangeUUIDLabel", "MakeNewProject", LanguageFile), "CREC", System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Warning);
            if (result == System.Windows.MessageBoxResult.Yes)// 注意事項を確認してOKだった場合は、編集を許可する
            {
                EditUUIDLabelTextBox.ReadOnly = false;
            }
        }

        /// <summary>
        /// データ監視間隔のComboBox選択内容が変わった時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataCheckIntervalComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (DataCheckIntervalComboBox.SelectedIndex)
            {
                case 0:
                    DataCheckIntervalTextBox.Text = "100";
                    DataCheckIntervalTextBox.ReadOnly = true;
                    break;
                case 1:
                    DataCheckIntervalTextBox.Text = "500";
                    DataCheckIntervalTextBox.ReadOnly = true;
                    break;
                case 2:
                    DataCheckIntervalTextBox.Text = "1000";
                    DataCheckIntervalTextBox.ReadOnly = true;
                    break;
                case 3:
                    DataCheckIntervalTextBox.ReadOnly = false;
                    break;
            }
        }

        #region 言語設定
        private void SetLanguage()// 言語ファイル（xml）を読み込んで表示する処理
        {
            this.Text = LanguageSettingClass.GetOtherMessage("FormName", "MakeNewProject", LanguageFile);
            IEnumerable<XElement> buttonItemDataList = from item in LanguageFile.Elements("MakeNewProject").Elements("Button").Elements("item") select item;
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
            IEnumerable<XElement> labelItemDataList = from item in LanguageFile.Elements("MakeNewProject").Elements("Label").Elements("item") select item;
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
            IEnumerable<XElement> radioButtonItemDataList = from item in LanguageFile.Elements("MakeNewProject").Elements("RadioButton").Elements("item") select item;
            foreach (XElement itemData in radioButtonItemDataList)
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
            IEnumerable<XElement> checkBoxItemDataList = from item in LanguageFile.Elements("MakeNewProject").Elements("CheckBox").Elements("item") select item;
            foreach (XElement itemData in checkBoxItemDataList)
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
            // CompressTypeComboBoxの内容を読み込み
            CompressTypeComboBox.Items.Clear();
            CompressTypeComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("NoCompress", "MakeNewProject", LanguageFile));
            CompressTypeComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("Zip", "MakeNewProject", LanguageFile));
            // DataCheckIntervalComboBoxの内容を読み込み
            DataCheckIntervalComboBox.Items.Clear();
            DataCheckIntervalComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("LocalDataCheckInterval", "MakeNewProject", LanguageFile));
            DataCheckIntervalComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("FastServerDataCheckInterval", "MakeNewProject", LanguageFile));
            DataCheckIntervalComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("SlowServerDataCheckInterval", "MakeNewProject", LanguageFile));
            DataCheckIntervalComboBox.Items.Add(LanguageSettingClass.GetOtherMessage("CustomDataCheckInterval", "MakeNewProject", LanguageFile));
        }
        #endregion
    }
}