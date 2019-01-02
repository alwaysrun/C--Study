using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHCre.Xugd.Net
{
    //internal enum IMUserType
    //{
    //    Invalid = 0,
    //    Client,
    //    Sync,
    //}

    internal class IMUserLoginRequest
    {
        public bool QueryMaster {get;set;}
    }

    internal class IMServerMasterData
    {
        public bool IsMaster { get; set; }
        public string SrvID {get; set;}
        public DateTime StartTime { get; set; }
    }
}
