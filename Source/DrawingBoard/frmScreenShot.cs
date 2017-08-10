using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;

namespace XDrawBoard
{
    public enum MouseLocation
    {
        Outside,
        TopLeft,
        TopMiddle,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomMiddle,
        BottomRight
    }

    public partial class FrmScreenShot : Form
    {
        #region "Var"
        const int SeledFrameWidth = 2;//定义外边框的宽度
        const int SeledHandWidth = SeledFrameWidth * 3;//定义手柄矩形的大小

        private Bitmap _bmpFullScreen;   //用于提供复制的图片源
        private MouseLocation _euMouseLocate = MouseLocation.Outside;//鼠标状态
        private Point _ptCursorLastPosition;
        private Point _ptSeledStart;
        private Point _ptSeledEnd;
        private Rectangle _rectSeledSreen = Rectangle.Empty;//选中的区域

        #region "定义八个要拖动的手柄区域"
        private Rectangle _rectTopLeftHand = Rectangle.Empty;
        private Rectangle _rectTopRightHand = Rectangle.Empty;
        private Rectangle _rectBottomLeftHand = Rectangle.Empty;
        private Rectangle _rectBottomRightHand = Rectangle.Empty;

        private Rectangle _rectTopMiddleHand = Rectangle.Empty;
        private Rectangle _rectBottomMiddleHand = Rectangle.Empty;
        private Rectangle _rectMiddleLeftHand = Rectangle.Empty;
        private Rectangle _rectMiddleRightHand = Rectangle.Empty;
        #endregion

        /// <summary>
        /// 保存文件时，为区分不同文件而生成的索引（每调用一次增加1）
        /// </summary>
        public int FileIndex { get; set; }
        #endregion

        public FrmScreenShot(Bitmap bmpScreen_)
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
               ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            this.BackgroundImage = _bmpFullScreen = bmpScreen_;
        }

        private void frmPickScreen_Load(object sender, EventArgs e)
        {
            //_bmpFullScreen = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            //Graphics grScreen = Graphics.FromImage(_bmpFullScreen);
            //grScreen.CopyFromScreen(new Point(0, 0), new Point(0, 0), Screen.PrimaryScreen.Bounds.Size);
            //this.BackgroundImage = _bmpFullScreen;

            this.Cursor = Cursors.Cross;
        }

        private void frmPickScreen_Paint(object sender, PaintEventArgs e)
        {
            Graphics grClient = e.Graphics;
            using (SolidBrush sb = new SolidBrush(Color.FromArgb(100, Color.Black)))
            {
                grClient.FillRectangle(sb, ClientRectangle);
            }

            DrawSeledScreen(grClient, _rectSeledSreen);
        }

