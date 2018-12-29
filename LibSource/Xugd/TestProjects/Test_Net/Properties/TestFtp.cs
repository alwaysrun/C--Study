using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;
using SHCre.Xugd.Net;

namespace Test_Net
{
    /// <summary>
    /// UnitTest1 的摘要说明
    /// </summary>
    [TestClass]
    public class TestFtp
    {
        public TestFtp()
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

        private bool FileProcess(FtpClient.CallbackStatus euStatus_, int nCur_, int nTotal_)
        {
            if (nCur_ == 100)
                return false;
            return true;
        }

        private void TestFolder(FtpClient client)
        {
            client.SetCurDir(@"Tnone\Tnone");
            client.SetCurDir(@"XNone\None");
            client.SetCurDir("Ynon");
            client.SetCurDir(@"\Tnone\Tnone");
            client.SetCurDir(@"\Znone\Xnon\Ynon");
            client.SetCurDir(@"\Znone");
            client.DeleteFolder(@"Xnon\Ynon", false);
            client.DeleteFolder(@"\Znone\Xnon", true);
            client.DeleteFolder(@"\Znone", false);
            client.SetCurDir(@"\Xnon");
            client.DeleteFolder(@"/Tnone", true);

            FtpClient.DetailList lstAll = client.ListDetail("");
            FtpClient.DetailList lstFile = lstAll.GetFiles();

            string strSrc = "FtpTest.cs";
            string strTarget = "Ftp.cs";
            client.Upload(@"c:\" + strSrc, null, true);
            if (client.IsFileExists(strTarget))
                client.DeleteFile(strTarget);
            client.RenameFile(strSrc, strTarget);
            try 
            {
                client.CreateFolder(strTarget);
            }
            catch(IOException)
            {
                throw;
            }
            //client.DeleteFolder(@"\Xnon", true);
        }

        [TestMethod]
        public void TestUri()
        {
            FtpClient client = new FtpClient("192.168.1.114", 20, "cre", "123456");
            try 
            {
//                client.CreateFolder(@"Tone\dbgview.exe\test");
//                FtpClient.DetailList lstDetail = client.ListDetail("");
//                 List<string> strFolder = client.ListDir("Znone\\");
//                 List<string> strFiles = client.ListDir("FtpTest.cs\\");
//                 strFiles = client.ListDir("test");
//                 strFiles = client.ListDir("dbgview.exe");
//                 strFiles = client.ListDir("none");

//                 client.SetCallback(new FtpClient.CallbackFun(FileProcess));
//                 //client.Download("dbgview.exe", @"c:\down.exe", true);
//                 client.Upload(@"c:\FtpTest.cs", null, true);

                TestFolder(client);


            }
            catch(Exception)
            {
                throw;
            }

        }
    }
}
