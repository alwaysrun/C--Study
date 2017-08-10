using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace XDrawBoard
{
    public class DrawLine:DrawShapeBase
    {
        public DrawLine(Pen penLine_, Point ptStart_)
            : base(penLine_, ptStart_)
        {
        }
    }
}
