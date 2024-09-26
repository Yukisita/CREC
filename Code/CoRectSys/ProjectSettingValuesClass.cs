/*
Program
Copyright (c) [2022-2024] [S.Yukisita]
This software is released under the MIT License.
https://github.com/Yukisita/CREC/blob/main/LICENSE
*/
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace CREC
{
    public enum CompressType// バックアップ時のファイル圧縮方法
    {
        SingleFile,
        ParData,
        NoCompress
    }

    public enum ListOutputFormat// リスト出力時のフォーマット
    {
        CSV,
        TSV
    }

    internal class ProjectSettingValuesClass
    {
        public string Name { get; set; } = string.Empty;// プロジェクト名
        public string ProjectDataFolderPath { get; set; } = string.Empty;// プロジェクトデータ保存場所のパス
        public string ProjectBackupFolderPath { get; set; } = string.Empty;// プロジェクトデータのバックアップ場所のパス
        public bool StartUpBackUp { get; set; } = false;// 起動時の自動バックアップ
        public bool CloseBackUp { get; set; } = false;// アプリケーション終了時の自動バックアップ
        public bool EditBackUp { get; set; } = false;// データ編集後の自動バックアップ
        public CompressType CompressType { get; set; } = CompressType.NoCompress;// 圧縮方法
        public string ListOutputPath { get; set; } = string.Empty;// リスト出力フォルダのパス
        public bool StartUpListOutput { get; set; } = false;// 起動時の自動リスト出力
        public bool CloseListOutput { get; set; } = false;// アプリケーション終了時の自動リスト出力
        public bool EditListOutput { get; set; } = false;// データ編集後の自動リスト出力
        public bool OpenListAfterOutput { get; set; } = false;// リスト出力後にファイルを開くか設定
        public ListOutputFormat ListOutputFormat { get; set; } = ListOutputFormat.CSV;// リスト出力時のフォーマット
        public string CreatedDate { get; set; } = string.Empty;// プロジェクト作成日
        public string ModifiedDate { get; set; } = string.Empty;// プロジェクト最終編集日
        public string AccessedDate { get; set; } = string.Empty;// プロジェクト最終アクセス日
        /// <summary>
        /// コレクションの名称ラベル
        /// </summary>
        public string CollectionNameLabel { get; set; } = "Name";
        /// <summary>
        /// コレクションの名称表示・非表示フラグ
        /// </summary>
        public bool CollectionNameVisible { get; set; } = true;
        /// <summary>
        /// コレクションのUUIDのラベル
        /// </summary>
        public string UUIDLabel { get; set; } = "UUID";
        /// <summary>
        /// コレクションのUUIDの表示・非表示フラグ
        /// </summary>
        public bool UUIDVisible { get; set; } = true;
        /// <summary>
        /// コレクションの管理コードのラベル
        /// </summary>
        public string ManagementCodeLabel { get; set; } = "Mgmt. code";
        /// <summary>
        /// コレクションの管理コードの表示・非表示フラグ
        /// </summary>
        public bool ManagementCodeVisible { get; set; } = true;
        /// <summary>
        /// コレクションの管理コードの自動入力有効・無効フラグ
        /// </summary>
        public bool ManagementCodeAutoFill { get; set; } = true;
        /// <summary>
        /// コレクションの登録日
        /// </summary>
        public string RegistrationDateLabel { get; set; } = "Registration Date";
        /// <summary>
        /// コレクションの登録日の表示・非表示フラグ
        /// </summary>
        public bool RegistrationDateVisible { get; set; } = true;
        /// <summary>
        /// コレクションのカテゴリのラベル
        /// </summary>
        public string CategoryLabel { get; set; } = "Category";
        /// <summary>
        /// コレクションのカテゴリの表示・非表示フラグ
        /// </summary>
        public bool CategoryVisible { get; set; } = true;
        /// <summary>
        /// コレクションのタグ1のラベル
        /// </summary>
        public string FirstTagLabel { get; set; } = "Tag1";
        /// <summary>
        /// コレクションのタグ1の表示・非表示フラグ
        /// </summary>
        public bool FirstTagVisible { get; set; } = true;
        /// <summary>
        /// コレクションのタグ2のラベル
        /// </summary>
        public string SecondTagLabel { get; set; } = "Tag2";
        /// <summary>
        /// コレクションのタグ2の表示・非表示フラグ
        /// </summary>
        public bool SecondTagVisible { get; set; } = true;
        /// <summary>
        /// コレクションのタグ3のラベル
        /// </summary>
        public string ThirdTagLabel { get; set; } = "Tag3";
        /// <summary>
        /// コレクションのタグ3の表示・非表示フラグ
        /// </summary>
        public bool ThirdTagVisible { get; set; } = true;
    }
}
