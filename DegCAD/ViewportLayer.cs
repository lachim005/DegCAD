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
    }
}
