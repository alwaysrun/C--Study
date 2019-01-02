using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace SHCre.Xugd.CFile
{
    partial class XFile
    {
        /// <summary>
        /// 在指定的路径下，根据输入文件名构建一个新的文件名：
        /// 如果同名文件或文件夹已存在，则在文件名后面（扩展名前面）添加一个数字作为新的名称
        /// </summary>
        /// <param name="strPath_">所在的目录</param>
        /// <param name="strName_">默认的文件名</param>
        /// <returns>最终构建的文件名（指定路径下不存在的一个文件名）</returns>
        public static string BuildNewFile(string strPath_, string strName_)
        {
            int nNumbe = 2;
            string strNewName = strName_;
            string strExt = Path.GetExtension(strName_);
            string strFile = Path.GetFileNameWithoutExtension(strName_);
            while (File.Exists(Path.Combine(strPath_, strNewName)) || Directory.Exists(Path.Combine(strPath_, strNewName)))
            {
                strNewName = string.Format("{0}({1}){2}", strFile, nNumbe++, strExt);
            }

            return strNewName;
        }

        /// <summary>
        /// 根据命名格式来构造文件名：
        /// 可使用{xxxx}来表示年、月、日等时间,x的具体值参考DateTime.ToString中的格式化字符串；
        /// 如"Log\{yyyy}\{MM}\{dd}\MyLog {HH}.log"就是按年月日创建目录，并每小时创建一个新的文件
        /// </summary>
        /// <param name="strNameFormat_">命名格式</param>
        /// <param name="dtTime_">用于格式化文件名的时间</param>
        /// <returns>文件名</returns>
        public static string BuildFileFromNameFormat(string strNameFormat_, DateTime? dtTime_=null)
        {
            FileNameFormater nameFormat = new FileNameFormater();
            return nameFormat.GetFormatName(strNameFormat_, dtTime_);
        }

        /// <summary>
        /// 根据命名格式来构造文件名的类：
        /// 可使用{xxxx}来表示年、月、日等时间,x的具体值参考DateTime.ToString中的格式化字符串；
        /// 如"Log\{yyyy}\{MM}\{dd}\MyLog {HH}.log"就是按年月日创建目录，并每小时创建一个新的文件
        /// </summary>
        public class FileNameFormater
        {
            Regex regName = new Regex("{.*?}");
            /// <summary>
            /// 用于格式化文件名的时间
            /// </summary>
            private DateTime _dtForFormat;

            /// <summary>
            /// 获取格式化后的名称：把字符串中对应的日期格式化字符串替换为真实的时间
            /// （如{yyyy-MM-dd}会替换为2014-03-05）
            /// </summary>
            /// <param name="strNameFormat_">用括号包住的日期格式化字符串</param>
            /// <param name="dtTime_">用于格式化的时间，如果为null则使用当前时间</param>
            /// <returns></returns>
            public string GetFormatName(string strNameFormat_, DateTime? dtTime_ = null)
            {
                _dtForFormat = dtTime_.HasValue ? dtTime_.Value : DateTime.Now;
                return regName.Replace(strNameFormat_, new MatchEvaluator(NameFormatReplace));
            }

            private string NameFormatReplace(Match mReplace_)
            {
                string strValue_ = mReplace_.Value;
                int nLen = strValue_.Length;
                if (nLen > 2)  // {xxxx}
                {
                    string strTimeFormat = strValue_.Substring(1, nLen - 2);
                    try
                    {
                        string strTime = _dtForFormat.ToString(strTimeFormat);
                        if (strTime != strTimeFormat) // 
                            strValue_ = strTime;
                    }
                    catch (FormatException) { }
                }

                return strValue_;
            }
        }
    }
}
