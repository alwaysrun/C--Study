using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.Net
{
    partial class XCommConnection
    {
        const int HeaderLength = 8;
        const byte HeaderFirst = 0xF5;
        
        // -XC-
        readonly byte[] _byCommHeader = { HeaderFirst, 0x58, 0x43, 0xFA };

        byte[] GetLenBytes(int nLen_)
        {
            return BitConverter.GetBytes(nLen_);
        }

        bool IsDataHeader(byte[] byData_, int nOffset_, int nCount_)
        {
            if (nCount_ < HeaderLength)
                return false;

            return XCompare.AreEqual(_byCommHeader, 0, byData_, nOffset_, 4);
        }


        int GetDataLen(byte[] byData_, int nOffset_)
        {
            return BitConverter.ToInt32(byData_, nOffset_ + 4);
        }

        byte[] BuildSendData(byte[] byData_)
        {
            if (!IsDataCarryHeader)
                return byData_;

            // Add Header
            byte[] bySend = new byte[byData_.Length + HeaderLength];

            int nStart = 0;
            Array.Copy(_byCommHeader, 0, bySend, nStart, 4);
            nStart += 4;
            Array.Copy(GetLenBytes(byData_.Length), 0, bySend, nStart, 4);
            nStart += 4;
            Array.Copy(byData_, 0, bySend, nStart, byData_.Length);

            return bySend;
        }
    }
}
