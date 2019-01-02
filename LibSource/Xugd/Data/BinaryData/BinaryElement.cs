using System;
using System.Text;

namespace SHCre.Xugd.Data
{
    /// <summary>
    /// 需要序列化成员的自定义属性：
    /// 只有有此属性的成员才会被序列化与反序列化
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true)]
    public class XBinaryElementAttribute : Attribute
    {
        /// <summary>
        /// 转换后字节数组长度,如果需要固定长度则设定,否则不要设定。
        /// 只对byte[]与string有效
        /// </summary>
        public int FixedSize { get; set; }
        /// <summary>
        /// 只对数组与列表（List）有效，表示子元素是否为固定长度。
        /// 只有在子元素为byte[]与string时才有效
        /// </summary>
        public int ItemFixedSize { get; set; }

        string _strDatetimeFormat;
        /// <summary>
        /// 时间日期序列化格式，默认继承XBinaryDataAttribute的。
        /// 前缀0x说明序列化为一个Ulong类型的长整数，后面的格式化字符串说明需要保留的精度。
        /// 否则，序列化为字符串，并使用"utf-8"进行编码。
        /// </summary>
        public string DateTimeFormat
        {
            get
            {
                if (string.IsNullOrEmpty(_strDatetimeFormat))
                {
                    if (DataAttribute != null)
                        _strDatetimeFormat = DataAttribute.DateTimeFormat;
                    if (string.IsNullOrEmpty(_strDatetimeFormat))
                        _strDatetimeFormat = "0xyyyyMMddHHmmssff";
                }

                return _strDatetimeFormat;
            }

            set { _strDatetimeFormat = value; }
        }
        /// <summary>
        /// 对字符串进行的编码方式，默认继承XBinaryDataAttribute的。
        /// </summary>
        public string EncodingName { get; set; }

        char[] _aryTrims = null;
        internal char[] TrimChars
        {
            get 
            {
                if(_aryTrims == null)
                {
                    if (!string.IsNullOrEmpty(EndCharsToTrim))
                        _aryTrims = EndCharsToTrim.ToCharArray();
                    if (_aryTrims == null)
                        _aryTrims = new char[0];
                }

                return _aryTrims;
            }
        }
        /// <summary>
        /// 字符串末尾需要移除的字符
        /// </summary>
        public string EndCharsToTrim { get; set; }
        /// <summary>
        /// 元素在类中的索引，默认-1。
        /// 如果设定则需要唯一且大于等于0；
        /// 类中序列化的元素要么全部设定，要么全部不设定。
        /// </summary>
        public int SeqIndex { get; set; }

        private Type _countType;
        /// <summary>
        /// 表示数组与List等元素数量头的类型。
        /// 不设定则继承XBinaryDataAttribute
        /// </summary>
        public Type CountHeadType
        {
            get
            {
                if (_countType == null)
                {
                    if (DataAttribute != null)
                        _countType = DataAttribute.CountHeadType;

                    if (_countType == null)
                        _countType = typeof(short);
                }

                return _countType;
            }

            set { _countType = value; }
        }

        private Type _sizeType;
        /// <summary>
        /// 在头部添加序列化后长度的类型。
        /// 不设定则继承XBinaryDataAttribute
        /// </summary>
        public Type SizeHeadType
        {
            get
            {
                if (_sizeType == null)
                {
                    if (DataAttribute != null)
                        _sizeType = DataAttribute.SizeHeadType;
                    if (_sizeType == null)
                        _sizeType = typeof(int);
                }

                return _sizeType;
            }

            set { _sizeType = value; }
        }

        ///// <summary>
        ///// 对于数组与列表是用Count（数组长度）还是Size（序列化后的Bytes长度）；
        ///// 默认使用Count
        ///// </summary>
        //public bool UseSizeForArray { get; set; }
        /// <summary>
        /// 此元素所在序列化类的序列化属性
        /// </summary>
        internal XBinaryDataAttribute DataAttribute { get; set; }

        Encoding _Encoding;
        /// <summary>
        /// 字符编码格式
        /// </summary>
        internal Encoding StringEncoding
        {
            get
            {
                if (_Encoding == null)
                {
                    string strName = EncodingName;
                    if (string.IsNullOrEmpty(strName) && DataAttribute != null)
                        strName = DataAttribute.EncodingName;

                    try
                    {
                        _Encoding = Encoding.GetEncoding(strName);
                    }
                    catch
                    {
                        _Encoding = Encoding.UTF8;
                    }
                }

                return _Encoding;
            }
        }

        /// <summary>
        /// 是否使用大端序列化
        /// </summary>
        public bool UseNetOrder
        {
            get
            {
                return DataAttribute == null ? true : DataAttribute.UseNetOrder;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public XBinaryElementAttribute()
        {
            this.FixedSize = -1;
            this.ItemFixedSize = -1;
            //this.UseSizeForArray = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nSeq_"></param>
        public XBinaryElementAttribute(int nSeq_)
            : this()
        {
            this.SeqIndex = nSeq_;
        }

        /// <summary>
        /// 获取子元素的属性：用ItemFixedSize设置FixedSize
        /// </summary>
        /// <returns></returns>
        public XBinaryElementAttribute GetItemAttribute()
        {
            XBinaryElementAttribute newAttri = new XBinaryElementAttribute()
            {
                DateTimeFormat = this.DateTimeFormat,
                EncodingName = this.EncodingName,
                DataAttribute = this.DataAttribute,
                EndCharsToTrim = this.EndCharsToTrim,
                FixedSize = this.ItemFixedSize,
            };

            return newAttri;
        }

        /// <summary>
        /// 用于类自身的属性信息
        /// </summary>
        /// <param name="dataAttr_"></param>
        /// <returns></returns>
        public static XBinaryElementAttribute GetClassAttribute(XBinaryDataAttribute dataAttr_)
        {
            XBinaryElementAttribute newAttr = new XBinaryElementAttribute()
            {
                DataAttribute = dataAttr_,
            };

            return newAttr;
        }
    }
}
