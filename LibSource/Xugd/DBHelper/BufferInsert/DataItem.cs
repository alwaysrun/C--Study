using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Config;
using System.Diagnostics;
using SHCre.Xugd.Common;
using System.Xml.Serialization;

namespace SHCre.Xugd.DbHelper
{
    /// <summary>
    /// 通过BufferInsert类添加的数据，必须继承此接口
    /// </summary>
    public interface IDbDataInsert : IXDapperInsert
    {
        /// <summary>
        /// 键值所在列的名（在插入更新时使用）；
        /// 如果为空，则直接插入不会判断是否存在
        /// </summary>
        /// <returns></returns>
        string GetKeyColName();
    }

    /// <summary>
    /// 用于缓存到xml文件中的数据类
    /// </summary>
    /// <typeparam name="U"></typeparam>
    public class XDbInsertData<U>
    {
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<U> Data { get; set; }

        /// <summary>
        /// 数据的数量
        /// </summary>
        [XmlIgnoreAttribute]
        public int Count { get { return Data == null ? 0 : Data.Count; } }

        /// <summary>
        /// 
        /// </summary>
        public XDbInsertData()
        {
            Data = new List<U>();
        }

        /// <summary>
        /// 从xml文件中读取，如果xml文件不存在，则自动构造一个默认的类
        /// </summary>
        /// <param name="strFile_"></param>
        /// <returns></returns>
        public static XDbInsertData<U> Read(string strFile_)
        {
            XDbInsertData<U> readData = null;
            try
            {
                readData = XConFile.Read<XDbInsertData<U>>(strFile_);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "SHCre.Xugd.DbHelper.XInsertData");
            }

            if (readData == null)
                readData = new XDbInsertData<U>();
            return readData;
        }

        /// <summary>
        /// 把数据写入到xml文件中去
        /// </summary>
        /// <param name="strFile_"></param>
        public void Write(string strFile_)
        {
            try
            {
                XConFile.Write(strFile_, this);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "SHCre.Xugd.DbHelper.XInsertData");
            }
        }
    }

    /// <summary>
    /// 用于存放错误数据到xml文件中的类
    /// </summary>
    /// <typeparam name="U"></typeparam>
    public class XDbErrorData<U>
    {
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<XDbErrorItem<U>> Data { get; set; }

        /// <summary>
        /// 数据个数
        /// </summary>
        [XmlIgnoreAttribute]
        public int Count { get { return Data == null ? 0 : Data.Count; } }

        /// <summary>
        /// 
        /// </summary>
        public XDbErrorData()
        {
            Data = new List<XDbErrorItem<U>>();
        }

        /// <summary>
        /// 从xml文件中读取类信息，如果读取失败则构造一个默认的类
        /// </summary>
        /// <param name="strFile_">真实使用的文件名后会添加日期(yyyyMMdd)</param>
        /// <returns></returns>
        public static XDbErrorData<U> Read(string strFile_)
        {
            XDbErrorData<U> readData = null;
            try
            {
                readData = XConFile.Read<XDbErrorData<U>>(GetFileName(strFile_));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "SHCre.Xugd.DbHelper.XErrorData");
            }

            if (readData == null)
                readData = new XDbErrorData<U>();
            return readData;
        }

        /// <summary>
        /// 把数据写入到文件
        /// </summary>
        /// <param name="strFile_">真实使用的文件名后会添加日期(yyyyMMdd)</param>
        public void Write(string strFile_)
        {
            try
            {
                XConFile.Write(GetFileName(strFile_), this);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, "SHCre.Xugd.DbHelper.XErrorData");
            }
        }

        private static string GetFileName(string strFile_)
        {
            string strExt = ".xml";
            if (strFile_.EndsWith(strExt, StringComparison.OrdinalIgnoreCase))
                strFile_ = strFile_.Remove(strFile_.Length - strExt.Length);

            return strFile_ + XTime.GetDateString(DateTime.Now, "yyyyMMdd") + strExt;
        }
    }

    /// <summary>
    /// 错误数据的类型
    /// </summary>
    /// <typeparam name="U"></typeparam>
    public class XDbErrorItem<U>
    {
        /// <summary>
        /// 具体的数据
        /// </summary>
        public U Item { get; set; }
        /// <summary>
        /// 插入或更新时使用的sql语句
        /// </summary>
        public string Sql { get; set; }
        /// <summary>
        /// 具体的出错信息（DbException.Message）
        /// </summary>
        public string Error { get; set; }
    }
}
