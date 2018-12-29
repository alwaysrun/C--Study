using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SHCre.Xugd.Office;

namespace ConsoleTest
{
    class ToTiff
    {
        public static void TestPrint()
        {
            Console.WriteLine(@"Print test.docx to Tiff(Under D:\XTest)");
            Console.WriteLine("Print test.docx");
            Print2Tiff.PrintWord(@"D:\XTest\test.docx", @"D:\XTest\test.tif");
            Console.WriteLine("Press key Print tx.xlsx");
            Console.ReadKey();
            Print2Tiff.PrintExcel(@"D:\XTest\tx.xlsx", @"D:\XTest\tx.tif");
            Console.WriteLine("Press key Print CTI.pptx");
            Console.ReadKey();
            Print2Tiff.PrintPowerpoint(@"D:\XTest\CTI.pptx", @"D:\XTest\cti.tif");
        }

        ///// <summary>
        ///// 打印Word文档
        ///// </summary>
        ///// <param name="wordfile">完整本地路径，包括文件名</param>
        ///// <param name="outputFile">输出Tif文件的完整路径</param>
        //public static void PrintWord(string wordfile, string outputFile)
        //{

        //    MsWord.ApplicationClass wApp = new MsWord.ApplicationClass();
        //    //wApp.Visible = false;
        //    //wApp.DisplayAlerts = MsWord.WdAlertLevel.wdAlertsNone;

        //    object oFilePath = wordfile;
        //    object oMissing = System.Reflection.Missing.Value;
        //    object oTrue = true;
        //    object oFalse = false;
        //    MsWord._Document wDoc = wApp.Documents.Open(ref oFilePath,
        //        ref oMissing, ref oTrue, ref oFalse);
        //    //MsWord.Document wDoc = wApp.Documents.Open(ref oFilePath);
        //    //String currentPrinterPath = word.ActivePrinter;
        //    //word.ActivePrinter = printerPath;
        //    oMissing = System.Reflection.Missing.Value;
        //    object Background = oMissing;
        //    object Range = MsWord.WdPrintOutRange.wdPrintAllDocument;
        //    object Copies = 1;
        //    object PageType = MsWord.WdPrintOutPages.wdPrintAllPages;
        //    object PrintToFile = true;
        //    object Collate = false;
        //    object ActivePrinterMacGX = oMissing;
        //    object ManualDuplexPrint = oMissing;
        //    object PrintZoomColumn = 1;
        //    object PrintZoomRow = 1;
        //    object printFile = outputFile;

        //    //wDoc.PageSetup.PaperSize = MsWord.WdPaperSize.wdPaperA4;

        //    wDoc.PrintOut(
        //        ref Background, //Background 此处为true,表示后台打印
        //        ref oMissing,
        //        ref Range, //Range 页面范围
        //        ref printFile,
        //        ref oMissing, //当 Range 设置为 wdPrintFromTo 时的起始页码
        //        ref oMissing,  //当 Range 设置为 wdPrintFromTo 时的结束页码
        //        ref oMissing,
        //        ref Copies,  //要打印的份数
        //        ref oMissing,
        //        ref PageType,
        //        ref PrintToFile,
        //        ref Collate,
        //        ref oMissing,
        //        ref oMissing,
        //        ref PrintZoomColumn,
        //        ref PrintZoomRow,
        //        ref oMissing,
        //        ref oMissing
        //        );

        //    //word.ActivePrinter = currentPrinterPath;
        //    object dummy = null;
        //    object dummy2 = (object)false;
        //    //DeleteWordFileContent();
        //    //wDoc.Close(ref dummy2, ref dummy, ref dummy);
        //    Thread.Sleep(5 * 1000);
        //    wApp.Quit(ref dummy2, ref dummy, ref dummy);

