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

            ControlPoints.AddRange(CalculateControlPoints(Points));
        }

        public override GeometryElement CloneElement()
        {
            return new CRSpline(Style, null, [.. Points]);
        }

        public override void Draw()
        {
            if (ViewportLayer is null) return;

            _spline.SetCatmullRomSpline(ViewportLayer, Points, ControlPoints);
        }

        public static IEnumerable<Vector2> CalculateControlPoints(List<Vector2> points)
        {
            for (int i = 1; i < points.Count - 1; i++)
            {
                yield return points[i] + ((points[i - 1] - points[i + 1]) / 6);
                yield return points[i] - ((points[i - 1] - points[i + 1]) / 6);
            }
        }
    }
}
