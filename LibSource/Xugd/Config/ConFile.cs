using System;
using System.Xml;

namespace SHCre.Xugd.Config
{
    /// <summary>
    /// 用于操作配置文件的路径接口
    /// </summary>
    public static class XConNode
    {
        static readonly char[] _szToTrim = {'\\', '/'};

        /// <summary>
        /// 路径间的分隔符
        /// </summary>
        public const char Separator = '/';

        /// <summary>
        /// 组合路径：如parent1/parent2与sub组合后，返回parent1/parent2/sub
        /// </summary>
        /// <param name="strParent_">父节点路径（必须符合XPath规则）</param>
        /// <param name="strSubNode_">子节点名称（不要有任何的分隔符，如'/'或'\'）</param>
        /// <returns>组合后的完整路径</returns>
        public static string Combine(string strParent_, string strSubNode_)
        {
            return string.Format("{0}/{1}", strParent_, strSubNode_);
        }

        /// <summary>
        /// 在路径上添加属性：如parent/sub下添加属性（名为Name，值为Value），返回parent/sub[@Name='Value']
        /// </summary>
        /// <param name="strPath_">路径（必须符合XPath规则）</param>
        /// <param name="strAttrName_">属性名称</param>
        /// <param name="strAttrValue_">属性的值</param>
        /// <returns>组合后的完整路径</returns>
        public static string Combine(string strPath_, string strAttrName_, string strAttrValue_)
        {
            return string.Format("{0}[@{1}=\'{2}\']", strPath_, strAttrName_, strAttrValue_);
        }

        /// <summary>
        /// 组合路径并添加属性：如parent1/parent2与sub下添加属性（名为Name，值为Value），
        /// 返回parent1/parent2/sub[@Name='Value']
        /// </summary>
        /// <param name="strParent_">父节点路径（必须符合XPath规则）</param>
        /// <param name="strSubNode_">子节点名称（不要有任何的分隔符，如'/'或'\'）</param>
        /// <param name="strAttrName_">属性名称</param>
        /// <param name="strAttrValue_">属性的值</param>
        /// <returns>组合后的完整路径</returns>
        public static string Combine(string strParent_, string strSubNode_, string strAttrName_, string strAttrValue_)
        {
            return string.Format("{0}/{1}[@{2}=\'{3}\']", strParent_, strSubNode_, strAttrName_, strAttrValue_);
        }
    };

    /// <summary>
    /// 用于读取XML配置文件的类：打开配置文件，读取节点内容以及读取节点属性。
    /// &lt;XNodes&gt;
    ///     &lt;Form Name="FrmMain"&gt;
    ///         &lt;NodeText&gt;Contents&lt;/NodeText&gt;
    ///     .....
    ///     &lt;/Form&gt;
    /// &lt;/XNodes&gt;
    /// 
    /// 配置文件要满足如上格式；我们提取的内容从Form开始。
    /// 如用于获取Contents的路径为"/Form[@Name='FrmMain']/NodeText"。
    /// 路径用反斜线'/'分割，属性前面需要添加一个@符号
    /// 并且最外层路径在内部处理掉，输入时要从第二层路径开始。
    /// </summary>
    public class XConReader
    {
        private XmlNode _Root = null;
        private const string _strName = "Name";

        /// <summary>
        /// 缺省构造函数，不做任何操作
        /// </summary>
        public XConReader() { }

        /// <summary>
        /// 构造函数，并打开配置文件（.XML）；出错时，抛出异常
        /// </summary>
        /// <param name="strFile_">配置文件名（包括完整路径）</param>
        public XConReader(string strFile_)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(strFile_);
            _Root = xmlDoc.DocumentElement;
        }

        /// <summary>
        /// 打开配置文件（.XML）
        /// </summary>
        /// <param name="strFile_">配置文件名（包括完整路径）</param>
        /// <returns>成功，返回true；否则，返回false</returns>
        public bool Open(string strFile_)
        {
            bool bSuccess = false;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(strFile_);
                _Root = xmlDoc.DocumentElement;
                bSuccess = true;
            }
            catch
            {
            }

