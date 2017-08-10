using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;

namespace XDrawBoard
{
    public partial class FrmMain : Form
    {
        FrmColorBoard _frmBoard;
        GraphicsPath _gpFormFrame = null;
        bool _bInDrawBoard = false;
        string _strBrushPreviewFile;

        #region "Init"
        public FrmMain()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);


            this.cbDrawMethod.SelectedIndex = 0;
            this.cbLineStyle.SelectedIndex = 0;
            this.cbStartCap.SelectedIndex = 0;
            this.cbEndCap.SelectedIndex = 0;

            ResetFromFrame(false);
            //this.Location = new Point((Screen.PrimaryScreen.Bounds.Width - this.Width) / 2, 0);
            this.Location = new Point((SystemInformation.WorkingArea.Width - this.Width) / 2, 0);
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            if (!XCheckPath() || !XInitBmp())
            {
                //this.Close();
                Application.Exit();
                return;
            }

            this.TopMost = true;
        }

        private void ResetFromFrame(bool bExpan_)
        {
            if(bExpan_)
            {
                this.Height += (this.panBoard.Height-2);
            }
            else
            {
                this.Height -= (this.panBoard.Height - 2);
            }

            //确定窗体的形状
            int nAngleSize = 5;
            Point pTL = new Point(0, 0);
            Point pTR = new Point(this.Width, 0);
            Point pBL = new Point(0, this.Height);
            Point pBR = new Point(this.Width, this.Height);
            _gpFormFrame = new GraphicsPath();
            _gpFormFrame.AddLines(new Point[] {
                new Point(pTL.X,pTL.Y+nAngleSize),new Point(pTL.X+nAngleSize,pTL.Y),
                new Point(pTR.X-nAngleSize,pTR.Y),new Point(pTR.X,pTR.Y+nAngleSize),
                new Point(pBR.X,pBR.Y-nAngleSize),new Point(pBR.X-nAngleSize,pBR.Y),
                new Point(pBL.X+nAngleSize,pBL.Y),new Point(pBL.X,pBL.Y-nAngleSize)
            });
            _gpFormFrame.CloseAllFigures();
            this.Region = new Region(_gpFormFrame);

            this.Invalidate();
        }

        private bool XCheckPath()
        {
            string strRunPath = Application.StartupPath;
            string[] arySubPaths = new string[] { "Picture", "Cursor" };
            foreach (var strSub in arySubPaths)
            {
                if (!Directory.Exists(Path.Combine(strRunPath, strSub)))
                {
                    string strInfo = string.Format("Sub-Directory {0} are missing", string.Join(",", arySubPaths));
                    MessageBox.Show(strInfo);
                    return false;
                }
            }

            return true;
        }

        private bool XInitBmp()
        {
            _strBrushPreviewFile = Path.Combine(Application.StartupPath, @"Picture\BrushBackgound.png");
            if (!File.Exists(_strBrushPreviewFile))
            {
                MessageBox.Show(@"Can not find file: .\Picture\BrushBackgound.png");
                return false;
            }

            var testMap = new Bitmap(Image.FromFile(_strBrushPreviewFile));
            return true;
        }
        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_gpFormFrame != null)
            {
                Graphics gPaint = e.Graphics;
                using (Pen pDrawOutLine = new Pen(Color.Gold, 2f))
                {
                    pDrawOutLine.Alignment = PenAlignment.Inset;
                    gPaint.DrawPath(pDrawOutLine, _gpFormFrame);
                }
            }
        }

        #region "Form close"
        private void lblClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void lblClose_MouseEnter(object sender, EventArgs e)
        {
            lblClose.ForeColor = Color.White;
            lblClose.BackColor = Color.GreenYellow;
        }

        private void lblClose_MouseLeave(object sender, EventArgs e)
        {
            lblClose.ForeColor = Color.Black;
            lblClose.BackColor = Color.Transparent;
        }
        #endregion

        #region "Form ShowAndHide"
        private void HideImmediately()
        {
            this.Top = 7 - this.Height;
            tmAutoHidden.Stop();
        }

        private void ForbidHide()
        {
            //_bCanHidden = false;
            tmAutoHidden.Stop();
        }
        private void RestoreHide()
        {
            tmAutoHidden.Interval = 1000;
            tmAutoHidden.Start();
            //_bCanHidden = true;
        }

        private void tmHidden_Tick(object sender, EventArgs e)
        {
            if (RectangleToScreen(this.DisplayRectangle).Contains(Cursor.Position))
            {
                return;
            }

            if (this.Bottom <= 11)
            {
                HideImmediately();

                return;
            }

            tmAutoHidden.Interval = 10;
            this.Top -= 11;
        }

        private void tmShow_Tick(object sender, EventArgs e)
        {
            ToShowForm();
        }

        private void ToShowForm()
        {
            if (this.Top + 11 > 0)
            {
                this.Top = 0;
                tmShow.Stop();

                if(_bInDrawBoard)
                {
                    this._frmBoard.BringToFront();
                }
                SetButtonStatus();
                RestoreHide();
                return;
            }

            this.Top += 11;
        }

        private void frmMain_MouseEnter(object sender, EventArgs e)
        {
            ToShowForm();
            this.tmShow.Start();
        }
        #endregion
        
        private void btnDrawBoard_Click(object sender, EventArgs e)
        {
            if(_bInDrawBoard)
            {
                CloseColorBoard();
            }
            else
            { 
                OpenColorBoard();
            }
        }

        private void chkTryErase_CheckedChanged(object sender, EventArgs e)
        {
            if(_frmBoard != null)
                CheckTryErase();
        }

        private void OpenColorBoard()
        {
            this.btnDrawBoard.Text = "关闭  画板";
            _bInDrawBoard = true;
            ResetFromFrame(true);

            //////////////////////////////////////////////////////////////////////////
            if (_frmBoard == null)
            {
                this.Visible = false;
                _frmBoard = new FrmColorBoard(this);
                _frmBoard.BoardInfo.Init(ckbScreenBoard.Checked, picBackgroundColor.BackColor);
                BuildSeledPen();
                this.Visible = true;
            }
            else
            {
                this.Visible = false;
                _frmBoard.BoardInfo.ResetScreenShot();
                _frmBoard.SetDrawBoard(ckbScreenBoard.Checked);
                //PreviewSeledPen(_frmBoard.BoardInfo.PenDrawing);
                this.Visible = true;
            }

            _frmBoard.BringToFront();
            _frmBoard.Show();
            CheckTryErase();

            this.BringToFront();
            SetButtonStatus();
        }

        private void CheckTryErase()
        {
            if (this.chkTryErase.Checked)
            {
                this.cbDrawMethod.SelectedIndex = (int)DrawMethods.FillRect;
                _frmBoard.SetDrawBoard(true);
                _frmBoard.ResetTryErase();
                this.cbDrawMethod.SelectedIndex = (int)DrawMethods.Eraser;
            }
        }

        public void CloseColorBoard()
        {
            this.btnDrawBoard.Text = "开启  画板";
            _bInDrawBoard = false;
            ResetFromFrame(false);

            if (_frmBoard != null)
            {
                _frmBoard.TryHideSelf();
            }
        }

        private void btnPrintScreen_Click(object sender, EventArgs e)
        {
            SaveCurrentScreen();
        }

        private bool SaveCurrentScreen()
        {
            HideImmediately();
            this.Visible = false;

            Bitmap bmpToSave = BoardUtility.PickScreenToBitmap();
            bool bSave = BoardUtility.SaveScreenShot(bmpToSave, _nScreenShotCount++);

            this.Visible = true;
            return bSave;
        }
        
        int _nScreenShotCount = 1;
        private void btnCutScreen_Click(object sender, EventArgs e)
        {
            HideImmediately();
            this.Visible = false;

            FrmScreenShot frmPick = new FrmScreenShot(BoardUtility.PickScreenToBitmap());
            frmPick.FileIndex = _nScreenShotCount++;
            frmPick.ShowDialog();

            this.Visible = true;
        }
                
        /// <summary>
        /// 检查撤销和恢复按键是否可用
        /// </summary>
        private void SetButtonStatus()
        {
            if (!_bInDrawBoard) return;

            btnUndo.Enabled = _frmBoard.BoardInfo.ShapeHistory.Count != 0;
            btnRedo.Enabled = _frmBoard.BoardInfo.UndoShapes.Count != 0;
        }

        private void btnRedo_Click(object sender, EventArgs e)
        {
            _frmBoard.BoardInfo.Redo();

            SetButtonStatus();
            _frmBoard.Invalidate();
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            _frmBoard.BoardInfo.Undo();

            SetButtonStatus();
            _frmBoard.Invalidate();
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            _frmBoard.BoardInfo.Clear();

            SetButtonStatus();
            _frmBoard.Invalidate();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_frmBoard == null) return;

            SaveColorboard();
        }

        public void SaveColorboard()
        {
            if (SaveCurrentScreen())
                _frmBoard.SetHasSaved();
        }

        #region "Pen changed"
        DashStyle GetPenDashStyle()
        {
            return (DashStyle)(this.cbLineStyle.SelectedIndex);
        }

        LineCap GetPenDashCap(int nSeled_)
        {
            LineCap euCap = LineCap.Flat;
            switch (nSeled_)
            {
                //case 1:
                //    euCap = LineCap.Square;
                //    break;
                //case 2:
                //    euCap = LineCap.Round;
                //    break;
                //case 3:
                //    euCap = LineCap.Triangle;
                //    break;
                case 1:
                    euCap = LineCap.SquareAnchor;
                    break;
                case 2:
                    euCap = LineCap.RoundAnchor;
                    break;
                case 3:
                    euCap = LineCap.DiamondAnchor;
                    break;
                case 4:
                    euCap = LineCap.ArrowAnchor;
                    break;
                default:
                    break;
            }

            return euCap;
        }

        void BuildSeledPen()
        {
            if (!_bInDrawBoard) return;

            int nAlpha = (int)(255.0f / 100 * (float)numOpacity.Value);
            Color penColor = Color.FromArgb(nAlpha, this.picPenColor.BackColor);
            Pen penLine = new Pen(penColor, (float)this.numPenSize.Value);
            penLine.DashStyle = GetPenDashStyle();
            if (this.cbStartCap.Enabled)
            {
                penLine.StartCap = GetPenDashCap(this.cbStartCap.SelectedIndex);
                penLine.EndCap = GetPenDashCap(this.cbEndCap.SelectedIndex);
            }

            _frmBoard.BoardInfo.PenDrawing = penLine;

            PreviewSeledPen(penLine);
        }

        private void PreviewSeledPen(Pen penLine_)
        {
            if (!_bInDrawBoard) return;

            Bitmap bmpBack;
            if (this.ckbScreenBoard.Checked)
            {
                bmpBack = new Bitmap(Image.FromFile(_strBrushPreviewFile), this.picBackgroundColor.Width, this.picBackgroundColor.Height);
            }
            else
            {
                bmpBack = new Bitmap(this.picBackgroundColor.Width, this.picBackgroundColor.Height);
            }

            using (Graphics grShow = Graphics.FromImage(bmpBack))
            {
                if (!this.ckbScreenBoard.Checked)
                {
                    using (Brush picBrush = new SolidBrush(this.picBackgroundColor.BackColor))
                    {
                        grShow.FillRectangle(picBrush, 0, 0, bmpBack.Width, bmpBack.Height);
                    }
                }

                PreviewDrawLine(penLine_, bmpBack, grShow);
            }
        }

        private void PreviewDrawLine(Pen penLine_, Bitmap bmpBack, Graphics grShow)
        {
            grShow.SmoothingMode = SmoothingMode.AntiAlias;
            Point pStart = new Point(bmpBack.Width / 10 * 2, bmpBack.Height / 10 * 1);
            Point pEnd = new Point(bmpBack.Width / 10 * 8, bmpBack.Height / 10 * 9);

            switch (_frmBoard.BoardInfo.PenMethod)
            {
                case DrawMethods.Pencil:
                    {
                        Point pMid1 = new Point(bmpBack.Width / 10 * 4, bmpBack.Height / 10 * 6);
                        Point pMid2 = new Point(bmpBack.Width / 10 * 6, bmpBack.Height / 10 * 4);
                        grShow.DrawCurve(penLine_, new Point[] { pStart, pMid1, pMid2, pEnd });
                    }
                    break;

                case DrawMethods.Beeline:
                    grShow.DrawLine(penLine_, pStart, pEnd);
                    break;

                case DrawMethods.Rectangle:
                    grShow.DrawRectangle(penLine_, pStart.X, pStart.Y, pEnd.X - pStart.X, pEnd.Y - pStart.Y);

                    break;
                case DrawMethods.Ellipse:
                    grShow.DrawEllipse(penLine_, pStart.X, pStart.Y, pEnd.X - pStart.X, pEnd.Y - pStart.Y);
                    break;

                case DrawMethods.FillRect:
                    using(Brush brFill = new SolidBrush(penLine_.Color))
                    {
                        grShow.FillRectangle(brFill, pStart.X, pStart.Y, pEnd.X - pStart.X, pEnd.Y - pStart.Y);
                    }
                    break;
                case DrawMethods.FillEllipse:
                    using(Brush brFill = new SolidBrush(penLine_.Color))
                    {
                        grShow.FillEllipse(brFill, pStart.X, pStart.Y, pEnd.X - pStart.X, pEnd.Y - pStart.Y);
                    }
                    break;

                default:
                    break;
            }
            this.picBackgroundColor.Image = bmpBack;
        }
        #endregion

        private void numOpacity_ValueChanged(object sender, EventArgs e)
        {
            BuildSeledPen();
        }

        private void numPenSize_ValueChanged(object sender, EventArgs e)
        {
            BuildSeledPen();
        }

        private void ckbScreenBoard_CheckedChanged(object sender, EventArgs e)
        {
            _frmBoard.SetDrawBoard(ckbScreenBoard.Checked);
            PreviewSeledPen(_frmBoard.BoardInfo.PenDrawing);
        }

        private void picBackgroundColor_Click(object sender, EventArgs e)
        {
            if (this.ckbScreenBoard.Checked)
            {
                this.Visible = false;
                _frmBoard.SetBoardScreen();
                this.Visible = true;
            }
            else
            {
                ForbidHide();
                using (ColorDialog dlgColor = new ColorDialog())
                {
                    dlgColor.AnyColor = true;
                    if (dlgColor.ShowDialog() == DialogResult.OK)
                    {
                        this.picBackgroundColor.BackColor = dlgColor.Color;
                        _frmBoard.SetBoardColor(dlgColor.Color);
                        PreviewSeledPen(_frmBoard.BoardInfo.PenDrawing);
                    }
                }
                RestoreHide();
            }
        }

        private void picPenColor_Click(object sender, EventArgs e)
        {
            ForbidHide();
            using (ColorDialog dlgColor = new ColorDialog())
            {
                dlgColor.AnyColor = true;
                if (dlgColor.ShowDialog() == DialogResult.OK)
                {
                    ((PictureBox)sender).BackColor = dlgColor.Color;
                    BuildSeledPen();

                    CheckTryErase();
                }
            }
            RestoreHide();
        }

        #region "Method"
        private void MethodChanged(DrawMethods euMethod)
        {
            if (_frmBoard.BoardInfo.PenMethod == euMethod) return;

            this.cbStartCap.Enabled = this.cbEndCap.Enabled = (euMethod == DrawMethods.Beeline);

            _frmBoard.SetDrawMethod(euMethod);
            BuildSeledPen();
        }
        private void cbDrawMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_bInDrawBoard) return;

            MethodChanged((DrawMethods)this.cbDrawMethod.SelectedIndex);
        }

        private void btnPencil_Click(object sender, EventArgs e)
        {
            this.cbDrawMethod.SelectedIndex = (int)DrawMethods.Pencil;
        }

        private void btnBeeLine_Click(object sender, EventArgs e)
        {
            this.cbDrawMethod.SelectedIndex = (int)DrawMethods.Beeline;
        }

        private void btnEaraser_Click(object sender, EventArgs e)
        {
            this.cbDrawMethod.SelectedIndex = (int)DrawMethods.Eraser;
        }
        #endregion

        private void cbLineStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildSeledPen();
        }

        #region "Cap"
        private void cbEndCap_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildSeledPen();
        }

        private void cbStartCap_SelectedIndexChanged(object sender, EventArgs e)
        {
            BuildSeledPen();
        }

        private void cbStartCap_DropDown(object sender, EventArgs e)
        {
            this.ForbidHide();
        }

        private void cbStartCap_DropDownClosed(object sender, EventArgs e)
        {
            this.RestoreHide();
        }

        private void cbEndCap_DropDown(object sender, EventArgs e)
        {
            this.ForbidHide();
        }

        private void cbEndCap_DropDownClosed(object sender, EventArgs e)
        {
            this.RestoreHide();
        }
        #endregion

        private bool _isClosed = false;
        public bool IsClosed
        {
            get { return _isClosed; }
        }
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _isClosed = true;
        }
    }
}