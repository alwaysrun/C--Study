using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHCre.Xugd.Data
{
    /// <summary>
    /// 检测字符串是否为Json字符串：
    /// 只是检测分隔符（'"','{','}','[',']')是否匹配（成对出现）。
    /// </summary>
    public class XJsonDataCheck
    {
        /// <summary>
        /// 发现Json字符串时调用，
        /// 参数为发现的Json字符串
        /// </summary>
        public Action<string> ActFoundJson;

        private List<JsonString> _lstJsons = new List<JsonString>();
        private Stack<char> _stackSeparators = new Stack<char>();
        static readonly char[] JsonSeparator = new char[] { '{', '[', '}', ']', '\"' };
        static readonly char[] StartSeparator = new char[] { '{', '[' };
        static readonly char[] EndSeparator = new char[] { '}', ']' };
        const char QuotationSeparator = '\"';
        const char EscSeparator = '\\';
        private bool _bIsInQuotation = false;
        private bool _bIsJsonStart = false;
        private string _strEndEsc = string.Empty;

        /// <summary>
        /// 输入的文本，在除Json字符串外剩余部分
        /// </summary>
        public string RemainData { get; private set; }

        /// <summary>
        /// 添加字符串：起始字符串（第一个添加的）必须以'[','{'开始；
        /// 发现Json字符串后，会激发ActFoundJson；
        /// 如果出错会抛出XJsonDataException异常
        /// </summary>
        /// <param name="strData_"></param>
        public void Add(string strData_)
        {
            Add(strData_, 0);
        }

        /// <summary>
        /// 添加字符串：起始字符串（第一个添加的）必须以'[','{'开始；
        /// 发现Json字符串后，会激发ActFoundJson；
        /// 如果出错会抛出XJsonDataException异常
        /// </summary>
        /// <param name="strData_"></param>
        /// <param name="nStart_"></param>
        public void Add(string strData_, int nStart_)
        {
            if (string.IsNullOrEmpty(strData_)) return;
            if (nStart_ >= strData_.Length)
                throw new ArgumentException("Start invalid");

            if (!_bIsJsonStart)
            {
                Clear(false);

                Complete();
            }

            AddJson(strData_, nStart_);
        }

        /// <summary>
        /// 完成添加：
        /// 如果RemainData中包含Json字符串，则会全部检测出来。
        /// </summary>
        public void Complete()
        {
            if (_bIsJsonStart)
                return;

            while (!string.IsNullOrEmpty(RemainData))
            {
                string strTmp = RemainData;
                RemainData = string.Empty;
                AddJson(strTmp, 0);
            }
        }

        private void AddJson(string strData_, int nStart_)
        {
            if (!_bIsJsonStart)
            {
                if (!FoundStart(strData_, nStart_))
                    throw new XJsonDataException("Not with json start separator: { or [");

                _bIsJsonStart = true;
            }

            FoundSeparator(strData_, nStart_);
        }

        private static bool FoundStart(string strData_, int nStart_)
        {
            return StartSeparator.Contains(strData_[nStart_]);
        }

        private void FoundSeparator(string strData_, int nStart_)
        {
            bool bFound = false;
            int nEndIndex = -1;
            string strAllData = _strEndEsc + strData_.Substring(nStart_);
            if (_bIsInQuotation)
                bFound = FoundQuotation(strAllData, 0, ref _bIsInQuotation, _stackSeparators, out nEndIndex);
            else
                bFound = FoundAnySeparator(strAllData, 0, ref _bIsInQuotation, _stackSeparators, out nEndIndex);

            if (!bFound)
            {
                _strEndEsc = GetEndEsc(strAllData);
                _lstJsons.Add(new JsonString(strData_, nStart_));
                return;
            }

            // Found
            nStart_ = _strEndEsc.Length;
            _strEndEsc = string.Empty;
            RemainData = strAllData.Substring(nEndIndex + 1);
            if (ActFoundJson != null)
            {
                string strJson = strAllData.Substring(nStart_, nEndIndex + 1 - nStart_);
                if (_lstJsons.Count > 0)
                {
                    int nTotalLen = _lstJsons.Sum(z => z.DataLen);
                    StringBuilder sbJson = new StringBuilder(nTotalLen);
                    _lstJsons.ForEach(z => sbJson.Append(z.JsonData));

                    strJson = sbJson.ToString() + strJson;
                }

                ActFoundJson(strJson);
            }

            Clear(false);
        }

        private static bool FoundQuotation(string strData_, int nStart_, ref bool bInQuotation_, Stack<char> stackSeparators_, out int nEndIndex_)
        {
            int nIndex = strData_.IndexOf(QuotationSeparator, nStart_);
            if (nIndex == -1)
            {
                nEndIndex_ = -1;
                return false;
            }

            if (IsReallyQuote(strData_, nStart_, nIndex))
            { // even number, is really quotation
                bInQuotation_ = false;
                return FoundAnySeparator(strData_, nIndex + 1, ref bInQuotation_, stackSeparators_, out nEndIndex_);
            }
            else
            { // Is escaped, continue to find
                return FoundQuotation(strData_, nIndex + 1, ref bInQuotation_, stackSeparators_, out nEndIndex_);
            }
        }

        private string GetEndEsc(string strData_)
        {
            int nIndex = strData_.Length;
            while(nIndex > 0)
            {
                if (strData_[nIndex - 1] != EscSeparator)
                    break;

                --nIndex;
            }

            return strData_.Substring(nIndex);
        }

        private static bool IsReallyQuote(string strData_, int nStart_, int nLen_)
        {
            int nCount = 0;
            while (nLen_ > nStart_)
            {
                if (strData_[--nLen_] == EscSeparator)
                    nCount++;
                else
                    break;
            }

            return nCount % 2 == 0;
        }

        private static bool FoundAnySeparator(string strData_, int nStart_, ref bool bInQuotation_, Stack<char> stackSeparators_, out int nEndIndex_)
        {
            int nIndex = strData_.IndexOfAny(JsonSeparator, nStart_);
            if (nIndex == -1)
            {
                nEndIndex_ = -1;
                return false;
            }

            if(strData_[nIndex] == QuotationSeparator)
            {
                if(IsReallyQuote(strData_, nStart_, nIndex))
                {
                    bInQuotation_ = true;
                    return FoundQuotation(strData_, nIndex + 1, ref bInQuotation_, stackSeparators_, out nEndIndex_);
                }
            }
            else
            {
                if (StartSeparator.Contains(strData_[nIndex]))
                {
                    stackSeparators_.Push(strData_[nIndex]);
                }
                else // It must end separator
                {
                    if(!IsPairSeparator(stackSeparators_.Peek(), strData_[nIndex]))
                        throw new XJsonDataException(string.Format("Invalid Separator {0} at {1}", strData_[nIndex], nIndex));
                                        
                    stackSeparators_.Pop();
                    if(stackSeparators_.Count == 0)
                    { // Found end, a json data found
                        nEndIndex_ = nIndex;
                        return true;
                    }
                }                    
            }
                
            return FoundAnySeparator(strData_, nIndex+1, ref bInQuotation_, stackSeparators_, out nEndIndex_);
        }

        private static bool IsPairSeparator(char chStart_, char chEnd_)
        {
            int nIndex = Array.IndexOf(StartSeparator, chStart_);
            if (nIndex == -1)
                throw new XJsonDataException(string.Format("Start separator {0} in stack invalid, This should not happen", chStart_));

            return EndSeparator[nIndex] == chEnd_;
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void Clear()
        {
            Clear(true);
        }

        private void Clear(bool bIncludeRemain_)
        {
            if (bIncludeRemain_)
                RemainData = string.Empty;

            _lstJsons.Clear();
            _stackSeparators.Clear();
            _bIsInQuotation = _bIsJsonStart = false;
        }

        /// <summary>
        /// 判断字符串是否是Json字符串：以'{'或'['开始，以'}'或']'结束；
        /// 且分隔符匹配，即'"','{','}','[',']'成对出现
        /// </summary>
        /// <param name="strData_"></param>
        /// <returns></returns>
        public static bool IsJson(string strData_)
        {
            return IsJson(strData_, 0);
        }

        /// <summary>
        /// 判断字符串是否是Json字符串：以'{'或'['开始，以'}'或']'结束；
        /// 且分隔符匹配，即'"','{','}','[',']'成对出现
        /// </summary>
        /// <param name="strData_"></param>
        /// <param name="nStart_"></param>
        /// <returns></returns>
        public static bool IsJson(string strData_, int nStart_)
        {
            if (string.IsNullOrEmpty(strData_) || nStart_ >= strData_.Length)
                return false;

            if (!StartSeparator.Contains(strData_[nStart_]))
                return false;
            if (!IsPairSeparator(strData_[nStart_], strData_[strData_.Length - 1]))
                return false;

            // check the string now
            int nEndIndex = -1;
            bool bInQuotation = false;
            Stack<char> stackSeparator = new Stack<char>();
            bool bFound = FoundAnySeparator(strData_, nStart_, ref bInQuotation, stackSeparator, out nEndIndex);

            return bFound && nEndIndex == strData_.Length - 1;
        }

        private class JsonString
        {
            public int Start { get; private set; }
            public string Data { get; private set; }

            public int DataLen
            {
                get
                {
                    return Data.Length - Start;
                }
            }

            public string JsonData
            {
                get
                {
                    return Data.Substring(Start);
                }
            }

            public JsonString(string strData_)
                : this(strData_, 0)
            { }

            public JsonString(string strData_, int nStart_)
            {
                Data = strData_;
                Start = nStart_;
            }
        }
    }
}