            return bSuccess;
        }

        /// <summary>
        /// 释放占用的内存，调用后，其他后续函数都将无效（不能再调用，除非重新Open）
        /// </summary>
        public void Close()
        {
            _Root = null;
        }

        /// <summary>
        /// 获取指定节点的属性为‘Name’的内容，需要保证配置文件已打开；出错时，抛出异常
        /// </summary>
        /// <param name="strNode_">要获取内容的节点路径；需要符合XPath规则</param>
        /// <returns>属性为‘Name’的内容</returns>
        public string GetName(string strNode_)
        {
            return GetAttr(strNode_, _strName);
        }

        /// <summary>
        /// 获取指定节点的属性为‘Name’的内容，需要保证配置文件已打开
        /// </summary>
        /// <param name="strNode_">要获取内容的节点路径；需要符合XPath规则</param>
        /// <param name="strDefault_">获取失败时，返回的内容</param>
        /// <returns>属性为‘Name’的内容；如果获取失败，返回strDefault_</returns>                
        public string GetName(string strNode_, string strDefault_)
        {
            return GetAttr(strNode_, _strName, strDefault_);
        }

        /// <summary>
        /// 获取所有节点的属性为‘Name’的内容，需要保证配置文件已打开
        /// </summary>
        /// <param name="strNode_">要获取内容的节点路径；需要符合XPath规则</param>
        /// <returns>属性为‘Name’的内容</returns>
        public string[] GetAllName(string strNode_)
        {
            return GetAllAttr(strNode_, _strName);
        }

        /// <summary>
        /// 获取指定节点下的内容，需要保证配置文件已打开；出错时，抛出异常
        /// </summary>
        /// <param name="strNode_">要获取内容的节点路径；需要符合XPath规则</param>
        /// <returns>获取到的内容</returns>
        public string GetText(string strNode_)
        {
            XmlNode nodElem = _Root.SelectSingleNode(strNode_);
            return (nodElem.InnerText);
        }

        /// <summary>
        /// 获取指定节点下的内容，需要保证配置文件已打开
        /// </summary>
        /// <param name="strNode_">要获取内容的节点路径；需要符合XPath规则</param>
        /// <param name="strDefault_">获取失败时，返回的内容</param>
        /// <returns>获取到的内容；如果获取失败，返回strDefault_</returns>
        public string GetText(string strNode_, string strDefault_)
        {
            try
            {
                XmlNode nodElem = _Root.SelectSingleNode(strNode_);
                return (nodElem.InnerText);
            }
            catch
            {
            }
            return strDefault_;
        }

        /// <summary>
        /// 获取指定节点下所有内容，需要保证配置文件已打开；出错时，抛出异常
        /// </summary>
        /// <param name="strNode_">要获取内容的节点路径；需要符合XPath规则</param>
        /// <returns>获取到的内容</returns>
        public string[] GetAllText(string strNode_)
        {
            XmlNodeList listElems = _Root.SelectNodes(strNode_);
            string[] strTexts = new string[listElems.Count];
            for (int i = 0; i < listElems.Count; ++i)
            {
                strTexts[i] = listElems.Item(i).InnerText;
            }

            return strTexts;
        }

        /// <summary>
        /// 获取满足属性的指定节点下的所有内容，需要保证配置文件已打开；出错时，抛出异常
        /// </summary>
        /// <param name="strNode_">要获取内容的节点路径；需要符合XPath规则</param>
        /// <param name="strAttrName_">要满足属性的名称</param>
        /// <param name="strAttrValue_">要满足属性的值</param>
        /// <returns>获取到的内容</returns>
        public string[] GetTextByAttr(string strNode_, string strAttrName_, string strAttrValue_)
        {
            string strPath = strNode_.Trim() + "[@" + strAttrName_.Trim() + "=\"" + strAttrValue_.Trim() + "\"]";
            XmlNodeList lstElems = _Root.SelectNodes(strPath);
            string[] strTexts = new string[lstElems.Count];
            int nIndex = 0;
            foreach(XmlNode node in lstElems)
            {
                strTexts[nIndex++] = node.InnerText;
            }

            return strTexts;
        }

        /// <summary>
        /// 获取指定节点的属性，需要保证配置文件已打开；出错时，抛出异常
        /// </summary>
        /// <param name="strNode_">要获取属性的节点路径；需要符合XPath规则</param>
        /// <param name="strName_">属性名称</param>
        /// <returns>获取的属性值</returns>
        public string GetAttr(string strNode_, string strName_)
        {

            XmlNode nodAttr = _Root.SelectSingleNode(strNode_).Attributes.GetNamedItem(strName_);
            return (nodAttr.Value);
        }

        /// <summary>
        /// 获取指定节点的属性，需要保证配置文件已打开
        /// </summary>
        /// <param name="strNode_">要获取属性的节点路径；需要符合XPath规则</param>
        /// <param name="strName_">属性名称</param>
        /// <param name="strDefault_">获取失败时，返回的属性</param>
        /// <returns>获取的属性值；如果获取失败，则返回strDefault_</returns>
        public string GetAttr(string strNode_, string strName_, string strDefault_)
        {
            try
            {
                XmlNode nodAttr = _Root.SelectSingleNode(strNode_).Attributes.GetNamedItem(strName_);
                return (nodAttr.Value);
            }
            catch
            {
            }

            return strDefault_;
        }

        /// <summary>
        /// 获取Bool类型的属性
        /// </summary>
        /// <param name="strNode_">要获取属性的节点路径；需要符合XPath规则</param>
        /// <param name="strName_">属性名称</param>
        /// <returns>值为非零整数，返回true；否则，返回false</returns>
        public bool GetBoolAttr(string strNode_, string strName_)
        {
            string strAttr = GetAttr(strNode_, strName_, "0");
            uint nAttr = 0;
            uint.TryParse(strAttr, out nAttr);

            return (nAttr != 0);
        }

        /// <summary>
        /// 获取uint（以十六进制表示）类型的属性
        /// </summary>
        /// <param name="strNode_">要获取属性的节点路径；需要符合XPath规则</param>
        /// <param name="strName_">属性名称</param>
        /// <returns>值为非零整数，返回true；否则，返回false</returns>
        public uint GetHexAttr(string strNode_, string strName_)
        {
            string strHex = GetAttr(strNode_, strName_, "0");

            if (strHex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                strHex = strHex.Substring(2);

            return uint.Parse(strHex, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// 获取指定节点的所有指定属性，需要保证配置文件已打开；出错时，抛出异常
        /// </summary>
        /// <param name="strNode_">要获取属性的节点路径；需要符合XPath规则</param>
        /// <param name="strName_">属性名称</param>
        /// <returns>获取的属性值</returns>
        public string[] GetAllAttr(string strNode_, string strName_)
        {

            XmlNodeList listAttrs = _Root.SelectNodes(strNode_);
            string[] strAttrs = new string[listAttrs.Count];
            for (int i = 0; i < listAttrs.Count; ++i)
            {
                strAttrs[i] = listAttrs[i].Attributes.GetNamedItem(strName_).Value;
            }

            return strAttrs;
        }
    }; // class XConReader

    /// <summary>
    /// 创建用于生成xml文件的类
    /// 生成的实例要在完成时要关闭（可以在using中创建实例）
    /// &lt;XNodes&gt;
    ///     .....
    /// &lt;/XNodes&gt;
    /// </summary>
    public class XConWriter : IDisposable
    {
        XmlWriter _xmlFile = null;
        const string c_strTopElement = "XNodes";
        private const string _strName = "Name";

        /// <summary>
        /// 缺省构造函数
        /// </summary>
        public XConWriter() { }

        /// <summary>
        /// 生成已打开文件的实例，出错会抛出异常
        /// </summary>
        /// <param name="strFile_">要创建的xml文件名（全路径）</param>
        public XConWriter(string strFile_)
        {
            XmlWriterSettings xmlSetting = new XmlWriterSettings();
            xmlSetting.Indent = true;
            _xmlFile = XmlWriter.Create(strFile_, xmlSetting);
            _xmlFile.WriteStartDocument();
            _xmlFile.WriteStartElement(c_strTopElement);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~XConWriter()
        {
            Dispose(false);
        }

        /// <summary>
        /// 创建xml文件
        /// </summary>
        /// <param name="strFile_">要创建的xml文件名（全路径）</param>
        /// <returns>成功返回ture；否则false</returns>
        public bool Create(string strFile_)
        {
            bool bSuccess = false;
            try
            {
                XmlWriterSettings xmlSetting = new XmlWriterSettings();
                xmlSetting.Indent = true;
                _xmlFile = XmlWriter.Create(strFile_, xmlSetting);
                _xmlFile.WriteStartDocument();
                _xmlFile.WriteStartElement(c_strTopElement);

                bSuccess = true;
            }
            catch (Exception) { }

            return bSuccess;
        }

        /// <summary>
        /// 在当前节点中添加'Name'属性，出错时抛出异常
        /// </summary>
        /// <param name="strValue_">属性值</param>
        public void WriteName(string strValue_)
        {
            WriteAttr(_strName, strValue_);
        }

        /// <summary>
        /// 在当前节点下创建包含'Name'属性的新节点，出错时抛出异常
        /// </summary>
        /// <param name="strNode_">新节点名称</param>
        /// <param name="strValue_">属性值</param>
        public void WriteName(string strNode_, string strValue_)
        {
            WriteAttr(strNode_, _strName, strValue_);
        }

        /// <summary>
        /// 在当前节点中添加属性，出错时抛出异常
        /// </summary>
        /// <param name="strAttr_">属性名称</param>
        /// <param name="strValue_">属性值</param>
        public void WriteAttr(string strAttr_, string strValue_)
        {
            _xmlFile.WriteStartAttribute(strAttr_);
            _xmlFile.WriteString(strValue_);
            _xmlFile.WriteEndAttribute();
        }

        /// <summary>
        /// 在当前节点中添加属性，出错时抛出异常
        /// </summary>
        /// <param name="strAttr_">属性名称</param>
        /// <param name="bValue_">属性值</param>
        public void WriteAttr(string strAttr_, bool bValue_)
        {
            _xmlFile.WriteStartAttribute(strAttr_);
            _xmlFile.WriteString(bValue_ ? "1" : "0");
            _xmlFile.WriteEndAttribute();
        }

        /// <summary>
        /// 在当前节点中添加十六进制值的属性，出错时抛出异常
        /// </summary>
        /// <param name="strAttr_">属性名称</param>
        /// <param name="nValue_">属性值</param>
        public void WriteAttr(string strAttr_, uint nValue_)
        {
            _xmlFile.WriteStartAttribute(strAttr_);
            _xmlFile.WriteString("0x" + nValue_.ToString("X"));
            _xmlFile.WriteEndAttribute();
        }

        /// <summary>
        /// 在当前节点下创建包含属性值的新节点，出错时抛出异常
        /// </summary>
        /// <param name="strNode_">新节点名称</param>
        /// <param name="strAttr_">属性名称</param>
        /// <param name="strValue_">属性值</param>
        public void WriteAttr(string strNode_, string strAttr_, string strValue_)
        {
            _xmlFile.WriteStartElement(strNode_);
            WriteAttr(strAttr_, strValue_);
            _xmlFile.WriteEndElement();
        }

        /// <summary>
        /// 在当前节点下创建包含属性值的新节点，出错时抛出异常
        /// </summary>
        /// <param name="strNode_">新节点名称</param>
        /// <param name="strAttr_">属性名称</param>
        /// <param name="bValue_">属性值</param>
        public void WriteAttr(string strNode_, string strAttr_, bool bValue_)
        {
            _xmlFile.WriteStartElement(strNode_);
            WriteAttr(strAttr_, bValue_);
            _xmlFile.WriteEndElement();
        }

        /// <summary>
        /// 在当前节点下创建包含十六进制值属性值的新节点，出错时抛出异常
        /// </summary>
        /// <param name="strNode_">新节点名称</param>
        /// <param name="strAttr_">属性名称</param>
        /// <param name="nValue_">属性值</param>
        public void WriteAttr(string strNode_, string strAttr_, uint nValue_)
        {
            _xmlFile.WriteStartElement(strNode_);
            WriteAttr(strAttr_, nValue_);
            _xmlFile.WriteEndElement();
        }

        /// <summary>
        /// 在当前节点下添加文本，出错时抛出异常
        /// </summary>
        /// <param name="strText_">文本内容</param>
        public void WriteText(string strText_)
        {
            _xmlFile.WriteString(strText_);
        }

        /// <summary>
        /// 在当前节点下创建包含文本的新的节点，出错时抛出异常
        /// </summary>
        /// <param name="strNode_">新节点名称</param>
        /// <param name="strText_">文本内容</param>
        public void WriteText(string strNode_, string strText_)
        {
            _xmlFile.WriteStartElement(strNode_);
            _xmlFile.WriteString(strText_);
            _xmlFile.WriteEndElement();
        }

        /// <summary>
        /// 在当前节点下创建包含属性和文本文本的新的节点，出错时抛出异常
        /// </summary>
        /// <param name="strNode_">新节点名称</param>
        /// <param name="strAttr_">属性名称</param>
        /// <param name="strValue_">属性值</param>
        /// <param name="strText_">文本内容</param>
        public void WriteBoth(string strNode_, string strAttr_, string strValue_, string strText_)
        {
            _xmlFile.WriteStartElement(strNode_);
            WriteAttr(strAttr_, strValue_);
            _xmlFile.WriteString(strText_);
            _xmlFile.WriteEndElement();
        }

        /// <summary>
        /// 在当前节点下创建包含属性和文本文本的新的节点，出错时抛出异常
        /// </summary>
        /// <param name="strNode_">新节点名称</param>
        /// <param name="strAttr_">属性名称</param>
        /// <param name="bValue_">属性值</param>
        /// <param name="strText_">文本内容</param>
        public void WriteBoth(string strNode_, string strAttr_, bool bValue_, string strText_)
        {
            _xmlFile.WriteStartElement(strNode_);
            WriteAttr(strAttr_, bValue_);
            _xmlFile.WriteString(strText_);
            _xmlFile.WriteEndElement();
        }

        /// <summary>
        /// 在当前节点下创建包含十六进制值属性和文本文本的新的节点，出错时抛出异常
        /// </summary>
        /// <param name="strNode_">新节点名称</param>
        /// <param name="strAttr_">属性名称</param>
        /// <param name="nValue_">属性值</param>
        /// <param name="strText_">文本内容</param>
        public void WriteBoth(string strNode_, string strAttr_, uint nValue_, string strText_)
        {
            _xmlFile.WriteStartElement(strNode_);
            WriteAttr(strAttr_, nValue_);
            _xmlFile.WriteString(strText_);
            _xmlFile.WriteEndElement();
        }

        /// <summary>
        /// 在当前节点下创建新的节点，出错时抛出异常
        /// 可以在其中添加属性与文本以及新的节点；完成后需要使用EndNode关闭
        /// </summary>
        /// <param name="strNode_">新节点名称</param>
        public void StartNode(string strNode_)
        {
            _xmlFile.WriteStartElement(strNode_);
        }

        /// <summary>
        /// 在当前节点下创建新的节点并设定属性，出错时抛出异常
        /// 可以在其中添加属性与文本以及新的节点；完成后需要使用EndNode关闭
        /// </summary>
        /// <param name="strNode_">新节点名称</param>
        /// <param name="strAttr_">属性名称</param>
        /// <param name="strValue_">属性值</param>
        public void StartNode(string strNode_, string strAttr_, string strValue_)
        {
            StartNode(strNode_);
            WriteAttr(strAttr_, strValue_);
        }

        /// <summary>
        /// 关闭当前节点，并返回当上一级节点
        /// </summary>
        public void EndNode()
        {
            _xmlFile.WriteEndElement();
        }

        /// <summary>
        /// 关闭已打开的文件
        /// </summary>
        public void Close()
        {
            if (null != _xmlFile)
            {
                try
                {
                    _xmlFile.WriteEndElement();
                    _xmlFile.WriteEndDocument();
                    _xmlFile.Flush();
                    _xmlFile.Close();
                    _xmlFile = null;
                }
                catch (Exception) { }
            }
        }

        /// <summary>
        /// 析构处理，如果文件已打开，则关闭之
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        private void Dispose(bool bDispose_)
        {
            Close();
        }
    }; // Class XConWriter
}
