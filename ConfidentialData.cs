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
    public partial class ConfidentialData : Form
    {
        string TargetContentsPath = ""; //詳細表示するデータのフォルダパス
        public ConfidentialData(string str1)
        {
            InitializeComponent();
            TargetContentsPath = str1;
        }

        private void ConfidentialData_Load(object sender, EventArgs e)
        {
            try
            {
                StreamReader streamreader = new StreamReader(TargetContentsPath + "\\confidentialdata.txt");
                ConfidentialDataTextBox.Text = streamreader.ReadToEnd();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "データの読み込みに失敗しました");
            }
        }
    }
}
