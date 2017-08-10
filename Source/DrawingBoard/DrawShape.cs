using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace XDrawBoard
{
    public class DrawShapeBase
    {
        protected const int MinDistanceEachStep = 2;

        private Point _ptStart = Point.Empty;
        /// <summary>
        /// 图形起点
        /// </summary>
        public Point ShapeStart
        {
            get { return _ptStart; }
        }

        private Point _ptEnd = Point.Empty;
        /// <summary>
        /// 图形终点
        /// </summary>
        public Point ShapeEnd
        {
            get { return _ptEnd; }
        }

        private Pen _penLine;
        protected Pen ShapePen
        {
            get { return _penLine; }
        }

        public DrawShapeBase(Pen penLine_, Point ptStart_)
        {
            _penLine = penLine_;
            _ptStart = ptStart_;
        }

        public virtual void MoveTo(Point ptCurrent_)
        {
            _ptEnd = ptCurrent_;
        }

        public virtual void Complete(Point ptEnd_)
        {
            _ptEnd = ptEnd_;
        }

        public virtual void Draw(Graphics grContext_, Bitmap btBackgound_)
        {
            grContext_.DrawLine(_penLine, _ptStart, _ptEnd);
        }

        public virtual bool IsValidShape()
        {
            return !IsDistanceSmall(_ptStart, _ptEnd);
        }


        protected bool IsDistanceSmall(Point ptOne_, Point ptTwo_)
        {
            return Math.Abs(ptOne_.X - ptTwo_.X) < MinDistanceEachStep
                && Math.Abs(ptOne_.Y - ptTwo_.Y) < MinDistanceEachStep;
        }
    }
}
