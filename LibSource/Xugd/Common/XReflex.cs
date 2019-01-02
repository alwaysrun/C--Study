using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SHCre.Xugd.Common
{    
    /// <summary>
    /// 反射相关定义
    /// </summary>
    public static class XReflex
    {
        /// <summary>
        /// 获取类型的完整名称
        /// </summary>
        /// <param name="objType_"></param>
        /// <param name="bFullName_">是否获取完整名称（包括完全限定名）</param>
        /// <returns></returns>
        public static string GetTypeName(Type objType_, bool bFullName_)
        {
            if (objType_ == null) return "(NULL)";

            string strName;
            if (bFullName_)
            {
                strName = objType_.FullName;
            }
            else
            {
                strName = objType_.Name;
                if (objType_.IsGenericType)
                {
                    try
                    {
                        Type[] tGens = objType_.GetGenericArguments();

                        if (tGens.Length > 0)
                        {
                            string strGens = string.Empty;
                            foreach (Type t in tGens)
                                strGens += t.Name + ",";
                            strName += string.Format("<{0}>", strGens.TrimEnd(','));
                        }
                    }
                    catch (NotSupportedException) { }
                }
            }

            return strName;
        }

        /// <summary>
        /// 获取类型的完整名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tData_"></param>
        /// <param name="bFullName_">是否获取完整名称（包括完全限定名）</param>
        /// <returns></returns>
        public static string GetTypeName<T>(T tData_, bool bFullName_)
        {
            Type objType = GetTypeFromInstance<T>(tData_);

            return GetTypeName(objType, bFullName_);
        }

        private static Type GetTypeFromInstance<T>(T tData_)
        {
            Type objType;
            if (tData_ == null)
                objType = typeof(T);
            else
                objType = tData_.GetType();
            return objType;
        }

        /// <summary>
        /// 获取类型所在程序集
        /// </summary>
        /// <param name="tInAssembly_">程序集中的任一类型(最简单则使用this.GetType())</param>
        /// <returns></returns>
        public static Assembly GetAssembly(Type tInAssembly_)
        {
            return Assembly.GetAssembly(tInAssembly_);
        }

        /// <summary>
        /// 获取类型所在程序集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tData_">程序集中的一个类型变量</param>
        /// <returns></returns>
        public static Assembly GetAssembly<T>(T tData_)
        {
            Type objType = GetTypeFromInstance<T>(tData_);
            return GetAssembly(objType);
        }

        /// <summary>
        /// 根据完整名称获取程序集
        /// </summary>
        /// <param name="strFileName_"></param>
        /// <returns></returns>
        public static Assembly GetAssembly(string strFileName_)
        {
            return Assembly.LoadFrom(strFileName_);
        }

        /// <summary>
        /// 根据类型名，构造对应实例
        /// </summary>
        /// <param name="strTypeFullName_">类型名</param>
        /// <param name="assContainsType_">类型所在程序集</param>
        /// <param name="bIgnoreCase_">对类型名是否区分大小写</param>
        /// <returns></returns>
        public static object BuildInstance(string strTypeFullName_, Assembly assContainsType_ = null, bool bIgnoreCase_ = false)
        {
            Type objType = GetTypeFromName(strTypeFullName_, assContainsType_, bIgnoreCase_);
            var objConstruct = objType.GetConstructor(Type.EmptyTypes);
            if (objConstruct == null)
            {
                string strInfo;
                if (objType.IsArray)
                    strInfo = " Is array, use BuildArray()";
                else
                    strInfo = " No default constructor";
                throw new ArgumentException(strTypeFullName_ + strInfo);
            }
            return objConstruct.Invoke(new object[0]);
        }

        private static Type GetTypeFromName(string strTypeFullName_, Assembly assContainsType_, bool bIgnoreCase_)
        {
            Type objType;
            if (assContainsType_ == null)
                objType = Type.GetType(strTypeFullName_, true, bIgnoreCase_);
            else
                objType = assContainsType_.GetType(strTypeFullName_, true, bIgnoreCase_);
            return objType;
        }

        /// <summary>
        /// 根据类型构造数组实例
        /// </summary>
        /// <param name="strItemTypeName_">数组成员的类型</param>
        /// <param name="nLength_">数组长度</param>
        /// <param name="assContainsType_">类型所在程序集</param>
        /// <param name="bIgnoreCase_">对类型名是否区分大小写</param>
        /// <returns></returns>
        public static Array BuildArray(string strItemTypeName_, int nLength_, Assembly assContainsType_ = null, bool bIgnoreCase_ = false)
        {
            Type objType = GetTypeFromName(strItemTypeName_, assContainsType_, bIgnoreCase_);
            return Array.CreateInstance(objType, nLength_);
        }
    }
}
