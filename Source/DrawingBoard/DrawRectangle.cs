using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace XDrawBoard
{
    public class DrawRectangle:DrawShapeBase
    {
        Rectangle _rectShape = Rectangle.Empty;
        protected Rectangle ShapeRect
        {
            get { return _rectShape; }
        }

        bool _bIsFill = false;
        protected bool IsFillShape
        {
            get { return _bIsFill; }
        }

        public DrawRectangle(Pen rectLine_, bool bFill_, Point ptStart_)
            :base(rectLine_, ptStart_)
        {
            _bIsFill = bFill_;
        }

        public override void MoveTo(Point ptCurrent_)
        {
            if (IsDistanceSmall(ShapeStart, ptCurrent_))
            {
                _rectShape = Rectangle.Empty;
                return;
            }

            _rectShape = BoardUtility.BuildRectangle(ShapeStart, ptCurrent_);
        }

        public override void Complete(Point ptEnd_)
        {
            base.Complete(ptEnd_);
            MoveTo(ptEnd_);
        }

        public override bool IsValidShape()
        {
            return _rectShape != Rectangle.Empty;
        }

        public override void Draw(Graphics grContext_, Bitmap btBackgound_)
        {
            if (!IsValidShape()) return;

            if (IsFillShape)
            {
                using (Brush br = new SolidBrush(ShapePen.Color))
                {
                    grContext_.FillRectangle(br, _rectShape);
                }
            }
            else
            {
                grContext_.DrawRectangle(ShapePen, _rectShape);
            }
        }
    }
}
