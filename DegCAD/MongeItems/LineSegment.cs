using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MongeItems
{
    public class LineSegment : IMongeItem
    {
        public Vector2 P1 { get; set; }
        public Vector2 P2 { get; set; }

        public Vector2[] SnapablePoints { get; } = new Vector2[0];

        public void Draw(GeometryDrawer gd)
        {
            gd.DrawLine(P1, P2, Style.Default);
        }
    }
}
