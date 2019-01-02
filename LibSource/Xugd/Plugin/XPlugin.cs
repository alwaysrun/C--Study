using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using SHCre.Xugd.Common;
using SHCre.Xugd.CFile;

namespace SHCre.Xugd.Plugin
{
    /// <summary>
    /// 用于插件或动态库的加载与配置
    /// </summary>
    public static class XPlugin
    {
        /// <summary>
        /// 从程序集中，动态构造类实例(需要缺省构造函数)；出错抛出XReflectException
        /// </summary>
        /// <typeparam name="TApp"></typeparam>
        /// <param name="strTypeName_">类的全名（包括命名空间）</param>
        /// <param name="strAssemblyName_">包含类的程序集名（弱名称一般就是文件名）</param>
        /// <param name="bIgnoreCase_">匹配类名时，是否区分大小写</param>
        /// <returns></returns>
        public static TApp CreateApp<TApp>(string strTypeName_, string strAssemblyName_, bool bIgnoreCase_ = false) where TApp : class
        {
            var assDll = Assembly.Load(XString.TrimEnd(strAssemblyName_, ".dll"));
            var vInst = assDll.CreateInstance(strTypeName_, bIgnoreCase_) as TApp;
            if (vInst == null)
                throw new XReflectException(strTypeName_, strAssemblyName_, typeof(TApp));

            return vInst;
        }

        /// <summary>
        /// 从库文件中，动态构造类实例(需要缺省构造函数)；出错抛出XReflectException
        /// </summary>
        /// <typeparam name="TApp"></typeparam>
        /// <param name="strTypeName_">类的全名（包括命名空间）</param>
        /// <param name="strFileName_">要加载的文件（Dll）</param>
        /// <param name="bIgnoreCase_">匹配类名时，是否区分大小写</param>
        /// <returns></returns>
        public static TApp CreateFrom<TApp>(string strTypeName_, string strFileName_, bool bIgnoreCase_ = false) where TApp : class
        {
            var assDll = Assembly.LoadFrom(XPath.GetFullPath(strFileName_));
            var vInst = assDll.CreateInstance(strTypeName_, bIgnoreCase_) as TApp;
            if (vInst == null)
                throw new XReflectException(strTypeName_, strFileName_, typeof(TApp));

            return vInst;
        }

        ///// <summary>
        ///// 动态构造类实例(需要缺省构造函数)；出错抛出XReflectException
        ///// </summary>
        ///// <typeparam name="TApp"></typeparam>
        ///// <param name="conPlugIn_"></param>
        ///// <returns></returns>
        //public static TApp CreateFrom<TApp>(XPluginLoadConfig conPlugIn_) where TApp : class
        //{
        //    var assDll = Assembly.LoadFrom(XPath.GetFullPath(conPlugIn_.FileName));
        //    var vInst = assDll.CreateInstance(conPlugIn_.ClassName) as TApp;
        //    if (vInst == null)
        //        throw new XReflectException(conPlugIn_.ClassName, conPlugIn_.FileName, typeof(TApp));

        //    return vInst;
        //}

        /// <summary>
        /// 根据库名，动态构造类实例(需要缺省构造函数)；出错抛出XReflectException
        /// </summary>
        /// <typeparam name="TApp"></typeparam>
        /// <param name="conDll_"></param>
        /// <returns></returns>
        public static TApp CreateFrom<TApp>(XDllLoadConfig conDll_) where TApp : class
        {
            var assDll = Assembly.LoadFrom(XPath.GetFullPath(conDll_.FileName));
            var vInst = assDll.CreateInstance(conDll_.ClassName) as TApp;
            if (vInst == null)
                throw new XReflectException(conDll_.ClassName, conDll_.FileName, typeof(TApp));

            return vInst;
        }
    }
}
