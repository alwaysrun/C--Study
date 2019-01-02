using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SHCre.Xugd.Extension
{
    partial class XKeyValueList
    {
        /// <summary>
        /// 键值对
        /// </summary>
        public class KeyValuePair
        {
            [XmlIgnore]
            private char _chSplitor;

            /// <summary>
            /// 字符串表示时的分隔符（默认为=，即key=value）
            /// </summary>
            [XmlIgnore]
            public char Splitor
            {
                get { return _chSplitor; }
                set
                {
                    _chSplitor = value;
                }
            }

            /// <summary>
            /// 键
            /// </summary>
            [XmlAttribute]
            public string Key { get; set; }

            /// <summary>
            /// 值
            /// </summary>
            [XmlAttribute]
            public string Value {get; set;}

            /// <summary>
            /// 
            /// </summary>
            /// <param name="chSplitor_"></param>
            public KeyValuePair(char chSplitor_ = '=')
            {
                Key = Value = string.Empty;
                _chSplitor = chSplitor_;
            }

            /// <summary>
            /// 根据输入的键、值构造键值对
            /// </summary>
            /// <param name="strKey_"></param>
            /// <param name="strValue_"></param>
            /// <param name="chSplitor_"></param>
            public KeyValuePair(string strKey_, string strValue_, char chSplitor_ = '=')
            {
                Key = strKey_;
                Value = strValue_;
                _chSplitor = chSplitor_;
            }

            /// <summary>
            /// 从字符串中分析出键值对：
            /// 如果没有内容则抛出ArgumentException；
            /// 如果没有分隔符（=），则值为空
            /// </summary>
            /// <param name="strKeyValue_"></param>
            /// <param name="chSplitor_"></param>
            /// <returns></returns>
            public static KeyValuePair Parse(string strKeyValue_, char chSplitor_='=')
            {
                if(string.IsNullOrEmpty(strKeyValue_))
                    throw new ArgumentException("KeyValue(like key=value) must set");

                string[] aryData = strKeyValue_.Split(new char[] { chSplitor_ }, 2, StringSplitOptions.RemoveEmptyEntries);
                if(aryData.Length==0)
                    throw new ArgumentException("KeyValue(like key=value) must set");

                // Set Keyvaluedata
                KeyValuePair keyValue = new KeyValuePair(aryData[0], string.Empty);
                if(aryData.Length>1)
                    keyValue.Value = aryData[1];

                return keyValue;
            }

            /// <summary>
            /// 字符串表示：
            /// 如Key=Value
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return string.Format("{0}{1}{2}", Key, Splitor, Value);
            }
        }
    }
}
