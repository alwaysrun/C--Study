using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace XDrawBoard
{
    public class DrawPencil : DrawMultiPoint
    {
        private Point[] _aryPoints = null;

        public DrawPencil(Pen penLine_, Point ptStart_)
            : base(penLine_, ptStart_)
        {
        }

        public override void MoveTo(Point ptCurrent_)
        {
            base.MoveTo(ptCurrent_);

            _aryPoints = null;
        }

        public override void Complete(Point ptEnd_)
        {
            base.Complete(ptEnd_);

            //_aryPoints = PointList.ToArray();
            if (!IsValidShape()) return;

            _aryPoints = new Point[PointList.Count];
            for (int i = 0; i < PointList.Count; ++i)
                _aryPoints[i] = PointList[i].Location;

            ClearPointList();
        }

        public override bool IsValidShape()
        {
            return _aryPoints != null
                || PointList.Count > 1;
        }

        public override void Draw(Graphics grContext_, Bitmap btBackgound_)
        {
            if (!IsValidShape()) return;

            if(_aryPoints == null)
            {
                _aryPoints = new Point[PointList.Count];
                for (int i = 0; i < PointList.Count; ++i)
                    _aryPoints[i] = PointList[i].Location;
            }
            grContext_.DrawCurve(ShapePen, _aryPoints);
        }
    }
}
