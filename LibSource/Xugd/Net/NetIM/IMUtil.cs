using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHCre.Xugd.Net
{
    partial class XIMConnection
    {
        const char DomainSeparatorInJid = '@';
        private string Jid2Name(string strJid_)
        {
            if (!string.IsNullOrEmpty(strJid_))
            {
                int nIndex = strJid_.IndexOf(DomainSeparatorInJid);
                if (nIndex >= 0)
                    return strJid_.Substring(0, nIndex);
            }

            return strJid_;
        }


        private string Name2Jid(string strName_)
        {
            if (strName_.Contains(DomainSeparatorInJid))
                return strName_;

            return strName_ + DomainSeparatorInJid + Domain;
        }
    } // class
}
