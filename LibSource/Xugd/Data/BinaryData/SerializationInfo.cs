using System;
using System.Collections.Generic;
using System.Linq;

namespace SHCre.Xugd.Data
{
    /// <summary>
    /// 序列化的数据：
    /// 如果是简单类型，则序列化后的数据存放在DataBytes中；
    /// 否则，存放在ChildRen列表中。
    /// </summary>
    internal class XSerializationInfo
    {
        /// <summary>
        /// 数据类型
        /// </summary>
        public Type DataType { get; set; }
        /// <summary>
        /// 在数组（列表）中的索引
        /// </summary>
        public int ArrayIndex { get; internal set; }
        /// <summary>
        /// 元素属性
        /// </summary>
        public XBinaryElementAttribute ElementAttribute { get; internal set; }
        /// <summary>
        /// 序列化后的数据头
        /// </summary>
        public XSerializationHeader DataHeader { get; internal set; }
        /// <summary>
        /// 序列化后的数据（只有是简单类型的才存放在此处，否则通过Children获取）
        /// </summary>
        public byte[] DataBytes { get; internal set; }
        /// <summary>
        /// 此元素的父
        /// </summary>
        public XSerializationInfo Parent { get; set; }
        /// <summary>
        /// 孩子元素
        /// </summary>
        public List<XSerializationInfo> Children { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public XSerializationInfo()
        {
            this.Children = new List<XSerializationInfo>();
            this.ArrayIndex = -1;
        }

        internal List<byte[]> GetSerializeList()
        {
            List<byte[]> lstDatas = new List<byte[]>();
            bool bNeedHead = (DataHeader != null && DataHeader.HeadType != XSerializeHeadType.None);
            if (bNeedHead)
                lstDatas.Add(null); // Occupy for head

            // Data
            if (Children != null && Children.Count>0)
            {
                Children.ForEach(z => lstDatas.AddRange(z.GetSerializeList()));
            }
            else
            {
                lstDatas.Add(DataBytes);
            }

            // Header
            if (DataHeader != null && DataHeader.HeadType == XSerializeHeadType.Size)
            {
                DataHeader.Value = lstDatas.Sum(z => z == null ? 0 : z.Length);
            }
            if (bNeedHead)
            {
                byte[] byHead = DataHeader.GetHeadData(ElementAttribute.UseNetOrder);
                lstDatas[0] = byHead;
            }

            return lstDatas;
        }

        /// <summary>
        /// 获取序列化后的数据
        /// </summary>
        /// <returns></returns>
        public byte[] GetSerializeData()
        {
            var lstDatas = GetSerializeList();
            int nSize = lstDatas.Sum(z => z == null ? 0 : z.Length);

            int nOffSet = 0;
            byte[] byData = new byte[nSize];
            foreach (var vData in lstDatas)
            {
                if (vData != null)
                {
                    vData.CopyTo(byData, nOffSet);
                    nOffSet += vData.Length;
                }
            }

            return byData;
        }

        /// <summary>
        /// 设定头类型：不设定则不需要头
        /// </summary>
        /// <param name="euType_"></param>
        /// <param name="valueType_"></param>
        public void SetHeadType(XSerializeHeadType euType_, Type valueType_)
        {
            if (euType_ == XSerializeHeadType.None)
                DataHeader = null;
            else
            {
                DataHeader = new XSerializationHeader()
                {
                    HeadType = euType_,
                    ValueType = valueType_,
                };
            }
        }

        /// <summary>
        /// 设定个数头信息
        /// </summary>
        /// <param name="nCount_"></param>
        /// <param name="valueType_"></param>
        public void SetHeadCount(int nCount_, Type valueType_)
        {
            DataHeader = new XSerializationHeader()
            {
                HeadType = XSerializeHeadType.Count,
                Value = nCount_,
                ValueType = valueType_,
            };
        }

        internal void ReorderMembers()
        {
            //this.Children = (from z in this.Children
            //                 orderby z.ReadIndex
            //                 select z).ToList();
        }
    } // class
}
