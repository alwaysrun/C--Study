using System;
using System.Collections.Generic;
using System.Text;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// 时间、日期相关处理接口
    /// </summary>
    public static class XTime
    {
        const int MultiOfSecond2MS = 1000;
        const int MultiOfMinute2Seconds = 60;
        /// <summary>
        /// 时间间隔的毫秒数：用于Timeout以及Sleep等
        /// </summary>
        /// <param name="nSecond_"></param>
        /// <returns></returns>
        public static int Second2Interval(int nSecond_)
        {
            return nSecond_ * MultiOfSecond2MS;
        }

        /// <summary>
        /// 时间间隔（毫秒）转换为秒
        /// </summary>
        /// <param name="nMs_"></param>
        /// <returns></returns>
        public static int Interval2Second(int nMs_)
        {
            return nMs_ / MultiOfSecond2MS;
        }

        /// <summary>
        /// 时间间隔的毫秒数：用于Timeout以及Sleep等
        /// </summary>
        /// <param name="nMinute_"></param>
        /// <returns></returns>
        public static int Minute2Interval(int nMinute_)
        {
            return Second2Interval(nMinute_ * MultiOfMinute2Seconds);
        }

        /// <summary>
        /// 时间间隔（毫秒）转换为分钟
        /// </summary>
        /// <param name="nMs_"></param>
        /// <returns></returns>
        public static int Interval2Minute(int nMs_)
        {
            return Interval2Second(nMs_) / MultiOfMinute2Seconds;
        }

        /// <summary>
        /// 把时间转换为毫秒
        /// </summary>
        /// <param name="nHour_"></param>
        /// <param name="nMinute_"></param>
        /// <param name="nSecond_"></param>
        /// <returns></returns>
        public static int ToMilliSecond(int nHour_, int nMinute_, int nSecond_)
        {
            return ((nHour_ * 60 + nMinute_) * 60 + nSecond_) * MultiOfSecond2MS;
        }

        /// <summary>
        /// 获取TimeSpan的字符串表示（x天 x小时 x分钟 x秒）
        /// </summary>
        /// <param name="tsTime_"></param>
        /// <returns></returns>
        public static string GetTimeSpanShowString(TimeSpan tsTime_)
        {
            string strTime = string.Empty;
            if (tsTime_.Days > 0)
                strTime += string.Format("{0}天 ", tsTime_.Days);
            if (tsTime_.Hours > 0)
                strTime += string.Format("{0}小时 ", tsTime_.Hours);
            if (tsTime_.Minutes > 0)
                strTime += string.Format("{0}分钟 ", tsTime_.Minutes);

            strTime += string.Format("{0}秒", tsTime_.Seconds);

            return strTime;
        }

        /// <summary>
        /// 获取时间完整字符串表示（"yyyy-MM-dd HH:mm:ss")或者（"yyyy-MM-dd HH:mm:ss.fff")
        /// </summary>
        /// <param name="dtTime_">时间</param>
        /// <param name="bWithMS_">是否需要毫秒</param>
        /// <returns>完整字符串表示</returns>
        public static string GetFullString(DateTime dtTime_, bool bWithMS_ = false)
        {
            if(bWithMS_)
                return dtTime_.ToString("yyyy-MM-dd HH:mm:ss.fff");
            else
                return dtTime_.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 获取日期字符串（完整"yyyy-MM-dd HH:mm:ss.fff")
        /// </summary>
        /// <param name="dtTime_">时间</param>
        /// <param name="strFormat_">时间格式</param>
        /// <returns>日期字符串</returns>
        public static string GetDateString(DateTime dtTime_, string strFormat_="yyyy-MM-dd")
        {
            return dtTime_.ToString(strFormat_);
        }

        /// <summary>
        /// 获取时间字符串（"HH:mm:ss")或者（"HH:mm:ss.fff"带有毫秒时）
        /// </summary>
        /// <param name="dtTime_">时间</param>
        /// <param name="bWithMS_">是否需要毫秒</param>
        /// <returns>时间字符串</returns>
        public static string GetTimeString(DateTime dtTime_, bool bWithMS_=false)
        {
            if( bWithMS_ )
                return dtTime_.ToString("HH:mm:ss.fff");
            else
                return dtTime_.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 从时间字符串中获取时间
        /// </summary>
        /// <param name="strTime_">时间字符串</param>
        /// <returns>时间</returns>
        public static DateTime GetDateTime(string strTime_)
        {
            return DateTime.Parse(strTime_);
        }

        /// <summary>
        /// 比较两字符串时间的大小
        /// </summary>
        /// <param name="strFirst_">时间1</param>
        /// <param name="strSecond_">时间2</param>
        /// <returns>两时间相等，返回0；时间1大于时间2，返回1；否则，返回-1</returns>
        public static int CompareTimeString(string strFirst_, string strSecond_)
        {
            DateTime dtFirst = DateTime.Parse(strFirst_);
            DateTime dtSeconde = DateTime.Parse(strSecond_);

            if(dtFirst == dtSeconde)
                return 0;

            if (dtFirst > dtSeconde)
                return 1;
            else
                return -1;
        }

        /// <summary>
        /// 获取指定日期开始时间（零时）
        /// </summary>
        /// <param name="dtTime_">时间</param>
        /// <returns>新的时间</returns>
        public static DateTime GetDayStart(DateTime dtTime_)
        {
            return new DateTime(dtTime_.Year, dtTime_.Month, dtTime_.Day);
        }

        /// <summary>
        /// 获取指定日期结束时间
        /// </summary>
        /// <param name="dtTime_">时间</param>
        /// <returns>新的时间</returns>
        public static DateTime GetDayEnd(DateTime dtTime_)
        {
            return (new DateTime(dtTime_.Year, dtTime_.Month, dtTime_.Day+1)).AddMilliseconds(-1);
        }
    }
}
