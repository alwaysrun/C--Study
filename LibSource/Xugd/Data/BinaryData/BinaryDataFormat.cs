using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.Data
{
    /// <summary>
    /// 序列化与反序列化类
    /// </summary>
    public class XBinaryDataFormat : IXDataFormat
    {
        /// <summary>
        /// 数据已序列化完成
        /// </summary>
        public Action<object> DataDeserialized { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oData_"></param>
        protected void InvokeDataDeserialzed(object oData_)
        {
            if (DataDeserialized != null)
                DataDeserialized(oData_);
        }

        /// <summary>
        /// 反序列化失败
        /// </summary>
        public Action<Exception> DeserializeFailed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex_"></param>
        protected void InvokeDeserializeFailed(Exception ex_)
        {
            if (DeserializeFailed != null)
                DeserializeFailed(ex_);
        }

        private static readonly object DataTypeLocker = new object();
        private static List<XBinaryDataTypeInfo> DataTypeInfos = new List<XBinaryDataTypeInfo>();

        private Type _defObjType;

        /// <summary>
        /// 用于反序列化的数据buffer
        /// </summary>
        public XDataBuffer Buffer { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objType_"></param>
        public XBinaryDataFormat(Type objType_)
            : this(objType_, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objType_"></param>
        /// <param name="dataBuffer_"></param>
        public XBinaryDataFormat(Type objType_, XDataBuffer dataBuffer_)
        {
            if (dataBuffer_ == null)
                dataBuffer_ = new XDataBuffer();

            this._defObjType = objType_;
            this.Buffer = dataBuffer_;
            this.Buffer.BufferChanged = new Action<XDataBuffer.ChangeMode>(this.OnBufferChanged);
        }

        /// <summary>
        /// 添加要反序列化的数据
        /// </summary>
        /// <param name="aryData_"></param>
        public void Add(byte[] aryData_)
        {
            this.Buffer.Add(aryData_);
        }

        /// <summary>
        /// 添加要反序列化的数据
        /// </summary>
        /// <param name="aryData_"></param>
        /// <param name="nOffset_"></param>
        /// <param name="nLength_"></param>
        public void Add(byte[] aryData_, int nOffset_, int nLength_)
        {
            this.Buffer.Add(aryData_, nOffset_, nLength_);
        }

        /// <summary>
        /// 清空要反序列化的数据
        /// </summary>
        public void Clear()
        {
            this.Buffer.Clear();
        }

        private static XBinaryDataTypeInfo GetDataTypeInfo(Type tDataType_)
        {
            lock (DataTypeLocker)
            {
                XBinaryDataTypeInfo typeInfo = DataTypeInfos.FirstOrDefault<XBinaryDataTypeInfo>(x => x.DataType == tDataType_);
                if (typeInfo == null)
                {
                    object[] dataAttrs = tDataType_.GetCustomAttributes(typeof(XBinaryDataAttribute), true);
                    if (dataAttrs.Length == 0)
                        throw new XBinaryDataException(string.Format("GetDataTypeInfo({0}): Not set XBinaryDataAttribute", tDataType_.ToString()), SHErrorCode.NotSupported);

                    typeInfo = new XBinaryDataTypeInfo(tDataType_, dataAttrs[0] as XBinaryDataAttribute);
                    foreach (MemberInfo fi in tDataType_.GetMembers(BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
                    {
                        object[] eleAttrs = fi.GetCustomAttributes(typeof(XBinaryElementAttribute), true);
                        if (eleAttrs.Length > 0)
                        {
                            XBinaryElementInfo beInfo = new XBinaryElementInfo(typeInfo, eleAttrs[0] as XBinaryElementAttribute, fi);
                            typeInfo.OrderItems.Add(beInfo);
                        }
                    }
                    typeInfo.ReordDataElements();

                    DataTypeInfos.Add(typeInfo);
                }

                return typeInfo;
            }
        }

        private void OnBufferChanged(XDataBuffer.ChangeMode euChange_)
        {
            //             if (clear)
            //             {
            //                 this.CurrentDeserializationInfo = null;
            //             }
            //             else if (this.Buffer.Length > 0)
            //             {
            //                 try
            //                 {
            //                     if (this.CurrentDeserializationInfo == null)
            //                     {
            //                         XSerializationInfo si;
            //                         bool orderdata = false;
            //                         if (Debugger.IsAttached)
            //                         {
            //                             orderdata = true;
            //                         }
            //                         GetFirstDeserializeInfo(out si, this.DeserializeType, orderdata);
            //                         this.CurrentDeserializationInfo = si;
            //                     }
            //                     if (GetDeserializeInfo(this.CurrentDeserializationInfo, this.Buffer, this.DeserializeError) == XSerializeResult.Ok)
            //                     {
            //                         if (this.DataDeserialized != null)
            //                         {
            //                             this.DataDeserialized(this.CurrentDeserializationInfo.ObjSelf);
            //                         }
            //                         if (this.SerializationInfoDeserialized != null)
            //                         {
            //                             this.SerializationInfoDeserialized(this.CurrentDeserializationInfo);
            //                         }
            //                         this.CurrentDeserializationInfo = null;
            //                         if (this.Buffer.Length > 0)
            //                         {
            //                             this.OnBufferChanged(false);
            //                         }
            //                     }
            //                 }
            //                 catch (Exception e)
            //                 {
            //                     if (this.DeserializeError != null)
            //                     {
            //                         this.DeserializeError(e.ToString());
            //                     }
            //                     this.CurrentDeserializationInfo = null;
            //                 }
            //             }
        }

        #region "Deserialize"
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <returns></returns>
        public object Deserialize()
        {
            var dataTypeInfo = GetDataTypeInfo(_defObjType);
            object tData = _defObjType.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            for (int i = 0; i < dataTypeInfo.OrderItems.Count; ++i)
            {
                DeserializeMembers(Buffer, dataTypeInfo.OrderItems[i], tData);
            }

            return tData;
        }

        /// <summary>
        /// 把数据反序列化为对应的类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="byBuffer_"></param>
        /// <returns></returns>
        public static T Bytes2Class<T>(byte[] byBuffer_) where T : class, new()
        {
            return Bytes2Class<T>(byBuffer_, 0, byBuffer_.Length);
        }

        /// <summary>
        /// 把数据反序列化为对应的类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="byBuffer_"></param>
        /// <param name="nIndex_"></param>
        /// <param name="nLength_"></param>
        /// <returns></returns>
        public static T Bytes2Class<T>(byte[] byBuffer_, int nIndex_, int nLength_) where T : class, new()
        {
            if (byBuffer_ == null)
                throw new ArgumentNullException("Data buffer cannot null");

            XDataBuffer beBuffer = new XDataBuffer();
            beBuffer.Add(byBuffer_, nIndex_, nLength_);
            return Bytes2Class<T>(beBuffer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="beBuffer_"></param>
        /// <returns></returns>
        public static T Bytes2Class<T>(XDataBuffer beBuffer_) where T : class, new()
        {
            if (beBuffer_ == null)
                throw new ArgumentNullException("Data buffer cannot null");

            Type objType = typeof(T);
            var dataTypeInfo = GetDataTypeInfo(objType);
            var nSize = beBuffer_.GetDecimal(dataTypeInfo.DataAttribute.ClassHeadType, dataTypeInfo.DataAttribute.UseNetOrder);
            T tData = null;
            if (nSize != 0)
            {
                tData = new T();
                for (int i = 0; i < dataTypeInfo.OrderItems.Count; ++i)
                {
                    DeserializeMembers(beBuffer_, dataTypeInfo.OrderItems[i], tData);
                }
            }

            return tData;
        }

        private static int GetBytesFromHead(XDataBuffer beBuffer_, XBinaryElementAttribute eleAttri_)
        {
            return (int)beBuffer_.GetDecimal(eleAttri_.SizeHeadType, eleAttri_.UseNetOrder);
        }

        private static int GetCountFromHead(XDataBuffer beBuffer_, XBinaryElementAttribute eleAttri_)
        {
            return (int)beBuffer_.GetDecimal(eleAttri_.CountHeadType, eleAttri_.UseNetOrder);
        }

        private static object GetDeserialization(Type objType_, XDataBuffer beBuffer_, XBinaryElementAttribute eleAttri_)
        {
            object objRetData = null;
            if (objType_ == typeof(string))
            {
                objRetData = DeserializeString(beBuffer_, eleAttri_);
            }
            else if (objType_ == typeof(DateTime))
            {
                objRetData = DeserializeDateTime(beBuffer_, eleAttri_);
            }
            else if (objType_.IsValueType)
            {
                objRetData = DeserializeValueType(beBuffer_, objType_, eleAttri_);
            }
            else if (objType_ == typeof(byte[]))
            {
                int nBytes = 0;
                if (eleAttri_.FixedSize > 0)
                {
                    nBytes = eleAttri_.FixedSize;
                }
                else
                {
                    nBytes = GetBytesFromHead(beBuffer_, eleAttri_);
                }

                if(nBytes>0)
                    objRetData = beBuffer_.GetBytes(nBytes);
            }
            else if (objType_.IsArray || objType_.GetInterface(typeof(IList).ToString(), false) != null)
            {
                objRetData = DeserializeArrayOrList(beBuffer_, objType_, eleAttri_);
            }
            else
            { // class
                XBinaryDataTypeInfo dataTypeInfo = GetDataTypeInfo(objType_);
                int nClassLen = GetBytesFromHead(beBuffer_, eleAttri_);
                if (nClassLen >0)
                {
                    objRetData = objType_.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                    for (int i = 0; i < dataTypeInfo.OrderItems.Count; ++i)
                    {
                        DeserializeMembers(beBuffer_, dataTypeInfo.OrderItems[i], objRetData);
                    }
                }
            }

            return objRetData;
        }

        private static string DeserializeString(XDataBuffer beBuffer_, XBinaryElementAttribute eleAttri_)
        {
            int nBytes = 0;
            if (eleAttri_.FixedSize > 0)
            {
                nBytes = eleAttri_.FixedSize;
            }
            else
            {
                nBytes = GetBytesFromHead(beBuffer_, eleAttri_);
            }

            if (nBytes > 0)
            {
                var vRet = beBuffer_.GetString(nBytes, eleAttri_.StringEncoding);
                if (!string.IsNullOrEmpty(eleAttri_.EndCharsToTrim))
                    vRet = vRet.TrimEnd(eleAttri_.TrimChars);
                return vRet;
            }
            return string.Empty;
        }

        private static DateTime DeserializeDateTime(XDataBuffer beBuffer_, XBinaryElementAttribute eleAttri_)
        {
            string strDate;
            string strFormat = eleAttri_.DateTimeFormat;
            if (strFormat.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                strFormat = strFormat.Substring(2); // Remove '0x'
                ulong uDate = beBuffer_.GetUInt64(eleAttri_.UseNetOrder);
                strDate = uDate.ToString("x16");
                if (strDate.Length > strFormat.Length)
                    strDate = strDate.Remove(strFormat.Length);
            }
            else
            {
                strDate = beBuffer_.GetString(strFormat.Length, Encoding.UTF8);
            }

            return DateTime.ParseExact(strDate, strFormat, null);
        }

        private static int GetValueTypeLength(Type valType_)
        {
            if (valType_ == typeof(bool))
                return 1;

            if ((valType_ == typeof(byte)) || (valType_ == typeof(sbyte)))
                return 1;

            if ((valType_ == typeof(short)) || (valType_ == typeof(ushort)))
                return 2;
            if ((valType_ == typeof(int)) || (valType_ == typeof(uint)))
                return 4;
            if ((valType_ == typeof(long)) || (valType_ == typeof(ulong)))
                return 8;

            if (valType_ == typeof(double))
                return 8;

            if (valType_ == typeof(float))
                return 4;

            throw new NotSupportedException("GetValueTypeLength: Not value type, " + valType_.ToString());
        }

        private static dynamic DeserializeValueType(XDataBuffer beBuffer_, Type objType_, XBinaryElementAttribute eleAttri_)
        {
            if (objType_ == typeof(char))
                return beBuffer_.GetChar();

            if (objType_.IsEnum)
                objType_ = Enum.GetUnderlyingType(objType_);
            return beBuffer_.GetDecimal(objType_, eleAttri_.UseNetOrder);
        }

        private static dynamic DeserializeArrayOrList(XDataBuffer beBuffer_, Type objType_, XBinaryElementAttribute eleAttri_)
        {
            // todo: Consider use Size instead Count
            int nCount = GetCountFromHead(beBuffer_, eleAttri_);
            if (nCount == 0)
                return objType_.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);

            // Items
            Type itemType;
            if (objType_.IsArray)
                itemType = objType_.GetElementType();
            else
                itemType = objType_.GetGenericArguments()[0];
            XBinaryElementAttribute itemAttri = eleAttri_.GetItemAttribute();
            object[] objItems = new object[nCount];
            for (int i = 0; i < nCount; ++i)
            {
                objItems[i] = GetDeserialization(itemType, beBuffer_, itemAttri);
            }

            if (objType_.IsArray)
            {
                Array aryData = Array.CreateInstance(itemType, nCount);
                for (int i = 0; i < nCount; ++i)
                    aryData.SetValue(objItems[i], i);

                return aryData;
            }

            // List
            object objList = objType_.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            IList iList = objList as IList;
            for (int i = 0; i < nCount; ++i)
                iList.Add(objItems[i]);

            return objList;
        }

        private static void DeserializeMembers(XDataBuffer beBuffer_, XBinaryElementInfo eleInfo_, object objParent_)
        {
            Type typeMember = null;
            switch (eleInfo_.SelfInfo.MemberType)
            {
                case MemberTypes.Field:
                    typeMember = (eleInfo_.SelfInfo as FieldInfo).FieldType;
                    break;

                case MemberTypes.Property:
                    typeMember = (eleInfo_.SelfInfo as PropertyInfo).PropertyType;
                    break;

                default:
                    return;
            }

            object objMember = GetDeserialization(typeMember, beBuffer_, eleInfo_.ElementAttribute);
            if (eleInfo_.SelfInfo.MemberType == MemberTypes.Field)
            {
                (eleInfo_.SelfInfo as FieldInfo).SetValue(objParent_, objMember);
            }
            else
            {
                (eleInfo_.SelfInfo as PropertyInfo).SetValue(objParent_, objMember, null);
            }
        }
        #endregion

        #region "Serialize"
        /// <summary>
        /// 序列化约定：
        /// Type    IsFixSize   Header
        /// string      Y       None
        ///  --         N       Size
        /// byte[]      Y       None
        ///  --         N       Size
        /// DateTime    X       None(以0x开始时转换为long类型序列化；否则转换转换为字符串并使用UTF8序列化，其长度与格式化字符串的长度相等)
        /// ValueType   X       None(char转换为两个字节)
        /// Array/List  X       Count(如果定义了UseSizeForArray则为Size)
        /// Class       X       Size
        /// </summary>
        /// <param name="objData_"></param>
        /// <returns></returns>
        public byte[] Serialize(object objData_)
        {
            if (objData_ == null)
                throw new ArgumentNullException("SerializeData: Data can not be null");

            Type objType = objData_.GetType();
            var serInfo = GetSerialization(objData_, objType);
            return serInfo.GetSerializeData();
        }

        /// <summary>
        /// 把类序列化为字节数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tData_"></param>
        /// <returns></returns>
        public static byte[] Class2Bytes<T>(T tData_) where T : class, new()
        {
            if (tData_ == null)
                throw new ArgumentNullException("Class2Bytes: Data can not be null");

            Type objType = tData_.GetType();
            var serInfo = GetSerialization(tData_, objType);
            return serInfo.GetSerializeData();
        }

        private static XSerializationInfo GetSerialization(object objData_, Type objType_)
        {
            XSerializationInfo serInfo = new XSerializationInfo()
            {
                Parent = null,
                DataType = objType_,
            };

            var dataTypeInfo = GetDataTypeInfo(objType_);
            serInfo.ElementAttribute = XBinaryElementAttribute.GetClassAttribute(dataTypeInfo.DataAttribute);
            if (objData_ != null)
            {
                foreach (XBinaryElementInfo beInfo in dataTypeInfo.SerializeItems)
                {
                    SerializeMembers(objData_, beInfo, serInfo);
                }
            }
            serInfo.SetHeadType(XSerializeHeadType.Size, dataTypeInfo.DataAttribute.SizeHeadType);

            return serInfo;
        }

        private static XSerializationInfo SerializeByType(object objData_, Type objType_, XSerializationInfo siParent_, XBinaryElementAttribute eleAttri_, int nAryIndex_ = -1)
        {
            XSerializationInfo serInfo = new XSerializationInfo()
            {
                Parent = siParent_,
                DataType = objType_,
                ArrayIndex = nAryIndex_,
                ElementAttribute = eleAttri_,
            };

            //  To Serialize
            if (objType_ == typeof(string))
            {
                string strData = null;
                if (objData_ != null)
                {
                    strData = (objData_ as string);
                    if (!string.IsNullOrEmpty(eleAttri_.EndCharsToTrim))
                        strData = strData.TrimEnd(eleAttri_.TrimChars);
                }
                serInfo.DataBytes = SerializeString(strData, eleAttri_.StringEncoding, eleAttri_.FixedSize);
                if (eleAttri_.FixedSize <= 0)
                    serInfo.SetHeadType(XSerializeHeadType.Size, eleAttri_.SizeHeadType);
            }
            else if (objType_ == typeof(DateTime))
            {
                SerializeDateTime((DateTime)objData_, serInfo);
            }
            else if (objType_.IsValueType)
            {
                SerializeValueType(objData_, objType_, serInfo);
            }
            else if (objType_ == typeof(byte[]))
            {
                byte[] aryData = objData_ as byte[];
                if (aryData == null)
                    aryData = new byte[0];

                int nLen = aryData.Length;
                if (eleAttri_.FixedSize > 0)
                    nLen = eleAttri_.FixedSize;
                serInfo.DataBytes = new byte[nLen]; //  avoid be modified by others
                Array.Copy(aryData, serInfo.DataBytes, Math.Min(nLen, aryData.Length));
                if (eleAttri_.FixedSize <= 0)
                    serInfo.SetHeadType(XSerializeHeadType.Size, eleAttri_.SizeHeadType);
            }
            else if (objType_.IsArray || objType_.GetInterface(typeof(IList).ToString(), false) != null)
            {
                SerializeArrayOrList(objData_, objType_, serInfo);
            }
            else
            { // class
                var dataTypeInfo = GetDataTypeInfo(objType_);
                if (objData_ != null)
                {
                    foreach (XBinaryElementInfo beInfo in dataTypeInfo.SerializeItems)
                    {
                        SerializeMembers(objData_, beInfo, serInfo);
                    }
                }
                serInfo.SetHeadType(XSerializeHeadType.Size, dataTypeInfo.DataAttribute.SizeHeadType);
            }

            serInfo.ReorderMembers();
            return serInfo;
        }

        private static void SerializeDateTime(DateTime dtData_, XSerializationInfo serInfo_)
        {
            string strFormat = serInfo_.ElementAttribute.DateTimeFormat;
            if (strFormat.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
            {
                strFormat = strFormat.Substring(2); // Remove '0x'
                string strTime = dtData_.ToString(strFormat);
                strTime = strTime.PadRight(0x10, '0');
                ulong uDate = ulong.Parse(strTime, NumberStyles.AllowHexSpecifier);
                SerializeValueType(uDate, typeof(ulong), serInfo_);
            }
            else
            {
                string strDate = dtData_.ToString(strFormat);
                serInfo_.DataBytes = SerializeString(strDate, Encoding.UTF8);
            }
        }

        private static void SerializeArrayOrList(object objData_, Type objType_, XSerializationInfo serInfo_)
        {
            int nIndex = 0;
            if (objData_ != null)
            {
                Type eleType;
                if (objType_.IsArray)
                    eleType = objType_.GetElementType();
                else
                    eleType = objType_.GetGenericArguments()[0];

                XBinaryElementAttribute beItemAttri = serInfo_.ElementAttribute.GetItemAttribute();
                foreach (var objEle in objData_ as IEnumerable)
                {
                    var itemType = eleType;
                    if (objEle != null) // 可能是定义类型的子类，此时需要单独获取
                        itemType = objEle.GetType();

                    serInfo_.Children.Add(SerializeByType(objEle, itemType, serInfo_, beItemAttri, nIndex));
                    nIndex++;
                }
            }

            // todo: check whether use Size instead Count
            serInfo_.SetHeadCount(nIndex, serInfo_.ElementAttribute.CountHeadType);
        }

        private static void SerializeMembers(object objData_, XBinaryElementInfo beInfo_, XSerializationInfo serInfo_)
        {
            object objMember = null;
            Type typeMember = null;
            switch (beInfo_.SelfInfo.MemberType)
            {
                case MemberTypes.Field:
                    objMember = (beInfo_.SelfInfo as FieldInfo).GetValue(objData_);
                    typeMember = (beInfo_.SelfInfo as FieldInfo).FieldType;
                    break;

                case MemberTypes.Property:
                    objMember = (beInfo_.SelfInfo as PropertyInfo).GetValue(objData_, new object[0]);
                    typeMember = (beInfo_.SelfInfo as PropertyInfo).PropertyType;
                    break;

                default:
                    return;
            }
            if (objMember != null)
                typeMember = objMember.GetType();

            serInfo_.Children.Add(SerializeByType(objMember, typeMember, serInfo_, beInfo_.ElementAttribute));
        }

        private static byte[] SerializeString(string strData_, Encoding enCoding_, int nFixSize_ = -1)
        {
            if (strData_ == null)
                strData_ = string.Empty;

            byte[] bsString = enCoding_.GetBytes(strData_);
            if ((nFixSize_ > 0) && (nFixSize_ != bsString.Length))
            {
                byte[] bsTemp = bsString;
                bsString = new byte[nFixSize_];
                Array.Copy(bsTemp, 0, bsString, 0, Math.Min(bsTemp.Length, nFixSize_));
            }

            return bsString;
        }

        private static void SerializeValueType(object objData_, Type objType_, XSerializationInfo serInfo_)
        {
            if (objType_.IsEnum)
                objType_ = Enum.GetUnderlyingType(objType_);

            if ((objType_ == typeof(byte)) || (objType_ == typeof(sbyte)))
                serInfo_.DataBytes = new byte[] { ((byte)objData_) };
            else if (objType_ == typeof(bool))
                serInfo_.DataBytes = BitConverter.GetBytes((bool)objData_);
            else if (objType_ == typeof(char))
            {
                serInfo_.DataBytes = BitConverter.GetBytes((char)objData_);
            }
            else
            {
                bool bNeedNetOrder_ = true;
                byte[] bsData;
                if (objType_ == typeof(short))
                {
                    bsData = BitConverter.GetBytes((short)objData_);
                }
                else if (objType_ == typeof(int))
                {
                    bsData = BitConverter.GetBytes((int)objData_);
                }
                else if (objType_ == typeof(long))
                {
                    bsData = BitConverter.GetBytes((long)objData_);
                }
                else if (objType_ == typeof(ushort))
                {
                    bsData = BitConverter.GetBytes((ushort)objData_);
                }
                else if (objType_ == typeof(uint))
                {
                    bsData = BitConverter.GetBytes((uint)objData_);
                }
                else if (objType_ == typeof(ulong))
                {
                    bsData = BitConverter.GetBytes((ulong)objData_);
                }
                else if (objType_ == typeof(double))
                {
                    bNeedNetOrder_ = false;
                    bsData = BitConverter.GetBytes((double)objData_);
                }
                else if (objType_ == typeof(float))
                {
                    bNeedNetOrder_ = false;
                    bsData = BitConverter.GetBytes((float)objData_);
                }
                else
                {
                    throw new NotSupportedException("SerializeValueType:" + objType_.ToString() + " Can not serialized");
                }

                if (bNeedNetOrder_ && serInfo_.ElementAttribute.UseNetOrder)
                    Array.Reverse(bsData);

                serInfo_.DataBytes = bsData;
            }
        } // SerializeValueType
        #endregion
    } // Class
} // Namespace
