using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MsWord = Microsoft.Office.Interop.Word;
using MsExcel = Microsoft.Office.Interop.Excel;
using MsPpt = Microsoft.Office.Interop.PowerPoint;
using MsCore = Microsoft.Office.Core;
using System.Threading;

namespace SHCre.Xugd.Office
{
    /// <summary>
    /// 打印Office文档为Tiff文件：
    /// 需要安装有‘Microsoft Office Document Image Writer’的虚拟打印机，
    /// 并设为默认打印机才行。
    /// </summary>
    public static class Print2Tiff
    {
        /// <summary>
        /// 打印Word文档
        /// </summary>
        /// <param name="strWord_">完整本地路径，包括文件名</param>
        /// <param name="strOutTif_">输出Tif文件的完整路径</param>
        /// <param name="nWaitSecond_">关闭application前等待的时间</param>
        public static void PrintWord(string strWord_, string strOutTif_, int nWaitSecond_=5)
        {
            MsWord.ApplicationClass wApp = new MsWord.ApplicationClass();
            //wApp.Visible = false;
            //wApp.DisplayAlerts = MsWord.WdAlertLevel.wdAlertsNone;

            object oTrue = true;
            object oFalse = false;
            object oMissing = System.Reflection.Missing.Value;
            object oWordFile = strWord_;
            MsWord._Document wDoc = wApp.Documents.Open(ref oWordFile,
                ref oMissing, ref oTrue, ref oFalse);

            oMissing = System.Reflection.Missing.Value;
            object oIsBackground = oMissing;
            object oPrintRange = MsWord.WdPrintOutRange.wdPrintAllDocument;
            object Copies = 1;
            object oPageType = MsWord.WdPrintOutPages.wdPrintAllPages;
            object oIsToFile = true;
            object oCollate = false;
            object oZoomColumn = 1;
            object oZoomRow = 1;
            object oOutFile = strOutTif_;
            //wDoc.PageSetup.PaperSize = MsWord.WdPaperSize.wdPaperA4;

            wDoc.PrintOut(
                ref oIsBackground, //Background 此处为true,表示后台打印
                ref oMissing,
                ref oPrintRange, //Range 页面范围
                ref oOutFile,
                ref oMissing, //当 Range 设置为 wdPrintFromTo 时的起始页码
                ref oMissing,  //当 Range 设置为 wdPrintFromTo 时的结束页码
                ref oMissing,
                ref Copies,  //要打印的份数
                ref oMissing,
                ref oPageType,
                ref oIsToFile,
                ref oCollate,
                ref oMissing,
                ref oMissing,
                ref oZoomColumn,
                ref oZoomRow,
                ref oMissing,
                ref oMissing
                );

            Thread.Sleep(nWaitSecond_ * 1000);
            oMissing = System.Reflection.Missing.Value;
            object oSaveChange = (object)false;
            wApp.Quit(ref oSaveChange, ref oMissing, ref oMissing);
        }

        /// <summary>
        /// Excel直接打印
        /// </summary>
        /// <param name="strExcel">完整本地路径，包括文件名</param>
        /// <param name="strOutTif_">输出Tif文件的完整路径</param>
        /// <param name="nWaitSecond_">关闭application前等待的时间</param>
        public static void PrintExcel(string strExcel, string strOutTif_, int nWaitSecond_ = 5)
        {
            MsExcel.ApplicationClass eApp = new MsExcel.ApplicationClass();
            //eApp.Visible = false;
            object oTrue = true;
            Object oMissing = System.Reflection.Missing.Value;
            MsExcel._Workbook eBook = eApp.Workbooks.Open(strExcel, oMissing, oTrue);

            object printFileName = strOutTif_;
            eBook.PrintOut(oMissing, oMissing, oMissing, oMissing, oMissing, true, oMissing, printFileName);

            Thread.Sleep(nWaitSecond_ * 1000);
            eBook.Close(false, oMissing, oMissing);
            eApp.Quit();
        }

        /// <summary>
        /// Powerpoint直接打印
        /// </summary>
        /// <param name="strPpt">完整本地路径，包括文件名</param>
        /// <param name="strOutTif_">输出Tif文件的完整路径</param>
        /// <param name="nWaitSecond_">关闭application前等待的时间</param>
        public static void PrintPowerpoint(string strPpt, string strOutTif_, int nWaitSecond_ = 5)
        {
            MsPpt.ApplicationClass pApp = new MsPpt.ApplicationClass();
            //pApp.Visible = MsCore.MsoTriState.msoFalse;
            MsPpt._Presentation pPresent = pApp.Presentations.Open(
                strPpt,
                MsCore.MsoTriState.msoTrue, 
                MsCore.MsoTriState.msoFalse,
                MsCore.MsoTriState.msoFalse);

            pPresent.PrintOut(-1, -1, strOutTif_);

            Thread.Sleep(nWaitSecond_ * 1000);
            pPresent.Close();
            pApp.Quit();
        }
    }
}
