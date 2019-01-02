using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.Data
{
    /// <summary>
    /// 数据序列化类型信息
    /// </summary>
    internal class XBinaryDataTypeInfo
    {
        /// <summary>
        /// 对应的序列化属性
        /// </summary>
        public XBinaryDataAttribute DataAttribute { get; private set; }
        /// <summary>
        /// 根据SeqIndex排序后的元素列表
        /// </summary>
        public List<XBinaryElementInfo> OrderItems { get; internal set; }
        /// <summary>
        /// 序列化时的元素列表（根据依赖关系调整后的OrderItems）
        /// </summary>
        internal List<XBinaryElementInfo> SerializeItems { get; private set; }
        /// <summary>
        /// 对应的类型
        /// </summary>
        public Type DataType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type_"></param>
        /// <param name="bdAttri_"></param>
        public XBinaryDataTypeInfo(Type type_, XBinaryDataAttribute bdAttri_)
        {
            this.OrderItems = new List<XBinaryElementInfo>();
            this.SerializeItems = new List<XBinaryElementInfo>();
            this.DataType = type_;
            this.DataAttribute = bdAttri_;
        }

        private int IsSerializeDependOn(int nIndex_)
        {
            //XBinaryElementInfo beInfo = this.SerializeItems[nIndex_];
            //for (int i = nIndex_ + 1; i < this.SerializeItems.Count; ++i)
            //{
            //    XBinaryElementInfo info = this.SerializeItems[i];
            //    if (beInfo.IsSerializeDependOn(info))
            //        return i;
            //}

            return -1;
        }

        internal void ReordDataElements()
        {
            if (this.OrderItems.FirstOrDefault<XBinaryElementInfo>(z => z.OrderIndex > 0) != null)
            {
                if (this.OrderItems.FirstOrDefault<XBinaryElementInfo>(z => z.OrderIndex < 0) != null)
                    throw new XBinaryDataException("OrderIndex cannot be set partly: " + this.DataType.ToString(), SHErrorCode.InvalidIndex);

                var orderElements = (from x in this.OrderItems group x by x.OrderIndex);
                if (orderElements.FirstOrDefault(z => z.Count() > 1) != null)
                    throw new XBinaryDataException("OrderIndex not unique: " + this.DataType.ToString(), SHErrorCode.InvalidIndex);

                this.OrderItems = (from z in this.OrderItems orderby z.OrderIndex select z).ToList();
            }

            for (int i = 0; i < this.OrderItems.Count; ++i)
                this.OrderItems[i].ReadIndex = i;

            this.SerializeItems.AddRange(this.OrderItems);

            //while (!RecordEleSerializeInfos()) ;
        }

        private bool RecordEleSerializeInfos()
        {
            for (int i = 0; i < this.SerializeItems.Count; ++i)
            {
                int nIndex = this.IsSerializeDependOn(i);
                if (nIndex != -1)
                {
                    XBinaryElementInfo ele = this.SerializeItems[i];
                    this.SerializeItems.RemoveAt(i);
                    this.SerializeItems.Insert(nIndex, ele);
                    return false;
                }
            }

            return true;
        }
    } //XBinaryDataTypeInfo
}
