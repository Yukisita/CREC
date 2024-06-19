using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml.Linq;

namespace CREC
{
    //public class MessageBoxClasscs
    public class GetMessageFromLangageFileClass
    {
        public static string GetMessageBoxMessage(string MessageKey, string FormName, string targetLanguageFilePath)// MessageKey:メッセージの名前、FormName:メッセージボックスの親Formの名前、言語ファイルの場所
        {
            try
            {
                XElement xElement = XElement.Load("language\\" + targetLanguageFilePath+".xml");
                IEnumerable<XElement> buttonItemDataList = from item in xElement.Elements(FormName).Elements("MessageBox").Elements("item") select item;
                foreach (XElement itemData in buttonItemDataList)
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
    
        public static string GetToolTipMessage(string MessageKey, string FormName, string targetLanguageFilePath)// MessageKey:メッセージの名前、FormName:メッセージボックスの親Formの名前、言語ファイルの場所
        {
            try
            {
                XElement xElement = XElement.Load("language\\" + targetLanguageFilePath + ".xml");
                IEnumerable<XElement> buttonItemDataList = from item in xElement.Elements(FormName).Elements("ToolTip").Elements("item") select item;
                foreach (XElement itemData in buttonItemDataList)
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
