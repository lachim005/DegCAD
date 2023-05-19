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
        public ViewPort Viewport { get; init; }

        public ViewportLayer(ViewPort viewport)
        {
            Canvas = new() { ClipToBounds = true };
            Viewport = viewport;
        }

        #region Old GD
        public ViewportLayer()
        {
            Canvas = new();
            Viewport = new();
        }

        public void Clear()
        {
            
        }

        public void DrawLine(ParametricLine2 line, double from, double to, Style s)
        {
        }

        public void DrawLine(Vector2 p1, Vector2 p2, Style s)
        {
        }

        public void DrawPlane(bool plane)
        {
        }

        public void DrawPointCross(Vector2 pt, Style style, double size = 0.2)
        {
        }

        public Vector2 DrawString(string text, Vector2 coords, int fontSize, Style s)
        {
            return (0, 0);
        }

        public void DrawCircle(Vector2 middle, Vector2 point, Style s)
        {
        }

        public void DrawCircle(Circle2 circle, Style s)
        {
        }

        public void DrawArc(Vector2 middle, Vector2 point, double angle, double endAngle, Style s)
        {
        }

        public void DrawArc(Circle2 circle, double angle, double endAngle, Style s)
        {
        }
        #endregion
    }
}
