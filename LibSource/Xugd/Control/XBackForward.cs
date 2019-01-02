using System.Collections.Generic;

namespace SHCre.Xugd.XControl
{
    /// <summary>
    /// 存放文件访问历史记录的泛型类
    /// </summary>
    public class XBackForward
    {
        LinkedList<string> _lstAllPaths;
        /// <summary>
        /// 当前显示的路径所在节点
        /// </summary>
        LinkedListNode<string> _lnodCur;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public XBackForward()
        {
            _lstAllPaths = new LinkedList<string>();
            _lnodCur = null;
        }

        /// <summary>
        /// 判断能否后退
        /// </summary>
        /// <returns>能后退返回true，否则返回false</returns>
        public bool CanBack()
        {
            return ((_lstAllPaths.Count > 1) && (_lnodCur != _lstAllPaths.First));
        }

        /// <summary>
        /// 判断能否前进
        /// </summary>
        /// <returns>能前进返回true，否则返回false</returns>
        public bool CanForward()
        {
            return ((_lstAllPaths.Count > 1) && (_lnodCur != _lstAllPaths.Last));
        }

        /// <summary>
        /// 设定当前路径
        /// </summary>
        /// <param name="strPath_">当前路径的标识信息（如全路径等）</param>
        public void SetCurrent(string strPath_)
        {
            while(_lnodCur != _lstAllPaths.Last )
                _lstAllPaths.RemoveLast();

            if ((null != _lnodCur) && (strPath_ == _lnodCur.Value))
                return;

            _lnodCur = _lstAllPaths.AddLast(strPath_);
        }

        /// <summary>
        /// 获取当前路径
        /// </summary>
        /// <returns>当前路径标识</returns>
        public string GetCurrent()
        {
            if (null == _lnodCur)
                return default(string);

            return _lnodCur.Value;
        }

        /// <summary>
        /// 删除一个后退路径（一般用于在后退时，发现路径已不存在了时用）
        /// </summary>
        public void RemoveBack()
        {
            if( (null != _lnodCur) && (null != _lnodCur.Previous) )
                _lstAllPaths.Remove(_lnodCur.Previous);
        }

        /// <summary>
        /// 删除一个前进路径（一般用于在前进时，发现路径已不存在了时用）
        /// </summary>
        public void RemoveForward()
        {
            if ((null != _lnodCur) && (null != _lnodCur.Next))
                _lstAllPaths.Remove(_lnodCur.Next);
        }

        /// <summary>
        /// 后退
        /// </summary>
        public void GoBack()
        {
            if (_lnodCur != _lstAllPaths.First)
                _lnodCur = _lnodCur.Previous;
        }

        /// <summary>
        /// 前进
        /// </summary>
        public void GoForward()
        {
            if (_lnodCur != _lstAllPaths.Last)
                _lnodCur = _lnodCur.Next;
        }

        /// <summary>
        /// 获取后退的路径（此时还未改变当前路径，完成后使用GoBack完成真正后退）
        /// </summary>
        /// <returns>后退的路径</returns>
        public string GetBackPath()
        {
            if ((null == _lnodCur) || (null == _lnodCur.Previous))
                return default(string);

            return _lnodCur.Previous.Value;
        }

        /// <summary>
        /// 获取前进的路径（此时还未改变当前路径，完成后使用GoForward完成真正前进）
        /// </summary>
        /// <returns>前进的路径</returns>
        public string GetForwardPath()
        {
            if ((null == _lnodCur) || (null == _lnodCur.Next))
                return default(string);

            return _lnodCur.Next.Value;
        }
    }
}
