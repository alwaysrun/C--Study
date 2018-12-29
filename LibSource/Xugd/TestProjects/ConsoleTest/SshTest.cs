using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHCre.Xugd.Ssh;

namespace ConsoleTest
{
    partial class Program
    {
        static void TestUpload()
        {
            string defIp = "192.168.88.189";
            string strDestPath = @"/home/fsdir/recordings/voices/";
            Console.Write("Input the Host(def-IP: {0}): ", defIp);
            string strHost = Console.ReadLine();
            if (string.IsNullOrEmpty(strHost))
                strHost = defIp;
            var sFtp = new Xsftp(strHost, "xugd", "xgdxgd");
            sFtp.OnFileTransfer += new Action<string, string, Xsftp.TransferMode, int, int>(sFtp_OnFileTransfer);
            Console.WriteLine("Connecting {0} ...", strHost);
            sFtp.Connect();

            sFtp.SetBaseDir(strDestPath);
            while (true)
            {
                try
                {
                    Console.Write("Input source file(Full Path): ");
                    string strSrc = Console.ReadLine();
                    if (string.IsNullOrEmpty(strSrc)) break;
                    Console.Write("Input dest path(Default {0}): ", strDestPath);
                    string strDest = Console.ReadLine();
                    sFtp.Upload(strSrc, strDest);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            sFtp.Close();
        }

        static void sFtp_OnFileTransfer(string arg1, string arg2, Xsftp.TransferMode euMode, int arg4, int arg5)
        {
            switch(euMode)
            {
                case Xsftp.TransferMode.Start:
                    Console.WriteLine("Transfer Start: {0}->{1}", arg1, arg2);
                    break;
                case Xsftp.TransferMode.Progress:
                    Console.Write(@" {0}/{1};", arg4, arg5);
                    break;
                case Xsftp.TransferMode.End:
                    Console.WriteLine("Transfer End");
                    break;
            }
        }
    }
}
