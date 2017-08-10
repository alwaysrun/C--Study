using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace XDrawBoard
{
    public enum RelativeLocate
    {
        Diff,
        SameX,
        SameY,
    }

    public class PointLocate
    {
        public Point Location;
        public RelativeLocate Relative;
    }

    public class DrawMultiPoint: DrawShapeBase
    {
        List<PointLocate> _lstPoints = new List<PointLocate>();
        protected List<PointLocate> PointList
        {
            get {return _lstPoints;}
        }

        public DrawMultiPoint(Pen penLine_, Point ptStart_)
            : base(penLine_, ptStart_)
        {
            _lstPoints.Add(new PointLocate()
            {
                Location = ptStart_,
                Relative = RelativeLocate.Diff
            });
        }

        public override void MoveTo(Point ptCurrent_)
        {
            if (_lstPoints.Count == 0)
            {
                _lstPoints.Add(new PointLocate()
                {
                    Location = ptCurrent_,
                    Relative = RelativeLocate.Diff
                });
            }

            PointLocate lastPoint = _lstPoints[_lstPoints.Count - 1];
            if (Math.Abs(lastPoint.Location.X - ptCurrent_.X) < MinDistanceEachStep)
            {
                if (Math.Abs(lastPoint.Location.Y - ptCurrent_.Y) < MinDistanceEachStep)
                {
                    return;
                }

                // Y is different
                if (lastPoint.Relative == RelativeLocate.SameX)
                    lastPoint.Location.Y = ptCurrent_.Y;
                else
                    _lstPoints.Add(new PointLocate()
                    {
                        Location = ptCurrent_,
                        Relative = RelativeLocate.SameX
                    });
            }
            else // X is different
            {
                if (Math.Abs(lastPoint.Location.Y - ptCurrent_.Y) < MinDistanceEachStep)
                { // Y is same
                    if (lastPoint.Relative == RelativeLocate.SameY)
                        lastPoint.Location.X = ptCurrent_.X;
                    else
                        _lstPoints.Add(new PointLocate()
                        {
                            Location = ptCurrent_,
                            Relative = RelativeLocate.SameY
                        });
                }
                else
                {
                    _lstPoints.Add(new PointLocate()
                    {
                        Location = ptCurrent_,
                        Relative = RelativeLocate.Diff
                    });
                }
            }
        }

        public override void Complete(Point ptEnd_)
        {
            base.Complete(ptEnd_);
            MoveTo(ptEnd_);
        }

        public override bool IsValidShape()
        {
            return _lstPoints.Count > 1;
        }

        protected void ClearPointList()
        {
            _lstPoints.Clear();
            _lstPoints.TrimExcess();
        }

    } // class
}
