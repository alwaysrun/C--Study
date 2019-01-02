using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Common;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SHCre.Xugd.Extension
{
    /// <summary>
    /// 对Oject对象的扩展函数
    /// </summary>
    public static class XObjectExtension
    {
        /// <summary>
        /// 获取类型对应类的名称：
        /// 如果是泛型只是返回泛型自身的类型，如List、Queue等
        /// </summary>
        /// <param name="objData_"></param>
        /// <returns></returns>
        public static string SimpleTypeName(this object objData_)
        {
            return objData_.GetType().Name;
        }

        /// <summary>
        /// 获取类型的对应名称：
        /// 如果是泛型返回泛型自身以及内部元素的类型。
        /// </summary>
        /// <param name="objData_"></param>
        /// <returns></returns>
        public static string ComplexTypeName(this object objData_)
        {
            return GetTypeName(objData_.GetType());
        }

        /// <summary>
        /// 打印内容（公共属性与字段），如果失败返回类名：
        /// 如果DeepPrint为true，则递归打印包含的内容（如子类、数字、列表等）；为false时，只打印当前类中的内容
        /// </summary>
        /// <param name="objData_"></param>
        /// <param name="bDeepPrint_"></param>
        /// <returns></returns>
        public static string PrintObjectSafe(this object objData_, bool bDeepPrint_)
        {
            try
            {
                return objData_.PrintObject(bDeepPrint_);
            }
            catch{}

            return objData_.SimpleTypeName();
        }

        /// <summary>
        /// 打印内容（公共属性与字段）：
        /// 如果DeepPrint为true，则递归打印包含的内容（如子类、数字、列表等）；为false时，只打印当前类中的内容
        /// </summary>
        /// <param name="objData_"></param>
        /// <param name="bDeepPrint_"></param>
        /// <returns></returns>
        public static string PrintObject(this object objData_, bool bDeepPrint_=true)
        {
            return objData_.PrintObject(bDeepPrint_, 0);
        }

        private static string PrintObject(this object objData_, bool bDeepPrint_, int nIndent_)
        {
            if (objData_ == null)
                return string.Empty;

            Type tData = objData_.GetType();
            string strResult = tData.Name + ": ";
            if (tData == typeof(string) || tData.IsValueType || tData == typeof(byte[]) || tData == typeof(Encoding))
            {
                strResult += PrintObjectValue(objData_, bDeepPrint_, 0);
                if (!strResult.EndsWith(XString.NewLine))
                    strResult += XString.NewLine;
                return strResult;
            }

            if (tData.IsClass)
            {
                if (IsGeneric(objData_, tData))
                    return PrintObjectValue(objData_, bDeepPrint_, nIndent_ + 1);

                strResult += XString.NewLine;
            }
            else
                strResult += XString.BlankSpace;
            string strTap = XString.TapIndent2Blanks(nIndent_+1);
            foreach(MemberInfo mInfo in tData.GetMembers(BindingFlags.Public|BindingFlags.Instance))
            {
                if (mInfo.MemberType != MemberTypes.Field && mInfo.MemberType != MemberTypes.Property)
                    continue;

                strResult += string.Format("{0}{1}: ", strTap, mInfo.Name);

                object objMem = null;
                if(mInfo.MemberType == MemberTypes.Field)
                {
                    objMem = (mInfo as FieldInfo).GetValue(objData_);
                }
                else // Property
                {
                    if ((mInfo as PropertyInfo).GetIndexParameters().Length == 0)
                        objMem = (mInfo as PropertyInfo).GetValue(objData_, new object[0]);
                }

                if (objMem == null)
                    strResult += "null";
                else
                    strResult += PrintObjectValue(objMem, bDeepPrint_, nIndent_+2);

                if (!strResult.EndsWith(XString.NewLine))
                    strResult += XString.NewLine;
            }

            if (!strResult.EndsWith(XString.NewLine))
                strResult += XString.NewLine;
            return strResult;
        }

        private static string PrintObjectValue(object objMember_, bool bDeepPrint_, int nIndent_)
        {
            string strResult = string.Empty;

            if (objMember_ is DateTime)
                return strResult + XTime.GetFullString((DateTime)objMember_, true);
            
            Type tData = objMember_.GetType();
            if (tData == typeof(string) || tData == typeof(char))
                return strResult + Regex.Escape(objMember_.ToString());
            if(tData.IsValueType)
                return strResult + objMember_.ToString();

            if (tData == typeof(byte[]))
                return strResult + BitConverter.ToString((byte[])objMember_);

            if (objMember_ is Encoding)
                return strResult + ((Encoding)objMember_).WebName;

            if(objMember_ is IEnumerable)
            {
                string strTap = XString.TapIndent2Blanks(nIndent_);
                IEnumerable iEnum = objMember_ as IEnumerable;

                string strName = GetTypeName(tData);
                if(bDeepPrint_)
                    strResult += strName + XString.NewLine;
                int nIndex = 0;
                foreach(object obj in iEnum)
                {
                    if (bDeepPrint_)
                    {
                        strResult += string.Format("{0}Elem[{1}]: ", strTap, nIndex);

                        if (obj is string || obj is DateTime || obj is ValueType)
                            strResult += obj.ToString();
                        else
                            strResult += obj.PrintObject(bDeepPrint_, nIndent_);

                        if (!strResult.EndsWith(XString.NewLine))
                            strResult += XString.NewLine;
                    }

                    ++nIndex;
                }
                if (!bDeepPrint_)
                {
                    strResult += string.Format("{0} Elems in {1}", nIndex, strName);
                }

                return strResult;
            }

            return strResult + (bDeepPrint_?objMember_.PrintObject(bDeepPrint_, nIndent_):tData.Name);
        }

        private static string GetTypeName(Type tData_)
        {
            string strName = tData_.Name;
            try 
            {
                Type[] tGens = tData_.GetGenericArguments();

                if (tGens.Length > 0)
                {
                    string strGens = string.Empty;
                    foreach (Type t in tGens)
                        strGens += t.Name + ",";
                    strName += string.Format("<{0}>", strGens.TrimEnd(','));
                }
            }
            catch(NotSupportedException){}

            return strName;
        }

        private static bool IsGeneric(object objData_, Type tData_)
        {
            bool bIsGen = false;
            if (objData_ is IEnumerable)
            {
                try
                {
                    Type[] tGens = tData_.GetGenericArguments();
                    bIsGen = (tGens.Length > 0);
                }
                catch (NotSupportedException) { }
            }

            return bIsGen;
        }
    } // class
} // namespace