        private void frmPickScreen_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            _ptCursorLastPosition = e.Location;
            if (_rectSeledSreen.IsEmpty)
            {
                _ptSeledStart = _ptSeledEnd = e.Location;
            }
            else
            {
                if (_rectTopLeftHand.Contains(e.Location))
                {
                    _euMouseLocate = MouseLocation.TopLeft;
                }
                else if (_rectBottomRightHand.Contains(e.Location))
                {
                    _euMouseLocate = MouseLocation.BottomRight;
                }
                else if (_rectTopRightHand.Contains(e.Location))
                {
                    _euMouseLocate = MouseLocation.TopRight;
                }
                else if (_rectBottomLeftHand.Contains(e.Location))
                {
                    _euMouseLocate = MouseLocation.BottomLeft;
                }
                else if (_rectTopMiddleHand.Contains(e.Location))
                {
                    _euMouseLocate = MouseLocation.TopMiddle;
                }
                else if (_rectBottomMiddleHand.Contains(e.Location))
                {
                    _euMouseLocate = MouseLocation.BottomMiddle;
                }
                else if (_rectMiddleLeftHand.Contains(e.Location))
                {
                    _euMouseLocate = MouseLocation.MiddleLeft;
                }
                else if (_rectMiddleRightHand.Contains(e.Location))
                {
                    _euMouseLocate = MouseLocation.MiddleRight;
                }
                else if (_rectSeledSreen.Contains(e.Location))
                {
                    _euMouseLocate = MouseLocation.MiddleCenter;
                }
                else
                {
                    _euMouseLocate = MouseLocation.Outside;
                }
            } // !_rectToSave.IsEmpty
        }

        private void frmPickScreen_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.None)
            {
                SetCursonWhenMouseMove(e);
                return;
            }
            if (e.Button != MouseButtons.Left) return;

            Point ptCurrent = e.Location;
            if (!_rectSeledSreen.IsEmpty)
            {
                switch (_euMouseLocate)
                {
                    case MouseLocation.TopLeft:
                        _ptSeledStart = ptCurrent;
                        break;
                    case MouseLocation.TopMiddle:
                        _ptSeledStart.Y = ptCurrent.Y;
                        break;
                    case MouseLocation.TopRight:
                        _ptSeledStart.Y = ptCurrent.Y;
                        _ptSeledEnd.X = ptCurrent.X;
                        break;
                    case MouseLocation.MiddleLeft:
                        _ptSeledStart.X = ptCurrent.X;
                        break;
                    case MouseLocation.MiddleCenter:
                        int offsetX = ptCurrent.X - _ptCursorLastPosition.X;
                        int offsetY = ptCurrent.Y - _ptCursorLastPosition.Y;
                        _ptCursorLastPosition = ptCurrent;

                        _ptSeledStart.X += offsetX;
                        _ptSeledEnd.X += offsetX;
                        _ptSeledStart.Y += offsetY;
                        _ptSeledEnd.Y += offsetY;
                        break;
                    case MouseLocation.MiddleRight:
                        _ptSeledEnd.X = ptCurrent.X;
                        break;
                    case MouseLocation.BottomLeft:
                        _ptSeledStart.X = ptCurrent.X;
                        _ptSeledEnd.Y = ptCurrent.Y;
                        break;
                    case MouseLocation.BottomMiddle:
                        _ptSeledEnd.Y = ptCurrent.Y;
                        break;
                    case MouseLocation.BottomRight:
                        _ptSeledEnd = ptCurrent;
                        break;

                    case MouseLocation.Outside:
                    default:
                        return;
                }
            }
            else // _rectToSave.IsEmpty
            {
                _ptSeledEnd = e.Location;

                _euMouseLocate = MouseLocation.BottomRight;
            }

            AjustSeledScreen();
            AjustSeledHand();

            this.Invalidate();//使界面失效，刷新界面
        }

        private void AjustSeledHand()
        {
            Rectangle rectFrame = _rectSeledSreen;
            //定义要标识的四个角
            Point pTL = new Point(rectFrame.Left, rectFrame.Top);
            Point pTR = new Point(rectFrame.Right, rectFrame.Top);
            Point pBL = new Point(rectFrame.Left, rectFrame.Bottom);
            Point pBR = new Point(rectFrame.Right, rectFrame.Bottom);

            int nHalfWidth = SeledHandWidth / 2;
            _rectTopLeftHand = new Rectangle(pTL.X - nHalfWidth, pTL.Y - nHalfWidth, SeledHandWidth, SeledHandWidth);
            _rectTopRightHand = new Rectangle(pTR.X - nHalfWidth, pTR.Y - nHalfWidth, SeledHandWidth, SeledHandWidth);
            _rectBottomLeftHand = new Rectangle(pBL.X - nHalfWidth, pBL.Y - nHalfWidth, SeledHandWidth, SeledHandWidth);
            _rectBottomRightHand = new Rectangle(pBR.X - nHalfWidth, pBR.Y - nHalfWidth, SeledHandWidth, SeledHandWidth);

            //计算中部手柄的宽度，中部手柄的宽度为整个宽度的1/3
            int middleWidth = (int)Math.Abs(Math.Abs(pTL.X - pTR.X) / 3.0);
            if (middleWidth > 10)
            {
                _rectTopMiddleHand = new Rectangle(pTL.X + middleWidth, pTR.Y - nHalfWidth, middleWidth, SeledHandWidth);
                _rectBottomMiddleHand = new Rectangle(pBL.X + middleWidth, pBR.Y - nHalfWidth, middleWidth, SeledHandWidth);
            }
            else
            {
                _rectTopMiddleHand = Rectangle.Empty;
                _rectBottomMiddleHand = Rectangle.Empty;
            }

            //计算边框中部手柄的高度
            int middleHight = (int)(Math.Abs(Math.Abs(pTL.Y - pBL.Y)) / 3.0);
            if (middleHight > 10)
            {
                _rectMiddleLeftHand = new Rectangle(pTL.X - nHalfWidth, pTL.Y + middleHight, SeledHandWidth, middleHight);
                _rectMiddleRightHand = new Rectangle(pTR.X - nHalfWidth, pTL.Y + middleHight, SeledHandWidth, middleHight);
            }
            else
            {
                _rectMiddleLeftHand = Rectangle.Empty;
                _rectMiddleRightHand = Rectangle.Empty;
            }
        }

        private void AjustSeledScreen()
        {
            const int MinPixel = 10;
            // Reset start&end
            int nMinX = Math.Min(_ptSeledStart.X, _ptSeledEnd.X);
            int nMaxX = Math.Max(_ptSeledStart.X, _ptSeledEnd.X);
            int nMinY = Math.Min(_ptSeledStart.Y, _ptSeledEnd.Y);
            int nMaxY = Math.Max(_ptSeledStart.Y, _ptSeledEnd.Y);
            if (nMinX < 0) nMinX = 0;
            if (nMinY < 0) nMinY = 0;
            if (nMaxX - nMinX < MinPixel) nMaxX = nMinX + MinPixel;
            if (nMaxY - nMinY < MinPixel) nMaxY = nMinY + MinPixel;
            if (nMaxX > _bmpFullScreen.Width)
            {
                nMaxX = _bmpFullScreen.Width;
                if (nMaxX - nMinX < MinPixel) nMinX = nMaxX - MinPixel;
            }
            if (nMaxY > _bmpFullScreen.Height)
            {
                nMaxY = _bmpFullScreen.Height;
                if (nMaxY - nMinY < MinPixel) nMinY = nMaxY - MinPixel;
            }

            // Adjust the rect
            _ptSeledStart = new Point(nMinX, nMinY);
            _ptSeledEnd = new Point(nMaxX, nMaxY);
            _rectSeledSreen = new Rectangle(nMinX, nMinY, nMaxX - nMinX, nMaxY - nMinY);
        }

        private void SetCursonWhenMouseMove(MouseEventArgs e)
        {
            if (!_rectSeledSreen.IsEmpty)//存在选择的区域时
            {
                //下面八个分别指定八个方向
                if (_rectTopLeftHand.Contains(e.Location) || _rectBottomRightHand.Contains(e.Location))
                {
                    //this.Cursor = BoardUtility.TryLoadCursor("ColouredAero_nwse.cur", Cursors.SizeNWSE);
                    this.Cursor = Cursors.SizeNWSE;
                }
                else if (_rectTopRightHand.Contains(e.Location) || _rectBottomLeftHand.Contains(e.Location))
                {
                    this.Cursor = Cursors.SizeNESW;
                }
                else if (_rectTopMiddleHand.Contains(e.Location) || _rectBottomMiddleHand.Contains(e.Location))
                {
                    this.Cursor = Cursors.SizeNS;
                }
                else if (_rectMiddleLeftHand.Contains(e.Location) || _rectMiddleRightHand.Contains(e.Location))
                {
                    this.Cursor = Cursors.SizeWE;
                }
                else
                {
                    if (_rectSeledSreen.Contains(e.Location))
                    {
                        this.Cursor = Cursors.SizeAll;
                    }
                    else
                    {
                        this.Cursor = Cursors.Arrow;
                    }
                }
            }
            else
            {
                this.Cursor = Cursors.PanSE; // Cursors.Cross;
            }
        }

        private void frmPickScreen_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (this._rectSeledSreen == Rectangle.Empty)
                {
                    this.Close();
                }
                else
                {
                    this._rectSeledSreen = Rectangle.Empty;
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// 显示用户选择的区域
        /// </summary>
        /// <param name="grClient_">绘图的画布</param>
        /// <param name="rectSeled_">要显示的区域</param>
        private void DrawSeledScreen(Graphics grClient_, Rectangle rectSeled_)
        {
            if (rectSeled_.IsEmpty) return;

            using (Bitmap bmpThis = new Bitmap(this.Width, this.Height))
            {
                using (Graphics grThis = Graphics.FromImage(bmpThis))
                {
                    //绘制要显示的图片区域
                    grThis.DrawImage(_bmpFullScreen, rectSeled_, rectSeled_, GraphicsUnit.Pixel);

                    DrawFrameAndHand(rectSeled_, grThis);
                }
                grClient_.DrawImage(bmpThis, 0, 0);
            }
        }

        private void DrawFrameAndHand(Rectangle rectSeled_, Graphics grThis)
        {
            using (Pen penLine = new Pen(Color.FromArgb(180, Color.Gold), SeledFrameWidth))
            {
                penLine.Alignment = PenAlignment.Center;
                grThis.DrawRectangle(penLine, rectSeled_);

                DrawHandAndTips(grThis, penLine);
            }
        }

        private void DrawHandAndTips(Graphics grThis, Pen penLine)
        {
            //绘制八个手柄
            //penLine.Color = Color.Gold;
            using (SolidBrush sbHand = new SolidBrush(Color.FromArgb(180, Color.GreenYellow)))
            {
                grThis.FillRectangle(sbHand, _rectTopLeftHand);
                grThis.FillRectangle(sbHand, _rectTopRightHand);
                grThis.FillRectangle(sbHand, _rectBottomLeftHand);
                grThis.FillRectangle(sbHand, _rectBottomRightHand);

                if (!_rectTopMiddleHand.IsEmpty)
                {
                    grThis.FillRectangle(sbHand, _rectTopMiddleHand);
                    grThis.FillRectangle(sbHand, _rectBottomMiddleHand);
                }
                if (!_rectMiddleLeftHand.IsEmpty)
                {
                    grThis.FillRectangle(sbHand, _rectMiddleLeftHand);
                    grThis.FillRectangle(sbHand, _rectMiddleRightHand);
                }

                //绘制手柄的外框
                penLine.Color = Color.Green;
                penLine.Width = 1.0f;

                grThis.DrawRectangle(penLine, _rectTopLeftHand);
                grThis.DrawRectangle(penLine, _rectTopRightHand);
                grThis.DrawRectangle(penLine, _rectBottomLeftHand);
                grThis.DrawRectangle(penLine, _rectBottomRightHand);
                if (!_rectTopMiddleHand.IsEmpty)
                {
                    grThis.DrawRectangle(penLine, _rectTopMiddleHand);
                    grThis.DrawRectangle(penLine, _rectBottomMiddleHand);
                }
                if (!_rectMiddleLeftHand.IsEmpty)
                {
                    grThis.DrawRectangle(penLine, _rectMiddleLeftHand);
                    grThis.DrawRectangle(penLine, _rectMiddleRightHand);
                }
            }

            DrawSeledTips(grThis);
        }

        private void DrawSeledTips(Graphics grThis)
        {
            //在矩形区域加入图片大小以及坐标信息
            using (SolidBrush sbMsgInfo = new SolidBrush(Color.FromArgb(200, Color.Gold)))
            {
                string strMsg = string.Format("矩形区域大小: {0}×{1}像素；按ESC键退出\n", _rectSeledSreen.Width, _rectSeledSreen.Height)
                    //+ BuildColorMsg()
                    + "双击或Ctrl+C复制截图，Ctrl+S保存截图；右击重新截图或退出";

                using (Font fontMsg = new Font(this.Font.Name, 16f, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    SizeF sizeFMsg = grThis.MeasureString(strMsg, fontMsg);
                    int nMsgHeight = (int)sizeFMsg.Height + fontMsg.Height;
                    int nMsgWidth = (int)sizeFMsg.Width + 5;

                    if (_rectSeledSreen.Left + nMsgWidth < _bmpFullScreen.Width)
                    {
                        if (_rectSeledSreen.Top > nMsgHeight)
                        {
                            grThis.DrawString(strMsg, fontMsg, sbMsgInfo,
                                new Point(_rectSeledSreen.Left, _rectSeledSreen.Top - nMsgHeight));
                        }
                        else if (_rectSeledSreen.Bottom + nMsgHeight < _bmpFullScreen.Height)
                        {
                            grThis.DrawString(strMsg, fontMsg, sbMsgInfo,
                                 new Point(_rectSeledSreen.Left, _rectSeledSreen.Bottom + fontMsg.Height));
                        }
                        else
                        {
                            sbMsgInfo.Color = Color.Black;
                            grThis.DrawString(strMsg, fontMsg, sbMsgInfo,
                                new Point(_rectSeledSreen.Left + 5, _rectSeledSreen.Top + this.Font.Height));
                        }
                    }
                    else
                    { // Need to show left
                        int nTop = _rectSeledSreen.Top;
                        int nLeft = _rectSeledSreen.Left - nMsgWidth;
                        if (nLeft < 5)
                            nLeft = 5;
                        grThis.DrawString(strMsg, fontMsg, sbMsgInfo, new PointF(nLeft, nTop));
                    }
                } // using-font
            } // using-brush
        }

        //private string BuildColorMsg()
        //{
        //    Color colorMouse = GetPointColor(_bmpScreen, Cursor.Position);

        //    string strColorName;
        //    if (colorMouse.IsNamedColor || colorMouse.IsKnownColor || colorMouse.IsSystemColor)
        //    {
        //        KnownColor kowncolor = colorMouse.ToKnownColor();
        //        strColorName = kowncolor.ToString();
        //    }
        //    else
        //    {
        //        strColorName = colorMouse.Name;
        //    }
        //    string colorMsg = string.Format("RGBA({0},{1},{2},{3}) [{4}]",
        //        colorMouse.R, colorMouse.G, colorMouse.B, colorMouse.A,
        //        strColorName);
        //    colorMsg = string.Format("鼠标位置: ({0},{1}); 背景颜色: {2}\n", Cursor.Position.X, Cursor.Position.Y, colorMsg);
        //    return colorMsg;
        //}
        ///// <summary>
        ///// 获取当前坐标颜色
        ///// </summary>
        ///// <param name="destIamge">目标图像</param>
        ///// <param name="postion">目标图像上的点坐标</param>
        ///// <returns></returns>
        //private Color GetPointColor(Image destIamge, Point postion)
        //{
        //    Bitmap tempBmp = new Bitmap(destIamge);
        //    return tempBmp.GetPixel(postion.X, postion.Y);
        //}

        private void ToSaveScreenShot()
        {
            if (_rectSeledSreen.IsEmpty) return;

            using (Bitmap bmpToSave = GetSavedBmp())
            {
                if (BoardUtility.SaveScreenShot(bmpToSave, FileIndex))
                    this.Close();
            }
        }

        private void ToCopyScreenshot()
        {
            if (_rectSeledSreen.IsEmpty) return;

            using (Bitmap bmpToSave = GetSavedBmp())
            {   //MemoryStream memStream = new MemoryStream();
                //bmpToSave.Save(memStream, ImageFormat.Gif);
                Clipboard.SetImage(bmpToSave);
            }
            this.Close();
        }

        private Bitmap GetSavedBmp()
        {
            Bitmap bmpToSave = new Bitmap(_rectSeledSreen.Width, _rectSeledSreen.Height);
            using (Graphics g = Graphics.FromImage(bmpToSave))
            {
                g.DrawImage(this.BackgroundImage,
                    new RectangleF(0f, 0f, (float)bmpToSave.Width, (float)bmpToSave.Height),
                    (RectangleF)_rectSeledSreen,
                    GraphicsUnit.Pixel);
            }
            return bmpToSave;
        }

        private void frmPickScreen_DoubleClick(object sender, EventArgs e)
        {
            if (_rectSeledSreen.Contains(Cursor.Position))
            {
                ToCopyScreenshot();
            }
        }

        private void frmPickScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Visible = false;
                this.Close();
            }
            else if (e.KeyCode == Keys.S && e.Modifiers == Keys.Control)
            {
                ToSaveScreenShot();
            }
            else if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            {
                ToCopyScreenshot();
            }
        }

        private void FrmScreenShot_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.BackgroundImage = null;
            _bmpFullScreen.Dispose();
        }
    }
}