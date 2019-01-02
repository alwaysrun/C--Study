using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHCre.Xugd.Ssh
{
    /// <summary>
    /// 文件信息
    /// </summary>
    public class XFtpFileInfo
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 权限字符串
        /// </summary>
        public string Permission { get; set; }
        /// <summary>
        /// 权限标志位
        /// </summary>
        public int PermFlag { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime MTime { get; set; }
        /// <summary>
        /// 是否是目录
        /// </summary>
        public bool IsDir { get; set; }
    }
}
