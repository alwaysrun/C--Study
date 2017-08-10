using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace XDrawBoard
{
    public static class BoardUtility
    {
        /// <summary>
        /// 加载光标
        /// </summary>
        /// <param name="lpFileName">光标文件名（包括*.cur和*.ani）</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursorFromFile(string lpFileName);

        static string _strCurFolder = null;
        public static Cursor TryLoadCursor(string strFile, Cursor defCurso)
        {
            if (_strCurFolder == null)
                _strCurFolder = Path.Combine(Application.StartupPath, "Cursor");
            string strCursorFile = Path.Combine(_strCurFolder, strFile);
            if (File.Exists(strCursorFile))
                return new Cursor(BoardUtility.LoadCursorFromFile(strCursorFile));
            else
                return defCurso;
        }

        public static bool SaveScreenShot(Bitmap bmpToSave, int nNameIndex_)
        {
            bool bSaved = false;
            using (SaveFileDialog dlgSaveFile = new SaveFileDialog())
            {
                dlgSaveFile.Title = "截屏另存为...";
                dlgSaveFile.FileName = string.Format("Pic{0}_{1}", DateTime.Now.ToString("yyyyMMddHHmm"), nNameIndex_);
                dlgSaveFile.Filter = "Jpg图片|*.jpg|Png图片|*.png|Bmp位图|*.bmp|Gif图片|*.gif|图标图片|*.ico";
                if (dlgSaveFile.ShowDialog() == DialogResult.OK)
                {
                    bSaved = true;
                    switch (dlgSaveFile.FilterIndex)
                    {
                        case 1:
                            bmpToSave.Save(dlgSaveFile.FileName, ImageFormat.Jpeg);
                            break;
                        case 2:
                            bmpToSave.Save(dlgSaveFile.FileName, ImageFormat.Png);
                            break;
                        case 3:
                            bmpToSave.Save(dlgSaveFile.FileName, ImageFormat.Bmp);
                            break;
                        case 4:
                            bmpToSave.Save(dlgSaveFile.FileName, ImageFormat.Gif);
                            break;
                        case 5:
                            bmpToSave.Save(dlgSaveFile.FileName, ImageFormat.Icon);
                            break;
                        default:
                            bSaved = false;
                            break;
                    }
                }

                return bSaved;
            }
        }

        public static Rectangle BuildRectangle(Point ptStart_, Point ptEnd_)
        {
            int nMinX = Math.Min(ptStart_.X, ptEnd_.X);
            int nMinY = Math.Min(ptStart_.Y, ptEnd_.Y);

            return new Rectangle(nMinX, nMinY, Math.Abs(ptStart_.X-ptEnd_.X), Math.Abs(ptStart_.Y-ptEnd_.Y));
        }

        public static void AdjustRectangle(ref Point ptStart_, ref Point ptEnd_)
        {
            int nMinX = Math.Min(ptStart_.X, ptEnd_.X);
            int nMaxX = Math.Max(ptStart_.X, ptEnd_.X);
            int nMinY = Math.Min(ptStart_.Y, ptEnd_.Y);
            int nMaxY = Math.Max(ptStart_.Y, ptEnd_.Y);

            ptStart_ = new Point(nMinX, nMinY);
            ptEnd_ = new Point(nMaxX, nMaxY);
        }

        public static Bitmap PickScreenToBitmap()
        {
            return PickScreenToBitmap(Color.Empty);
        }

        public static Bitmap PickScreenToBitmap(Color colorFill_)
        {
            Bitmap bmpScreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics grScreen = Graphics.FromImage(bmpScreen);
            if(colorFill_==Color.Empty)
            {
                grScreen.CopyFromScreen(new Point(0, 0), new Point(0, 0), Screen.PrimaryScreen.Bounds.Size);
            }
            else
            {
                using(SolidBrush sbFill = new SolidBrush(colorFill_))
                {
                    grScreen.FillRectangle(sbFill, 0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                }
            }
            return bmpScreen;
        }
    } // class
}
