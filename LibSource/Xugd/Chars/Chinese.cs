using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Microsoft.International.Converters.PinYinConverter;

namespace SHCre.Xugd.Chars
{
    /// <summary>
    /// �����йغ��ֵ���
    /// </summary>
    public static class Chinese
    {

        /// <summary>
        /// ƴ��������
        /// </summary>
        public enum Tones
        {
            /// <summary>
            /// һ��ƽ����ƽ
            /// </summary>
            Yinping = 1,
            /// <summary>
            /// ����������ƽ
            /// </summary>
            Yangping,
            /// <summary>
            /// �����䣬����
            /// </summary>
            Shangsheng,
            /// <summary>
            /// ��������ȥ��
            /// </summary>
            Qusheng
        };

        /// <summary>
        /// ��ȡ������������1��2��3��4������������ƴ����
        /// ������Ч����ʱ���ַ������֡��հ׶����������֣����׳�NotSupportedException
        /// </summary>
        /// <param name="chHanzi_">Ҫ�жϵĺ���</param>
        /// <returns>���ֶ�Ӧ������ƴ���б�</returns>
        public static List<string> GetPinyinWithTone(char chHanzi_)
        {
            ChineseChar chChar = new ChineseChar(chHanzi_);
            return new List<string>(chChar.Pinyins);
        }

        /// <summary>
        /// ��ȡƴ����
        /// ������Ч����ʱ���ַ������֡��հ׶����������֣����׳�NotSupportedException
        /// </summary>
        /// <param name="chHanzi_">Ҫ�жϵĺ���</param>
        /// <returns>���ֶ�Ӧ��ƴ��</returns>
        public static string GetPinyin(char chHanzi_)
        {
            string strPinyin = GetPinyinWithTone(chHanzi_)[0];
            return strPinyin.Substring(0, strPinyin.Length - 1);
        }

        /// <summary>
        /// ��ȡ����������ƴ����
        /// ������Ч����ʱ���ַ������֡��հ׶����������֣����׳�NotSupportedException
        /// </summary>
        /// <param name="chHanzi_">Ҫ�жϵĺ���</param>
        /// <param name="euTone_">ƴ��������</param>
        /// <returns>���ֶ�Ӧƴ��</returns>
        public static string GetPinyin(char chHanzi_, out Tones euTone_)
        {
            string strPinyin = GetPinyinWithTone(chHanzi_)[0];
            euTone_ = (Tones)int.Parse(strPinyin[strPinyin.Length - 1].ToString());
            return strPinyin.Substring(0, strPinyin.Length - 1);
        }

        /// <summary>
        /// ��ȡ�����ַ�����Ӧ��ƴ������������������ƴ�����ÿո�ָ
        /// ���зǺ����ַ���ֱ����Ϊƴ������
        /// </summary>
        /// <param name="strHanzi_">�����ַ���</param>
        /// <returns>��Ӧ��ƴ��</returns>
        public static string GetPinyin(string strHanzi_)
        {
            StringBuilder sbPinyin = new StringBuilder(strHanzi_.Length * 3);
            foreach (char ch in strHanzi_.Trim())
            {
                if (char.IsPunctuation(ch) || char.IsWhiteSpace(ch))
                    sbPinyin.Append(ch);

                try 
                {
                    sbPinyin.Append(GetPinyin(ch) + " ");
                }
                catch
                {
                    sbPinyin.Append(ch);
                }
            }

            return sbPinyin.ToString().Trim();
        }

        /// <summary>
        /// �ж��Ƿ�����Ч�ĺ���(ʹ��΢��Ŀ⣬��ʼ�ٶȱȽ���)
        /// </summary>
        /// <param name="chHanzi_">Ҫ�жϵĺ���</param>
        /// <returns>�Ǻ��֣�����true�����򣬷���false</returns>
        public static bool IsValidHanzi(char chHanzi_)
        {
            return ChineseChar.IsValidChar(chHanzi_);
        }

        /// <summary>
        /// �ж��ַ������Ƿ��������(ʹ��΢��Ŀ⣬��ʼ�ٶȱȽ���)
        /// </summary>
        /// <param name="strHanzi_">Ҫ�жϵ��ַ���</param>
        /// <returns>�������֣�����true�����򣬷���false</returns>
        public static bool HasHanzi(string strHanzi_)
        {
            foreach(char ch in strHanzi_.Trim())
            {
                if (IsValidHanzi(ch))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// �ж��Ƿ�����Ч��Unicode����(ʹ�ñ��뷶Χ��'\u4E00'~'\u9FA5'��'\uF900'~'\uFA2D')
        /// </summary>
        /// <param name="chHanzi_">Ҫ�жϵĺ���</param>
        /// <returns>�Ǻ��֣�����true�����򣬷���false</returns>
        public static bool IsUnicodeHanzi(char chHanzi_)
        {
            return ((chHanzi_ >= '\u4E00') && (chHanzi_ <= '\u9FA5'))
                    || ((chHanzi_ >= '\uF900') && (chHanzi_ <= '\uFA2D'));
        }

        /// <summary>
        /// �ж��ַ������Ƿ��������(ʹ�ñ��뷶Χ��'\u4E00'~'\u9FA5'��'\uF900'~'\uFA2D')
        /// </summary>
        /// <param name="strHanzi_">Ҫ�жϵ��ַ���</param>
        /// <returns>�������֣�����true�����򣬷���false</returns>
        public static bool HasUnicodeHanzi(string strHanzi_)
        {
            Regex regHanzi = new Regex(@"[\u4E00-\u9FA5\uF900-\uFA2D]+");
            return regHanzi.IsMatch(strHanzi_);
        }

        /// <summary>
        /// �ж��Ƿ�����Ч��ƴ��
        /// </summary>
        /// <param name="strPinyin_">ƴ��</param>
        /// <param name="euTone_">����</param>
        /// <returns>��ƴ��������true�����򣬷���false</returns>
        public static bool IsValidPinyin(string strPinyin_, Tones euTone_)
        {
            return ChineseChar.IsValidPinyin(strPinyin_.Trim() + ((int)euTone_).ToString());
        }
    }
}
