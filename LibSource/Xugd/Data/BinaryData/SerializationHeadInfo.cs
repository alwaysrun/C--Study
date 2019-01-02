using System;

namespace SHCre.Xugd.Data
{
    internal enum XSerializeHeadType
    {
        None,
        Size,
        Count,
    }

    internal class XSerializationHeader
    {
        public byte[] GetHeadData(bool bUseNetOrder_)
        {
            byte[] byData;
            if (ValueType == null || ValueType == typeof(int))
                byData = BitConverter.GetBytes(Value);
            else if (ValueType == typeof(short))
                byData = BitConverter.GetBytes(Convert.ToInt16(Value));
            else if (ValueType == typeof(uint))
                byData = BitConverter.GetBytes(Convert.ToUInt32(Value));
            else if (ValueType == typeof(ushort))
                byData = BitConverter.GetBytes(Convert.ToUInt16(Value));
            else if (ValueType == typeof(byte))
                byData = BitConverter.GetBytes(Convert.ToByte(Value));
            else if (ValueType == typeof(sbyte))
                byData = BitConverter.GetBytes(Convert.ToSByte(Value));
            else
                throw new NotSupportedException("Header type can not " + ValueType.ToString());

            if (bUseNetOrder_)
                Array.Reverse(byData);

            return byData;
        }

        /// <summary>
        /// 数据头的类型：None，Size，Count；
        /// </summary>
        internal XSerializeHeadType HeadType { get; set; }

        /// <summary>
        /// 数据的类型，默认为int
        /// </summary>
        public Type ValueType { get; set; }

        public int Value { get; internal set; }
    }
}
