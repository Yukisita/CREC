/*
MainForm
Copyright (c) [2022-2024] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/

namespace CREC
{
    public class ConfigValuesClass
    {
        public bool AllowEdit { get; set; } = true;
        public bool AllowEditID { get; set; } = false;
        public bool ShowConfidentialData { get; set; } = true;
        public bool ShowUserAssistToolTips { get; set; } = true;
        public bool AutoSearch { get; set; } = true;
        public bool RecentShownContents { get; set; } = true;
        public bool BootUpdateCheck { get; set; } = true;
        public string AutoLoadProjectPath { get; set; } = string.Empty;
        public bool OpenLastTimeProject { get; set; } = false;
        public int FontsizeOffset { get; set; } = 0;

    }
}