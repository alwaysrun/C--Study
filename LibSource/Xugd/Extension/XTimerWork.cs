using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.CFile;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.Extension
{

    /// <summary>
    /// 工作结果：如果不需要定时器再次调用执行，返回Complete；
    /// 否则（返回其他值或抛出异常）定时器会再次执行调用
    /// </summary>
    public enum XTimerWorkResult
    {
        /// <summary>
        /// 出错
        /// </summary>
        Error = 0,
        /// <summary>
        /// 需要再次调用（定时调用）
        /// </summary>
        Restart,
        /// <summary>
        /// 完成（不需要再次调用）
        /// </summary>
        Complete,
    }

    /// <summary>
    /// 定时工作类：如果工作进程Func返回XTimerWorkResult.Complete则执行完成，不再重启定时器；否则（返回其他或抛出异常），则会再次执行（重启定时器）。
    /// 必须先通过SetWorkInfo设定工作信息，并通过Start来启用才开始工作。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class XTimerWork<T> : XLogEventsBase where T : class
    {
        T _tData;
        Func<T, XTimerWorkResult> _funWork;
        System.Timers.Timer _tmWork = new System.Timers.Timer();
        bool _bStart = true;
        bool _bWorking = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nInterSecond_">定时器重启时间间隔（秒数）</param>
        public XTimerWork(int nInterSecond_)
        {
            _tmWork.AutoReset = false;
            _tmWork.Elapsed += new System.Timers.ElapsedEventHandler(tmWork_Elapsed);
            ResetInterval(nInterSecond_);

            BuildLogPrefix<T>("XTimerWork");
        }

        /// <summary>
        /// 重设定时器时间间隔（如果定时器已启用，在设定的时间间隔后触发）
        /// </summary>
        /// <param name="nInterSecond_"></param>
        public void ResetInterval(int nInterSecond_)
        {
            if (nInterSecond_ < 1)
                nInterSecond_ = 5;
            _tmWork.Interval = XTime.Second2Interval(nInterSecond_);
        }

        void tmWork_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_bStart || (_funWork == null)) return;

            XTimerWorkResult euResult = XTimerWorkResult.Error;
            try
            {
                _bWorking = true;
                InvokeOnDebug("Timer-begin");

                euResult = _funWork(_tData);
            }
            catch(Exception ex)
            {
                InvokeOnExcept(ex);
            }
            finally
            {
                _bWorking = false;
            }

            InvokeOnDebug("Timer-end(WorkResult:{0})", euResult);
            if ((euResult != XTimerWorkResult.Complete) && _bStart)
                _tmWork.Start();
        }

        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="funToWork_">回调函数：如果工作进程Func如果返回false或抛出异常，则循环（重启定时器）；否则执行完成，不再重启定时器</param>
        /// <param name="tData_"></param>
        public void SetWorkInfo(Func<T, XTimerWorkResult> funToWork_, T tData_)
        {
            InvokeOnLogger("#SetWorkInfo");
            if (funToWork_ == null)
                throw new ArgumentNullException("funToWork can not NULL");

            _funWork = funToWork_;
            _tData = tData_;
        }

        /// <summary>
        /// 判断是否已调用了SetWorkInfo正确的设定了。
        /// </summary>
        /// <returns></returns>
        public bool IsWorkInfoSet()
        {
            return _funWork != null;
        }

        /// <summary>
        /// 重设定时器：如果定时器已启动，则重新开始计时；否则，启动定时器。
        /// </summary>
        public void ResetAndStart(){
            if (_funWork == null)
                throw new ArgumentNullException("Set work-routine by SetWorkInfo first");

            if(_tmWork.Enabled){
                _tmWork.Interval = _tmWork.Interval;
            }
            else{
                _tmWork.Start();
            }
        }

        /// <summary>
        /// 重启
        /// </summary>
        /// <param name="bExcuteWorkImmediate_">是否先立即执行作业：
        /// true先执行作业，并等待作业完成后返回；否则，只启用定时器</param>
        public void Start(bool bExcuteWorkImmediate_)
        {
            InvokeOnLogger("#Start");
            if (_funWork == null)
                throw new ArgumentNullException("Set work-routine by SetWorkInfo first");

            _bStart = true;
            if (bExcuteWorkImmediate_ && !_bWorking)
            {
                _tmWork.Stop();

                XThread.StartPool((zWork) => tmWork_Elapsed(zWork, null), _tmWork);
            }
            else
            {
                if (!_tmWork.Enabled)
                    _tmWork.Start();
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            InvokeOnLogger("#Stop");
            _bStart = false;
            _tmWork.Stop();
        }
    } // class
}
