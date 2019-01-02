using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHCre.Xugd.MsgData
{
    /// <summary>
    /// 发送消息的数据接口
    /// </summary>
    public interface IMsgDataBase
    {
    }    
    
    /// <summary>
    /// 应答接口
    /// </summary>
    public interface IMsgResponseBase : IMsgDataBase
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        /// <returns></returns>
        bool IsSuccess();
    }

    /// <summary>
    /// 消息格式化
    /// </summary>
    public static class XMsgFormat 
    {

        /// <summary>
        /// 类序列化为Json字符串
        /// </summary>
        /// <param name="oData_">不能是简单类型（string、int等不能序列化，必须封装到类中）</param>
        /// <returns></returns>
        public static string Msg2Json(IMsgDataBase oData_)
        {
            return LitJson.JsonMapper.ToJson(oData_);
        }

        /// <summary>
        /// Json字符串反序列化为类。
        /// 反序列化是根据属性名进行匹配，所以属性名相同者类型必须兼容；
        /// 对于无法匹配的属性，规则为（设序列化前的类型为OriT，当前类型为DestT）：
        /// DestT中属性比OriT中属性多（如，OriT为DestT的父类），可序列化成功；
        /// DestT中属性比OriT中属性少，如果只少一个则照样序列化成功，多余一个则失败，抛出异常。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strJson_"></param>
        /// <returns></returns>
        public static T Json2Msg<T>(string strJson_) where T : IMsgDataBase
        {
            return LitJson.JsonMapper.ToObject<T>(strJson_);
        }

        ///// <summary>
        ///// Json字符串反序列化为object
        ///// </summary>
        ///// <param name="strJson_"></param>
        ///// <param name="dataType_"></param>
        ///// <returns></returns>
        //public static object Json2Object(string strJson_, Type dataType_)
        //{
        //    return LitJson.JsonMapper.ToObject(strJson_, dataType_);
        //}

        /// <summary>
        /// 消息封装在XMsgWithType后转为Json
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="euType_"></param>
        /// <param name="argData_">要封装的消息，Json后存放在XMsgWithType.Data</param>
        /// <param name="nDataIndex_"></param>
        /// <param name="nVersion_"></param>
        /// <param name="dicHeader_"></param>
        /// <returns></returns>
        public static string Data2Json<TEnum>(TEnum euType_, IMsgDataBase argData_, int nDataIndex_, int nVersion_ = 1, Dictionary<string, string> dicHeader_ = null)
        {
            return ToDataJson<TEnum>(euType_, argData_, nDataIndex_, nVersion_, dicHeader_, XMsgMode.Data);
        }

        /// <summary>
        /// 消息封装在XMsgWithType后转为Json；
        /// 此类型的消息是需要对方应答的
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="euType_"></param>
        /// <param name="argData_">要封装的消息，Json后存放在XMsgWithType.Data</param>
        /// <param name="nDataIndex_"></param>
        /// <param name="bIsRequest_">是请求还是应答</param>
        /// <param name="nVersion_"></param>
        /// <param name="dicHeader_"></param>
        /// <returns></returns>
        public static string Request2Json<TEnum>(TEnum euType_, IMsgDataBase argData_, int nDataIndex_, bool bIsRequest_, int nVersion_ = 1, Dictionary<string, string> dicHeader_ = null)
        {
            return ToDataJson(euType_, argData_, nDataIndex_, nVersion_, dicHeader_,
                bIsRequest_ ? XMsgMode.Request : XMsgMode.Response);
        }

        private static string ToDataJson<TEnum>(TEnum euType_, IMsgDataBase oData_, int nDataIndex_, int nVersion_, Dictionary<string, string> dicHeader_, XMsgMode euMode_)
        {
            var tData = new XMsgWithType<TEnum>()
            {
                Index = nDataIndex_,
                Type = euType_,
                Mode = euMode_,
                Version = nVersion_,
            };
            tData.SetData(oData_);
            tData.AddHeader(dicHeader_);

            return LitJson.JsonMapper.ToJson(tData);
        }

        /// <summary>
        /// Json字符串转为XMsgWithType
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="strJson_"></param>
        /// <returns></returns>
        public static XMsgWithType<TEnum> Json2Data<TEnum>(string strJson_) where TEnum : struct,IComparable
        {
            return LitJson.JsonMapper.ToObject<XMsgWithType<TEnum>>(strJson_);
        }
    }

    #region "MsgWithType"
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum XMsgMode 
    {
        /// <summary>
        /// 普通消息
        /// </summary>
        Data = 0,
        /// <summary>
        /// 请求，需要应答
        /// </summary>
        Request = 1,
        /// <summary>
        /// 应答
        /// </summary>
        Response = 2,
    };

    /// <summary>
    /// 带类型的Json格式化消息封装类
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public class XMsgWithType<TEnum>
    {
        /// <summary>
        /// 消息的索引号：在应答时用于Response与Request匹配
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 消息请求类型
        /// </summary>
        public XMsgMode Mode {get;set;}
        /// <summary>
        /// 消息对应的类型
        /// </summary>
        public TEnum Type { get; set; }
        /// <summary>
        /// 消息序列化后的Json字符串
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// 扩展头，根据需要扩展(Key,Value)
        /// </summary>
        public Dictionary<string, string> Header { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public XMsgWithType()
        //{
        //    //Header = new Dictionary<string, string>();
        //}

        /// <summary>
        /// 获取对应的消息：
        /// 根据DataType即可确定类型，然后调用此即可得到消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetData<T>()
        {
            return LitJson.JsonMapper.ToObject<T>(Data);
        }

        /// <summary>
        /// 设定消息，转换为Json赋给Data
        /// </summary>
        /// <param name="argData_"></param>
        public void SetData(IMsgDataBase argData_)
        {
            Data = LitJson.JsonMapper.ToJson(argData_);
        }

        /// <summary>
        /// 是否是请求
        /// </summary>
        /// <returns></returns>
        public bool IsRequest()
        {
            return Mode == XMsgMode.Request;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsResponse()
        {
            return Mode == XMsgMode.Response;
        }

        /// <summary>
        /// 添加头
        /// </summary>
        /// <param name="strName_"></param>
        /// <param name="strValue_"></param>
        public void AddHeader(string strName_, string strValue_)
        {
            if (Header == null)
                Header = new Dictionary<string, string>();
            Header[strName_] = strValue_;
        }

        /// <summary>
        /// 添加头
        /// </summary>
        /// <param name="dicHeader_"></param>
        public void AddHeader(Dictionary<string, string> dicHeader_)
        {
            if (dicHeader_ == null || dicHeader_.Count == 0)
                return;
            if (Header == null)
                Header = new Dictionary<string, string>();

            foreach (var ph in dicHeader_)
                Header[ph.Key] = ph.Value;
        }

        /// <summary>
        /// 获取头：如果未设定，则返回空
        /// </summary>
        /// <param name="strKey_"></param>
        /// <returns></returns>
        public string GetHeader(string strKey_)
        {
            string strValue=null;
            if(Header!=null)
                Header.TryGetValue(strKey_, out strValue);
            return strValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return LitJson.JsonMapper.ToJson(this);
        }
    }
    #endregion

    #region "RevMsg"
    /// <summary>
    /// 接收到的数据类型
    /// </summary>
    public enum XMsgReceivedType
    {
        /// <summary>
        /// 无效
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// 普通数据
        /// </summary>
        Normal,
        /// <summary>
        /// 直接发送给自身的（可能是请求或应答），
        /// Topic是自身
        /// </summary>
        Direct = 0x10 | Normal,
        /// <summary>
        /// 通过主题订阅的（是事件或广播）
        /// </summary>
        Topic = 0x20 | Normal,
        /// <summary>
        /// 发送失败返回的数据
        /// </summary>
        SendBack = 0x100,
    }

    /// <summary>
    /// 接收到的数据信息（发送的Data数据后得到的此结构体）
    /// </summary>
    public class XMsgReceivedArgs : EventArgs
    {
        /// <summary>
        /// 发送者（对于SendBack，也要设为对方，即发送时的接收者）
        /// </summary>
        public string From { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// 数据的类型
        /// </summary>
        public XMsgReceivedType Type { get; set; }
        /// <summary>
        /// 具体接收到的数据
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 属性，通过AddProperty添加
        /// </summary>
        public Dictionary<string, string> Property { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strKey_"></param>
        /// <param name="strValue_"></param>
        public void AddProperty(string strKey_, string strValue_)
        {
            if (Property == null)
                Property = new Dictionary<string, string>();
            Property[strKey_] = strValue_;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strKey_"></param>
        /// <returns></returns>
        public string GetProperty(string strKey_)
        {
            string strValue = null;
            if (Property != null)
                Property.TryGetValue(strKey_, out strValue);

            return strValue;
        }

        /// <summary>
        /// 获取对应的消息：
        /// 根据DataType即可确定类型，然后调用此即可得到消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetData<T>()
        {
            return LitJson.JsonMapper.ToObject<T>(Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return LitJson.JsonMapper.ToJson(this);
        }
    }
    #endregion
}
