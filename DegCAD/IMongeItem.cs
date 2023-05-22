using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DegCAD
{
    public interface IMongeItem
    {
        void Draw(ViewportLayer gd);
        public Vector2[] SnapablePoints { get; }
        public ParametricLine2[] SnapableLines { get; }
        public Circle2[] SnapableCircles { get; }
        public Style Style { get; set; }
        public void AddToViewportLayer(ViewportLayer vpl);
        public void SetVisibility(Visibility visibility);
        public IMongeItem Clone();
    }
}
