using SHCre.Xugd.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Test_Net
{
    
    
    /// <summary>
    ///这是 XFlagTest 的测试类，旨在
    ///包含所有 XFlagTest 单元测试
    ///</summary>
    [TestClass()]
    public class TestCommon
    {


        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///GetSubDigits 的测试
        ///</summary>
        [TestMethod()]
        public void GetSubDigitsTest()
        {
            uint nNumber_ = 0xABCDEF98; //unchecked((uint)-1); // TODO: 初始化为适当的值
            uint nTest = XFlag.GetSubDigits(nNumber_, 2, 10);
            nTest = XFlag.GetSubDigits(nNumber_, 2, -2);
            Assert.AreEqual(0x98U, XFlag.GetSubDigits(nNumber_, 2));
            Assert.AreEqual(0xfU, XFlag.GetSubDigits(nNumber_, 1, 2));
        }

        /// <summary>
        /// 拼音的声调
        /// </summary>
        [Flags]
        public enum Tones:byte
        {
            Invalid = 0,
            /// <summary>
            /// 一声平，阴平
            /// </summary>
            Yinping = 1,
            /// <summary>
            /// 二声仰，阳平
            /// </summary>
            Yangping = 2,
            /// <summary>
            /// 三声弯，上声
            /// </summary>
            Shangsheng = 4,
            /// <summary>
            /// 四声降，去声
            /// </summary>
            Qusheng = 8
        };

        [TestMethod]
        public void TestEnum()
        {
            Tones tn = Tones.Qusheng;

            string strNum = tn.ToString("d");
            string str = tn.ToString();
            Assert.AreEqual(strNum, XConvert.Enum2Digits(tn));
            Assert.AreEqual(str, XConvert.Enum2Name(tn));
        }

        [TestMethod]
        public void TestEnumCompare()
        {
            Tones tn = Tones.Qusheng;
            for(int i=0 ; i<10 ; i++)
            {
                var ret = Convert.ToInt32(tn) == i;
            }
        }

        [TestMethod]
        public void TestFlag()
        {
            Tones tOne = Tones.Yinping;
            Tones tYang = Tones.Yinping | Tones.Yangping;
            Assert.IsTrue(XFlag.Check(tYang, Tones.Yangping));

            XFlag.Set(ref tOne, Tones.Yangping);
            Assert.AreEqual(tOne, tYang);

            XFlag.Clear(ref tOne, Tones.Yangping);
            Assert.AreEqual(tOne, Tones.Yinping);

            bool bDefine = Enum.IsDefined(typeof(Tones), "3");
            Tones tGetNum = (Tones)Enum.Parse(typeof(Tones), "3");
            Tones tGetName = (Tones)Enum.Parse(typeof(Tones), "Yinping, Yangping");
        }

        [TestMethod]
        public void TestVersion()
        {
            XVersion vFirst = new XVersion(0x010002);
            XVersion vSecond = XVersion.Parse("1.2");
            XVersion vThird = new XVersion(1, 2);

            Assert.AreEqual(vFirst, vSecond);
            Assert.AreEqual(vSecond, vThird);

            Assert.IsTrue(vFirst == vSecond);
            XVersion vFour = new XVersion(2, 0);
            Assert.IsTrue(vThird != vFour);
        }

        [TestMethod]
        public void TestTimeSpan()
        {
            string strTime = XTime.GetTimeSpanShowString(new TimeSpan(1, 2, 3));
            strTime = XTime.GetTimeSpanShowString(new TimeSpan(1, 2, 3, 4));
            strTime = XTime.GetTimeSpanShowString(new TimeSpan(0, 0, 23));
        }

        [TestMethod]
        public void TestMemberInfo()
        {
            Type tEx = typeof(SHException);
            var mems = tEx.GetMembers();
        }

        [TestMethod]
        public void TestSequence()
        {
            XSafeSequence seqData = new XSafeSequence();
            for (int i = 1; i < 1000; ++i)
            {
                Assert.AreEqual(i, seqData.GetNext());
            }
            seqData.Reset(int.MaxValue - 2);
            for (int i = 1; i < 1000; ++i)
            {
                var next = seqData.GetNext();
            }

            XSafeSequence rangeData = new XSafeSequence(-10, 10);
            for (int i = 0; i < 1000; i++)
            {
                int nData = rangeData.GetNext();
                Assert.IsTrue(nData >= -10 && nData <= 10);
            }
        }

        [TestMethod]
        public void TestNumber()
        {
            var nNum = 123.456;
            string strOut = string.Format("{0:N}", nNum);
            strOut = XFormat.DecimalWithPrecision(nNum);
            strOut = XFormat.DecimalWithExactPrecision(nNum);
            strOut = XFormat.IntegerWithLength(nNum);
            strOut = XFormat.IntegerWithLength(nNum, 6);

            nNum = 12.1001;
            strOut = string.Format("{0:.##}", nNum);
            strOut = string.Format("{0:####.##}", nNum);

            strOut = XFormat.DecimalWithPrecision(nNum);
            strOut = XFormat.DecimalWithExactPrecision(nNum);
            strOut = XFormat.IntegerWithLength(nNum);
            strOut = XFormat.IntegerWithLength(nNum, 6);
        }

        public class TestDbInfo
        {
            [XmlAttribute]
            public int NO { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public DateTime? Birthday { get; set; }
            public bool? IsLeave { get; set; }
        }

        public class TestXML
        {
            public List<TestDbInfo> Openfire{get;set;}

            public TestXML()
            {
                Openfire = new List<TestDbInfo>();
            }
        }

        [TestMethod]
        public void TestXmlConvert()
        {
            int nRand = XRandom.GetInt(1000);           

            TestDbInfo dbInfo = new TestDbInfo()
            {
                NO = nRand,
                Email = "Email@" + nRand,
                Birthday = DateTime.Now,
                IsLeave = false,
            };
            TestXML tXml = new TestXML();
            tXml.Openfire.Add(dbInfo);
            tXml.Openfire.Add(dbInfo);

            string strXml = XConvert.Type2Xml(tXml);

            var get = XConvert.Xml2Type<TestXML>(strXml);
        }

        class ReflexTest
        {
            public ReflexTest(){}
            public ReflexTest(int none)
            {
                Info = string.Empty;
                Number = none;
                Now = DateTime.Now;
            }
            string Info;
            int Number;
            DateTime Now;
        }

        [TestMethod]
        public void TestReflex()
        {
            ReflexTest cTest = new ReflexTest(0);
            string strName = XReflex.GetTypeName(cTest, false);
            strName = XReflex.GetTypeName(cTest, true);
            object objReflex = XReflex.BuildInstance(strName, Assembly.GetAssembly(this.GetType()));

            int[] aryInt = null;
            strName = XReflex.GetTypeName(aryInt, false);
            //objReflex = XReflex.BuildInstance(strName);
            objReflex = XReflex.BuildArray(XReflex.GetTypeName(cTest, true), 10, Assembly.GetAssembly(cTest.GetType()));

            List<int> lstNum = null;
            strName = XReflex.GetTypeName(lstNum, false);
            strName = XReflex.GetTypeName(lstNum, true);
            objReflex = XReflex.BuildInstance(strName);
        }

        [TestMethod]
        public void TestExeCmd()
        {
            string strCmd = "calc";
            //var strOut = XProcess.ExecCommand(strCmd);


            //strCmd = "-1start \"\" \"C:\\Users\\Xugd\\Desktop\\新建 文件夹\\新建文本文档.txt\"";
            var strCmd2 = "\"C:\\Users\\Xugd\\Desktop\\新建 文件夹\\新建.txt\"";
            var strOut = XProcess.ExecCommand(15, strCmd2, strCmd);
        }

        public enum EnumTest
        {
            Invalid = 0,
            [XEnumMark(BoundType=XBoundaryType.Lower, Remark="Start")]
            StartIndex,
            None,
            [XEnumMark(BoundType = XBoundaryType.Upper, Remark = "End")]
            EndIndex
        }

        [TestMethod]
        public void TestXAttr()
        {
            var dictAll = XAttr.GetEnumMarkField(typeof(EnumTest));
        }

        [TestMethod]
        public void TestSafeResetSeq()
        {
            XSafeSequence seq = new XSafeSequence();
            XSafeResetSeq retSeq = new XSafeResetSeq(1, 99999);
            int nOldMinute = DateTime.Now.Minute;
            int nNew = nOldMinute;
            retSeq.TryReset(XRandom.GetInt(100, 200), nOldMinute);
            Console.WriteLine(DateTime.Now);
            while (true)
            {
                nNew = DateTime.Now.Minute;
                if (nNew != nOldMinute)
                {
                    nOldMinute = nNew;
                    Console.Write("{0}: ", DateTime.Now);
                }

                Console.WriteLine(retSeq.GetNext(DateTime.Now.Minute));
                Thread.Sleep(1000);
            }
        }
    }
}
