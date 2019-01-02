using System;
using SHCre.Xugd.Common;
using SHCre.Xugd.Extension;

namespace SHCre.Xugd.Data
{
    /// <summary>
    /// 处理Json数据时引发的异常
    /// </summary>
    public class XJsonDataException : XBaseException
    {
        /// <summary>
        /// 
        /// </summary>
        public XJsonDataException():base()
        {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strMsg_"></param>
        public XJsonDataException(string strMsg_)
            : base(strMsg_)
        {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strMsg_"></param>
        /// <param name="exInner_"></param>
        public XJsonDataException(string strMsg_, Exception exInner_)
            : base(strMsg_, exInner_)
        { }
    }

    /// <summary>
    /// 序列化数据出错
    /// </summary>
    public class XJsonSerializeException:XJsonDataException
    {
        /// <summary>
        /// 要序列化的数据
        /// </summary>
        public object DataObject {get; private set;}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex_"></param>
        /// <param name="objData_">要序列化的数据</param>
        public XJsonSerializeException(Exception ex_, object objData_):
            base(ex_.Message, ex_)
        {
            DataObject = objData_;
        }

        /// <summary>
        /// 获取消息（包括类的内容)
        /// </summary>
        /// <returns></returns>
        public override string GetMessage()
        {
            return string.Format("XJsonSerializeException({0}) of: {1}", 
                this.Message, DataObject.PrintObjectSafe(false));
        }
    }

    /// <summary>
    /// 反序列化出错
    /// </summary>
    public class XJsonDeserializeException : XJsonDataException
    {
        /// <summary>
        /// Json字符串
        /// </summary>
        public string DataJson { get; private set; }
        /// <summary>
        /// 反序列化类型
        /// </summary>
        public Type DataType {get; private set;}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex_"></param>
        /// <param name="strJson_">Json字符串</param>
        /// <param name="tData_">反序列化类型</param>
        public XJsonDeserializeException(Exception ex_, string strJson_, Type tData_)
            : base(ex_.Message, ex_)
        {
            DataJson = strJson_;
            DataType = tData_;
        }

        /// <summary>
        /// 获取消息（包括Json字符串与要反序列化的类型)
        /// </summary>
        /// <returns></returns>
        public override string GetMessage()
        {
            return string.Format("XJsonDeserializeException({0}) of {1} from: {2}",
                this.Message, 
                XReflex.GetTypeName(DataType, false),
                DataJson);
        }
    }
}
