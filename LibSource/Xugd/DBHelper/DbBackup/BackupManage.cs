using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Common;
using SHCre.Xugd.Extension;
using SHCre.Xugd.CFile;
using System.IO;
using SHCre.Xugd.Data;

namespace SHCre.Xugd.DbHelper
{
    public partial class XDbBackup
    {
        XQueueIssue<BackupItems> _quBackup = null;
        XFile.FileNameFormater _pathFormat = new XFile.FileNameFormater();

        bool ToSaveData(BackupItems bakItem_)
        {
            string strPath = XPath.GetFullPath(XFile.BuildFileFromNameFormat(StorePath));
            string strFullName = Path.Combine(strPath, bakItem_.StoreName());

            var dbItem = bakItem_.DbItem;
            string strContext = XJsonDataFormat.Class2Json(dbItem) + "\n";

            InvokeOnDebug("Backup {0} to {1}", XString.PrintLimit(strContext, 100), strFullName);
            XPath.CreateFullPath(strPath);
            File.AppendAllText(strFullName, strContext, Encoding.UTF8);
            return true;
        }

        //////////////////////////////////////////////////////////////////////////
        class BackupData
        {
            public string Result {get;set;}
            public IXDapperInsert DbData { get; set; }

            public BackupData(string strMsg_, IXDapperInsert dbData_)
            {
                Result = strMsg_;
                DbData = dbData_;
            }

            public BackupData(Exception exErr_, IXDapperInsert dbData_)
            {
                var exBase = exErr_ as XBaseException;
                if(exBase == null)
                    Result = string.Format("{0}({1})", XReflex.GetTypeName(exErr_, false), exErr_.Message);
                else
                    Result = exBase.GetMessage();

                DbData = dbData_;
            }
        }

        class BackupItems
        {
            public string NamePostfix { get; set; }
            public BackupData DbItem { get; set; }

            public string StoreName()
            {
                string strName = string.Format("Db{0}({1}){2}.bak",
                    DateTime.Now.ToString("yyyyMMddHH"),
                    DbItem.DbData.GetTableName(),
                    NamePostfix);
                return strName;
            }
        }
    } // class XDbBackup
}
