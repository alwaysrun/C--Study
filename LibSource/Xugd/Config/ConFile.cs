using System;
using System.Xml;

namespace SHCre.Xugd.Config
{
    /// <summary>
    /// ���ڲ��������ļ���·���ӿ�
    /// </summary>
    public static class XConNode
    {
        static readonly char[] _szToTrim = {'\\', '/'};

        /// <summary>
        /// ·����ķָ���
        /// </summary>
        public const char Separator = '/';

        /// <summary>
        /// ���·������parent1/parent2��sub��Ϻ󣬷���parent1/parent2/sub
        /// </summary>
        /// <param name="strParent_">���ڵ�·�����������XPath����</param>
        /// <param name="strSubNode_">�ӽڵ����ƣ���Ҫ���κεķָ�������'/'��'\'��</param>
        /// <returns>��Ϻ������·��</returns>
        public static string Combine(string strParent_, string strSubNode_)
        {
            return string.Format("{0}/{1}", strParent_, strSubNode_);
        }

        /// <summary>
        /// ��·����������ԣ���parent/sub��������ԣ���ΪName��ֵΪValue��������parent/sub[@Name='Value']
        /// </summary>
        /// <param name="strPath_">·�����������XPath����</param>
        /// <param name="strAttrName_">��������</param>
        /// <param name="strAttrValue_">���Ե�ֵ</param>
        /// <returns>��Ϻ������·��</returns>
        public static string Combine(string strPath_, string strAttrName_, string strAttrValue_)
        {
            return string.Format("{0}[@{1}=\'{2}\']", strPath_, strAttrName_, strAttrValue_);
        }

        /// <summary>
        /// ���·����������ԣ���parent1/parent2��sub��������ԣ���ΪName��ֵΪValue����
        /// ����parent1/parent2/sub[@Name='Value']
        /// </summary>
        /// <param name="strParent_">���ڵ�·�����������XPath����</param>
        /// <param name="strSubNode_">�ӽڵ����ƣ���Ҫ���κεķָ�������'/'��'\'��</param>
        /// <param name="strAttrName_">��������</param>
        /// <param name="strAttrValue_">���Ե�ֵ</param>
        /// <returns>��Ϻ������·��</returns>
        public static string Combine(string strParent_, string strSubNode_, string strAttrName_, string strAttrValue_)
        {
            return string.Format("{0}/{1}[@{2}=\'{3}\']", strParent_, strSubNode_, strAttrName_, strAttrValue_);
        }
    };

    /// <summary>
    /// ���ڶ�ȡXML�����ļ����ࣺ�������ļ�����ȡ�ڵ������Լ���ȡ�ڵ����ԡ�
    /// &lt;XNodes&gt;
    ///     &lt;Form Name="FrmMain"&gt;
    ///         &lt;NodeText&gt;Contents&lt;/NodeText&gt;
    ///     .....
    ///     &lt;/Form&gt;
    /// &lt;/XNodes&gt;
    /// 
    /// �����ļ�Ҫ�������ϸ�ʽ��������ȡ�����ݴ�Form��ʼ��
    /// �����ڻ�ȡContents��·��Ϊ"/Form[@Name='FrmMain']/NodeText"��
    /// ·���÷�б��'/'�ָ����ǰ����Ҫ���һ��@����
    /// ���������·�����ڲ������������ʱҪ�ӵڶ���·����ʼ��
    /// </summary>
    public class XConReader
    {
        private XmlNode _Root = null;
        private const string _strName = "Name";

        /// <summary>
        /// ȱʡ���캯���������κβ���
        /// </summary>
        public XConReader() { }

        /// <summary>
        /// ���캯�������������ļ���.XML��������ʱ���׳��쳣
        /// </summary>
        /// <param name="strFile_">�����ļ�������������·����</param>
        public XConReader(string strFile_)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(strFile_);
            _Root = xmlDoc.DocumentElement;
        }

