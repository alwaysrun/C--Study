using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace SHCre.Xugd.CFile
{
    //partial class XLogFile
    //{
    //    private Timer _tmRemoveFiles;
    //    /// <summary>
    //    /// 要移除文件的过滤条件（默认所有文件）：
    //    /// 移除起始目录为FileNameFormat的顶层目录
    //    /// </summary>
    //    public string RemoveFileFilter { get; set; }

    //    private TimeSpan _tsExpire = TimeSpan.FromHours(0);
    //    /// <summary>
    //    /// 文件过期时间（删除过期的文件），时间间隔小于1小时将被忽略，不做删除。
    //    /// </summary>
    //    //[Obsolete("To removed next version, not used")]
    //    public TimeSpan RemoveFileExpire
    //    {
    //        get { return _tsExpire; }
    //        set
    //        {
    //            if (_tsExpire == value)
    //                return;
    //            _tsExpire = value;

    //            if (_tsExpire < TimeSpan.FromHours(1))
    //            {
    //                if (_tmRemoveFiles != null)
    //                    _tmRemoveFiles.Change(Timeout.Infinite, Timeout.Infinite);
    //            }
    //            else
    //            {
    //                if (_tmRemoveFiles == null)
    //                    _tmRemoveFiles = new Timer(this.TimerRemoveFileProce);

    //                // 定期进行一次清理
    //                double fPeriod = _tsExpire.TotalHours / 24;
    //                if (fPeriod < 1)
    //                    fPeriod = 1;
    //                else if (fPeriod > 8)
    //                    fPeriod = 8;
    //                _tmRemoveFiles.Change(TimeSpan.FromMinutes(0.5), TimeSpan.FromHours(fPeriod));
    //            }
    //        }
    //    }

    //    private string GetLogFolder(string strFile_)
    //    {
    //        // Root path
    //        Regex regName = new Regex("{.*?}");
    //        var mFirst = regName.Match(strFile_);
    //        if (mFirst.Success)
    //            strFile_ = strFile_.Substring(0, mFirst.Index + mFirst.Length);

    //        return Path.GetDirectoryName(strFile_);
    //    }

    //    /// <summary>
    //    /// 删除文件的
    //    /// </summary>
    //    /// <param name="oParam_"></param>
    //    private void TimerRemoveFileProce(object oParam_)
    //    {
    //        try
    //        {
    //            string strLogPath = _strCurFileName;
    //            if (string.IsNullOrEmpty(strLogPath))
    //                strLogPath = XFile.BuildFileFromNameFormat(FileNameFormat);
    //            try
    //            {
    //                if (Path.IsPathRooted(strLogPath))
    //                    strLogPath = GetLogFolder(FileNameFormat);
    //                else
    //                    strLogPath = XFolder.GetTopFolder(strLogPath);
    //            }
    //            catch (Exception)
    //            {
    //                strLogPath = Path.GetDirectoryName(strLogPath);
    //            }

    //            XFile.DeleteFilesTried(strLogPath, DateTime.Now - RemoveFileExpire, RemoveFileFilter);
    //        }
    //        catch (Exception ex)
    //        {
    //            Trace.WriteLine(ex.Message, "SHCre.Xugd.CFile.XLogFile");
    //        }
    //    }
    //}
}