        //    //Type wordType = word.GetType();
        //    ////打开WORD文档
        //    //MsWord.Documents docs = word.Documents;
        //    //Type docsType = docs.GetType();
        //    //object objDocName = wordfile;
        //    //MsWord.Document doc = (MsWord.Document)docsType.InvokeMember("Open", System.Reflection.BindingFlags.InvokeMethod, null, docs, new Object[] { objDocName, true, true });
        //    ////Word.
        //    ////打印输出到指定文件
        //    ////可以使用 doc.PrintOut();方法,次方法调用中的参数设置较繁琐,建议使用 Type.InvokeMember 来调用时可以不用将PrintOut的参数设置全,只设置4个主要参数
        //    //Type docType = doc.GetType();
        //    //object printFileName = outputFile;
        //    //Object oMissing = System.Reflection.Missing.Value;
        //    //object bPrint2File = true;
        //    ////doc.PrintOut(ref oMissing, ref oMissing, ref oMissing,
        //    ////    ref printFileName,
        //    ////    ref oMissing, ref oMissing,
        //    ////    ref oMissing, ref oMissing,
        //    ////    ref oMissing, ref oMissing,
        //    ////    ref bPrint2File);
        //    //docType.InvokeMember("PrintOut", System.Reflection.BindingFlags.InvokeMethod, null, doc, new object[] { false, false, MsWord.WdPrintOutRange.wdPrintAllDocument, printFileName });
        //    ////退出WORD
        //    //wordType.InvokeMember("Quit", System.Reflection.BindingFlags.InvokeMethod, null, word, null);
        //}

        ///// <summary>
        ///// Execl直接打印
        ///// </summary>
        ///// <param name="execlfile">完整本地路径，包括文件名</param>
        //public static void PrintExcel(string execlfile, string ExceloutputFile)
        //{
        //    MsExcel.ApplicationClass eApp = new MsExcel.ApplicationClass();
        //    //eApp.Visible = false;
        //    object oTrue = true;
        //    Object oMissing = System.Reflection.Missing.Value;
        //    MsExcel._Workbook eBook = eApp.Workbooks.Open(execlfile, oMissing, oTrue);

        //    object printFileName = ExceloutputFile;
        //    eBook.PrintOut(oMissing, oMissing, oMissing, oMissing, oMissing, true, oMissing, printFileName);

        //    Thread.Sleep(5 * 1000);
        //    eBook.Close(false, oMissing, oMissing);
        //    eApp.Quit();

        //    //MsExcel.ApplicationClass eapp = new MsExcel.ApplicationClass();
        //    //Type eType = eapp.GetType();
        //    //MsExcel.Workbooks Ewb = eapp.Workbooks;
        //    //Type elType = Ewb.GetType();
        //    //object objelName = execlfile;
        //    //MsExcel.Workbook ebook = (MsExcel.Workbook)elType.InvokeMember("Open", System.Reflection.BindingFlags.InvokeMethod, null, Ewb, new Object[] { objelName, true, true });
        //    //object printFileName = ExceloutputFile;
        //    //Object oMissing = System.Reflection.Missing.Value;
        //    //ebook.PrintOut(oMissing, oMissing, oMissing, oMissing, oMissing, true, oMissing, printFileName);
        //    ////Thread.Sleep(1000 * 10);
        //    //eType.InvokeMember("Quit", System.Reflection.BindingFlags.InvokeMethod, null, eapp, null);
        //} 

        //public static void PrintPowerpoint(string strSourceFile_, string strDestFile_)
        //{
        //    MsPpt.ApplicationClass pApp = new MsPpt.ApplicationClass();
        //    //pApp.Visible = MsCore.MsoTriState.msoFalse;
        //    MsPpt._Presentation pPresent = pApp.Presentations.Open(strSourceFile_,
        //        MsCore.MsoTriState.msoTrue, MsCore.MsoTriState.msoFalse,
        //        MsCore.MsoTriState.msoFalse);

        //    Thread.Sleep(5 * 1000);
        //    pPresent.PrintOut(-1, -1, strDestFile_);
        //    pPresent.Close();

        //    pApp.Quit();
        //}
    }
}
