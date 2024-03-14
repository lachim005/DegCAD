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

        public double SnapThreshold { get; set; } = 4;
        public bool SnapToGrid { get; set; }
        public double GridWidth { get; set; } = 5;
        public double GridHeight { get; set; } = 3;

        public MFSnapper(MFPage page)
        {
            Page = page;
        }

        public double? TrySnapX(double x)
        {
            double closestX = x;
            double closestDistance = SnapThreshold + 1;

            void TestVal(double val)
            {
                double dist = Math.Abs(x - val);
                if (dist <= closestDistance)
                {
                    closestX = val;
                    closestDistance = dist;
                }
            }

            TestVal(0);
            TestVal(Page.PaperWidth);

            foreach (var item in Page.Items)
            {
                if (item.IsSelected) continue;

                TestVal(item.CX);
                TestVal(item.CX + item.CWidth);
            }

            if (closestDistance < SnapThreshold)
            {
                return closestX;
            }
            if (!SnapToGrid) return null;

            return Math.Round(x / GridWidth) * GridWidth;
        }

        public double? TrySnapY(double y)
        {
            double closestY = y;
            double closestDistance = SnapThreshold + 1;

            void TestVal(double val)
            {
                double dist = Math.Abs(y - val);
                if (dist <= closestDistance)
                {
                    closestY = val;
                    closestDistance = dist;
                }
            }

            TestVal(0);
            TestVal(Page.PaperHeight);

            foreach (var item in Page.Items)
            {
                if (item.IsSelected) continue;

                TestVal(item.CY);
                TestVal(item.CY + item.CHeight);
            }

            if (closestDistance < SnapThreshold)
            {
                return closestY;
            }
            if (!SnapToGrid) return null;

            return Math.Round(y / GridHeight) * GridHeight;
        }

        public double SnapX(double x) => TrySnapX(x) ?? x;
        public double SnapY(double y) => TrySnapY(y) ?? y;
    }
}
