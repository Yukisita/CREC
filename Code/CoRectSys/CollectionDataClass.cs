/*
Program
Copyright (c) [2022-2025] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml.Linq;

namespace CREC
{
    public class CollectionDataValuesClass
    {
        private string collectionFolderPath = string.Empty;
        /// <summary>
        /// CollectionFolderPath: Collection folder path
        /// </summary>
        public string CollectionFolderPath
        {
            get
            {
                return collectionFolderPath;
            }
            set
            {
                collectionFolderPath = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionFolderPath.Length == 0)
                {
                    collectionFolderPath = " - ";
                }
            }
        }

        private string collectionName = string.Empty;
        /// <summary>
        /// CollectionName: Collection name
        /// </summary>
        public string CollectionName
        {
            get
            {
                return collectionName;
            }
            set
            {
                collectionName = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionName.Length == 0)
                {
                    collectionName = " - ";
                }
            }
        }

        private string collectionID = string.Empty;
        /// <summary>
        /// CollectionID: Collection ID
        /// </summary>
        public string CollectionID
        {
            get
            {
                return collectionID;
            }
            set
            {
                collectionID = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionID.Length == 0)
                {
                    collectionID = " - ";
                }
            }
        }

        private string collectionMC = string.Empty;
        /// <summary>
        /// CollectionMC: Collection management code
        /// </summary>
        public string CollectionMC
        {
            get
            {
                return collectionMC;
            }
            set
            {
                collectionMC = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionMC.Length == 0)
                {
                    collectionMC = " - ";
                }
            }
        }

        private string collectionRegistrationDate = string.Empty;
        /// <summary>
        /// CollectionRegistrationDate: Collection registration date
        /// </summary>
        public string CollectionRegistrationDate
        {
            get
            {
                return collectionRegistrationDate;
            }
            set
            {
                collectionRegistrationDate = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionRegistrationDate.Length == 0)
                {
                    collectionRegistrationDate = " - ";
                }
            }
        }

        private string collectionCategory = string.Empty;
        /// <summary>
        /// CollectionType: Collection type
        /// </summary>
        public string CollectionCategory
        {
            get
            {
                return collectionCategory;
            }
            set
            {
                collectionCategory = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionCategory.Length == 0)
                {
                    collectionCategory = " - ";
                }
            }
        }

        private string collectionTag1 = string.Empty;
        /// <summary>
        /// CollectionTag1: Collection tag1
        /// </summary>
        public string CollectionTag1
        {
            get
            {
                return collectionTag1;
            }
            set
            {
                collectionTag1 = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionTag1.Length == 0)
                {
                    collectionTag1 = " - ";
                }
            }
        }

        private string collectionTag2 = string.Empty;
        /// <summary>
        /// CollectionTag2: Collection tag2
        /// </summary>
        public string CollectionTag2
        {
            get
            {
                return collectionTag2;
            }
            set
            {
                collectionTag2 = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionTag2.Length == 0)
                {
                    collectionTag2 = " - ";
                }
            }
        }

        private string collectionTag3 = string.Empty;
        /// <summary>
        /// CollectionTag3: Collection tag3
        /// </summary>
        public string CollectionTag3
        {
            get
            {
                return collectionTag3;
            }
            set
            {
                collectionTag3 = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionTag3.Length == 0)
                {
                    collectionTag3 = " - ";
                }
            }
        }

        private string collectionRealLocation = string.Empty;
        /// <summary>
        /// CollectionRealLocation: Collection real location
        /// </summary>
        public string CollectionRealLocation
        {
            get
            {
                return collectionRealLocation;
            }
            set
            {
                collectionRealLocation = value.Replace("\r", string.Empty).Replace("\n", string.Empty);
                if (collectionRealLocation.Length == 0)
                {
                    collectionRealLocation = " - ";
                }
            }
        }
    }

    public class CollectionDataClass
    {
        /// <summary>
        /// コレクションのデータ読み込み
        /// </summary>
        /// <param name="CollectionFolderPath">読み込み対象のコレクションのフォルダ</param>
        /// <param name="CollectionDataValues">読み込み対象のコレクションのデータ（参照渡し）</param>
        /// <param name="languageData">言語データ</param>
        /// <returns>読み込んだコレクションのデータ</returns>
        public static bool LoadCollectionIndexData(string CollectionFolderPath, ref CollectionDataValuesClass CollectionDataValues, XElement languageData)
        {
            try
            {
                string CollectionDataFilePath = CollectionFolderPath + @"\index.txt";
                if (!System.IO.File.Exists(CollectionDataFilePath))
                {
                    if (!CollectionIndexRecovery_IndexFileNotFound(CollectionFolderPath, languageData))
                    {
                        return false;
                    }
                }

                string[] CollectionDataLines = System.IO.File.ReadAllLines(CollectionDataFilePath, Encoding.GetEncoding("UTF-8"));
                foreach (string line in CollectionDataLines)
                {
                    string[] CollectionDataLineSplit = line.Split(',');
                    if (CollectionDataLineSplit.Length < 2)
                    {
                        return false;
                    }

                    switch (CollectionDataLineSplit[0])
                    {
                        // 後方互換用になる予定
                        case "名称":
                            CollectionDataValues.CollectionName = line.Substring(3);
                            break;
                        case "ID":
                            CollectionDataValues.CollectionID = line.Substring(3);
                            break;
                        case "MC":
                            CollectionDataValues.CollectionMC = line.Substring(3);
                            break;
                        case "登録日":
                            CollectionDataValues.CollectionRegistrationDate = line.Substring(4);
                            break;
                        case "カテゴリ":
                            CollectionDataValues.CollectionCategory = line.Substring(5);
                            break;
                        case "タグ1":
                            CollectionDataValues.CollectionTag1 = line.Substring(4);
                            break;
                        case "タグ2":
                            CollectionDataValues.CollectionTag2 = line.Substring(4);
                            break;
                        case "タグ3":
                            CollectionDataValues.CollectionTag3 = line.Substring(4);
                            break;
                        case "場所1(Real)":
                            CollectionDataValues.CollectionRealLocation = CollectionDataLineSplit[1];
                            break;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("IndexFileReadFailed", "CollectionDataClass", languageData) + ex.Message, "CREC");
                return CollectionIndexRecovery_IndexFileNotFound(CollectionFolderPath, languageData);
            }
        }

        /// <summary>
        /// コレクションのIndexファイルが見つからない場合の処理
        /// </summary>
        /// <param name="CollectionFolderPath"></param>
        /// <param name="languageData">言語データ</param>
        /// <returns></returns>
        private static bool CollectionIndexRecovery_IndexFileNotFound(string CollectionFolderPath, XElement languageData)
        {
            try
            {
                // バックアップデータを探索
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("IndexFileRestoredFromBackupData", "CollectionDataClass", languageData), "CREC");
                if (System.IO.File.Exists(CollectionFolderPath + @"\index_old.txt"))
                {
                    // バックアップデータから復元
                    try
                    {
                        File.Copy(CollectionFolderPath + "\\index_old.txt", CollectionFolderPath + "\\index.txt", true);
                        MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("IndexFileRestoredFromBackupDataSuccessed", "CollectionDataClass", languageData), "CREC");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("IndexFileRestoredFromBackupDataFailed", "CollectionDataClass", languageData) + ex.Message, "CREC");
                    }
                }
                else
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("IndexFileRestoredFromBackupDataBackupDataNotFound", "CollectionDataClass", languageData), "CREC");
                }
                // Indexを再生成する
                CollectionDataValuesClass RecoveryCollectionDataValues = new CollectionDataValuesClass();
                RecoveryCollectionDataValues.CollectionID = new DirectoryInfo(CollectionFolderPath).Name; // IDはフォルダ名
                SaveCollectionIndexData(CollectionFolderPath, RecoveryCollectionDataValues, languageData);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("IndexFileRestoreFailed", "CollectionDataClass", languageData) + ex.Message, "CREC");
                return false;
            }
        }

        /// <summary>
        /// コレクションのデータ保存
        /// </summary>
        /// <param name="CollectionFolderPath">保存対象のコレクションのフォルダ</param>
        /// <param name="CollectionDataValues">保存対象のコレクションのデータ</param>
        /// <param name="languageData">言語データ</param>
        /// <returns>成功したらtrue</returns>
        public static bool SaveCollectionIndexData(string CollectionFolderPath, CollectionDataValuesClass CollectionDataValues, XElement languageData)
        {
            try
            {
                // 現在の値をIndexデータに保存
                StreamWriter Indexfile = new StreamWriter(CollectionFolderPath + "\\index.txt", false, Encoding.GetEncoding("UTF-8"));
                Indexfile.WriteLine(string.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}\n{6}\n{7}\n{8}",
                    "名称," + CollectionDataValues.CollectionName,
                    "ID," + CollectionDataValues.CollectionID,
                    "MC," + CollectionDataValues.CollectionMC,
                    "登録日," + CollectionDataValues.CollectionRegistrationDate,
                    "カテゴリ," + CollectionDataValues.CollectionCategory,
                    "タグ1," + CollectionDataValues.CollectionTag1,
                    "タグ2," + CollectionDataValues.CollectionTag2,
                    "タグ3," + CollectionDataValues.CollectionTag3,
                    "場所1(Real)," + CollectionDataValues.CollectionRealLocation));
                Indexfile.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("IndexFileSaveFailed", "CollectionDataClass", languageData) + ex.Message, "CREC");
                return false;
            }
        }

        /// <summary>
        /// コレクションIndexのデータバックアップ
        /// </summary>
        /// <param name="CollectionFolderPath">コレクションのフォルダパス</param>
        /// <param name="CollectionDataValues">コレクションのデータ</param>
        /// <param name="languageData">言語データ</param>
        /// <returns>バックアップ成功若しくはバックアップ対象なしの場合はTrue</returns>
        public static bool BackupCollectionIndexData(string CollectionFolderPath, CollectionDataValuesClass CollectionDataValues, XElement languageData)
        {
            string CollectionDataFilePath = CollectionFolderPath + @"\index.txt";
            if (System.IO.File.Exists(CollectionDataFilePath))// Indexが存在する場合はバックアップを作成
            {
                try
                {
                    File.Copy(CollectionFolderPath + "\\index.txt", CollectionFolderPath + "\\index_old.txt", true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(LanguageSettingClass.GetMessageBoxMessage("IndexFileBackupFailed", "CollectionDataClass", languageData) + ex.Message, "CREC");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// サムネイルを選択して生成
        /// </summary>
        /// <param name="CollectionFolderPath">コレクションのフォルダパス</param>
        /// <param name="languageData">言語データ</param>
        /// <returns>サムネイル生成に成功したらTrue、その他False</returns>
        public static bool MakeTumbnailPicture(string CollectionFolderPath, XElement languageData)
        {
            // サムネイルとする画像を選択
            OpenFileDialog openFolderDialog = new OpenFileDialog();
            openFolderDialog.InitialDirectory = CollectionFolderPath + "\\pictures";
            openFolderDialog.Title = LanguageSettingClass.GetOtherMessage("SelectThumbnail", "CollectionDataClass", languageData);
            openFolderDialog.Filter = "JPEG|*.jpg;*.jpeg;*.jfif;*.jpe" + "|PNG|*.png" + "|GIF|*.gif" + "|ICO|*.ico";
            if (openFolderDialog.ShowDialog() == DialogResult.OK)// 画像読み込み成功
            {
                try
                {
                    // 画像最大サイズを400x400に設定
                    int TargetWidth = 400;
                    int TargetHeight = 400;
                    Bitmap TargetBitmap = new Bitmap(openFolderDialog.FileName);
                    // 画像サイズ設定
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
                        if (imageCodecInfo.FormatID == ImageFormat.Png.Guid)
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
                    ConvertedBitmap.Save(CollectionFolderPath + "\\SystemData\\NewThumbnail.png", imageCodecInfos, encoderParameters);
                    ConvertedBitmap.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(LanguageSettingClass.GetOtherMessage("ThumbnailSetFaild", "CollectionDataClass", languageData) + ex.Message, "CREC");
                    return false;
                }
            }
            else// 画像が選択されなかった場合
            {
                return false;
            }
        }
    }
}