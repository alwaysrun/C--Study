using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SHCre.Xugd.CFile;
using SHCre.Xugd.Common;
using System.Text.RegularExpressions;
using SHCre.Xugd.Net;
using SHCre.Xugd.Extension;
using System.Reflection;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;

namespace ConsoleTest
{
    partial class Program
    {
        enum TestCheck
        {
            Invalid = 0,
            One,
            Two,
        };

        struct TestPtr
        {
            public IntPtr pTest;
        }

        static void Main(string[] args)
        {
            //var proThis = new Program();
            //EventFunTest();

            //TestUniqueList();
            ToTiff.TestPrint();

            //TestPtr tPtr = new TestPtr();
            //if (tPtr.pTest == IntPtr.Zero)
            //{
            //    Console.WriteLine("Is zero");
            //}

            //var byAddr = IPAddress.Parse("1.2.3.4").GetAddressBytes();
            //var nIp = BitConverter.ToUInt32(byAddr, 0);

            //Dictionary<string, string> dicTest = new Dictionary<string, string>();
            //dicTest["sdf"] = "now";

            //CheckDicType(dicTest);

            //TestCheck tChk = TestCheck.One;
            //CheckEnumType(tChk);

            //XLogSimple logSim = new XLogSimple("log\\test.log");
            //logSim.VerInfo();

            //Console.WriteLine(GetStack());

            //Fun2Predicate(z => z > 2);

            //TestUpload();

            //Console.WriteLine("To start TTSL");
            //TtsTest();
            //Tts2WavFile();
            //Speak2WavFile();
            //Thread.Sleep(100);
            //Wav2Text();

            //EventActionTest();
            //LockTest lkTest = new LockTest();
            //lkTest.LockFirst();
            //TestCovariant();

            //Dictionary<string, string> dictTest = new Dictionary<string, string>();
            //string strValue;
            //dictTest.TryGetValue("None", out strValue);
            //var ips = XNetAddress.GetLocalIPs();

            //DateTime? dtNone = null;
            //DateTime? dtNow = DateTime.Now;
            //Console.Write("{0} - {1}", dtNone, dtNow);
            //proThis.AssemblyTest();

            //ManalEventTest();
            //TaskTest();

            Console.WriteLine("Press key to exit");
            Console.ReadKey();
        }

        class UniqueVoice : IUniqueListItemBase
        {
            public string Number { get; set; }
            public DateTime DtAdd { get; set; }

            public UniqueVoice(string strNum_)
            {
                Number = strNum_;
                DtAdd = DateTime.Now;
            }

            public string GetUniqueKey()
            {
                return Number;
            }
        };
        static void TestUniqueList()
        {
            List<UniqueVoice> lstVoice = new List<UniqueVoice>();
            lstVoice.Add(new UniqueVoice("123"));
            lstVoice.Add(new UniqueVoice("234"));
            lstVoice.Add(new UniqueVoice("345"));
            lstVoice.Add(new UniqueVoice("123"));
            lstVoice.Add(new UniqueVoice("345"));
            lstVoice.Add(new UniqueVoice("dasfd"));
            lstVoice.Add(new UniqueVoice("234"));

            var lstRes = lstVoice.WhereUnique();
        }

        static void Fun2Predicate(Func<int, bool> funCmp_)
        {
            List<int> lstAll = new List<int>();
            lstAll.Add(1);
            lstAll.Add(4);
            lstAll.Add(3);
            lstAll.Add(2);
            lstAll.Sort((x, y) => x.CompareTo(y));

            var preCmp = new Predicate<int>(funCmp_);
            int nIndex = lstAll.FindIndex(preCmp);
            Console.WriteLine("Find index: {0}", nIndex);
        }

        static void CheckEnumType(object oEnum_)
        {
            if(oEnum_ is TestCheck)
            {
                var euType = (TestCheck)oEnum_;
                Console.WriteLine(euType);
            }
        }

        static void CheckDicType(object obj_)
        {
            //if(obj_ is Dictionary<string, string>) // this always false
            {
                var dicString = obj_ as Dictionary<string, string>;
                var dicInt = obj_ as Dictionary<string, int>;
                if(dicInt == null && dicString == null)
                {

                }
            }
        }

        private static string GetStack()
        {
            StackTrace st = new StackTrace(1, true);
            var sf = st.GetFrame(0);
            var t = sf.GetType();
            return string.Format("{0}-{1}[{2}]: {3}", sf.GetFileName(),
                sf.GetMethod().Name, sf.GetFileLineNumber(), t);
        }

