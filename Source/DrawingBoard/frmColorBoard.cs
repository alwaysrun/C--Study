using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace XDrawBoard
{
    public partial class FrmColorBoard : Form
    {
        private DrawBoardInfo _boardInfo = new DrawBoardInfo();
        public DrawBoardInfo BoardInfo
        {
            get { return _boardInfo; }
            set { _boardInfo = value; }
        }

        FrmMain _frmParent;
        bool _bIsModified = false;
        //public bool IsModified
        //{
        //    get { return _bIsModified; }
        //    set { _bIsModified = value; }
        //}

        public FrmColorBoard(FrmMain frmParent_)
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.FormBorderStyle = FormBorderStyle.None;

            _frmParent = frmParent_;

            this.WindowState = FormWindowState.Maximized;
            this.Cursor = _boardInfo.LoadCursor();
        }

        private void frmLockScreen_Paint(object sender, PaintEventArgs e)
        {
            Point pTL = new Point(0, 0);
            Point pTR = new Point(this.ClientRectangle.Width-1, 0);
            Point pBL = new Point(0, this.ClientRectangle.Height-1);
            Point pBR = new Point(this.ClientRectangle.Width-1, this.ClientRectangle.Height-1);
            
            int lineLength =20;
            Graphics g = e.Graphics;
            using (Pen pLine=new Pen(Color.FromArgb(200,Color.Black),5))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                pLine.Alignment = PenAlignment.Inset;
                pLine.StartCap = LineCap.Round;
                pLine.EndCap = LineCap.ArrowAnchor;

                /*左上角两条线*/
                g.DrawLine(pLine, pTL, new Point(pTL.X + lineLength, pTL.Y));
                g.DrawLine(pLine, pTL, new Point(pTL.X, pTL.Y + lineLength));
                /*右上角两条线*/
                g.DrawLine(pLine, pTR, new Point(pTR.X - lineLength, pTR.Y));
                g.DrawLine(pLine, pTR, new Point(pTR.X, pTR.Y + lineLength));
                /*左下角两条线*/
                g.DrawLine(pLine, pBL, new Point(pBL.X, pBL.Y - lineLength));
                g.DrawLine(pLine, pBL, new Point(pBL.X + lineLength, pBL.Y));
                /*右下角两条线*/
                g.DrawLine(pLine, pBR, new Point(pBR.X, pBR.Y - lineLength));
                g.DrawLine(pLine, pBR, new Point(pBR.X - lineLength, pBR.Y));
            }
            //this.TopMost = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            _boardInfo.DrawShapes(e.Graphics);
        }

        public void SetDrawMethod(DrawMethods euMethod_)
        {
            if (_boardInfo.PenMethod == euMethod_) return;
            _boardInfo.PenMethod = euMethod_;
            this.Cursor = _boardInfo.LoadCursor();
        }

        public void SetDrawBoard(bool bIsSceen_)
        {
            //if (_boardInfo.IsScreenBoard == bIsSceen_) return;

            _boardInfo.IsScreenBoard = bIsSceen_;
            this.BackgroundImage = _boardInfo.ColorBoard;

            this.Invalidate();
        }

        public void SetBoardColor(Color colorBoard_)
        {
            if(_boardInfo.SetBoardColor(colorBoard_))
            {
                this.BackgroundImage = _boardInfo.ColorBoard;
                this.Invalidate();
            }
        }

        public void ResetTryErase()
        {
            _boardInfo.Clear();
            _boardInfo.ShapeDrawStart(new Point(1, 1));
            _boardInfo.ShapeDrawEnd(new Point(this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1));
        }

        public void SetBoardScreen()
        {
            this.Visible = false;
            _boardInfo.SetBoardScreen();
            this.Visible = true;

            this.BackgroundImage = _boardInfo.ColorBoard;
            this.Invalidate();
        }

        private void frmLockScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode==Keys.Escape)
            {
                _boardInfo.ShapeHistory.Clear();
                _boardInfo.IsScreenBoard = false;
                this.Close();
            }
            else if(e.KeyCode==Keys.S && e.Modifiers==Keys.Control)
            {
                _frmParent.SaveColorboard();
            }
        }

        private void frmLockScreen_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            _boardInfo.ShapeDrawStart(e.Location);
        }

        private void frmLockScreen_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            _boardInfo.ShapeDrawMoved(e.Location);

            //this._boardInfo.EndPoint = e.Location;
            //if (_boardInfo.PenMethod == DrawMethods.TwowayArrow || _boardInfo.PenMethod == DrawMethods.Pencil || _boardInfo.PenMethod == DrawMethods.Heighlighter)
            //{
            //    int count = _boardInfo.TempPointPath.Points.Count;

            //    if (count > 0)
            //    {
            //        if (Math.Abs(_boardInfo.TempPointPath.Points[count - 1].X - e.Location.X) < 3 &&
            //            Math.Abs(_boardInfo.TempPointPath.Points[count - 1].Y - e.Location.Y) < 3) return;
            //    }
            //    _boardInfo.TempPointPath.Points.Add(e.Location);
            //}
            this.Invalidate();
        }

        private void frmLockScreen_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            if (_boardInfo.ShapeDrawEnd(e.Location))
                _bIsModified = true;
        }

        private void FrmColorBoard_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Visible)
            {
                _frmParent.CloseColorBoard();
                e.Cancel = !_frmParent.IsClosed;
            }
            else
            {
                TryHideSelf();
            }
        }

        public void TryHideSelf()
        {
            if (_bIsModified)
            {
                if (DialogResult.Yes == MessageBox.Show("画板还未保存，是否先保存画板？", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    _frmParent.SaveColorboard();
                }
            }
            this.Visible = false;
        }

        public void SetHasSaved()
        {
            _bIsModified = false;
        }
    }
}