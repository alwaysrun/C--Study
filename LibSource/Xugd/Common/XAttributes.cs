using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// 边界类型
    /// </summary>
    public enum XBoundaryType
    {
        /// <summary>
        /// 不是边界
        /// </summary>
        None = 0,
        /// <summary>
        /// 下边界
        /// </summary>
        Lower,
        /// <summary>
        /// 上边界
        /// </summary>
        Upper,
    }

    /// <summary>
    /// 用于描述枚举边界信息
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class XEnumMarkAttribute:Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public XBoundaryType BoundType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XEnumMarkAttribute()
        {
            BoundType = XBoundaryType.None;
            Remark = string.Empty;
        }
    }

    /// <summary>
    /// 定义属性相关函数
    /// </summary>
    public static class XAttr
    {
        /// <summary>
        /// 获取枚举描述字段
        /// </summary>
        /// <param name="enumType_"></param>
        /// <returns></returns>
        public static Dictionary<string,XEnumMarkAttribute> GetEnumMarkField(Type enumType_)
        {
            Dictionary<string, XEnumMarkAttribute> dictAttri = new Dictionary<string, XEnumMarkAttribute>();
            var fsAll = enumType_.GetFields(BindingFlags.Static | BindingFlags.Public);
            foreach(var fd in fsAll)
            {
                var elAttr = fd.GetCustomAttributes(typeof(XEnumMarkAttribute), false);
                if(elAttr.Length > 0)
                {
                    dictAttri.Add(fd.Name, elAttr[0] as XEnumMarkAttribute);
                }
            }

            return dictAttri;
        }
    } // XAttr
}
