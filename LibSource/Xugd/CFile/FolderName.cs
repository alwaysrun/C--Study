using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SHCre.Xugd.CFile
{
    partial class XFolder
    {
        /// <summary>
        /// 在指定的路径下，根据输入文件夹名构建一个新的文件夹名：
        /// 如果同名文件或文件夹已存在，则在文件夹名后面添加一个数字作为新的名称
        /// </summary>
        /// <param name="strPath_">所在的目录</param>
        /// <param name="strName_">默认的文件夹名</param>
        /// <returns>最终构建的文件夹名（指定路径下不存在的一个文件夹名）</returns>
        public static string BuildNewFolder(string strPath_, string strName_)
        {
            int nNumber = 2;
            string strNewName = strName_;
            while (Directory.Exists(Path.Combine(strPath_, strNewName)) || File.Exists(Path.Combine(strPath_, strNewName)))
            {
                strNewName = string.Format("{0}({1})", strName_, nNumber++);
            }

            return strNewName;
        }
    }
}
