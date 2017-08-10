using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace XDrawBoard
{
    public enum DrawMethods
    {
        Pencil,
        Beeline,
        Rectangle,
        Ellipse,
        Eraser,
        FillRect,
        FillEllipse,
    }

    public class DrawBoardInfo
    {

        #region "Var"
        #region "board&screen"
        private bool _isScreenBoard = false;
        public bool IsScreenBoard
        {
            get { return _isScreenBoard; }
            set { _isScreenBoard = value; }
        }

        private Color _colorBoard;
        private Bitmap _bmpColorBoard;
        public Bitmap ColorBoard
        {
            get
            {
                return _isScreenBoard ? _bmpScreenShot : _bmpColorBoard;
            }
        }

        private Bitmap _bmpScreenShot;
        public Bitmap ScreenShot
        {
            get { return _bmpScreenShot; }
        }
        #endregion

        private List<DrawShapeBase> _lstShapes = new List<DrawShapeBase>();
        public List<DrawShapeBase> ShapeHistory
        {
            get { return _lstShapes; }
            set { _lstShapes = value; }
        }

        List<DrawShapeBase> _lstUndoShapes = new List<DrawShapeBase>();
        public List<DrawShapeBase> UndoShapes
        {
            get { return _lstUndoShapes; }
        }

        private DrawMethods _euMothod;
        public DrawMethods PenMethod
        {
            get { return _euMothod; }
            set { _euMothod = value; }
        }

        private Pen _penDraw;
        public Pen PenDrawing
        {
            get { return _penDraw; }
            set { _penDraw = value; }
        }

        DrawShapeBase _curShape = null;
        public DrawShapeBase CurrenShape
        {
            get { return _curShape; }
        }
        #endregion

        public DrawBoardInfo()
        {
        }

        public void Init(bool bIsColorBoard_, Color boardColor_)
        {
            _penDraw = Pens.Black;
            _euMothod = DrawMethods.Pencil;
            _bmpScreenShot = BoardUtility.PickScreenToBitmap();

            IsScreenBoard = bIsColorBoard_;
            _colorBoard = boardColor_;
            _bmpColorBoard = BoardUtility.PickScreenToBitmap(boardColor_);
        }

        public void ResetScreenShot()
        {
            _bmpScreenShot = BoardUtility.PickScreenToBitmap();
        }

        public void Clear()
        {
            _lstShapes.Clear();
            _lstUndoShapes.Clear();
            _curShape = null;
        }

        public void Redo()
        {
            if (_lstUndoShapes.Count == 0) return;

            _lstShapes.Add(_lstUndoShapes[_lstUndoShapes.Count-1]);
            _lstUndoShapes.RemoveAt(_lstUndoShapes.Count - 1);
        }

        public void Undo()
        {
            if (_lstShapes.Count == 0) return;

            _lstUndoShapes.Add(_lstShapes[_lstShapes.Count - 1]);
            _lstShapes.RemoveAt(_lstShapes.Count - 1);
        }

        public bool SetBoardColor(Color colorBoard_)
        {
            if (_colorBoard == colorBoard_) return false;

            _colorBoard = colorBoard_;
            _bmpColorBoard = BoardUtility.PickScreenToBitmap(colorBoard_);
            return true;
        }

        public void SetBoardScreen()
        {
            _bmpScreenShot = BoardUtility.PickScreenToBitmap();
        }

        public void ShapeDrawStart(Point ptLocation_)
        {
            switch(_euMothod)
            {
                case DrawMethods.Pencil:
                    _curShape = new DrawPencil(PenDrawing, ptLocation_);
                    break;

                case DrawMethods.Beeline:
                    _curShape = new DrawLine(PenDrawing, ptLocation_);
                    break;

                case DrawMethods.Rectangle:
                    _curShape = new DrawRectangle(PenDrawing, false, ptLocation_);
                    break;

                case DrawMethods.Ellipse:
                    _curShape = new DrawEllipse(PenDrawing, false, ptLocation_);
                    break;

                case DrawMethods.FillRect:
                    _curShape = new DrawRectangle(PenDrawing, true, ptLocation_);
                    break;

                case DrawMethods.FillEllipse:
                    _curShape = new DrawEllipse(PenDrawing, true, ptLocation_);
                    break;

                case DrawMethods.Eraser:
                    _curShape = new DrawEraser(ptLocation_);
                    break;
            }
        }

        public void ShapeDrawMoved(Point ptLoc_)
        {
            if(_curShape != null)
            {
                _curShape.MoveTo(ptLoc_);
            }
        }

        public bool ShapeDrawEnd(Point ptEnd_)
        {
            if(_curShape == null) return false;

            _curShape.Complete(ptEnd_);
            if (_curShape.IsValidShape())
            {
                _lstShapes.Add(_curShape);
                _lstUndoShapes.Clear();
                return true;
            }

            _curShape = null;
            return true;
        }

        public void DrawShapes(Graphics grContext_)
        {
            if (_bmpScreenShot == null) return;

            Graphics gShow = grContext_;
            {
                gShow.SmoothingMode = SmoothingMode.AntiAlias;
                
                foreach (DrawShapeBase shapBase in _lstShapes)
                {
                    shapBase.Draw(gShow, ColorBoard);
                }

                if (_curShape != null)
                    _curShape.Draw(gShow, ColorBoard);
            }
        }

        #region 加载光标
        public Cursor LoadCursor()
        {
            Cursor cursor;
            switch (_euMothod)
            {
                case DrawMethods.Beeline:
                    cursor = BoardUtility.TryLoadCursor("DrawLine.cur", Cursors.Cross);
                    break;
                case DrawMethods.Rectangle:
                case DrawMethods.FillRect:
                    cursor = BoardUtility.TryLoadCursor("DrawRectangle.cur", Cursors.Cross);
                    break;
                case DrawMethods.Ellipse:
                case DrawMethods.FillEllipse:
                    cursor = BoardUtility.TryLoadCursor("DrawEllipse.cur", Cursors.Cross);
                    break;
                case DrawMethods.Eraser:
                    cursor = BoardUtility.TryLoadCursor("DrawEaraser.cur", Cursors.NoMove2D);
                    break;

                case DrawMethods.Pencil:
                default:
                    cursor = BoardUtility.TryLoadCursor("DrawPencil.cur", Cursors.UpArrow);
                    break;
            }
            return cursor;
        }
        #endregion
    }
}