        private static void TaskTest()
        {
            Console.WriteLine("To Start Task");
            CancellationTokenSource ctcSum = new CancellationTokenSource();
            Task<int> tSum = Task<int>.Factory.StartNew(() =>
                {
                    int nSum = 0;
                    for(int i=1 ; i<100 ; ++i)
                    {
                        Console.WriteLine("Sum: {0}", i);
                        nSum += i;
                        //if (ctcSum.Token.IsCancellationRequested)
                        //    break;
                        if (ctcSum.Token.WaitHandle.WaitOne(100))
                            break;

                        if (i > 50)
                            ctcSum.Cancel();
                    }
                    return nSum;
                }, ctcSum.Token);

            Task<int> tAll = Task<int>.Factory.StartNew(() =>
            {
                int nSum = 0;
                for (int i = 1; i < 100; ++i)
                {
                    Console.WriteLine("All: {0}", i);
                    nSum += i;
                    //if (ctcSum.Token.IsCancellationRequested)
                    //    break;
                    if (ctcSum.Token.WaitHandle.WaitOne(100))
                        break;

                    if (i > 50)
                        ctcSum.Cancel();
                }
                return nSum;
            }, ctcSum.Token);

            Thread.Sleep(1000 );
            ctcSum.Cancel();
            Console.WriteLine("Task Result: {0}; {1}", tSum.Result, tAll.Result);        
        }

        private static void ManalEventTest()
        {
            ManualResetEvent evt = new ManualResetEvent(false);
            XThread.StartThread(() =>
            {
                try
                {
                    Thread.Sleep(1000);
                    evt.Set();
                }
                catch (Exception )
                {
                }
            });
            evt.WaitOne(100);
            evt.Close();
        }

        private void AssemblyTest()
        {
            //Assembly ass = Assembly.GetCallingAssembly();
            var notEx = new XNotFoundException();
            Assembly ass = Assembly.GetAssembly(notEx.GetType());
            var ver = ass.GetName();
            var verAttr = ass.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
            var fileVer = verAttr[0].ToString();

            XLogConfig logCon = new XLogConfig();
            logCon.InitDayLog("AssTest");
            XLogSimple log = new XLogSimple(logCon);
            log.VerInfo(this.GetType());
            log.VerInfo(log.GetType());
            log.VerInfo(notEx.GetType());
        }

        class LockTest
        {
            object _lkerTest = new object();

            public void LockFirst()
            {
                lock (_lkerTest)
                {
                    LockSecond();
                    Console.WriteLine("First");
                }
            }

            public void LockSecond()
            {
                lock (_lkerTest)
                {
                    Console.WriteLine("Second");
                }
            }
        }

        #region "Event Test"
        class EventTest
        {
            public event Action<string> ActTest;
            public event Func<int> FunTest;

            public bool IsActSet()
            {
                return ActTest!=null && ActTest.GetInvocationList().Length > 0;
            }

            public void TestFun()
            {
                int nAll = 0;
                var lstFuns = FunTest.GetInvocationList();
                foreach(var fun in lstFuns)
                {
                    var funT = fun as Func<int>;
                    nAll += funT();
                }
                Console.WriteLine("Fun return: {0}", nAll);
            }

            public void SelfEvent()
            {
                ActTest += new Action<string>(EventTest_ActTest);
            }

            public void TestAct()
            {
                IsActValid(ActTest);
            }

            public void ClearAct()
            {
                ActTest = null;
            }

            public bool IsActValid(Action<string> act_)
            {
                if(act_ != null)
                {
                    act_("ActValid test");
                    return true;
                }

                return false;
            }

            void EventTest_ActTest(string obj)
            {
                Console.WriteLine("Self " + obj);
            }

            public void InvokeTest(string strInfo)
            {
                if (ActTest != null)
                    ActTest(strInfo);
            }
        }
        static void EventFunTest()
        {
            var cTest = new EventTest();
            var isSet = cTest.IsActSet();

            cTest.FunTest += () => { Console.WriteLine("FunTest 1"); return 1; };
            cTest.FunTest += () => { Console.WriteLine("FunTest 2"); return 2; };
            cTest.FunTest += () => { Console.WriteLine("FunTest 3"); return 3; };

            cTest.TestFun();
        }

        private static void EventActionTest()
        {
            EventTest cTest = null;
            //try 
            //{
            //    cTest.ActTest("no");
            //}
            //catch(Exception ex)
            //{
            //    int i = 0;
            //}
            cTest = new EventTest();
            for (int i = 1; i < 8; ++i)
            {
                var tAction = Task.Factory.StartNew((z) =>
                    {
                        int nIndex = (int)z;
                        StartActionTest(cTest, nIndex);
                    }, i);
            }
             
            cTest.InvokeTest("Call[0]");
            //cTest.ActTest += cTest_NewTest;
            for (int i = 1; i < 8; ++i)
            {
                Thread.Sleep(1000);
                cTest.InvokeTest(string.Format("Call[{0}]", i));
                GC.Collect();
            }

            var isSet = cTest.IsActSet();

            Thread.Sleep(5000);
            cTest.InvokeTest("Last");
        }

        private static void StartActionTest(EventTest cTest, int i)
        {
            cTest.ActTest += (z) =>
            {
                Console.WriteLine("{0}: {1}", i, z);
            };

            Thread.Sleep(i * 1000 + 5);
            Console.WriteLine("{0} quit", i);
        }

        static void cTest_NewTest(string obj)
        {
            Console.WriteLine("New-" + obj);
        }

        static void cTest_ActTest(string strInfo_, string obj)
        {
            Console.WriteLine(strInfo_ + obj);
        }
        #endregion


    } // class
}
