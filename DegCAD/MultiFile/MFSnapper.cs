using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MultiFile
{
    public class MFSnapper
    {
        public MFPage Page { get; set; }

        public double SnapThreshold { get; set; } = 5;
        public double? SnapToGrid { get; set; }

        public MFSnapper(MFPage page)
        {
            Page = page;
        }

        public double? TrySnapX(double x)
        {
            if (Math.Abs(x) <= SnapThreshold)
            {
                return 0;
            }
            if (Math.Abs(x - Page.PaperWidth) <= SnapThreshold)
            {
                return Page.PaperWidth;
            }

            return null;
        }

        public double? TrySnapY(double y)
        {
            if (Math.Abs(y) <= SnapThreshold)
            {
                return 0;
            }
            if (Math.Abs(y - Page.PaperHeight) <= SnapThreshold)
            {
                return Page.PaperHeight;
            }

            return null;
        }

        public double SnapX(double x) => TrySnapX(x) ?? x;
        public double SnapY(double y) => TrySnapY(y) ?? y;
    }
}
