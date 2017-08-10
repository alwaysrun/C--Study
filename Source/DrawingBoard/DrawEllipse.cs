using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace XDrawBoard
{
    public class DrawEllipse : DrawRectangle
    {
        public DrawEllipse(Pen rectLine_, bool bFill_, Point ptStart_)
            : base(rectLine_, bFill_, ptStart_)
        {
        }

        public override void Draw(Graphics grContext_, Bitmap btBackgound_)
        {
            if (!IsValidShape()) return;

            if (IsFillShape)
            {
                using (Brush br = new SolidBrush(ShapePen.Color))
                {
                    grContext_.FillEllipse(br, ShapeRect);
                }
            }
            else
            {
                grContext_.DrawEllipse(ShapePen, ShapeRect);
            }
        }
    }
}
