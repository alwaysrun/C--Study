using System.Collections.Generic;

namespace SHCre.Xugd.XControl
{
    /// <summary>
    /// ����ļ�������ʷ��¼�ķ�����
    /// </summary>
    public class XBackForward
    {
        LinkedList<string> _lstAllPaths;
        /// <summary>
        /// ��ǰ��ʾ��·�����ڽڵ�
        /// </summary>
        LinkedListNode<string> _lnodCur;

        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public XBackForward()
        {
            _lstAllPaths = new LinkedList<string>();
            _lnodCur = null;
        }

        /// <summary>
        /// �ж��ܷ����
        /// </summary>
        /// <returns>�ܺ��˷���true�����򷵻�false</returns>
        public bool CanBack()
        {
            return ((_lstAllPaths.Count > 1) && (_lnodCur != _lstAllPaths.First));
        }

        /// <summary>
        /// �ж��ܷ�ǰ��
        /// </summary>
        /// <returns>��ǰ������true�����򷵻�false</returns>
        public bool CanForward()
        {
            return ((_lstAllPaths.Count > 1) && (_lnodCur != _lstAllPaths.Last));
        }

        /// <summary>
        /// �趨��ǰ·��
        /// </summary>
        /// <param name="strPath_">��ǰ·���ı�ʶ��Ϣ����ȫ·���ȣ�</param>
        public void SetCurrent(string strPath_)
        {
            while(_lnodCur != _lstAllPaths.Last )
                _lstAllPaths.RemoveLast();

            if ((null != _lnodCur) && (strPath_ == _lnodCur.Value))
                return;

            _lnodCur = _lstAllPaths.AddLast(strPath_);
        }

        /// <summary>
        /// ��ȡ��ǰ·��
        /// </summary>
        /// <returns>��ǰ·����ʶ</returns>
        public string GetCurrent()
        {
            if (null == _lnodCur)
                return default(string);

            return _lnodCur.Value;
        }

        /// <summary>
        /// ɾ��һ������·����һ�������ں���ʱ������·���Ѳ�������ʱ�ã�
        /// </summary>
        public void RemoveBack()
        {
            if( (null != _lnodCur) && (null != _lnodCur.Previous) )
                _lstAllPaths.Remove(_lnodCur.Previous);
        }

        /// <summary>
        /// ɾ��һ��ǰ��·����һ��������ǰ��ʱ������·���Ѳ�������ʱ�ã�
        /// </summary>
        public void RemoveForward()
        {
            if ((null != _lnodCur) && (null != _lnodCur.Next))
                _lstAllPaths.Remove(_lnodCur.Next);
        }

        /// <summary>
        /// ����
        /// </summary>
        public void GoBack()
        {
            if (_lnodCur != _lstAllPaths.First)
                _lnodCur = _lnodCur.Previous;
        }

        /// <summary>
        /// ǰ��
        /// </summary>
        public void GoForward()
        {
            if (_lnodCur != _lstAllPaths.Last)
                _lnodCur = _lnodCur.Next;
        }

        /// <summary>
        /// ��ȡ���˵�·������ʱ��δ�ı䵱ǰ·������ɺ�ʹ��GoBack����������ˣ�
        /// </summary>
        /// <returns>���˵�·��</returns>
        public string GetBackPath()
        {
            if ((null == _lnodCur) || (null == _lnodCur.Previous))
                return default(string);

            return _lnodCur.Previous.Value;
        }

        /// <summary>
        /// ��ȡǰ����·������ʱ��δ�ı䵱ǰ·������ɺ�ʹ��GoForward�������ǰ����
        /// </summary>
        /// <returns>ǰ����·��</returns>
        public string GetForwardPath()
        {
            if ((null == _lnodCur) || (null == _lnodCur.Next))
                return default(string);

            return _lnodCur.Next.Value;
        }
    }
}
