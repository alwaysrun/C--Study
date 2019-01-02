using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using SHCre.Xugd.WinAPI;

namespace SHCre.Xugd.CFile
{
    /// <summary>
    /// 小图标列表类：包含小图标与对应文件的类型说明：
    /// .exe(可执行文件)、.ico(图标文件)、.lnk(快捷方式)、.msc(微软通用管理文档)、.scr(屏保程序)等
    /// 每个程序的图标都不同，所以需要根据文件本身来获取，其他类型直接根据扩展名获取
    /// 
    /// 通过GetList获取列表（可以直接赋值给ListVIew的SmallImageList），
    /// 然后通过GetFileIndex与GetDirIndex来获取图标位置（直接赋值给ListViewItem的ImageIndex即可）与类型信息
    /// </summary>
    public class XFileImgs
    {
        const string _strDirName = "Dir";
        const string _strUnknowName = "Unk";
        readonly string[] _strExtChecks = { ".EXE", ".ICO", ".LNK", ".MSC", ".SCR" };
        int _nUnkIndex = -1;
        int _nDirIndex = -1;
        ImageList _lstImages = null;
        List<string> _lstInfo = null;

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        public XFileImgs()
        {
            _lstImages = new ImageList();
            _lstImages.ColorDepth = ColorDepth.Depth32Bit;
            _lstInfo = new List<string>();

            Reset();
        }

        /// <summary>
        /// 移除所有图标信息，重新添加。
        /// 一般在重新枚举时（进入新的文件夹，特别是文件变化比较大时）需要调用此接口。
        /// </summary>
        public void Reset()
        {
            // Remove all
            _nUnkIndex = -1;
            _nDirIndex = -1;
            _lstImages.Images.Clear();
            _lstInfo.Clear();

            // And the unknown file 
            _nUnkIndex = AddExtInfo(_strUnknowName, _strUnknowName);
            try
            {
                string strInfo;
                Icon hIcon;

                hIcon = XShell.GetDirInfo(out strInfo);
                _nDirIndex = _lstImages.Images.Count;
                _lstImages.Images.Add(_strDirName, hIcon);
                _lstInfo.Add(strInfo);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 清空所有图标信息
        /// </summary>
        public void Clear()
        {
            _lstImages.Images.Clear();
            _lstInfo.Clear();
        }

        /// <summary>
        /// 获取图标列表
        /// </summary>
        /// <returns>小图标列表</returns>
        public ImageList GetList()
        {
            return _lstImages;
        }

        int AddFileInfo(string strName_, string strFile_)
        {
            int nIndex = _lstImages.Images.IndexOfKey(strName_);
            if (-1 == nIndex)
            {
                try
                {
                    string strInfo;
                    Icon hIcon = XShell.GetFileInfo(strFile_, out strInfo);

                    nIndex = _lstImages.Images.Count;
                    _lstImages.Images.Add(strName_, hIcon);
                    _lstInfo.Add(strInfo);
                }
                catch { }
            }

            return nIndex;
        }

        int AddExtInfo(string strExt_, string strFile_)
        {
            int nIndex = _lstImages.Images.IndexOfKey(strExt_);
            if (-1 == nIndex)
            {
                try
                {
                    bool bGet = false;
                    Icon hIcon = null;
                    string strInfo = null;
                    if (File.Exists(strFile_))
                    {
                        try
                        {
                            hIcon = XShell.GetFileInfo(strFile_, out strInfo);
                            bGet = true;
                        }
                        catch { }
                    }
                    if (!bGet)
                        hIcon = XShell.GetFileInfoByExt(strFile_, out strInfo);

                    nIndex = _lstImages.Images.Count;
                    _lstImages.Images.Add(strExt_, hIcon);
                    _lstInfo.Add(strInfo);
                }
                catch
                {
                    nIndex = _nUnkIndex;
                }
            }

            return nIndex;
        }

        /// <summary>
        /// 获取文件对应图标
        /// </summary>
        /// <param name="strFile_">文件名</param>
        /// <returns>图标在列表中的位置</returns>
        public int GetFileIndex(string strFile_)
        {
            string strName = Path.GetFileName(strFile_).ToUpper();
            string strExt = Path.GetExtension(strName);
            if (string.IsNullOrEmpty(strExt))
                return _nUnkIndex;

            //////////////////////////////////////////////////////////////////////////
            // Get File Icon
            int nIndex = -1;

            if (-1 != Array.IndexOf<string>(_strExtChecks, strExt))
                nIndex = AddFileInfo(strName, strFile_);
            if (-1 == nIndex)
            {
                nIndex = AddExtInfo(strExt, strFile_);
            }

            return nIndex;
        }

        /// <summary>
        /// 获取文件对应图标的位置与类型信息
        /// </summary>
        /// <param name="strFile_">文件名</param>
        /// <param name="strInfo_">类型信息</param>
        /// <returns>图标在列表中的位置</returns>
        public int GetFileIndex(string strFile_, out string strInfo_)
        {
            int nIndex = GetFileIndex(strFile_);

            // Get the info and return
            if (-1 != nIndex)
                strInfo_ = _lstInfo[nIndex];
            else
                strInfo_ = string.Empty;
            return nIndex;
        }

        /// <summary>
        /// 获取文件夹的图标
        /// </summary>
        /// <returns>图标在列表中的位置</returns>
        public int GetDirIndex()
        {
            return _nDirIndex;
        }

        /// <summary>
        /// 获取文件夹的图标与类型信息
        /// </summary>
        /// <param name="strInfo_">类型信息</param>
        /// <returns>图标在列表中的位置</returns>
        public int GetDirIndex(out string strInfo_)
        {
            if (-1 != _nDirIndex)
                strInfo_ = _lstInfo[_nDirIndex];
            else
                strInfo_ = string.Empty;

            return _nDirIndex;
        }

        /// <summary>
        /// 自己添加图标
        /// </summary>
        /// <param name="hIcon_">图标的句柄</param>
        /// <param name="strName_">图标名称（唯一确定图标）</param>
        /// <param name="strInfo_">图标说明</param>
        /// <returns>图标在列表中的位置</returns>
        public int AddIcon(Icon hIcon_, string strName_, string strInfo_)
        {
            int nIndex = _lstImages.Images.IndexOfKey(strName_);
            if (-1 == nIndex)
            {
                try
                {
                    nIndex = _lstImages.Images.Count;
                    _lstImages.Images.Add(strName_, hIcon_);
                    _lstInfo.Add(strInfo_);
                }
                catch { }
            }

            return nIndex;
        }

        /// <summary>
        /// 获取指定名称的图标
        /// </summary>
        /// <param name="strName_">图标名称（唯一确定图标）</param>
        /// <returns>图标在列表中的位置</returns>
        public int GetIconIndex(string strName_)
        {
            return _lstImages.Images.IndexOfKey(strName_);
        }

        /// <summary>
        /// 获取指定名称的图标与信息
        /// </summary>
        /// <param name="strName_">图标名称（唯一确定图标）</param>
        /// <param name="strInfo_">图标说明</param>
        /// <returns></returns>
        public int GetIconIndex(string strName_, out string strInfo_)
        {
            int nIndex = _lstImages.Images.IndexOfKey(strName_);
            if (-1 == nIndex)
                strInfo_ = string.Empty;
            else
                strInfo_ = _lstInfo[nIndex];

            return nIndex;
        }
    };
}