        /// <summary>
        /// �������ļ���.XML��
        /// </summary>
        /// <param name="strFile_">�����ļ�������������·����</param>
        /// <returns>�ɹ�������true�����򣬷���false</returns>
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
        /// �ͷ�ռ�õ��ڴ棬���ú�������������������Ч�������ٵ��ã���������Open��
        /// </summary>
        public void Close()
        {
            _Root = null;
        }

        /// <summary>
        /// ��ȡָ���ڵ������Ϊ��Name�������ݣ���Ҫ��֤�����ļ��Ѵ򿪣�����ʱ���׳��쳣
        /// </summary>
        /// <param name="strNode_">Ҫ��ȡ���ݵĽڵ�·������Ҫ����XPath����</param>
        /// <returns>����Ϊ��Name��������</returns>
        public string GetName(string strNode_)
        {
            return GetAttr(strNode_, _strName);
        }

        /// <summary>
        /// ��ȡָ���ڵ������Ϊ��Name�������ݣ���Ҫ��֤�����ļ��Ѵ�
        /// </summary>
        /// <param name="strNode_">Ҫ��ȡ���ݵĽڵ�·������Ҫ����XPath����</param>
        /// <param name="strDefault_">��ȡʧ��ʱ�����ص�����</param>
        /// <returns>����Ϊ��Name�������ݣ������ȡʧ�ܣ�����strDefault_</returns>                
        public string GetName(string strNode_, string strDefault_)
        {
            return GetAttr(strNode_, _strName, strDefault_);
        }

        /// <summary>
        /// ��ȡ���нڵ������Ϊ��Name�������ݣ���Ҫ��֤�����ļ��Ѵ�
        /// </summary>
        /// <param name="strNode_">Ҫ��ȡ���ݵĽڵ�·������Ҫ����XPath����</param>
        /// <returns>����Ϊ��Name��������</returns>
        public string[] GetAllName(string strNode_)
        {
            return GetAllAttr(strNode_, _strName);
        }

        /// <summary>
        /// ��ȡָ���ڵ��µ����ݣ���Ҫ��֤�����ļ��Ѵ򿪣�����ʱ���׳��쳣
        /// </summary>
        /// <param name="strNode_">Ҫ��ȡ���ݵĽڵ�·������Ҫ����XPath����</param>
        /// <returns>��ȡ��������</returns>
        public string GetText(string strNode_)
        {
            XmlNode nodElem = _Root.SelectSingleNode(strNode_);
            return (nodElem.InnerText);
        }

        /// <summary>
        /// ��ȡָ���ڵ��µ����ݣ���Ҫ��֤�����ļ��Ѵ�
        /// </summary>
        /// <param name="strNode_">Ҫ��ȡ���ݵĽڵ�·������Ҫ����XPath����</param>
        /// <param name="strDefault_">��ȡʧ��ʱ�����ص�����</param>
        /// <returns>��ȡ�������ݣ������ȡʧ�ܣ�����strDefault_</returns>
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
        /// ��ȡָ���ڵ����������ݣ���Ҫ��֤�����ļ��Ѵ򿪣�����ʱ���׳��쳣
        /// </summary>
        /// <param name="strNode_">Ҫ��ȡ���ݵĽڵ�·������Ҫ����XPath����</param>
        /// <returns>��ȡ��������</returns>
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
        /// ��ȡ�������Ե�ָ���ڵ��µ��������ݣ���Ҫ��֤�����ļ��Ѵ򿪣�����ʱ���׳��쳣
        /// </summary>
        /// <param name="strNode_">Ҫ��ȡ���ݵĽڵ�·������Ҫ����XPath����</param>
        /// <param name="strAttrName_">Ҫ�������Ե�����</param>
        /// <param name="strAttrValue_">Ҫ�������Ե�ֵ</param>
        /// <returns>��ȡ��������</returns>
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
        /// ��ȡָ���ڵ�����ԣ���Ҫ��֤�����ļ��Ѵ򿪣�����ʱ���׳��쳣
        /// </summary>
        /// <param name="strNode_">Ҫ��ȡ���ԵĽڵ�·������Ҫ����XPath����</param>
        /// <param name="strName_">��������</param>
        /// <returns>��ȡ������ֵ</returns>
        public string GetAttr(string strNode_, string strName_)
        {

            XmlNode nodAttr = _Root.SelectSingleNode(strNode_).Attributes.GetNamedItem(strName_);
            return (nodAttr.Value);
        }

