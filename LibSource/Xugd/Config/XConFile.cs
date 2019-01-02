using System.IO;
using System.Xml.Serialization;
using SHCre.Xugd.CFile;
using System;
using System.Text;

namespace SHCre.Xugd.Config
{
    /// <summary>
    /// 配置文件的基类：
    /// 集成此基类的子类可通过Read从XML文件中反序列化出类，Save把内容序列化到XML文件中；
    /// 可通过[XmlIgnore]来禁止序列化指定内容，通过[XmlAttribute]把内容序列化为属性（Attribute）
    /// </summary>
    public abstract class XConBase
    {
        /// <summary>
        /// 把配置文件反序列化到T类型的变量内存中：
        /// 配置文件必须与T结构统一
        /// </summary>
        /// <typeparam name="T">反序列化到的结构体类型</typeparam>
        /// <param name="strFile_">配置文件</param>
        /// <returns>反序列化的结果</returns>
        public static T Read<T>(string strFile_) where T : XConBase
        {
            return XConFile.Read<T>(strFile_);
        }

        /// <summary>
        /// 把变量序列化到配置文件文件中去
        /// </summary>
        /// <param name="strFile_">配置文件</param>
        public virtual void Save(string strFile_ )
        {
            if (strFile_ == null)
                throw new ArgumentException("File name can not null");

            strFile_ = XPath.GetFullPath(strFile_);
            XmlSerializer xmlSerial = new XmlSerializer(this.GetType());
            XPath.CreateFullPath(Path.GetDirectoryName(strFile_));
            using (Stream sWrite = new FileStream(strFile_, FileMode.Create))
            {
                xmlSerial.Serialize(sWrite, this);
                sWrite.Close();
            }
        }
    }

    /// <summary>
    /// 配置文件读写，配置文件必须与类型（必须为public且保证有默认构造函数）统一，可通过属性来描述是否序列化：
    /// public class AlarmAccessServerUser
    ///{
    ///    // 序列化为UserName子节点（AlarmAccessServerUser节点下的子节点）
    ///    public string UserName { get; set; }
    ///    // 序列化时会忽略（不写到配置文件中）
    ///    [XmlIgnore]
    ///    public string UserId { get; set; }
    ///    // 序列化为属性（AlarmAccessServerUser下的Password属性）
    ///    [XmlAttribute]
    ///    public string Password { get; set; }
    ///    
    ///    public void Init() {...} // 推荐通过此方法来实现初始化
    ///    public static AlarmAccessServerUser Read(string strFile_)
    ///    {
    ///         var con = XConFile.Read &lt; AlarmAccessServerUser &gt; (strFile);
    ///         if(con == null)
    ///         {
    ///             con = new AlarmAccessServerUser();
    ///             con.Init();
    ///             XConFile.Write(strFile, con);
    ///         }
    ///         return con;
    ///    }
    /// }
    /// </summary>
    public static class XConFile
    {
        /// <summary>
        /// 把配置文件反序列化到T类型的变量内存中：
        /// XML必须与T结构统一(T最好是顶层类，不要嵌套到其他类中去)；
        /// </summary>
        /// <typeparam name="T">反序列化到的结构体类型</typeparam>
        /// <param name="strFile_">配置文件</param>
        /// <returns>反序列化的结果</returns>
        public static T Read<T>(string strFile_) where T:class
        {
            return Read(strFile_, typeof(T)) as T;
        }

        /// <summary>
        /// 把配置文件反序列化到类的变量内存中：
        /// XML必须与类结构统一；
        /// 带中文的节点内容需要在头加上编码属性(encoding="gb2312")
        /// </summary>
        /// <param name="strFile_"></param>
        /// <param name="objType_"></param>
        /// <returns></returns>
        public static object Read(string strFile_, Type objType_) 
        {
            strFile_ = XPath.GetFullPath(strFile_);
            if (!File.Exists(strFile_))
                return null;

            XmlSerializer xmlSerial = new XmlSerializer(objType_);
            object objGet = null;
            using (Stream fStream = new FileStream(strFile_, FileMode.Open))
            {
                using (TextReader txtRead = new StreamReader(fStream, Encoding.UTF8))
                {
                    objGet = xmlSerial.Deserialize(txtRead);
                }

                fStream.Close();
            }

            return objGet;
        }

        /// <summary>
        /// 把指定类型序列化到配置文件文件中去
        /// </summary>
        /// <typeparam name="T">结构体类型(T最好是顶层类，不要嵌套到其他类中去)</typeparam>
        /// <param name="strFile_">配置文件</param>
        /// <param name="tObj_">要序列化的内容</param>
        public static void Write<T>(string strFile_, T tObj_) where T : class
        {
            strFile_ = XPath.GetFullPath(strFile_);
            if(tObj_ == null)
            {
                File.WriteAllBytes(strFile_, new byte[0]);
            }
            else
            {
                XmlSerializer xmlSerial = new XmlSerializer(tObj_.GetType());
                XPath.CreateFullPath(Path.GetDirectoryName(strFile_));
                using (Stream fStream = new FileStream(strFile_, FileMode.Create))
                {
                    using (TextWriter txtWriter = new StreamWriter(fStream, Encoding.UTF8))
                    {
                        xmlSerial.Serialize(txtWriter, tObj_);
                        txtWriter.Close();
                    }

                    fStream.Close();
                }
            }
        }

        /// <summary>
        /// 安全写：先写入到临时文件，再重命名为目的文件（防止破坏）；
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strFile_">目的文件</param>
        /// <param name="tObj_"></param>
        /// <param name="strTmpFile_">临时中间文件（如果不提供，则使用'File_tmp.xml'）</param>
        public static void WriteSafe<T>(string strFile_, T tObj_, string strTmpFile_=null) where T : class
        {
            strFile_ = XPath.GetFullPath(strFile_);
            if(string.IsNullOrEmpty(strTmpFile_))
            {
                string strName = Path.GetFileNameWithoutExtension(strFile_) + "_tmp.xml";
                strTmpFile_ = Path.Combine(Path.GetDirectoryName(strFile_), strName);
            }
            else
            {
                strTmpFile_ = XPath.GetFullPath(strTmpFile_);
            }

            Write(strTmpFile_, tObj_);

            File.Copy(strTmpFile_, strFile_, true);
            File.Delete(strTmpFile_);
        }
    } // class
}
