using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace CREC
{
    //public class MessageBoxClasscs
    public class LangageSettingClass
    {
        public static string GetMessageBoxMessage(string MessageKey, string FormName, string targetLanguageFilePath)// MessageKey:メッセージの名前、FormName:メッセージボックスの親Formの名前、言語ファイルの場所
        {
            return SearchFromLanguageFile(MessageKey, FormName, targetLanguageFilePath, "MessageBox");
        }

        public static string GetToolTipMessage(string MessageKey, string FormName, string targetLanguageFilePath)// MessageKey:メッセージの名前、FormName:メッセージボックスの親Formの名前、言語ファイルの場所
        {
            return SearchFromLanguageFile(MessageKey, FormName, targetLanguageFilePath, "ToolTip");
        }

        public static string GetOtherMessage(string MessageKey, string FormName, string targetLanguageFilePath)// MessageKey:メッセージの名前、FormName:メッセージボックスの親Formの名前、言語ファイルの場所
        {
            return SearchFromLanguageFile(MessageKey, FormName, targetLanguageFilePath, "Other");
        }

        private static string SearchFromLanguageFile(string MessageKey, string FormName, string targetLanguageFilePath, string targetElements)// 検索エンジン
        {
            try
            {
                XElement xElement = XElement.Load("language\\" + targetLanguageFilePath + ".xml");
                IEnumerable<XElement> ItemDataList = from item in xElement.Elements(FormName).Elements(targetElements).Elements("item") select item;
                foreach (XElement itemData in ItemDataList)
                {
                    try
                    {
                        if (itemData.Element("itemname").Value == MessageKey)
                        {
                            return itemData.Element("itemtext").Value;
                        }
                    }
                    catch (Exception ex)
                    {
                        return ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "言語ファイルが破損しています。\nLanguage File is BROKEN.";
        }
    }
}