        /// <summary>
        /// ��ȡָ���ڵ�����ԣ���Ҫ��֤�����ļ��Ѵ�
        /// </summary>
        /// <param name="strNode_">Ҫ��ȡ���ԵĽڵ�·������Ҫ����XPath����</param>
        /// <param name="strName_">��������</param>
        /// <param name="strDefault_">��ȡʧ��ʱ�����ص�����</param>
        /// <returns>��ȡ������ֵ�������ȡʧ�ܣ��򷵻�strDefault_</returns>
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
        /// ��ȡBool���͵�����
        /// </summary>
        /// <param name="strNode_">Ҫ��ȡ���ԵĽڵ�·������Ҫ����XPath����</param>
        /// <param name="strName_">��������</param>
        /// <returns>ֵΪ��������������true�����򣬷���false</returns>
        public bool GetBoolAttr(string strNode_, string strName_)
        {
            string strAttr = GetAttr(strNode_, strName_, "0");
            uint nAttr = 0;
            uint.TryParse(strAttr, out nAttr);

            return (nAttr != 0);
        }

        /// <summary>
        /// ��ȡuint����ʮ�����Ʊ�ʾ�����͵�����
        /// </summary>
        /// <param name="strNode_">Ҫ��ȡ���ԵĽڵ�·������Ҫ����XPath����</param>
        /// <param name="strName_">��������</param>
        /// <returns>ֵΪ��������������true�����򣬷���false</returns>
        public uint GetHexAttr(string strNode_, string strName_)
        {
            string strHex = GetAttr(strNode_, strName_, "0");

            if (strHex.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                strHex = strHex.Substring(2);

            return uint.Parse(strHex, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// ��ȡָ���ڵ������ָ�����ԣ���Ҫ��֤�����ļ��Ѵ򿪣�����ʱ���׳��쳣
        /// </summary>
        /// <param name="strNode_">Ҫ��ȡ���ԵĽڵ�·������Ҫ����XPath����</param>
        /// <param name="strName_">��������</param>
        /// <returns>��ȡ������ֵ</returns>
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
    /// ������������xml�ļ�����
    /// ���ɵ�ʵ��Ҫ�����ʱҪ�رգ�������using�д���ʵ����
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
        /// ȱʡ���캯��
        /// </summary>
        public XConWriter() { }

        /// <summary>
        /// �����Ѵ��ļ���ʵ����������׳��쳣
        /// </summary>
        /// <param name="strFile_">Ҫ������xml�ļ�����ȫ·����</param>
        public XConWriter(string strFile_)
        {
            XmlWriterSettings xmlSetting = new XmlWriterSettings();
            xmlSetting.Indent = true;
            _xmlFile = XmlWriter.Create(strFile_, xmlSetting);
            _xmlFile.WriteStartDocument();
            _xmlFile.WriteStartElement(c_strTopElement);
        }

        /// <summary>
        /// ��������
        /// </summary>
        ~XConWriter()
        {
            Dispose(false);
        }

        /// <summary>
        /// ����xml�ļ�
        /// </summary>
        /// <param name="strFile_">Ҫ������xml�ļ�����ȫ·����</param>
        /// <returns>�ɹ�����ture������false</returns>
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
        /// �ڵ�ǰ�ڵ������'Name'���ԣ�����ʱ�׳��쳣
        /// </summary>
        /// <param name="strValue_">����ֵ</param>
        public void WriteName(string strValue_)
        {
            WriteAttr(_strName, strValue_);
        }

        /// <summary>
        /// �ڵ�ǰ�ڵ��´�������'Name'���Ե��½ڵ㣬����ʱ�׳��쳣
        /// </summary>
        /// <param name="strNode_">�½ڵ�����</param>
        /// <param name="strValue_">����ֵ</param>
        public void WriteName(string strNode_, string strValue_)
        {
            WriteAttr(strNode_, _strName, strValue_);
        }

        /// <summary>
        /// �ڵ�ǰ�ڵ���������ԣ�����ʱ�׳��쳣
        /// </summary>
        /// <param name="strAttr_">��������</param>
        /// <param name="strValue_">����ֵ</param>
        public void WriteAttr(string strAttr_, string strValue_)
        {
            _xmlFile.WriteStartAttribute(strAttr_);
            _xmlFile.WriteString(strValue_);
            _xmlFile.WriteEndAttribute();
        }

        /// <summary>
        /// �ڵ�ǰ�ڵ���������ԣ�����ʱ�׳��쳣
        /// </summary>
        /// <param name="strAttr_">��������</param>
        /// <param name="bValue_">����ֵ</param>
        public void WriteAttr(string strAttr_, bool bValue_)
        {
            _xmlFile.WriteStartAttribute(strAttr_);
            _xmlFile.WriteString(bValue_ ? "1" : "0");
            _xmlFile.WriteEndAttribute();
        }

        /// <summary>
        /// �ڵ�ǰ�ڵ������ʮ������ֵ�����ԣ�����ʱ�׳��쳣
        /// </summary>
        /// <param name="strAttr_">��������</param>
        /// <param name="nValue_">����ֵ</param>
        public void WriteAttr(string strAttr_, uint nValue_)
        {
            _xmlFile.WriteStartAttribute(strAttr_);
            _xmlFile.WriteString("0x" + nValue_.ToString("X"));
            _xmlFile.WriteEndAttribute();
        }

        /// <summary>
        /// �ڵ�ǰ�ڵ��´�����������ֵ���½ڵ㣬����ʱ�׳��쳣
        /// </summary>
        /// <param name="strNode_">�½ڵ�����</param>
        /// <param name="strAttr_">��������</param>
        /// <param name="strValue_">����ֵ</param>
        public void WriteAttr(string strNode_, string strAttr_, string strValue_)
        {
            _xmlFile.WriteStartElement(strNode_);
            WriteAttr(strAttr_, strValue_);
            _xmlFile.WriteEndElement();
        }

        /// <summary>
        /// �ڵ�ǰ�ڵ��´�����������ֵ���½ڵ㣬����ʱ�׳��쳣
        /// </summary>
        /// <param name="strNode_">�½ڵ�����</param>
        /// <param name="strAttr_">��������</param>
        /// <param name="bValue_">����ֵ</param>
        public void WriteAttr(string strNode_, string strAttr_, bool bValue_)
        {
            _xmlFile.WriteStartElement(strNode_);
            WriteAttr(strAttr_, bValue_);
            _xmlFile.WriteEndElement();
        }

        /// <summary>
        /// �ڵ�ǰ�ڵ��´�������ʮ������ֵ����ֵ���½ڵ㣬����ʱ�׳��쳣
        /// </summary>
        /// <param name="strNode_">�½ڵ�����</param>
        /// <param name="strAttr_">��������</param>
        /// <param name="nValue_">����ֵ</param>
        public void WriteAttr(string strNode_, string strAttr_, uint nValue_)
        {
            _xmlFile.WriteStartElement(strNode_);
            WriteAttr(strAttr_, nValue_);
            _xmlFile.WriteEndElement();
        }

        /// <summary>
        /// �ڵ�ǰ�ڵ�������ı�������ʱ�׳��쳣
        /// </summary>
        /// <param name="strText_">�ı�����</param>
        public void WriteText(string strText_)
        {
            _xmlFile.WriteString(strText_);
        }

        /// <summary>
        /// �ڵ�ǰ�ڵ��´��������ı����µĽڵ㣬����ʱ�׳��쳣
        /// </summary>
        /// <param name="strNode_">�½ڵ�����</param>
        /// <param name="strText_">�ı�����</param>
        public void WriteText(string strNode_, string strText_)
        {
            _xmlFile.WriteStartElement(strNode_);
            _xmlFile.WriteString(strText_);
            _xmlFile.WriteEndElement();
        }

        /// <summary>
        /// �ڵ�ǰ�ڵ��´����������Ժ��ı��ı����µĽڵ㣬����ʱ�׳��쳣
        /// </summary>
        /// <param name="strNode_">�½ڵ�����</param>
        /// <param name="strAttr_">��������</param>
        /// <param name="strValue_">����ֵ</param>
        /// <param name="strText_">�ı�����</param>
        public void WriteBoth(string strNode_, string strAttr_, string strValue_, string strText_)
        {
            _xmlFile.WriteStartElement(strNode_);
            WriteAttr(strAttr_, strValue_);
            _xmlFile.WriteString(strText_);
            _xmlFile.WriteEndElement();
        }

        /// <summary>
        /// �ڵ�ǰ�ڵ��´����������Ժ��ı��ı����µĽڵ㣬����ʱ�׳��쳣
        /// </summary>
        /// <param name="strNode_">�½ڵ�����</param>
        /// <param name="strAttr_">��������</param>
        /// <param name="bValue_">����ֵ</param>
        /// <param name="strText_">�ı�����</param>
        public void WriteBoth(string strNode_, string strAttr_, bool bValue_, string strText_)
        {
            _xmlFile.WriteStartElement(strNode_);
            WriteAttr(strAttr_, bValue_);
            _xmlFile.WriteString(strText_);
            _xmlFile.WriteEndElement();
        }

        /// <summary>
        /// �ڵ�ǰ�ڵ��´�������ʮ������ֵ���Ժ��ı��ı����µĽڵ㣬����ʱ�׳��쳣
        /// </summary>
        /// <param name="strNode_">�½ڵ�����</param>
        /// <param name="strAttr_">��������</param>
        /// <param name="nValue_">����ֵ</param>
        /// <param name="strText_">�ı�����</param>
        public void WriteBoth(string strNode_, string strAttr_, uint nValue_, string strText_)
        {
            _xmlFile.WriteStartElement(strNode_);
            WriteAttr(strAttr_, nValue_);
            _xmlFile.WriteString(strText_);
            _xmlFile.WriteEndElement();
        }

        /// <summary>
        /// �ڵ�ǰ�ڵ��´����µĽڵ㣬����ʱ�׳��쳣
        /// ��������������������ı��Լ��µĽڵ㣻��ɺ���Ҫʹ��EndNode�ر�
        /// </summary>
        /// <param name="strNode_">�½ڵ�����</param>
        public void StartNode(string strNode_)
        {
            _xmlFile.WriteStartElement(strNode_);
        }

        /// <summary>
        /// �ڵ�ǰ�ڵ��´����µĽڵ㲢�趨���ԣ�����ʱ�׳��쳣
        /// ��������������������ı��Լ��µĽڵ㣻��ɺ���Ҫʹ��EndNode�ر�
        /// </summary>
        /// <param name="strNode_">�½ڵ�����</param>
        /// <param name="strAttr_">��������</param>
        /// <param name="strValue_">����ֵ</param>
        public void StartNode(string strNode_, string strAttr_, string strValue_)
        {
            StartNode(strNode_);
            WriteAttr(strAttr_, strValue_);
        }

        /// <summary>
        /// �رյ�ǰ�ڵ㣬�����ص���һ���ڵ�
        /// </summary>
        public void EndNode()
        {
            _xmlFile.WriteEndElement();
        }

        /// <summary>
        /// �ر��Ѵ򿪵��ļ�
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
        /// ������������ļ��Ѵ򿪣���ر�֮
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
