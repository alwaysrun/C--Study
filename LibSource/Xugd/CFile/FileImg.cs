using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using SHCre.Xugd.WinAPI;

namespace SHCre.Xugd.CFile
{
    /// <summary>
    /// Сͼ���б��ࣺ����Сͼ�����Ӧ�ļ�������˵����
    /// .exe(��ִ���ļ�)��.ico(ͼ���ļ�)��.lnk(��ݷ�ʽ)��.msc(΢��ͨ�ù����ĵ�)��.scr(��������)��
    /// ÿ�������ͼ�궼��ͬ��������Ҫ�����ļ���������ȡ����������ֱ�Ӹ�����չ����ȡ
    /// 
    /// ͨ��GetList��ȡ�б�����ֱ�Ӹ�ֵ��ListVIew��SmallImageList����
    /// Ȼ��ͨ��GetFileIndex��GetDirIndex����ȡͼ��λ�ã�ֱ�Ӹ�ֵ��ListViewItem��ImageIndex���ɣ���������Ϣ
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
        /// ��ʼ�����캯��
        /// </summary>
        public XFileImgs()
        {
            _lstImages = new ImageList();
            _lstImages.ColorDepth = ColorDepth.Depth32Bit;
            _lstInfo = new List<string>();

            Reset();
        }

        /// <summary>
        /// �Ƴ�����ͼ����Ϣ��������ӡ�
        /// һ��������ö��ʱ�������µ��ļ��У��ر����ļ��仯�Ƚϴ�ʱ����Ҫ���ô˽ӿڡ�
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
        /// �������ͼ����Ϣ
        /// </summary>
        public void Clear()
        {
            _lstImages.Images.Clear();
            _lstInfo.Clear();
        }

        /// <summary>
        /// ��ȡͼ���б�
        /// </summary>
        /// <returns>Сͼ���б�</returns>
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
        /// ��ȡ�ļ���Ӧͼ��
        /// </summary>
        /// <param name="strFile_">�ļ���</param>
        /// <returns>ͼ�����б��е�λ��</returns>
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
        /// ��ȡ�ļ���Ӧͼ���λ����������Ϣ
        /// </summary>
        /// <param name="strFile_">�ļ���</param>
        /// <param name="strInfo_">������Ϣ</param>
        /// <returns>ͼ�����б��е�λ��</returns>
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
        /// ��ȡ�ļ��е�ͼ��
        /// </summary>
        /// <returns>ͼ�����б��е�λ��</returns>
        public int GetDirIndex()
        {
            return _nDirIndex;
        }

        /// <summary>
        /// ��ȡ�ļ��е�ͼ����������Ϣ
        /// </summary>
        /// <param name="strInfo_">������Ϣ</param>
        /// <returns>ͼ�����б��е�λ��</returns>
        public int GetDirIndex(out string strInfo_)
        {
            if (-1 != _nDirIndex)
                strInfo_ = _lstInfo[_nDirIndex];
            else
                strInfo_ = string.Empty;

            return _nDirIndex;
        }

        /// <summary>
        /// �Լ����ͼ��
        /// </summary>
        /// <param name="hIcon_">ͼ��ľ��</param>
        /// <param name="strName_">ͼ�����ƣ�Ψһȷ��ͼ�꣩</param>
        /// <param name="strInfo_">ͼ��˵��</param>
        /// <returns>ͼ�����б��е�λ��</returns>
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
        /// ��ȡָ�����Ƶ�ͼ��
        /// </summary>
        /// <param name="strName_">ͼ�����ƣ�Ψһȷ��ͼ�꣩</param>
        /// <returns>ͼ�����б��е�λ��</returns>
        public int GetIconIndex(string strName_)
        {
            return _lstImages.Images.IndexOfKey(strName_);
        }

        /// <summary>
        /// ��ȡָ�����Ƶ�ͼ������Ϣ
        /// </summary>
        /// <param name="strName_">ͼ�����ƣ�Ψһȷ��ͼ�꣩</param>
        /// <param name="strInfo_">ͼ��˵��</param>
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
