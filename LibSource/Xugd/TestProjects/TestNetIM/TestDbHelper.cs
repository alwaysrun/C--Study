using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using SHCre.Xugd.DbHelper;
using SHCre.Xugd.Common;

namespace TestForm
{    
    public partial class TestDbHelper : Form
    {
        //int _nTestTimes = 0;
        public TestDbHelper()
        {
            InitializeComponent();
        } 

        class DbTest
        {
            public string ID {get;set;}
            public bool MyInt {get;set;}
            public ulong MyNum { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XDapperORM.ConnectOracle orclConn = new XDapperORM.ConnectOracle("192.168.1.202", "ncre", "ncre", "(SERVICE_NAME=ORCL)", XDatabaseType.Oracle);
            //XDapperORM.ConnectOracle orclConn = new XDapperORM.ConnectOracle("", "ncre", "ncre", "pc100");
            //if (++_nTestTimes % 2 == 0)
            //    orclConn.DbCreateFrom = XDatabaseType.Oracle;
            //XDapperORM.ConnectSqlServer orclConn = new XDapperORM.ConnectSqlServer("192.168.1.200", "TestDB", "cre", "123456");
            XDapperORM orclORM = new XDapperORM(orclConn);
            if (orclORM.CanConnect())
            {
                DbTest tDb = new DbTest()
                {
                    ID = XRandom.GetString(10),
                    MyInt = true,
                    MyNum = long.MaxValue,
                };
                orclORM.Insert("TestDB", tDb);

                var lstAll = orclORM.GetMulti<DbTest>("TestDB");
                int nCount = lstAll.Count;

                var dtOrm = orclORM.GetDateTime();
                this.txtTime.Text = dtOrm.ToString();
            }
            else
            {
                this.txtTime.Text = "Can not connect";
            }
        }

        [DllImport("Library\\xugd.fscti.dll", EntryPoint = "FsSetRecordEnable")]
        private static extern void SetRecordEnable(
        [MarshalAs(UnmanagedType.I1)]
            bool bEnable_);

        [DllImport("Library\\xugd.fscti.dll", EntryPoint = "FsLoginDN", CharSet = CharSet.Ansi)]
        private static extern int LoginDN(
            [MarshalAs(UnmanagedType.LPStr)]
            string strPhoneExt_,
            [MarshalAs(UnmanagedType.LPStr)]
            string strAgentId_,
            [MarshalAs(UnmanagedType.LPStr)]
            string strGroup_,
            [MarshalAs(UnmanagedType.I1)] 
            bool bDefBusy_);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)] 
        delegate void DelCallback(
            [MarshalAs(UnmanagedType.I1)] 
            bool isRunning_,
            int nSessionCount_,
            float fIdleCpu_);

        [DllImport("Library\\xugd.fscti.dll", EntryPoint = "FsStartCti")]
        private static extern int StartCti(
            [MarshalAs(UnmanagedType.LPStr)]
            string strAddr_,
            int nPort_,
            [MarshalAs(UnmanagedType.LPStr)]
            string strPsw_,
            [MarshalAs(UnmanagedType.LPWStr)]
            string strLogPath_,
            DelCallback pCallback_);

        [DllImport("Library\\xugd.fscti.dll")]
        private static extern int FsGetPhoneAddr(
            [MarshalAs(UnmanagedType.LPStr)] string strExt_,
            [MarshalAs(UnmanagedType.LPStr)] StringBuilder strAddr_,
            int nSize_);

        string GetPhoneAddr(string strExt_)
        {
            StringBuilder sbAddr = new StringBuilder(30);
            int nResult = FsGetPhoneAddr(strExt_, sbAddr, sbAddr.Capacity);
            return sbAddr.ToString();
        }

        void GetInfo(bool isRunning_, int nSessionCount_, float fIdleCpu_)
        {
            string strInfo = string.Format("{0}: " +
            "Session: {1}" + 
            ", Cpu: {2}", 
            DateTime.Now, nSessionCount_, fIdleCpu_);
            AddItem(strInfo);
        }

        void AddItem(string strInfo_){
            if(this.InvokeRequired){
                this.BeginInvoke(new Action<string>(AddItem), strInfo_);
                return;
            }

            this.lvwInfo.Items.Add(new ListViewItem(strInfo_));
            this.lvwInfo.EnsureVisible(this.lvwInfo.Items.Count-1);
        }

        DelCallback _delCall=null; 
        
        private void btnTest_Click(object sender, EventArgs e)
        {
            if(_delCall == null)
                _delCall = new DelCallback(GetInfo); 

            int nRet = StartCti("192.168.6.204", 8021, "ClueCon", "FsInvoked.log", _delCall);

            string strAddr = GetPhoneAddr("1000");
            strAddr = GetPhoneAddr("1011");

            nRet = LoginDN("1011", "81011", "", false);
            nRet = LoginDN("", "", "", false);
            SetRecordEnable(true);
            AddItem("Started");
        }
    }
}
