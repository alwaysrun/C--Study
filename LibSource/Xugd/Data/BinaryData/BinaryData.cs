using System;

namespace SHCre.Xugd.Data
{
    /// <summary>
    /// 自定义序列化属性，只有设定了XBinaryDataAttribute的类才可通过XBinaryDataFormat进行序列化与反序列化。
    /// 只可以序列化C#的基本值类型、枚举类型、string、DateTime、byte[]、数组、列表IList，以及由他们组成的类。
    /// 
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
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class XBinaryDataAttribute : Attribute
    {
        /// <summary>
        /// 时间日期序列化格式，默认"0xyyyyMMddHHmmssff"。
        /// 前缀0x说明序列化为一个Ulong类型的长整数，后面的格式化字符串说明需要保留的精度。
        /// 否则，序列化为字符串，并使用"utf-8"进行编码。
        /// </summary>
        public string DateTimeFormat { get; set; }
        /// <summary>
        /// 对字符串进行的编码方式，默认"utf-8"。
        /// </summary>
        public string EncodingName { get; set; }
        /// <summary>
        /// 是否使用大端格式进行编码，默认false。
        /// </summary>
        public bool UseNetOrder { get; set; }
        /// <summary>
        /// 字符串末尾需要移除的字符
        /// </summary>
        public string TrimEndChars { get; set; }
        /// <summary>
        /// 用于在头部存储序列化后bytes长度的类型。
        /// 默认typeof(int)；
        /// </summary>
        public Type SizeHeadType { get; set; }

        /// <summary>
        /// 类的头类型
        /// </summary>
        internal Type ClassHeadType 
        {
            get { return SizeHeadType == null ? typeof(int) : SizeHeadType; }
        }

        /// <summary>
        /// 表示数组与IList等元素数量头的类型。
        /// 默认typeof(short)
        /// </summary>
        public Type CountHeadType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XBinaryDataAttribute()
        {
            DateTimeFormat = "0xyyyyMMddHHmmssff";
            EncodingName = "utf-8";
            UseNetOrder = false;
            TrimEndChars = "\0";
            SizeHeadType = typeof(int);
            CountHeadType = typeof(short);
        }
    }
}
