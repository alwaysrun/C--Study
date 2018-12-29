using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SHCre.Xugd.CreAPI;
using SHCre.Xugd.Common;

namespace Test_CreAPI
{
    /// <summary>
    /// UnitTest1 的摘要说明
    /// </summary>
    [TestClass]
    public class TestCSymbol
    {
        public TestCSymbol()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region 其他测试属性
        //
        // 您可以在编写测试时使用下列其他属性:
        //
        // 在运行类中的第一个测试之前使用 ClassInitialize 运行代码
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // 在类中的所有测试都已运行之后使用 ClassCleanup 运行代码
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 在运行每个测试之前使用 TestInitialize 运行代码 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 在运行每个测试之后使用 TestCleanup 运行代码
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        string _strSCryptDll = @"F:\Project\SVNFiles\Projects\CMVP32\Interface\Dll\CreSCrypt.dll";

        [TestMethod]
        public void TestCrypt()
        {
            byte[] byKey = SecAlg.Crypto.BuildKey(_strSCryptDll, SecAlg.Crypto.Alg.RSA,  "TestCrypt");

            uint nProvider = SecAlg.AddByPath(_strSCryptDll);
            IntPtr ptrAlg = SecAlg.Crypto.GetAlg(nProvider, SecAlg.Crypto.Alg.RSA);
            SecAlg.Crypto.ConvertKey(ptrAlg, byKey);
            SecAlg.Crypto.ReleaseAlg(ptrAlg);
        }

        [TestMethod]
        public void TestLocalConnection()
        {
            // Mac
            byte[] byMac = CSymbol.GetMacAddr();
            string strMac = XConvert.MacAddrBytes2String(byMac);
            byte[] byMacNew = XConvert.MacAddrString2Bytes(strMac);
            CollectionAssert.AreEqual(byMac, byMacNew);

            // Ip
            byte[] byIpv4 = CSymbol.GetIPv4Addr();
            string strIpv4 = XConvert.IPAddrBytes2String(byIpv4);
            byte[] byIpv4New = XConvert.IPAddrString2Bytes(strIpv4);
            CollectionAssert.AreEqual(byIpv4, byIpv4New);

            // Disk
            string strCpuSN = CSymbol.GetCpuSN();
            string strBios = CSymbol.GetBiosSN();
            string strDiskSN = CSymbol.GetDiskSN();

            byte[] byDriveC = CSymbol.GetDriveGuid('c');
            string strDriveC = XConvert.GuidBytes2String(byDriveC);
            byte[] byDriveCNew = XConvert.GuidString2Bytes(strDriveC);
            CollectionAssert.AreEqual(byDriveC, byDriveCNew);

            byte[] bySysDrive = CSymbol.GetSysDriveGuid();
            CollectionAssert.AreEqual(byDriveC, bySysDrive);

            byte[] byIpv4Gateway = CSymbol.GetIPv4Gateway();
        }
    }
}
