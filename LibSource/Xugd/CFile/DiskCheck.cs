using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.CFile
{
    /// <summary>
    /// 磁盘相关错误
    /// </summary>
    public static class XDisk
    {
        /// <summary>
        /// 判断分区是否可用（有可用空间）；
        /// 如果磁盘不存在或不可用返回false
        /// </summary>
        /// <param name="strDriveLetter_">如果是单字符则为（a~z,A~Z）；如果多个字符，则必须是绝对目录（如：c:\dir)</param>
        /// <returns></returns>
        public static bool IsAvailable(string strDriveLetter_)
        {
            try
            {
                var dInfo = new DriveInfo(strDriveLetter_);
                if (!dInfo.IsReady)
                    return false;

                return dInfo.AvailableFreeSpace > 0;
            }
            catch(Exception){}

            return false;
        }

        /// <summary>
        /// 获取以MB计算的磁盘大小；
        /// 如果磁盘不可用或不存在则返回0
        /// </summary>
        /// <param name="strDriveLetter_"></param>
        /// <returns></returns>
        public static long GetFreeSizeInMb(string strDriveLetter_)
        {
            try
            {
                var dInfo = new DriveInfo(strDriveLetter_);
                if(dInfo.IsReady)
                {
                    return XSize.Bytes2Mb(dInfo.AvailableFreeSpace);
                }
            }
            catch (Exception) { }

            return 0;
        }

        /// <summary>
        /// 返回磁盘字节数；
        /// 如果磁盘不可用或不存在则返回0
        /// </summary>
        /// <param name="strDriveLetter_"></param>
        /// <returns></returns>
        public static long GetFreeSize(string strDriveLetter_)
        {
            try
            {
                var dInfo = new DriveInfo(strDriveLetter_);
                if (dInfo.IsReady)
                {
                    return dInfo.AvailableFreeSpace;
                }
            }
            catch (Exception) { }

            return 0;
        }

        /// <summary>
        /// 获取空闲空间最大的数据盘符
        /// </summary>
        /// <param name="nMinSpaceInMB">要求盘符的最小空间：如果没有大于此空间的数据盘符，则返回系统盘</param>
        /// <returns></returns>
        public static char GetMaxSpaceDrive(int nMinSpaceInMB=500)
        {
            long nMaxSpace = nMinSpaceInMB * 1024 * 1024;
            DriveInfo drvMax = null;
            char chSystem = char.ToUpper(Environment.SystemDirectory[0]);
            var allDrive = DriveInfo.GetDrives();
            foreach (var dr in allDrive)
            {
                if (dr.IsReady && (dr.DriveType == DriveType.Fixed) && (dr.AvailableFreeSpace > nMaxSpace))
                {
                    if (char.ToUpper(dr.RootDirectory.FullName[0]) == chSystem)
                        continue;

                    nMaxSpace = dr.AvailableFreeSpace;
                    drvMax = dr;
                }
            }

            if (drvMax != null)
                return drvMax.RootDirectory.FullName[0];

            return chSystem;
        }
    } // class
}
