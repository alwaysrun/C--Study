using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Extension;
using SHCre.Xugd.CFile;

namespace SHCre.Xugd.DbHelper
{
    /// <summary>
    /// 保存数据库数据到文件(.bak)
    /// </summary>
    public partial class XDbBackup : XLogEventsBase
    {
        string _strStorePath;
        /// <summary>
        /// 保存路径，默认"DbBackup\{yyyy}\{MM}"
        /// </summary>
        public string StorePath 
        {
            get { return _strStorePath; }
            set 
            {
                if(string.IsNullOrEmpty(value))
                {
                    _strStorePath = @"DbBackup\{yyyy}\{MM}";
                }
                else
                {
                    _strStorePath = value;
                }

                InvokeOnLogger("StorePath: " + _strStorePath);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strPath_">文件的保存路径</param>
        public XDbBackup(string strPath_)
        {
            StorePath = strPath_;
            LogPrefix = "DbBackup.";

            _quBackup = new XQueueIssue<BackupItems>(ToSaveData, 100, false);
            _quBackup.LogPrefix = "QuItem.";
            _quBackup.OnExcept += new Action<Exception, string>(_quBackup_OnExcept);
            _quBackup.OnLogger += new Action<string, XLogSimple.LogLevels>(_quBackup_OnLogger);
        }

        void _quBackup_OnExcept(Exception exErr_, string arg2)
        {
            InvokeOnExcept(exErr_, arg2);
        }

        void _quBackup_OnLogger(string strMsg, XLogSimple.LogLevels arg2)
        {
            InvokeOnLogger(strMsg, arg2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bEnabled_"></param>
        public override void SetDebugEnabled(bool bEnabled_)
        {
            base.SetDebugEnabled(bEnabled_);
            if (_quBackup != null)
                _quBackup.SetDebugEnabled(bEnabled_);
        }

        /// <summary>
        /// 添加要备份的数据
        /// </summary>
        /// <param name="dbData_">数据</param>
        /// <param name="strResult_">备份原因</param>
        /// <param name="strNamePostfix_">文件名后缀（db{时间}({表明}){Postfilx}.bak)</param>
        public void AddItem(IXDapperInsert dbData_, string strResult_, string strNamePostfix_)
        {
            _quBackup.AddItem(new BackupItems()
                {
                    NamePostfix = strNamePostfix_,
                    DbItem = new BackupData(strResult_, dbData_),
                });
        }

        /// <summary>
        /// 添加要备份的数据
        /// </summary>
        /// <param name="dbData_">数据</param>
        /// <param name="exErr_">出错异常</param>
        /// <param name="strNamePostfix_">文件名后缀（db{时间}({表明}){Postfilx}.bak)</param>
        public void AddItem(IXDapperInsert dbData_, Exception exErr_, string strNamePostfix_)
        {
            _quBackup.AddItem(new BackupItems()
            {
                NamePostfix = strNamePostfix_,
                DbItem = new BackupData(exErr_, dbData_),
            });
        }
        //public void Backup
    } // Class
} // NS
