using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.Data
{
    /// <summary>
    /// 管理Byte[]的类。
    /// 对数据操作时，分为是否为Peek：
    /// 如果Peek，说明只是移动Peek指针，并不影响实际的数据（可通过ResetPeek还原后，再次取数据；也可通过StorePeek，把Peek过的数据清除）；
    /// 如果是非Peek，则会影响实际的数据，即取出过的数据将被清除（将不能再次去取）；
    /// </summary>
    public class XDataBuffer
    {
        /// <summary>
        /// 变化的类型
        /// </summary>
        public enum ChangeMode
        {
            /// <summary>
            /// 
            /// </summary>
            Invalid = 0,
            /// <summary>
            /// 添加
            /// </summary>
            Add,
            /// <summary>
            /// 清空
            /// </summary>
            Clear,
        };

        /// <summary>
        /// 数据变化（Add、Clear）时，引发的动作
        /// </summary>
        public Action<ChangeMode> BufferChanged { get; set; }

        // Fields
        private readonly List<BytesSegment> Buffer;

        /// <summary>
        /// 
        /// </summary>
        public XDataBuffer()
        {
            Buffer = new List<BytesSegment>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nDefBolcks_"></param>
        public XDataBuffer(int nDefBolcks_)
        {
            Buffer = new List<BytesSegment>(nDefBolcks_);
        }

        #region "Buffer"
        /// <summary>
        /// 数据实际长度
        /// </summary>
        public int Length
        {
            get
            {
                lock (this)
                {
                    return this.GetLength(false);
                }
            }
        }

        /// <summary>
        /// 可Peek的数据长度
        /// </summary>
        public int PeekLength
        {
            get
            {
                lock (this)
                {
                    return this.GetLength(true);
                }
            }
        }

        private int GetLength(bool bPeek_ = false)
        {
            int nLen = 0;
            if (bPeek_)
            {
                this.Buffer.ForEach(delegate(BytesSegment x)
                {
                    nLen += x.PeekLength;
                });
            }
            else
            {
                this.Buffer.ForEach(delegate(BytesSegment x)
                {
                    nLen += x.Length;
                });
            }

            return nLen;
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="byData_"></param>
        public void Add(byte[] byData_)
        {
            this.Add(byData_, 0, byData_.Length);
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="byData_"></param>
        /// <param name="nOffSet_"></param>
        /// <param name="nLen_"></param>
        public void Add(byte[] byData_, int nOffSet_, int nLen_)
        {
            if (byData_.Length < (nOffSet_ + nLen_))
                throw new XBinaryDataException(string.Format("XDataBuffer.Add(byte[], {0}, {1}): Lenth invalid", nOffSet_, nLen_), SHErrorCode.BadLength);

            lock (this)
            {
                this.Buffer.Add(new BytesSegment
                {
                    Buffer = byData_,
                    StartIndex = nOffSet_,
                    EndIndex = nOffSet_ + nLen_,
                    PeekStart = nOffSet_
                });
            }

            if (this.BufferChanged != null)
            {
                this.BufferChanged(ChangeMode.Add);
            }
        }

        /// <summary>
        /// 清空
        /// </summary>
        public void Clear()
        {
            lock (this)
            {
                this.Buffer.Clear();
            }

            if (this.BufferChanged != null)
            {
                this.BufferChanged(ChangeMode.Clear);
            }
        }

        /// <summary>
        /// 重设Peek数据的指针
        /// </summary>
        public void ResetPeek()
        {
            lock (this)
            {
                this.Buffer.ForEach(delegate(BytesSegment x)
                {
                    x.PeekStart = x.StartIndex;
                });
            }
        }

        /// <summary>
        /// 把Peek指针保存为真实的指针（即Peek过的数据设为无效）
        /// </summary>
        public void StorePeek()
        {
            lock (this)
            {
                this.Buffer.ForEach(delegate(BytesSegment x)
                {
                    x.StartIndex = x.PeekStart;
                });
                this.Buffer.RemoveAll(x => x.Length == 0);
            }
        }

        /// <summary>
        /// 压缩：移除所有长度为零的子数组
        /// </summary>
        public void Compress()
        {
            lock (this)
            {
                this.Buffer.RemoveAll(x => x.Length == 0);
            }
        }
        #endregion

        #region "Get"
        /// <summary>
        /// 获取布尔值（1字节）
        /// </summary>
        /// <param name="bPeek_"></param>
        /// <returns></returns>
        public bool GetBool(bool bPeek_ = false)
        {
            lock (this)
            {
                int nStartIndex;
                byte[] byBuffer = this.GetBuffer(bPeek_, 1, out nStartIndex, false);
                return (byBuffer[nStartIndex] != 0);
            }
        }
        
        /// <summary>
        /// 获取字符（2字节）
        /// </summary>
        /// <param name="bPeek_"></param>
        /// <returns></returns>
        public char GetChar(bool bPeek_ = false)
        {
            lock (this)
            {
                int nStartIndex;
                byte[] byBuffer = this.GetBuffer(bPeek_, 2, out nStartIndex, false);
                return BitConverter.ToChar(byBuffer, nStartIndex);
            }
        }

        /// <summary>
        /// 获取byte
        /// </summary>
        /// <param name="bPeek_"></param>
        /// <returns></returns>
        public byte GetByte(bool bPeek_ = false)
        {
            lock (this)
            {
                int nStartIndex;
                byte[] byBuffer = this.GetBuffer(bPeek_, 1, out nStartIndex, false);
                return byBuffer[nStartIndex];
            }
        }

        /// <summary>
        /// 获取缓冲区
        /// </summary>
        /// <param name="nLen_"></param>
        /// <param name="bPeek_"></param>
        /// <returns></returns>
        public byte[] GetBytes(int nLen_, bool bPeek_ = false)
        {
            lock (this)
            {
                int nStartIndex;
                return this.GetBuffer(bPeek_, nLen_, out nStartIndex, true);
            }
        }

        /// <summary>
        /// 获取数值: (s)byte, (u)short, (u)int, (u)lon, float, double, bool。
        /// 如果类型不匹配，抛出异常
        /// </summary>
        /// <param name="decType_"></param>
        /// <param name="bNetOrder_"></param>
        /// <param name="bPeek_"></param>
        /// <returns></returns>
        public dynamic GetDecimal(Type decType_, bool bNetOrder_, bool bPeek_ = false)
        {
            if (decType_ == typeof(bool))
            {
                return this.GetBool(bPeek_);
            }

            if (decType_ == typeof(int))
            {
                return this.GetInt32(bNetOrder_, bPeek_);
            }
            if (decType_ == typeof(uint))
            {
                return this.GetUInt32(bNetOrder_, bPeek_);
            }
            if (decType_ == typeof(short))
            {
                return this.GetInt16(bNetOrder_, bPeek_);
            }
            if (decType_ == typeof(ushort))
            {
                return this.GetUInt16(bNetOrder_, bPeek_);
            }
            if (decType_ == typeof(long))
            {
                return this.GetInt64(bNetOrder_, bPeek_);
            }
            if (decType_ == typeof(ulong))
            {
                return this.GetUInt64(bNetOrder_, bPeek_);
            }

            if (decType_ == typeof(double))
            {
                return this.GetDouble(bPeek_);
            }
            if (decType_ == typeof(float))
            {
                return this.GetSingle(bPeek_);
            }

            if (decType_ == typeof(byte))
            {
                return this.GetByte(bPeek_);
            }
            if (decType_ == typeof(sbyte))
            {
                return (sbyte)this.GetByte(bPeek_);
            }

            throw new NotSupportedException("GetDecimalImpl: Is not decimal type, " + decType_.ToString());
        }

        /// <summary>
        /// short
        /// </summary>
        /// <param name="bNetOrder_"></param>
        /// <param name="bPeek_"></param>
        /// <returns></returns>
        public short GetInt16(bool bNetOrder_, bool bPeek_ = false)
        {
            lock (this)
            {
                int nStartIndex;
                byte[] byBuffer = this.GetBuffer(bPeek_, 2, out nStartIndex, false);
                short ret = BitConverter.ToInt16(byBuffer, nStartIndex);

                if (bNetOrder_)
                    ret = IPAddress.HostToNetworkOrder(ret);
                return ret;
            }
        }

        /// <summary>
        /// int
        /// </summary>
        /// <param name="bNetOrder_"></param>
        /// <param name="bPeek_"></param>
        /// <returns></returns>
        public int GetInt32(bool bNetOrder_, bool bPeek_ = false)
        {
            lock (this)
            {
                int nStartIndex;
                byte[] byBuffer = this.GetBuffer(bPeek_, 4, out nStartIndex, false);
                int ret = BitConverter.ToInt32(byBuffer, nStartIndex);

                if (bNetOrder_)
                    ret = IPAddress.HostToNetworkOrder(ret);
                return ret;
            }
        }

        /// <summary>
        /// long
        /// </summary>
        /// <param name="bNetOrder_"></param>
        /// <param name="bPeek_"></param>
        /// <returns></returns>
        public long GetInt64(bool bNetOrder_, bool bPeek_ = false)
        {
            lock (this)
            {
                int nStartIndex;
                byte[] byBuffer = this.GetBuffer(bPeek_, 8, out nStartIndex, false);
                long ret = BitConverter.ToInt64(byBuffer, nStartIndex);

                if (bNetOrder_)
                    ret = IPAddress.HostToNetworkOrder(ret);
                return ret;
            }
        }

        /// <summary>
        /// ushort
        /// </summary>
        /// <param name="bNetOrder_"></param>
        /// <param name="bPeek_"></param>
        /// <returns></returns>
        public ushort GetUInt16(bool bNetOrder_, bool bPeek_ = false)
        {
            return (ushort)GetInt16(bNetOrder_, bPeek_);
        }

        /// <summary>
        /// uint
        /// </summary>
        /// <param name="bNetOrder_"></param>
        /// <param name="bPeek_"></param>
        /// <returns></returns>
        public uint GetUInt32(bool bNetOrder_, bool bPeek_ = false)
        {
            return (uint)GetInt32(bNetOrder_, bPeek_);
        }

        /// <summary>
        /// ulong
        /// </summary>
        /// <param name="bNetOrder_"></param>
        /// <param name="bPeek_"></param>
        /// <returns></returns>
        public ulong GetUInt64(bool bNetOrder_, bool bPeek_ = false)
        {
            return (ulong)GetInt64(bNetOrder_, bPeek_);
        }

        /// <summary>
        /// double(8bytes)
        /// </summary>
        /// <param name="bPeek_"></param>
        /// <returns></returns>
        public double GetDouble(bool bPeek_ = false)
        {
            lock (this)
            {
                int nStartIndex;
                byte[] byBuffer = this.GetBuffer(bPeek_, 8, out nStartIndex, false);
                return BitConverter.ToDouble(byBuffer, nStartIndex);
            }
        }

        /// <summary>
        /// float(4bytes)
        /// </summary>
        /// <param name="bPeek_"></param>
        /// <returns></returns>
        public float GetSingle(bool bPeek_ = false)
        {
            lock (this)
            {
                int nStartIndex;
                byte[] byBuffer = this.GetBuffer(bPeek_, 4, out nStartIndex, false);
                return BitConverter.ToSingle(byBuffer, nStartIndex);
            }
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="nBytes_"></param>
        /// <param name="encoding_">编码方式</param>
        /// <param name="bPeek_"></param>
        /// <returns></returns>
        public string GetString(int nBytes_, Encoding encoding_, bool bPeek_ = false)
        {
            if (nBytes_ == 0)
                return string.Empty;

            lock (this)
            {
                int nStartIndex;
                byte[] byBuffer = this.GetBuffer(bPeek_, nBytes_, out nStartIndex, false);
                return encoding_.GetString(byBuffer, nStartIndex, nBytes_);
            }
        }
        #endregion

        private void CheckLength(bool bPeek_, int nBytes_)
        {
            int nLen = bPeek_ ? this.PeekLength : this.Length;
            if (nLen < nBytes_)
            {
                throw new XBinaryDataException(string.Format("XDataBuffer.CheckLength: buffer length({0}) is less than required({1})", nLen, nBytes_), SHErrorCode.NoData);
            }
        }

        private byte[] GetBuffer(bool bPeek_, int nBytes_, out int nStartIndex_, bool bAlwaysCopy_ = false)
        {
            this.CheckLength(bPeek_, nBytes_);

            if (!bAlwaysCopy_)
            { // As CheckLength success, the buffer at least has one item.
                if (this.Buffer[0].GetLength(bPeek_) >= nBytes_)
                {
                    var byFirst = this.Buffer[0];
                    nStartIndex_ = byFirst.GetIndex(bPeek_);
                    byFirst.AddIndex(bPeek_, nBytes_);

                    if (!bPeek_ && byFirst.Length == 0)
                    {
                        this.Buffer.RemoveAt(0);
                    }
                    return byFirst.Buffer;
                }
            }

            // Copy Return-buffer from Buffer
            nStartIndex_ = 0;
            int nHasGet = 0;
            byte[] byGets = new byte[nBytes_];
            for (int i = 0; i < this.Buffer.Count; i++)
            {
                int nLen;
                BytesSegment curBuffer = this.Buffer[i];
                if (curBuffer.GetLength(bPeek_) <= (nBytes_ - nHasGet))
                {
                    nLen = curBuffer.GetLength(bPeek_);
                }
                else
                {
                    nLen = nBytes_ - nHasGet;
                }
                Array.Copy(curBuffer.Buffer, curBuffer.GetIndex(bPeek_), byGets, nHasGet, nLen);
                curBuffer.AddIndex(bPeek_, nLen);
                nHasGet += nLen;

                if (nHasGet == nBytes_)
                {
                    if (!bPeek_)
                        this.Buffer.RemoveAll(x => x.Length == 0);
                    return byGets;
                }
            }

            return byGets;
        }

        // Nested Types
        private class BytesSegment
        {
            // Methods
            internal void AddIndex(bool bPeek_, int index)
            {
                if (bPeek_)
                {
                    this.PeekStart += index;
                }
                else
                {
                    this.StartIndex += index;
                }
            }

            internal int GetIndex(bool bPeek_)
            {
                if (!bPeek_)
                {
                    return this.StartIndex;
                }
                return this.PeekStart;
            }

            internal int GetLength(bool bPeek_)
            {
                if (!bPeek_)
                {
                    return (this.EndIndex - this.StartIndex);
                }

                return (this.EndIndex - this.PeekStart);
            }

            //internal void SetLength(bool bPeek_, int nLen_)
            //{
            //    if (bPeek_)
            //    {
            //        this.PeekStart = this.EndIndex - nLen_;
            //    }
            //    else
            //    {
            //        this.StartIndex = this.EndIndex - nLen_;
            //    }
            //}

            // Properties
            public byte[] Buffer { get; set; }
            public int StartIndex { get; set; }
            public int PeekStart { get; set; }
            public int EndIndex { get; set; }

            public int Length
            {
                get
                {
                    return (this.EndIndex - this.StartIndex);
                }
            }

            public int PeekLength
            {
                get
                {
                    return (this.EndIndex - this.PeekStart);
                }
            }
        } //BytesSegment

    } // XDataBuffer
} // namespace
