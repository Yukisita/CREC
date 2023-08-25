using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CoRectSys
{
    public partial class AddForm : Form
    {
        // 諸々変数の宣言
        string TargetFolderPath = ""; //データ保管場所
        string TargetContentsPath = ""; //詳細表示するデータのフォルダパス
        string TargetIndexPath = ""; //Indexの場所
        string TargetDetailsPath = ""; //説明txtのパス
        string[] cols;
        bool AllowEditID = false;//ID編集の可不可、デフォルトでは不可

        public AddForm(string str1,string str2)
        {
            InitializeComponent();
            TargetFolderPath = str1;
            TargetContentsPath = str2;
        }

        private void AddForm_Load(object sender, EventArgs e)
        {
            IDBox.TextChanged -= IDNumberBox_TextChanged; // ID重複確認イベントを一時停止
            // index読み込み
            TargetIndexPath = TargetContentsPath + "\\Edit_index.txt"; 
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
                            NameBox.Text = cols[1];
                            break;
                        case "ID":
                            IDBox.Text = cols[1];
                            break;
                        case "登録日":
                            RegistrationDateBox.Text = cols[1];
                            break;
                        case "カテゴリ":
                            CategoryBox.Text = cols[1];
                            break;
                        case "タグ1":
                            TagBox1.Text = cols[1];
                            break;
                        case "タグ2":
                            TagBox2.Text = cols[1];
                            break;
                        case "タグ3":
                            TagBox3.Text = cols[1];
                            break;
                        case "場所1(Real)":
                            RealLocationBox.Text = cols[1];
                            break;
                    }
                }
                TargetDetailsPath = (TargetContentsPath + "\\Edit_details.txt");
                StreamReader sr = null;
                try
                {
                    sr = new StreamReader(TargetDetailsPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "データの読み込みに失敗しました");
                }
                finally
                {
                    if (sr != null)
                    {
                        DetailsBox.Text = sr.ReadToEnd();
                        sr.Close();
                    }
                }
                IDBox.TextChanged += IDNumberBox_TextChanged; // ID重複確認イベントを再開
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Indexは新規作成されます。"); 
                IDBox.TextChanged += IDNumberBox_TextChanged; // ID重複確認イベントを再開
                // 現在時刻からIDを設定
                DateTime DT = DateTime.Now;
                IDBox.Text = DT.ToString("yyMMddHHmmssf");// ID設定
                RegistrationDateBox.Text = DT.ToString("yyyy/MM/dd");
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)// 保存して終了
        {
            TargetContentsPath = TargetFolderPath + "\\" + IDBox.Text;

            Directory.CreateDirectory(TargetContentsPath);
            Directory.CreateDirectory(TargetContentsPath+"\\data");
            Directory.CreateDirectory(TargetContentsPath+"\\pictures");
            StreamWriter Indexfile = new StreamWriter(TargetContentsPath + "\\Edit_index.txt", false, Encoding.GetEncoding("UTF-8"));
            Indexfile.WriteLine(string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}", "名称," + NameBox.Text, "ID," + IDBox.Text, "登録日," + RegistrationDateBox.Text, "カテゴリ," + CategoryBox.Text, "タグ1," + TagBox1.Text, "タグ2," + TagBox2.Text, "タグ3," + TagBox3.Text, "場所1(Real)," + RealLocationBox.Text));
            Indexfile.Close();
            StreamWriter Detailsfile = new StreamWriter(TargetContentsPath + "\\Edit_details.txt", false, Encoding.GetEncoding("UTF-8"));
            Detailsfile.WriteLine(string.Format("{0}", DetailsBox.Text));
            Detailsfile.Close();
            StreamWriter ConfidentialDataFile = new StreamWriter(TargetContentsPath + "\\Edit_confidentialdata.txt", false, Encoding.GetEncoding("UTF-8"));
            ConfidentialDataFile.WriteLine(string.Format("{0}", ConfidentialDataTextBox.Text));
            ConfidentialDataFile.Close();
            // 編集後のファイルを既存のものに上書き
            File.Copy(TargetContentsPath + "\\Edit_index.txt", TargetContentsPath + "\\index.txt", true);
            File.Copy(TargetContentsPath + "\\Edit_details.txt", TargetContentsPath + "\\details.txt", true);
            File.Copy(TargetContentsPath + "\\Edit_confidentialdata.txt", TargetContentsPath + "\\confidentialdata.txt", true);
            File.Delete(TargetContentsPath + "\\Edit_index.txt");
            File.Delete(TargetContentsPath + "\\Edit_details.txt");
            File.Delete(TargetContentsPath + "\\Edit_confidentialdata.txt");
            Close();
        }

        private void AddDataButton_Click(object sender, EventArgs e)// データ追加
        {
            TargetContentsPath = TargetFolderPath + "\\" + IDBox.Text;
            if (IDBox.Text.Length == 0)
            {
                MessageBox.Show("先にIDを入力してください。");
                return;
            }
            else
            {
                Directory.CreateDirectory(TargetContentsPath);
                Directory.CreateDirectory(TargetContentsPath + "\\data");
            }

            try
            {
                System.Diagnostics.Process.Start(TargetContentsPath + "\\data");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "フォルダを開けませんでした");
            }
        }

        private void AddPictureButton_Click(object sender, EventArgs e)// 画像追加
        {
            TargetContentsPath = TargetFolderPath + "\\" + IDBox.Text;
            if (IDBox.Text.Length == 0)
            {
                MessageBox.Show("先にIDを入力してください。");
                return;
            }
            else
            {
                Directory.CreateDirectory(TargetContentsPath);
                Directory.CreateDirectory(TargetContentsPath + "\\pictures");
            }

            try
            {
                System.Diagnostics.Process.Start(TargetContentsPath + "\\pictures");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "フォルダを開けませんでした");
            }
        }

        private void AddForm_Closing(object sender, CancelEventArgs e)// 終了時に変更が保存されていないときの確認
        {
            TargetContentsPath = TargetFolderPath + "\\" + IDBox.Text;
            if (File.Exists(TargetContentsPath + "\\Edit_index.txt"))
            {
                DialogResult CloseDialogResult = MessageBox.Show("Data is dirty. Close without saving?", "CoRectSys", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                if (CloseDialogResult == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (CloseDialogResult == DialogResult.OK)
                {
                    File.Delete(TargetContentsPath + "\\Edit_index.txt");
                    File.Delete(TargetContentsPath + "\\Edit_details.txt");
                }
            }
        }

        // メッソド類
        private void IDNumberBox_TextChanged(object sender, EventArgs e)// ID重複確認
        {
            if(System.IO.Directory.Exists(TargetFolderPath + "\\" + IDBox.Text))
            {
                MessageBox.Show("指定されたIDは使用済みです。");
            }
        }
        private void AllowChangeIDButton_Click(object sender, EventArgs e)// ID編集許可・不許可の切り替え
        {
            if (AllowEditID == false)
            {
                AllowEditID = true;
                AllowChangeIDButton.Text = "編集可";
                IDBox.ReadOnly = false;
            }
            else if (AllowEditID == true)
            {
                AllowEditID = false;
                AllowChangeIDButton.Text = "編集不可";
                IDBox.ReadOnly = true;
            }
        }
        private void EditConfidentialDataButton_Click(object sender, EventArgs e)// 機密情報の編集画面へ遷移
        {
            // 編集用にファイルをコピー
            try
            {
                File.Copy(TargetContentsPath + "\\confidentialdata.txt", TargetContentsPath + "\\Edit_confidentialdata.txt");
            }
            catch { }
            // 機密情報編集用TextBoxを表示し、他の入力フォームを非表示に
            NameLabel.Visible = false;
            NameBox.Visible = false;
            IDLabel.Visible = false;
            IDBox.Visible = false;
            AllowChangeIDButton.Visible = false;
            RegistrationDateLabel.Visible = false;
            RegistrationDateBox.Visible = false;
            CategoryLabel.Visible = false;
            CategoryBox.Visible = false;
            TagLabel.Visible = false;
            TagBox1.Visible = false;
            TagBox2.Visible = false;
            TagBox3.Visible = false;
            RealLocationLabel.Visible = false;
            RealLocationBox.Visible = false;
            ConfidentialDataLabel.Visible = false;
            EditConfidentialDataButton.Visible = false;
            detailsLabel.Visible = false;
            DetailsBox.Visible = false;
            AddDataButton.Visible = false;
            AddPictureButton.Visible = false;
            SaveButton.Visible = false;
            ConfidentialDataTextBox.Visible = true;
            ECDModeLabel.Visible = true;
            CloseECDModeButoon.Visible = true;
            // 機密情報を読み込んで表示
            string TargetConfidentialDataPath = ""; //編集する機密データのパス
            TargetConfidentialDataPath = (TargetContentsPath + "\\Edit_confidentialdata.txt");
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(TargetConfidentialDataPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "データの読み込みに失敗しました");
            }
            finally
            {
                if (sr != null)
                {
                    ConfidentialDataTextBox.Text = sr.ReadToEnd();
                    sr.Close();
                }
            }
        }

        private void CloseECDModeButoon_Click(object sender, EventArgs e)
        {
            // 機密情報編集用TextBoxを非表示し、他の入力フォームを再表示
            NameLabel.Visible = true;
            NameBox.Visible = true;
            IDLabel.Visible = true;
            IDBox.Visible = true;
            AllowChangeIDButton.Visible = true;
            RegistrationDateLabel.Visible = true;
            RegistrationDateBox.Visible = true;
            CategoryLabel.Visible = true;
            CategoryBox.Visible = true;
            TagLabel.Visible = true;
            TagBox1.Visible = true;
            TagBox2.Visible = true;
            TagBox3.Visible = true;
            RealLocationLabel.Visible = true;
            RealLocationBox.Visible = true;
            ConfidentialDataLabel.Visible = true;
            EditConfidentialDataButton.Visible = true;
            detailsLabel.Visible = true;
            DetailsBox.Visible = true;
            AddDataButton.Visible = true;
            AddPictureButton.Visible = true;
            SaveButton.Visible = true;
            ConfidentialDataTextBox.Visible = false;
            ECDModeLabel.Visible = false;
            CloseECDModeButoon.Visible = false;
        }
    }
}
