using System.Reflection;

namespace SHCre.Xugd.Data
{
    /// <summary>
    /// 序列化时，类中元素类型信息
    /// </summary>
    internal class XBinaryElementInfo
    {
        // Property
        /// <summary>
        /// 对应成员信息
        /// </summary>
        public MemberInfo SelfInfo { get; private set; }
        /// <summary>
        /// 用于排序的索引
        /// </summary>
        public int OrderIndex { get; private set; }
        /// <summary>
        /// 序列化读取索引
        /// </summary>
        internal int ReadIndex { get; set; }
        /// <summary>
        /// 序列化属性
        /// </summary>
        public XBinaryElementAttribute ElementAttribute { get; set; }
        /// <summary>
        /// 父的类型信息
        /// </summary>
        public XBinaryDataTypeInfo ParentTypeInfo { get; private set; }

        public XBinaryElementInfo(XBinaryDataTypeInfo dataTypeInfo_, XBinaryElementAttribute beAttri_, MemberInfo mInfo_)
        {
            this.ParentTypeInfo = dataTypeInfo_;
            this.ElementAttribute = beAttri_;
            this.SelfInfo = mInfo_;
            //this.ParseElementAttribute();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string strInfo = string.Empty;
            strInfo += this.ParentTypeInfo.DataType.ToString();
            if (this.SelfInfo != null)
                strInfo += "." + this.SelfInfo.Name;

            return strInfo;
        }
    } // class
}
