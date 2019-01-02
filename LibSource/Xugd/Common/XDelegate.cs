using System;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// 操作完成时信息基类
    /// </summary>
    public class XAsyncResult
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exError_"></param>
        protected XAsyncResult(Exception exError_)
        {
            Result = exError_;
        }

        static XAsyncResult s_Ok = new XAsyncResult(null);
        /// <summary>
        /// 返回成功结果
        /// </summary>
        public static XAsyncResult OK 
        {
            get { return s_Ok; }
        }

        /// <summary>
        /// 获取一个结果
        /// </summary>
        /// <param name="exError_"></param>
        /// <returns></returns>
        public static XAsyncResult Get(Exception exError_ = null)
        {
            if (exError_ == null)
                return s_Ok;

            return new XAsyncResult(exError_);
        }

        /// <summary>
        /// 错误信息：成功则为null
        /// </summary>
        public Exception Result { get; private set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess
        {
            get { return Result == null; }
        }

        /// <summary>
        /// 触发Act
        /// </summary>
        /// <param name="act_"></param>
        /// <param name="ex_"></param>
        public static void InvokeAct(Action<XAsyncResult> act_, Exception ex_)
        {
            if (act_ != null)
                act_(new XAsyncResult(ex_));
        }

        /// <summary>
        /// 触发Act
        /// </summary>
        /// <param name="act_"></param>
        public void InvokeAct(Action<XAsyncResult> act_)
        {
            if (act_ != null)
                act_(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Result == null)
                return "Success";

            return string.Format("{0}({1})", XReflex.GetTypeName(Result, false), Result.Message);
        }
    }
    /// <summary>
    /// 没有参数与返回值的委托
    /// </summary>
    public delegate void DelNoneParamAct();

    /// <summary>
    /// 一个参数无返回值的委托
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="arg"></param>
    public delegate void DelOneParamAct<T>(T arg);

    /// <summary>
    /// 两个参数无返回值的委托
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    public delegate void DelTwoParamAct<T1, T2>(T1 arg1, T2 arg2);

    /// <summary>
    /// 无参数有返回值的委托
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public delegate TResult DelNoneParamFunc<TResult>();

    /// <summary>
    /// 一个参数有返回值的委托
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="arg1"></param>
    /// <returns></returns>
    public delegate TResult DelOneParamFunc<T1, TResult>(T1 arg1);

    /// <summary>
    /// 两个参数有返回值的委托
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <returns></returns>
    public delegate TResult DelTwoParamFunc<T1, T2, TResult>(T1 arg1, T2 arg2);
}
