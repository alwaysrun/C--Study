using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace XDrawBoard
{
    public class DrawEraser : DrawMultiPoint
    {
        Rectangle[] _aryRects;

        public DrawEraser(Point ptStart_)
            : base(null, ptStart_)
        {
        }

        public override void MoveTo(Point ptCurrent_)
        {
            base.MoveTo(ptCurrent_);

            _aryRects = null;
        }

        public override void Complete(Point ptEnd_)
        {
            base.Complete(ptEnd_);

            if (IsValidShape())
                BuildRectangels();
        }

        public override bool IsValidShape()
        {
            return _aryRects != null
                || PointList.Count > 1;
        }

        Rectangle BuildRect(Point ptStart_, Point ptEnd_)
        {
            const int EraserWidth = 6;

            BoardUtility.AdjustRectangle(ref ptStart_,ref ptEnd_);
            if(ptStart_.X>EraserWidth)
                ptStart_.X -= EraserWidth;
            else
                ptStart_.X = 0;
            if(ptStart_.Y>EraserWidth)
            ptStart_.Y -= EraserWidth;
            else
                ptStart_.Y -= EraserWidth;

            return new Rectangle(ptStart_.X, ptStart_.Y, (ptEnd_.X - ptStart_.X + EraserWidth), (ptEnd_.Y - ptStart_.Y + EraserWidth));
        }

        void BuildRectangels()
        {
            _aryRects = new Rectangle[PointList.Count - 1];
            for(int i=0 ; i<PointList.Count-1 ; ++i)
            {
                _aryRects[i] = BuildRect(PointList[i].Location, PointList[i + 1].Location);
            }
        }

        public override void Draw(Graphics grContext_, Bitmap btBackgound_)
        {
            if(!IsValidShape()) return;

            if(_aryRects == null)
                BuildRectangels();

            using (TextureBrush tBrush = new TextureBrush(btBackgound_))
            {
                grContext_.FillRectangles(tBrush, _aryRects);
            }
        }
    } // class
}
