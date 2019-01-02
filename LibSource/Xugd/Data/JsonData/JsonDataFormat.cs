using System;
using System.Reflection;
using LitJson;
using System.Collections.Generic;

namespace SHCre.Xugd.Data
{
    /// <summary>
    /// Json序列化相关操作类：
    /// 出错时会引发XJsonDataException异常
    /// </summary>
    public class XJsonDataFormat
    {
        private Assembly _dataAssembly = null;

        /// <summary>
        /// 根据数据所在程序集名称构造：
        /// 只有需要反序列化DataWithName时才需要
        /// </summary>
        /// <param name="strAssemblyName_"></param>
        public XJsonDataFormat(string strAssemblyName_)
        {
            _dataAssembly = Assembly.Load(strAssemblyName_);
        }

        /// <summary>
        /// 根据数据所在程序集包含的类型构造：
        /// 只有需要反序列化DataWithName时才需要
        /// </summary>
        /// <param name="typeInDataAssembly_"></param>
        public XJsonDataFormat(Type typeInDataAssembly_)
        {
            _dataAssembly = Assembly.GetAssembly(typeInDataAssembly_);
        }

        /// <summary>
        /// 类序列化为Json字符串
        /// </summary>
        /// <param name="oData_">不能是简单类型（string、int等不能序列化，必须封装到类中）</param>
        /// <returns></returns>
        public static string Class2Json(object oData_)
        {
            try
            {
                return JsonMapper.ToJson(oData_);
            }
            catch (JsonException ex)
            {
                throw new XJsonSerializeException(ex, oData_);
            }
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
        public static T Json2Class<T>(string strJson_)
        {
            try
            {
                return JsonMapper.ToObject<T>(strJson_);
            }
            catch (JsonException ex)
            {
                throw new XJsonDeserializeException(ex, strJson_, typeof(T));
            }
        }

        /// <summary>
        /// Json字符串反序列化为object
        /// </summary>
        /// <param name="strJson_"></param>
        /// <param name="dataType_"></param>
        /// <returns></returns>
        public static object Json2Object(string strJson_, Type dataType_)
        {
            try
            {
                return JsonMapper.ToObject(strJson_, dataType_);
            }
            catch (JsonException ex)
            {
                throw new XJsonDeserializeException(ex, strJson_, dataType_);
            }
        }


        #region "Data with type"
        /// <summary>
        ///封装数据与对应的类型的类：
        ///一般用于通讯时传递的类信息（通过类型来区分传递的具体是哪个类）
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        public class DataWithType<TEnum>
        {
            /// <summary>
            /// 数据的索引号：
            /// 在应答时用于Response与Request匹配
            /// </summary>
            public int DataIndex {get; set;}
            /// <summary>
            /// 数据对应的类型
            /// </summary>
            public TEnum DataType { get; set; }
            /// <summary>
            /// 是否是应答
            /// </summary>
            public bool IsResponse { get; set; }
            /// <summary>
            /// 是否需要应答，只有IsResponse为false时才有效
            /// </summary>
            public bool NeedResponse { get; set; }
            /// <summary>
            /// 数据序列化后的Json字符串
            /// </summary>
            public string DataJson { get; set; }
            /// <summary>
            /// 数据头，可根据需要添加所需信息(Key,Value)，值只能是简单类型（ValueType与string）
            /// </summary>
            public Dictionary<string, object> Header { get; set; }

            /// <summary>
            /// 获取版本信息（应答Response直接使用请求Request的版本号）
            /// </summary>
            /// <returns></returns>
            public int GetVerion()
            {
                object nVer;
                if (!Header.TryGetValue(DataWithTypeHeader.Version, out nVer))
                    nVer = 0;

                return (int)nVer;
            }

            /// <summary>
            /// 
            /// </summary>
            public DataWithType()
            {
                Header = new Dictionary<string, object>();
            }

            /// <summary>
            /// 获取对应的数据：
            /// 根据DataType即可确定类型，然后调用此即可得到数据
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public T GetData<T>()
            {
                return Json2Class<T>(DataJson);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static class DataWithTypeHeader
        {
            /// <summary>
            /// 版本号
            /// </summary>
            public static string Version = "Version";
        }
        /// <summary>
        /// 数据序列化为包含数据类型的Json字符串
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="euType_"></param>
        /// <param name="oData_"></param>
        /// <param name="nDataIndex_"></param>
        /// <param name="nVersion_"></param>
        /// <param name="bIsReponse_">是否为应答</param>
        /// <param name="bNeedResponse_">是否需要应答，只有IsReponse为false是才有效</param>
        /// <returns></returns>
        public static string Data2JsonWithType<TEnum>(TEnum euType_, object oData_, int nDataIndex_, int nVersion_, bool bIsReponse_, bool bNeedResponse_)
        {
            //if (!typeof(TEnum).IsEnum)
            //{
            //    throw new ArgumentException("First param Type must be Enum-Type");
            //}

            try
            {
                DataWithType<TEnum> dataType = new DataWithType<TEnum>()
                {
                    DataIndex = nDataIndex_,
                    DataType = euType_,
                    IsResponse = bIsReponse_,
                    NeedResponse = bNeedResponse_,
                    DataJson = JsonMapper.ToJson(oData_),
                };
                dataType.Header[DataWithTypeHeader.Version] = nVersion_;

                return JsonMapper.ToJson(dataType);
            }
            catch (JsonException ex)
            {
                throw new XJsonSerializeException(ex, oData_);
            }
        }

        /// <summary>
        /// Json字符串反序列化为包含类型的DataWithType；
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="strJson_"></param>
        /// <returns></returns>
        public static DataWithType<TEnum> Json2DataWithType<TEnum>(string strJson_)
        {
            try
            {
                return JsonMapper.ToObject<DataWithType<TEnum>>(strJson_);
            }
            catch (JsonException ex)
            {
                throw new XJsonDeserializeException(ex, strJson_, typeof(DataWithType<TEnum>));
            }
        }
        #endregion

        //#region "Data with name"
        ///// <summary>
        ///// 封装类与对应完全限定名称（包含命名空间）
        ///// </summary>
        //public class DataWithName
        //{
        //    /// <summary>
        //    /// 数据对应类型的完全限定名称
        //    /// </summary>
        //    public string DataName { get; set; }
        //    /// <summary>
        //    /// 数据序列化后的Json字符串
        //    /// </summary>
        //    public string DataJson { get; set; }
        //}

        ///// <summary>
        ///// 根据Assembly的名称设定Assembly
        ///// </summary>
        ///// <param name="strAssemblyName_"></param>
        //public void SetAssembly(string strAssemblyName_)
        //{
        //    _dataAssembly = Assembly.Load(strAssemblyName_);
        //}

        ///// <summary>
        ///// 根据Assembly包含的类型设定Assembly
        ///// </summary>
        ///// <param name="typeInDataAssembly_"></param>
        //public void SetAssembly(Type typeInDataAssembly_)
        //{
        //    _dataAssembly = Assembly.GetAssembly(typeInDataAssembly_);
        //}

        ///// <summary>
        ///// 从包含类型名的Json字符串中反序列化数据
        ///// </summary>
        ///// <param name="strJson_"></param>
        ///// <returns></returns>
        //public object JsonWithName2Object(string strJson_)
        //{
        //    try
        //    {
        //        DataWithName dataName = JsonMapper.ToObject<DataWithName>(strJson_);
        //        Type dataType = _dataAssembly.GetType(dataName.DataName, true);
        //        return JsonMapper.ToObject(dataName.DataJson, dataType);
        //    }
        //    catch (JsonException ex)
        //    {
        //        throw new XJsonDeserializeException(ex, strJson_, typeof(DataWithName));
        //    }
        //}

        ///// <summary>
        ///// 把数据序列化为包含数据类型名的Json字符串
        ///// </summary>
        ///// <param name="objData_"></param>
        ///// <returns></returns>
        //public static string Data2JsonWithName(object objData_)
        //{
        //    try
        //    {
        //        DataWithName dataName = new DataWithName()
        //        {
        //            DataName = objData_.GetType().FullName,
        //            DataJson = JsonMapper.ToJson(objData_),
        //        };

        //        return JsonMapper.ToJson(dataName);
        //    }
        //    catch (JsonException ex)
        //    {
        //        throw new XJsonSerializeException(ex, objData_);
        //    }
        //}

        ///// <summary>
        ///// Json字符串反序列化为DataWithName
        ///// </summary>
        ///// <param name="strJson_"></param>
        ///// <returns></returns>
        //public static DataWithName Json2DataWithName(string strJson_)
        //{
        //    try
        //    {
        //        return JsonMapper.ToObject<DataWithName>(strJson_);
        //    }
        //    catch (JsonException ex)
        //    {
        //        throw new XJsonDeserializeException(ex, strJson_, typeof(DataWithName));
        //    }
        //}

        ///// <summary>
        ///// 从包含类型名的Json字符串中反序列化数据：
        ///// 如果要大量反序列化，请使用对应的实例函数接口
        ///// </summary>
        ///// <param name="strJson_"></param>
        ///// <param name="strDataAssemblyName_">数据对应类型所在Assembly（程序集）的全名</param>
        ///// <returns></returns>
        //public static object JsonWithName2Object(string strJson_, string strDataAssemblyName_)
        //{
        //    try
        //    {
        //        DataWithName dataName = JsonMapper.ToObject<DataWithName>(strJson_);
        //        Type dataType = Type.GetType(string.Format("{0},{1}", dataName.DataName, strDataAssemblyName_));
        //        return JsonMapper.ToObject(dataName.DataJson, dataType);
        //    }
        //    catch (JsonException ex)
        //    {
        //        throw new XJsonDeserializeException(ex, strJson_, typeof(DataWithName));
        //    }
        //}

        ///// <summary>
        ///// 从包含类型名的Json字符串中反序列化数据：
        ///// 如果要大量反序列化，请使用对应的实例函数接口
        ///// </summary>
        ///// <param name="strJson_"></param>
        ///// <param name="typeInSameAssembly_">与要反序列化的数据在同一个Assembly的任意类型</param>
        ///// <returns></returns>
        //public static object JsonWithName2Object(string strJson_, Type typeInSameAssembly_)
        //{
        //    return JsonWithName2Object(strJson_, typeInSameAssembly_.Assembly.FullName);
        //}
        //#endregion
    }
}
