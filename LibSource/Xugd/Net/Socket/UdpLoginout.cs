using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.Net
{
    internal static class UdpLoginout
    {
        internal const int LoginoutDataLen = 2;
        internal static readonly byte[] LoginRequest = { 0xFF, 0x00 };
        internal static readonly byte[] LoginResponse = { 0xFF, 0x00 };
        internal static readonly byte[] LogoutRequest = { 0x00, 0xFF };

        internal static bool IsLoginRequest(byte[] byBuffer_, int nOffset_, int nCount_)
        {
            if (nCount_ != LoginoutDataLen)
                return false;

            return XCompare.AreEqual(byBuffer_, nOffset_, LoginRequest, 0, LoginoutDataLen);
        }

        internal static bool IsLoginResponse(byte[] byBuffer_, int nOffset_, int nCount_)
        {
            if (nCount_ != LoginoutDataLen)
                return false;

            return XCompare.AreEqual(byBuffer_, nOffset_, LoginResponse, 0, LoginoutDataLen);
        }

        internal static bool IsLogoutRequest(byte[] byBuffer_, int nOffset_, int nCount_)
        {
            if (nCount_ != LoginoutDataLen)
                return false;

            return XCompare.AreEqual(byBuffer_, nOffset_, LogoutRequest, 0, LoginoutDataLen);
        }
    }
}
