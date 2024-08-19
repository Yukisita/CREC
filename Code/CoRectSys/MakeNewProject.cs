/*
MakeNewProjectForm
Copyright (c) [2022-2024] [S.Yukisita]
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
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
        string TargetCRECPath = "";//管理ファイル（.crec）のファイルパス
        string[] cols;// List等読み込み用
        string ColorSetting = "Blue";
        string CurrentLanguageFileName = "";// 言語設定
        public string ReturnTargetProject { get; private set; } = "";
        public MakeNewProject(string tmp, string colorSetting, string currentLanguage)
        {
            InitializeComponent();
            TargetCRECPath = tmp;
            ColorSetting = colorSetting;
            CurrentLanguageFileName = currentLanguage;
            SetColorMethod();
        }
        private void MakeNewProject_Load(object sender, EventArgs e)// 既存の.crecを読み込み or 新規作成
        {
            // 言語設定
            SetLanguage("language\\" + CurrentLanguageFileName + ".xml");
            // 必要な項目を初期化
            CompressTypeComboBox.SelectedIndex = 0;
            if (TargetCRECPath.Length > 0)// 編集の場合は既存の.crecを読み込み
            {
                label1.Text = "入力されたパスをプロジェクトフォルダとして設定します。";
                IEnumerable<string> tmp = null;
                tmp = File.ReadLines(TargetCRECPath, Encoding.GetEncoding("UTF-8"));
                foreach (string line in tmp)
                {
                    cols = line.Split(',');
                    switch (cols[0])
                    {
                        case "projectname":
                            EditProjectNameTextBox.Text = cols[1];
                            break;
                        case "projectlocation":
                            EditProjectLocationTextBox.Text = cols[1];
                            break;
                        case "backuplocation":
                            if (cols[1] == "null")
                            {
                                EditBackupLocationTextBox.Text = "";
                            }
                            else
                            {
                                EditBackupLocationTextBox.Text = cols[1];
                            }
                            break;
                        case "autobackup":
                            if (cols[1].Contains("S"))
                            {
                                StartUpBackUpCheckBox.Checked = true;
                            }
                            else
                            {
                                StartUpBackUpCheckBox.Checked = false;
                            }
                            if (cols[1].Contains("C"))
                            {
                                CloseBackUpCheckBox.Checked = true;
                            }
                            else
                            {
                                CloseBackUpCheckBox.Checked = false;
                            }
                            if (cols[1].Contains("E"))
                            {
                                EditedBackUpCheckBox.Checked = true;
                            }
                            else
                            {
                                EditedBackUpCheckBox.Checked = false;
                            }
                            break;
                        case "CompressType":
                            try
                            {
                                CompressTypeComboBox.SelectedIndex = Convert.ToInt32(cols[1]);
                            }
                            catch (Exception ex)
                            {
                                System.Windows.Forms.MessageBox.Show(ex.Message);
                                CompressTypeComboBox.SelectedIndex = 1;
                            }
                            break;
                        case "Listoutputlocation":
                            if (cols[1] == "null")
                            {
                                EditListOutputLocationTextBox.Text = "";
                            }
                            else
                            {
                                EditListOutputLocationTextBox.Text = cols[1];
                            }
                            break;
                        case "autoListoutput":
                            if (cols[1].Contains("S"))
                            {
                                StartUpListOutputCheckBox.Checked = true;
                            }
                            else
                            {
                                StartUpListOutputCheckBox.Checked = false;
                            }
                            if (cols[1].Contains("C"))
                            {
                                CloseListOutputCheckBox.Checked = true;
                            }
                            else
                            {
                                CloseListOutputCheckBox.Checked = false;
                            }
                            if (cols[1].Contains("E"))
                            {
                                EditedListOutputCheckBox.Checked = true;
                            }
                            else
                            {
                                EditedListOutputCheckBox.Checked = false;
                            }
                            break;
                        case "openListafteroutput":
                            if (cols[1].Contains("O"))
                            {
                                OpenListAfterOutputCheckBox.Checked = true;
                            }
                            else
                            {
                                OpenListAfterOutputCheckBox.Checked = false;
                            }
                            break;
                        case "ListOutputFormat":
                            if (cols[1] == "CSV")
                            {
                                CSVOutputRadioButton.Checked = true;
                            }
                            else if (cols[1] == "TSV")
                            {
                                TSVOutputRadioButton.Checked = true;
                            }
                            break;
                        case "created":
                            break;
                        case "modified":
                            break;
                        case "accessed":
                            break;
                        case "ShowObjectNameLabel":
                            try
                            {
                                if (cols[1].Length > 0)
                                {
                                    EditObjectNameLabelTextBox.Text = cols[1];
                                }
                                else
                                {
                                    EditObjectNameLabelTextBox.Text = "名称";
                                }
                                if (cols[2] == "f")
                                {
                                    ShowObjectNameLabelVisibleCheckBox.Checked = false;
                                }
                                else
                                {
                                    ShowObjectNameLabelVisibleCheckBox.Checked = true;
                                }
                            }
                            catch
                            {
                                ShowObjectNameLabelVisibleCheckBox.Checked = true;
                            }
                            break;
                        case "ShowIDLabel":
                            try
                            {
                                if (cols[1].Length > 0)
                                {
                                    EditUUIDLabelTextBox.Text = cols[1];
                                }
                                else
                                {
                                    EditUUIDLabelTextBox.Text = "ID";
                                }
                                if (cols[2] == "f")
                                {
                                    ShowIDLabelVisibleCheckBox.Checked = false;
                                }
                                else
                                {
                                    ShowIDLabelVisibleCheckBox.Checked = true;
                                }
                            }
                            catch
                            {
                                ShowIDLabelVisibleCheckBox.Checked = true;
                            }
                            break;
                        case "ShowMCLabel":
                            try
                            {
                                if (cols[1].Length > 0)
                                {
                                    EditMCLabelTextBox.Text = cols[1];
                                }
                                else
                                {
                                    EditMCLabelTextBox.Text = "管理コード";
                                }
                                if (cols[2] == "f")
                                {
                                    ShowMCLabelVisibleCheckBox.Checked = false;
                                }
                                else
                                {
                                    ShowMCLabelVisibleCheckBox.Checked = true;
                                }
                            }
                            catch
                            {
                                ShowMCLabelVisibleCheckBox.Checked = true;
                            }
                            break;
                        case "AutoMCFill":
                            try
                            {
                                if (cols[1] == "f")
                                {
                                    AutoMCFillCheckBox.Checked = false;
                                }
                                else
                                {
                                    AutoMCFillCheckBox.Checked = true;
                                }
                            }
                            catch
                            {
                                AutoMCFillCheckBox.Checked = true;
                            }
                            break;
                        case "ShowRegistrationDateLabel":
                            try
                            {
                                if (cols[1].Length > 0)
                                {
                                    EditRegistrationDateLabelTextBox.Text = cols[1];
                                }
                                else
                                {
                                    EditRegistrationDateLabelTextBox.Text = "登録日";
                                }
                                if (cols[2] == "f")
                                {
                                    ShowRegistrationDateLabelVisibleCheckBox.Checked = false;
                                }
                                else
                                {
                                    ShowRegistrationDateLabelVisibleCheckBox.Checked = true;
                                }
                            }
                            catch
                            {
                                ShowRegistrationDateLabelVisibleCheckBox.Checked = true;
                            }
                            break;
                        case "ShowCategoryLabel":
                            try
                            {
                                if (cols[1].Length > 0)
                                {
                                    EditCategoryLabelTextBox.Text = cols[1];
                                }
                                else
                                {
                                    EditCategoryLabelTextBox.Text = "カテゴリ";
                                }
                                if (cols[2] == "f")
                                {
                                    ShowCategoryLabelVisibleCheckBox.Checked = false;
                                }
                                else
                                {
                                    ShowCategoryLabelVisibleCheckBox.Checked = true;
                                }
                            }
                            catch
                            {
                                ShowCategoryLabelVisibleCheckBox.Checked = true;
                            }
                            break;
                        case "Tag1Name":
                            try
                            {
                                if (cols[1].Length > 0)
                                {
                                    EditTag1NameTextBox.Text = cols[1];
                                }
                                else
                                {
                                    EditTag1NameTextBox.Text = "タグ１";
                                }
                                if (cols[2] == "f")
                                {
                                    Tag1NameVisibleCheckBox.Checked = false;
                                }
                                else
                                {
                                    Tag1NameVisibleCheckBox.Checked = true;
                                }
                            }
                            catch
                            {
                                Tag1NameVisibleCheckBox.Checked = true;
                            }
                            break;
                        case "Tag2Name":
                            try
                            {
                                if (cols[1].Length > 0)
                                {
                                    EditTag2NameTextBox.Text = cols[1];
                                }
                                else
                                {
                                    EditTag2NameTextBox.Text = "タグ２";
                                }
                                if (cols[2] == "f")
                                {
                                    Tag2NameVisibleCheckBox.Checked = false;
                                }
                                else
                                {
                                    Tag2NameVisibleCheckBox.Checked = true;
                                }
                            }
                            catch
                            {
                                Tag2NameVisibleCheckBox.Checked = true;
                            }
                            break;
                        case "Tag3Name":
                            try
                            {
                                if (cols[1].Length > 0)
                                {
                                    EditTag3NameTextBox.Text = cols[1];
                                }
                                else
                                {
                                    EditTag3NameTextBox.Text = "タグ３";
                                }
                                if (cols[2] == "f")
                                {
                                    Tag3NameVisibleCheckBox.Checked = false;
                                }
                                else
                                {
                                    Tag3NameVisibleCheckBox.Checked = true;
                                }
                            }
                            catch
                            {
                                Tag3NameVisibleCheckBox.Checked = true;
                            }
                            break;
                        case "ShowRealLocationLabel":
                            try
                            {
                                if (cols[1].Length > 0)
                                {
                                    EditRealLocationLabelTextBox.Text = cols[1];
                                }
                                else
                                {
                                    EditRealLocationLabelTextBox.Text = "現物保管場所";
                                }
                                if (cols[2] == "f")
                                {
                                    ShowRealLocationLabelVisibleCheckBox.Checked = false;
                                }
                                else
                                {
                                    ShowRealLocationLabelVisibleCheckBox.Checked = true;
                                }
                            }
                            catch
                            {
                                ShowRealLocationLabelVisibleCheckBox.Checked = true;
                            }
                            break;
                        case "ShowDataLocationLabel":
                            try
                            {
                                if (cols[1].Length > 0)
                                {
                                    EditDataLocationLabelTextBox.Text = cols[1];
                                }
                                else
                                {
                                    EditDataLocationLabelTextBox.Text = "データ保管場所";
                                }
                                if (cols[2] == "f")
                                {
                                    ShowDataLocationLabelVisibleCheckBox.Checked = false;
                                }
                                else
                                {
                                    ShowDataLocationLabelVisibleCheckBox.Checked = true;
                                }
                            }
                            catch
                            {
                                ShowDataLocationLabelVisibleCheckBox.Checked = true;
                            }
                            break;
                    }
                }

            }
            else// 追加の場合はデフォルト値を各ラベルから取得してTextBoxに入れる
            {
                EditObjectNameLabelTextBox.Text = ObjectNameLabel.Text;
                EditUUIDLabelTextBox.Text = UUIDLabel.Text;
                EditMCLabelTextBox.Text = MCLabel.Text;
                EditRegistrationDateLabelTextBox.Text = RegistrationDateLabel.Text;
                EditCategoryLabelTextBox.Text = CategoryLabel.Text;
                EditTag1NameTextBox.Text = Tag1NameLabel.Text;
                EditTag2NameTextBox.Text = Tag2NameLabel.Text;
                EditTag3NameTextBox.Text = Tag3NameLabel.Text;
                EditRealLocationLabelTextBox.Text = RealLocationLabel.Text;
                EditDataLocationLabelTextBox.Text = DataLocationLabel.Text;
            }
        }
        private void MakeNewProjectButton_Click(object sender, EventArgs e)// 保存してプロジェクト編集画面を閉じる
        {
            // 内容を確認
            int error = 0;// 入力内容に不備がない場合は0、不備を発見した場合は1に変更
            if (EditProjectNameTextBox.TextLength == 0)
            {
                error = 1;
                MessageBox.Show("プロジェクト名を空欄にすることはできません。", "CREC");
            }
            if (EditProjectLocationTextBox.TextLength == 0)
            {
                error = 1;
                MessageBox.Show("プロジェクトの作成場所を空欄にすることはできません。", "CREC");
            }

            if (error == 0)// 記入内容に問題がなかった場合は.crsファイルを作成
            {
                // プロジェクトデータ保管場所が存在するか判定し、作成
                if (TargetCRECPath.Length == 0)// 新規プロジェクト作成の場合
                {
                    if (Directory.Exists(EditProjectLocationTextBox.Text))
                    {
                        MessageBox.Show("指定されたプロジェクトフォルダは既に存在しています。", "CREC");
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
                        MessageBox.Show("指定されたプロジェクトフォルダが見つかりました。", "CREC");
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
                        MessageBox.Show("指定されたバックアップフォルダは既に存在しています。", "CREC");
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
                if (TargetCRECPath.Length > 0)// 編集時は既存の.crecを削除
                {
                    try
                    {
                        File.Delete(TargetCRECPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("既存の管理ファイルの削除に失敗しました。\n新規に管理ファイルを作成します。\n" + ex.Message, "CREC");
                    }
                }
                // crec出力先の設定
                if (TargetCRECPath.Length > 0)
                {

                }
                else
                {
                    TargetCRECPath = System.Environment.CurrentDirectory + "\\" + EditProjectNameTextBox.Text + ".crec";
                }
                StreamWriter sw = new StreamWriter(TargetCRECPath, false, Encoding.GetEncoding("UTF-8"));

                sw.WriteLine("{0},{1}\n{2},{3}", "projectname", EditProjectNameTextBox.Text, "projectlocation", EditProjectLocationTextBox.Text);
                // バックアップ場所の設定
                if (EditBackupLocationTextBox.Text.Length == 0)
                {
                    sw.WriteLine("{0},{1}", "backuplocation", "null");
                }
                else
                {
                    sw.WriteLine("{0},{1}", "backuplocation", EditBackupLocationTextBox.Text);
                }
                // 自動バックアップの設定
                sw.Write("autobackup,");
                if (StartUpBackUpCheckBox.Checked == true)
                {
                    sw.Write("S");
                }
                if (CloseBackUpCheckBox.Checked == true)
                {
                    sw.Write("C");
                }
                if (EditedBackUpCheckBox.Checked == true)
                {
                    sw.Write("E");
                }
                sw.Write('\n');
                // 圧縮方法
                sw.Write("CompressType,");
                sw.Write(Convert.ToString(CompressTypeComboBox.SelectedIndex));
                sw.Write('\n');
                // 一覧出力先の設定
                if (EditListOutputLocationTextBox.Text.Length == 0)
                {
                    sw.WriteLine("{0},{1}", "Listoutputlocation", "null");
                }
                else
                {
                    sw.WriteLine("{0},{1}", "Listoutputlocation", EditListOutputLocationTextBox.Text);
                }
                // 自動一覧出力の設定
                sw.Write("autoListoutput,");
                if (StartUpListOutputCheckBox.Checked == true)
                {
                    sw.Write("S");
                }
                if (CloseListOutputCheckBox.Checked == true)
                {
                    sw.Write("C");
                }
                if (EditedListOutputCheckBox.Checked == true)
                {
                    sw.Write("E");
                }
                sw.Write('\n');
                sw.Write("openListafteroutput,");
                if (OpenListAfterOutputCheckBox.Checked == true)
                {
                    sw.Write("O");
                }
                sw.Write('\n');
                sw.Write("ListOutputFormat,");
                if (CSVOutputRadioButton.Checked)
                {
                    sw.Write("CSV");
                }
                else if (TSVOutputRadioButton.Checked)
                {
                    sw.Write("TSV");
                }
                sw.Write('\n');
                // 現在時刻を取得 
                DateTime DT = DateTime.Now;
                sw.WriteLine("{0},{1}\n{2},{3}\n{4},{5}", "created", DT.ToString("yyyy/MM/dd hh:mm:ss"), "modified", DT.ToString("yyyy/MM/dd hh:mm:ss"), "accesssed", DT.ToString("yyyy/MM/dd hh:mm:ss"));
                // 各ラベルの表示名設定を書き込み
                if (EditObjectNameLabelTextBox.Text.Length > 0)
                {
                    sw.Write("{0},{1},", "ShowObjectNameLabel", EditObjectNameLabelTextBox.Text);
                }
                else
                {
                    sw.Write("{0},{1},", "ShowObjectNameLabel", ObjectNameLabel.Text);
                }
                if (ShowObjectNameLabelVisibleCheckBox.Checked == true)
                {
                    sw.Write("t\n");
                }
                else if (ShowObjectNameLabelVisibleCheckBox.Checked == false)
                {
                    sw.Write("f\n");
                }
                if (EditUUIDLabelTextBox.Text.Length > 0)
                {
                    sw.Write("{0},{1},", "ShowIDLabel", EditUUIDLabelTextBox.Text);
                }
                else
                {
                    sw.Write("{0},{1},", "ShowIDLabel", UUIDLabel.Text);
                }
                if (ShowIDLabelVisibleCheckBox.Checked == true)
                {
                    sw.Write("t\n");
                }
                else if (ShowIDLabelVisibleCheckBox.Checked == false)
                {
                    sw.Write("f\n");
                }
                if (EditMCLabelTextBox.Text.Length > 0)
                {
                    sw.Write("{0},{1},", "ShowMCLabel", EditMCLabelTextBox.Text);
                }
                else
                {
                    sw.Write("{0},{1},", "ShowMCLabel", MCLabel.Text);
                }
                if (ShowMCLabelVisibleCheckBox.Checked == true)
                {
                    sw.Write("t\n");
                }
                else if (ShowMCLabelVisibleCheckBox.Checked == false)
                {
                    sw.Write("f\n");
                }
                if (AutoMCFillCheckBox.Checked == true)
                {
                    sw.Write("AutoMCFill,t\n");
                }
                else
                {
                    sw.Write("AutoMCFill,f\n");
                }
                if (EditRegistrationDateLabelTextBox.Text.Length > 0)
                {
                    sw.Write("{0},{1},", "ShowRegistrationDateLabel", EditRegistrationDateLabelTextBox.Text);
                }
                else
                {
                    sw.Write("{0},{1},", "ShowRegistrationDateLabel", RegistrationDateLabel.Text);
                }
                if (ShowRegistrationDateLabelVisibleCheckBox.Checked == true)
                {
                    sw.Write("t\n");
                }
                else if (ShowRegistrationDateLabelVisibleCheckBox.Checked == false)
                {
                    sw.Write("f\n");
                }
                if (EditCategoryLabelTextBox.Text.Length > 0)
                {
                    sw.Write("{0},{1},", "ShowCategoryLabel", EditCategoryLabelTextBox.Text);
                }
                else
                {
                    sw.Write("{0},{1},", "ShowCategoryLabel", CategoryLabel.Text);
                }
                if (ShowCategoryLabelVisibleCheckBox.Checked == true)
                {
                    sw.Write("t\n");
                }
                else if (ShowCategoryLabelVisibleCheckBox.Checked == false)
                {
                    sw.Write("f\n");
                }
                if (EditTag1NameTextBox.Text.Length > 0)
                {
                    sw.Write("{0},{1},", "Tag1Name", EditTag1NameTextBox.Text);
                }
                else
                {
                    sw.Write("{0},{1},", "Tag1Name", Tag1NameLabel.Text);
                }
                if (Tag1NameVisibleCheckBox.Checked == true)
                {
                    sw.Write("t\n");
                }
                else if (Tag1NameVisibleCheckBox.Checked == false)
                {
                    sw.Write("f\n");
                }
                if (EditTag2NameTextBox.Text.Length > 0)
                {
                    sw.Write("{0},{1},", "Tag2Name", EditTag2NameTextBox.Text);
                }
                else
                {
                    sw.Write("{0},{1},", "Tag2Name", Tag2NameLabel.Text);
                }
                if (Tag2NameVisibleCheckBox.Checked == true)
                {
                    sw.Write("t\n");
                }
                else if (Tag2NameVisibleCheckBox.Checked == false)
                {
                    sw.Write("f\n");
                }
                if (EditTag3NameTextBox.Text.Length > 0)
                {
                    sw.Write("{0},{1},", "Tag3Name", EditTag3NameTextBox.Text);
                }
                else
                {
                    sw.Write("{0},{1},", "Tag3Name", Tag3NameLabel.Text);
                }
                if (Tag3NameVisibleCheckBox.Checked == true)
                {
                    sw.Write("t\n");
                }
                else if (Tag3NameVisibleCheckBox.Checked == false)
                {
                    sw.Write("f\n");
                }
                if (EditRealLocationLabelTextBox.Text.Length > 0)
                {
                    sw.Write("{0},{1},", "ShowRealLocationLabel", EditRealLocationLabelTextBox.Text);
                }
                else
                {
                    sw.Write("{0},{1},", "ShowRealLocationLabel", RealLocationLabel.Text);
                }
                if (ShowRealLocationLabelVisibleCheckBox.Checked == true)
                {
                    sw.Write("t\n");
                }
                else if (ShowRealLocationLabelVisibleCheckBox.Checked == false)
                {
                    sw.Write("f\n");
                }
                if (EditDataLocationLabelTextBox.Text.Length > 0)
                {
                    sw.Write("{0},{1},", "ShowDataLocationLabel", EditDataLocationLabelTextBox.Text);
                }
                else
                {
                    sw.Write("{0},{1},", "ShowDataLocationLabel", DataLocationLabel.Text);
                }
                if (ShowDataLocationLabelVisibleCheckBox.Checked == true)
                {
                    sw.Write("t\n");
                }
                else if (ShowDataLocationLabelVisibleCheckBox.Checked == false)
                {
                    sw.Write("f\n");
                }
                sw.Close();
                ReturnTargetProject = TargetCRECPath;
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
                if (TargetCRECPath.Length == 0)
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
            switch (ColorSetting)
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
                    ColorSetting = "Blue";
                    break;
            }
        }

        #region 言語設定
        private void SetLanguage(string targetLanguageFilePath)// 言語ファイル（xml）を読み込んで表示する処理
        {
            this.Text = LanguageSettingClass.GetOtherMessage("FormName", "MakeNewProject", CurrentLanguageFileName);
            XElement xElement = XElement.Load(targetLanguageFilePath);
            IEnumerable<XElement> buttonItemDataList = from item in xElement.Elements("MakeNewProject").Elements("Button").Elements("item") select item;
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
            IEnumerable<XElement> labelItemDataList = from item in xElement.Elements("MakeNewProject").Elements("Label").Elements("item") select item;
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
            IEnumerable<XElement> radioButtonItemDataList = from item in xElement.Elements("MakeNewProject").Elements("RadioButton").Elements("item") select item;
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
            IEnumerable<XElement> checkBoxItemDataList = from item in xElement.Elements("MakeNewProject").Elements("CheckBox").Elements("item") select item;
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
        }
        #endregion

        private void EditUUIDLabelTextBox_Click(object sender, EventArgs e)// UUIDのラベルを変更可能に切り替える
        {
            System.Windows.MessageBoxResult result = System.Windows.MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("AskChangeUUIDLabel", "MakeNewProject", CurrentLanguageFileName), "CREC", System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Warning);
            if (result == System.Windows.MessageBoxResult.Yes)// 注意事項を確認してOKだった場合は、編集を許可する
            {
                EditUUIDLabelTextBox.ReadOnly = false;
            }
        }
    }
}