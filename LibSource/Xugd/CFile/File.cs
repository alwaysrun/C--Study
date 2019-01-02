using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace SHCre.Xugd.CFile
{
    /// <summary>
    /// 文件操作相关辅助接口
    /// </summary>
    public static partial class XFile
    {
        /// <summary>
        /// 删除文件（无论文件是否是只读的）
        /// </summary>
        /// <param name="strFile_">要删除的文件</param>
        public static void DeleteFile(string strFile_)
        {
            FileInfo fInfo = new FileInfo(strFile_);
            if (fInfo.Exists)
            {
                fInfo.Attributes = FileAttributes.Normal;
                fInfo.Delete();
            }
        }
        
        /// <summary>
        /// 删除指定文件夹下，满足条件的文件；遇到满足条件的文件删除出错，会停止
        /// </summary>
        /// <param name="strFolder_">查找的文件夹</param>
        /// <param name="strFilter_">删除文件的条件（默认所有文件）</param>
        public static void DeleteFiles(string strFolder_, string strFilter_="*.*")
        {
            strFolder_ = XPath.GetFullPath(strFolder_);
            if (string.IsNullOrEmpty(strFilter_))
                strFilter_ = "*.*";

            string[] allFiles = Directory.GetFiles(strFolder_, strFilter_, SearchOption.AllDirectories);
            foreach(string strF in allFiles)
            {
                DeleteFile(strF);
            }
            XFolder.DeleteEmptyFolder(strFolder_);
        }

        /// <summary>
        /// 删除旧文件（最后修改日期早于dtRemoved_）；遇到满足条件的文件删除出错，会停止
        /// </summary>
        /// <param name="strFolder_">文件夹</param>
        /// <param name="dtRemoved_">要删除文件的最后日期</param>
        /// <param name="strFilter_">要删除文件条件</param>
        public static void DeleteFiles(string strFolder_, DateTime dtRemoved_, string strFilter_ = "*.*")
        {
            strFolder_ = XPath.GetFullPath(strFolder_);
            if (string.IsNullOrEmpty(strFilter_))
                strFilter_ = "*.*";

            string[] allFiles = Directory.GetFiles(strFolder_, strFilter_, SearchOption.AllDirectories);
            foreach (string strF in allFiles)
            {
                if (dtRemoved_ > File.GetLastWriteTime(strF))
                    DeleteFile(strF);
            }

            XFolder.DeleteEmptyFolder(strFolder_);
        }

        /// <summary>
        /// 删除指定文件夹下，满足条件的文件；尽力删除，遇到删除出错的文件会跳过继续执行
        /// </summary>
        /// <param name="strFolder_">查找的文件夹</param>
        /// <param name="strFilter_">删除文件的条件（默认所有文件）</param>
        public static void DeleteFilesTried(string strFolder_, string strFilter_ = "*.*")
        {
            strFolder_ = XPath.GetFullPath(strFolder_);
            if (string.IsNullOrEmpty(strFilter_))
                strFilter_ = "*.*";

            string[] allFiles = Directory.GetFiles(strFolder_, strFilter_, SearchOption.AllDirectories);
            foreach (string strF in allFiles)
            {
                try
                {
                    DeleteFile(strF);
                }
                catch(Exception ex)
                {
                    Trace.WriteLine(ex.Message, "SHCre.Xugd.CFile.XFile");
                }
            }
            XFolder.DeleteEmptyFolder(strFolder_);
        }

        /// <summary>
        /// 删除旧文件（最后修改日期早于dtRemoved_）；
        /// </summary>
        /// <param name="strFolder_">文件夹</param>
        /// <param name="dtRemoved_">最后日期</param>
        /// <param name="strFilter_">要删除文件条件</param>
        public static void DeleteFilesTried(string strFolder_, DateTime dtRemoved_, string strFilter_ = "*.*")
        {
            strFolder_ = XPath.GetFullPath(strFolder_);
            if (string.IsNullOrEmpty(strFilter_))
                strFilter_ = "*.*";

            string[] allFiles = Directory.GetFiles(strFolder_, strFilter_, SearchOption.AllDirectories);
            foreach (string strF in allFiles)
            {
                if (dtRemoved_> File.GetLastWriteTime(strF))
                {
                    try
                    {
                        DeleteFile(strF);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message, "SHCre.Xugd.CFile.XFile");
                    }
                }
            }

            XFolder.DeleteEmptyFolder(strFolder_);
        }

        /// <summary>
        /// 创建文件，并把数组内容写入文件中
        /// </summary>
        /// <param name="strFile_">文件名</param>
        /// <param name="byContents_">文件内容</param>
        /// <param name="bOverWrite_">如果文件已存在，是否覆盖</param>
        public static void CreateFile(string strFile_, byte[] byContents_, bool bOverWrite_)
        {
            if ((byContents_ == null) || (byContents_.Length == 0))
                throw new ArgumentException("Invalid File contents");

            strFile_ = XPath.GetFullPath(strFile_);
            FileInfo fInfo = new FileInfo(strFile_);
            if (fInfo.Exists && !bOverWrite_)
                throw new ArgumentException("File has existed");

            using (FileStream fs = fInfo.Create())
            {
                fs.Write(byContents_, 0, byContents_.Length);

                fs.Flush();
                fs.Close();
            }
        }

        /// <summary>
        /// 读取文件内容到数组（文件必须小于64K）：
        /// 文件超过64K，则抛出NotSupportedException("File too large(Must less than 64K)")异常
        /// </summary>
        /// <param name="strFile_">文件名</param>
        /// <returns>包含文件内容的数组</returns>
        public static byte[] ReadFile(string strFile_)
        {
            const int MaxFile_Size = 64*1024;

            strFile_ = XPath.GetFullPath(strFile_);
            FileInfo fInfo = new FileInfo(strFile_);
            if (!fInfo.Exists)
                throw (new FileNotFoundException("File " + strFile_ + " not found"));
            if( fInfo.Length > MaxFile_Size)
                throw new NotSupportedException("File too large(Must less than 64K)");

            byte[] byContent = null;
            using (FileStream fs = fInfo.OpenRead())
            {
                int nReadLen = 0, nReadOffset = 0;
                int nToRead = (int)fInfo.Length;
                byContent = new byte[nToRead];
                do
                {
                    nReadLen = fs.Read(byContent, nReadOffset, nToRead);
                    if (nReadLen == 0)
                        break;

                    nReadOffset += nReadLen;
                    nToRead -= nReadLen;
                } while (true);

                fs.Close();
            }

            return byContent;
        }

        /// <summary>
        /// 创建文件，并把内容写入文件中：
        /// 可使用File.WriteAllText覆盖写，
        /// File.AppendAllText追加写
        /// </summary>
        /// <param name="strFile_">文件名</param>
        /// <param name="strContents_">文件内容</param>
        /// <param name="bOverWrite_">如果文件已存在，是否覆盖</param>
        public static void CreateTextFile(string strFile_, string strContents_, bool bOverWrite_)
        {
            if (string.IsNullOrEmpty(strContents_))
                throw new ArgumentException("Invalid File contents");

            strFile_ = XPath.GetFullPath(strFile_);
            FileInfo fInfo = new FileInfo(strFile_);
            if (fInfo.Exists && !bOverWrite_)
                throw new ArgumentException("File has existed");

            using (FileStream fs = fInfo.Create())
            {
                using (StreamWriter fWrite = new StreamWriter(fs))
                {
                    fWrite.Write(strContents_);
                    fWrite.Close();
                }

                fs.Close();
            }
        }

        /// <summary>
        /// 读取文件内容到字符串（文件必须小于64K）：
        /// 文件超过64K，则抛出NotSupportedException("File too large(Must less than 64K)")异常
        /// </summary>
        /// <param name="strFile_">文件名</param>
        /// <returns>包含文件内容字符串</returns>
        public static string ReadTextFile(string strFile_)
        {
            const int MaxFile_Size = 64 * 1024;

            strFile_ = XPath.GetFullPath(strFile_);
            FileInfo fInfo = new FileInfo(strFile_);
            if (!fInfo.Exists)
                throw (new FileNotFoundException("File " + strFile_ + " not found"));
            if (fInfo.Length > MaxFile_Size)
                throw new NotSupportedException("File too large(Must less than 64K)");

            string strContent = null;
            using (FileStream fs = fInfo.OpenRead())
            {
                using(StreamReader fRead = new StreamReader(fs))
                {
                    strContent = fRead.ReadToEnd();
                    fRead.Close();
                }

                fs.Close();
            }

            return strContent;
        }
    }
}
