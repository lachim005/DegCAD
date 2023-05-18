using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD.MongeItems
{
    /// <summary>
    /// Static class that can display the x axis and the zero mark
    /// </summary>
    public class Axis : IMongeItem
    {
        static ParametricLine2 axis = new((0, 0), (1, 0));

        public Vector2[] SnapablePoints { get; } = new Vector2[1] { (0, 0) };

        public ParametricLine2[] SnapableLines { get; } = new ParametricLine2[1] { new((0, 0), (1, 0)) };

        public Circle2[] SnapableCircles { get; } = new Circle2[0];

        public Style Style { get; } = Style.Default;

        public void AddToViewportLayer(ViewportLayer vpl)
        {
            
        }

        public void Draw(ViewportLayer gd)
        {
            Draw(gd, Style);
        }
        public void Draw(ViewportLayer gd, Style s)
        {
            //Axis
            gd.DrawLine(axis, double.NegativeInfinity, double.PositiveInfinity, s);
            //Zero mark
            gd.DrawLine((0, -.2), (0, .2), s);
        }
    }
}
