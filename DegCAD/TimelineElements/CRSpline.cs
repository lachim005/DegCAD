using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DegCAD.TimelineElements
{
    public class CRSpline : GeometryElement
    {
        private readonly Path _spline;

        public List<Vector2> Points;
        public List<Vector2> ControlPoints = [];

        public CRSpline(Style s, ViewportLayer? vpl, params Vector2[] points)
        {
            if (points.Length < 4)
            {
                throw new ArgumentException("Příliš malý počet bodů");
            }

            Points = new(points);
            CalculatePoints();

            _spline = new();
            AddShape(_spline);

            Style = s;

            if (vpl is not null)
            {
                AddToViewportLayer(vpl);
            }
        }

        public void CalculatePoints()
        {
            ControlPoints.Clear();

            for (int i = 1; i < Points.Count - 1; i++)
            {
                ControlPoints.Add(Points[i] + ((Points[i - 1] - Points[i + 1]) / 6));
                ControlPoints.Add(Points[i] - ((Points[i - 1] - Points[i + 1]) / 6));
            }
        }

        public override GeometryElement CloneElement()
        {
            return new CRSpline(Style, null, [.. Points]);
        }

        public override void Draw()
        {
            if (ViewportLayer is null) return;

            var vp = ViewportLayer.Viewport;

            StringBuilder sb = new();
            var startingPoint = vp.CanvasToScreen(Points[1]);
            sb.Append("M " + startingPoint.X.ToString(CultureInfo.InvariantCulture) + " " + startingPoint.Y.ToString(CultureInfo.InvariantCulture));

            int cpc = 1;
            for (int i = 1; i < Points.Count - 2; i++)
            {
                sb.Append(" C ");
                var cp1 = vp.CanvasToScreen(ControlPoints[cpc++]);
                sb.Append(cp1.X.ToString(CultureInfo.InvariantCulture) + " " + cp1.Y.ToString(CultureInfo.InvariantCulture));
                sb.Append(", ");
                var cp2 = vp.CanvasToScreen(ControlPoints[cpc++]);
                sb.Append(cp2.X.ToString(CultureInfo.InvariantCulture) + " " + cp2.Y.ToString(CultureInfo.InvariantCulture));
                sb.Append(", ");
                var pt = vp.CanvasToScreen(Points[i + 1]);
                sb.Append(pt.X.ToString(CultureInfo.InvariantCulture) + " " + pt.Y.ToString(CultureInfo.InvariantCulture));
            }

            _spline.Data = Geometry.Parse(sb.ToString());
        }
    }
}
