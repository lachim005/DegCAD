using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DegCAD
{
    /// <summary>
    /// A class that encapsulates the drawing to the viewport
    /// </summary>
    public class ViewportLayer
    {
        public Canvas Canvas { get; init; }

        public ViewportLayer()
        {
            Canvas = new();
        }

        #region Old GD
        public void Clear()
        {
            
        }

        public void DrawLine(ParametricLine2 line, double from, double to, Style s)
        {
            throw new NotImplementedException();
        }

        public void DrawLine(Vector2 p1, Vector2 p2, Style s)
        {
            throw new NotImplementedException();
        }

        public void DrawPlane(bool plane)
        {
            throw new NotImplementedException();
        }

        public void DrawPointCross(Vector2 pt, Style style, double size = 0.2)
        {
            throw new NotImplementedException();
        }

        public Vector2 DrawString(string text, Vector2 coords, int fontSize, Style s)
        {
            throw new NotImplementedException();
        }

        public void DrawCircle(Vector2 middle, Vector2 point, Style s)
        {
            throw new NotImplementedException();
        }

        public void DrawCircle(Circle2 circle, Style s)
        {
            throw new NotImplementedException();
        }

        public void DrawArc(Vector2 middle, Vector2 point, double angle, double endAngle, Style s)
        {
            throw new NotImplementedException();
        }

        public void DrawArc(Circle2 circle, double angle, double endAngle, Style s)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
