using System;
using System.Threading;
using System.IO;

namespace SHCre.Xugd.CFile
{
    partial class XLogFile
    {
        bool _bNeedReopen = false;
        // 当前文件名
        string _strCurFileName = string.Empty; 
        Timer _tmNameFormat;
        XFile.FileNameFormater _nameFormater = new XFile.FileNameFormater();

        string _strNameFormat = string.Empty;
        /// <summary>
        /// 日志记录文件格式（参考Config中的NameFormate）
        /// </summary>
        public string FileNameFormat
        {
            get { return _strNameFormat; }
            set
            {
                _bStartLog = !string.IsNullOrEmpty(value);
                _strNameFormat = value;

                if (_bStartLog)
                    StartNameCheck();
                else
                    StopNameCheck();
            }
        }

        void StartNameCheck(){
            if (_tmNameFormat == null)
                _tmNameFormat = new Timer(TimerNameCheckProc);

            TimerNameCheckProc(_tmNameFormat);
        }

        void StopNameCheck(){
            if (_tmNameFormat != null)
                _tmNameFormat.Change(Timeout.Infinite, Timeout.Infinite);
        }

        void TimerNameCheckProc(object oParam_)
        {
            try
            {
                DateTime dtNow = DateTime.Now;
                string strName = _nameFormater.GetFormatName(FileNameFormat, dtNow);
                if (strName != _strCurFileName)
                {
                    lock (_lkerWriteFile)
                    {
                        _strCurFileName = strName;
                        _bNeedReopen = true;
                    }

                    dtNow = DateTime.Now;
                }

                DateTime dtNext = dtNow.AddHours(1);
                DateTime dtExpire = new DateTime(dtNext.Year, dtNext.Month, dtNext.Day, dtNext.Hour, 0, 0);
                _tmNameFormat.Change(dtExpire - dtNow, TimeSpan.FromHours(1));
            }
            catch(Exception ex)
            {
                File.AppendAllText(XPath.GetFullPath("xugd.logfile.err"), ex.ToString() + "\n");
            }
        }

    } // XLogFile
}
