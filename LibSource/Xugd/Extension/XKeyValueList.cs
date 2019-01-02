using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SHCre.Xugd.Extension
{
    /// <summary>
    /// 键值对列表
    /// </summary>
    public partial class XKeyValueList
    {
        /// <summary>
        /// 字符串表示时，键值对间的分隔符
        /// </summary>
        [XmlAttribute]
        public char DataSplitor {get; set;}
        /// <summary>
        /// 字符串表示时，键与值间的分隔符
        /// </summary>
        [XmlAttribute]
        public char KeyValueSplitor {get; set;}
        /// <summary>
        /// 键比较时，是否区分大小写
        /// </summary>
        [XmlAttribute]
        public bool IgnoreCase {get; set;}

        XSafeList<KeyValuePair> _lstData = new XSafeList<KeyValuePair>();
        /// <summary>
        /// 返回整个列表的一个副本
        /// </summary>
        public List<KeyValuePair> Collect
        {
            get { return _lstData.Collect; }
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        /// <param name="bIgnoreCase_"></param>
        /// <param name="chDataSplitor_"></param>
        /// <param name="chKeyValueSplitor_"></param>
        public XKeyValueList(bool bIgnoreCase_ = true, char chDataSplitor_ = '\n', char chKeyValueSplitor_ = '=')
        {
            DataSplitor = chDataSplitor_;
            KeyValueSplitor = chKeyValueSplitor_;
            IgnoreCase = bIgnoreCase_;
        }

        /// <summary>
        /// 设定键值对：
        /// 如果Key为空，则不做任何操作；
        /// 如果Value为空，则为移除键值对；
        /// 否则，如果存在则更新，否则添加。
        /// </summary>
        /// <param name="strKey_"></param>
        /// <param name="strValue_"></param>
        public void Set(string strKey_, string strValue_)
        {
            if (string.IsNullOrEmpty(strKey_))
                return;

            var firstData = _lstData.FirstOrDefault(z => string.Compare(z.Key, strKey_, IgnoreCase) == 0);
            if(firstData==null)
            {
                if(!string.IsNullOrEmpty(strValue_))
                    _lstData.Add(new KeyValuePair(strKey_, strValue_, KeyValueSplitor));                   
            }
            else
            {
                if (string.IsNullOrEmpty(strValue_))
                    _lstData.Remove(firstData);
                else
                    firstData.Value = strValue_;
            }
        }

        /// <summary>
        /// 是否存在指定的键
        /// </summary>
        /// <param name="strKey_"></param>
        /// <returns></returns>
        public bool Contains(string strKey_)
        {
            return _lstData.Contains(z => string.Compare(z.Key, strKey_, IgnoreCase) == 0);
        }

        /// <summary>
        /// 添加键值对：直接添加，不做任何判断
        /// </summary>
        /// <param name="kvData_"></param>
        public void Add(KeyValuePair kvData_)
        {
            _lstData.Add(kvData_);
        }

        /// <summary>
        /// 添加多个键值对：不做任何判断直接添加
        /// </summary>
        /// <param name="colDatas_"></param>
        public void AddRange(IEnumerable<KeyValuePair> colDatas_)
        {
            _lstData.AddRange(colDatas_);
        }

        /// <summary>
        /// 清空键值对
        /// </summary>
        public void Clear()
        {
            _lstData.Clear();
        }

        /// <summary>
        /// 字符串表示信息：
        /// 如：k1=v1;k2=v2
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join(DataSplitor.ToString(), _lstData.Collect); ;
        }

        /// <summary>
        /// 从字符串中构造键值对列表
        /// </summary>
        /// <param name="strList_"></param>
        /// <param name="bIgnoreCase_"></param>
        /// <param name="chDataSplitor_"></param>
        /// <param name="chKeyValueSplitor_"></param>
        /// <returns></returns>
        public static XKeyValueList Parse(string strList_, bool bIgnoreCase_=true, char chDataSplitor_='\n', char chKeyValueSplitor_='=')
        {
            XKeyValueList dataList = new XKeyValueList()
            {
                DataSplitor = chDataSplitor_,
                KeyValueSplitor = chKeyValueSplitor_,
                IgnoreCase = bIgnoreCase_,
            };

            string[] aryData = strList_.Split(new char[]{chDataSplitor_}, StringSplitOptions.RemoveEmptyEntries);
            foreach(string str in aryData)
            {
                dataList.Add(KeyValuePair.Parse(str, chKeyValueSplitor_));
            }

            return dataList;
        }
    }
}
